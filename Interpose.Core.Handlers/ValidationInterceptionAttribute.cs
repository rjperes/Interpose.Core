using System;
using Interpose.Core.Interceptors;

namespace Interpose.Core.Handlers
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ValidationInterceptionAttribute : InterceptionAttribute, IHandlerFactory
    {
        public ValidationInterceptionAttribute() : base(typeof(ValidationInterceptionHandler))
        {
        }

        public IInterceptionHandler Instantiate(IServiceProvider serviceProvider)
        {
            return new ValidationInterceptionHandler(serviceProvider);
        }
    }
}
