using Interpose.Core.Handlers;

namespace Interpose.Core.Tests
{
    public interface ICalculator
    {
        [ValidationInterception]
        int Add(int a, int b);
    }
}
