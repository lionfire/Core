
using LionFire.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LionFire.Shell
{
    /// <summary>
    /// FullScrenShellWindow is the fullscreen window for LionFire apps.  It provides an AllowsTransparency=false window 
    /// that by default shows the TabbedWindowPresenter
    /// The ShellContentPresenter can be reparented between this window and the FullScreenShellWindow.
    /// </summary>
    public partial class FullScreenWindowView : ShellWindowBase
    {

#if TOPORT
        public override void Restore()
        {
#if TOPORT
            this.Topmost = false;
            this.ShellContentPresenterGrid.Children.Remove(shellContentPresenter);

#if DESKTOP_STUFF
            //CloseDesktop();
            //RestoreRes();
#endif

            this.shellContentPresenter.IsFullScreen = false;

            this.Visibility = Visibility.Hidden;
            this.ShowInTaskbar = false;

            this.ShellWindow.Restore();
#endif
        }
        private UIElement ShellContent => shellContentPresenter;

        public FullScreenShellWindow(TabbedWindowPresenter scp)
            : base(scp)
        {
            InitializeComponent();

            //this.WindowState = WindowState.Normal;
            //this.WindowStyle = WindowStyle.None;
            //this.ResizeMode = ResizeMode.NoResize;
            this.WindowState = WindowState.Maximized; // Do this last to ensure takes up whole screen
            this.Visibility = Visibility.Visible;

            this.Title = AppInfo.Instance.AppDisplayName;
            //this.Topmost = true;
        }

        // TODO: Set desktop number to move to a different screen
        //public int DesktopNumber { get; set; }

        // TODO: Change resolution
        //public int[2] Resolution { get; set; }

        


        internal void Maximize()
        {
            if (ShellOptions.MinimizeAllOnFullScreen)
            {
                LionFire.Avalon.WindowInterop.MinimizeAllWindows(); 
                WpfShell.Instance.MinimizedAll = true;
            }

            this.ShowInTaskbar = true;

            this.ShellContentPresenterGrid.Children.Insert(0, shellContentPresenter);

            this.Visibility = Visibility.Visible;
            this.Topmost = true;

#if DESKTOP_STUFF
            //CreateDesktop();
            //ChangeRes();
            //System.Threading.Thread.Sleep(5000);
            //bool switchResult = PInvokeDesktop.SwitchDesktop(desktop);
            //Log.Info("SwitchDesktop: " + switchResult);

            //CloseDesktop();
#endif
        }


        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            this.Topmost = false;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (Visibility == Visibility.Visible)
            {
                this.Topmost = true;
            }
        }


#if DESKTOP_STUFF
        private void ChangeRes()
        {
            User32.ChangeDisplaySettings(
        }
        private void RestoreRes()
        {
        }

        private void CreateDesktop()
        {
            //GCHandle handle = GCHandle.Alloc(myStruct);
            //try
            //{
            //    IntPtr pinnedAddress = handle.AddrOfPinnedObject();
            //}
            //finally
            //{
            //    handle.Free();
            //}
            desktop = PInvokeDesktop.CreateDesktop(LionEnvironment.LionAppName + "-Desktop", null, null, 0, ACCESS_MASK.GENERIC_ALL, null);
        }
        private void CloseDesktop()
        {
            if (desktop == IntPtr.Zero) return;
            PInvokeDesktop.CloseDesktop(desktop);
            desktop = IntPtr.Zero;
        }
        IntPtr desktop;
#endif

#endif

    }

#if DESKTOP_STUFF
    //public class SafeDesktopHandle : BaseSafeHandle
    //{
    //    public SafeDesktopHandle(IntPtr handle, bool ownsHandle)
    //        : base(handle, ownsHandle)
    //    { }

    //    protected override bool CloseNativeHandle(IntPtr handle)
    //    {
    //        return WindowStationAndDesktop.CloseDesktop(handle);
    //    }
    //}

    class User32
    {
        [DllImport("user32.dll")]
        public static extern int EnumDisplaySettings(
          string deviceName, int modeNum, ref DEVMODE devMode);
        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(
              ref DEVMODE devMode, int flags);

        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int CDS_UPDATEREGISTRY = 0x01;
        public const int CDS_TEST = 0x02;
        public const int DISP_CHANGE_SUCCESSFUL = 0;
        public const int DISP_CHANGE_RESTART = 1;
        public const int DISP_CHANGE_FAILED = -1;
    }

    internal static class PInvokeDesktop
    {
        //[DllImport("user32", EntryPoint = "CreateDesktopW", CharSet = CharSet.Unicode, SetLastError = true)]
        //public static extern IntPtr CreateDesktopW(string lpszDesktop, IntPtr lpszDevice, IntPtr pDevmode, int dwFlags,
        //                                          int dwDesiredAccess, IntPtr lpsa);

        // ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.WIN32COM.v10.en/dllproc/base/createdesktop.htm
        [DllImport("user32.dll", EntryPoint = "CreateDesktop", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateDesktop(
                        [MarshalAs(UnmanagedType.LPWStr)] string desktopName,
                        [MarshalAs(UnmanagedType.LPWStr)] string device, // must be null.
                        [MarshalAs(UnmanagedType.LPWStr)] string deviceMode, // must be null,
                        [MarshalAs(UnmanagedType.U4)] int flags,  // use 0
                        [MarshalAs(UnmanagedType.U4)] ACCESS_MASK accessMask,
                        //[MarshalAs(UnmanagedType.LPStruct)] SECURITY_ATTRIBUTES attributes
            [MarshalAs(UnmanagedType.LPWStr)] string attributes
            );


        // ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.WIN32COM.v10.en/dllproc/base/closedesktop.htm
        [DllImport("user32.dll", EntryPoint = "CloseDesktop", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseDesktop(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern bool SwitchDesktop(IntPtr hDesktop);
    }
    [Flags]
    internal enum ACCESS_MASK : uint
    {
        DESKTOP_NONE = 0,
        DESKTOP_READOBJECTS = 0x0001,
        DESKTOP_CREATEWINDOW = 0x0002,
        DESKTOP_CREATEMENU = 0x0004,
        DESKTOP_HOOKCONTROL = 0x0008,
        DESKTOP_JOURNALRECORD = 0x0010,
        DESKTOP_JOURNALPLAYBACK = 0x0020,
        DESKTOP_ENUMERATE = 0x0040,
        DESKTOP_WRITEOBJECTS = 0x0080,
        DESKTOP_SWITCHDESKTOP = 0x0100,

        GENERIC_ALL = (DESKTOP_READOBJECTS | DESKTOP_CREATEWINDOW | DESKTOP_CREATEMENU |
                        DESKTOP_HOOKCONTROL | DESKTOP_JOURNALRECORD | DESKTOP_JOURNALPLAYBACK |
                        DESKTOP_ENUMERATE | DESKTOP_WRITEOBJECTS | DESKTOP_SWITCHDESKTOP
            //| STANDARD_ACCESS.STANDARD_RIGHTS_REQUIRED

                        ),
    }
#if false
    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public unsafe byte* lpSecurityDescriptor;
        public int bInheritHandle;
    }
#else
    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr lpSecurityDescriptor;
        public int bInheritHandle;
    }
#endif

#endif
}
