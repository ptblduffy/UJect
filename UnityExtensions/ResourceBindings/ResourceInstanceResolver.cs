// Copyright (c) 2024 Eric Bennett McDuffee

using UJect.Resolvers;
using UnityEngine;

namespace UJect.UnityExtensions
{
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