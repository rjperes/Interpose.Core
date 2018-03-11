using System.Threading;

namespace Interpose.Core.Tests
{
    public class LongWait : ILongWait
    {
        public void DoLongOperation()
        {
            Thread.Sleep(5 * 1000);
        }
    }
}
