using NUnit.Framework;
using UJect.Exceptions;

namespace UJect.Tests.Binding
{
    [TestFixture]
    public class DuplicateBindingTests
    {
        private DiContainer diContainer;

        [Test]
        public void TestBindingTwiceThrowsDuplicateBindingException()
        {
            diContainer = new DiContainer();
            diContainer.Bind<IInterface>().ToNewInstance<Impl>();
            Assert.Throws<DuplicateBindingException>(() =>
            {
                diContainer.Bind<IInterface>().ToNewInstance<Impl>();
            });
        }
        
        [Test]
        public void TestBindingTwiceWithSameKeyThrowsDuplicateBindingException()
        {
            var bindingId = "Hello";
            diContainer = new DiContainer();
            diContainer.Bind<IInterface>().WithId(bindingId).ToNewInstance<Impl>();
            Assert.Throws<DuplicateBindingException>(() =>
            {
                diContainer.Bind<IInterface>().WithId(bindingId).ToNewInstance<Impl>();
            });
        }
        
        [Test]
        public void TestBindingAfterUnbindDoesNotThrowDuplicateBindingException()
        {
            diContainer = new DiContainer();
            diContainer.Bind<IInterface>().ToNewInstance<Impl>();
            diContainer.Unbind<IInterface>();
            Assert.DoesNotThrow(() =>
            {
                diContainer.Bind<IInterface>().ToNewInstance<Impl>();
            });
        }

        private interface IInterface { }

        private class Impl : IInterface { }
    }
}