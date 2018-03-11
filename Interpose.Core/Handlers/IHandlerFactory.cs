using System;

namespace Interpose.Core.Handlers
{
    public interface IHandlerFactory
    {
        IInterceptionHandler Instantiate(IServiceProvider serviceProvider);
    }
}
