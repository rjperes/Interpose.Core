using Interpose.Core.Interceptors;
using System;

namespace Interpose.Core.Generators
{
	public abstract class InterceptedTypeGenerator
	{
		public abstract Type Generate(IInterceptor interceptor, Type baseType, Type handlerType, params Type [] additionalInterfaceTypes);
	}
}
