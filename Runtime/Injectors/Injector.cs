using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UJect.Exceptions;

namespace UJect.Injection
{
    internal class Injector
    {
        private static readonly Type         injectAttributeType      = typeof(InjectAttribute);
        private const           BindingFlags INJECTABLE_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly Lazy<HashSet<InjectionKey>>       dependsOn              = new Lazy<HashSet<InjectionKey>>();
        private readonly Lazy<List<InjectableConstructor>> injectableConstructors = new Lazy<List<InjectableConstructor>>();
        private readonly Lazy<List<InjectableField>>       injectableFields       = new Lazy<List<InjectableField>>();
        private readonly Lazy<List<InjectableProperty>>    injectableProperties   = new Lazy<List<InjectableProperty>>();
        private readonly Type                        referencedType;

        public IReadOnlyList<InjectableConstructor> InjectableConstructors => LazyValueOrEmpty(injectableConstructors);
        public IReadOnlyList<InjectableField>       InjectableFields       => LazyValueOrEmpty(injectableFields);

        public Injector(Type objType)
        {
            referencedType = objType;
            
            // Don't bother fetching fields or constructors from interfaces. They can't have them anyways
            if (referencedType.IsInterface) return;
            
            FetchConstructors();
            FetchFields();
            FetchProperties();
        }

        /// <summary>
        /// All dependencies of the contained type, as determined by fields and constructors
        /// </summary>
        public IEnumerable<InjectionKey> DependsOn => LazyValueOrEmpty(dependsOn);

        private void FetchFields()
        {
            var fields = referencedType.GetFields(INJECTABLE_BINDING_FLAGS)
                .Where(fi => fi.IsDefined(injectAttributeType, true));

            var cleared = false;
            foreach (var fieldInfo in fields)
            {
                if (!cleared)
                {
                    // This if is to avoid allocating injectableFields.Value if we end up not finding any fields to add
                    cleared = true;
                    injectableFields.Value.Clear();
                }
                
                var customId = fieldInfo.GetCustomAttribute<InjectAttribute>(true).CustomId;
                var fieldInjectionKey = new InjectionKey(fieldInfo.FieldType, customId);
                dependsOn.Value.Add(fieldInjectionKey);
                injectableFields.Value.Add(new InjectableField(fieldInfo, fieldInjectionKey));
            }
        }
        
        private void FetchProperties()
        {
            var properties = referencedType.GetProperties(INJECTABLE_BINDING_FLAGS)
                .Where(pi => pi.IsDefined(injectAttributeType, true));

            var cleared = false;
            foreach (var propertyInfo in properties)
            {
                if (!cleared)
                {
                    // This if is to avoid allocating injectableProperties.Value if we end up not finding any properties to add
                    cleared = true;
                    injectableProperties.Value.Clear();
                }
                
                var customId = propertyInfo.GetCustomAttribute<InjectAttribute>(true).CustomId;
                var fieldInjectionKey = new InjectionKey(propertyInfo.PropertyType, customId);
                dependsOn.Value.Add(fieldInjectionKey);
                injectableProperties.Value.Add(new InjectableProperty(propertyInfo, fieldInjectionKey));
            }
        }
        
        private void FetchConstructors()
        {
            injectableConstructors.Value.Clear();
            if (referencedType.IsInterface) throw new InvalidOperationException("You shouldn't be fetching for an interface!");
            var constructors = referencedType.GetConstructors(INJECTABLE_BINDING_FLAGS);
            foreach (var constructorInfo in constructors)
            {
                var parameterInfos = constructorInfo.GetParameters();

                List<InjectionKey> injectedParamsKeys = null;
                List<Type> nonInjectedParamTypes = null;

                var foundUninjectedParam = false;

                for (var paramIndex = 0; paramIndex < parameterInfos.Length; paramIndex++)
                {
                    var parameterInfo = parameterInfos[paramIndex];
                    var injectAttribute = parameterInfo.GetCustomAttribute<InjectAttribute>(true);

                    var isInjectedParam = injectAttribute != null;
                    if (isInjectedParam)
                    {
                        if (foundUninjectedParam) throw new InvalidOperationException("Injected args must come before injected args in constructor!");

                        var customId = injectAttribute.CustomId;
                        var argKey = new InjectionKey(parameterInfo.ParameterType, customId);
                        dependsOn.Value.Add(argKey);
                        injectedParamsKeys = injectedParamsKeys ?? new List<InjectionKey>();
                        injectedParamsKeys.Add(argKey);
                    }
                    else
                    {
                        foundUninjectedParam = true;
                        nonInjectedParamTypes = nonInjectedParamTypes ?? new List<Type>();
                        nonInjectedParamTypes.Add(parameterInfo.ParameterType);
                    }
                }

                var injectableConstructor = new InjectableConstructor(constructorInfo, injectedParamsKeys, nonInjectedParamTypes);
                injectableConstructors.Value.Add(injectableConstructor);
            }

            // We're going to sort all injectable constructors by number of injected args first, then by number of non-injected args
            // This makes the later lookup logic simpler
            injectableConstructors.Value.Sort((c1, c2) =>
            {
                var c1InjectedParamCount = c1.InjectedParamKeys?.Count ?? 0;
                var c2InjectedParamCount = c2.InjectedParamKeys?.Count ?? 0;
                
                var injectParamCountCompare =  c2InjectedParamCount.CompareTo(c1InjectedParamCount);
                if (injectParamCountCompare != 0) return injectParamCountCompare;

                var c1NonInjectedParamsCount = c1.NonInjectedParams?.Count ?? 0;
                var c2NonInjectedParamsCount = c2.NonInjectedParams?.Count ?? 0;
                
                var nonInjectedParamCountCompare =  c2NonInjectedParamsCount.CompareTo(c1NonInjectedParamsCount);
                return nonInjectedParamCountCompare;
            });
        }

