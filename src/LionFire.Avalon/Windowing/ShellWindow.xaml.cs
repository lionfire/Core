using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;
using LionFire.Valor.Gui.Shell;
using LionFire.Avalon;
using System.Diagnostics;
using LionFire.Applications;

namespace LionFire.Shell
{

    /// <summary>
    /// ShellWindow is the windowed window for LionFire apps.  It provides an AllowsTransparency window with a default 
    /// theme of rounded corners and custom dragmove caption bar.
    /// The ShellContentPresenter can be reparented between this window and the FullScreenShellWindow.
    /// </summary>    
    public partial class ShellWindow : ShellWindowBase
    {
        #region Configuration

        //public bool TransparentWindow { get { this.shellContentPresenter.TransparentWindowedWindow; } }

        #endregion

        #region Ontology

        public Grid BGGrid { get { return bgGrid; } }
        private FullScreenShellWindow FullScreenShellWindow { get { return shellContentPresenter.FullScreenShellWindow; } }

        #endregion

        #region Settings Properties
        
        #region UseCustomTitleBar

        public bool UseCustomTitleBar
        {
            get { return WpfLionFireShell.Instance.UseCustomTitleBar; }
        }

        private bool _UseCustomTitleBar
        {
            get { return useCustomTitleBar; }
            set
            {
                if (useCustomTitleBar == value) return;

                useCustomTitleBar = value;

                if (value)
                {
                    CustomTitleBar.Visibility = System.Windows.Visibility.Visible;
                    this.WindowStyle = WindowStyle.None;
                }
                else
                {
                    CustomTitleBar.Visibility = System.Windows.Visibility.Collapsed;
                    this.WindowStyle = WindowStyle.SingleBorderWindow;

                }
                TopGridTitleHeight.Height = value ? new GridLength(25) : new GridLength(0);

            }
        } private bool useCustomTitleBar = true;

        #endregion

        #region ShowWindowButtons

        public bool ShowWindowButtons
        {
            get {
                return TitleBarExpander.Visibility == Visibility.Visible;
            }
            set
            {
                TitleBarExpander.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                UpdateTitleBarRowHeight();
            }
        }

        private void UpdateTitleBarRowHeight()
        {
            //if (!ShowWindowButtons && TitleBarRow.Height.Value != 0) TitleBarRowHeight = TitleBarRow.Height; // REVIEW

            //TitleBarRow.Height = ShowWindowButtons || FullWidthTitleBar ? TitleBarRowHeight : new GridLength(0);
        }
        //private GridLength TitleBarRowHeight;
        #endregion

        public bool FullWidthTitleBar {
            get { return WindowTitleText.Visibility == Visibility.Visible; }
            set {
                WindowTitleText.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                DragWindowButton.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
                UpdatePresenterMargin();
                UpdateTitleBarRowHeight();
            }
        }

        #endregion

        #region Construction

        public ShellWindow(ShellContentPresenter shellContentPresenter)
            : base(shellContentPresenter)
        {
            Visibility = Visibility.Collapsed;
            InitializeComponent();
            FullWidthTitleBar = true; // TOCONFIG  per app
            _UseCustomTitleBar = UseCustomTitleBar;

            //if (TransparentWindow)
            //{
            //    this.AllowsTransparency = true;
            //    this.Background = new SolidColorBrush(Colors.Transparent);
            //}
            WindowTitleText.Text = LionEnvironment.LionAppDisplayName;
            TitleBarExpander.Expanded += TitleBarExpander_Expanded;
            TitleBarExpander.Collapsed += TitleBarExpander_Collapsed;
            TitleBarExpander.SizeChanged += TitleBarExpander_SizeChanged;
            TitleBarExpander.IsExpanded = true; // TOCONFIG  per app

            //#if DEV
            if (LionFire.Applications.LionFireApp.IsDevMode)
            {
                debugButton.Visibility = System.Windows.Visibility.Visible;
            }
            //#endif

            this.Width = WpfLionFireShell.Instance.DefaultWindowedSize.Width;
            this.Height = WpfLionFireShell.Instance.DefaultWindowedSize.Height;

            WindowsSettings.Load(); // Consider moving to more appropriate places

            this.Loaded += new RoutedEventHandler(ShellWindow_Loaded);
            DependencyPropertyDescriptor.FromProperty(Window.WindowStateProperty, typeof(Window)).AddValueChanged(this, OnWindowStateChanged);

            WpfLionFireShell.Instance.TopmostChanged += new Action<bool>(Instance_TopmostChanged);

            this.Title = LionEnvironment.LionAppDisplayName;
        }

        void ShellWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.SizeToContent = SizeToContent.Manual;
            UpdateTopmost();
        }

