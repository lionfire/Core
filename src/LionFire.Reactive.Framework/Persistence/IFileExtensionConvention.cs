using LionFire.IO.Reactive.Hjson;
using LionFire.Ontology;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LionFire.Persistence.Filesystemlike;

public interface IFileExtensionConvention
{
    string FileExtensionForType(Type type)
    {
        return IFileExtensionConventionCache.FileExtensions.GetOrAdd(type, t =>
        {
            var attr = type.GetCustomAttribute<FileExtensionAttribute>();
            if (attr != null) { return attr.Extension; }

            var name = type.Name;

            var attr2 = type.GetCustomAttribute<AliasAttribute>();
            if (attr2 != null) { name = attr2.Alias; }

            return name.ToKebabCase(); ;
        });
    }
}

file static class IFileExtensionConventionCache
{
    public static ConcurrentDictionary<Type, string> FileExtensions = new();
}
