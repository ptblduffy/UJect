using System;
using System.Collections.Generic;

namespace UJect.Injection
{
    internal static class InjectorCache
    {
        private static readonly Dictionary<Type, Injector> injectorCache = new();

        internal static int CachedInjectorCount => injectorCache.Count;
        
        /// <summary>
        /// Get or create an injector for the given type. Stored in a static dictionary for further access.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Injector GetOrCreateInjector(Type t)
        {
            if (!injectorCache.TryGetValue(t, out var existingInjector))
            {
                existingInjector = new Injector(t);
                injectorCache.Add(t, existingInjector);
            }

            return existingInjector;
        }

        /// <summary>
        /// Clear the injector cache.
        /// </summary>
        public static void ClearCache()
        {
            injectorCache.Clear();
        }
    }
}