using System.IO;
using System.Text;
using NUnit.Framework;
using Veeam.Signature.Solution;

namespace Veeam.Signature.UnitTests.Solution
{
    [TestFixture]
    public class StreamDataProviderTests
    {
        [Test]
        public void GetNextBlock_ForNormalStreamTest()
        {
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes("test")))
            using (var provider = new StreamDataProvider(stream, 3))
            {
                var block1 = provider.GetNextBlock();
                Assert.AreEqual(0, ((SubStreamForSha256Alg)block1.SubStream).OffsetInRealStream);
                Assert.AreEqual(0, block1.Num);

                var block2 = provider.GetNextBlock();
                Assert.AreEqual(3, ((SubStreamForSha256Alg)block2.SubStream).OffsetInRealStream);
                Assert.AreEqual(1, block2.Num);

                var block3 = provider.GetNextBlock();
                Assert.AreEqual(6, ((SubStreamForSha256Alg)block3.SubStream).OffsetInRealStream);
                Assert.AreEqual(2, block3.Num);

                var block4 = provider.GetNextBlock();
                Assert.IsNull(block4);

                var block5 = provider.GetNextBlock();
                Assert.IsNull(block5);
            }
        }

        [Test]
        public void GetNextBlock_ForSmallStreamTest()
        {
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes("test")))
            using (var provider = new StreamDataProvider(stream, 10))
            {
                var block1 = provider.GetNextBlock();
                Assert.AreEqual(0, ((SubStreamForSha256Alg)block1.SubStream).OffsetInRealStream);
                Assert.AreEqual(0, block1.Num);

                var block2 = provider.GetNextBlock();
                Assert.IsNull(block2);
            }
        }

        [Test]
        public void GetNextBlock_ForEmptyStreamTest()
        {
            using (var stream = new MemoryStream())
            using (var provider = new StreamDataProvider(stream, 3))
            {
                var block1 = provider.GetNextBlock();
                Assert.IsNull(block1);
            }
        }

    }
}
