using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UJect.RuntimeAssert;

namespace UJect
{
    public sealed partial class DiContainer : IDisposable
    {
        private readonly        DiContainer                         parentContainer;
        private readonly        string                              containerName;
        private readonly        Dictionary<InjectionKey, IResolver> dependencyResolvers = new Dictionary<InjectionKey, IResolver>();
        private readonly DependencyTree dependencyTree = new DependencyTree();

        private enum DiPhase : byte
        {
            Bind     = 0,
            Resolved = 1,
        }

        private DiPhase Phase { get; set; }

        private bool isDisposed = false;

        public DiContainer(DiContainer parentContainer = null, string containerName = null)
        {
            this.parentContainer = parentContainer;
            this.containerName   = containerName;
            Phase                = DiPhase.Bind;
            BindInstance(this);
        }

        public DiContainer CreateChildContainer(string childContainerName = null)
        {
            return new DiContainer(this, childContainerName);
        }

        public override string ToString()
        {
            return $"{nameof(DiContainer)} \"{this.containerName}\"";
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
                Debug.Log($"---- Unbinding {key}");
                dependencyResolvers.Remove(key);
                resolvedInstances.Remove(key);
                resolver.Dispose();
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

        internal void InstallBindingInternal(InjectionKey fromKey, InjectionKey toKey, IResolver dependencyResolver)
        {
            Debug.Log($"---- Binding {fromKey} => {toKey}");
            AddDependencies(fromKey, toKey);

            if (dependencyResolvers.ContainsKey(fromKey))
            {
                throw new ArgumentException($"Existing binding found for binding key {fromKey}. Another cannot be added!");
            }
            else
            {
                dependencyResolvers.Add(fromKey, dependencyResolver);
            }

            if (Phase > DiPhase.Bind)
            {
                ResolveInstance(fromKey, dependencyResolver);
            }
        }
        
        
        private void AddDependencies(InjectionKey fromKey, InjectionKey toKey)
        {
            dependencyTree.AddDependency(fromKey, toKey, true);
            
            var injector = InjectorCache.GetInjector(toKey.InjectedResourceType);
            foreach (var dependsOn in injector.DependsOn)
            {
                dependencyTree.AddDependency(toKey, dependsOn, false);
            }
            
            if(dependencyTree.HasCycle(out var onDependency))
            {
                throw new InvalidOperationException($"Adding dependency binding for {fromKey} to {toKey} resulted in a dependency cycle!\n{onDependency}");
            }
        }

        #endregion Internal Binding Methods

        #region Resolvers

        private readonly Dictionary<InjectionKey, object> resolvedInstances = new Dictionary<InjectionKey, object>();

        public void Resolve()
        {

            if (Phase < DiPhase.Resolved)
            {
                Phase = DiPhase.Resolved;
                resolvedInstances.Clear();

                if (dependencyTree.HasCycle(out var dep))
                {
                    throw new InvalidOperationException("Cycle detected in dependency tree!");
                }

                var sorted = dependencyTree.Sorted();


                foreach (var injectionKey in sorted)
                {
                    var deps = dependencyTree.GetDependenciesFor(injectionKey);
                    
                    Debug.Log(injectionKey + "\n\t" + string.Join("\n\t", deps.Select(r=>r.ToString())));
                    
                }
                
                Debug.Log(string.Join(", ", dependencyTree.Sorted().Select(r=>r.ToString())));

                foreach (var injectionKey in dependencyTree.Sorted())
                {
                    if (dependencyResolvers.TryGetValue(injectionKey, out var resolver))
                    {
                        ResolveInstance(injectionKey, resolver);
                    }
                }
            }
        }

        private void ResolveInstance(InjectionKey injectionKey, IResolver resolver)
        {
            Debug.Log($"Resolving instance of {injectionKey}");
            var instance = resolver.Resolve();
            InjectInto(instance);
            resolvedInstances.Add(injectionKey, instance);
        }

        #endregion


        public TInterface GetDependency<TInterface>(string customId = null) where TInterface : class
        {
            AssertIsFalse(isDisposed, "You should not try to retrieve bindings from a disposed container!");

            var key = new InjectionKey(typeof(TInterface), customId);
            if (TryGetDependencyInternal<TInterface>(key, out var dependency))
            {
                return dependency;
            }

            throw new ArgumentException($"No dependency of type {typeof(TInterface)} found{(customId != null ? $" with customId \"{customId}\"" : "")}");
        }

        public bool TryGetDependency<TInterface>(out TInterface dependency) where TInterface : class
        {
            return TryGetDependency(null, out dependency);
        }

        public bool TryGetDependency<TType>(string customId, out TType dependency)
        {
            var key = new InjectionKey(typeof(TType), customId);
            return TryGetDependencyInternal(key, out dependency);
        }

        private bool TryGetDependencyInternal<TType>(InjectionKey key, out TType dependency)
        {
            AssertIsFalse(isDisposed, "You should not try to retrieve bindings from a disposed container!");
            Resolve();

            if (resolvedInstances.TryGetValue(key, out var resolvedDependency))
            {
                dependency = (TType)resolvedDependency;
                AssertIsFalse(IsNullOrDestroyed(dependency), "Null dependency detected. It should have been unregistered!");
                return true;
            }

            if (parentContainer != null && parentContainer.TryGetDependencyInternal<TType>(key, out var parentDependency))
            {
                dependency = parentDependency;
                AssertIsFalse(IsNullOrDestroyed(dependency), "Null dependency detected. It should have been unregistered!");
                return true;
            }

            dependency = default;
            return false;
        }
        
        private static bool IsNullOrDestroyed<TType>(TType obj)
        {
            if (obj == null)
            {
                return true;
            }
            if (obj is UnityEngine.Object uobj)
            {
                return uobj == null;
            }
            return false;
        }

        internal bool TryGetDependencyForInjectionInternal(InjectionKey key, out object dependency)
        {
            AssertIsFalse(isDisposed, "You should not try to retrieve bindings from a disposed container!");
            Resolve();

            if (resolvedInstances.TryGetValue(key, out var resolvedDependency))
            {
                dependency = resolvedDependency;
                AssertIsFalse(IsNullOrDestroyed(dependency), "Null dependency detected. It should have been unregistered!");
                return true;
            }

            if (parentContainer != null && parentContainer.TryGetDependencyForInjectionInternal(key, out var parentDependency))
            {
                dependency = parentDependency;
                AssertIsFalse(IsNullOrDestroyed(dependency), "Null dependency detected. It should have been unregistered!");
                return true;
            }
            
            Debug.Log(string.Join("\n", resolvedInstances.Keys.Select(o=>o.ToString())));

            dependency = default;
            return false;
        }

        public void InjectInto(object obj)
        {
            AssertObjectIsAlive(obj, "Can't inject into null Object!");
            var objType = obj.GetType();
            InjectorCache.GetInjector(objType).InjectFields(obj, this);
        }

        public void Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;
            foreach (var resolver in dependencyResolvers.Values)
            {
                resolver.Dispose();
            }

            dependencyResolvers.Clear();
        }
    }
}