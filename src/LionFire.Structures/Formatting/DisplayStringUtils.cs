using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LionFire
{

    public static class AggregateExtensions
    {
        public static string ToStringList(this IEnumerable<string> e, string separator = ", ", string noneString = "(none)")
        {
            return !e.Any() ? noneString : e.Aggregate((x, y) => x + separator + y);
        }
        public static string ToStringList(this IEnumerable<object> e, string separator = ", ", string noneString = "(none)")
        {
            return !e.Any() ? noneString :  e.Select(o => o.ToString()).Aggregate((x, y) => x + separator + y);
        }
    }

    public static class ToStringExtensions
    {
        public static string ToTypeNameSafe(this object obj)
        {
            if (obj == null) return "(null)";
            return obj.GetType().Name;
        }
        public static string ToTypeFullNameSafe(this object obj)
        {
            if (obj == null) return "(null)";
            return obj.GetType().FullName;
        }

        public static string ToStringSafe(this object obj)
        {
            if (obj == null) return "(null)";
            try
            {
                return obj.ToString();
            }
            catch (Exception)
            {
                return obj.GetType().Name + "-(ToString threw exception)";
            }
        }

        public static string ToPercentString(this double perunage, int decimals = 0)
        {
            var format = decimals == 0 ? "{0:0.%}" : $"{{0:0.{Enumerable.Repeat("0", decimals).Aggregate((x,y)=>x+y)}%}}"; // OPTIMIZE
            return double.IsNaN(perunage) ? "???" : String.Format(format, perunage);
        }
        public static string ToPercentString(this float perunage)
        {
            return double.IsNaN(perunage) ? "???" : String.Format("{0:0.%}", perunage);
        }


        public static string ToPluralName(this Type type)
        {
            var attr = type.GetCustomAttribute<PluralAttribute>();
            if (attr != null) { return attr.PluralName; }

            var name = type.Name;
            return name + (name.EndsWith("s") ? "es" : "s");
        }

    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class PluralAttribute : Attribute
    {
        public PluralAttribute(string pluralName)
        {
            this.pluralName = pluralName;
        }

        public string PluralName
        {
            get { return pluralName; }
        } private readonly string pluralName;
    }

    public static class DisplayStringExtensions
    {
        public static string ToPluralDisplayName(this Type type)
        {
            throw new NotImplementedException();
        }

        public static string TemplateTypeNameToDisplayString(this string name, string defaultName = "(unnamed)", bool spaces = true)
        {
            if (name[0] != 'T') return ToDisplayString(name, defaultName);
            return ToDisplayString(name.Substring(1), defaultName: defaultName, spaces: spaces);
        }

        public static string ParameterTypeNameToDisplayString(this string name, string defaultName = "(unnamed)", bool spaces = true)
        {
            if (name[0] != 'P') return ToDisplayString(name, defaultName);
            return ToDisplayString(name.Substring(1), defaultName: defaultName, spaces: spaces);
        }

        public static string ToDisplayString(this string name, string defaultName = "(unnamed)", bool spaces = true)
        {
            if (name == null) return defaultName;

            StringBuilder sb = new StringBuilder();

            bool lastWasUpper = false;
            bool lastWasNumber = false;

            foreach (var c in name)
            {
                if (char.IsUpper(c))
                {
                    if (spaces && !lastWasUpper && sb.Length > 0) sb.Append(' ');

                    lastWasUpper = true;
                }
                else
                {
                    lastWasUpper = false;
                    if (char.IsNumber(c))
                    {
                        if (spaces && !lastWasNumber && sb.Length > 0) sb.Append(' ');

                        lastWasNumber = true;
                    }
                    else
                    {
                        lastWasNumber = false;
                    }
                }

                sb.Append(c);
            }
            return sb.ToString();
        }
        public static string ToTitleCase(this string name)
        {
            return name[0].ToString().ToUpperInvariant() + name.Substring(1);
        }

        //public static string ToXamlString(this object obj) // FUTURE?
        //{
        //    // TODO: Add default param, other params, based on attributes?
        //    return "{" + obj.GetType().Name + "}";
        //}
    }
}
