using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Veeam.Signature.Solution
{
    public class Sha256HashCalculator : IOutputConsumer, IDisposable
    {
        //private SHA256 sha256;
        private readonly Action<long, byte[]> callback;

        public Sha256HashCalculator(Action<long, byte[]> callback)
        {
            this.callback = callback;
            //sha256 = SHA256.Create();
        }

        public void Process(Block block)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(block.SubStream);
                callback(block.Num, hash);
            }
        }

        public void Dispose()
        {
            //sha256?.Dispose();
            //sha256 = null;
        }
    }
}
