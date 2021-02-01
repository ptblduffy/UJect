using NUnit.Framework;
using UJect;

namespace UJect.Tests
{
    [TestFixture]
    public class NewInstanceBinder
    {
        [Test]
        public void TestBindAcyclicGraph()
        {
            var container = new DiContainer();
            container.Bind<IInterface1>().ToNewInstance<Impl1>();
            container.Bind<IInterface2>().ToInstance(new Impl2());
            container.Resolve();

            var fetchedImpl1 = container.GetDependency<IInterface1>();
            Assert.NotNull(fetchedImpl1.Impl2);
        }


        private interface IInterface1
        {
            IInterface2 Impl2 { get; }
        }

        private interface IInterface2 { }

        private class Impl1 : IInterface1
        {
            public IInterface2 Impl2 { get; }

            public Impl1([Inject] IInterface2 impl2)
            {
                this.Impl2 = impl2;
            }

        }

        private class Impl2 : IInterface2 { }
    }
}