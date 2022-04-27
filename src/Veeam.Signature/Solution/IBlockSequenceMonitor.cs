using System;

namespace Veeam.Signature.Solution
{
    public interface IBlockSequenceMonitor
    {
        void DoFor(long num, Action action);
    }
}
