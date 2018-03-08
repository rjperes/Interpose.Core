using Interpose.Core.Generators;
using Interpose.Core.Handlers;
using Interpose.Core.Proxies;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Interpose.Core.Interceptors
{
    /// <summary>
    /// Intercepts instances implementing interfaces.
    /// </summary>
	public sealed class InterfaceInterceptor : IInstanceInterceptor
	{
		private readonly InterceptedTypeGenerator generator;
        private readonly IServiceProvider serviceProvider;

        public InterfaceInterceptor(InterceptedTypeGenerator generator)
		{
			this.generator = generator;
		}

        public InterfaceInterceptor(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.generator = serviceProvider.GetRequiredService<InterceptedTypeGenerator>();
        }

		public InterfaceInterceptor() : this(RoslynInterceptedTypeGenerator.Instance)
		{
		}

        private object Instantiate(Type type, params object [] arguments)
        {
            object instance = null;

            if (this.serviceProvider == null)
            {
                instance = Activator.CreateInstance(type, arguments);
            }
            else
            {
                instance = ActivatorUtilities.CreateInstance(this.serviceProvider, type, arguments);
            }

            return instance;
        }

        public object Intercept(object target, Type typeToIntercept, IInterceptionHandler handler)
		{
			if (target == null)
			{
				throw new ArgumentNullException(nameof(target));
			}

			if (typeToIntercept == null)
			{
				throw new ArgumentNullException(nameof(typeToIntercept));
			}

			if (handler == null)
			{
				throw new ArgumentNullException(nameof(handler));
			}

			if (typeToIntercept.IsInstanceOfType(target) == false)
			{
				throw new ArgumentException("Instance to intercept does not implement interception interface", nameof(target));
			}

			if (typeToIntercept.IsInterface == false)
			{
				throw new ArgumentException("Type to intercept is not an interface", nameof(typeToIntercept));
			}

			if (this.CanIntercept(target) == false)
			{
				throw new ArgumentException("Type to intercept cannot be intercepted with this interceptor", nameof(target));
			}

			var interfaceProxyType = this.generator.Generate(this, typeof(InterfaceProxy), null, typeToIntercept);

			var newInstance = this.Instantiate(interfaceProxyType, this, handler, target);

			return newInstance;
		}

		public bool CanIntercept(object instance)
		{
			return instance.GetType().GetInterfaces().Any() == true;
		}
	}
}
