using System;
using System.Reflection;

namespace LionFire.Data.Connections.ExtensionMethods
{
    public static class ConnectionManagerExtensionMethods
    {

        public static string GetConfigurationKey(this Type type)
        {
            return
                          (type.GetProperty("ConfigurationKey", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | BindingFlags.NonPublic)
                          ?? type.GetProperty("ConfigurationKey", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
                          ?.GetMethod.Invoke(null, null) as string
                          ?? throw new ArgumentException($"{type.FullName} must have static property 'ConfigurationKey' that returns non-null");

        }
    }
}
