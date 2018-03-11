using Interpose.Core.Interceptors;
using System;
using System.Threading;

namespace Interpose.Core.Handlers
{

    public sealed class RetriesInterceptionHandler : IInterceptionHandler
    {
        private readonly int numRetries;
        private readonly TimeSpan delay;

        public RetriesInterceptionHandler(int numRetries, TimeSpan delay)
        {
            this.numRetries = numRetries;
            this.delay = delay;
        }

        public void Invoke(InterceptionArgs arg)
        {
            var retries = 0;

            while (retries < this.numRetries)
            {
                try
                {
                    arg.Proceed();
                    break;
                }
                catch
                {
                    retries++;
                    if (retries == this.numRetries)
                    {
                        throw;
                    }
                    Thread.Sleep(this.delay);
                }
            }
        }
    }
}
