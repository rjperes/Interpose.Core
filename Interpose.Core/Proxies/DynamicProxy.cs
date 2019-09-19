using Interpose.Core.Handlers;
using Interpose.Core.Interceptors;
using System;
using System.Dynamic;

namespace Interpose.Core.Proxies
{
	internal sealed class DynamicProxy : DynamicObject, IInterceptionProxy
	{
		private readonly IInterceptionHandler handler;

		public DynamicProxy(IInterceptor interceptor, object target, IInterceptionHandler handler)
		{
			if (interceptor == null)
			{
				throw new ArgumentNullException(nameof(interceptor));
			}

			if (target == null)
			{
				throw new ArgumentNullException(nameof(target));
			}

			if (handler == null)
			{
				throw new ArgumentNullException(nameof(handler));
			}

			this.Interceptor = interceptor;
			this.Target = target;
			this.handler = handler;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			var method = this.Target.GetType().GetMethod(binder.Name);
            Func<object> handler = () => { return method.Invoke(this.Target, args); };
			var arg = new InterceptionArgs(this.Target, method, handler, args);

			this.handler.Invoke(arg);

			if (arg.Handled == true)
			{
				result = arg.Result;
			}
			else
			{
				result = method.Invoke(this.Target, args);
			}

			return true;
		}

		public IInterceptor Interceptor { get; }
        public object Target { get; }
    }
}