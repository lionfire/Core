using LionFire.Reactive.Persistence;
using LionFire.UI.Workspaces;
using Microsoft.Extensions.DependencyInjection;
using LionFire.IO.Reactive.Hjson;
using Microsoft.Extensions.Options;
using LionFire.Referencing;
using LionFire.Persistence.Filesystem;

namespace LionFire.Blazor.Components;

public class WorkspaceTypesConfigurator : IWorkspaceServiceConfigurator
{
    public WorkspaceConfiguration Options { get; }
    public IServiceProvider ServiceProvider { get; }

    public WorkspaceTypesConfigurator(IServiceProvider serviceProvider, IOptions<WorkspaceConfiguration> options)
    {
        Options = options.Value;
        ServiceProvider = serviceProvider;
    }

    public async ValueTask ConfigureWorkspaceServices(IServiceProvider? userServices, IReference workspaceReference, IServiceCollection services)
    {
        if (workspaceReference is FileReference fileReference)
        {
            var WorkspacesDir = fileReference.Path;
            await WorkspaceSchemas.InitFilesystemSchemas(WorkspacesDir);

            var dir = new LionFire.IO.Reactive.DirectorySelector(WorkspacesDir) { Recursive = true };

            var method = typeof(WorkspaceTypesConfigurator).GetMethod(nameof(AddFsRWForType))!;
            foreach (var type in Options.MemberTypes)
            {
                var genericMethod = method.MakeGenericMethod(type);
                genericMethod.Invoke(this, [services, dir]);
            }
        }
    }

    // FUTURE: Replace with VOS (or some sort of VFS)
    private void AddFsRWForType<TDocument>(IServiceCollection services, IO.Reactive.DirectorySelector dir)
        where TDocument : notnull
    {
        var r = ActivatorUtilities.CreateInstance<HjsonFsDirectoryReaderRx<string, TDocument>>(ServiceProvider, dir);
        var w = ActivatorUtilities.CreateInstance<HjsonFsDirectoryWriterRx<string, TDocument>>(ServiceProvider, dir);
        services.AddSingleton<IObservableReaderWriter<string, TDocument>>(sp => new ObservableReaderWriterFromComponents<string, TDocument>(r, w));
    }
}
