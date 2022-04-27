using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Veeam.Signature.Solution;

namespace Veeam.Signature.UnitTests.Solution
{
    [TestFixture]
    public class BlockSequenceMonitorTests
    {
        [Test]
        public void Test()
        {
            var monitor = new BlockSequenceMonitor();
            var list = new List<int>();

            for (var i = 0; i < 10; i++)
            {
                new Thread(arg =>
                {
                    var num = (int)arg;
                    monitor.DoFor(num, () =>
                    {
                        list.Add(num);
                    });
                }).Start(i);
            }

            monitor.DoFor(10, () => {});

            for (var i = 0; i < list.Count; i++)
            {
                Assert.AreEqual(i, list[i]);
            }
        }
    }
}
