namespace UJect.Resolvers
{
    public interface IResolver
    {
        object Resolve();
    }
    
    public abstract class ResolverBase<TImpl> : IResolver<TImpl>
    {
        public abstract TImpl ResolveTypedInstance();
        object IResolver.Resolve() => ResolveTypedInstance();
    }
}