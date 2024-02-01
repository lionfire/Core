// Based on http://www.codeproject.com/Articles/232972/Creating-an-application-like-Google-Desktop-in-WPF
//  Retrieved on Jul 11, 2014, used under CPOL license.

using LionFire.Avalon.WinForms;
using LionFire.Collections;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LionFire.Avalon.Windowing.AppBar
{
    public class TransPrev
    {
        private static ILogger log = Log.Get();

        internal void Show()
        {
            log.Info("show transprev");
        }

        internal void Hide()
        {
            log.Info("hide transprev");
        }

        internal void SetArrow(AppBarEdge edge)
        {
            log.Info("SetArrow");
        }
    }

    // TODO: you can remove window borders with SetWindowLong. 

    /// <summary>
    /// Interaction logic for AppBarWindow.xaml
    /// </summary>
    public partial class AppBarWindow : Window, IKeyed<string>
    {



        #region Key

        public string Key
        {
            get { return key; }
            set
            {
                if (key == value) return;
                if (key != default(string)) throw new AlreadySetException();
                key = value;
            }
        } private string key;

        object IROKeyed.Key {get{return Key;}}

        #endregion
                

        

        #region State

        // Is AppBar registered?
        bool fBarRegistered = false;

        bool nclButtonDown = false;

        // Number of AppBar's message for WndProc
        int uCallBack;

        #region AppBarEdge

        public AppBarEdge AppBarEdge
        {
            get { return appBarEdge; }
            set
            {
                if (appBarEdge == value) return;

                {
                    if (IsInitialized) UnregisterBar();
                    appBarEdge = value;
                    if (IsInitialized) RegisterBar();
                }
            }
        } private AppBarEdge appBarEdge;


        #endregion

        #endregion

        #region TransPrev

        TransPrev transPrev = new TransPrev();

        #endregion


        #region Construction

        public AppBarWindow(string key):this(){this.Key=key;}
        public AppBarWindow()
        {
            this.IsVisibleChanged += AppBarWindow_IsVisibleChanged;
            this.SizeChanged += AppBarWindow_SizeChanged;
            this.SourceInitialized += AppBarWindow_SourceInitialized;
            InitializeComponent();
            WindowStyle = System.Windows.WindowStyle.ToolWindow;
        }

        void AppBarWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //l.Trace("Size changed: " + this.ActualWidth + "x" + ActualHeight);
            if (isVisiblePending)
            {
                isVisiblePending = false;
                RegisterBar();
            }
        }

        #endregion

        #region Register / Unregister


        #region IsVisible

        public bool Visible
        {
            get { return visible; }
            set
            {
                if (visible == value) return;

                visible = value;

                if (value)
                {
                    this.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        } private bool visible = false;

        void AppBarWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //l.Trace("AppBarWindow_IsVisibleChanged");
            if (IsVisible)
            {
                UpdateSize();
                if (!fBarRegistered)
                {
                    if(ActualWidth== 0 || ActualHeight == 0)
                    {
                        isVisiblePending=true;
                        //RegisterBar();
                    }
                    else
                    {
                        RegisterBar();
                    }
                }
            }
            else
            {
                if (fBarRegistered)
                {
                    UnregisterBar();
                }
            }
        }
        private bool isVisiblePending = false;

        #endregion

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (nclButtonDown)
            {
                if (fBarRegistered)
                {
                    UnregisterBar();
                    transPrev.Show();
                }
                RefreshTransPrev();
            }
        }

        void RegisterBar()
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            if (helper.Handle == IntPtr.Zero) { l.Trace("AppBar: No Handle yet."); return; }
            HwndSource mainWindowSrc = (HwndSource)HwndSource.FromHwnd(helper.Handle);

            var abd = new AppBarInterop.APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = mainWindowSrc.Handle;

            if (!fBarRegistered)
            {
                uCallBack = AppBarInterop.RegisterWindowMessage("AppBarMessage");
                abd.uCallbackMessage = uCallBack;

                uint ret = AppBarInterop.SHAppBarMessage(AppBarInterop.ABM_NEW, ref abd);
                fBarRegistered = true;

                ABSetPos();
            }
        }

        void UnregisterBar()
        {
            if (fBarRegistered)
            {

                WindowInteropHelper helper = new WindowInteropHelper(this);
                HwndSource mainWindowSrc = (HwndSource)HwndSource.FromHwnd(helper.Handle);

                var abd = new AppBarInterop.APPBARDATA();
                abd.cbSize = Marshal.SizeOf(abd);
                abd.hWnd = mainWindowSrc.Handle;

                AppBarInterop.SHAppBarMessage(AppBarInterop.ABM_REMOVE, ref abd);
                fBarRegistered = false;
            }
        }

        #endregion

        #region ExtendGlass

        void ExtendGlass()
        {
            try
            {
                int isGlassEnabled = 0;
                AppBarInterop.DwmIsCompositionEnabled(ref isGlassEnabled);
                if (Environment.OSVersion.Version.Major > 5 && isGlassEnabled > 0)
                {
                    WindowInteropHelper helper = new WindowInteropHelper(this);
                    HwndSource mainWindowSrc = (HwndSource)HwndSource.FromHwnd(helper.Handle);

                    mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;
                    this.Background = Brushes.Transparent;

                    var margins = new AppBarInterop.MARGINS();
                    margins.cxLeftWidth = -1;
                    margins.cxRightWidth = -1;
                    margins.cyBottomHeight = -1;
                    margins.cyTopHeight = -1;

                    AppBarInterop.DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
                }
            }
            catch (DllNotFoundException) { }
        }

        #endregion
        
        private void UpdateSize()
        {
            var uEdge = (int)AppBarEdge;
            if (uEdge == AppBarInterop.ABE_LEFT || uEdge == AppBarInterop.ABE_RIGHT)
            {
                this.Height = double.NaN;
            }
            else
            {
                this.Width = double.NaN;
            }
        }
        private void UpdatePos()
        {
            //if (abd.uEdge == AppBarInterop.ABE_LEFT || abd.uEdge == AppBarInterop.ABE_RIGHT)
            //{
            //    if (abd.uEdge == AppBarInterop.ABE_LEFT)
            //    {

            //    }
            //    else
            //    {
                    
            //        this.Left = abd.rc.right - (int)this.ActualWidth;

            //    }
            //}
            //else
            //{
            //    abd.rc.left = 0;
            //    abd.rc.right = (int)SystemParameters.PrimaryScreenWidth;
            //    if (abd.uEdge == AppBarInterop.ABE_TOP)
            //    {
            //        abd.rc.top = 0;
            //        abd.rc.bottom = (int)this.ActualHeight;
            //    }
            //    else
            //    {
            //        abd.rc.bottom = (int)SystemParameters.PrimaryScreenHeight;
            //        abd.rc.top = abd.rc.bottom - (int)this.ActualHeight;
            //    }
            //}
        }

        private int VirtualScreenHeight
        {
            get { return (int)SystemParameters.VirtualScreenHeight; }
            //abd.rc.bottom = (int)SystemParameters.PrimaryScreenHeight;
        }
        private int VirtualScreenWidth
        {
            get { return (int)SystemParameters.VirtualScreenWidth; }
            //abd.rc.right = (int)SystemParameters.PrimaryScreenWidth;
        }



        #region Move

        void ABSetPos()
        {
            if (fBarRegistered)
            {
                WindowInteropHelper helper = new WindowInteropHelper(this);
                HwndSource mainWindowSrc = (HwndSource)HwndSource.FromHwnd(helper.Handle);

                var abd = new AppBarInterop.APPBARDATA();
                abd.cbSize = Marshal.SizeOf(abd);
                abd.hWnd = mainWindowSrc.Handle;
                abd.uEdge = (int)AppBarEdge;

                UpdateSize();
                UpdatePos();

                if (abd.uEdge == AppBarInterop.ABE_LEFT || abd.uEdge == AppBarInterop.ABE_RIGHT)
                {
                    abd.rc.top = 0;
                    abd.rc.bottom = (int)VirtualScreenHeight;
                    if (abd.uEdge == AppBarInterop.ABE_LEFT)
                    {
                        abd.rc.left = 0;
                        abd.rc.right = (int)this.ActualWidth;
                    }
                    else
                    {
                        abd.rc.right = (int)VirtualScreenWidth;
                        abd.rc.left = abd.rc.right - (int)this.ActualWidth;
                    }
                }
                else
                {
                    WpfScreen screen = WpfScreen.GetScreenFrom(new Point(VirtualScreenWidth, VirtualScreenHeight));

                    abd.rc.left = 0;
                    abd.rc.left = (int)screen.WorkingArea.Left; // HARDCODE - FUTURE: do something more sophisticated here
                    abd.rc.right = (int)VirtualScreenWidth;
                    if (abd.uEdge == AppBarInterop.ABE_TOP)
                    {
                        abd.rc.top = 0;
                        abd.rc.bottom = (int)this.ActualHeight;
                    }
                    else
                    {
                        abd.rc.bottom = (int)VirtualScreenHeight;
                        abd.rc.top = abd.rc.bottom - (int)this.ActualHeight;
                    }
                }

                AppBarInterop.SHAppBarMessage(AppBarInterop.ABM_QUERYPOS, ref abd);

                AppBarInterop.SHAppBarMessage(AppBarInterop.ABM_SETPOS, ref abd);
                AppBarInterop.MoveWindow(abd.hWnd, abd.rc.left, abd.rc.top, abd.rc.right - abd.rc.left, abd.rc.bottom - abd.rc.top, true);
            }
        }

        #endregion

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == uCallBack && wParam.ToInt32() == AppBarInterop.ABN_POSCHANGED)
            {
                ABSetPos();
                handled = true;
            }
            else if (msg == AppBarInterop.WM_DWMCOMPOSITIONCHANGED)
            {
                ExtendGlass();
                handled = true;
            }
            else if (msg == AppBarInterop.WM_NCLBUTTONDOWN)
            {
                nclButtonDown = true;
            }
            else if (msg == AppBarInterop.WM_EXITSIZEMOVE)
            {
                nclButtonDown = false;
                transPrev.Hide();
                CalculateHorizontalEdge();
                RegisterBar();
            }
            return IntPtr.Zero;
        }


        void RefreshTransPrev()
        {
            CalculateHorizontalEdge();

            transPrev.SetArrow(AppBarEdge);
        }

        void CalculateHorizontalEdge()
        {
            if (SystemParameters.PrimaryScreenWidth / 2 > this.Left)
                AppBarEdge = AppBarEdge.Left;
            else
                AppBarEdge = AppBarEdge.Right;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            l.Fatal("OnSourceInitialized");
            base.OnSourceInitialized(e);

        }
        void AppBarWindow_SourceInitialized(object sender, EventArgs e)
        {
            l.Fatal("AppBarWindow_SourceInitialized");

            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            HwndSource source = HwndSource.FromHwnd(hwnd);
            source.AddHook(new HwndSourceHook(WndProc));

            ExtendGlass();
            RegisterBar();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnregisterBar();
            Application.Current.Shutdown();
        }


        private static readonly ILogger l = Log.Get();

    }
}
