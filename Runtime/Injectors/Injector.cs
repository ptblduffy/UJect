using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UJect.Exceptions;

namespace UJect.Injection
{
    internal class Injector
    {
        private static readonly Type         injectAttributeType      = typeof(InjectAttribute);
        private const           BindingFlags INJECTABLE_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly HashSet<InjectionKey>       dependsOn              = new();
        private readonly List<InjectableConstructor> injectableConstructors = new();
        private readonly List<InjectableField>       injectableFields       = new();
        private readonly Type                        referencedType;

        public IReadOnlyList<InjectableConstructor> InjectableConstructors => injectableConstructors;
        public IReadOnlyList<InjectableField> InjectableFields => injectableFields;

        public Injector(Type objType)
        {
            referencedType = objType;
            FetchFields();
            FetchConstructors();
        }

        /// <summary>
        /// All dependencies of the contained type, as determined by fields and constructors
        /// </summary>
        public IEnumerable<InjectionKey> DependsOn => dependsOn;

        private void FetchFields()
        {
            injectableFields.Clear();
            var fields = referencedType.GetFields(INJECTABLE_BINDING_FLAGS)
                .Where(fi => fi.IsDefined(injectAttributeType, true));

            foreach (var fieldInfo in fields)
            {
                var customId = fieldInfo.GetCustomAttribute<InjectAttribute>(true).CustomId;
                var fieldInjectionKey = new InjectionKey(fieldInfo.FieldType, customId);
                dependsOn.Add(fieldInjectionKey);
                injectableFields.Add(new InjectableField(fieldInfo, fieldInjectionKey));
            }
        }

        private void FetchConstructors()
        {
            injectableConstructors.Clear();
            var constructors = referencedType.GetConstructors(INJECTABLE_BINDING_FLAGS);
            foreach (var constructorInfo in constructors)
            {
                var parameterInfos = constructorInfo.GetParameters();
                if (parameterInfos.All(pi => pi.IsDefined(injectAttributeType, true)))
                {
                    var argsKeys = new InjectionKey[parameterInfos.Length];

                    for (var paramIndex = 0; paramIndex < parameterInfos.Length; paramIndex++)
                    {
                        var parameterInfo = parameterInfos[paramIndex];
                        var customId = parameterInfo.GetCustomAttribute<InjectAttribute>(true).CustomId;
                        var argKey = new InjectionKey(parameterInfo.ParameterType, customId);
                        dependsOn.Add(argKey);
                        argsKeys[paramIndex] = argKey;
                    }

                    var injectableConstructor = new InjectableConstructor(constructorInfo, argsKeys);

                    injectableConstructors.Add(injectableConstructor);
                }
            }

            injectableConstructors.Sort((c1, c2) => c2.ParamKeys.Length.CompareTo(c1.ParamKeys.Length));
        }

        public void InjectFields(object obj, DiContainer diContainer)
        {
            foreach (var injectableField in injectableFields)
            {
                if (diContainer.TryGetDependencyInternal<object>(injectableField.InjectionKey, out var dependency))
                {
                    injectableField.FieldInfo.SetValue(obj, dependency);
                }
                else
                {
                    throw new InvalidOperationException($"No dependency found for injected field {injectableField.FieldInfo.Name} with key {injectableField.InjectionKey} in {obj}");
                }
            }
        }

        public object CreateInstance(DiContainer diContainer, Type newInstanceType)
        {
            if (injectableConstructors.Count == 0)
            {
                throw new InjectionException(newInstanceType, "No constructor found");
            }
            
            var constructorPair = injectableConstructors.First();
            var paramKeys = constructorPair.ParamKeys;
            var args = new object[paramKeys.Length];
            for (var paramIndex = 0; paramIndex < paramKeys.Length; paramIndex++)
            {
                var argKey = paramKeys[paramIndex];
                if (diContainer.TryGetDependencyInternal<object>(argKey, out var dep))
                {
                    args[paramIndex] = dep;
                }
                else
                {
                    throw new InvalidOperationException("Missing dependency for object constructor");
                }
            }

            var instance = constructorPair.ConstructorInfo.Invoke(args);
            
            // We don't call InjectFields here because it'll be called automatically when the instance resolves
            
            return instance;
        }

        public TImpl CreateInstance<TImpl>(DiContainer diContainer) => (TImpl)CreateInstance(diContainer, typeof(TImpl));

        #region Helper Structs

        internal struct InjectableConstructor
        {
            public readonly ConstructorInfo ConstructorInfo;
            public readonly InjectionKey[]  ParamKeys;

            public InjectableConstructor(ConstructorInfo constructorInfo, InjectionKey[] paramKeys)
            {
                ConstructorInfo = constructorInfo;
                ParamKeys       = paramKeys;
            }
        }

        internal struct InjectableField
        {
            public readonly FieldInfo    FieldInfo;
            public readonly InjectionKey InjectionKey;

            public InjectableField(FieldInfo fieldInfo, InjectionKey injectionKey)
            {
                FieldInfo    = fieldInfo;
                InjectionKey = injectionKey;
            }
        }

        #endregion
    }
}