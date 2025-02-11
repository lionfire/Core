namespace LionFire.Schemas.Filesystem;

public static class FileNameAliasUtils
{
    public static string GetAlias<T>()
    {
        var type = typeof(T);
        var attr = type.GetCustomAttributes(typeof(FileNameAlias), true).FirstOrDefault() as FileNameAlias;
        return attr?.Alias ?? type.Name;
    }
}
