using NUnit.Framework;
using UJect.Exceptions;

namespace UJect.Tests.Binding
{
    [TestFixture]
    public class SharedInstanceBindingTests
    {
        [Test]
        public void TestDoubleBindUnsharedInstance()
        {
            var container = new DiContainer();
            container.Bind<IInterface1>().AsUnsharedInstance().ToNewInstance<ThreeInterfaceClass>();
            container.Bind<IInterface2>().AsUnsharedInstance().ToNewInstance<ThreeInterfaceClass>();

            var int1 = container.Get<IInterface1>();
            var int2 = container.Get<IInterface2>();

            Assert.IsFalse(ReferenceEquals(int1, int2), "Unshared instance should be different for each interface!");
        }
        
        [Test]
        public void TestBindUnsharedAndShared()
        {
            var container = new DiContainer();
            container.Bind<IInterface1>().AsUnsharedInstance().ToNewInstance<ThreeInterfaceClass>();
            container.Bind<IInterface2>().ToNewInstance<ThreeInterfaceClass>();
            container.Bind<IInterface3>().ToNewInstance<ThreeInterfaceClass>();

            var int1 = container.Get<IInterface1>();
            var int2 = container.Get<IInterface2>();
            var int3 = container.Get<IInterface3>();

            // The unshared instance should be independent of the other two
            Assert.IsFalse(ReferenceEquals(int1, int2), "Unshared instance should be different for each interface!");
            Assert.IsFalse(ReferenceEquals(int1, int3), "Unshared instance should be different for each interface!");
            
            // The other two interfaces should share an instance
            Assert.IsTrue(ReferenceEquals(int2, int3), "Should share an instance!");
        }
        
        [Test]
        public void TestSharedInstanceIsUnbound()
        {
            var container = new DiContainer();
            container.Bind<IInterface1>().ToNewInstance<ThreeInterfaceClass>();
            container.Bind<IInterface2>().ToNewInstance<ThreeInterfaceClass>();
            container.Bind<IInterface3>().ToNewInstance<ThreeInterfaceClass>();
            
            container.TryResolveAll();

            var instance = container.Get<IInterface1>();

            Assert.AreEqual(3, container.SharedInstanceCache.RefCount(new InjectionKey(typeof(ThreeInterfaceClass))), "Should be 3 references to ThreeInterfaceClass's shared instance");

            container.Unbind<IInterface1>();
            Assert.AreEqual(2, container.SharedInstanceCache.RefCount(new InjectionKey(typeof(ThreeInterfaceClass))), "Should be 2 references to ThreeInterfaceClass's shared instance");
            container.Unbind<IInterface2>();
            Assert.AreEqual(1, container.SharedInstanceCache.RefCount(new InjectionKey(typeof(ThreeInterfaceClass))), "Should be 1 references to ThreeInterfaceClass's shared instance");
            container.Unbind<IInterface3>();
            Assert.AreEqual(0, container.SharedInstanceCache.RefCount(new InjectionKey(typeof(ThreeInterfaceClass))), "Should be 0 references to ThreeInterfaceClass's shared instance");
            
            container.Bind<IInterface1>().ToNewInstance<ThreeInterfaceClass>();
            var instance2 = container.Get<IInterface1>();
            
            Assert.IsFalse(ReferenceEquals(instance, instance2), "New instance should have been created after no more references to the first instance");
        }

        private class ThreeInterfaceClass : IInterface1, IInterface2, IInterface3 { }

        private interface IInterface1 { }

        private interface IInterface2 { }
        private interface IInterface3 { }


        [Test]
        public void TestDuplicateBindExceptionThrownForUnsharedInstances()
        {
            var container = new DiContainer();

            container.Bind<IInterface1>().AsUnsharedInstance().ToNewInstance<Class1>();
            Assert.Throws<DuplicateBindingException>(() =>
            {
                container.Bind<IInterface1>().AsUnsharedInstance().ToNewInstance<Class2>();
            });
        }

        private class Class1 : IInterface1 { }

        private class Class2 : IInterface1 { }
    }
}