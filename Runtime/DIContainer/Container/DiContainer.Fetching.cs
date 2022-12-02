using System;
using JetBrains.Annotations;
using UJect.Assertions;
using UJect.Utilities;

namespace UJect
{
    public sealed partial class DiContainer
    {
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
            RuntimeAssert.AssertIsFalse(isDisposed, "You should not try to retrieve bindings from a disposed container!");

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
        public bool TryGet<TType>(string customId, [NotNull] out TType dependency)
        {
            var key = new InjectionKey(typeof(TType), customId);
            return TryGetDependencyInternal(key, out dependency);
        }

        internal bool TryGetDependencyInternal<TType>(InjectionKey key, out TType dependency)
        {
            RuntimeAssert.AssertIsFalse(isDisposed, "You should not try to retrieve bindings from a disposed container!");
            TryResolveAll();

            if (TryGetDependencyResolvedInstance<TType>(key, out var dependencyInstance))
            {
                dependency = (TType)dependencyInstance.InstanceObject;
                return true;
            }

            dependency = default;
            return false;
        }

        /// <summary>
        /// Try to retrieve the IResolvedInstance at a given key.
        /// If resolution is necessary, that will run first, followed by looking in the current container, and then the parent container.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dependency"></param>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        private bool TryGetDependencyResolvedInstance<TType>(InjectionKey key, out IResolvedInstance dependency)
        {
            RuntimeAssert.AssertIsFalse(isDisposed, "You should not try to retrieve bindings from a disposed container!");
            TryResolveAll();

            if (resolvedInstances.TryGetValue(key, out var resolvedDependency))
            {
                dependency = resolvedDependency;
                RuntimeAssert.AssertNotNullFormat(resolvedDependency, "Null dependency detected in: {0} for key {1}. It should have been unregistered!", this, key);
                return true;
            }

            if (parentContainer != null && parentContainer.TryGetDependencyResolvedInstance<TType>(key, out var parentDependency))
            {
                dependency = parentDependency;
                return true;
            }

            dependency = default;
            return false;
        }
    }
}