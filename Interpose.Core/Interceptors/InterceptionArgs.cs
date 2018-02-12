using System;
using System.Reflection;

namespace Interpose.Core.Interceptors
{
	[Serializable]
	public sealed class InterceptionArgs : EventArgs
	{
		private object result;

		public InterceptionArgs(object target, MethodInfo method, Func<object> baseMethod, object[] arguments)
		{
			this.Target = target;
            this.Method = method;
			this.Arguments = arguments;
			this.BaseMethod = baseMethod;
		}

		public void Proceed()
		{
			if (this.BaseMethod != null)
			{
				this.Result = this.BaseMethod();
			}
		}

		public object Target { get; }
		public object[] Arguments { get; }
        public MethodInfo Method { get; }
		public Func<object> BaseMethod { get; }
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
