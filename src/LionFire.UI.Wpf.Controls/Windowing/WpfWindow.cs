using LionFire.Shell;
using System.ComponentModel;
using System.Windows;

namespace LionFire.UI
{
    public interface IWpfWindow : INotifyPropertyChanged
    {
        
        UIElement View { get; }
    }

    public interface IWpfWindowView
    {
        void Restore();
    }


    public class WpfWindow : IWpfWindow
    {

        UIElement View { get; }

        ShellWindowBase CurrentShellWindow
        {
            get => currentShellWindow;
            set
            {
                if (value == currentShellWindow) return;

                currentShellWindow = value;

                OnPropertyChanged(nameof(CurrentShellWindow));
                OnPropertyChanged(nameof(View));
            }
        }
        private ShellWindowBase currentShellWindow;


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion


    }
    
}
