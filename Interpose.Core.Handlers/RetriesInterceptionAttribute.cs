using Interpose.Core.Interceptors;
using System;

namespace Interpose.Core.Handlers
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class RetriesInterceptionAttribute : InterceptionAttribute, IHandlerFactory
    {
        public RetriesInterceptionAttribute(int numRetries, int delayMilliseconds) : base(typeof(RetriesInterceptionHandler))
        {
            this.NumRetries = numRetries;
            this.Delay = TimeSpan.FromMilliseconds(delayMilliseconds);
        }

        public int NumRetries { get; }
        public TimeSpan Delay { get; }

        public IInterceptionHandler Instantiate(IServiceProvider serviceProvider)
        {
            return new RetriesInterceptionHandler(this.NumRetries, this.Delay);
        }
    }
}
