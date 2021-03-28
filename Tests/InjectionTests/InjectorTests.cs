using System.Linq;
using NUnit.Framework;
using UJect.Injection;

namespace UJect.Tests.InjectionTests
{
    [TestFixture]
    public class InjectionTests
    {
        private class InjectableFieldsType
        {
            [Inject]
            public IInterface field1;

            [Inject("A")]
            public IInterface field2;

            public IInterface field3;

            public InjectableFieldsType(IInterface param1) { }

            public InjectableFieldsType([Inject("A")] IInterface param1, [Inject("B")] IInterface param2) { }
        }
        
        [Test]
        public void TestInjectorContainsAllInjectableFields()
        {
            var injector = new Injector(typeof(InjectableFieldsType));
            Assert.AreEqual(2, injector.InjectableFields.Count, "Injector should recognize 2 fields!");

            var orderedFields = injector.InjectableFields.OrderBy(injField => injField.FieldInfo.Name).ToList();
            
            Assert.AreEqual(typeof(IInterface), orderedFields[0].InjectionKey.InjectedResourceType, "Field InjectionKey resource type should match");
            Assert.AreEqual("field1", orderedFields[0].FieldInfo.Name);
            
            Assert.AreEqual(typeof(IInterface), orderedFields[1].InjectionKey.InjectedResourceType, "Field InjectionKey resource type should match");
            Assert.AreEqual("field2", orderedFields[1].FieldInfo.Name);
            
            Assert.AreEqual("A", orderedFields[1].InjectionKey.InjectedResourceName, "Constructor param 1 name should match");

        }
        
        [Test]
        public void TestInjectorContainsAllInjectableConstructors()
        {
            var injector = new Injector(typeof(InjectableFieldsType));
            Assert.AreEqual(1, injector.InjectableConstructors.Count, "Injector should recognize 1 constructor!");

            var injectableConstructor = injector.InjectableConstructors.First();
            
            Assert.AreEqual(2, injectableConstructor.ParamKeys.Length, "Constructor should have two params");

            Assert.AreEqual(typeof(IInterface), injectableConstructor.ParamKeys[0].InjectedResourceType, "Constructor param 0 type should match!");
            Assert.AreEqual("A", injectableConstructor.ParamKeys[0].InjectedResourceName, "Constructor param 0 name should match");
            
            Assert.AreEqual(typeof(IInterface), injectableConstructor.ParamKeys[1].InjectedResourceType, "Constructor param 1 type should match!");
            Assert.AreEqual("B", injectableConstructor.ParamKeys[1].InjectedResourceName, "Constructor param 1 name should match");
        }

        [Test]
        public void TestInjectorCache()
        {
            InjectorCache.ClearCache();
            
            //Should reuse the first instance of an injector
            var instance = InjectorCache.GetOrCreateInjector(typeof(IInterface));
            Assert.IsNotNull(instance, "Injector instance should not be null");
            Assert.AreEqual(1, InjectorCache.CachedInjectorCount);
            var secondInstance = InjectorCache.GetOrCreateInjector(typeof(IInterface));
            Assert.AreEqual(1, InjectorCache.CachedInjectorCount);
            Assert.IsTrue(ReferenceEquals(instance, secondInstance), "Should reuse injector instance!");
            
            //Clear should reset to zero
            InjectorCache.ClearCache();
            Assert.AreEqual(0, InjectorCache.CachedInjectorCount);
        }
        
        private interface IInterface
        {
            
        }
        
    }
}