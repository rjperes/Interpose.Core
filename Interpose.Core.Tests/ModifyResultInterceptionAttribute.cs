using Interpose.Core.Interceptors;

namespace Interpose.Core.Tests
{
    public class ModifyResultInterceptionAttribute : InterceptionAttribute
    {
        public ModifyResultInterceptionAttribute() : base(typeof(ModifyResultHandler))
        {

        }
    }
}
