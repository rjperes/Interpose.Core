namespace Interpose.Core.Tests
{
    class Program
    {
        static void Main()
        {
            var tests = new InterceptionTests();
            //handlers
            tests.CanValidate();
            tests.CanCreateTransactions();
            tests.CanCache();
            tests.CanLog();
            tests.CanRetry();
            tests.CanMakeAsync();
            //standatd
            tests.CanDoVirtualInterception();
            tests.CanDoDynamicInterception();
            tests.CanDoInterfaceInterception();
            tests.CanDoDynamicInterceptionWithAttributes();
            tests.CanDoDynamicInterceptionWithRegistry();
            tests.CanCallBaseImplementation();
            tests.CanCacheInterfaceGeneration();
            tests.CanDoDispatchProxyInterception();
        }
    }
}
