using NUnit.Framework;
using UJect.Injection;

namespace UJect.Tests.InjectionTests
{
    [TestFixture]
    public class InjectionScopeTests
    {
        
        [Test]
        public void TestInjectorContainsAllInjectableFields()
        {
            var container = new DiContainer();
            var injectable = new Injectable("A");
            container.Bind<IInjectable>().ToInstance(injectable);
            
            var instance = new TestClass();
            container.BindInstance(instance);
            
            container.TryResolveAll();
            
            Assert.IsNotNull(instance.Injectable);
            Assert.AreEqual("A", instance.Injectable.Name);
        }
        
        private abstract class ProtectedBaseClass
        {
            [Inject]
            protected IInjectable injectable;

        }

        private class TestClass : ProtectedBaseClass
        {
            public IInjectable Injectable => injectable;
        }
        
        private interface IInjectable
        {
            string Name { get; }
        }

        private class Injectable : IInjectable
        {
            public string Name { get; }
            
            public Injectable(string name)
            {
                Name = name;
            }
        }
    }
}