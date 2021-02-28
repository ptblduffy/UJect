using System;
using NUnit.Framework;
using UJect.Exceptions;
using UJect.Injection;

namespace UJect.Tests
{
    [TestFixture]
    public class CyclicBindingTests
    {
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
            [Inject]
            private IInterface1 impl1;
        }
        
        [Test]
        public void TestBindCyclicGraph()
        {
            var container = new DiContainer();
            container.Bind<IInterface1>().ToInstance(new Impl1());
            Assert.Throws<CyclicDependencyException>(() =>
            {
                container.Bind<IInterface2>().ToInstance(new Impl2());
            });
        }
        
    }
}