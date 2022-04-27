using System;
using System.Diagnostics;
using System.IO;
using Veeam.Signature.Solution;

namespace Veeam.Signature
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //args = new[] { "/i", "C:\\Work\\ACDC.zip", "/b", $"{1024 * 1024}"  };
            var parameters = GetCommandStringParameters(args);
            if (parameters == null)
            {
                return;
            }

            // Usage of TPL was disabled => async/await is disabled as well
            // Let`s try from suggestion that threads count = count of CPUs
            var workersCount = Environment.ProcessorCount;
            Trace.TraceInformation($"Define workers count: {workersCount}");
            Trace.TraceInformation($"Define block size: {parameters.BlockSize} bytes");

            var startT = DateTime.UtcNow;
            Trace.TraceInformation($"Start time: {startT}");

            try
            {
                // Use Autofac nuget. Too many 'new'
                using (var stream = TryOpenFile(parameters.InputFile.FullName))
                using (var inputDataProvider = new StreamDataProvider(stream, parameters.BlockSize))
                using (var sequenceMonitor = new BlockSequenceMonitor())
                using (var shaCalculator = new Sha256HashCalculator(ShowHashInConsole, sequenceMonitor))
                using (var workersManager = new WorkersManager(workersCount, inputDataProvider, shaCalculator))
                {
                    workersManager.ProcessAndWait();
                }
            }
            catch (IOException ex)
            {
                Trace.TraceError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            var endT = DateTime.UtcNow;
            Trace.TraceInformation($"Finish time: {endT}");
            Trace.TraceInformation($"Total time: {endT - startT}");
        }

        private static Stream TryOpenFile(string fileName)
        {
            try
            {
                Trace.TraceInformation($"Open file: {fileName}");
                return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (IOException ex)
            {
                throw new ProcessingFailedException($"Error while opening file '{fileName}'", ex);
            }
        }

        private static ProgramParameters GetCommandStringParameters(string[] args)
        {
            var parameters = new ProgramParameters();
            var parser = new CommandLineParser.CommandLineParser();
            parser.ShowUsageOnEmptyCommandline = true;
            parser.ExtractArgumentAttributes(parameters);
            try
            {
                parser.ParseCommandLine(args);
            }
            catch (Exception ex)
            {
                parser.ShowParsedArguments(true);
                Trace.TraceError($"Error while validate arguments': {ex.Message}{Environment.NewLine}at {ex.StackTrace}");
            }
            return parser.ParsingSucceeded ? parameters : null;
        }

        private static void ShowHashInConsole(long blockNum, byte[] hash)
        {
            Console.WriteLine($"{blockNum}: {BitConverter.ToString(hash)}");
        }
    }
}
