using Interpose.Core.Interceptors;
using System;
using System.Linq;

namespace Interpose.Core.Handlers
{
	public sealed class AttributesInterceptionHandler : IInterceptionHandler
	{
		public static readonly IInterceptionHandler Instance = new AttributesInterceptionHandler();

		public void Invoke(InterceptionArgs arg)
		{
			var attrs = arg.Target.GetType().GetCustomAttributes(true).OfType<InterceptionAttribute>().OrderBy(x => x.Order);

			foreach (var attr in attrs)
			{
				var handlerType = attr.InterceptionHandlerType;

				if (handlerType != null)
				{
					var handler = Activator.CreateInstance(handlerType) as IInterceptionHandler;

					if (handler != null)
					{
						handler.Invoke(arg);
					}
				}
			}
		}
	}
}
