// Copyright (c) 2022 Eric Bennett McDuffee

using System;
using NUnit.Framework;
using UJect.Exceptions;
using UJect.Injection;

namespace UJect.Tests
{
    [TestFixture]
    public class MultiBindTests
    {
        [Test]
        public void TestBindingMultipleInterfaces()
        {
            var container = new DiContainer();

            container.Bind<IInterface1, IInterface2>().ToNewInstance<Impl12>();
            container.TryResolveAll();

            var impl1 = container.Get<IInterface1>();
            var impl2 = container.Get<IInterface2>();

            Assert.IsNotNull(impl1);
            Assert.IsNotNull(impl2);
            Assert.AreEqual(typeof(Impl12), impl1.GetType());
            Assert.AreEqual(typeof(Impl12), impl2.GetType());
            Assert.IsTrue(object.ReferenceEquals(impl1, impl2), "Should be using the same reference");
        }

        [Test]
        public void TestDuplicateBinding()
        {
            var container = new DiContainer();

            container.Bind<IInterface1, IInterface2>().ToNewInstance<Impl12>();
            Assert.Throws<DuplicateBindingException>(() => container.Bind<IInterface1>().ToNewInstance<Impl12>());
        }
        
        [Test]
        public void TestMultiBindingDependsOnItself()
        {
            var container = new DiContainer();

            Assert.Throws<CyclicDependencyException>(() => container.Bind<IInterface1, IInterface2>().ToNewInstance<Impl12_Broken>());
        }

        private interface IInterface1 { }

        private interface IInterface2 { }

        private class Impl12_Broken : IInterface1, IInterface2
        {
            [Inject]
            private IInterface1 impl1;
        }

        private class Impl12 : IInterface1, IInterface2
        {
        }
    }
}