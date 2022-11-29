using UJect.Assertions;
using UJect.Exceptions;
using UJect.Factories;
using UJect.Resolvers;
using Uject.Utilities;

namespace UJect
{
    public sealed partial class DiContainer
    {
        #region Public Binding Interface

        [LibraryEntryPoint]
        public IDiBinder<TInterface1> Bind<TInterface1>()
        {
            RuntimeAssert.AssertIsFalse(isDisposed, "You should not try to bind to a disposed container!");
            return new DiBinder<TInterface1>(this);
        }

        [LibraryEntryPoint]
        public IDiBinder<TInterface1, TInterface2> Bind<TInterface1, TInterface2>()
        {
            RuntimeAssert.AssertIsFalse(isDisposed, "You should not try to bind to a disposed container!");
            return new DiBinder<TInterface1, TInterface2>(this);
        }

        [LibraryEntryPoint]
        public IDiBinder<TInterface1, TInterface2, TInterface3> Bind<TInterface1, TInterface2, TInterface3>()
        {
            RuntimeAssert.AssertIsFalse(isDisposed, "You should not try to bind to a disposed container!");
            return new DiBinder<TInterface1, TInterface2, TInterface3>(this);
        }

        [LibraryEntryPoint]
        public IDiBinder<TInterface1, TInterface2, TInterface3, TInterface4> Bind<TInterface1, TInterface2, TInterface3, TInterface4>()
        {
            RuntimeAssert.AssertIsFalse(isDisposed, "You should not try to bind to a disposed container!");
            return new DiBinder<TInterface1, TInterface2, TInterface3, TInterface4>(this);
        }

        [LibraryEntryPoint]
        public IDiBinder<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5> Bind<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5>()
        {
            RuntimeAssert.AssertIsFalse(isDisposed, "You should not try to bind to a disposed container!");
            return new DiBinder<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5>(this);
        }

        [LibraryEntryPoint]
        public DiContainer BindInstance<TClass>(TClass instance, string customId = null) where TClass : class
        {
            RuntimeAssert.AssertIsFalse(isDisposed, "You should not try to bind to a disposed container!");
            RuntimeAssert.AssertIsFalse(typeof(TClass).IsInterface, "You should not try to bind an instance of an interface!");
            InstallBindingInternal<TClass, TClass>(customId, new InstanceResolver<TClass>(instance));
            return this;
        }

        [LibraryEntryPoint]
        public bool Unbind<TType>(string customId = null)
        {
            var key = new InjectionKey(typeof(TType), customId);
            if (!dependencyResolvers.ContainsKey(key))
            {
                return false;
            }

            dependencyResolvers.Remove(key);
            resolvedInstances.Remove(key);
            return true;
        }

        #endregion Public Binding Interface

        #region Internal Binding Methods

        internal void InstallBindingInternal<TFrom, TTo>(string customId, IResolver dependencyResolver) where TTo : TFrom
        {
            var fromKey = new InjectionKey(typeof(TFrom), customId);
            var toKey = new InjectionKey(typeof(TTo));
            InstallBindingInternal(fromKey, toKey, dependencyResolver);
        }

        internal void InstallFactoryBinding<TFrom, TTo>(string customId, IInstanceFactory<TTo> factory)
        {
            var fromKey = new InjectionKey(typeof(TFrom), customId);
            var toKey = new InjectionKey(typeof(TTo));

            var factoryIntKey = new InjectionKey(typeof(IInstanceFactory<TTo>), customId);
            var factoryKey = new InjectionKey(factory.GetType());

            //Bind the interface to the concrete implementation
            InstallBindingInternal(fromKey, toKey, new ExternalFactoryResolver<TTo>(factory, this));
            //Bind the factory interface to the factory implementation
            InstallBindingInternal(factoryIntKey, factoryKey, new InstanceResolver<IInstanceFactory<TTo>>(factory));
            //Add a dependency on the factory interface to the interface. This will ensure the factory's dependencies are ready before the factory is used
            AddDependencies(fromKey, factoryIntKey);
        }

        private void InstallBindingInternal(InjectionKey fromKey, InjectionKey toKey, IResolver dependencyResolver)
        {
            AddDependencies(fromKey, toKey);

            if (dependencyResolvers.ContainsKey(fromKey))
            {
                throw new DuplicateBindingException(fromKey);
            }

            dependencyResolvers.Add(fromKey, dependencyResolver);

            Phase = DiPhase.Bind;
        }

        #endregion Internal Binding Methods
    }
}