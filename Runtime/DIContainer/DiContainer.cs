using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UJect.Exceptions;
using UJect.Factories;
using UJect.Injection;
using UJect.Resolvers;
using Uject.Utilities;
using UJect.Utilities;
using UnityEngine;
using static UJect.Assertions.RuntimeAssert;

namespace UJect
{
    public sealed class DiContainer : IDisposable
    {
        private readonly Dictionary<InjectionKey, object>    resolvedInstances = new Dictionary<InjectionKey, object>();
        private readonly Dictionary<InjectionKey, IResolver> dependencyResolvers = new Dictionary<InjectionKey, IResolver>();
        private readonly DependencyTree                      dependencyTree      = new DependencyTree();
        private readonly DiContainer                         parentContainer;
        private readonly string                              containerName;

        private DiPhase phase;
        private bool    isDisposed;
        
        private DiPhase Phase
        {
            get => phase;
            set => phase = value;
        }
        
        /// <summary>
        /// Create a new DiContainer with an empty name
        /// </summary>
        public DiContainer() : this(null, null){}
        
        /// <summary>
        /// Create a new DiContainer with the provided name
        /// </summary>
        /// <param name="containerName"></param>
        public DiContainer(string containerName) : this(null, containerName){}

        /// <summary>
        /// Create a new DiContainer with the provided name and parent container
        /// </summary>
        /// <param name="parentContainer"></param>
        /// <param name="containerName"></param>
        internal DiContainer(DiContainer parentContainer = null, string containerName = null)
        {
            this.parentContainer = parentContainer;
            this.containerName   = containerName;
            Phase                = DiPhase.Bind;
            BindInstance(this);
        }
        
        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            foreach (var resolvedInstance in resolvedInstances.Values)
            {
                if (!LifetimeCheck.IsNullOrDestroyed(resolvedInstance) && (resolvedInstance is IDisposable disposable))
                {
                    disposable.Dispose();
                }
            }

            foreach (var dependencyResolver in dependencyResolvers.Values)
            {
                if (!LifetimeCheck.IsNullOrDestroyed(dependencyResolver) && (dependencyResolver is IDisposable disposable))
                {
                    disposable.Dispose();
                }
            }
            
            resolvedInstances.Clear();
            dependencyResolvers.Clear();
        }

        [LibraryEntryPoint]
        [NotNull]
        public DiContainer CreateChildContainer(string childContainerName = null)
        {
            return new DiContainer(this, childContainerName);
        }

        public override string ToString()
        {
            return $"{nameof(DiContainer)} \"{containerName}\"";
        }

