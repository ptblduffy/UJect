using NUnit.Framework;
using UJect;
using Assert = UnityEngine.Assertions.Assert;

namespace Tests.RefCounterTests
{
    [TestFixture]
    public class RefCounterTests
    {
        [Test]
        public void RefCountIncrDecrTest()
        {
            var key = "HelloWorld";
            
            var refCounter = new RefCounter<string>();
            
            Assert.AreEqual(0, refCounter.RefCount(key));

            var maxRefCount = 5;

            for (int i = 1; i <= maxRefCount; i++)
            {
                refCounter.Increment(key);
                Assert.AreEqual(i, refCounter.RefCount(key));
            }

            for (int i = 1; i <= maxRefCount; i++)
            {
                refCounter.Decrement(key);
                Assert.AreEqual(maxRefCount- i, refCounter.RefCount(key));
            }
            
            Assert.AreEqual(0, refCounter.RefCount(key));
        }

        [Test]
        public void RefCountNeverGoesBelowZero()
        {
            var key = "HelloWorld";
            var refCounter = new RefCounter<string>();
            refCounter.Increment(key); // 1
            
            refCounter.Decrement(key); // 0
            refCounter.Decrement(key); // -1, but should stay at 0
            
            Assert.AreEqual(0, refCounter.RefCount(key));
        }
    }
}