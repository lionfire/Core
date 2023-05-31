using System.Linq;
using System.Reflection;

namespace LionFire.Types;

public class TypeNameMultiRegistryOptions
{
    public bool AutoRegisterFullNames { get; set; }
    public bool AutoRegisterNames { get; set; }
    public List<string> RegistryNames { get; set; } = new List<string> {
        TypeNameRegistryNames.Name,
        TypeNameRegistryNames.FullName,
    };

    public Dictionary<string, TypeNameRegistry> Registries { get; set; } = new();

    public TypeNameRegistry this[string name]
    {
        get
        {
            if (Registries.TryGetValue(name, out var registry)) return registry;
            return Registries[name] = new TypeNameRegistry();
        }
    }

    public void Register<T>()
    {
        var attr = typeof(T).GetCustomAttributes<RegisterTypeNameAttribute>().FirstOrDefault() ?? throw new ArgumentException($"Type {typeof(T).FullName} does not have a {nameof(RegisterTypeNameAttribute)}");

        this[attr.RegistryName].Register<T>();
    }
}
