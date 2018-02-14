using System;
using System.Reflection;

namespace Interpose.Core.Interceptors
{
    [Serializable]
    public sealed class InterceptionArgs : EventArgs
    {
        private object result;
        private readonly Func<object> baseMethod;

        public InterceptionArgs(object target, MethodInfo method, Func<object> baseMethod, object[] arguments)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (baseMethod == null)
            {
                throw new ArgumentNullException(nameof(baseMethod));
            }

            this.Target = target;
            this.Method = method;
            this.Arguments = arguments ?? new object[0];
            this.baseMethod = baseMethod;
        }

        public void Proceed()
        {
            this.Result = this.baseMethod();
        }

        public object Target { get; }
        public object[] Arguments { get; }
        public MethodInfo Method { get; }
        public bool Handled { get; set; }

        public object Result
        {
            get
            {
                return this.result;
            }
            set
            {
                this.result = value;
                this.Handled = true;
            }
        }
    }
}
