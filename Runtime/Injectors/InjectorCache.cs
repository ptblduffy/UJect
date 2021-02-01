using System;
using System.Collections.Generic;

namespace UJect
{
    internal static class InjectorCache
    {
        private static readonly Dictionary<Type, Injector> injectorCache = new Dictionary<Type, Injector>();

        public static Injector GetInjector(Type t)
        {
            if (!injectorCache.TryGetValue(t, out var existingInjector))
            {
                existingInjector = new Injector(t);
                injectorCache.Add(t, existingInjector);
            }

            return existingInjector;
        }
    }
}