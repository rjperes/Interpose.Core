using System;
using Interpose.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Interpose.Core.Handlers
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class LoggingInterceptionAttribute : InterceptionAttribute, IHandlerFactory
    {
        public LoggingInterceptionAttribute() : base(typeof(LoggingInterceptionHandler))
        {
        }

        public LoggingInterceptionAttribute(LogLevel logLevel) : this()
        {
            this.LogLevel = LogLevel;
        }

        public LogLevel LogLevel { get; }

        public IInterceptionHandler Instantiate(IServiceProvider serviceProvider)
        {
            return new LoggingInterceptionHandler(serviceProvider.GetService<ILogger<LoggingInterceptionHandler>>(), this.LogLevel);
        }
    }
}
