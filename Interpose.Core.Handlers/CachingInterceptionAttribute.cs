using System;
using Interpose.Core.Interceptors;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Interpose.Core.Handlers
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CachingInterceptionAttribute : InterceptionAttribute, IHandlerFactory
    {
        public CachingInterceptionAttribute(int durationSeconds) : base(typeof(CachingInterceptionHandler))
        {
            this.Duration = TimeSpan.FromSeconds(durationSeconds);
        }

        public TimeSpan Duration { get; }

        public IInterceptionHandler Instantiate(IServiceProvider serviceProvider)
        {
            return new CachingInterceptionHandler(serviceProvider.GetRequiredService<IMemoryCache>(), this.Duration);
        }
    }
}
