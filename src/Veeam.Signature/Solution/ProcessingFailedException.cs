using System;

namespace Veeam.Signature.Solution
{
    public class ProcessingFailedException : Exception
    {
        public ProcessingFailedException(string message, Exception e) : base(message, e)
        {

        }
    }
}
