using System;
using Object = UnityEngine.Object;

namespace UJect
{
    public interface IDiBinder<TInterface>
    {
        IDiBinder<TInterface> WithId(string id);
        DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface;
        DiContainer ToNewInstance<TImpl>() where TImpl : TInterface;
        DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl : TInterface;
        DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl : TInterface;

        DiContainer ToResource<TImpl>(string resourcePath) where TImpl : UnityEngine.Object, TInterface;
    }

    internal class DiBinder<TInterface> : IDiBinder<TInterface>
    {
        private readonly DiContainer dependencies;

        private string customId = null;

        public DiBinder(DiContainer dependencies)
        {
            this.dependencies = dependencies;
        }

        public IDiBinder<TInterface> WithId(string id)
        {
            this.customId = id;
            return this;
        }

        public DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface
        {
            var resolver = new InstanceResolver<TImpl>(instance);
            dependencies.InstallBindingInternal<TInterface, TImpl>(customId, resolver);
            return dependencies;
        }

        public DiContainer ToNewInstance<TImpl>() where TImpl : TInterface
        {
            var resolver = new NewInstanceResolver<TImpl>(dependencies);
            dependencies.InstallBindingInternal<TInterface, TImpl>(customId, resolver);
            return dependencies;
        }

        public DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl : TInterface
        {
            var resolver = new FunctionInstanceResolver<TImpl>(factoryMethod);
            dependencies.InstallBindingInternal<TInterface, TImpl>(customId, resolver);
            return dependencies;
        }

        public DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl : TInterface
        {
            var resolver = new ExternalFactoryResolver<TImpl>(factoryImpl);
            dependencies.InstallBindingInternal<TInterface, TImpl>(customId, resolver);
            return dependencies;
        }

        public DiContainer ToResource<TImpl>(string resourcePath) where TImpl : UnityEngine.Object, TInterface
        {
            var resolver = new ResourceInstanceResolver<TImpl>(resourcePath);
            dependencies.InstallBindingInternal<TInterface, TImpl>(customId, resolver);
            return dependencies;
        }
    }
}