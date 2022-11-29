using System;
using UJect.Factories;
using UJect.Injection;

namespace UJect.Resolvers
{
    /// <summary>
    /// Simplest resolver. Always returns the same provided instance of TImpl
    /// </summary>
    /// <typeparam name="TImpl"></typeparam>
    internal class InstanceResolver<TImpl> : ResolverBase<TImpl>
    {
        private readonly IResolvedInstance<TImpl> resolvedInstance;

        public InstanceResolver(TImpl instance)
        {
            this.resolvedInstance = new PocoResolvedInstance<TImpl>(instance);
        }

        public override IResolvedInstance<TImpl> ResolveTypedInstance() => resolvedInstance;
    }

    /// <summary>
    /// Resolves by getting a new instance if one has already been created, or creating a new one.
    /// Useful for resolving the same instance for multiple interface bindings
    /// </summary>
    /// <typeparam name="TImpl"></typeparam>
    internal class NewInstanceResolver<TImpl> : ResolverBase<TImpl>
    {
        private readonly DiContainer diContainer;
        private PocoResolvedInstance<TImpl> resolvedNewInstance;
        private bool hasResolved;

        public NewInstanceResolver(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }

        public override IResolvedInstance<TImpl> ResolveTypedInstance()
        {
            if (!hasResolved)
            {
                hasResolved = true;
                var injector = InjectorCache.GetOrCreateInjector(typeof(TImpl));
                var instanceObject = injector.CreateInstance<TImpl>(diContainer);
                resolvedNewInstance = new PocoResolvedInstance<TImpl>(instanceObject);
            }
            return resolvedNewInstance;
        }
    }

    /// <summary>
    /// Resolve an instance via a Func
    /// </summary>
    /// <typeparam name="TImpl"></typeparam>
    internal class FunctionInstanceResolver<TImpl> : ResolverBase<TImpl>
    {
        private readonly Func<TImpl> resolve;

        public FunctionInstanceResolver(Func<TImpl> resolve)
        {
            this.resolve = resolve;
        }

        public override IResolvedInstance<TImpl> ResolveTypedInstance()
        {
            var newInstance = resolve.Invoke();
            return new PocoResolvedInstance<TImpl>(newInstance);
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

        public override IResolvedInstance<TImpl> ResolveTypedInstance()
        {
            diContainer.InjectInto(factory);
            var newInstance = factory.CreateInstance();
            return new PocoResolvedInstance<TImpl>(newInstance);
        }
    }
}