using System;
using Interpose.Core.Interceptors;

namespace Interpose.Core.Handlers
{
    public sealed class DelegateInterceptionHandler : IInterceptionHandler
    {
        private readonly Action<InterceptionArgs> action;

        public DelegateInterceptionHandler(Action<InterceptionArgs> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.action = action;
        }

        public void Invoke(InterceptionArgs arg)
        {
            this.action(arg);
        }

        public static implicit operator DelegateInterceptionHandler(Action<InterceptionArgs> action)
        {
            return new DelegateInterceptionHandler(action);
        }
    }
}
