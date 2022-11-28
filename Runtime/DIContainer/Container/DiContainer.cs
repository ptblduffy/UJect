using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UJect.Assertions;
using UJect.Exceptions;
using UJect.Factories;
using UJect.Injection;
using UJect.Resolvers;
using Uject.Utilities;
using UJect.Utilities;
using static UJect.Assertions.RuntimeAssert;

namespace UJect
{
    public sealed partial class DiContainer : IDisposable
    {
        private readonly Dictionary<InjectionKey, IResolvedInstance> resolvedInstances   = new Dictionary<InjectionKey, IResolvedInstance>();
        private readonly Dictionary<InjectionKey, IResolver>         dependencyResolvers = new Dictionary<InjectionKey, IResolver>();
        private readonly DependencyTree                              dependencyTree      = new DependencyTree();
        private readonly DiContainer                                 parentContainer;
        private readonly string                                      containerName;

        private DiPhase phase;
        private bool    isDisposed;
        
        private DiPhase Phase
        {
            get => phase;
            set => phase = value;
        }

        /// <summary>
        /// Name of DiContainer.
        /// </summary>
        public string ContainerName => containerName;
        
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
                if (dependencyResolver is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            
            resolvedInstances.Clear();
            dependencyResolvers.Clear();
        }

        /// <summary>
        /// Create a new DiContainer that is a child of this container.
        /// </summary>
        /// <param name="childContainerName"></param>
        /// <returns></returns>
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

        private enum DiPhase : byte
        {
            Bind     = 0,
            Resolved = 1
        }

        #region Internal Binding Methods

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

                foreach (var resolvedInstance in resolvedInstances.Select(r => r.Value))
                {
                    if (!LifetimeCheck.IsNullOrDestroyed(resolvedInstance))
                    {
                        resolvedInstance.Initialize(this);
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
            var resolvedInstance = resolver.Resolve();
            AssertObjectIsAlive(resolvedInstance, $"Cannot inject into dead resolved instance for key {injectionKey}");
            InjectInto(resolvedInstance.InstanceObject);
            resolvedInstances.Add(injectionKey, resolvedInstance);
        }

        #endregion Resolving Instances

    }
}