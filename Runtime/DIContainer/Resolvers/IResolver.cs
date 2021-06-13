using System;

namespace UJect.Resolvers
{
    /// <summary>
    /// IResolvers must be able to resolve a single instance of a dependency.
    /// Resolve will be called once when the instance is first resolved. That instance will be cached internally
    /// </summary>
    public interface IResolver : IDisposable
    {
        object Resolve();
    }
    
    /// <summary>
    /// IResolvers must be able to resolve a single instance of a dependency.
    /// Resolve will be called once when the instance is first resolved. That instance will be cached internally
    /// </summary>
    public interface IResolver<out TImpl> : IResolver
    {
        TImpl ResolveTypedInstance();
    }
}