using System;
using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using Veeam.Signature.Solution;

namespace Veeam.Signature.UnitTests.Solution
{
    [TestFixture]
    public class WorkersManagerTests
    {
        private IBlockSequenceMonitor monitor;

        [SetUp]
        public void SetUp()
        {
            monitor = Substitute.For<IBlockSequenceMonitor>();
            monitor
                .When(m => m.DoFor(Arg.Any<long>(), Arg.Any<Action>()))
                .Do(_ => _.Arg<Action>()());
        }

        [Test]
        [TestCase(10)]
        [TestCase(1)]
        public void ProcessAndWaitTest(int workersCount)
        {
            var blocks = new[]
            {
                new Block(0, null),
                new Block(1, null),
                new Block(2, null),
                null
            };

            var i = 0;
            var input = Substitute.For<IInputProvider>();
            input.GetNextBlock().Returns(_ => i < blocks.Length ? blocks[i++] : null);

            var output = Substitute.For<IOutputConsumer>();

            using (var workers = new WorkersManager(workersCount, input, output))
            {
                workers.ProcessAndWait();
            }

            output.Received().Process(blocks[0]);
            output.Received().Process(blocks[1]);
            output.Received().Process(blocks[2]);
            output.DidNotReceive().Process(null);
        }

        [Test]
        public void EmptyStreamTest()
        {
            var result = new Dictionary<long, byte[]>();
            using (var stream = new MemoryStream())
            using (var inputDataProvider = new StreamDataProvider(stream, 1024 * 1024))
            using (var shaCalculator = new Sha256HashCalculator((i, b) => result.Add(i, b), monitor))
            using (var workersManager = new WorkersManager(2, inputDataProvider, shaCalculator))
            {
                workersManager.ProcessAndWait();
            }
            Assert.IsEmpty(result);
        }

        [Test]
        [TestCase("C:\\Work\\ACDC.zip", 8)]
        [TestCase("C:\\Work\\ACDC.zip", 32)]
        //[Ignore("it is a integration test with real large file. prepare it before")]
        public void CompareThreadsTest(string fileName, int workersCount)
        {
            var res1 = Run(fileName, 1);

            var resN = Run(fileName, workersCount);
            foreach (var key in res1.Keys)
            {
                CollectionAssert.AreEquivalent(res1[key], resN[key]);
            }
        }

        [Test]
        [TestCase("C:\\Work\\ACDC.zip", 8)]
        [TestCase("C:\\Work\\ACDC.zip", 32)]
        //[Ignore("it is a integration test with real large file. prepare it before")]
        public void CheckSequenceTest(string fileName, int workersCount)
        {
            var result = new List<long>();
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var inputDataProvider = new StreamDataProvider(stream, 1024 * 1024))
            using (var sequenceMonitor = new BlockSequenceMonitor())
            using (var shaCalculator = new Sha256HashCalculator((i, b) => result.Add(i), sequenceMonitor))
            using (var workersManager = new WorkersManager(workersCount, inputDataProvider, shaCalculator))
            {
                workersManager.ProcessAndWait();
            }
            for (var i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(i, result[i]);
            }
        }

        private Dictionary<long, byte[]> Run(string fileName, int workersCount)
        {
            var result = new Dictionary<long, byte[]>();
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var inputDataProvider = new StreamDataProvider(stream, 1024 * 1024))
            using (var shaCalculator = new Sha256HashCalculator((i, b) => result.Add(i, b), monitor))
            using (var workersManager = new WorkersManager(workersCount, inputDataProvider, shaCalculator))
            {
                workersManager.ProcessAndWait();
            }

            return result;
        }
    }
}
