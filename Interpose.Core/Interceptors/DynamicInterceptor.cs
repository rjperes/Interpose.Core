using Interpose.Core.Handlers;
using Interpose.Core.Proxies;
using System;

namespace Interpose.Core.Interceptors
{
    /// <summary>
    /// Intercepts any object.
    /// </summary>
	public sealed class DynamicInterceptor : IInstanceInterceptor
	{
		public static readonly IInstanceInterceptor Instance = new DynamicInterceptor();

		public object Intercept(object target, Type typeToIntercept, IInterceptionHandler handler)
		{
			if (target == null)
			{
				throw new ArgumentNullException(nameof(target));
			}

			if (typeToIntercept == null)
			{
				throw new ArgumentNullException(nameof(typeToIntercept));
			}

			if (handler == null)
			{
				throw new ArgumentNullException(nameof(handler));
			}

			if (typeToIntercept.IsInstanceOfType(target) == false)
			{
				throw new ArgumentException("Instance to intercept does not implement interception interface", nameof(target));
			}

			if (this.CanIntercept(target) == false)
			{
				throw new ArgumentException("Type to intercept cannot be intercepted with this interceptor", nameof(target));
			}

			return new DynamicProxy(this, target, handler);
		}

		public bool CanIntercept(object instance)
		{
			return instance != null;
		}
	}
}
