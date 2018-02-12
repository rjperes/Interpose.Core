using Interpose.Core.Interceptors;

namespace Interpose.Core.Tests
{
    public class ConsoleLogInterceptionAttribute : InterceptionAttribute
	{
		public ConsoleLogInterceptionAttribute() : base(typeof(ConsoleInterceptionHandler))
		{
		}
	}
}
