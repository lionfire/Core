using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Ontology;

public static class PluralX
{
    public static string GetPluralName<T>() => GetPluralName(typeof(T));

    public static string GetPluralName(this Type type)
    {
        var pluralAttribute = type.GetCustomAttributes(typeof(PluralAttribute), true).FirstOrDefault() as PluralAttribute;
        if (pluralAttribute != null)
        {
            return pluralAttribute.PluralName;
        }

        // ENH: use Humanizr nuget

        var aliasAttribute = type.GetCustomAttributes(typeof(AliasAttribute), true).FirstOrDefault() as AliasAttribute;
        if (aliasAttribute != null)
        {
            return aliasAttribute.Alias + "s";
        }

        var name = type.Name;
        if (name.EndsWith("y"))
        {
            return name.Substring(0, name.Length - 1) + "ies";
        }
        return name + (name.EndsWith("s") ? "es" : "s");
    }

  }
