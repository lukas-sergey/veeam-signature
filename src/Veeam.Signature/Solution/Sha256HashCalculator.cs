using System;
using System.Security.Cryptography;

namespace Veeam.Signature.Solution
{
    public class Sha256HashCalculator : IOutputConsumer, IDisposable
    {
        private readonly Action<long, byte[]> callback;
        private readonly IBlockSequenceMonitor monitor;

        public Sha256HashCalculator(Action<long, byte[]> callback, IBlockSequenceMonitor monitor)
        {
            this.callback = callback;
            this.monitor = monitor;
        }

        public void Process(Block block)
        {
            monitor.DoFor(block.Num, () =>
            {
                using (var sha256 = SHA256.Create())
                {
                    var hash = sha256.ComputeHash(block.SubStream);
                    callback(block.Num, hash);
                }
            });
        }

        public void Dispose()
        {
        }
    }
}
