using System;

namespace LionFire.UI.Windowing
{
    public class DesktopProfile
    {
        // ENH: Don't just use desktop width and height, but get sizes of all monitors

        public DesktopProfile(double desktopWidth, double desktopHeight)
        {
            this.DesktopWidth = (int)desktopWidth;
            this.DesktopHeight = (int)desktopHeight;
        }
        public DesktopProfile(int desktopWidth, int desktopHeight)
        {
            this.DesktopWidth = desktopWidth;
            this.DesktopHeight = desktopHeight;
        }
        public DesktopProfile(string key)
        {
            var chunks = key.Split('x');
            this.DesktopWidth = Convert.ToInt32(chunks[0]);
            this.DesktopHeight = Convert.ToInt32(chunks[1]);
        }

        public int DesktopWidth { get;/*init;*/  }
        public int DesktopHeight { get;/* init;*/ }

        public string Key => $"{DesktopWidth}x{DesktopHeight}";
    }
}
