using LionFire.IO.Reactive.Filesystem;
using LionFire.IO.Reactive.Hjson;
using LionFire.Persistence.Filesystem;
using LionFire.Reactive.Persistence;
using LionFire.Referencing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LionFire.Workspaces;

public class WorkspaceTypesConfigurator : IWorkspaceServiceConfigurator
{
    public WorkspaceConfiguration Options { get; }
    public IServiceProvider ServiceProvider { get; }

    public WorkspaceTypesConfigurator(IServiceProvider serviceProvider, IOptions<WorkspaceConfiguration> options)
    {
        Options = options.Value;
        ServiceProvider = serviceProvider;
    }

    public async ValueTask ConfigureWorkspaceServices(IServiceCollection services, UserWorkspacesService userWorkspacesService, string? workspaceId)
    {
        if (userWorkspacesService.UserWorkspaces == null) return;

        var workspaceReference = userWorkspacesService.UserWorkspaces.GetChild(workspaceId);

        await WorkspaceSchemas.InitFilesystemSchemas(userWorkspacesService.UserWorkspaces);

        var dir = new LionFire.IO.Reactive.DirectoryReferenceSelector(userWorkspacesService.UserWorkspaces) { Recursive = true };

        var method = typeof(FsObservableCollectionFactoryX).GetMethod(nameof(FsObservableCollectionFactoryX.RegisterObservablesInSubDirForType))!;

        foreach (var type in Options.MemberTypes)
        {
            var genericMethod = method.MakeGenericMethod(type);
            genericMethod.Invoke(this, [services, ServiceProvider, dir.Path, null]);
        }
    }

}
