using Interpose.Core.Handlers;
using Interpose.Core.Interceptors;
using System;
using System.Dynamic;

namespace Interpose.Core.Proxies
{
	internal sealed class DynamicProxy : DynamicObject, IInterceptionProxy
	{
		private readonly IInterceptionHandler handler;
		private readonly object target;

		public DynamicProxy(IInterceptor interceptor, object target, IInterceptionHandler handler)
		{
			if (interceptor == null)
			{
				throw new ArgumentNullException("target");
			}

			if (target == null)
			{
				throw new ArgumentNullException("target");
			}

			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}

			this.Interceptor = interceptor;
			this.target = target;
			this.handler = handler;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			var method = this.target.GetType().GetMethod(binder.Name);
			var arg = new InterceptionArgs(this.target, method, args);

			this.handler.Invoke(arg);

			if (arg.Handled == true)
			{
				result = arg.Result;
			}
			else
			{
				result = method.Invoke(this.target, args);
			}

			return true;
		}

		public IInterceptor Interceptor { get; private set; }
	}
}