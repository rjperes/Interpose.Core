using System;
using System.Reflection;

namespace Interpose.Core.Interceptors
{
	[Serializable]
	public sealed class InterceptionArgs : EventArgs
	{
		private object result;

        public InterceptionArgs(object target, Func<object> baseMethod, object[] arguments): this(target, (MethodInfo) null, arguments)
        {
            this.BaseMethod = baseMethod;
        }

        public InterceptionArgs(object target, MethodInfo method, object[] arguments)
		{
			this.Target = target;
			this.Method = method;
			this.Arguments = arguments;
		}

		public void Proceed()
		{
            if (this.BaseMethod != null)
            {
                this.Result = this.BaseMethod();
            }
            else
            {
                this.Result = this.Method.Invoke(this.Target, this.Arguments);
            }
		}

		public object Target { get; }
		public MethodInfo Method { get; }
		public object[] Arguments { get; }
		public bool Handled { get; set; }
        public Func<object> BaseMethod { get; }

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
