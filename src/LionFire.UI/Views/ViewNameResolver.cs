using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.UI
{
    public class ViewNameResolver
    {
        public static string GetViewName<T>(T obj = null)
            where T : class => GetViewName(typeof(T));

        public static string GetViewName(object obj)
            => GetViewName(obj.GetType(), obj);

        public static string GetViewName(Type type, object obj = null)
        {
            string viewName;
            var attr = type.GetCustomAttribute<ViewAttribute>();
            if (attr != null && !string.IsNullOrEmpty(attr.DefaultViewName))
            {
                viewName = attr.DefaultViewName;
            }
            else{ viewName = type.Name; }
            return viewName;
        }
    }
}
