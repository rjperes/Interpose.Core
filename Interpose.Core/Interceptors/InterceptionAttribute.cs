using Interpose.Core.Handlers;
using System;

namespace Interpose.Core.Interceptors
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public abstract class InterceptionAttribute : Attribute, IInterceptionHandler
	{
		public int Order { get; set; }

		public abstract void Invoke(InterceptionArgs arg);
	}
}
