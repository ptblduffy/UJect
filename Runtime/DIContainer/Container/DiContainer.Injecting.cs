using System;
using JetBrains.Annotations;
using UJect.Assertions;
using UJect.Injection;
using UJect.Utilities;

namespace UJect
{
    public sealed partial class DiContainer
    {
        /// <summary>
        /// Create an instance of type T, with all injectable constructor params and fields injected
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [LibraryEntryPoint]
        [NotNull]
        public T CreateInjectedInstance<T>() => CreateInjectedInstance<T>(null);

        /// <summary>
        /// Create an instance of type instanceType given an array of non-injected constructor params, with all injectable constructor params and fields injected
        /// </summary>
        [LibraryEntryPoint]
        [NotNull]
        public T CreateInjectedInstance<T>([CanBeNull] params object[] nonInjectedConstructorParams) => (T)CreateInjectedInstance(typeof(T), nonInjectedConstructorParams);

        /// <summary>
        /// Create an instance of type instanceType, with all injectable constructor params and fields injected
        /// </summary>
        [LibraryEntryPoint]
        [NotNull]
        public object CreateInjectedInstance(Type instanceType) => CreateInjectedInstance(instanceType, null);

        /// <summary>
        /// Create an instance of type instanceType given an array of non-injected constructor params, with all injectable constructor params and fields injected
        /// </summary>
        [LibraryEntryPoint]
        [NotNull]
        public object CreateInjectedInstance(Type instanceType, [CanBeNull] params object[] nonInjectedConstructorParams)
        {
            var injector = InjectorCache.GetOrCreateInjector(instanceType);
            // Injector.CreateInstance will do constructor injection but not field injection
            var instance = injector.CreateInstance(this, nonInjectedConstructorParams);
            injector.InjectFields(instance, this);
            return instance;
        }

        /// <summary>
        /// Inject all *currently resolved objects* into a given object.
        /// *NOTE*: This will *NOT* resolve unresolved instances, as order of operations is not known.
        /// </summary>
        /// <param name="obj"></param>
        [LibraryEntryPoint]
        public void InjectInto(object obj)
        {
            RuntimeAssert.AssertNotNull(obj, "Can't inject into null Object!");
            var objType = obj.GetType();
            InjectorCache.GetOrCreateInjector(objType).InjectFields(obj, this);
        }
    }
}