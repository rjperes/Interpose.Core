using Interpose.Core.Interceptors;
using System.Threading;
using System.Threading.Tasks;

namespace Interpose.Core.Handlers
{

    public sealed class AsyncInterceptionHandler : IInterceptionHandler
    {
        public static readonly IInterceptionHandler Instance = new AsyncInterceptionHandler();

        public void Invoke(InterceptionArgs arg)
        {
            if (arg.Method.ReturnType == typeof(void))
            {
                new Thread(() =>
                {
                    arg.Proceed();
                }).Start();
            }
        }
    }
}
