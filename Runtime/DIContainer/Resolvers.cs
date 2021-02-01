using System;
using System.Linq;

namespace UJect
{
    internal interface IResolver : IDisposable
    {
        object Resolve();
    }

    internal interface IResolver<TImpl> : IResolver
    {
        TImpl GetInstance();
    }
    
    internal class InstanceResolver<TImpl> : IResolver<TImpl>
    {
        private readonly TImpl       instance;

        public InstanceResolver(TImpl instance, DiContainer diContainer)
        {
            this.instance    = instance;
        }
        
        public TImpl GetInstance()
        {
            return instance;
        }

        object IResolver.Resolve() => GetInstance();

        public void Dispose()
        {
        }
    }
    
    internal class NewInstanceResolver<TImpl> : IResolver<TImpl>
    {
        private readonly DiContainer diContainer;

        public NewInstanceResolver(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }

        public TImpl GetInstance()
        {
            var injector = InjectorCache.GetInjector(typeof(TImpl));
            var instance = injector.Create<TImpl>(this.diContainer);
            return instance;
        }

        public void Dispose()
        {
            
        }

        public object Resolve() => GetInstance();
    }

    internal class FunctionInstanceResolver<TImpl> : IResolver<TImpl>
    {
        private readonly Func<TImpl> resolve;

        public FunctionInstanceResolver(Func<TImpl> resolve, DiContainer diContainer)
        {
            this.resolve     = resolve;
        }

        public TImpl GetInstance()
        {
            var newInstance = resolve.Invoke();
            return newInstance;
        }

        object IResolver.Resolve() => GetInstance();
        public void Dispose() { }
    }

    internal class ExternalFactoryResolver<TImpl> : IResolver<TImpl>
    {
        private readonly IInstanceFactory<TImpl> factory;

        public ExternalFactoryResolver(IInstanceFactory<TImpl> factory, DiContainer diContainer)
        {
            this.factory     = factory;
        }

        public TImpl GetInstance()
        {
            var newInstance = factory.CreateInstance();
            return newInstance;
        }

        object IResolver.Resolve() => GetInstance();

        public void Dispose()
        {
            factory.Dispose();
        }
    }
}