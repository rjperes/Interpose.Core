using System.Threading;

namespace Interpose.Core.Tests
{
    public class Calculator : ICalculator
    {
        public int Add(int a, int b)
        {
            var c = a + b;
            Thread.Sleep(5 * 1000);
            return c;
        }
    }
}
