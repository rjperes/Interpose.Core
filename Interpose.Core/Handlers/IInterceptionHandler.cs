
using Interpose.Core.Interceptors;

namespace Interpose.Core.Handlers
{
	public interface IInterceptionHandler
	{
		void Invoke(InterceptionArgs arg);
	}
}
