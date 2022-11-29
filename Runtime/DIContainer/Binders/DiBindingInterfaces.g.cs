using System;
using UJect.Factories;
using UJect.Resolvers;
using Uject.Utilities;

namespace UJect
{

    public interface IDiBinder<in TInterface1> : IDiBinder
    {
        /// <summary>
        /// Bind the type resource with the custom ID provided. This allows disambiguating multiple resources of the same type
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The same binder</returns>
        [LibraryEntryPoint]
        IDiBinder<TInterface1> WithId(string id);
        
        /// <summary>
        /// Bind the given type to a provided concrete implementation instance of that type.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface1;
        
        /// <summary>
        /// Bind the given type to a new instance of the provided concrete implementation of that type.
        /// </summary>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToNewInstance<TImpl>() where TImpl : TInterface1;
        
        /// <summary>
        /// Bind the given type to a function that will provide a concrete instance of that type
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl : TInterface1;
        
        /// <summary>
        /// Bind the given type to a factory implementation that will provide a concrete instance of that type.
        /// Factories can be injected into before resolution, making them useful when you want to use a bunch of injected resources to
        /// create the instance.
        /// </summary>
        /// <param name="factoryImpl"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl : TInterface1;

        /// <summary>
        /// Bind the given type to a custom resolver.
        /// </summary>
        /// <param name="customResolver"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToCustomResolver<TImpl>(IResolver customResolver) where TImpl : TInterface1;
    }

    public interface IDiBinder<in TInterface1, in TInterface2> : IDiBinder
    {
        /// <summary>
        /// Bind the type resource with the custom ID provided. This allows disambiguating multiple resources of the same type
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The same binder</returns>
        [LibraryEntryPoint]
        IDiBinder<TInterface1, TInterface2> WithId(string id);
        
        /// <summary>
        /// Bind the given type to a provided concrete implementation instance of that type.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface1, TInterface2;
        
        /// <summary>
        /// Bind the given type to a new instance of the provided concrete implementation of that type.
        /// </summary>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToNewInstance<TImpl>() where TImpl : TInterface1, TInterface2;
        
        /// <summary>
        /// Bind the given type to a function that will provide a concrete instance of that type
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl : TInterface1, TInterface2;
        
        /// <summary>
        /// Bind the given type to a factory implementation that will provide a concrete instance of that type.
        /// Factories can be injected into before resolution, making them useful when you want to use a bunch of injected resources to
        /// create the instance.
        /// </summary>
        /// <param name="factoryImpl"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl : TInterface1, TInterface2;

        /// <summary>
        /// Bind the given type to a custom resolver.
        /// </summary>
        /// <param name="customResolver"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToCustomResolver<TImpl>(IResolver customResolver) where TImpl : TInterface1, TInterface2;
    }

    public interface IDiBinder<in TInterface1, in TInterface2, in TInterface3> : IDiBinder
    {
        /// <summary>
        /// Bind the type resource with the custom ID provided. This allows disambiguating multiple resources of the same type
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The same binder</returns>
        [LibraryEntryPoint]
        IDiBinder<TInterface1, TInterface2, TInterface3> WithId(string id);
        
        /// <summary>
        /// Bind the given type to a provided concrete implementation instance of that type.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface1, TInterface2, TInterface3;
        
        /// <summary>
        /// Bind the given type to a new instance of the provided concrete implementation of that type.
        /// </summary>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToNewInstance<TImpl>() where TImpl : TInterface1, TInterface2, TInterface3;
        
        /// <summary>
        /// Bind the given type to a function that will provide a concrete instance of that type
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl : TInterface1, TInterface2, TInterface3;
        
        /// <summary>
        /// Bind the given type to a factory implementation that will provide a concrete instance of that type.
        /// Factories can be injected into before resolution, making them useful when you want to use a bunch of injected resources to
        /// create the instance.
        /// </summary>
        /// <param name="factoryImpl"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl : TInterface1, TInterface2, TInterface3;

        /// <summary>
        /// Bind the given type to a custom resolver.
        /// </summary>
        /// <param name="customResolver"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToCustomResolver<TImpl>(IResolver customResolver) where TImpl : TInterface1, TInterface2, TInterface3;
    }

    public interface IDiBinder<in TInterface1, in TInterface2, in TInterface3, in TInterface4> : IDiBinder
    {
        /// <summary>
        /// Bind the type resource with the custom ID provided. This allows disambiguating multiple resources of the same type
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The same binder</returns>
        [LibraryEntryPoint]
        IDiBinder<TInterface1, TInterface2, TInterface3, TInterface4> WithId(string id);
        
        /// <summary>
        /// Bind the given type to a provided concrete implementation instance of that type.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface1, TInterface2, TInterface3, TInterface4;
        
        /// <summary>
        /// Bind the given type to a new instance of the provided concrete implementation of that type.
        /// </summary>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToNewInstance<TImpl>() where TImpl : TInterface1, TInterface2, TInterface3, TInterface4;
        
        /// <summary>
        /// Bind the given type to a function that will provide a concrete instance of that type
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl : TInterface1, TInterface2, TInterface3, TInterface4;
        
        /// <summary>
        /// Bind the given type to a factory implementation that will provide a concrete instance of that type.
        /// Factories can be injected into before resolution, making them useful when you want to use a bunch of injected resources to
        /// create the instance.
        /// </summary>
        /// <param name="factoryImpl"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl : TInterface1, TInterface2, TInterface3, TInterface4;

        /// <summary>
        /// Bind the given type to a custom resolver.
        /// </summary>
        /// <param name="customResolver"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToCustomResolver<TImpl>(IResolver customResolver) where TImpl : TInterface1, TInterface2, TInterface3, TInterface4;
    }

    public interface IDiBinder<in TInterface1, in TInterface2, in TInterface3, in TInterface4, in TInterface5> : IDiBinder
    {
        /// <summary>
        /// Bind the type resource with the custom ID provided. This allows disambiguating multiple resources of the same type
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The same binder</returns>
        [LibraryEntryPoint]
        IDiBinder<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5> WithId(string id);
        
        /// <summary>
        /// Bind the given type to a provided concrete implementation instance of that type.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface1, TInterface2, TInterface3, TInterface4, TInterface5;
        
        /// <summary>
        /// Bind the given type to a new instance of the provided concrete implementation of that type.
        /// </summary>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToNewInstance<TImpl>() where TImpl : TInterface1, TInterface2, TInterface3, TInterface4, TInterface5;
        
        /// <summary>
        /// Bind the given type to a function that will provide a concrete instance of that type
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl : TInterface1, TInterface2, TInterface3, TInterface4, TInterface5;
        
        /// <summary>
        /// Bind the given type to a factory implementation that will provide a concrete instance of that type.
        /// Factories can be injected into before resolution, making them useful when you want to use a bunch of injected resources to
        /// create the instance.
        /// </summary>
        /// <param name="factoryImpl"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl : TInterface1, TInterface2, TInterface3, TInterface4, TInterface5;

        /// <summary>
        /// Bind the given type to a custom resolver.
        /// </summary>
        /// <param name="customResolver"></param>
        /// <typeparam name="TImpl"></typeparam>
        /// <returns>The original container</returns>
        [LibraryEntryPoint]
        DiContainer ToCustomResolver<TImpl>(IResolver customResolver) where TImpl : TInterface1, TInterface2, TInterface3, TInterface4, TInterface5;
    }
}