using System.IO;
using CommandLineParser.Arguments;

namespace Veeam.Signature.Solution
{
    public class ProgramParameters
    {
        [BoundedValueArgument(typeof(int), 'b', "blocksize", MinValue = 1, MaxValue = int.MaxValue, Description = "Block size")]
        public int BlockSize { get; set; }

        [FileArgument('i', "input", Description = "Input file", FileMustExist = true)]
        public FileInfo InputFile;
    }
}
