using Interpose.Core.Generators;
using Interpose.Core.Handlers;
using Interpose.Core.Proxies;
using System;
using System.Linq;

namespace Interpose.Core.Interceptors
{
	public sealed class InterfaceInterceptor : IInstanceInterceptor
	{
		private readonly InterceptedTypeGenerator generator;

		public InterfaceInterceptor(InterceptedTypeGenerator generator)
		{
			this.generator = generator;
		}

		public InterfaceInterceptor() : this(RoslynInterceptedTypeGenerator.Instance)
		{
		}

		public object Intercept(object instance, Type typeToIntercept, IInterceptionHandler handler)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if (typeToIntercept == null)
			{
				throw new ArgumentNullException(nameof(typeToIntercept));
			}

			if (handler == null)
			{
				throw new ArgumentNullException(nameof(handler));
			}

			if (typeToIntercept.IsInstanceOfType(instance) == false)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if (typeToIntercept.IsInterface == false)
			{
				throw new ArgumentNullException(nameof(typeToIntercept));
			}

			if (this.CanIntercept(instance) == false)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			var interfaceProxy = this.generator.Generate(this, typeof(InterfaceProxy), null, typeToIntercept);

			var newInstance = Activator.CreateInstance(interfaceProxy, this, handler, instance);

			return newInstance;
		}

		public bool CanIntercept(object instance)
		{
			return instance.GetType().GetInterfaces().Any() == true;
		}
	}
}
