using NUnit.Framework;
using UJect;
using UJect.Injection;

namespace UJect.Tests
{
    [TestFixture]
    public class NewInstanceBinderTests
    {
        [Test]
        public void TestBindAcyclicGraph()
        {
            var container = new DiContainer();
            container.Bind<IInterface1>().ToNewInstance<Impl1>();
            container.TryResolveAll();

            var instance = container.Get<IInterface1>();
            Assert.IsNotNull(instance);
            var instance2 = container.Get<IInterface1>();
            Assert.AreEqual(instance, instance2);

        }

        private interface IInterface1 { }

        private class Impl1 : IInterface1 { }
    }
}