        /// <summary>
        ///     Get a bound dependency. Throws an exception if the dependency isn't found.
        /// </summary>
        /// <param name="customId"></param>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [LibraryEntryPoint]
        [NotNull]
        public TInterface Get<TInterface>(string customId = null) where TInterface : class
        {
            AssertIsFalse(isDisposed, "You should not try to retrieve bindings from a disposed container!");

            var key = new InjectionKey(typeof(TInterface), customId);
            if (TryGetDependencyInternal<TInterface>(key, out var dependency))
            {
                return dependency;
            }

            throw new ArgumentException($"No dependency of type {typeof(TInterface)} found{(customId != null ? $" with customId \"{customId}\"" : "")}");
        }

        /// <summary>
        ///     Try to get a bound dependency based on the given interface
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="dependency">The returned dependency</param>
        /// <returns>True if dependency is bound, false otherwise</returns>
        [LibraryEntryPoint]
        public bool TryGet<TInterface>([NotNull] out TInterface dependency) where TInterface : class
        {
            return TryGet(null, out dependency);
        }

        /// <summary>
        ///     Try to get a bound dependency based on the given key
        /// </summary>
        /// <param name="customId"></param>
        /// <param name="dependency">The returned dependency</param>
        /// <returns>True if dependency is bound, false otherwise</returns>
        [LibraryEntryPoint]
        public bool TryGet<TType>(string customId, [NotNull]  out TType dependency)
        {
            var key = new InjectionKey(typeof(TType), customId);
            return TryGetDependencyInternal(key, out dependency);
        }

        internal bool TryGetDependencyInternal<TType>(InjectionKey key, out TType dependency)
        {
            AssertIsFalse(isDisposed, "You should not try to retrieve bindings from a disposed container!");
            TryResolveAll();

            if (resolvedInstances.TryGetValue(key, out var resolvedDependency))
            {
                dependency = (TType)resolvedDependency;
                AssertObjectIsAlive(dependency, $"Null dependency detected in: {this}. It should have been unregistered!");
                return true;
            }

            if (parentContainer != null && parentContainer.TryGetDependencyInternal<TType>(key, out var parentDependency))
            {
                dependency = parentDependency;
                AssertObjectIsAlive(dependency, $"Null dependency detected in parent: {parentContainer}. It should have been unregistered!");
                return true;
            }

            dependency = default;
            return false;
        }

        [LibraryEntryPoint]
        public void InjectInto(object obj)
        {
            AssertObjectIsAlive(obj, "Can't inject into null Object!");
            var objType = obj.GetType();
            InjectorCache.GetOrCreateInjector(objType).InjectFields(obj, this);
        }

        private enum DiPhase : byte
        {
            Bind     = 0,
            Resolved = 1
        }

        #region Public Binding Interface

        [LibraryEntryPoint]
        public IDiBinder<TInterface> Bind<TInterface>()
        {
            AssertIsFalse(isDisposed, "You should not try to bind to a disposed container!");

            return new DiBinder<TInterface>(this);
        }

        [LibraryEntryPoint]
        public DiContainer BindInstance<TClass>(TClass instance, string customId = null) where TClass : class
        {
            AssertIsFalse(isDisposed, "You should not try to bind to a disposed container!");
            AssertIsFalse(typeof(TClass).IsInterface, "You should not try to bind an instance of an interface!");
            InstallBindingInternal<TClass, TClass>(customId, new InstanceResolver<TClass>(instance));
            return this;
        }

        [LibraryEntryPoint]
        public bool Unbind<TType>(string customId = null)
        {
            var key = new InjectionKey(typeof(TType), customId);
            if (!dependencyResolvers.ContainsKey(key))
            {
                return false;
            }

            dependencyResolvers.Remove(key);
            resolvedInstances.Remove(key);
            return true;

        }

        #endregion Public Binding Interface

        #region Internal Binding Methods

        internal void InstallBindingInternal<TFrom, TTo>(string customId, IResolver<TTo> dependencyResolver) where TTo : TFrom
        {
            var fromKey = new InjectionKey(typeof(TFrom), customId);
            var toKey = new InjectionKey(typeof(TTo));
            InstallBindingInternal(fromKey, toKey, dependencyResolver);
        }

        internal void InstallFactoryBinding<TFrom, TTo>(string customId, IInstanceFactory<TTo> factory)
        {
            var fromKey = new InjectionKey(typeof(TFrom), customId);
            var toKey = new InjectionKey(typeof(TTo));

            var factoryIntKey = new InjectionKey(typeof(IInstanceFactory<TTo>), customId);
            var factoryKey = new InjectionKey(factory.GetType());

            //Bind the interface to the concrete implementation
            InstallBindingInternal(fromKey, toKey, new ExternalFactoryResolver<TTo>(factory, this));
            //Bind the factory interface to the factory implementation
            InstallBindingInternal(factoryIntKey, factoryKey, new InstanceResolver<IInstanceFactory<TTo>>(factory));
            //Add a dependency on the factory interface to the interface. This will ensure the factory's dependencies are ready before the factory is used
            AddDependencies(fromKey, factoryIntKey);
        }

        private void InstallBindingInternal(InjectionKey fromKey, InjectionKey toKey, IResolver dependencyResolver)
        {
            AddDependencies(fromKey, toKey);

            if (dependencyResolvers.ContainsKey(fromKey))
            {
                throw new DuplicateBindingException(fromKey);
            }

            dependencyResolvers.Add(fromKey, dependencyResolver);

            Phase = DiPhase.Bind;
        }

        /// <summary>
        /// Mark depends as dependent on dependency
        /// </summary>
        /// <param name="depends"></param>
        /// <param name="dependency"></param>
        /// <exception cref="CyclicDependencyException"></exception>
        private void AddDependencies(InjectionKey depends, InjectionKey dependency)
        {
            dependencyTree.AddDependency(depends, dependency);

            if (dependencyTree.HasCycle(out var onDependency))
            {
                throw new CyclicDependencyException($"Adding dependency binding for {depends} to {dependency} resulted in a dependency cycle!\n{onDependency}");
            }
        }
        
        #endregion Internal Binding Methods

        #region Resolving Instances

        /// <summary>
        ///     Resolve all dependencies, if required. You shouldn't have to call this, it should happen automatically when attempting to retrieve a dependency.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        [LibraryEntryPoint]
        public void TryResolveAll()
        {
            if (Phase < DiPhase.Resolved)
            {
                Phase = DiPhase.Resolved;
                if (dependencyTree.HasCycle(out _))
                {
                    throw new InvalidOperationException("Cycle detected in dependency tree!");
                }

                foreach (var injectionKey in dependencyTree.Sorted())
                {
                    if (resolvedInstances.ContainsKey(injectionKey))
                    {
                        //Already resolved. Skip
                        continue;
                    }

                    if (dependencyResolvers.TryGetValue(injectionKey, out var resolver))
                    {
                        ResolveInstance(injectionKey, resolver);
                    }
                }
            }
        }

        /// <summary>
        ///     Resolve a specific dependency instance. The instance will have its dependencies injected
        /// </summary>
        /// <param name="injectionKey"></param>
        /// <param name="resolver"></param>
        private void ResolveInstance(InjectionKey injectionKey, IResolver resolver)
        {
            var instance = resolver.Resolve();
            InjectInto(instance);
            resolvedInstances.Add(injectionKey, instance);
        }

        #endregion Resolving Instances

    }
}