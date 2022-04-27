using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using Veeam.Signature.Solution;

namespace Veeam.Signature.UnitTests.Solution
{
    [TestFixture]
    public class Sha256HashCalculatorTests
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

        [TestCase("F8-34-1B-85-0B-43-A9-7B", "CF-E9-C3-FD-9F-47-AD-41-61-EA-95-7D-CE-89-01-B3-85-8B-86-F2-05-5F-48-2C-33-36-4F-98-2E-5F-AA-C5")]
        [TestCase("F8", "25-15-5A-6E-7F-E2-EC-79-EF-14-95-2D-57-02-E2-4A-E6-1E-10-19-62-5B-C8-33-20-E4-17-99-5F-C9-47-E9")]
        public void ProcessTest(string input, string expected)
        {
            string result = null;
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(input)))
            using (var shaCalc = new Sha256HashCalculator((i, sha) => { result = BitConverter.ToString(sha); }, monitor))
            {
                var block = new Block(0, stream);
                shaCalc.Process(block);
                Assert.AreEqual(expected, result);
            }
        }

        [Test]
        public void CheckSequence()
        {
            using (var stream = new MemoryStream(new byte[] { 0, 1, 2 }))
            using (var shaCalculator = new Sha256HashCalculator((i, b) => { }, monitor))
            {
                var block = new Block(4, stream);
                shaCalculator.Process(block);
            }
        }
    }
}
