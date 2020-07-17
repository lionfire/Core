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
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using LionFire.Structures;

namespace LionFire.Shell
{
    public abstract class ShellWindowBase : Window
    {
        #region Dependencies

        protected readonly ShellContentPresenter shellContentPresenter;
        protected ShellWindow ShellWindow { get { return shellContentPresenter.ShellWindow; } }

        #region Derived

        protected ShellOptions ShellOptions => shellContentPresenter.ShellPresenter.Shell.ShellOptions;

        #endregion

        #endregion

        private Brush topmostBrush1 = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        private Brush topmostBrush2 = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        Ellipse TopmostEllipse2;
        Ellipse TopmostEllipse1;

        public ShellWindowBase() { }

        public ShellWindowBase(ShellContentPresenter shellContentPresenter)
        {
            this.shellContentPresenter = shellContentPresenter;

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
            MoveToForeground.DoOnProcess(Process.GetCurrentProcess());
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

        protected void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            Restore();
        }

        public abstract void Restore();

        protected void topmostButton_Click(object sender, RoutedEventArgs e)
        {
            this.shellContentPresenter.Topmost ^= true;
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        protected void closeButton_Click(object sender, RoutedEventArgs e)
        {
            DoClose();
        }
        public void DoClose(Window isClosing=null)
        {
            if (shellContentPresenter != null)
            {
                shellContentPresenter.CloseWindows(isClosing);
            }
        }

        protected void debugButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            //WpfShell.Instance.IsDebugWindowVisible ^= true;
        }
        protected void menuButton_Click(object sender, RoutedEventArgs e)
        {
            WpfShell.Instance.EventAggregator.Publish(ManualSingleton<MToggleAppMenu>.GuaranteedInstance);
        }

        protected virtual void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // FUTURE: Allow dragging full-screen window to another screen
        }

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
        private static readonly ILogger l = Log.Get();

    }

    public class MoveToForeground
    {
        [DllImportAttribute("User32.dll")]
        private static extern int FindWindow(String ClassName, String WindowName);

        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_SHOWWINDOW = 0x0040;
        const int SWP_NOACTIVATE = 0x0010;
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        public static void DoOnProcess(string processName)
        {
            var allProcs = Process.GetProcessesByName(processName);
            if (allProcs.Length ==1 )
            {
                Process proc = allProcs[0];
                DoOnProcess(proc);
            }
            else if (allProcs.Length > 1)
            {
                foreach (var p in allProcs)
                {
                    DoOnProcess(p);
                }
            }
            else
            {
                throw new Exception("Proc not found: " + processName);
            }
        }
        public static void DoOnProcess(Process proc)
        {
            
                int hWnd = FindWindow(null, proc.MainWindowTitle.ToString());
                // Change behavior by settings the wFlags params. See http://msdn.microsoft.com/en-us/library/ms633545(VS.85).aspx
                SetWindowPos(new IntPtr(hWnd), 0, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW | SWP_NOACTIVATE);
            
        }
    }
}

