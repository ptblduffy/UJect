// Copyright (c) 2022 Eric Bennett McDuffee

using UJect.Resolvers;
using UnityEngine;

namespace UJect.UnityExtensions
{
    public static class UnityBinderExtensions
    {
        public static UnityDiBinder<TInterface> UnityBind<TInterface>(this DiContainer diContainer) => new UnityDiBinder<TInterface>(diContainer);
    }

    public class UnityDiBinder<TInterface> : DiBinder<TInterface>
    {
        public UnityDiBinder(DiContainer dependencies) : base(dependencies) { }
        
        public DiContainer ToResource<TImpl>(string resourcePath) where TImpl: Object, TInterface
        {
            var resolver = new ResourceInstanceResolver<TImpl>(resourcePath);
            return ToCustomResolver<TImpl>(resolver);
        }
    }

    internal class ResourceInstanceResolver<TImpl> : ResolverBase<TImpl> where TImpl : Object
    {
        private readonly string resourcePath;

        public ResourceInstanceResolver(string resourcePath)
        {
            this.resourcePath = resourcePath;
        }

        public override IResolvedInstance<TImpl> ResolveTypedInstance()
        {
            var loadedResource = Resources.Load<TImpl>(resourcePath);
            return new UnityObjectResolvedInstance<TImpl>(loadedResource);
        }
    }
}