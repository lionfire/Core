using LionFire.Instantiating;
using LionFire.MultiTyping;
using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    public static class AssetPathUtils
    {
        public static string DefaultRoot()
        {
            return LionFireEnvironment.Directories.AppProgramDataDir;
        }

        public static string GetSubpath<T>(string assetSubpath = null, object context = null)
        {
            return GetSubpath(typeof(T), assetSubpath, context);
        }
        public static string GetSubpath(object obj, string assetSubpath = null, object context = null)
        {
            var type = obj.GetType();
            return GetSubpath(type, assetSubpath,context);
        }
        private static string GetSubpath(Type type, string assetSubpath = null, object context = null)
        {
            // OLD Another approach, specifying type via context.RootObject - not sure it is needed or worth it. 
            //if ( context .ObjectAsType<PeristenceContext>() is PeristenceContext pc)
            //{
            //    type = context.ObjectAsType<PeristenceContext>()?.RootObject?.GetType();
            //}
            if (type == null) throw new ArgumentNullException(nameof(type));

            string diskPath;

            var attr = (AssetPathAttribute)type.GetTypeInfo().GetCustomAttributes(typeof(AssetPathAttribute), true).FirstOrDefault();
            if (attr != null)
            {
                diskPath = attr.AssetPath;
            }
            else
            {
                diskPath = type.Name;
                if (type.Name.StartsWith("TValue") && type.Name.Length > 1 && char.IsUpper(type.Name[1]))
                {
                    diskPath = diskPath.Substring(1);
                }
            }

            var path = diskPath;
            if (assetSubpath != null)
            {
                path = Path.Combine(path, assetSubpath);
            }

            return path;
        }
    }
}
