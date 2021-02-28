using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UJect.Injection;

namespace UJect.Tests
{
    [TestFixture]
    public class AcyclicBindingTests
    {

        [Test]
        public void TestBindAcyclicGraph()
        {
            var container = new DiContainer();
            container.Bind<IInterface1>().ToInstance(new Impl1());
            container.Bind<IInterface2>().ToInstance(new Impl2());
            container.TryResolveAll();
            Assert.Pass("No exception thrown!");
        }
        
        private interface IInterface1
        {
        }
        
        private interface IInterface2
        {
        }

        private class Impl1 : IInterface1
        {
            [Inject]
            private IInterface2 impl2;
        }
        
        private class Impl2 : IInterface2
        {
        }
    }
}