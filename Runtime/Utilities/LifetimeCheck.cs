namespace UJect.Utilities
{
    internal static class LifetimeCheck
    {
        public static bool IsNullOrDestroyed(IResolvedInstance obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return true;
            }

            if (obj.IsDestroyed)
            {
                return true;
            }

            return false;
        }
    }
}