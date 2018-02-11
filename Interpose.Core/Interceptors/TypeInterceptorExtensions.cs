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
	}
}
