using LionFire.Shell;
using System.ComponentModel;
using System.Windows;

namespace LionFire.UI.Entities
{
    public class WpfWindowBase<T> : UIView<T>, IWpfWindow, IWindow
        where T : Window
    {

        #region Window pass-through
        
        Window IWpfWindow.View => View;

        public bool Topmost
        {
            get => View.Topmost;
            set => View.Topmost = value;
        }

        public void Restore() => View.WindowState = WindowState.Normal;
        public void Minimize() => View.WindowState = WindowState.Minimized;
        public void Maximize() => View.WindowState = WindowState.Maximized;

        #endregion
    }

    public class WpfFullScreenWindow : WpfWindowBase<FullScreenWindowView>
    {
        public WpfFullScreenWindow()
        {
            Key = "(fullscreen)";
        }

        //ShellWindowBase CurrentShellWindow
        //{
        //    get => currentShellWindow;
        //    set
        //    {
        //        if (value == currentShellWindow) return;

        //        currentShellWindow = value;

        //        OnPropertyChanged(nameof(CurrentShellWindow));
        //        OnPropertyChanged(nameof(View));
        //    }
        //}
        //private ShellWindowBase currentShellWindow;

        
    }
}
