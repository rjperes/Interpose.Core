namespace Interpose.Core.Tests
{
    class Program
    {
        static void Main()
        {
            var tests = new InterceptionTests();
            tests.CanDoVirtualInterception();
            tests.CanDoDynamicInterception();
            tests.CanDoInterfaceInterception();
            tests.CanDoDynamicInterceptionWithAttributes();
            tests.CanDoDynamicInterceptionWithRegistry();
        }
    }
}
