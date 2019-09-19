using System;
using System.Reflection;
using Interpose.Core.Handlers;
using Interpose.Core.Interceptors;

namespace Interpose.Core.Proxies
{
    public class DispatchProxy<T> : DispatchProxy, IInterceptionProxy
    {
        private IInterceptionHandler handler;
        private object target;
        private IInterceptor interceptor;

        public IInterceptor Interceptor => interceptor;

        public object Target => target;

        public static T Create(T target, IInterceptionHandler handler, IInterceptor interceptor)
        {
            var proxy = Create<T, DispatchProxy<T>>();
            (proxy as DispatchProxy<T>).Initialize(target, handler, interceptor);

            return (T) proxy;
        }

        private void Initialize(object target, IInterceptionHandler handler, IInterceptor interceptor)
        {
            this.target = target;
            this.handler = handler;
            this.interceptor = interceptor;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            InterceptionArgs arg = null;
            arg = new InterceptionArgs(this.target, targetMethod, () =>
            {
                arg.Result = targetMethod.Invoke(this.target, args);
                return arg.Result;
            }, args);
            this.handler.Invoke(arg);
            return arg.Result;
        }
    }
}
