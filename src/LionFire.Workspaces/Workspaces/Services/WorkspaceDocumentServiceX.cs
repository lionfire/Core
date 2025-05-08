using LionFire.Workspaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public static class WorkspaceDocumentServiceX
{
    public static IServiceCollection AddWorkspaceDocumentService<TKey, TValue>(this IServiceCollection services)
        where TKey : notnull
        where TValue : notnull
    {
        services
            .AddHostedSingleton<DirectoryWorkspaceDocumentService<TValue>>();

        return services;
    }
}
