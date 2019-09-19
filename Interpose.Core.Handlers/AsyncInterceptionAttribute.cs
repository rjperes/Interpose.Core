using Interpose.Core.Interceptors;
using System;

namespace Interpose.Core.Handlers
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class AsyncInterceptionAttribute : InterceptionAttribute
    {
        public AsyncInterceptionAttribute() : base(typeof(AsyncInterceptionHandler))
        {
        }
    }
}
