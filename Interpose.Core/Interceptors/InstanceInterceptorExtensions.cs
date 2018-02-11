using Interpose.Core.Handlers;
using System;
using System.Linq;

namespace Interpose.Core.Interceptors
{
	public static class InstanceInterceptorExtensions
	{
		public static object InterceptWithAttributes(this IInstanceInterceptor interceptor, object instance)
		{
			return Intercept(interceptor, instance, AttributesInterceptionHandler.Instance);
		}

		public static object Intercept(this IInstanceInterceptor interceptor, object instance, IInterceptionHandler handler)
		{
			return interceptor.Intercept(instance, instance.GetType().GetInterfaces().First(), handler);
		}

		public static T Intercept<T>(this IInstanceInterceptor interceptor, T instance, IInterceptionHandler handler)
		{
			return (T) interceptor.Intercept((object) instance, typeof(T), handler);
		}
	}
}
