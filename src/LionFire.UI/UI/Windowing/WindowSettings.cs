using LionFire.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.UI.Windowing
{
    public class WindowSettings
    {
        public const string DefaultWindowName = "DefaultWindow";

        public Dictionary<string, WindowProfile> Profiles { get; set; } = new Dictionary<string, WindowProfile>();

        
        public WindowProfile GetProfile(int desktopWidth, int desktopHeight)
        {
            var key = new DesktopProfile(desktopWidth, desktopHeight).Key;
            var profile = Profiles.GetOrAdd(key, _=>new WindowProfile());
            return profile;
        }
    }
}
