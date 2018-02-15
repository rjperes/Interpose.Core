using System;

namespace Interpose.Core.Interceptors
{
    /// <summary>
    /// Contract for a type interceptor.
    /// </summary>
	public interface ITypeInterceptor : IInterceptor
	{
        /// <summary>
        /// Creates an intercepted type proxy.
        /// </summary>
        /// <param name="typeToIntercept">The type to intercept.</param>
        /// <param name="handlerType">The interception handler type.</param>
        /// <returns>An interception proxy.</returns>
		Type Intercept(Type typeToIntercept, Type handlerType);

        /// <summary>
        /// Checks if a type can be intercepted.
        /// </summary>
        /// <param name="typeToIntercept">The type to intercept.</param>
        /// <returns>True if the type can be intercepted, false otherwise.</returns>
		bool CanIntercept(Type typeToIntercept);
	}
}
