// Copyright (c) 2024 Eric Bennett McDuffee

using System;
using System.Collections.Generic;
using NUnit.Framework;
using UJect.Exceptions;
using UJect.Factories;
using UJect.Injection;

namespace UJect.Tests
{
    [TestFixture]
    public partial class MultiBindTests
    {
        [Test]
        public void TestDuplicateBinding_BindMultiTwice()
        {
            var container = new DiContainer();

            container.Bind<IInterface1, IInterface2>().ToNewInstance<Impl12>();
            Assert.Throws<DuplicateBindingException>(() => container.Bind<IInterface1, IInterface2>().ToNewInstance<Impl12>());
        }
        
        [Test]
        public void TestDuplicateBinding_BindMultiThenSingle()
        {
            var container = new DiContainer();

            container.Bind<IInterface1, IInterface2>().ToNewInstance<Impl12>();
            Assert.Throws<DuplicateBindingException>(() => container.Bind<IInterface1>().ToNewInstance<Impl12>());
        }
        
        [Test]
        public void TestDuplicateBinding_BindSingleThenMulti()
        {
            var container = new DiContainer();

            container.Bind<IInterface1>().ToNewInstance<Impl12>();
            Assert.Throws<DuplicateBindingException>(() => container.Bind<IInterface1, IInterface2>().ToNewInstance<Impl12>());
        }
        
        [Test]
        public void TestMultiBindingDependsOnItself()
        {
            var container = new DiContainer();

            Assert.Throws<CyclicDependencyException>(() => container.Bind<IInterface1, IInterface2>().ToNewInstance<Impl12_Broken>());
        }

        private class Impl12_Broken : IInterface1, IInterface2
        {
            [Inject]
            private IInterface1 impl1;
        }
    }
}
