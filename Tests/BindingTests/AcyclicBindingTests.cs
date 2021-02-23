using System.Runtime.InteropServices;
using NUnit.Framework;

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
        
        private class NamedImpl1 : IInterface1
        {
            [Inject("Bob")]
            public NamedImpl2 namedImpl2Bob;
            
            [Inject("Jerry")]
            public NamedImpl2 namedImpl2Jerry;
        }
        
        private class NamedImpl2 : IInterface1
        {
            public readonly string Name;
            
            public NamedImpl2(string name)
            {
                Name = name;
            }
        }
        
    }
}