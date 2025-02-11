#nullable enable
namespace LionFire.ExtensionMethods;

public static class StringUtilsX // MOVE
{
    public static string? TrimStartString(this string? path, string potentialPrefix)
    {
        if (path == null) return null;
        if (path.StartsWith(potentialPrefix))
        {
            path = path.Substring(potentialPrefix.Length);
        }
        return path;
    }
}
