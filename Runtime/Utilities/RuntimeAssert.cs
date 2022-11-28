using System;
using UJect.Utilities;

namespace UJect.Assertions
{
    internal static class RuntimeAssert
    {
        public static void AssertIsFalse(bool condition, string message)
        {
            if (condition)
            {
                throw new InvalidOperationException(message);
            }
        }
        
        public static void AssertNotNull<T>(T obj, string message) where T : class
        {
            if (obj == null)
            {
                throw new InvalidOperationException(message);
            }
        }

        public static void AssertNotNullFormat<T, TArg1, TArg2>(T obj, string messageFormat, TArg1 messageArg1, TArg2 messageArg2) where T : class
        {
            if (obj == null)
            {
                throw new InvalidOperationException(string.Format(messageFormat, messageArg1, messageArg2));
            }
        }

        public static void AssertObjectIsAliveFormat<TArg>(IResolvedInstance obj, string messageFormat, TArg messageArg)
        {
            if (LifetimeCheck.IsNullOrDestroyed(obj))
            {
                throw new InvalidOperationException(string.Format(messageFormat, messageArg));
            }
        }
    }
}