#nullable enable
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LionFire.Types;

public class TypeNameRegistry
{
    public Dictionary<string, Type> Types { get; } = new Dictionary<string, Type>();
    public Dictionary<Type, string> TypeNames { get; } = new Dictionary<Type, string>();

    public void RegisterFullName<T>()
    {
        if (typeof(T).FullName == null) return;
        Types.Add(typeof(T).FullName, typeof(T));
        TypeNames.Add(typeof(T), typeof(T).FullName);
    }
    public void RegisterFullName(Type type)
    {
        if (type.FullName == null) return;

        Types.Add(type.FullName, type);
        TypeNames.Add(type, type.FullName);
    }

    public void RegisterName<T>()
    {
        Types.Add(typeof(T).Name, typeof(T));
        TypeNames.Add(typeof(T), typeof(T).Name);
    }
    public void RegisterName(Type type)
    {
        Types.Add(type.Name, type);
        TypeNames.Add(type, type.Name);
    }
    public void Register<T>(string name)
    {
        Types.Add(name, typeof(T));
        TypeNames.Add(typeof(T), name);
    }
    public void Register<T>()
    {
        var attr = typeof(T).GetCustomAttributes<RegisterTypeNameAttribute>().FirstOrDefault() ?? throw new ArgumentException($"Type {typeof(T).FullName} does not have a {nameof(RegisterTypeNameAttribute)}");
        Register<T>(attr.Name);
    }
}
