using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Interpose.Core.Interceptors;

namespace Interpose.Core.Handlers
{

    public sealed class ValidationInterceptionHandler : IInterceptionHandler
    {
        public static readonly IInterceptionHandler Instance = new ValidationInterceptionHandler();
        private readonly IServiceProvider serviceProvider;

        public ValidationInterceptionHandler()
        {
        }

        public ValidationInterceptionHandler(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Invoke(InterceptionArgs arg)
        {
            Array.ForEach(arg.Arguments, x => Validator.ValidateObject(x, new ValidationContext(x, this.serviceProvider, new Dictionary<object, object>())));

            Validator.ValidateObject(arg.Target, new ValidationContext(arg.Target, this.serviceProvider, new Dictionary<object, object>()));
        }
    }
}
