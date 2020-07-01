using LionFire.Applications;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LionFire.Avalon
{
#if FUTURE // For click-thru
    // From http://stackoverflow.com/questions/2842667/how-to-create-a-semi-transparent-window-in-wpf-that-allows-mouse-events-to-pass

    public static class WindowsServices
{
  const int WS_EX_TRANSPARENT = 0x00000020;
  const int GWL_EXSTYLE = (-20);

  [DllImport("user32.dll")]
  static extern int GetWindowLong(IntPtr hwnd, int index);

  [DllImport("user32.dll")]
  static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

  public static void SetWindowExTransparent(IntPtr hwnd)
  {
    var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
    SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
  }
}
for your window set:

WindowStyle = None
Topmost = true
AllowsTransparency = true
in code behind for the window add:

protected override void OnSourceInitialized(EventArgs e)
{
  base.OnSourceInitialized(e);
  var hwnd = new WindowInteropHelper(this).Handle;
  WindowsServices.SetWindowExTransparent(hwnd);
}
and viola - click-through window! See original answer in: http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/a3cb7db6-5014-430f-a5c2-c9746b077d4f
    
#endif


    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window, INotifyPropertyChanged
    {
        #region Construction

        public SplashWindow(AppInfo appInfo, DevMode devMode)
        {
            InitializeComponent();
          
#if DEBUG
            this.Opacity = 0.5;
            this.Top = 0;
            this.Left = 0;
            this.IsHitTestVisible = false;
            //this.RenderTransform = new ScaleTransform(0.5, 0.5);
#endif
            this.Closing += SplashWindow_Closing;
            this.Loaded += SplashWindow_Loaded;

            DevText.Visibility = DevMode.IsDevMode ? Visibility.Visible : Visibility.Hidden;

            DebugText.Visibility = 
#if DEBUG
                Visibility.Visible;
#else
                Visibility.Hidden;
#endif

            TraceText.Visibility = 
#if TRACE
                Visibility.Visible;
#else
                Visibility.Hidden;
#endif

            PrealphaText.Visibility = LionFireEnvironment.Compilation.BuildType == "Prealpha" ? Visibility.Visible : Visibility.Hidden;
            AlphaText.Visibility = LionFireEnvironment.Compilation.BuildType == "Alpha" ? Visibility.Visible : Visibility.Hidden;
            BetaText.Visibility = LionFireEnvironment.Compilation.BuildType == "Beta" ? Visibility.Visible : Visibility.Hidden;
            AppInfo = appInfo;
            DevMode = devMode;
        }

        void SplashWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.BringIntoView();
            
        }

        private static readonly ILogger l = Log.Get();
		
        #endregion

        #region Event Handlers

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        #endregion

        #region View Properties

        public string AppName => AppInfo.AppDisplayName;

        public AppInfo AppInfo { get; }
        public DevMode DevMode { get; }

        #endregion

        Storyboard sb;
        void SplashWindow_Closing(object sender, CancelEventArgs e)
        {
            if (sb != null) return;
            

            e.Cancel = true;

            sb = FindResource("closeStoryBoard") as Storyboard;

            sb.Begin();
            //sb.Completed += sb_Completed;
        }

        void sb_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        #region Misc

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion
    }
}
