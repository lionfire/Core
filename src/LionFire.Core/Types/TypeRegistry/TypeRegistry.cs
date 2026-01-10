using LionFire;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using LionFire.ExtensionMethods;

namespace LionFire.TypeRegistration;

/// <summary>
/// A simple type registry that stores types by their short name and full name.
/// Open generic types are stored by their base name (e.g., "AtrBot" for AtrBot&lt;&gt;).
/// Callers are responsible for closing open generics with appropriate type arguments.
/// </summary>
public class TypeRegistry
{
    public static Predicate<Type> DefaultPredicate => t => !t.IsAbstract && !t.IsInterface;

    /// <summary>
    /// Creates a TypeRegistry by scanning assemblies for types implementing the interface.
    /// Open generic types are registered by their base name (without backtick suffix).
    /// </summary>
    public TypeRegistry(Type? interfaceType, IEnumerable<Assembly> assemblies, Predicate<Type>? filter = null)
    {
        filter ??= DefaultPredicate;

        foreach (var type in assemblies.SelectMany(a => a.GetTypes()))
        {
            // For open generics, check if they could implement the interface when closed
            if (type.IsGenericTypeDefinition)
            {
                // We can't directly check IsAssignableFrom on open generics,
                // so we check the base type and interfaces for generic patterns
                if (interfaceType == null || CouldImplementInterface(type, interfaceType))
                {
                    if (filter(type))
                    {
                        Add(type);
                    }
                }
            }
            else
            {
                if ((interfaceType == null || interfaceType.IsAssignableFrom(type)) && filter(type))
                {
                    Add(type);
                }
            }
        }
    }

    /// <summary>
    /// Creates a TypeRegistry with explicitly provided types.
    /// </summary>
    public TypeRegistry(IEnumerable<Type> types)
    {
        foreach (var type in types) { Add(type); }
    }

    /// <summary>
    /// Checks if an open generic type could implement an interface when closed.
    /// </summary>
    private static bool CouldImplementInterface(Type openGenericType, Type interfaceType)
    {
        // Check base types
        var baseType = openGenericType.BaseType;
        while (baseType != null)
        {
            if (baseType.IsGenericType)
            {
                var genericDef = baseType.GetGenericTypeDefinition();
                if (interfaceType.IsAssignableFrom(genericDef) ||
                    genericDef.GetInterfaces().Any(i => i.IsGenericType && interfaceType.IsAssignableFrom(i.GetGenericTypeDefinition())))
                {
                    return true;
                }
            }
            if (interfaceType.IsAssignableFrom(baseType))
            {
                return true;
            }
            baseType = baseType.BaseType;
        }

        // Check interfaces
        foreach (var iface in openGenericType.GetInterfaces())
        {
            if (iface.IsGenericType)
            {
                var genericDef = iface.GetGenericTypeDefinition();
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == genericDef)
                {
                    return true;
                }
            }
            if (interfaceType.IsAssignableFrom(iface))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Resolves a type by name. Tries in order:
    /// 1. Exact match in Names dictionary
    /// 2. Full CLR type name
    /// Note: May return open generic types - caller is responsible for closing them.
    /// </summary>
    public Type? GetType(string name)
        => GetTypeFromName(name)
        ?? GetTypeFromFullName(name);

    public Type? GetTypeFromFullName(string name) => FullNames.TryGetValue(name, out var type) ? type : null;
    public Type? GetTypeFromName(string name) => Names.TryGetValue(name, out var type) ? type : null;

    public Type GetTypeFromNameOrThrow(string name) => GetType(name) ?? throw new ArgumentException($"Could not resolve '{name}'.");

    public IReadOnlyDictionary<string, Type> FullNames => fullNames;
    private Dictionary<string, Type> fullNames = new Dictionary<string, Type>();
    public IReadOnlyDictionary<string, Type> Names => names;
    private Dictionary<string, Type> names = new Dictionary<string, Type>();
    public IReadOnlyDictionary<string, List<Type>> ConflictingNames => conflictingNames;
    private Dictionary<string, List<Type>> conflictingNames = new Dictionary<string, List<Type>>();

    /// <summary>
    /// Gets a friendly name for a type, including generic type parameters if closed.
    /// For open generics like AtrBot&lt;&gt;, returns just "AtrBot".
    /// </summary>
    public static string GetFriendlyTypeName(Type type)
    {
        var baseName = type.Name.Split('`')[0];

        if (type.IsGenericType && !type.IsGenericTypeDefinition)
        {
            var typeArgs = type.GetGenericArguments();
            var argNames = string.Join(",", typeArgs.Select(t => t.Name));
            return $"{baseName}<{argNames}>";
        }

        return baseName;
    }

    public void Add(Type type)
    {
        var fullName = type.FullName!;
        if (fullNames.ContainsKey(fullName))
        {
            if (fullNames[fullName] != type)
            {
                throw new DuplicateNotAllowedException($"Duplicate type FullName: {fullName}");
            }
            else
            {
                Debug.WriteLine($"Type already registered: {fullName}");
                return;
            }
        }

        fullNames.Add(fullName, type);

        // Use base name (without backtick for generics)
        var baseName = type.Name.Split('`')[0];

        // Register base name with conflict tracking
        if (conflictingNames.ContainsKey(baseName))
        {
            conflictingNames[baseName].Add(type);
        }
        else if (names.Remove(baseName, out Type? existingType))
        {
            // Move to conflicts
            conflictingNames.Add(baseName, [type, existingType]);
        }
        else
        {
            names.Add(baseName, type);
        }
    }
}
