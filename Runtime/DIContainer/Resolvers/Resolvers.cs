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

    internal class NewInstanceResolver<TImpl> : ResolverBase<TImpl>
    {
        private readonly DiContainer diContainer;

        public NewInstanceResolver(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }

        public override IResolvedInstance<TImpl> ResolveTypedInstance()
        {
            var injector = InjectorCache.GetOrCreateInjector(typeof(TImpl));
            var newInstance = injector.CreateInstance<TImpl>(diContainer);
            return new PocoResolvedInstance<TImpl>(newInstance);
        }
    }

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