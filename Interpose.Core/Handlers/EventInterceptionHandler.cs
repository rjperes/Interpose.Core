using Interpose.Core.Interceptors;
using System;

namespace Interpose.Core.Handlers
{
	public sealed class EventInterceptionHandler : IInterceptionHandler
	{
		public event EventHandler<InterceptionArgs> Interception;

		public void Invoke(InterceptionArgs arg)
		{
			var handler = this.Interception;

			if (handler != null)
			{
				handler(this, arg);
			}
		}
	}
}
