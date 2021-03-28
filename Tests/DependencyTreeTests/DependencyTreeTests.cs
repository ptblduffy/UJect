using System.Linq;
using NUnit.Framework;
using UJect.Injection;
using UnityEngine;

namespace UJect.Tests
{
    [TestFixture]
    public class DependencyTreeTests
    {
        private DependencyTree dependencyTree;

        [SetUp]
        public void SetUp()
        {
            dependencyTree = new DependencyTree();
        }

        [Test]
        public void TestDependencyTreeOrdering()
        {
            var interface1Key = new InjectionKey(typeof(IInterface1));
            var impl1Key = new InjectionKey(typeof(Impl1));
            
            var interface2Key = new InjectionKey(typeof(IInterface2));
            var impl2Key = new InjectionKey(typeof(Impl2));
            
            var interface3Key = new InjectionKey(typeof(IInterface3));

            //First, we'll mock a simple interface binding of interface1 to impl1
            dependencyTree.AddDependency(interface1Key, impl1Key);
            var sorted = dependencyTree.Sorted().ToList();
            
            Assert.AreEqual(sorted[0], interface3Key, "Interface 3 should be the first thing to resolve, since impl1 depends on it and interface1 depends on impl1");
            Assert.AreEqual(sorted[1], interface2Key, "Interface 2 should be the second thing to resolve, since impl1 depends on it and interface1 depends on impl1");
            Assert.AreEqual(sorted[2], impl1Key, "Impl 1 should be the third, since interface 1 depends on it");
            Assert.AreEqual(sorted[3], interface1Key, "Finally, interface 1 should be resolved");
            
            Assert.AreEqual(1, dependencyTree.RootKeys.Count(), "Should be a single root, interface1");
            Assert.AreEqual(interface1Key, dependencyTree.RootKeys.First());
            
            //Next, we'll bind interface 2
            dependencyTree.AddDependency(interface2Key, impl2Key);
            sorted = dependencyTree.Sorted().ToList();
            
            Assert.AreEqual(2, dependencyTree.RootKeys.Count(), "Should now be two roots");
            Assert.IsTrue(dependencyTree.RootKeys.Contains(interface1Key));
            Assert.IsTrue(dependencyTree.RootKeys.Contains(interface2Key));
            
            Assert.AreEqual(sorted[0], impl2Key, "Now we should be resolving impl2 first");
            Assert.AreEqual(sorted[1], interface2Key, "Interface 2 should be the second thing to resolve, since impl1 depends on it and interface1 depends on impl1");
            Assert.AreEqual(sorted[2], interface3Key, "Interface 3 should be the third thing to resolve, since impl1 depends on it and interface1 depends on impl1");
            Assert.AreEqual(sorted[3], impl1Key, "Impl 1 should be the fourth, since interface 1 depends on it");
            Assert.AreEqual(sorted[4], interface1Key, "Finally, interface 1 should be resolved");
            
        }


        private interface IInterface1 { }

        private class Impl1 : IInterface1
        {
            [Inject]
            public IInterface2 impl2;
            
            [Inject]
            public IInterface3 impl3;
        }

        private interface IInterface2 { }

        private class Impl2 : IInterface3 { }

        private interface IInterface3 { }

        private class Impl3 : IInterface4
        {
            [Inject]
            public IInterface4 impl4;
        }

        private interface IInterface4 { }

        private class Impl4 : IInterface4 { }
    }
}