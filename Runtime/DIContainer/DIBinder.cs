using System;
using UJect.Factories;
using UJect.Resolvers;
using Uject.Utilities;
using Object = UnityEngine.Object;

namespace UJect
{
    public interface IDiBinder<in TInterface>
    {
        /// <summary>
        /// Bind the type resource with the custom ID provided. This allows disambiguating multiple resources of the same type
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The same binder</returns>
        [LibraryEntryPoint]
        IDiBinder<TInterface> WithId(string id);

        /// <summary>
        /// Bind the type as an unshared instance (meaning every access to a given interface will get a separate implementation instance)
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The same binder</returns>
        [LibraryEntryPoint]
        IDiBinder<TInterface> AsUnsharedInstance();

        
        /// <summary>
        /// Bind the given type to a provided concrete implementation instance of that type.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface;
        
        /// <summary>
        /// Bind the given type to a new instance of the provided concrete implementation of that type.
        /// </summary>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToNewInstance<TImpl>() where TImpl : TInterface;
        
        /// <summary>
        /// Bind the given type to a function that will provide a concrete instance of that type
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl : TInterface;
        
        /// <summary>
        /// Bind the given type to a factory implementation that will provide a concrete instance of that type.
        /// Factories can be injected into before resolution, making them useful when you want to use a bunch of injected resources to
        /// create the instance.
        /// </summary>
        /// <param name="factoryImpl"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl : TInterface;

        /// <summary>
        /// Bind the given type to a Unity resource at the provided path. That resource will be loaded via Resource.Load&lt;TImpl&gt;(resourcePath);
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToResource<TImpl>(string resourcePath) where TImpl : Object, TInterface;

        /// <summary>
        /// Bind the given type to a custom resolver.
        /// </summary>
        /// <param name="customResolver"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToCustomResolver<TImpl>(IResolver<TImpl> customResolver) where TImpl : TInterface;
    }

    internal class DiBinder<TInterface> : IDiBinder<TInterface>
    {
        private readonly DiContainer dependencies;

        private string customId;
        private bool   isSharedImplementationInstance = true;

        public DiBinder(DiContainer dependencies)
        {
            this.dependencies = dependencies;
        }

        /// <summary>
        /// Bind the type resource with the custom ID provided. This allows disambiguating multiple resources of the same type
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The same binder</returns>
        [LibraryEntryPoint]
        public IDiBinder<TInterface> WithId(string id)
        {
            customId = id;
            return this;
        }

        /// <summary>
        /// Bind the type as an unshared instance (meaning every access to a given interface will get a separate implementation instance)
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The same binder</returns>
        [LibraryEntryPoint]
        public IDiBinder<TInterface> AsUnsharedInstance()
        {
            isSharedImplementationInstance = false;
            return this;
        }
        
        /// <summary>
        /// Bind the given type to a provided concrete implementation instance of that type.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface
        {
            var resolver = WrapResolverForSharedInstanceIfNecessary(new InstanceResolver<TImpl>(instance));
            dependencies.InstallBindingInternal<TInterface, TImpl>(customId, resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a new instance of the provided concrete implementation of that type.
        /// </summary>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToNewInstance<TImpl>() where TImpl : TInterface
        {
            var resolver = WrapResolverForSharedInstanceIfNecessary(new NewInstanceResolver<TImpl>(dependencies));
            dependencies.InstallBindingInternal<TInterface, TImpl>(customId, resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a function that will provide a concrete instance of that type
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl : TInterface
        {
            var resolver = WrapResolverForSharedInstanceIfNecessary(new FunctionInstanceResolver<TImpl>(factoryMethod));
            dependencies.InstallBindingInternal<TInterface, TImpl>(customId, resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a factory implementation that will provide a concrete instance of that type.
        /// Factories can be injected into before resolution, making them useful when you want to use a bunch of injected resources to
        /// create the instance.
        /// </summary>
        /// <param name="factoryImpl"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl : TInterface
        {
            var fromKey = new InjectionKey(typeof(TInterface), customId);
            var toKey = new InjectionKey(typeof(TImpl));

            var factoryIntKey = new InjectionKey(typeof(IInstanceFactory<TImpl>), customId);
            var factoryKey = new InjectionKey(factoryImpl.GetType());

            //Bind the interface to the concrete implementation
            dependencies.InstallBindingInternal(fromKey, toKey, WrapResolverForSharedInstanceIfNecessary(new ExternalFactoryResolver<TImpl>(factoryImpl, dependencies)));
            //Bind the factory interface to the factory implementation
            dependencies.InstallBindingInternal(factoryIntKey, factoryKey, new InstanceResolver<IInstanceFactory<TImpl>>(factoryImpl));
            //Add a dependency on the factory interface to the interface. This will ensure the factory's dependencies are ready before the factory is used
            dependencies.AddDependencies(fromKey, factoryIntKey);

            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a Unity resource at the provided path. That resource will be loaded via Resource.Load<TImpl>(resourcePath);
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToResource<TImpl>(string resourcePath) where TImpl : Object, TInterface
        {
            var resolver = WrapResolverForSharedInstanceIfNecessary(new ResourceInstanceResolver<TImpl>(resourcePath));
            dependencies.InstallBindingInternal<TInterface, TImpl>(customId, resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a custom resolver.
        /// </summary>
        /// <param name="customResolver"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToCustomResolver<TImpl>(IResolver<TImpl> customResolver) where TImpl : TInterface
        {
            var resolver = WrapResolverForSharedInstanceIfNecessary(customResolver);
            dependencies.InstallBindingInternal<TInterface, TImpl>(customId, resolver);
            return dependencies;
        }

        /// <summary>
        /// If we're going to use a Shared Instance, wrap the internal resolver (which may get a new instance every time) in a SharedInstanceResolver, which will look for an already
        /// resolved instance before invoking the internal resolver
        /// </summary>
        /// <param name="originalResolver"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns></returns>
        private IResolver<TImpl> WrapResolverForSharedInstanceIfNecessary<TImpl>(IResolver<TImpl> originalResolver)
        {
            var resolver = originalResolver;
            if (isSharedImplementationInstance)
            {
                resolver = new SharedInstanceResolver<TImpl>(dependencies, customId, resolver);
            }

            return resolver;
        }
    }
}