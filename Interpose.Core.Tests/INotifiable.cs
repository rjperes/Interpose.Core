using System.ComponentModel;

namespace Interpose.Core.Tests
{
    public interface INotifiable : INotifyPropertyChanged, INotifyPropertyChanging
    {
        string Name { get; set; }
    }
}