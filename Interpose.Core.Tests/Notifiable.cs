using System.ComponentModel;

namespace Interpose.Core.Tests
{
    internal class Notifiable : INotifiable
    {
        public string Name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
    }
}