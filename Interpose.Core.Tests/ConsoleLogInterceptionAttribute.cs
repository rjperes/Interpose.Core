using Interpose.Core.Interceptors;
using System;

namespace Interpose.Core.Tests
{
	public class ConsoleLogInterceptionAttribute : InterceptionAttribute
	{
		public override void Invoke(InterceptionArgs arg)
		{
			Console.Out.WriteLine("Calling " + arg.Method);
		}
	}
}
