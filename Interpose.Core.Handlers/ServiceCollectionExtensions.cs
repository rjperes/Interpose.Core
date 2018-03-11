using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Interpose.Core.Handlers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLoggingHandler(this IServiceCollection services, LogLevel logLevel)
        {
            services.AddSingleton<LoggingInterceptionHandler>(sp => new LoggingInterceptionHandler(sp.GetRequiredService<ILogger<LoggingInterceptionHandler>>(), logLevel));
            return services;
        }

        public static IServiceCollection AddLoggingHandler(this IServiceCollection services)
        {
            services.AddSingleton<LoggingInterceptionHandler>(sp => new LoggingInterceptionHandler(sp.GetRequiredService<ILogger<LoggingInterceptionHandler>>()));
            return services;
        }
    }
}
