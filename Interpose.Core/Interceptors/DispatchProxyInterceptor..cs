using Interpose.Core.Handlers;
using Interpose.Core.Proxies;
using System;
using System.Linq;
using System.Reflection;

namespace Interpose.Core.Interceptors
{
    public class DispatchProxyInterceptor : IInstanceInterceptor
    {
        public static readonly IInstanceInterceptor Instance = new DispatchProxyInterceptor();

        public bool CanIntercept(object target)
        {
            return target.GetType().GetInterfaces().Any() == true;
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

            var proxy = typeof(DispatchProxy<>).MakeGenericType(typeToIntercept).GetMethod(nameof(DispatchProxy<object>.Create), BindingFlags.Static | BindingFlags.Public).Invoke(null, new[] { target, handler, this });
            return proxy;
        }
    }
}
