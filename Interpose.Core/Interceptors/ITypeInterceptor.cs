using System;

namespace Interpose.Core.Interceptors
{
	public interface ITypeInterceptor : IInterceptor
	{
		Type Intercept(Type typeToIntercept, Type interceptionType);

		bool CanIntercept(Type typeToIntercept);
	}
}
