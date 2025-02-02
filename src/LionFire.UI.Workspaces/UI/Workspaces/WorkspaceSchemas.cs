using LionFire.Schemas;
using LionFire.Schemas.Filesystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.UI.Workspaces;

public class WorkspaceSchemas
{
    public static string Namespace = "https://schemas.lionfire.ca/2025/ui";

    public static TypeSchemaBundle Types { get; }

    public static DirectorySchema DirectorySchema { get; }

    static WorkspaceSchemas()
    {
        Types = new TypeSchemaBundle(
            typeof(IWorkspace)
            );
        Types.AddDefaultAliases();

        DirectorySchema = new DirectorySchema
        {
            CollectionType = new DirectoryCollectionSchema
            {
                CollectionType = TypeSchemaBundle.DefaultAliasFactory(typeof(IWorkspace)), // "Workspace",
            },
        };
    }

    public static async ValueTask InitFilesystemSchemas(string workspacesDir)
    {
        await DirectorySchemaOnNativeFs.InitSchema(DirectorySchema, workspacesDir);
    }

}




