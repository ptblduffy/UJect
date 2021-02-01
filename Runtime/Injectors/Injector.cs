using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UJect
{
    internal class Injector
    {
        private readonly Type referencedType;

        public IEnumerable<DiContainer.InjectionKey> DependsOn => dependsOn;

        private readonly List<(FieldInfo fieldInfo, string customId)>                injectableFields       = new List<(FieldInfo fieldInfo, string customId)>();
        private readonly List<(ConstructorInfo, DiContainer.InjectionKey[] argKeys)> injectableConstructors = new List<(ConstructorInfo, DiContainer.InjectionKey[] argKeys)>();

        private readonly HashSet<DiContainer.InjectionKey> dependsOn = new HashSet<DiContainer.InjectionKey>();

        public Injector(Type objType)
        {
            this.referencedType = objType;
            FetchFields();
            FetchConstructors();
        }

        private void FetchFields()
        {
            this.injectableFields.Clear();
            var fields = referencedType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(fi => fi.IsDefined(typeof(InjectAttribute), true));

            foreach (var fieldInfo in fields)
            {
                var customId = fieldInfo.GetCustomAttribute<InjectAttribute>(true).CustomId;
                var fieldInjectionKey = new DiContainer.InjectionKey(fieldInfo.FieldType, customId);
                dependsOn.Add(fieldInjectionKey);

                injectableFields.Add((fieldInfo, customId));
            }
        }


        private void FetchConstructors()
        {
            this.injectableConstructors.Clear();
            var constructors = referencedType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var constructor in constructors)
            {
                var parameterInfos = constructor.GetParameters();
                if (parameterInfos.All(pi => pi.IsDefined(typeof(InjectAttribute), true)))
                {
                    var argsKeys = new DiContainer.InjectionKey[parameterInfos.Length];

                    for (var paramIndex = 0; paramIndex < parameterInfos.Length; paramIndex++)
                    {
                        var parameterInfo = parameterInfos[paramIndex];
                        var customId = parameterInfo.GetCustomAttribute<InjectAttribute>().CustomId;
                        var argKey = new DiContainer.InjectionKey(parameterInfo.ParameterType, customId);
                        dependsOn.Add(argKey);
                        argsKeys[paramIndex] = argKey;
                    }
                    
                    injectableConstructors.Add((constructor, argsKeys));
                }
            }

            injectableConstructors.Sort((c1, c2) => c2.argKeys.Length.CompareTo(c1.argKeys.Length));
        }


        public void InjectFields(object obj, DiContainer diContainer)
        {
            foreach (var (injectedField, customId) in injectableFields)
            {
                var injectionKey = new DiContainer.InjectionKey(injectedField.FieldType, customId);
                if (diContainer.TryGetDependencyForInjectionInternal(injectionKey, out var dependency))
                {
                    injectedField.SetValue(obj, dependency);
                }
                else
                {
                    throw new InvalidOperationException($"No dependency found for injected field {injectedField.Name} with key {injectionKey} in {obj}");
                }
            }
        }

        public TImpl Create<TImpl>(DiContainer diContainer)
        {
            var constructorPair = this.injectableConstructors.First();
            var argKeys = constructorPair.argKeys;
            var args = new object[argKeys.Length];
            for (var paramIndex = 0; paramIndex < argKeys.Length; paramIndex++)
            {
                var argKey = argKeys[paramIndex];
                if (diContainer.TryGetDependencyForInjectionInternal(argKey, out var dep))
                {
                    args[paramIndex] = dep;
                }
                else
                {
                    throw new InvalidOperationException("Missing dependency for object constructor");
                }
            }

            return (TImpl)constructorPair.Item1.Invoke(args);
        }
    }
}