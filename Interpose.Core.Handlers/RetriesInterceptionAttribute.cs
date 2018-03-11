using Interpose.Core.Interceptors;
using System;

namespace Interpose.Core.Handlers
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class RetriesInterceptionAttribute : InterceptionAttribute
    {
        public RetriesInterceptionAttribute(int numRetries, TimeSpan delay) : base(typeof(RetriesInterceptionHandler))
        {
            this.NumRetries = numRetries;
            this.Delay = delay;
        }

        public int NumRetries { get; }
        public TimeSpan Delay { get; }
    }
}
