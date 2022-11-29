namespace UJect
{
    public abstract class ResolvedInstanceBase<TImpl> : IResolvedInstance<TImpl>
    {
        public abstract bool   IsDestroyed         { get; }
        public abstract TImpl  InstanceObjectTyped { get; }
        public          object InstanceObject      => InstanceObjectTyped;

        private bool isInitialized = false;
        
        public void Initialize(DiContainer diContainer)
        {
            if (IsDestroyed) return;
            
            if (isInitialized) return;
            isInitialized = true;
            
            if (InstanceObjectTyped is IInitializable initializable)
            {
                initializable.Initialize(diContainer);
            }
        }

    }
}