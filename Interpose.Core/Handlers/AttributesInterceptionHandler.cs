using Interpose.Core.Interceptors;
using System.Linq;

namespace Interpose.Core.Handlers
{
	public sealed class AttributesInterceptionHandler : IInterceptionHandler
	{
		public static readonly IInterceptionHandler Instance = new AttributesInterceptionHandler();

		public void Invoke(InterceptionArgs arg)
		{
			var attrs = arg.Method.GetCustomAttributes(true).OfType<InterceptionAttribute>().OrderBy(x => x.Order).Cast<IInterceptionHandler>();

			foreach (var attr in attrs)
			{
				attr.Invoke(arg);
			}
		}
	}
}
