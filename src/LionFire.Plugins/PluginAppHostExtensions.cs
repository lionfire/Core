using LionFire.Applications;
using LionFire.Applications.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Plugins
{

    public class PluginContext
    {
        public List<string> SearchPaths { get; set; }

        ///// <summary>
        ///// Specific version requirements to load.
        ///// 
        ///// E.g.: ["LionFire.Serialization.Json.Newtonsoft"] = "1.0.0-1.5.0"
        ///// </summary>
        //public Dictionary<string, string> PluginVersions { get; set; }
    }

    public static class PluginAppHostExtensions
    {
        public static IAppHost AddPlugins(this IAppHost app, string directory = null)
        {
            //if (directory == null)
            //{
            //    var path = app.GetType().Assembly.Location;
            //    directory = Path.GetDirectoryName(path);
            //}

            //if (directory == null)
            //{
            //    var context = App.Get<PluginContext>();
            //    if (context?.archPaths != null && context.SearchPaths.Count > 0)
            //    {
            //        foreach (var dir in context.SearchPaths)
            //        {
            //            app.AddPlugins(dir);
            //        }
            //        return app;
            //    }
            //}

            //if (directory == null)
            //{
            //    // Warn no plugin locations to load from.
            //    return app;
            //}
            
            ////app.Add<

            return app;
        }

    }

    public class PluginStatus
    {
    }

    public class PluginManager
    {

        public ConcurrentDictionary<string, PluginStatus> Plugins { get; private set; } = new ConcurrentDictionary<string, PluginStatus>();


        
    }

}
