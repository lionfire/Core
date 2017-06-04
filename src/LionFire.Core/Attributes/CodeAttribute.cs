using System;
using System.Linq;
using System.Reflection;

namespace LionFire.Attributes
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class CodeAttribute : Attribute
    {
        public CodeAttribute(string code)
        {
            this.Code = code;
        }

        public string Code { get; private set; }
    }
    public static class CodeAttributeExtensions
    {

        public static T ResolveCode<T>(this string str)
            where T : struct, IConvertible
        {
            foreach (var mi in typeof(T).GetFields().Where(m => m.IsLiteral))
            {
                var attr = mi.GetCustomAttribute<CodeAttribute>();
                if (attr == null) continue;
                if (attr.Code == str) return (T)mi.GetValue(null);
            }
            return default(T);
        }
    }
}
