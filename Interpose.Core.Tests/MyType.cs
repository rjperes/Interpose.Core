namespace Interpose.Core.Tests
{
	public class MyType : IMyType
	{
		public virtual string MyProperty { get; set; }

		public virtual int MyMethod() => 0;
	}
}
