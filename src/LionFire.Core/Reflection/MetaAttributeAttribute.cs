using LionFire.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class MetaAttributeAttribute : System.Attribute
    {
        public MetaAttributeAttribute(string key, string val)
        {
            this.key = key;
            this.val = val;
        }

        public string Key
        {
            get { return key; }
        } readonly string key;

        public string Value
        {
            get { return val; }
        } readonly string val;

    }

    public static class AttributeAttributeExtensions
    {
        public static Dictionary<string, string> GetMetaAttributes(this Type type, bool inherit = false)
        {
            var dict = new Dictionary<string, string>();
            foreach (var attr in type.GetCustomAttributes(inherit).OfType<MetaAttributeAttribute>())
            {
                dict.Add(attr.Key, attr.Value);
            }
            return dict;
        }

        public static string GetMetaAttribute(this Type type, string attributeName, bool inherit = false)
        {
            var attrs = GetMetaAttributes(type, inherit);

            return attrs.TryGetValue(attributeName);
        }
        
    }
        
}
