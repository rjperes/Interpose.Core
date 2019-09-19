using System;
using System.Diagnostics;
using Interpose.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Interpose.Core.Handlers
{

    public class LoggingInterceptionHandler : IInterceptionHandler
    {
        private readonly ILogger<LoggingInterceptionHandler> logger;
        private readonly LogLevel logLevel;

        public LoggingInterceptionHandler(ILogger<LoggingInterceptionHandler> logger)
        {
            this.logger = logger;
        }

        public LoggingInterceptionHandler(ILogger<LoggingInterceptionHandler> logger, LogLevel logLevel) : this(logger)
        {
            this.logLevel = logLevel;
        }

        public void Invoke(InterceptionArgs arg)
        {
            var timer = Stopwatch.StartNew();
            this.logger.Log(this.logLevel, new EventId(), $"About to invoke {arg.Method} of type {arg.Method.DeclaringType}", null, (state, ex) => state.ToString());
            try
            {
                arg.Proceed();
                this.logger.Log(this.logLevel, new EventId(), $"Invokation of {arg.Method} of type {arg.Method.DeclaringType} took {timer.Elapsed}", null, (state, ex) => state.ToString());
            }
            catch (Exception ex)
            {
                this.logger.Log(this.logLevel, new EventId(), $"Invokation of {arg.Method} took {timer.Elapsed} and resulted in exception {ex}", ex, (state, _) => state.ToString());
            }
        }
    }
}
