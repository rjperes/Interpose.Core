using Interpose.Core.Interceptors;
using System;

namespace Interpose.Core.Handlers
{
	internal sealed class ConditionalInterceptionHandler : IInterceptionHandler
	{
		private readonly Func<InterceptionArgs, bool> condition;
		private readonly IInterceptionHandler handler;

		public ConditionalInterceptionHandler(Func<InterceptionArgs, bool> condition, IInterceptionHandler handler)
		{
			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			if (handler == null)
			{
				throw new ArgumentNullException(nameof(handler));
			}

			this.condition = condition;
			this.handler = handler;
		}

		public void Invoke(InterceptionArgs arg)
		{
			if (this.condition(arg) == true)
			{
				this.handler.Invoke(arg);
			}
		}
	}
}
