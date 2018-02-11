using Interpose.Core.Handlers;
using System;

namespace Interpose.Core.Interceptors
{
	public interface IInstanceInterceptor : IInterceptor
	{
		object Intercept(object instance, Type typeToIntercept, IInterceptionHandler handler);

		bool CanIntercept(object instance);
	}
}
