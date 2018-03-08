using Interpose.Core.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Interpose.Core.Handlers
{
	public sealed class AttributesInterceptionHandler : IInterceptionHandler, IDisposable
	{
		public static readonly IInterceptionHandler Instance = new AttributesInterceptionHandler();

        private readonly Dictionary<Type, IInterceptionHandler> handlers = new Dictionary<Type, IInterceptionHandler>();
        private readonly IServiceProvider serviceProvider;

        public AttributesInterceptionHandler()
        {
        }

        public AttributesInterceptionHandler(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void Dispose()
        {
            this.handlers.Clear();
        }

        private IInterceptionHandler Instantiate(Type handlerType)
        {
            if (this.handlers.TryGetValue(handlerType, out var handler) == false)
            {
                if (this.serviceProvider == null)
                {
                    handler = Activator.CreateInstance(handlerType) as IInterceptionHandler;
                }
                else
                {
                    handler = ActivatorUtilities.CreateInstance(this.serviceProvider, handlerType) as IInterceptionHandler;
                }

                handlers[handlerType] = handler;
            }

            return handler;
        }

        public void Invoke(InterceptionArgs arg)
		{
			var attrs = arg
                .Target
                .GetType()
                .GetCustomAttributes(true)
                .Concat(arg.Method.GetCustomAttributes(true))
                .OfType<InterceptionAttribute>()
                .OrderBy(x => x.Order);

			foreach (var attr in attrs)
			{
				var handlerType = attr.InterceptionHandlerType;

                if (handlerType != null)
				{
                    var handler = this.Instantiate(handlerType);

					if (handler != null)
					{
						handler.Invoke(arg);
					}
				}
			}
		}
	}
}
