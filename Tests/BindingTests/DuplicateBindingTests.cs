using NUnit.Framework;
using UJect.Exceptions;

namespace UJect.Tests
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

        private interface IInterface { }

        private class Impl : IInterface { }
    }
}