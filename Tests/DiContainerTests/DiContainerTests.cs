using System;
using NUnit.Framework;
using UJect.Injection;

namespace UJect.Tests
{
    [TestFixture]
    public class DiContainerTests
    {

        [Test]
        public void TestCreateInjectedInstance()
        {
            var container = new DiContainer();
            var impl1 = new Impl1();
            var impl2 = new Impl2();
            container.Bind<IInterface>().ToInstance(impl1);
            container.Bind<IInterface2>().ToInstance(impl2);

            var injectable = container.CreateInjectedInstance<InjectableType>();

            Assert.AreEqual(impl1, injectable.Field1, "Field1 should contain a reference to Impl1 due to field injection");
            Assert.AreEqual(impl2, injectable.Param1, "Field1 should contain a reference to Impl1 due to constructor injection");
        }

        [Test]
        public void TestDisposingInjectedInstance()
        {
            ITestDisposable testDisposable = null;
            using (var container = new DiContainer())
            {
                container.Bind<ITestDisposable>().ToNewInstance<TestDisposable>();
                testDisposable = container.Get<ITestDisposable>();
            }

            Assert.NotNull(testDisposable);
            Assert.IsTrue(testDisposable.IsDisposed);
        }

        private class InjectableType
        {
            [Inject]
            public IInterface Field1;

            public readonly IInterface2 Param1;

            public InjectableType([Inject]IInterface2 param1) => this.Param1 = param1;
        }

        private interface IInterface { }

        private class Impl1: IInterface { }

        private interface IInterface2 { }
        private class Impl2: IInterface2 { }

        private interface ITestDisposable : IDisposable
        {
            bool IsDisposed { get; }
        }

        private class TestDisposable : ITestDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

    }
}