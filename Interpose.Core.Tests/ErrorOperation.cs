using System;
using System.Collections.Generic;
using System.Text;

namespace Interpose.Core.Tests
{
    public class ErrorOperation : IErrorOperation
    {
        public void Throw()
        {
            throw new InvalidOperationException();
        }
    }
}
