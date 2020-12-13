using System.ComponentModel;
using System.Windows;

namespace LionFire.UI.Entities
{
    public interface IWpfWindow : INotifyPropertyChanged, IWindow
    {

        Window View { get; }
    }
}
