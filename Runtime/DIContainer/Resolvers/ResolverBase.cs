namespace UJect.Resolvers
{
    /// <summary>
    /// Simple abstract class to extend for custom resolver classes.
    /// </summary>
    /// <typeparam name="TImpl"></typeparam>
    public abstract class ResolverBase<TImpl> : IResolver
    {
        public abstract IResolvedInstance<TImpl> ResolveTypedInstance();
        IResolvedInstance IResolver.Resolve() => ResolveTypedInstance();
    }
}