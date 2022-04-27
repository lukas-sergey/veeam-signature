using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Veeam.Signature.Solution
{
    // My quick implementation of monitor
    public class BlockSequenceMonitor : IBlockSequenceMonitor, IDisposable
    {
        private readonly ConcurrentDictionary<long, AutoResetEvent> blockSequence = new ConcurrentDictionary<long, AutoResetEvent>();

        public void DoFor(long num, Action action)
        {
            WaitForPrevious(num);
            try
            {
                action();
            }
            finally
            {
                Set(num);
            }
        }

        public void Dispose()
        {
            foreach (var e in blockSequence)
            {
                e.Value.Dispose();
            }
            blockSequence.Clear();
        }

        private void Set(long num)
        {
            Get(num).Set();

            // Do not dispose this one, but we can dispose last one )
            if (blockSequence.TryRemove(num - 1, out var last))
            {
                last.Dispose();
            }
        }
        private void WaitForPrevious(long num)
        {
            if (num > 0)
            {
                Get(num - 1).WaitOne();
            }
        }

        private AutoResetEvent Get(long number)
        {
            return blockSequence.GetOrAdd(number, i => new AutoResetEvent(false));
        }
    }
}
