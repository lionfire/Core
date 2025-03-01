using LionFire.Persistence.Filesystem;
using LionFire.Referencing;
using LionFire.Schemas;
using LionFire.Schemas.Filesystem;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.UI.Workspaces;

public class WorkspaceSchemas
{
    public static string Namespace = "https://schemas.lionfire.ca/2025/ui";

    public static TypeSchemaBundle Types { get; }
    //public static LionFire.Vfs.Conventions.Types Types2 { get; } // UNUSED

    public static DirectorySchema DirectorySchema { get; }

    static WorkspaceSchemas()
    {
        Types = new TypeSchemaBundle(
            typeof(IWorkspace)
            );
        Types.AddDefaultAliases();

#if UNUSED
        Types2 = new LionFire.Vfs.Conventions.Types();
        Types2.DefaultType = TypeSchemaBundle.DefaultAliasFactory(typeof(IWorkspace)); // "Workspace",
        Types2.WhitelistEnabled = true;
#endif

        DirectorySchema = new DirectorySchema
        {
            CollectionType = new DirectoryCollectionSchema
            {
                CollectionType = TypeSchemaBundle.DefaultAliasFactory(typeof(IWorkspace)), // "Workspace",
            },
        };
    }

    public static async ValueTask InitFilesystemSchemas(IReference workspacesDir)
    {
        if (workspacesDir is FileReference fileReference)
        {
            await DirectorySchemaOnNativeFs.InitSchema(DirectorySchema, fileReference.Path);
        }
        else
        {
            throw new NotSupportedException();
        }
    }
}




