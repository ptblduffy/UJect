using System;
using UJect.Utilities;

namespace UJect.Assertions
{
    public static class RuntimeAssert
    {
        public static void AssertIsTrue(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        public static void AssertIsFalse(bool condition, string message)
        {
            if (condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        public static void AssertObjectIsAlive(object obj, string message)
        {
            if (obj == null)
            {
                throw new InvalidOperationException(message);
            }
        }
        
        public static void AssertObjectIsAlive(IResolvedInstance obj, string message)
        {
            if (LifetimeCheck.IsNullOrDestroyed(obj))
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}