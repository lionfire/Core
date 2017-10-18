using LionFire.UI.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LionFire.Wpf
{
    public static class WindowUtils
    {
        public static int DesktopWidth
        {
            get
            {
                int w = 0;
                foreach (var s in System.Windows.Forms.Screen.AllScreens)
                {
                    w += s.Bounds.Width;
                }

                return w;
            }
        }
        public static int DesktopHeight
        {
            get
            {
                int h = 0;
                foreach (var s in System.Windows.Forms.Screen.AllScreens)
                {
                    h += s.Bounds.Height;
                }

                return h;
            }
        }

        public static WindowProfile GetCurrentProfile(this WindowSettings windowSettings)
        {
            return windowSettings.GetProfile(DesktopWidth, DesktopHeight);
        }

        public static void UpdateWindowProfile(WindowSettings windowSettings, System.Windows.Window window, Dictionary<string, Window> otherWindows = null)
        {
            var profile = GetCurrentProfile(windowSettings);

            if (profile.MainWindow == null)
            {
                profile.MainWindow = new WindowLayout();
            }
            UpdateWindowLayout(profile.MainWindow, window);

            if (otherWindows != null)
            {
                foreach (var kvp in otherWindows)
                {
                    throw new NotImplementedException();
                    //UpdateWindow(profile.GetWindow(kvp.Key), kvp.Value);
                }
            }
        }

        public static void UpdateWindowLayout(WindowLayout windowLayout, Window window)
        {
            windowLayout.X = (int)window.Left;
            windowLayout.Y = (int)window.Top;
            windowLayout.Width = (int)window.ActualWidth;
            windowLayout.Height = (int)window.ActualHeight;
        }

        public static void RestoreWindowLayout(WindowLayout windowLayout, Window window)
        {
            if(windowLayout.X > 0) window.Left = windowLayout.X;
            if (windowLayout.Y> 0) window.Top = windowLayout.Y;
            if (windowLayout.Width > 0) window.Width= windowLayout.Width;
            if (windowLayout.Height > 0) window.Height = windowLayout.Height;
        }

        public static void LoadSettings(WindowSettings windowSettings, Window window)
        {
            var profile = GetCurrentProfile(windowSettings);

            RestoreWindowLayout(profile.MainWindow, window);
        }
    }
}
