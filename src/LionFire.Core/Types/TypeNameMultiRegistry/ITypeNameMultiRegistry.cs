namespace LionFire.Types;

public interface ITypeNameMultiRegistry
{
    TypeNameMultiRegistryOptions Options { get; }

    Type? GetType(string name);
    Type? GetTypeWithPreferredRegistry(string name, string? registryName);
    Type? GetTypeFromPreferredRegistry(string name, string? registryName);

    string? GetTypeName(Type type);
    string? GetTypeNameWithPreferredRegistry(Type type, string? registryName);
    string? GetTypeNameFromPreferredRegistry(Type type, string? registryName);
}
