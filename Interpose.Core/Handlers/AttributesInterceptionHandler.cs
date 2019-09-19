using Interpose.Core.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        private IEnumerable<InterceptionAttribute> GetInterceptionAttributes(Type type)
        {
            if (type == typeof(object))
            {
                return new InterceptionAttribute[0];
            }

            return type
                .GetCustomAttributes<InterceptionAttribute>(true);
        }


        public void Invoke(InterceptionArgs arg)
		{
            var methodAttrs = arg
                .Method
                .GetCustomAttributes<InterceptionAttribute>(true)
                .OrderByDescending(x => x.Order);

            var typeAttrs = this.GetInterceptionAttributes(arg.Target.GetType())
                .OrderByDescending(x => x.Order);

            var interfaceAttrs = arg
                .Target
                .GetType()
                .GetInterfaces()
                .SelectMany(x => this.GetInterceptionAttributes(x))
                .OrderByDescending(x => x.Order);

            var attrs = methodAttrs
                .Concat(typeAttrs)
                .Concat(interfaceAttrs);

            foreach (var attr in attrs)
			{
                IInterceptionHandler handler = null;

                if (attr is IHandlerFactory)
                {
                    handler = (attr as IHandlerFactory).Instantiate(this.serviceProvider);
                }
                else
                {
                    var handlerType = attr.InterceptionHandlerType;

                    if (handlerType != null)
                    {
                        handler = this.Instantiate(handlerType);
                    }
                }

                if (handler != null)
                {
                    handler.Invoke(arg);
                }
            }
        }
	}
}
