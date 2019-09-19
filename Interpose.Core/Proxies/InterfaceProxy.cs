using Interpose.Core.Handlers;
using Interpose.Core.Interceptors;

namespace Interpose.Core.Proxies
{
	public abstract class InterfaceProxy
	{
		protected readonly object target;
		protected readonly IInterceptor interceptor;
		protected readonly IInterceptionHandler handler;

		protected InterfaceProxy(IInterceptor interceptor, IInterceptionHandler handler, object target)
		{
			this.target = target;
			this.interceptor = interceptor;
			this.handler = handler;
		}
	}
}
