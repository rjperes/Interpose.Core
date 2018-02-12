using Interpose.Core.Handlers;
using System;
using System.Linq;

namespace Interpose.Core.Interceptors
{
	public static class InterceptionExtensions
	{
		public static readonly IInterceptor[] Interceptors = typeof(IInterceptor).Assembly.GetExportedTypes().Where(t => (typeof(IInterceptor).IsAssignableFrom(t) == true) && (t.IsInterface == false) && (t.IsAbstract == false)).Select(t => Activator.CreateInstance(t)).OfType<IInterceptor>().ToArray();
		public static readonly IInstanceInterceptor[] InstanceInterceptors = Interceptors.OfType<IInstanceInterceptor>().ToArray();
		public static readonly ITypeInterceptor[] TypeInterceptors = Interceptors.OfType<ITypeInterceptor>().ToArray();

		public static T Intercept<T, THandler>(this T instance) where T : class where THandler : IInterceptionHandler, new()
		{
			return Intercept<T>(instance, new THandler());
		}

		public static T Intercept<T>(this T instance, IInterceptionHandler handler) where T : class
		{
			foreach (var interceptor in InstanceInterceptors)
			{
				if (interceptor.CanIntercept(instance) == true)
				{
					var proxy = interceptor.Intercept(instance, handler);

					return proxy;
				}
			}

			throw new ArgumentException("Could not find an interceptor for given instance.", nameof(instance));
		}

		public static Type Intercept<THandler>(this Type type) where THandler : IInterceptionHandler, new()
		{
			return Intercept(type, typeof(THandler));
		}

		public static Type Intercept(this Type type, Type handlerType)
		{
			foreach (var interceptor in TypeInterceptors)
			{
				if (interceptor.CanIntercept(type) == true)
				{
					var proxy = interceptor.Intercept(type, handlerType);

					return proxy;
				}
			}

			throw new ArgumentException("Could not find an interceptor for given type.", nameof(type));
		}
	}
}
