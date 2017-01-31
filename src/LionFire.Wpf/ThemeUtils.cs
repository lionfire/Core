using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Wpf
{
    public class ThemeInjector
    {
        public static void SetTheme(object control, string themeName) 
        {
            if (control == null) return;
            if (control.GetType().FullName == "Xceed.Wpf.AvalonDock.DockingManager")
            {
                var typeName = "Xceed.Wpf.AvalonDock.Themes." + themeName;
                //var assemblyName = "Xceed.Wpf.AvalonDock.Themes.VS2013";
                //Type type = Type.GetType(typeName); // TODO - use 
            }
            // ENH
            //Vs2013DarkTheme;
        }
    }

}
