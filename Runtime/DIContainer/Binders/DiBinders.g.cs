using System;
using UJect.Factories;
using UJect.Resolvers;
using UJect.Utilities;

namespace UJect
{

    public class DiBinder<TInterface1> : IDiBinder<TInterface1>
    {
        private readonly DiContainer dependencies;

        private string customId;

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
        public IDiBinder<TInterface1> WithId(string id)
        {
            customId = id;
            return this;
        }

        /// <summary>
        /// Bind the given type to a provided concrete implementation instance of that type.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface1
        {
            var resolver = new InstanceResolver<TImpl>(instance);
            InstallBindings<TImpl>(resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a new instance of the provided concrete implementation of that type.
        /// </summary>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToNewInstance<TImpl>() where TImpl :  TInterface1
        {
            var resolver = new NewInstanceResolver<TImpl>(dependencies);
            InstallBindings<TImpl>(resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a function that will provide a concrete instance of that type
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl :  TInterface1
        {
            var resolver = new FunctionInstanceResolver<TImpl>(factoryMethod);
            InstallBindings<TImpl>(resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a custom resolver.
        /// </summary>
        /// <param name="customResolver"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToCustomResolver<TImpl>(IResolver customResolver) where TImpl :  TInterface1
        {
            InstallBindings<TImpl>(customResolver);
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
        public DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl :  TInterface1
        {
            dependencies.InstallFactoryBinding<TInterface1, TImpl>(customId, factoryImpl);
            return dependencies;
        }

        private void InstallBindings<TImpl>(IResolver resolver) where TImpl :  TInterface1
        {
            dependencies.InstallBindingInternal<TInterface1, TImpl>(customId, resolver);
        }
    }

    public class DiBinder<TInterface1, TInterface2> : IDiBinder<TInterface1, TInterface2>
    {
        private readonly DiContainer dependencies;

        private string customId;

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
        public IDiBinder<TInterface1, TInterface2> WithId(string id)
        {
            customId = id;
            return this;
        }

        /// <summary>
        /// Bind the given type to a provided concrete implementation instance of that type.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface1, TInterface2
        {
            var resolver = new InstanceResolver<TImpl>(instance);
            InstallBindings<TImpl>(resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a new instance of the provided concrete implementation of that type.
        /// </summary>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToNewInstance<TImpl>() where TImpl :  TInterface1, TInterface2
        {
            var resolver = new NewInstanceResolver<TImpl>(dependencies);
            InstallBindings<TImpl>(resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a function that will provide a concrete instance of that type
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl :  TInterface1, TInterface2
        {
            var resolver = new FunctionInstanceResolver<TImpl>(factoryMethod);
            InstallBindings<TImpl>(resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a custom resolver.
        /// </summary>
        /// <param name="customResolver"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToCustomResolver<TImpl>(IResolver customResolver) where TImpl :  TInterface1, TInterface2
        {
            InstallBindings<TImpl>(customResolver);
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
        public DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl :  TInterface1, TInterface2
        {
            dependencies.InstallFactoryBinding<TInterface1, TImpl>(customId, factoryImpl);
            dependencies.InstallFactoryBinding<TInterface2, TImpl>(customId, factoryImpl);
            return dependencies;
        }

        private void InstallBindings<TImpl>(IResolver resolver) where TImpl :  TInterface1, TInterface2
        {
            dependencies.InstallBindingInternal<TInterface1, TImpl>(customId, resolver);
            dependencies.InstallBindingInternal<TInterface2, TImpl>(customId, resolver);
        }


    }

    public class DiBinder<TInterface1, TInterface2, TInterface3> : IDiBinder<TInterface1, TInterface2, TInterface3>
    {
        private readonly DiContainer dependencies;

        private string customId;

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
        public IDiBinder<TInterface1, TInterface2, TInterface3> WithId(string id)
        {
            customId = id;
            return this;
        }

        /// <summary>
        /// Bind the given type to a provided concrete implementation instance of that type.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface1, TInterface2, TInterface3
        {
            var resolver = new InstanceResolver<TImpl>(instance);
            InstallBindings<TImpl>(resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a new instance of the provided concrete implementation of that type.
        /// </summary>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToNewInstance<TImpl>() where TImpl :  TInterface1, TInterface2, TInterface3
        {
            var resolver = new NewInstanceResolver<TImpl>(dependencies);
            InstallBindings<TImpl>(resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a function that will provide a concrete instance of that type
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl :  TInterface1, TInterface2, TInterface3
        {
            var resolver = new FunctionInstanceResolver<TImpl>(factoryMethod);
            InstallBindings<TImpl>(resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a custom resolver.
        /// </summary>
        /// <param name="customResolver"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToCustomResolver<TImpl>(IResolver customResolver) where TImpl :  TInterface1, TInterface2, TInterface3
        {
            InstallBindings<TImpl>(customResolver);
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
        public DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl :  TInterface1, TInterface2, TInterface3
        {
            dependencies.InstallFactoryBinding<TInterface1, TImpl>(customId, factoryImpl);
            dependencies.InstallFactoryBinding<TInterface2, TImpl>(customId, factoryImpl);
            dependencies.InstallFactoryBinding<TInterface3, TImpl>(customId, factoryImpl);
            return dependencies;
        }

        private void InstallBindings<TImpl>(IResolver resolver) where TImpl :  TInterface1, TInterface2, TInterface3
        {
            dependencies.InstallBindingInternal<TInterface1, TImpl>(customId, resolver);
            dependencies.InstallBindingInternal<TInterface2, TImpl>(customId, resolver);
            dependencies.InstallBindingInternal<TInterface3, TImpl>(customId, resolver);
        }


    }

    public class DiBinder<TInterface1, TInterface2, TInterface3, TInterface4> : IDiBinder<TInterface1, TInterface2, TInterface3, TInterface4>
    {
        private readonly DiContainer dependencies;

        private string customId;

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
        public IDiBinder<TInterface1, TInterface2, TInterface3, TInterface4> WithId(string id)
        {
            customId = id;
            return this;
        }

        /// <summary>
        /// Bind the given type to a provided concrete implementation instance of that type.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface1, TInterface2, TInterface3, TInterface4
        {
            var resolver = new InstanceResolver<TImpl>(instance);
            InstallBindings<TImpl>(resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a new instance of the provided concrete implementation of that type.
        /// </summary>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToNewInstance<TImpl>() where TImpl :  TInterface1, TInterface2, TInterface3, TInterface4
        {
            var resolver = new NewInstanceResolver<TImpl>(dependencies);
            InstallBindings<TImpl>(resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a function that will provide a concrete instance of that type
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl :  TInterface1, TInterface2, TInterface3, TInterface4
        {
            var resolver = new FunctionInstanceResolver<TImpl>(factoryMethod);
            InstallBindings<TImpl>(resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a custom resolver.
        /// </summary>
        /// <param name="customResolver"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToCustomResolver<TImpl>(IResolver customResolver) where TImpl :  TInterface1, TInterface2, TInterface3, TInterface4
        {
            InstallBindings<TImpl>(customResolver);
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
        public DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl :  TInterface1, TInterface2, TInterface3, TInterface4
        {
            dependencies.InstallFactoryBinding<TInterface1, TImpl>(customId, factoryImpl);
            dependencies.InstallFactoryBinding<TInterface2, TImpl>(customId, factoryImpl);
            dependencies.InstallFactoryBinding<TInterface3, TImpl>(customId, factoryImpl);
            dependencies.InstallFactoryBinding<TInterface4, TImpl>(customId, factoryImpl);
            return dependencies;
        }

        private void InstallBindings<TImpl>(IResolver resolver) where TImpl :  TInterface1, TInterface2, TInterface3, TInterface4
        {
            dependencies.InstallBindingInternal<TInterface1, TImpl>(customId, resolver);
            dependencies.InstallBindingInternal<TInterface2, TImpl>(customId, resolver);
            dependencies.InstallBindingInternal<TInterface3, TImpl>(customId, resolver);
            dependencies.InstallBindingInternal<TInterface4, TImpl>(customId, resolver);
        }


    }

    public class DiBinder<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5> : IDiBinder<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5>
    {
        private readonly DiContainer dependencies;

        private string customId;

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
        public IDiBinder<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5> WithId(string id)
        {
            customId = id;
            return this;
        }

        /// <summary>
        /// Bind the given type to a provided concrete implementation instance of that type.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface1, TInterface2, TInterface3, TInterface4, TInterface5
        {
            var resolver = new InstanceResolver<TImpl>(instance);
            InstallBindings<TImpl>(resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a new instance of the provided concrete implementation of that type.
        /// </summary>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToNewInstance<TImpl>() where TImpl :  TInterface1, TInterface2, TInterface3, TInterface4, TInterface5
        {
            var resolver = new NewInstanceResolver<TImpl>(dependencies);
            InstallBindings<TImpl>(resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a function that will provide a concrete instance of that type
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl :  TInterface1, TInterface2, TInterface3, TInterface4, TInterface5
        {
            var resolver = new FunctionInstanceResolver<TImpl>(factoryMethod);
            InstallBindings<TImpl>(resolver);
            return dependencies;
        }

        /// <summary>
        /// Bind the given type to a custom resolver.
        /// </summary>
        /// <param name="customResolver"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        public DiContainer ToCustomResolver<TImpl>(IResolver customResolver) where TImpl :  TInterface1, TInterface2, TInterface3, TInterface4, TInterface5
        {
            InstallBindings<TImpl>(customResolver);
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
        public DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl :  TInterface1, TInterface2, TInterface3, TInterface4, TInterface5
        {
            dependencies.InstallFactoryBinding<TInterface1, TImpl>(customId, factoryImpl);
            dependencies.InstallFactoryBinding<TInterface2, TImpl>(customId, factoryImpl);
            dependencies.InstallFactoryBinding<TInterface3, TImpl>(customId, factoryImpl);
            dependencies.InstallFactoryBinding<TInterface4, TImpl>(customId, factoryImpl);
            dependencies.InstallFactoryBinding<TInterface5, TImpl>(customId, factoryImpl);
            return dependencies;
        }

        private void InstallBindings<TImpl>(IResolver resolver) where TImpl :  TInterface1, TInterface2, TInterface3, TInterface4, TInterface5
        {
            dependencies.InstallBindingInternal<TInterface1, TImpl>(customId, resolver);
            dependencies.InstallBindingInternal<TInterface2, TImpl>(customId, resolver);
            dependencies.InstallBindingInternal<TInterface3, TImpl>(customId, resolver);
            dependencies.InstallBindingInternal<TInterface4, TImpl>(customId, resolver);
            dependencies.InstallBindingInternal<TInterface5, TImpl>(customId, resolver);
        }


    }
}