        public void InjectFields(object obj, DiContainer diContainer)
        {
            foreach (var injectableField in InjectableFields)
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
        
        public TImpl CreateInstance<TImpl>(DiContainer diContainer, params object[] nonInjectedParams) => (TImpl)CreateInstance(diContainer, nonInjectedParams);

        public object CreateInstance(DiContainer diContainer, params object[] nonInjectedParams)
        {
            if (!TryFindMatchingConstructor(nonInjectedParams, out var injectableConstructor))
            {
                throw new InjectionException(referencedType, $"No constructor found{(nonInjectedParams?.Length > 0 ? $" that takes non-injected parameters [{string.Join(",", nonInjectedParams.Select(p=>p?.GetType().Name ?? "null"))}]" : string.Empty)}");
            }
            
            var paramKeys = injectableConstructor.InjectedParamKeys;
            var injectedParamKeyCount = injectableConstructor.InjectedParamKeys?.Count ?? 0;
            var nonInjectedParamKeyCount = injectableConstructor.NonInjectedParams?.Count ?? 0;
            var args = new object[injectedParamKeyCount + nonInjectedParamKeyCount];
            var paramIndex = 0;
            for (; paramIndex < injectedParamKeyCount; paramIndex++)
            {
                var argKey = paramKeys[paramIndex];
                if (diContainer.TryGetDependencyInternal<object>(argKey, out var dep))
                {
                    args[paramIndex] = dep;
                }
                else
                {
                    throw new InjectionException(referencedType, "Missing dependency for object constructor");
                }
            }
            
            // Copy any extra args into the args for the constructor
            if (nonInjectedParams != null)
            {
                Array.Copy(nonInjectedParams, 0, args, paramIndex, nonInjectedParams.Length);
            }
            var instance = injectableConstructor.ConstructorInfo.Invoke(args);
            
            // We don't call InjectFields here because it'll be called automatically when the instance resolves
            
            return instance;
        }

        private bool TryFindMatchingConstructor([CanBeNull] object[] extraArgs, out InjectableConstructor constructor)
        {
            // Injectable constructors are sorted by the most number of args first
            for (int i = 0; i < InjectableConstructors.Count; i++)
            {
                constructor = InjectableConstructors[i];

                var nonInjectedConstructorParams = constructor.NonInjectedParams;

                var extraArgsCount = extraArgs?.Length ?? 0;
                var nonInjectedConstructorParamsCount = nonInjectedConstructorParams?.Count ?? 0;
                
                // If one or the other is null, but not both, they can't possibly be equal
                if (extraArgsCount != nonInjectedConstructorParamsCount) continue;
                // Otherwise if one is null, they both must be, so they're equal
                if (extraArgsCount == 0 && nonInjectedConstructorParamsCount == 0) return true;

                if (ObjectAndParamsMatch(extraArgs, nonInjectedConstructorParams)) return true;
            }

            constructor = default;
            return false;

        }
        
        private static bool ObjectAndParamsMatch(object[] extraArgs, IReadOnlyList<Type> paramTypes)
        {
            // If the counts aren't equal, these constructors can't be equal
            if (extraArgs.Length != paramTypes.Count) return false;
            for (int i = 0; i < paramTypes.Count; i++)
            {
                var passedObjectType = extraArgs[i].GetType();
                var nonInjectedParamType = paramTypes[i];

                if (!nonInjectedParamType.IsAssignableFrom(passedObjectType)) return false;

            }

            return true;
        }
        
        private static IReadOnlyList<T> LazyValueOrEmpty<T>(Lazy<List<T>> lazy)
            => lazy.IsValueCreated ? (IReadOnlyList<T>)lazy.Value : Array.Empty<T>();
        
        private static IEnumerable<T> LazyValueOrEmpty<T>(Lazy<HashSet<T>> lazy)
            => lazy.IsValueCreated ? (IEnumerable<T>)lazy.Value : Array.Empty<T>();
        
        #region Helper Structs

        internal struct InjectableConstructor
        {
            [NotNull]
            public readonly ConstructorInfo ConstructorInfo;
            [CanBeNull]
            public readonly IReadOnlyList<InjectionKey>  InjectedParamKeys;
            [CanBeNull]
            public readonly IReadOnlyList<Type> NonInjectedParams;

            public InjectableConstructor(ConstructorInfo constructorInfo, [CanBeNull] IReadOnlyList<InjectionKey> injectedParamKeys, [CanBeNull] IReadOnlyList<Type> nonInjectedParams)
            {
                ConstructorInfo = constructorInfo;
                InjectedParamKeys       = injectedParamKeys;
                NonInjectedParams = nonInjectedParams;
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

        internal struct InjectableProperty
        {
            public readonly PropertyInfo PropertyInfo;
            public readonly InjectionKey InjectionKey;

            public InjectableProperty(PropertyInfo propertyInfo, InjectionKey injectionKey)
            {
                PropertyInfo = propertyInfo;
                InjectionKey = injectionKey;
            }
        }
        #endregion
    }
}