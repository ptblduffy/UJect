// Copyright (c) 2024 Eric Bennett McDuffee

using System;
using System.Collections.Generic;
using NUnit.Framework;
using UJect.Exceptions;
using UJect.Factories;
using UJect.Injection;

namespace UJect.Tests
{
    [TestFixture]
    public partial class MultiBindTests
    {


        private static IEnumerable<Action<IDiBinder<IInterface1, IInterface2>>> BindMethods2
        {
            get
            {
                yield return binder => binder.ToNewInstance<Impl12>();
                yield return binder => binder.ToFactoryMethod(()=>new Impl12());
                yield return binder => binder.ToFactory(new Impl12Factory());
                yield return binder => binder.ToInstance(new Impl12());
            }
        }

        [Test]
        [TestCaseSource(nameof(BindMethods2))]
        public void TestBindingMultipleInterfaces(Action<IDiBinder<IInterface1, IInterface2>> bindMethod)
        {
            var container = new DiContainer();

            var binder = container.Bind<IInterface1, IInterface2>();
            bindMethod(binder);
            container.TryResolveAll();

            // IInterface1
            var instance1 = container.Get<IInterface1>();
            Assert.IsNotNull(instance1);
            Assert.AreEqual(typeof(Impl12), instance1.GetType());
            // IInterface2
            var instance2 = container.Get<IInterface2>();
            Assert.IsNotNull(instance2);
            Assert.AreEqual(typeof(Impl12), instance2.GetType());
            Assert.IsTrue(object.ReferenceEquals(instance1, instance2), "Should be using the same reference");
        }
    
        private static IEnumerable<Action<IDiBinder<IInterface1, IInterface2, IInterface3>>> BindMethods3
        {
            get
            {
                yield return binder => binder.ToNewInstance<Impl123>();
                yield return binder => binder.ToFactoryMethod(()=>new Impl123());
                yield return binder => binder.ToFactory(new Impl123Factory());
                yield return binder => binder.ToInstance(new Impl123());
            }
        }

        [Test]
        [TestCaseSource(nameof(BindMethods3))]
        public void TestBindingMultipleInterfaces(Action<IDiBinder<IInterface1, IInterface2, IInterface3>> bindMethod)
        {
            var container = new DiContainer();

            var binder = container.Bind<IInterface1, IInterface2, IInterface3>();
            bindMethod(binder);
            container.TryResolveAll();

            // IInterface1
            var instance1 = container.Get<IInterface1>();
            Assert.IsNotNull(instance1);
            Assert.AreEqual(typeof(Impl123), instance1.GetType());
            // IInterface2
            var instance2 = container.Get<IInterface2>();
            Assert.IsNotNull(instance2);
            Assert.AreEqual(typeof(Impl123), instance2.GetType());
            Assert.IsTrue(object.ReferenceEquals(instance1, instance2), "Should be using the same reference");
            // IInterface3
            var instance3 = container.Get<IInterface3>();
            Assert.IsNotNull(instance3);
            Assert.AreEqual(typeof(Impl123), instance3.GetType());
            Assert.IsTrue(object.ReferenceEquals(instance2, instance3), "Should be using the same reference");
        }
    
        private static IEnumerable<Action<IDiBinder<IInterface1, IInterface2, IInterface3, IInterface4>>> BindMethods4
        {
            get
            {
                yield return binder => binder.ToNewInstance<Impl1234>();
                yield return binder => binder.ToFactoryMethod(()=>new Impl1234());
                yield return binder => binder.ToFactory(new Impl1234Factory());
                yield return binder => binder.ToInstance(new Impl1234());
            }
        }

