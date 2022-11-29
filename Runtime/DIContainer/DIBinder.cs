using System;
using UJect.Factories;
using UJect.Resolvers;
using Uject.Utilities;

namespace UJect
{
    public interface IDiBinder
    {
    }

    // public class DiBinder<TInterface> : IDiBinder<TInterface>
    // {
    //     private readonly DiContainer dependencies;
    //
    //     private string customId;
    //
    //     public DiBinder(DiContainer dependencies)
    //     {
    //         this.dependencies = dependencies;
    //     }
    //
    //     /// <summary>
    //     /// Bind the type resource with the custom ID provided. This allows disambiguating multiple resources of the same type
    //     /// </summary>
    //     /// <param name="id"></param>
    //     /// <returns>The same binder</returns>
    //     [LibraryEntryPoint]
    //     public IDiBinder<TInterface> WithId(string id)
    //     {
    //         customId = id;
    //         return this;
    //     }
    //
    //     /// <summary>
    //     /// Bind the given type to a provided concrete implementation instance of that type.
    //     /// </summary>
    //     /// <param name="instance"></param>
    //     /// <typeparam name="TImpl"></typeparam>
    //     /// <returns>The original container</returns>
    //     [LibraryEntryPoint]
    //     public DiContainer ToInstance<TImpl>(TImpl instance) where TImpl : TInterface
    //     {
    //         var resolver = new InstanceResolver<TImpl>(instance);
    //         dependencies.InstallBindingInternal<TInterface, TImpl>(customId, resolver);
    //         return dependencies;
    //     }
    //
    //     /// <summary>
    //     /// Bind the given type to a new instance of the provided concrete implementation of that type.
    //     /// </summary>
    //     /// <typeparam name="TImpl"></typeparam>
    //     /// <returns>The original container</returns>
    //     [LibraryEntryPoint]
    //     public DiContainer ToNewInstance<TImpl>() where TImpl : TInterface
    //     {
    //         var resolver = new NewInstanceResolver<TImpl>(dependencies);
    //         dependencies.InstallBindingInternal<TInterface, TImpl>(customId, resolver);
    //         return dependencies;
    //     }
    //
    //     /// <summary>
    //     /// Bind the given type to a function that will provide a concrete instance of that type
    //     /// </summary>
    //     /// <param name="factoryMethod"></param>
    //     /// <returns>The original container</returns>
    //     [LibraryEntryPoint]
    //     public DiContainer ToFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl : TInterface
    //     {
    //         var resolver = new FunctionInstanceResolver<TImpl>(factoryMethod);
    //         dependencies.InstallBindingInternal<TInterface, TImpl>(customId, resolver);
    //         return dependencies;
    //     }
    //
    //     /// <summary>
    //     /// Bind the given type to a factory implementation that will provide a concrete instance of that type.
    //     /// Factories can be injected into before resolution, making them useful when you want to use a bunch of injected resources to
    //     /// create the instance.
    //     /// </summary>
    //     /// <param name="factoryImpl"></param>
    //     /// <typeparam name="TImpl"></typeparam>
    //     /// <returns>The original container</returns>
    //     [LibraryEntryPoint]
    //     public DiContainer ToFactory<TImpl>(IInstanceFactory<TImpl> factoryImpl) where TImpl : TInterface
    //     {
    //         dependencies.InstallFactoryBinding<TInterface, TImpl>(customId, factoryImpl);
    //         return dependencies;
    //     }
    //
    //     /// <summary>
    //     /// Bind the given type to a custom resolver.
    //     /// </summary>
    //     /// <param name="customResolver"></param>
    //     /// <typeparam name="TImpl"></typeparam>
    //     /// <returns>The original container</returns>
    //     [LibraryEntryPoint]
    //     public DiContainer ToCustomResolver<TImpl>(IResolver customResolver) where TImpl : TInterface
    //     {
    //         dependencies.InstallBindingInternal<TInterface, TImpl>(customId, customResolver);
    //         return dependencies;
    //     }
    // }
}