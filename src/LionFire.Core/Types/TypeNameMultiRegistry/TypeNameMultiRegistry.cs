using Microsoft.Extensions.Options;
using System.Linq;

namespace LionFire.Types;

public class TypeNameMultiRegistry : ITypeNameMultiRegistry
{
    public TypeNameMultiRegistryOptions Options => optionsMonitor.CurrentValue;
    private readonly IOptionsMonitor<TypeNameMultiRegistryOptions> optionsMonitor;

    public TypeNameMultiRegistry(IOptionsMonitor<TypeNameMultiRegistryOptions> optionsMonitor)
    {
        this.optionsMonitor = optionsMonitor;
    }

    #region GetType

    public Type? GetTypeFromPreferredRegistry(string name, string? registryName)
        => registryName == null ? null : Options?.Registries.TryGetValue(registryName)?.Types.TryGetValue(name);

    public Type? GetTypeWithPreferredRegistry(string name, string? registryName)
        => (registryName == null ? null : Options?.Registries.TryGetValue(registryName)?.Types.TryGetValue(name))
        ?? GetType(name);

    public Type? GetType(string name)
    {
        foreach (var registry in Options.RegistryNames.Select(n => Options.Registries.TryGetValue(n)).Where(r => r != null).Cast<TypeNameRegistry>())
        {
            var type = registry.Types.TryGetValue(name);
            if (type != null) return type;
        }
        return null;
    }

    #endregion

    #region GetTypeName

    public string? GetTypeNameFromPreferredRegistry(Type type, string? registryName)
        => registryName == null ? null : Options?.Registries.TryGetValue(registryName)?.TypeNames.TryGetValue(type);

    public string? GetTypeNameWithPreferredRegistry(Type type, string? registryName)
        => (registryName == null ? null : Options?.Registries.TryGetValue(registryName)?.TypeNames.TryGetValue(type))
        ?? GetTypeName(type);

    public string? GetTypeName(Type type)
    {
        foreach (var registry in Options.RegistryNames.Select(n => Options.Registries.TryGetValue(n)).Where(r => r != null).Cast<TypeNameRegistry>())
        {
            var name = registry.TypeNames.TryGetValue(type);
            if (name != null) return name;
        }
        return null;
    }

    #endregion
}