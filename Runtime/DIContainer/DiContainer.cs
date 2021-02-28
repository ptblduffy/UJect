using System;
using System.Collections.Generic;
using System.Linq;
using UJect.Assertions;
using UJect.Exceptions;
using UJect.Factories;
using UJect.Injection;
using UJect.Resolvers;
using UnityEngine;
using static UJect.Assertions.RuntimeAssert;
using Object = UnityEngine.Object;

namespace UJect
{
    public sealed partial class DiContainer : IDisposable
    {
        private readonly Dictionary<InjectionKey, IResolver> dependencyResolvers = new Dictionary<InjectionKey, IResolver>();
        private readonly DependencyTree                      dependencyTree      = new DependencyTree();
        private readonly DiContainer                         parentContainer;
        private readonly string                              containerName;

        private bool isDisposed;

        public DiContainer(DiContainer parentContainer = null, string containerName = null)
        {
            this.parentContainer = parentContainer;
            this.containerName   = containerName;
            Phase                = DiPhase.Bind;
            BindInstance(this);
        }

        private DiPhase Phase
        {
            get => phase;
            set
            {
                if (phase != value)
                {
                    Debug.Log($"DI moving to phase {value}");
                    phase = value;
                }
            }
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            dependencyResolvers.Clear();
        }

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
        public bool TryGet<TInterface>(out TInterface dependency) where TInterface : class
        {
            return TryGet(null, out dependency);
        }

        /// <summary>
        ///     Try to get a bound dependency based on the given key
        /// </summary>
        /// <param name="customId"></param>
        /// <param name="dependency">The returned dependency</param>
        /// <returns>True if dependency is bound, false otherwise</returns>
        public bool TryGet<TType>(string customId, out TType dependency)
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
                RuntimeAssert.AssertObjectIsAlive(dependency, $"Null dependency detected in: {this}. It should have been unregistered!");
                return true;
            }

            if (parentContainer != null && parentContainer.TryGetDependencyInternal<TType>(key, out var parentDependency))
            {
                dependency = parentDependency;
                RuntimeAssert.AssertObjectIsAlive(dependency, $"Null dependency detected in parent: {parentContainer}. It should have been unregistered!");
                return true;
            }

            dependency = default;
            return false;
        }

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

        public IDiBinder<TInterface> Bind<TInterface>()
        {
            AssertIsFalse(isDisposed, "You should not try to bind to a disposed container!");

            return new DiBinder<TInterface>(this);
        }

        public DiContainer BindInstance<TClass>(TClass instance, string customId = null) where TClass : class
        {
            AssertIsFalse(isDisposed, "You should not try to bind to a disposed container!");
            AssertIsFalse(typeof(TClass).IsInterface, "You should not try to bind an instance of an interface!");
            InstallBindingInternal<TClass, TClass>(customId, new InstanceResolver<TClass>(instance));
            return this;
        }

        public bool Unbind<TType>(string customId = null)
        {
            var key = new InjectionKey(typeof(TType), customId);
            if (dependencyResolvers.TryGetValue(key, out var resolver))
            {
                dependencyResolvers.Remove(key);
                resolvedInstances.Remove(key);
                return true;
            }

            return false;
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

        #region Resolvers

        private readonly Dictionary<InjectionKey, object> resolvedInstances = new Dictionary<InjectionKey, object>();
        private          DiPhase                          phase;

        /// <summary>
        ///     Resolve all dependencies, if required. You shouldn't have to call this, it should happen automatically when attempting to retrieve a dependency.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
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

        #endregion
    }
}