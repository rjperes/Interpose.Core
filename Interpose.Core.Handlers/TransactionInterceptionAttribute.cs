using System;
using System.Transactions;
using Interpose.Core.Interceptors;

namespace Interpose.Core.Handlers
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class TransactionInterceptionAttribute : InterceptionAttribute, IHandlerFactory
    {
        public TransactionInterceptionAttribute(TransactionScopeAsyncFlowOption asyncFlowOption, TransactionScopeOption scopeOption, TimeSpan timeout, IsolationLevel isolationLevel) : base(typeof(TransactionInterceptionHandler))
        {
            this.AsyncFlowOption = asyncFlowOption;
            this.ScopeOption = scopeOption;
            this.Timeout = timeout;
            this.IsolationLevel = IsolationLevel;
        }

        public TransactionScopeAsyncFlowOption AsyncFlowOption { get; }
        public TransactionScopeOption ScopeOption { get; }
        public TimeSpan Timeout { get; }
        public IsolationLevel IsolationLevel { get; }

        public IInterceptionHandler Instantiate(IServiceProvider serviceProvider)
        {
            return new TransactionInterceptionHandler(this.AsyncFlowOption, this.ScopeOption, this.Timeout, this.IsolationLevel);
        }
    }
}
