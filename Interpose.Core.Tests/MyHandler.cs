using Interpose.Core.Handlers;
using Interpose.Core.Interceptors;

namespace Interpose.Core.Tests
{
	public class MyHandler : IInterceptionHandler
	{
		public void Invoke(InterceptionArgs arg)
		{
			//call base implementation
			arg.Proceed();
			//then change the result
			arg.Result = 20;
		}
	}
}
