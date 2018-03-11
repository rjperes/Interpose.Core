using Interpose.Core.Interceptors;

namespace Interpose.Core.Handlers
{
    public sealed class SelfInterceptionHandler : IInterceptionHandler
    {
        public void Invoke(InterceptionArgs arg)
        {
            if (arg.Target is IInterceptionHandler)
            {
                (arg.Target as IInterceptionHandler).Invoke(arg);
            }
        }
    }
}
