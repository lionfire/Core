using System.Collections.Generic;

namespace LionFire.Shell
{
    public class ShellStartupOptions
    {
        public List<ViewInstantiation> StartupViews { get; } = new List<ViewInstantiation>();
    }
}
