using LionFire.ExtensionMethods;
using System.Collections.Generic;

namespace LionFire.UI.Windowing
{
    public class WindowProfile
    {
        public WindowLayout MainWindow { get; set; }
        public Dictionary<string, WindowLayout> OtherWindows { get; set; }

        public WindowLayout GetWindow(string name)
        {
            if (OtherWindows == null) OtherWindows = new Dictionary<string, WindowLayout>();
            return OtherWindows.GetOrAdd(name, _ => new WindowLayout());
        }
    }
}
