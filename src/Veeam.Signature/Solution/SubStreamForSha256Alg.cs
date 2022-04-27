using System;
using System.IO;

namespace Veeam.Signature.Solution
{
    // Just for optimization purposes. TBD: check it.
    // As we now HashAlgorithm.ComputeHash(SubStream inputStream) uses only SubStream.Read method.
    public class SubStreamForSha256Alg : Stream
    {
        public long OffsetInRealStream { get; }

        private readonly Stream realStream;
        private readonly long blockSize;
        private long alreadyReadDataSize;

        public SubStreamForSha256Alg(Stream realStream, long offsetInRealStream, long blockSize)
        {
            this.realStream = realStream;
            this.blockSize = blockSize;
            this.OffsetInRealStream = offsetInRealStream;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                lock (realStream)
                {
                    realStream.Position = OffsetInRealStream + alreadyReadDataSize;
                    if (alreadyReadDataSize + count > blockSize)
                    {
                        count = (int)(blockSize - alreadyReadDataSize);
                    }

                    if (count <= 0)
                    {
                        return 0;
                    }

                    var result = realStream.Read(buffer, offset, count);
                    alreadyReadDataSize += result;
                    return result;
                }
            }
            catch (IOException ex)
            {
                throw new ProcessingFailedException($"Error while reading input file", ex);
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead { get; }
        public override bool CanSeek { get; }
        public override bool CanWrite { get; }
        public override long Length { get; }
        public override long Position { get; set; }
    }
}
