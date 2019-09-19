
using Interpose.Core.Interceptors;

namespace Interpose.Core.Proxies
{
	public interface IInterceptionProxy
	{
		IInterceptor Interceptor { get; }
		object Target { get; }
	}
}
