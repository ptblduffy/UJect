using System;
using UJect.Factories;
using UJect.Resolvers;
using Object = UnityEngine.Object;

namespace UJect
{
    public interface IDiBinder<in TInterface>
    {
        /// <summary>
        /// Bind the type resource with the custom ID provided. This allows disambiguating multiple resources of the same type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IDiBinder<TInterface> WithId(string id);
        
        /// <summary>
        /// Bind the given type to a provided concrete implementation instance of that type.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns></returns>
        DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface;
        
        /// <summary>
        /// Bind the given type to a new instance of the provided concrete implementation of that type.
        /// </summary>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns></returns>
        DiContainer ToNewInstance<TImpl>() where TImpl : TInterface;
        
        /// <summary>
        /// Bind the given type to a function that will provide a concrete instance of that type
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns></returns>
        DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl : TInterface;
        
        /// <summary>
        /// Bind the given type to a factory implementation that will provide a concrete instance of that type.
        /// Factories can be injected into before resolution, making them useful when you want to use a bunch of injected resources to
        /// create the instance.
        /// </summary>
        /// <param name="factoryImpl"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns></returns>
        DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl : TInterface;

        /// <summary>
        /// Bind the given type to a Unity resource at the provided path. That resource will be loaded via Resource.Load<TImpl>(resourcePath);
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns></returns>
        DiContainer ToResource<TImpl>(string resourcePath) where TImpl : Object, TInterface;
    }

    internal class DiBinder<TInterface> : IDiBinder<TInterface>
    {
        private readonly DiContainer dependencies;

        private string customId;

        public DiBinder(DiContainer dependencies)
        {
            this.dependencies = dependencies;
        }

        public IDiBinder<TInterface> WithId(string id)
        {
            customId = id;
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
            dependencies.InstallFactoryBinding<TInterface, TImpl>(customId, factoryImpl);
            return dependencies;
        }

        public DiContainer ToResource<TImpl>(string resourcePath) where TImpl : Object, TInterface
        {
            var resolver = new ResourceInstanceResolver<TImpl>(resourcePath);
            dependencies.InstallBindingInternal<TInterface, TImpl>(customId, resolver);
            return dependencies;
        }

        public DiContainer ToCustomResolver<TImpl>(IResolver<TImpl> customResolver) where TImpl : TInterface
        {
            dependencies.InstallBindingInternal<TInterface, TImpl>(customId, customResolver);
            return dependencies;
        }
    }
}