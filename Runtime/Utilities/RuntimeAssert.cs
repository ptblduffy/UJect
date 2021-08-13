using System;
using UJect.Utilities;
using Object = UnityEngine.Object;

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
            if (LifetimeCheck.IsNullOrDestroyed(obj))
            {
                throw new InvalidOperationException(message);
            }
        }

        public static void AssertObjectIsAlive(object obj, string message, object arg)
        {
            if (LifetimeCheck.IsNullOrDestroyed(obj))
            {
                throw new InvalidOperationException(string.Format(message, arg));
            }
        }
    }
}