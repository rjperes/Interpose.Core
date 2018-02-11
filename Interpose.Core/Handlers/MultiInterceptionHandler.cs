using Interpose.Core.Interceptors;
using System.Collections.Generic;

namespace Interpose.Core.Handlers
{
	public sealed class MultiInterceptionHandler : IInterceptionHandler
	{
		public IList<IInterceptionHandler> Handlers { get; } = new List<IInterceptionHandler>();

		public void Invoke(InterceptionArgs arg)
		{
			for (var i = 0; i < this.Handlers.Count; ++i)
			{
				this.Handlers[i].Invoke(arg);

				if (arg.Handled == true)
				{
					break;
				}
			}
		}
	}
}
