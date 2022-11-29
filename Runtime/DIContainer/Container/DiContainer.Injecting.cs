using System;
using JetBrains.Annotations;
using UJect.Assertions;
using UJect.Injection;
using Uject.Utilities;

namespace UJect
{
    public sealed partial class DiContainer
    {
        /// <summary>
        /// Create an instance of type T, with all constructor params and fields injected
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [LibraryEntryPoint]
        [NotNull]
        public T CreateInjectedInstance<T>() => (T)CreateInjectedInstance(typeof(T));

        /// <summary>
        /// Create an instance of type instanceType, with all constructor params and fields injected
        /// </summary>
        [LibraryEntryPoint]
        [NotNull]
        public object CreateInjectedInstance(Type instanceType)
        {
            var injector = InjectorCache.GetOrCreateInjector(instanceType);
            // Injector.CreateInstance will do constructor injection but not field injection
            var instance = injector.CreateInstance(this, instanceType);
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