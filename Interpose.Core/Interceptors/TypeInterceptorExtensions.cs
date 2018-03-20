using Interpose.Core.Handlers;
using System;

namespace Interpose.Core.Interceptors
{
	public static class TypeInterceptorExtensions
	{
		public static Type Intercept<TToIntercept, TInterceptor>(this ITypeInterceptor interceptor) where TInterceptor : IInterceptionHandler, new()
		{
			return interceptor.Intercept(typeof(TToIntercept), typeof(TInterceptor));
		}

        public static Type InterceptWithAttributes(this ITypeInterceptor interceptor, Type type)
        {
            return interceptor.Intercept(type, typeof(AttributesInterceptionHandler));
        }

        public static Type InterceptWithAttributes<TService>(this ITypeInterceptor interceptor) where TService : class
        {
            return InterceptWithAttributes(interceptor, typeof(TService));
        }
    }
}
