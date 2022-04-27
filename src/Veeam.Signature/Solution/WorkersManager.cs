using System;
using System.Collections.Generic;
using System.Threading;

namespace Veeam.Signature.Solution
{
    public class WorkersManager : IDisposable
    {
        private readonly IInputProvider inputProvider;
        private readonly IOutputConsumer outputConsumer;
        private readonly List<AutoResetEvent> doneEvents = new List<AutoResetEvent>();
        private readonly int workerCount;

        public WorkersManager(int workerCount, IInputProvider inputProvider, IOutputConsumer outputConsumer)
        {
            this.workerCount = workerCount;
            this.inputProvider = inputProvider;
            this.outputConsumer = outputConsumer;
        }

        public void ProcessAndWait()
        {
            // TODO: lazy thread creating
            for (var i = 0; i < workerCount; i++)
            {
                var thread = new Thread(ProcessInner);
                var doneEvent = new AutoResetEvent(false);
                thread.Start(doneEvent);
                doneEvents.Add(doneEvent);
            }
            doneEvents.ForEach(e => e.WaitOne());
        }

        public void Dispose()
        {
            doneEvents.ForEach(e => e.Dispose());
            doneEvents.Clear();
        }

        private void ProcessInner(object obj)
        {
            var doneEvent = (AutoResetEvent)obj;
            Block block;
            do
            {
                block = inputProvider.GetNextBlock();
                if (block != null)
                {
                    outputConsumer.Process(block);
                }
            } while (block != null);
            doneEvent.Set();
        }
    }
}
