using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Shell
{
    public class TabManager
    {
        public static TabManager Instance { get { return Singleton<TabManager>.Instance; } }

        public static string GetTabNameFromType<T>(T obj = null)
            where T : class
        {
            string tabName;
            var attr = typeof(T).GetCustomAttribute<ShellPresenterAttribute>();
            if (attr != null && !String.IsNullOrEmpty(attr.DefaultTabName))
            {
                tabName = attr.DefaultTabName;
            }
            else
            {
                tabName = typeof(T).Name;
            }
            return tabName;
        }
        
    }
}
