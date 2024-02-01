
// TODO - use System.Windows.SystemParameters.WorkArea instead of WinForms
using LionFire.ObjectBus;
using LionFire.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LionFire.Avalon
{
    public class WindowSettings
    {
        public bool IsMaximized;
        public Rect Bounds;
    }

    public class WindowSettingsProfile
    {
        public bool IsDebugWindowVisible;
        public WindowSettings MainWindow { get; set; }
        public WindowSettings DebugWindow { get; set; }

        public Dictionary<string, WindowSettings> OtherWindows { get; set; }
    }

    //public static class RectExtensions
    //{
    //    public static Rect ConstrainTo(this Rect rect, Rect contraint)
    //    {

    //    }
    //}



    public class WindowsSettings
    {
        #region Members

        public Dictionary<string, WindowSettingsProfile> ProfilesByResolution = new Dictionary<string, WindowSettingsProfile>();

        #endregion

        #region Derived Properties

        public WindowSettingsProfile CurrentProfile
        {
            get { return ProfilesByResolution.GetOrAddNew(CurrentProfileResolution); }
        }

        #endregion

        #region Static - CurrentProfile

        public static string CurrentProfileName
        {
            get
            {
                return
#if DEBUG
 WpfLionFireShell.AutostartControl != null ? WpfLionFireShell.AutostartControl.Name : 
#endif
 "";
            }
        }
        public static string CurrentProfilePrefix { get { return CurrentProfileName + (string.IsNullOrEmpty(CurrentProfileName) ? "" : "-"); } }

        public static string CurrentProfileResolution
        {
            get
            {
                int w = 0;
                int h = 0;
                foreach (var s in System.Windows.Forms.Screen.AllScreens)
                {
                    w += s.Bounds.Width;
                    h += s.Bounds.Height;
                }

                return CurrentProfilePrefix + w.ToString() + "x" + h.ToString();
            }
        }

        #endregion

        #region (Static) Instance from Vos

        public static WindowsSettings Instance
        {
            get
            {
                return VWindowsSettings.TryGetOrCreate();
            }
        }

        //public static VobHandle<WindowsSettings> VWindowsSettings { get { return V.ActiveData["AppSettings/Windows"].ToHandle<WindowsSettings>(); } }
        public static VobHandle<WindowsSettings> VWindowsSettings { get { return V.ActiveData["AppSettings/" + VosPath.MachineSubpath + "/Windows"].ToHandle<WindowsSettings>(); } }

        #endregion

        #region (Static) Settings

        public static WindowSettings MainWindowSettings
        {
            get
            {
                var s = Instance.CurrentProfile.MainWindow;

                if (s == null)
                {
                    // WorkArea is primary display.  User could later move it to secondary display.
                    var w = System.Windows.SystemParameters.WorkArea.Width;
                    var h = System.Windows.SystemParameters.WorkArea.Height;

                    var p = 40;

                    s = new WindowSettings()
                    {
                        Bounds = new Rect(p, p, Math.Min(1024, w - (2 * p)), Math.Min(768, h - (2 * p))),
                        IsMaximized = !Applications.LionFireApp.IsDevMode,
                    };
                    Instance.CurrentProfile.MainWindow = s;
                }

                // FUTURE: At window settings load time. Bail out the user if he/she places the window off screen in some bizarre location.  Constrain bounds.  
                // Another possibility: they switched resolutions, removed a monitor, went from portrait to landscape or vice versa.

                return s;
            }
        }

        public static WindowSettings DebugWindowSettings
        {
            get
            {
                var s = Instance.CurrentProfile.DebugWindow;
                if (s == null)
                {
                    s = new WindowSettings()
                    {
                        Bounds = new Rect(0, 114, 800, 600),
                        IsMaximized = false,
                    };
                    Instance.CurrentProfile.DebugWindow = s;
                }
                return s;
            }
        }

        #endregion

        #region (Static) Save/Load

        public static void Save()
        {
            {
                var w = WpfLionFireShell.Instance.MainWindow;
                if (w != null)
                {
                    MainWindowSettings.Bounds = new Rect(w.Left, w.Top, w.Width, w.Height);
                }
            }

            {
                var w = WpfLionFireShell.Instance.DebugWindow;
                if (w != null)
                {
                    DebugWindowSettings.Bounds = new Rect(w.Left, w.Top, w.Width, w.Height);
                }
            }

            Instance.CurrentProfile.IsDebugWindowVisible = WpfLionFireShell.Instance.IsDebugWindowVisible;

            VWindowsSettings.Save();
        }
        public static void Load()
        {
            VWindowsSettings.TryEnsureRetrieved();

            {
                var w = WpfLionFireShell.Instance.MainWindow;

                w.Left = MainWindowSettings.Bounds.Left;
                w.Top = MainWindowSettings.Bounds.Top;
                w.Width = MainWindowSettings.Bounds.Width;
                w.Height = MainWindowSettings.Bounds.Height;
            }

            MainWindowSettings.IsMaximized = WpfLionFireShell.Instance.MainPresenter.IsFullScreen;


            WpfLionFireShell.Instance.IsDebugWindowVisible = Instance.CurrentProfile.IsDebugWindowVisible;
            {
                Window w = WpfLionFireShell.Instance.DebugWindow;
                var s = DebugWindowSettings;

                if (w != null)
                {
                    w.Left = s.Bounds.Left;
                    w.Top = s.Bounds.Top;
                    w.Width = s.Bounds.Width;
                    w.Height = s.Bounds.Height;
                }
            }
        }

        #endregion


    }


}
