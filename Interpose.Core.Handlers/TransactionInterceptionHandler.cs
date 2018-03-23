using System;
using System.Transactions;
using Interpose.Core.Interceptors;

namespace Interpose.Core.Handlers
{

    public sealed class TransactionInterceptionHandler : IInterceptionHandler
    {
        private readonly TransactionScopeAsyncFlowOption asyncFlowOption;
        private readonly TransactionScopeOption scopeOption;
        private readonly TimeSpan timeout = TimeSpan.FromMinutes(1);
        private readonly IsolationLevel isolationLevel;

        public TransactionInterceptionHandler()
        {
        }

        public TransactionInterceptionHandler(TimeSpan timeout, TransactionScopeAsyncFlowOption asyncFlowOption = TransactionScopeAsyncFlowOption.Suppress, TransactionScopeOption scopeOption = TransactionScopeOption.Required, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            this.asyncFlowOption = asyncFlowOption;
            this.scopeOption = scopeOption;
            this.timeout = timeout;
            this.isolationLevel = isolationLevel;
        }

        public void Invoke(InterceptionArgs arg)
        {
            var options = new TransactionOptions() { IsolationLevel = this.isolationLevel, Timeout = this.timeout };
            new TransactionScope(this.scopeOption, options, this.asyncFlowOption);
        }
    }
}
