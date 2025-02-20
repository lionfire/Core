﻿using Hjson;

namespace LionFire.Schemas.Filesystem;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
public class SchemaAttribute : Attribute
{
    public SchemaAttribute(string schemaUri)
    {
        SchemaUri = schemaUri;
    }
    public string SchemaUri { get; set; }

}

// REVIEW: Consider replacing with LionFire.Vfs.Conventions.Types
[FileNameAlias("dir")]
[Schema("https://schemas.lionfire.ca/2025/filesystem")]
public class DirectorySchema //: Schema
{
    public DirectorySchema() //: base("https://schemas.lionfire.ca/2025/filesystem")
    {
    }

    public DirectoryCollectionSchema? CollectionType { get; set; }

}

public class DirectoryCollectionSchema
{
    public string? CollectionType { get; set; }
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


    // TODO: Replace dir with IReference
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

            #region Blue

            Directory.CreateDirectory(Path.Combine(dir, "Blue"));

            await File.WriteAllTextAsync(Path.Combine(dir, "Blue", "workspace.hjson"), """
            Description: This is a sample workspace named 'Blue'
            """);

            #region Bots

            Directory.CreateDirectory(Path.Combine(dir, "Blue", "Bots"));
            await File.WriteAllTextAsync(Path.Combine(dir, "Blue", "Bots", "TestBot1.bot.hjson"), """
            Description: This is a sample bot 1
            """);
            await File.WriteAllTextAsync(Path.Combine(dir, "Blue", "Bots", "TestBot2.bot.hjson"), """
            Description: This is a sample bot 2
            """);

            #endregion

            #endregion

            Directory.CreateDirectory(Path.Combine(dir, "Red"));
            await File.WriteAllTextAsync(Path.Combine(dir, "Red", "workspace.hjson"), """
            Description: This is a sample workspace named 'Red'
            SampleProperty: Sample value.
            """);


            Directory.CreateDirectory(Path.Combine(dir, "Misc"));
            await File.WriteAllTextAsync(Path.Combine(dir, "Misc", "Green.workspace.hjson"), """
            Description: This is a sample workspace named 'Green'
            SampleProperty: Sample value.
            """);
        }
    }
}