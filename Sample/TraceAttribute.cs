using Interpose.Core.Handlers;
using Interpose.Core.Interceptors;
using System;

namespace Sample
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class TraceAttribute : InterceptionAttribute, IHandlerFactory, IInterceptionHandler
    {
        public TraceAttribute() : base(typeof(TraceAttribute))
        {
        }

        public IInterceptionHandler Instantiate(IServiceProvider serviceProvider)
        {
            return this;
        }

        public void Invoke(InterceptionArgs arg)
        {
            System.Diagnostics.Trace.WriteLine("Before call");
            arg.Proceed();
            System.Diagnostics.Trace.WriteLine("After call");
        }
    }
}