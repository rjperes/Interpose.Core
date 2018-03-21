using Interpose.Core.Interceptors;
using System;
using System.Linq;
using System.Security.Claims;

namespace Interpose.Core.Handlers
{
    public sealed class AccessControlInterceptionHandler : IInterceptionHandler
    {
        private readonly string[] roles;

        public AccessControlInterceptionHandler(params string [] roles)
        {
            if (roles == null)
            {
                throw new ArgumentNullException(nameof(roles));
            }

            this.roles = roles;
        }

        public void Invoke(InterceptionArgs arg)
        {
            var principal = ClaimsPrincipal.Current;

            if ((principal == null) || (!this.roles.Any(role => principal.IsInRole(role))))
            {
                throw new InvalidOperationException("Attepted to access protected method without needed security.");
            }

            arg.Proceed();
        }
    }
}