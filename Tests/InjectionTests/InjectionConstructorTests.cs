using System;
using System.Linq;
using NUnit.Framework;
using UJect.Exceptions;
using UJect.Injection;
using UnityEngine;

namespace UJect.Tests.InjectionTests
{
    [TestFixture]
    public class InjectionConstructorTests
    {

        [SetUp]
        public void SetUp()
        {
            InjectorCache.ClearCache();
        }
        
        [Test]
        public void TestInjectorContainsAllConstructors()
        {
            var injector = new Injector(typeof(MultiConstructorType));
            Assert.AreEqual(4, injector.InjectableConstructors.Count, "Injector should recognize 2 constructor!");

            void AssertConstructorParamCounts(int expectedInjectableCount, int expectedNonInjectableCount, Injector.InjectableConstructor injectableConstructor)
            {
                var expectedTotal = expectedInjectableCount + expectedNonInjectableCount;

                var actualInjectableCount = injectableConstructor.InjectedParamKeys?.Count ?? 0;
                var actualnonInjectableCount = injectableConstructor.NonInjectedParams?.Count ?? 0;
                var actualTotal = actualInjectableCount + actualnonInjectableCount;

                Assert.AreEqual(expectedTotal, actualTotal, $"Constructor should have {expectedTotal} param(s) total");
                Assert.AreEqual(expectedInjectableCount, actualInjectableCount, $"Constructor should have {expectedInjectableCount} injectable param(s) total");
                Assert.AreEqual(expectedNonInjectableCount, actualnonInjectableCount, $"Constructor should have {expectedNonInjectableCount} non-injectable param(s) total");
            }

            var firstConstructor = injector.InjectableConstructors[0];
            AssertConstructorParamCounts(2, 0, firstConstructor);

            Assert.AreEqual(typeof(IInterface), firstConstructor.InjectedParamKeys[0].InjectedResourceType, "Constructor param 0 type should match!");
            Assert.AreEqual("A", firstConstructor.InjectedParamKeys[0].InjectedResourceName, "Constructor param 0 name should match");

            Assert.AreEqual(typeof(IInterface), firstConstructor.InjectedParamKeys[1].InjectedResourceType, "Constructor param 1 type should match!");
            Assert.AreEqual("B", firstConstructor.InjectedParamKeys[1].InjectedResourceName, "Constructor param 1 name should match");

            var secondConstructor = injector.InjectableConstructors[1];
            AssertConstructorParamCounts(1, 1, secondConstructor);
            
            Assert.AreEqual(typeof(IInterface), secondConstructor.InjectedParamKeys[0].InjectedResourceType, "Constructor param 0 type should match!");
            Assert.AreEqual("A", secondConstructor.InjectedParamKeys[0].InjectedResourceName, "Constructor param 0 name should match");
            
            Assert.AreEqual(typeof(int), secondConstructor.NonInjectedParams[0], "Constructor param 0 type should match!");

            var thirdConstructor = injector.InjectableConstructors[2];
            AssertConstructorParamCounts(1, 0, thirdConstructor);

            var fourthConstructor = injector.InjectableConstructors[3];
            AssertConstructorParamCounts(0, 1, fourthConstructor);
            Assert.AreEqual(typeof(int), fourthConstructor.NonInjectedParams[0], "Constructor param 0 type should match!");

        }

        [Test]
        public void TestCreateAllInjectedInstance()
        {
            var injector = new Injector(typeof(AllInjectedConstructorClass));
            var container = new DiContainer("TestContainer");

            var implA = new Impl();
            var implB = new Impl();
            
            container.Bind<IInterface>().WithId("A").ToInstance(implA);
            container.Bind<IInterface>().WithId("B").ToInstance(implB);
            var instance = injector.CreateInstance<AllInjectedConstructorClass>(container);
            
            Assert.IsNotNull(instance);
            Assert.AreEqual(implA, instance.Param1);
            Assert.AreEqual(implB, instance.Param2);
        }
        
        [Test]
        public void TestCreateAllInjectedInstance_BadArgs()
        {
            var injector = new Injector(typeof(AllInjectedConstructorClass));
            var container = new DiContainer("TestContainer");

            var implA = new Impl();
            var implB = new Impl();
            
            container.Bind<IInterface>().WithId("A").ToInstance(implA);
            container.Bind<IInterface>().WithId("B").ToInstance(implB);
            Assert.Throws<InjectionException>(() =>
            {
                injector.CreateInstance<AllInjectedConstructorClass>(container, 123);
            });
        }
        
