using System.IO;

namespace Veeam.Signature.Solution
{
    public class Block
    {
        public long Num { get; }

        public Stream SubStream { get; }

        public Block(long num, Stream subStream)
        {
            Num = num;
            SubStream = subStream;
        }
    }
}