        [Test]
        [TestCaseSource(nameof(BindMethods4))]
        public void TestBindingMultipleInterfaces(Action<IDiBinder<IInterface1, IInterface2, IInterface3, IInterface4>> bindMethod)
        {
            var container = new DiContainer();

            var binder = container.Bind<IInterface1, IInterface2, IInterface3, IInterface4>();
            bindMethod(binder);
            container.TryResolveAll();

            // IInterface1
            var instance1 = container.Get<IInterface1>();
            Assert.IsNotNull(instance1);
            Assert.AreEqual(typeof(Impl1234), instance1.GetType());
            // IInterface2
            var instance2 = container.Get<IInterface2>();
            Assert.IsNotNull(instance2);
            Assert.AreEqual(typeof(Impl1234), instance2.GetType());
            Assert.IsTrue(object.ReferenceEquals(instance1, instance2), "Should be using the same reference");
            // IInterface3
            var instance3 = container.Get<IInterface3>();
            Assert.IsNotNull(instance3);
            Assert.AreEqual(typeof(Impl1234), instance3.GetType());
            Assert.IsTrue(object.ReferenceEquals(instance2, instance3), "Should be using the same reference");
            // IInterface4
            var instance4 = container.Get<IInterface4>();
            Assert.IsNotNull(instance4);
            Assert.AreEqual(typeof(Impl1234), instance4.GetType());
            Assert.IsTrue(object.ReferenceEquals(instance3, instance4), "Should be using the same reference");
        }
    
        private static IEnumerable<Action<IDiBinder<IInterface1, IInterface2, IInterface3, IInterface4, IInterface5>>> BindMethods5
        {
            get
            {
                yield return binder => binder.ToNewInstance<Impl12345>();
                yield return binder => binder.ToFactoryMethod(()=>new Impl12345());
                yield return binder => binder.ToFactory(new Impl12345Factory());
                yield return binder => binder.ToInstance(new Impl12345());
            }
        }

        [Test]
        [TestCaseSource(nameof(BindMethods5))]
        public void TestBindingMultipleInterfaces(Action<IDiBinder<IInterface1, IInterface2, IInterface3, IInterface4, IInterface5>> bindMethod)
        {
            var container = new DiContainer();

            var binder = container.Bind<IInterface1, IInterface2, IInterface3, IInterface4, IInterface5>();
            bindMethod(binder);
            container.TryResolveAll();

            // IInterface1
            var instance1 = container.Get<IInterface1>();
            Assert.IsNotNull(instance1);
            Assert.AreEqual(typeof(Impl12345), instance1.GetType());
            // IInterface2
            var instance2 = container.Get<IInterface2>();
            Assert.IsNotNull(instance2);
            Assert.AreEqual(typeof(Impl12345), instance2.GetType());
            Assert.IsTrue(object.ReferenceEquals(instance1, instance2), "Should be using the same reference");
            // IInterface3
            var instance3 = container.Get<IInterface3>();
            Assert.IsNotNull(instance3);
            Assert.AreEqual(typeof(Impl12345), instance3.GetType());
            Assert.IsTrue(object.ReferenceEquals(instance2, instance3), "Should be using the same reference");
            // IInterface4
            var instance4 = container.Get<IInterface4>();
            Assert.IsNotNull(instance4);
            Assert.AreEqual(typeof(Impl12345), instance4.GetType());
            Assert.IsTrue(object.ReferenceEquals(instance3, instance4), "Should be using the same reference");
            // IInterface5
            var instance5 = container.Get<IInterface5>();
            Assert.IsNotNull(instance5);
            Assert.AreEqual(typeof(Impl12345), instance5.GetType());
            Assert.IsTrue(object.ReferenceEquals(instance4, instance5), "Should be using the same reference");
        }
    


    private class Impl12 : IInterface1, IInterface2
    {
    }
    
    private class Impl12Factory : IInstanceFactory<Impl12>
    {
        public Impl12 CreateInstance() => new Impl12();
    }

    private class Impl123 : IInterface1, IInterface2, IInterface3
    {
    }
    
    private class Impl123Factory : IInstanceFactory<Impl123>
    {
        public Impl123 CreateInstance() => new Impl123();
    }

    private class Impl1234 : IInterface1, IInterface2, IInterface3, IInterface4
    {
    }
    
    private class Impl1234Factory : IInstanceFactory<Impl1234>
    {
        public Impl1234 CreateInstance() => new Impl1234();
    }

    private class Impl12345 : IInterface1, IInterface2, IInterface3, IInterface4, IInterface5
    {
    }
    
    private class Impl12345Factory : IInstanceFactory<Impl12345>
    {
        public Impl12345 CreateInstance() => new Impl12345();
    }
    public interface IInterface1 { }
    public interface IInterface2 { }
    public interface IInterface3 { }
    public interface IInterface4 { }
    public interface IInterface5 { }

    }
}