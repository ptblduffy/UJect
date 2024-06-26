// Copyright (c) 2024 Eric Bennett McDuffee

namespace UJect
{
    public interface IResolvedInstance : IInitializable
    {
        object InstanceObject { get; }
        bool   IsDestroyed    { get; }
    }
    
    public interface IResolvedInstance<TImpl> : IResolvedInstance
    {
        TImpl InstanceObjectTyped { get; }
    }
}
