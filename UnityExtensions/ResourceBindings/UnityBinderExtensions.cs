namespace UJect.UnityExtensions
{
    public static class UnityBinderExtensions
    {
        public static UnityDiBinder<TInterface1> UnityBind<TInterface1>(this DiContainer diContainer)
            => new UnityDiBinder<TInterface1>(diContainer);

        public static UnityDiBinder<TInterface1, TInterface2> UnityBind<TInterface1, TInterface2>(this DiContainer diContainer)
            => new UnityDiBinder<TInterface1, TInterface2>(diContainer);

        public static UnityDiBinder<TInterface1, TInterface2, TInterface3> UnityBind<TInterface1, TInterface2, TInterface3>(this DiContainer diContainer)
            => new UnityDiBinder<TInterface1, TInterface2, TInterface3>(diContainer);

        public static UnityDiBinder<TInterface1, TInterface2, TInterface3, TInterface4> UnityBind<TInterface1, TInterface2, TInterface3, TInterface4>(this DiContainer diContainer)
            => new UnityDiBinder<TInterface1, TInterface2, TInterface3, TInterface4>(diContainer);

        public static UnityDiBinder<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5> UnityBind<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5>(this DiContainer diContainer)
            => new UnityDiBinder<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5>(diContainer);
    }
}