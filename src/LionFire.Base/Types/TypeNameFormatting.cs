using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.ExtensionMethods;

public static class TypeNameFormatting
{
    public static string ToHumanReadableName(this Type? type, string nullText = "null")
    {
        if(type==null) return nullText;

        if (!type.IsGenericType) return type.Name;

        string typeName = type.Name.Substring(0, type.Name.IndexOf('`'));
        string genericArguments = string.Join(", ", type.GetGenericArguments().Select(arg => arg.ToHumanReadableName()));
        return $"{typeName}<{genericArguments}>";
    }
    
    // OLD
    ///// <summary>
    ///// Display text for the Type of the member
    ///// </summary>
    //public static string DisplayTypeName(this Type type)
    //{
    //    if (type == null) return "null";
    //    var genericTypeDefinition = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
    //    if (genericTypeDefinition == null) return type.Name;

    //    if (genericTypeDefinition == typeof(IObservable<>))
    //    {
    //        return type.GetGenericArguments()[0].Name;
    //    }
    //    else if (genericTypeDefinition == typeof(IAsyncEnumerable<>))
    //    {
    //        return type.GetGenericArguments()[0].Name;
    //    }
    //    else if (genericTypeDefinition.Name == "SettablePropertyAsync`2" || genericTypeDefinition.Name == "PropertyAsync`2")
    //    {
    //        return type.GetGenericArguments()[1].Name;
    //    }
    //    return type.Name;
    //}
}
