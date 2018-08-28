using System;
using Interpose.Core.Interceptors;

namespace Interpose.Core.Handlers
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class CallbackInterceptionAttribute : InterceptionAttribute, IHandlerFactory
    {
        public CallbackInterceptionAttribute(Type type, string methodName, object key = null) : base(typeof(CallbackInterceptionHandler))
        {
            this.Type = type;
            this.MethodName = methodName;
            this.Key = key;
        }

        public Type Type { get; }
        public string MethodName { get; }
        public object Key { get; }

        public IInterceptionHandler Instantiate(IServiceProvider serviceProvider)
        {
            return new CallbackInterceptionHandler(serviceProvider, this.Type, this.MethodName, this.Key);
        }
    }
}
