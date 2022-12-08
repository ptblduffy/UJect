using System.Linq;
using NUnit.Framework;
using UJect.Injection;

namespace UJect.Tests.InjectionTests
{
    [TestFixture]
    public class InjectionTests
    {
        [SetUp]
        public void SetUp()
        {
            InjectorCache.ClearCache();
        }

        [Test]
        public void TestInjectorContainsAllInjectableFields()
        {
            var injector = new Injector(typeof(InjectionTests.InjectableFieldsType));
            Assert.AreEqual(2, injector.InjectableFields.Count, "Injector should recognize 2 fields!");

            var orderedFields = injector.InjectableFields.OrderBy(injField => injField.FieldInfo.Name).ToList();
            
            Assert.AreEqual(typeof(InjectionTests.IInterface), orderedFields[0].InjectionKey.InjectedResourceType, "Field InjectionKey resource type should match");
            Assert.AreEqual("field1", orderedFields[0].FieldInfo.Name);
            
            Assert.AreEqual(typeof(InjectionTests.IInterface), orderedFields[1].InjectionKey.InjectedResourceType, "Field InjectionKey resource type should match");
            Assert.AreEqual("field2", orderedFields[1].FieldInfo.Name);
            
            Assert.AreEqual("A", orderedFields[1].InjectionKey.InjectedResourceName, "Constructor param 1 name should match");

        }

        [Test]
        public void TestInjectorCache()
        {
            InjectorCache.ClearCache();

            //Should reuse the first instance of an injector
            var instance = InjectorCache.GetOrCreateInjector(typeof(InjectableFieldsType));
            Assert.IsNotNull(instance, "Injector instance should not be null");
            Assert.AreEqual(1, InjectorCache.CachedInjectorCount);
            var secondInstance = InjectorCache.GetOrCreateInjector(typeof(InjectableFieldsType));
            Assert.AreEqual(1, InjectorCache.CachedInjectorCount);
            Assert.IsTrue(ReferenceEquals(instance, secondInstance), "Should reuse injector instance!");
            
            //Clear should reset to zero
            InjectorCache.ClearCache();
            Assert.AreEqual(0, InjectorCache.CachedInjectorCount);
        }
        
        private interface IInterface
        {
            
        }
        
        private class InjectableFieldsType
        {
            [Inject]
            public InjectionTests.IInterface field1;

            [Inject("A")]
            public InjectionTests.IInterface field2;

            public InjectionTests.IInterface field3;
        }
        
    }
}