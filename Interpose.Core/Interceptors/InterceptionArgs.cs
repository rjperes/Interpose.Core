using System;
using System.Reflection;

namespace Interpose.Core.Interceptors
{
	[Serializable]
	public sealed class InterceptionArgs : EventArgs
	{
		private object result;

        public InterceptionArgs(object instance, Func<object> baseMethod, object[] arguments): this(instance, (MethodInfo) null, arguments)
        {
            this.BaseMethod = baseMethod;
        }

        public InterceptionArgs(object instance, MethodInfo method, object[] arguments)
		{
			this.Instance = instance;
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
                this.Result = this.Method.Invoke(this.Instance, this.Arguments);
            }
		}

		public object Instance { get; }
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
