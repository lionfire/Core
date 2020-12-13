using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using LionFire.Shell;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;
using LionFire.Avalon;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using LionFire.Structures;
using Microsoft.Extensions.Options;

namespace LionFire.Shell
{
    public abstract class ShellWindowBase : Window
    {
#if TOPORT
        #region Dependencies

        protected readonly TabbedWindowPresenter shellContentPresenter;

        //public IOptionsMonitor<ShellOptions> ShellOptionsMonitor { get; }

        #region Derived

        protected ShellWindow ShellWindow => shellContentPresenter.ShellWindow;
        //protected ShellOptions ShellOptions => ShellOptionsMonitor.CurrentValue;

        #endregion

        #endregion

        #region Construction and Destruction

        public ShellWindowBase() { }

        public ShellWindowBase(TabbedWindowPresenter shellContentPresenter
            //, IOptionsMonitor<ShellOptions> shellOptionsMonitor
            )
        {
            this.shellContentPresenter = shellContentPresenter;
            //ShellOptionsMonitor = shellOptionsMonitor;
            WpfShell.Instance.ShellPresenter.MainPresenter.TopmostChanged += new Action<bool>(Instance_TopmostChanged);
            this.Loaded += new RoutedEventHandler(ShellWindowBase_Loaded);

            this.Closing += ShellWindowBase_Closing;
            this.Initialized += ShellWindowBase_Initialized;
            this.ContentRendered += ShellWindowBase_ContentRendered;

            this.DataContext = shellContentPresenter;
        }

        void ShellWindowBase_ContentRendered(object sender, EventArgs e)
        {
            this.Topmost = shellContentPresenter.Topmost;
            this.Activate();
        }

        void ShellWindowBase_Initialized(object sender, EventArgs e)
        {
            //this.Topmost = true; // REVIEW - why was this here?
            MoveWindowToForeground.DoOnProcess(Process.GetCurrentProcess());
        }

        void ShellWindowBase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Closing -= ShellWindowBase_Closing;
            try
            {
                DoClose(isClosing: this.ShellWindow);
            }
            catch { }
        }

        void ShellWindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            TopmostEllipse2 = (Ellipse)this.FindName("TopmostEllipse2");
            TopmostEllipse1 = (Ellipse)this.FindName("TopmostEllipse1");

            UpdateTopmost();
            //this.Show();
            //this.Activate();
            //Topmost = true;  
            //Topmost = false; 
            //Focus();         
        }

        #endregion

        #region Graphics

        private Brush topmostBrush1 = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        private Brush topmostBrush2 = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        Ellipse TopmostEllipse2;
        Ellipse TopmostEllipse1;

        #endregion


        #region Event Handlers

        protected void restoreButton_Click(object sender, RoutedEventArgs e) => Restore();

        protected void topmostButton_Click(object sender, RoutedEventArgs e) => this.shellContentPresenter.Topmost ^= true;

        private void minimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        protected void closeButton_Click(object sender, RoutedEventArgs e) => DoClose();
        protected void debugButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            //WpfShell.Instance.IsDebugWindowVisible ^= true;
        }
        protected void menuButton_Click(object sender, RoutedEventArgs e) => WpfShell.Instance.EventAggregator.Publish(ManualSingleton<MToggleAppMenu>.GuaranteedInstance);
        #endregion
#endif

        protected virtual void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // FUTURE: Allow dragging full-screen window to another screen
        }
#if TOPORT

#region (Public) Methods

        public abstract void Restore();

        public void DoClose(Window isClosing = null)
        {
            if (shellContentPresenter != null)
            {
                shellContentPresenter.CloseWindows(isClosing);
            }
        }

#endregion

#region (Protected) Methods

        protected void UpdateTopmost()
        {
            if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(new Action(() => UpdateTopmost())); return; }
            try
            {
                if (TopmostEllipse2 == null || TopmostEllipse1 == null)
                {
                    TopmostEllipse2 = (Ellipse)this.FindName("TopmostEllipse2");
                    TopmostEllipse1 = (Ellipse)this.FindName("TopmostEllipse1");
                }

                if (TopmostEllipse2 != null) TopmostEllipse2.Fill = shellContentPresenter.Topmost ? topmostBrush1 : topmostBrush2;
                if (TopmostEllipse1 != null) TopmostEllipse1.Fill = !shellContentPresenter.Topmost ? topmostBrush1 : topmostBrush2;

                //  MOVE to XAML?
                //topmostButton.Background = LionFireShell.Instance.Topmost
                //    ? checkedBackgroundBrush
                //    : uncheckedBackgroundBrush;
            }
            catch (Exception ex)
            {
                l.Error(ex.ToString());
            }
        }

        protected void Instance_TopmostChanged(bool obj)
        {
            UpdateTopmost();
        }

#endregion

#endif
#region Misc

        private static readonly ILogger l = Log.Get();

#endregion
    }
}

