// Copyright (c) 2024 Eric Bennett McDuffee

namespace UJect
{

    internal class PocoResolvedInstance<TImpl> : ResolvedInstanceBase<TImpl>
    {
        public override bool IsDestroyed => false;
        public override TImpl InstanceObjectTyped { get; }
        
        public PocoResolvedInstance(TImpl instanceObject)
        {
            InstanceObjectTyped = instanceObject;
        }
    }
}
