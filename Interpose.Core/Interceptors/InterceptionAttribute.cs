using System;

namespace Interpose.Core.Interceptors
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public class InterceptionAttribute : Attribute
	{
        public InterceptionAttribute(Type interceptionHandlerType)
        {
            this.InterceptionHandlerType = interceptionHandlerType;
        }

		public int Order { get; set; }

        public Type InterceptionHandlerType { get; }
	}
}
