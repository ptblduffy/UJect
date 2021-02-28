namespace UJect.Resolvers
{
    /// <summary>
    /// Simple abstract class to extend for custom resolver classes.
    /// </summary>
    /// <typeparam name="TImpl"></typeparam>
    public abstract class ResolverBase<TImpl> : IResolver<TImpl>
    {
        public abstract TImpl ResolveTypedInstance();
        object IResolver.Resolve() => ResolveTypedInstance();
    }
}