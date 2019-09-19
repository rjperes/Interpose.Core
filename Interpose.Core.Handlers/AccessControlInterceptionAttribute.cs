using Interpose.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interpose.Core.Handlers
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AccessControlInterceptionAttribute : InterceptionAttribute, IHandlerFactory
    {
        public AccessControlInterceptionAttribute(params string [] roles) : base(typeof(AccessControlInterceptionHandler))
        {
            this.Roles = roles;
        }

        public string [] Roles { get; }

        public IInterceptionHandler Instantiate(IServiceProvider serviceProvider)
        {
            return new AccessControlInterceptionHandler(this.Roles);
        }
    }
}
