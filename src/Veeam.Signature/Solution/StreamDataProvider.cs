using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Veeam.Signature.Solution
{
    public class StreamDataProvider : IInputProvider, IDisposable
    {
        private Stream stream;
        private readonly long blockSize;
        private readonly long blocksCount;

        private int currentBlockNum = -1;

        public StreamDataProvider(Stream stream, long blockSize)
        {
            this.blockSize = blockSize;
            this.stream = stream;
            blocksCount = (long)Math.Ceiling((double)stream.Length / blockSize);
            Trace.TraceInformation($"Define blocks count: {blocksCount}");
        }

        public Block GetNextBlock()
        {
            var num = Interlocked.Increment(ref currentBlockNum);
            if (num >= blocksCount)
            {
                return null;
            }
            return new Block(num, new SubStreamForSha256Alg(stream, num * blockSize, blockSize));
        }

        // TODO: implement MS Dispose pattern?
        public void Dispose()
        {
            stream?.Dispose();
            stream = null;
        }
    }
}
