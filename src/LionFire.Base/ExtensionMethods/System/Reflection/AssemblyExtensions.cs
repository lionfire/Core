using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LionFire.Reflection;

public static class AssemblyExtensions
{
    public static IList<Type> TypesWith(this Assembly assembly, Type attributeType)
    {
        var list = new List<Type>();

        foreach(var type in assembly.GetTypes())
        {
            var attrs = type.GetCustomAttributes(attributeType,false); // REVIEW inherit parameter
            if (attrs != null && attrs.Any()) list.Add(type);
        }
        return list;
    }
    public static IList<Type> TypesWith<AttributeType>(this Assembly assembly)
        where AttributeType : Attribute
    {
        var list = new List<Type>();

        foreach (var type in assembly.GetTypes())
        {
            var attrs = type.GetCustomAttributes<AttributeType>();
            if (attrs != null && attrs.Any()) list.Add(type);
        }
        return list;
    }
    
}
