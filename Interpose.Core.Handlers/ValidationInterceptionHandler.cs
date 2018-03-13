using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Interpose.Core.Interceptors;

namespace Interpose.Core.Handlers
{

    public sealed class ValidationInterceptionHandler : IInterceptionHandler
    {
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
            Validator.ValidateObject(arg.Target, new ValidationContext(arg.Target, this.serviceProvider, new Dictionary<object, object>()));
        }
    }
}
