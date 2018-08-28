using System;
using System.Reflection;
using Interpose.Core.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace Interpose.Core.Handlers
{
    public sealed class CallbackInterceptionHandler : IInterceptionHandler
    {
        public CallbackInterceptionHandler(IServiceProvider serviceProvider, Type type, string methodName, object key = null)
        {
            this._serviceProvider = serviceProvider;
            this._type = type;
            this._methodName = methodName;
            this._key = key;
        }

        public CallbackInterceptionHandler(Type type, string methodName, object key = null) : this(null, type, methodName, key)
        {
        }

        public CallbackInterceptionHandler(Action action)
        {
            this._action = action;
        }

        public CallbackInterceptionHandler(MethodInfo method)
        {
            this._methodName = method.Name;
            this._type = method.DeclaringType;
        }

        private readonly IServiceProvider _serviceProvider;
        private readonly Action _action;
        private Type _type { get; }
        private string _methodName { get; }
        private object _key { get; }

        public void Invoke(InterceptionArgs arg)
        {
            arg.Proceed();

            if (this._action == null)
            {
                var method = this._type.GetMethod(this._methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);
                object[] parameters = (method.GetParameters().Length == 0) ? null : new[] { this._key };
                object instance = null;

                if (method.IsStatic == false)
                {
                    instance = (this._serviceProvider == null) ? Activator.CreateInstance(this._type) : ActivatorUtilities.CreateInstance(this._serviceProvider, this._type);
                }

                method.Invoke(instance, parameters);
            }
            else
            {
                this._action();
            }
        }
    }
}