using Interpose.Core.Interceptors;
using System;

namespace Interpose.Core.Handlers
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class NotifyPropertyChangeAttribute : InterceptionAttribute
    {
        public NotifyPropertyChangeAttribute() : base(typeof(NotifyPropertyChangeHandler))
        {
        }
    }
}
