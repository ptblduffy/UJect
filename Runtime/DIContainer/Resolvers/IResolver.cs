namespace UJect.Resolvers
{
    public interface IResolver<TImpl> : IResolver
    {
        TImpl ResolveTypedInstance();
    }
}