using LionFire.IO.Reactive.Filesystem;
using LionFire.Workspaces;
using LionFire.Workspaces.Services;
using Microsoft.Extensions.DependencyInjection;
using LionFire.IO.Reactive.Filesystem;
using LionFire.Execution;

namespace LionFire.Hosting;

public static class WorkspaceDocumentServiceX
{
    public static IServiceCollection AddWorkspaceDocumentService<TKey, TValue, TValueVM, TRunner>(this IServiceCollection services)
        where TKey : notnull
        where TValue : notnull
        where TValueVM : IEnablable
        where TRunner : IRunner<TValue>
    {
        services
            .AddHostedSingleton<DirectoryWorkspaceDocumentService<TValue, TValueVM, TRunner>>();

        return services;
    }
}
