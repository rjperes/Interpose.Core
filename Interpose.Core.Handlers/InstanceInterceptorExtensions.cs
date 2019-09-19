using Interpose.Core.Interceptors;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Transactions;

namespace Interpose.Core.Handlers
{
    public static class InstanceInterceptorExtensions
    {
        public static T InterceptWithCache<T>(this IInstanceInterceptor interceptor, T target, TimeSpan duration)
        {
            var options = Options.Create<MemoryCacheOptions>(new MemoryCacheOptions());
            return interceptor.Intercept<T>(target, new CachingInterceptionHandler(new MemoryCache(options), duration));
        }

        public static T InterceptAsync<T>(this IInstanceInterceptor interceptor, T target)
        {
            return interceptor.Intercept<T>(target, AsyncInterceptionHandler.Instance);
        }

        public static T InterceptWithRetries<T>(this IInstanceInterceptor interceptor, T target, int numRetries, TimeSpan delay)
        {
            return interceptor.Intercept<T>(target, new RetriesInterceptionHandler(numRetries, delay));
        }

        public static T InterceptWithAccessControl<T>(this IInstanceInterceptor interceptor, T target, params string [] roles)
        {
            return interceptor.Intercept<T>(target, new AccessControlInterceptionHandler(roles));
        }

        public static T InterceptWithTransaction<T>(this IInstanceInterceptor interceptor, T target, TimeSpan timeout, TransactionScopeAsyncFlowOption asyncFlowOption = TransactionScopeAsyncFlowOption.Suppress, TransactionScopeOption scopeOption = TransactionScopeOption.Required, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            return interceptor.Intercept<T>(target, new TransactionInterceptionHandler(timeout, asyncFlowOption, scopeOption, isolationLevel));
        }

        public static T InterceptWithValidation<T>(this IInstanceInterceptor interceptor, T target)
        {
            return interceptor.Intercept<T>(target, ValidationInterceptionHandler.Instance);
        }

        public static T InterceptWithTimeout<T>(this IInstanceInterceptor interceptor, T target, TimeSpan timeout)
        {
            return interceptor.Intercept<T>(target, new TimeoutInterceptionHandler(timeout));
        }

        public static T InterceptWithCallback<T>(this IInstanceInterceptor interceptor, T target, Action callback)
        {
            return interceptor.Intercept<T>(target, new CallbackInterceptionHandler(callback));
        }
    }
}
