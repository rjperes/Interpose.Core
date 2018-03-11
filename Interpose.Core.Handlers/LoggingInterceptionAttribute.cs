using System;
using Interpose.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Interpose.Core.Handlers
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class LoggingInterceptionAttribute : InterceptionAttribute
    {
        public LoggingInterceptionAttribute() : base(typeof(LoggingInterceptionHandler))
        {
        }

        public LoggingInterceptionAttribute(LogLevel logLevel) : this()
        {
            this.LogLevel = LogLevel;
        }

        public LogLevel LogLevel { get; }
    }
}
