using Interpose.Core.Interceptors;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Interpose.Core.Handlers
{
    public sealed class NotifyPropertyChangeHandler : IInterceptionHandler
    {
        public static readonly IInterceptionHandler Instance = new NotifyPropertyChangeHandler();

        public void Invoke(InterceptionArgs arg)
        {
            var isSetter = arg.Method.Name.StartsWith("set_");
            var propertyName = isSetter ? arg.Method.Name.Substring(4) : string.Empty;
            var isNotifyPropertyChanging = isSetter && arg.Target is INotifyPropertyChanging;
            var isNotifyPropertyChanged = isSetter && arg.Target is INotifyPropertyChanged;
           
            if (isNotifyPropertyChanging)
            {
                var eventDelegate = (MulticastDelegate)arg.Target.GetType().GetField(nameof(INotifyPropertyChanging.PropertyChanging), BindingFlags.Instance | BindingFlags.NonPublic).GetValue(arg.Target);
                var args = new PropertyChangingEventArgs(propertyName);

                if (eventDelegate != null)
                {
                    foreach (var handler in eventDelegate.GetInvocationList())
                    {
                        handler.Method.Invoke(handler.Target, new object[] { arg.Target, args });
                    }
                }
            }

            arg.Proceed();

            if (isNotifyPropertyChanged)
            {
                var eventDelegate = (MulticastDelegate)arg.Target.GetType().GetField(nameof(INotifyPropertyChanged.PropertyChanged), BindingFlags.Instance | BindingFlags.NonPublic).GetValue(arg.Target);
                var args = new PropertyChangedEventArgs(propertyName);

                if (eventDelegate != null)
                {
                    foreach (var handler in eventDelegate.GetInvocationList())
                    {
                        handler.Method.Invoke(handler.Target, new object[] { arg.Target, args });
                    }
                }
            }
        }
    }
}