        private void UpdatePresenterMargin()
        {
            if (FullWidthTitleBar && TitleBarExpander.IsExpanded)
            {
                //ShellContentPresenterGrid.Margin = new Thickness(-1, TitleBarExpander.ActualHeight, -1, -1);
                //ShellContentPresenterGrid.Margin = new Thickness(-1, -1, -1, -1);
                WideTitleBar.BorderThickness = new Thickness(0, 0, 0, 1);
            }
            else
            {
                //ShellContentPresenterGrid.Margin = new Thickness(-1, -1, -1, -1);
                WideTitleBar.BorderThickness = new Thickness(0, 0, 0, 0);
            }
        }

        #endregion

        #region Event Handlers
        
        private void TitleBarExpander_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePresenterMargin();
        }

        private void TitleBarExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            UpdatePresenterMargin();
            UpdateTitleBarRowHeight();
        }

        private void TitleBarExpander_Expanded(object sender, RoutedEventArgs e)
        {
            UpdatePresenterMargin();
            UpdateTitleBarRowHeight();
        }

        private void OnWindowStateChanged(object sender, EventArgs e)
        {
        }

        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            _Maximize();
        }

        protected override void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        #endregion

        

        #region Methods

        bool maximizing = false;
        private void _Maximize()
        {
            if (maximizing) return;
            maximizing = true;
            //try
            {
                //WindowState = WindowState.Normal;
                //WindowStyle = WindowStyle.None;
                //Topmost = true;

                //WindowState = WindowState.Maximized;

                this.Visibility = Visibility.Hidden;

                //if (FullScreenShellWindow == null)
                //{
                //    FullScreenShellWindow = new FullScreenShellWindow(this);
                //}
                ShellContentPresenterGrid.Children.Remove(shellContentPresenter);

                shellContentPresenter.IsFullScreen = true;
                FullScreenShellWindow.Maximize();

                this.ShowInTaskbar = false;

            }
            //finally
            {
                maximizing = false;
            }
        }

        /// <summary>
        /// REVIEW - Clean this up to only what is needed
        /// </summary>
        public override void Restore()
        {
            //if (fe.Parent != null)
            //{
            //    //throw new ArgumentException("Detach object before passing it to restore")
            //    //((Panel)ShellContent.Parent).Children.Remove(ShellContent);
            //    //FullScreenShellWindow.Content = fe;
            //}

            if (!ShellContentPresenterGrid.Children.Contains(shellContentPresenter))
            {
                //this.ShowInTaskbar = false;
                this.ShowInTaskbar = shellContentPresenter.ShowInTaskbar;

                ShellContentPresenterGrid.Children.Add(shellContentPresenter);
                shellContentPresenter.Width = double.NaN;
                shellContentPresenter.Height = double.NaN;
                shellContentPresenter.InvalidateMeasure();
                this.Visibility = Visibility.Visible;

                Window Window = this;
                Window.WindowState = System.Windows.WindowState.Minimized;
                
                if (!Window.IsVisible)
                {
                    Window.Show();
                }

                if (Window.WindowState == WindowState.Minimized)
                {
                    Window.WindowState = WindowState.Normal;
                }

                Window.Activate();
                Window.Topmost = true;  // important
                Window.Topmost = false; // important
                Window.Focus();         // important

            }

            if (WpfLionFireShell.Instance.UndoMinimizeAllOnRestore && WpfLionFireShell.Instance.MinimizedAll)
            {
                WpfLionFireShell.Instance.MinimizedAll = false;
                LionFire.Avalon.WindowInterop.UndoMinimizeAllWindows();
            }

            ThreadPool.QueueUserWorkItem(x =>
            {
                //Thread.Sleep(1800);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Window Window = this;
                    
                    Window.WindowState = System.Windows.WindowState.Minimized;

                    if (!Window.IsVisible)
                    {
                        Window.Show();
                    }


                    if (Window.WindowState == WindowState.Minimized)
                    {
                        Window.WindowState = WindowState.Normal;
                    }

                    Window.Activate();
                    Window.Topmost = true;  // important
                    Window.Topmost = false; // important
                    Window.Focus();         // important

                    MoveToForeground.DoOnProcess(Process.GetCurrentProcess().ProcessName);

                    WpfLionFireShell.Instance.BringToFront();
                }));
            });
            
            Window w = this;
            if (!w.IsVisible)
            {
                w.Show();
            }

            if (w.WindowState == WindowState.Minimized)
            {
                w.WindowState = WindowState.Normal;
            }

            w.Activate();
            w.Topmost = true;  // important
            w.Topmost = false; // important
            w.Focus();         // important

            MoveToForeground.DoOnProcess(Process.GetCurrentProcess().ProcessName);
        }

        #endregion
    }
}
