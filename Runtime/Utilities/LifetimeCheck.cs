namespace UJect.Utilities
{
    internal static class LifetimeCheck
    {
        public static bool IsNullOrDestroyed(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return true;
            }

            //Unity has a special null check that does a lifetime check
            if (obj is UnityEngine.Object unityObject && unityObject == null)
            {
                return true;
            }

            return false;
        }
    }
}