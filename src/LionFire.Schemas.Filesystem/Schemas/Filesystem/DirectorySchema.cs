using Hjson;

namespace LionFire.Schemas.Filesystem;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
public class FileNameAlias : Attribute
{
    public FileNameAlias(string alias)
    {
        Alias = alias;
    }
    public string Alias { get; set; }
}

[FileNameAlias("dir")]
public class DirectorySchema : Schema
{
    public DirectorySchema() : base("https://schemas.lionfire.ca/2025/filesystem")
    {
    }

    public DirectoryCollectionSchema? CollectionType { get; set; }

}

public class DirectoryCollectionSchema
{
    public string? CollectionType { get; set; }
}

public static class FileNameAliasUtils
{
    public static string GetAlias<T>()
    {
        var type = typeof(T);
        var attr = type.GetCustomAttributes(typeof(FileNameAlias), true).FirstOrDefault() as FileNameAlias;
        return attr?.Alias ?? type.Name;
    }
}

/// <summary>
/// </summary>
/// <remarks>
/// Hard-coded implementation
/// 
/// HARDCODES:
/// - hjson
/// - native (System.IO) filesystem
/// </remarks>
public class DirectorySchemaOnNativeFs
{
    const string ext = ".hjson";


    public static async ValueTask InitSchema(DirectorySchema directorySchema, string dir)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);

            var schemaFilename = FileNameAliasUtils.GetAlias<DirectorySchema>() + ext;

            await File.WriteAllTextAsync(Path.Combine(dir, FileNameAliasUtils.GetAlias<DirectorySchema>() + ext), Hjson.JsonValue.Parse(System.Text.Json.JsonSerializer.Serialize(directorySchema)).ToString(Stringify.Hjson));
            //await File.WriteAllTextAsync(Path.Combine(dir, ".dir.hjson"), """
            //CollectionType: Workspace
            //""");

            Directory.CreateDirectory(Path.Combine(dir, "Blue"));
            await File.WriteAllTextAsync(Path.Combine(dir, "Blue", "workspace.hjson"), """
            Description: This is a sample workspace named 'Blue'
            """);

            Directory.CreateDirectory(Path.Combine(dir, "Red"));
            await File.WriteAllTextAsync(Path.Combine(dir, "Red", "workspace.hjson"), """
            Description: This is a sample workspace named 'Red'
            SampleProperty: Sample value.
            """);
        }
    }
}