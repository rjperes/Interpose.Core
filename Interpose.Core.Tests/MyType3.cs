namespace Interpose.Core.Tests
{
	public class MyType3 : IMyType
	{
		public virtual string MyProperty { get; set; }

		[ConsoleLogInterception]
        [ModifyResultInterception]
		public virtual int MyMethod() => 0;
	}
}
