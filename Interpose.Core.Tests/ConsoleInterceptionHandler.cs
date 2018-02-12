using Interpose.Core.Handlers;
using Interpose.Core.Interceptors;
using System;

namespace Interpose.Core.Tests
{
    internal class ConsoleInterceptionHandler : IInterceptionHandler
    {
        public void Invoke(InterceptionArgs arg)
        {
            Console.Out.WriteLine($"Calling {arg.Method}");
        }
    }
}