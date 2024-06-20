// Copyright (c) 2024 Eric Bennett McDuffee

using System;
using System.Collections.Generic;
using UJect.Assertions;
using UJect.Exceptions;
using UJect.Factories;
using UJect.Resolvers;
using UJect.Utilities;

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
            InstallBinding<TClass, TClass>(customId, new InstanceResolver<TClass>(instance));
            return this;
        }

        [LibraryEntryPoint]
        public bool Unbind<TType>(string customId = null)
        {
            var key = new InjectionKey(typeof(TType), customId);
            if (!dependencyResolvers.TryGetValue(key, out var resolver))
            {
                return false;
            }

            if (allKeyLookup.TryGetValue(resolver, out var keys))
            {
                throw new InvalidOperationException("Attempting to unbind single type of multi-key injected type!");
            }

            dependencyResolvers.Remove(key);
            resolvedInstances.Remove(key);
            return true;
        }

        #endregion Public Binding Interface

        #region Internal Binding Methods

        internal void InstallBinding<TFrom, TTo>(string customId, IResolver dependencyResolver) where TTo : TFrom
        {
            if (allKeyLookup.TryGetValue(dependencyResolver, out var keys)) throw new DuplicateBindingException(keys);
            var fromKey = new InjectionKey(typeof(TFrom), customId);
            var toKey = new InjectionKey(typeof(TTo));
            InstallBindingInternal(fromKey, toKey, dependencyResolver);
        }

        internal void InstallMultiBinding(InjectionKey toKey, IResolver dependencyResolver, params InjectionKey[] fromKeys)
        {
            if (allKeyLookup.TryGetValue(dependencyResolver, out var keys)) throw new DuplicateBindingException(keys);
            foreach (var fromKey in fromKeys)
            {
                InstallBindingInternal(fromKey, toKey, dependencyResolver);
            }
            allKeyLookup.Add(dependencyResolver, fromKeys);
        }

        internal void InstallFactoryBinding<TFrom, TTo>(string customId, IInstanceFactory<TTo> factory) where TTo : TFrom
        {
            var fromKey = new InjectionKey(typeof(TFrom), customId);

            var factoryIntKey = new InjectionKey(typeof(IInstanceFactory<TTo>), customId);
            var factoryKey = new InjectionKey(factory.GetType());

            var factoryResolver = new ExternalFactoryResolver<TTo>(factory, this);
            var instanceResolver = new ExternalFactoryResolver<TTo>(factory, this);

            var toKey = new InjectionKey(typeof(TTo));
            //Bind the interface to the concrete implementation
            InstallBindingInternal(fromKey, toKey, factoryResolver);
            //Bind the factory interface to the factory implementation
            InstallBindingInternal(factoryIntKey, factoryKey, instanceResolver);
            //Add a dependency on the factory interface to the interface. This will ensure the factory's dependencies are ready before the factory is used
            AddDependencies(fromKey, factoryIntKey);
        }

        internal void InstallFactoryMultiBind<TTo>(string customId, IInstanceFactory<TTo> factory, params InjectionKey[] fromKeys)
        {
            var factoryInterfaceKey = new InjectionKey(typeof(IInstanceFactory<TTo>), customId);
            var factoryKey = new InjectionKey(factory.GetType());
            var factoryResolver = new ExternalFactoryResolver<TTo>(factory, this);
            var instanceResolver = new ExternalFactoryResolver<TTo>(factory, this);
            var toKey = new InjectionKey(typeof(TTo));
            //Bind the interface to the concrete implementation
            InstallBindingInternal(factoryInterfaceKey, factoryKey, instanceResolver);
            foreach (var fromKey in fromKeys)
            {
                InstallBindingInternal(fromKey, toKey, factoryResolver);
                AddDependencies(fromKey, factoryInterfaceKey);
            }

            allKeyLookup[factoryResolver] = fromKeys;
        }

        private void InstallBindingInternal(InjectionKey fromKey, InjectionKey toKey, IResolver dependencyResolver)
        {
            AddDependencies(fromKey, toKey);

            if (!dependencyResolvers.TryAdd(fromKey, dependencyResolver))
            {
                throw new DuplicateBindingException(fromKey);
            }

            Phase = DiPhase.Bind;
        }

        #endregion Internal Binding Methods
    }
}