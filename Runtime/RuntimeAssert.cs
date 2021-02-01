using System;

namespace UJect
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
            if (ReferenceEquals(null, obj))
            {
                throw new InvalidOperationException(message);
            }

            if (obj is UnityEngine.Object unityObject && unityObject == null)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}