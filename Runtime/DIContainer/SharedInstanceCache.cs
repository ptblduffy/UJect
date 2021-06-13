using System;
using System.Collections.Generic;

namespace UJect
{
    internal class SharedInstanceCache
    {
        private readonly Dictionary<InjectionKey, object> sharedInstances         = new Dictionary<InjectionKey, object>();
        private readonly RefCounter<InjectionKey> refCounter = new RefCounter<InjectionKey>();

        public int RefCount(InjectionKey key) => refCounter.RefCount(key);

        public T GetOrCreateSharedInstance<T>(InjectionKey key, Func<T> createInstance)
        {
            if (sharedInstances.TryGetValue(key, out var sharedInstance))
            {
                refCounter.Increment(key);
                return (T)sharedInstance;
            }

            var newInstance = createInstance();
            refCounter.Increment(key);
            sharedInstances[key] = newInstance;
            return newInstance;
        }

        public void ReleaseBinding(InjectionKey key)
        {
            refCounter.Decrement(key);
            if (refCounter.RefCount(key) == 0)
            {
                sharedInstances.Remove(key);
            }
        }
    }

    internal class RefCounter<TKey>
    {
        private readonly Dictionary<TKey, int> refCounts = new Dictionary<TKey, int>();

        public int RefCount(TKey key)
        {
            if (refCounts.TryGetValue(key, out var count))
            {
                return count;
            }

            return 0;
        }

        public void Increment(TKey key)
        {
            var count = refCounts.TryGetValue(key, out var existingCount) ? existingCount : 0;
            refCounts[key] = count + 1;
        }

        public void Decrement(TKey key)
        {
            if (refCounts.TryGetValue(key, out var count))
            {
                count -= 1;
            }

            if (count <= 0)
            {
                refCounts.Remove(key);
            }
            else
            {
                refCounts[key] = count;
            }
        }
    }
}