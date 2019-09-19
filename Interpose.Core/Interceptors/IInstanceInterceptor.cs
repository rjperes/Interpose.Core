using Interpose.Core.Handlers;
using System;

namespace Interpose.Core.Interceptors
{
    /// <summary>
    /// Contract for an instance interceptor.
    /// </summary>
	public interface IInstanceInterceptor : IInterceptor
	{
        /// <summary>
        /// Creates an intercepted proxy.
        /// </summary>
        /// <param name="target">The object to intercept.</param>
        /// <param name="typeToIntercept">The type to intercept.</param>
        /// <param name="handler">The interception handler.</param>
        /// <returns>The intercepted proxy.</returns>
		object Intercept(object target, Type typeToIntercept, IInterceptionHandler handler);

        /// <summary>
        /// Checks if an object can be intercepted.
        /// </summary>
        /// <param name="target">The object to intercept.</param>
        /// <returns>True if the object can be intercepted, false otherwise.</returns>
		bool CanIntercept(object target);
	}
}
