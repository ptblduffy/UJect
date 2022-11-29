using UnityEngine;

namespace UJect.UnityExtensions
{
    public class UnityDiBinder<TInterface1> : DiBinder<TInterface1>
    {
        public UnityDiBinder(DiContainer dependencies) : base(dependencies) { }

        public DiContainer ToResource<TImpl>(string resourcePath) where TImpl : Object, TInterface1
        {
            var resolver = new ResourceInstanceResolver<TImpl>(resourcePath);
            return ToCustomResolver<TImpl>(resolver);
        }
    }

    public class UnityDiBinder<TInterface1, TInterface2> : DiBinder<TInterface1, TInterface2>
    {
        public UnityDiBinder(DiContainer dependencies) : base(dependencies) { }

        public DiContainer ToResource<TImpl>(string resourcePath) where TImpl : Object, TInterface1, TInterface2
        {
            var resolver = new ResourceInstanceResolver<TImpl>(resourcePath);
            return ToCustomResolver<TImpl>(resolver);
        }
    }

    public class UnityDiBinder<TInterface1, TInterface2, TInterface3> : DiBinder<TInterface1, TInterface2, TInterface3>
    {
        public UnityDiBinder(DiContainer dependencies) : base(dependencies) { }

        public DiContainer ToResource<TImpl>(string resourcePath) where TImpl : Object, TInterface1, TInterface2, TInterface3
        {
            var resolver = new ResourceInstanceResolver<TImpl>(resourcePath);
            return ToCustomResolver<TImpl>(resolver);
        }
    }

    public class UnityDiBinder<TInterface1, TInterface2, TInterface3, TInterface4> : DiBinder<TInterface1, TInterface2, TInterface3, TInterface4>
    {
        public UnityDiBinder(DiContainer dependencies) : base(dependencies) { }

        public DiContainer ToResource<TImpl>(string resourcePath) where TImpl : Object, TInterface1, TInterface2, TInterface3, TInterface4
        {
            var resolver = new ResourceInstanceResolver<TImpl>(resourcePath);
            return ToCustomResolver<TImpl>(resolver);
        }
    }

    public class UnityDiBinder<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5> : DiBinder<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5>
    {
        public UnityDiBinder(DiContainer dependencies) : base(dependencies) { }

        public DiContainer ToResource<TImpl>(string resourcePath) where TImpl : Object, TInterface1, TInterface2, TInterface3, TInterface4, TInterface5
        {
            var resolver = new ResourceInstanceResolver<TImpl>(resourcePath);
            return ToCustomResolver<TImpl>(resolver);
        }
    }
}