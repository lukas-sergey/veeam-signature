using System.IO;
using NUnit.Framework;
using Veeam.Signature.Solution;

namespace Veeam.Signature.UnitTests.Solution
{
    [TestFixture]
    public class SubStreamForSha256AlgTests
    {
        [Test]
        public void Read_SmallBlockInTheBeginingTest()
        {
            using (var stream = new MemoryStream(GenerateBytes()))
            {
                var substream = new SubStreamForSha256Alg(stream, 0, 10);
                var buffer = new byte[10];

                Assert.AreEqual(4, substream.Read(buffer, 0, 4));
                CollectionAssert.AreEqual(buffer, new[] { 0, 1, 2, 3, 0, 0, 0, 0, 0, 0 });

                Assert.AreEqual(6, substream.Read(buffer, 0, 7));
                CollectionAssert.AreEqual(buffer, new[] { 4, 5, 6, 7, 8, 9, 0, 0, 0, 0 });
            }
        }

        [Test]
        public void Read_SmallBlockInTheMiddleTest()
        {
            using (var stream = new MemoryStream(GenerateBytes()))
            {
                var substream = new SubStreamForSha256Alg(stream, 40, 10);
                var buffer = new byte[10];

                Assert.AreEqual(4, substream.Read(buffer, 0, 4));
                CollectionAssert.AreEqual(buffer, new[] { 40, 41, 42, 43, 0, 0, 0, 0, 0, 0 });

                Assert.AreEqual(6, substream.Read(buffer, 0, 7));
                CollectionAssert.AreEqual(buffer, new[] { 44, 45, 46, 47, 48, 49, 0, 0, 0, 0 });
            }
        }

        [Test]
        public void Read_SmallBlockInTheEndTest()
        {
            using (var stream = new MemoryStream(GenerateBytes()))
            {
                var substream = new SubStreamForSha256Alg(stream, 250, 10);
                var buffer = new byte[10];

                Assert.AreEqual(4, substream.Read(buffer, 0, 4));
                CollectionAssert.AreEqual(buffer, new[] { 250, 251, 252, 253, 0, 0, 0, 0, 0, 0 });

                buffer = new byte[10];
                Assert.AreEqual(2, substream.Read(buffer, 0, 7));
                CollectionAssert.AreEqual(buffer, new[] { 254, 255, 0, 0, 0, 0, 0, 0, 0, 0 });
            }
        }

        [Test]
        public void Read_BiggestBlockTest()
        {
            using (var stream = new MemoryStream(GenerateBytes()))
            {
                var substream = new SubStreamForSha256Alg(stream, 0, 300);
                var buffer = new byte[300];

                Assert.AreEqual(256, substream.Read(buffer, 0, 300));
                for (var i = 0; i < stream.Length; i++)
                {
                    Assert.AreEqual(i, buffer[i]);
                }
            }
        }

        private byte[] GenerateBytes()
        {
            var result = new byte[256];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = (byte)i;
            }
            return result;
        }
    }
}
