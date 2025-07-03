using LionFire;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using LionFire.ExtensionMethods;

namespace LionFire.TypeRegistration;

public class TypeRegistry
{
    public static Predicate<Type> DefaultPredicate => t => !t.IsAbstract && !t.IsInterface;

    public TypeRegistry(Type? interfaceType, IEnumerable<Assembly> assemblies, Predicate<Type>? filter = null)
    {
        assemblies.SelectMany(a => a.GetTypes())
                           .Where(t => interfaceType == null || interfaceType.IsAssignableFrom(t)
                               && (filter != null ? filter(t) : DefaultPredicate(t)))
                           .ForEach(type => Add(type));
    }

    public Type? GetType(string name) => GetTypeFromName(name) ?? GetTypeFromFullName(name);
    public Type? GetTypeFromFullName(string name) => FullNames.TryGetValue(name, out var type) ? type : null;
    public Type? GetTypeFromName(string name) => Names.TryGetValue(name, out var type) ? type : null;
    public Type GetTypeFromNameOrThrow(string name) => GetTypeFromName(name) ?? throw new ArgumentException($"Could not resolve {name}");

    public IReadOnlyDictionary<string, Type> FullNames => fullNames;
    private Dictionary<string, Type> fullNames = new Dictionary<string, Type>();
    public IReadOnlyDictionary<string, Type> Names => names;
    private Dictionary<string, Type> names = new Dictionary<string, Type>();
    public IReadOnlyDictionary<string, List<Type>> ConflictingNames => conflictingNames;
    private Dictionary<string, List<Type>> conflictingNames = new Dictionary<string, List<Type>>();

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

        var name = type.Name;
        name = name.Split('`')[0];

        if (conflictingNames.ContainsKey(name))
        {
            conflictingNames[name].Add(type);
        }
        else if (names.Remove(name, out Type? existingType))
        {
            conflictingNames.Add(name, [type, existingType]);
        }
        else
        {
            names.Add(name, type);
        }
    }
}
