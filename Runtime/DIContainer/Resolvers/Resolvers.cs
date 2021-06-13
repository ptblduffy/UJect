using System;
using UJect.Factories;
using UJect.Injection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UJect.Resolvers
{

    internal class SharedInstanceResolver<TImpl> : ResolverBase<TImpl>
    {
        private readonly DiContainer      diContainer;
        private readonly IResolver<TImpl> innerResolver;
        private readonly InjectionKey     sharedInstanceKey;

        public SharedInstanceResolver(DiContainer diContainer, string injectionId, IResolver<TImpl> innerResolver)
        {
            this.diContainer   = diContainer;
            this.innerResolver = innerResolver;
            this.sharedInstanceKey = new InjectionKey(typeof(TImpl), injectionId);
        }

        public override TImpl ResolveTypedInstance()
        {
            Debug.Log($"Resolving shared instance for key: {sharedInstanceKey}");
            var instance = diContainer.SharedInstanceCache.GetOrCreateSharedInstance(sharedInstanceKey, () => innerResolver.ResolveTypedInstance());
            return instance;
        }

        public override void Dispose()
        {
            base.Dispose();
            diContainer.SharedInstanceCache.ReleaseBinding(sharedInstanceKey);
        }
    }
    
    /// <summary>
    /// Simplest resolver. Always returns the same provided instance of TImpl
    /// </summary>
    /// <typeparam name="TImpl"></typeparam>
    internal class InstanceResolver<TImpl> : ResolverBase<TImpl>
    {
        private readonly TImpl instance;

        public InstanceResolver(TImpl instance)
        {
            this.instance = instance;
        }

        public override TImpl ResolveTypedInstance() => instance;
    }

    internal class NewInstanceResolver<TImpl> : ResolverBase<TImpl>
    {
        private readonly DiContainer diContainer;

        public NewInstanceResolver(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }

        public override TImpl ResolveTypedInstance()
        {
            var injector = InjectorCache.GetOrCreateInjector(typeof(TImpl));
            var instance = injector.CreateInstance<TImpl>(diContainer);
            return instance;
        }
    }

    internal class FunctionInstanceResolver<TImpl> : ResolverBase<TImpl>
    {
        private readonly Func<TImpl> resolve;

        public FunctionInstanceResolver(Func<TImpl> resolve)
        {
            this.resolve = resolve;
        }

        public override TImpl ResolveTypedInstance()
        {
            var newInstance = resolve.Invoke();
            return newInstance;
        }
    }

    internal class ExternalFactoryResolver<TImpl> : ResolverBase<TImpl>
    {
        private readonly DiContainer             diContainer;
        private readonly IInstanceFactory<TImpl> factory;

        public ExternalFactoryResolver(IInstanceFactory<TImpl> factory, DiContainer diContainer)
        {
            this.factory     = factory;
            this.diContainer = diContainer;
        }

        public override TImpl ResolveTypedInstance()
        {
            diContainer.InjectInto(factory);
            var newInstance = factory.CreateInstance();
            return newInstance;
        }
    }

    internal class ResourceInstanceResolver<TImpl> : ResolverBase<TImpl> where TImpl : Object
    {
        private readonly string resourcePath;

        public ResourceInstanceResolver(string resourcePath)
        {
            this.resourcePath = resourcePath;
        }

        public override TImpl ResolveTypedInstance()
        {
            var loadedResource = Resources.Load<TImpl>(resourcePath);
            return loadedResource;
        }
    }
}