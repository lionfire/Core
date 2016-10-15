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
        public static string GetSubpath<T>(string assetSubpath)
        {
            string diskPath;
            var attr = (AssetPathAttribute)typeof(T).GetTypeInfo().GetCustomAttributes(typeof(AssetPathAttribute), false).FirstOrDefault();
            if (attr != null)
            {
                diskPath = attr.AssetPath;
            }
            else
            {
                diskPath = typeof(T).Name;
                if (typeof(T).Name.StartsWith("T") && typeof(T).Name.Length > 1 && char.IsUpper(typeof(T).Name[1]))
                {
                    diskPath = diskPath.Substring(1);
                }                
            }

            var path = diskPath;
            path = Path.Combine(path, assetSubpath);

            return path;
        }
    }
}
