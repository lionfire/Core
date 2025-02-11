using LionFire.IO.Reactive.Hjson;
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
            return type.Name.ToKebabCase(); ;
        });
    }
}

file static class IFileExtensionConventionCache
{
    public static ConcurrentDictionary<Type, string> FileExtensions = new();
}
