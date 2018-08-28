using Interpose.Core.Interceptors;
using System;
using System.Threading.Tasks;

namespace Interpose.Core.Handlers
{
    public sealed class TimeoutInterceptionHandler : IInterceptionHandler
    {
        public TimeoutInterceptionHandler(int timeoutSeconds)
        {
            this.TimeoutSeconds = timeoutSeconds;
        }

        public TimeoutInterceptionHandler(TimeSpan timeout)
        {
            this.TimeoutSeconds = (int)timeout.TotalSeconds;
        }

        public int TimeoutSeconds { get; }

        public void Invoke(InterceptionArgs arg)
        {
            var task = Task.Run(() =>
            {
                arg.Proceed();
            });

            if (task.Wait(TimeSpan.FromSeconds(this.TimeoutSeconds)) == false)
            {
                throw new TimeoutException();
            }
        }
    }
}