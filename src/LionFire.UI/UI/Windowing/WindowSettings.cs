using LionFire.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.UI.Windowing
{
    public class DesktopProfile
    {
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

        public int DesktopWidth { get; set; }
        public int DesktopHeight { get; set; }

        public string Key { get { return DesktopWidth + "x" + DesktopHeight; } }
    }

    public class WindowProfile
    {
        public WindowLayout MainWindow { get; set; }
        public Dictionary<string, WindowLayout> OtherWindows { get; set; }

        public WindowLayout GetWindow(string name)
        {
            if (OtherWindows == null) OtherWindows = new Dictionary<string, Windowing.WindowLayout>();
            return OtherWindows.GetOrAdd(name, _ => new WindowLayout());
        }
    }

    public class WindowSettings
    {
        public Dictionary<string, WindowProfile> Profiles { get; set; } = new Dictionary<string, WindowProfile>();

        public WindowProfile GetProfile(int desktopWidth, int desktopHeight)
        {
            var key = new DesktopProfile(desktopWidth, desktopHeight).Key;
            var profile = Profiles.GetOrAdd(key, _=>new WindowProfile());
            return profile;
        }

        

    }


    

}
