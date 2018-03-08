using Interpose.Core.Interceptors;
using System;
using System.Collections.Generic;

namespace Interpose.Core.Handlers
{
	public sealed class MultiInterceptionHandler : IInterceptionHandler
	{
		public List<IInterceptionHandler> Handlers { get; } = new List<IInterceptionHandler>();

        public MultiInterceptionHandler(params IInterceptionHandler [] handlers)
        {
            this.Handlers.AddRange(handlers ?? new IInterceptionHandler[0]);
        }

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

        public static MultiInterceptionHandler operator + (MultiInterceptionHandler multi, IInterceptionHandler handler)
        {
            if (multi == null)
            {
                throw new ArgumentNullException(nameof(multi));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            multi.Handlers.Add(handler);
            return multi;
        }
    }
}
