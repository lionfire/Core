using System;
using System.Reflection;

namespace LionFire.UI.Entities
{
    public static class UIEntityUtils
    {
        public static string GetDefaultKeyForUIEntity(Type type)
        {
            string key = null;
            var attr = type.GetCustomAttribute<DefaultKeyAttribute>();
            if (attr != null)
            {
                key = attr.Key;
            }
            if (key == null)
            {
                key = type.Name;
            }
            return key;
        }
    }
}
