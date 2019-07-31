using LionFire.DependencyInjection;
using LionFire.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Applications.Hosting
{
    public static class AppInfoInstances 
    {
        public static AppInfo CurrentAppInfo => DependencyContext.Current.GetService<AppInfo>();
        public static AppInfo MainAppInfo => DependencyContext.Default.GetService<AppInfo>();
    }
}
