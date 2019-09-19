using System;
using Interpose.Core.Interceptors;

namespace Interpose.Core.Handlers
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class TimeoutInterceptionAttribute : InterceptionAttribute, IHandlerFactory
    {
        public TimeoutInterceptionAttribute(int timeoutSeconds) : base(typeof(TimeoutInterceptionHandler))
        {
            this.TimeoutSeconds = timeoutSeconds;
        }

        public int TimeoutSeconds { get; }

        public IInterceptionHandler Instantiate(IServiceProvider serviceProvider)
        {
            return new TimeoutInterceptionHandler(this.TimeoutSeconds);
        }
    }
}