        [Test]
        public void TestCreateAllInjectedInstance_MissingDependency()
        {
            var injector = new Injector(typeof(AllInjectedConstructorClass));
            var container = new DiContainer("TestContainer");

            var implA = new Impl();
            
            container.Bind<IInterface>().WithId("A").ToInstance(implA);
            Assert.Throws<InjectionException>(() =>
            {
                injector.CreateInstance<AllInjectedConstructorClass>(container);
            });
        }
        
        [Test]
        public void TestCreateSomeInjectedInstance()
        {
            var injector = new Injector(typeof(SomeInjectedConstructorClass));
            var container = new DiContainer("TestContainer");

            var implA = new Impl();
            var intArg = 123;
            
            container.Bind<IInterface>().WithId("A").ToInstance(implA);
            var instance = injector.CreateInstance<SomeInjectedConstructorClass>(container, intArg);
            
            Assert.IsNotNull(instance);
            Assert.AreEqual(implA, instance.Param1);
            Assert.AreEqual(intArg, instance.Param2);
        }
        
        [Test]
        public void TestCreateSomeInjectedInstance_BadArgs()
        {
            var injector = new Injector(typeof(SomeInjectedConstructorClass));
            var container = new DiContainer("TestContainer");

            var implA = new Impl();
            
            container.Bind<IInterface>().WithId("A").ToInstance(implA);
            Assert.Throws<InjectionException>(() =>
            {
                // Too few params
                injector.CreateInstance<SomeInjectedConstructorClass>(container);
            });
            
            Assert.Throws<InjectionException>(() =>
            {
                // Too many params
                injector.CreateInstance<SomeInjectedConstructorClass>(container, 1, 2);
            });
            
            Assert.Throws<InjectionException>(() =>
            {
                // Bad param type
                injector.CreateInstance<SomeInjectedConstructorClass>(container, "hello");
            });
        }
        
        [Test]
        public void TestCreateNoneInjectedInstance()
        {
            var injector = new Injector(typeof(NoneInjectedConstructorClass));
            var container = new DiContainer("TestContainer");

            var intArg1 = 123;
            var intArg2 = 456;
            
            var instance = injector.CreateInstance<NoneInjectedConstructorClass>(container, intArg1, intArg2);
            
            Assert.IsNotNull(instance);
            Assert.AreEqual(intArg1, instance.Param1);
            Assert.AreEqual(intArg2, instance.Param2);
        }
        
        [Test]
        public void TestCreateNoneInjectedInstance_BadArgs()
        {
            var injector = new Injector(typeof(NoneInjectedConstructorClass));
            var container = new DiContainer("TestContainer");

            var intArg1 = 123;
            
            Assert.Throws<InjectionException>(() =>
            {
                // Too few params
                injector.CreateInstance<NoneInjectedConstructorClass>(container, intArg1);
            });
            
            Assert.Throws<InjectionException>(() =>
            {
                // Too many params
                injector.CreateInstance<NoneInjectedConstructorClass>(container, intArg1, intArg1, intArg1);
            });
            
            Assert.Throws<InjectionException>(() =>
            {
                // Bad param type
                injector.CreateInstance<NoneInjectedConstructorClass>(container, "hello", "world");
            });
        }
        
        private class MultiConstructorType
        {
            public MultiConstructorType([Inject("A")] IInterface param1, [Inject("B")] IInterface param2) { }
            public MultiConstructorType([Inject("A")] IInterface param1, int abc) { }
            public MultiConstructorType([Inject] IInterface param1) { }
            public MultiConstructorType(int abc) { }
        }

        private class AllInjectedConstructorClass
        {
            public IInterface Param1 { get; }
            public IInterface Param2 { get; }

            public AllInjectedConstructorClass([Inject("A")] IInterface param1, [Inject("B")] IInterface param2)
            {
                Param1 = param1;
                Param2 = param2;
            }
        }
        
        private class SomeInjectedConstructorClass
        {
            public IInterface Param1 { get; }
            public int Param2 { get; }

            public SomeInjectedConstructorClass([Inject("A")] IInterface param1, int param2)
            {
                Param1 = param1;
                Param2 = param2;
            }
        }
        
        private class NoneInjectedConstructorClass
        {
            public int Param1 { get; }
            public int        Param2 { get; }

            public NoneInjectedConstructorClass(int param1, int param2)
            {
                Param1 = param1;
                Param2 = param2;
            }
        }
        private interface IInterface
        {
        }

        private class Impl : IInterface
        {
            
        }
    }
}