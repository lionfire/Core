using LionFire.Referencing;
using LionFire.Vos.Aliases;
using LionFire.Vos.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public static class VobAliasServiceExtensions
{
    public static IServiceCollection VobAlias(this IServiceCollection services, string path, string target)
    {
        var chunks = LionPath.ToPathArray(path);

        services.InitializeVob(new ArraySegment<string>(chunks, 0, chunks.Length - 1), v =>
        {
            v.MultiTyped().AsTypeOrCreateDefault<VobAliases>().Set(path, target);
        });
        //services.InitializeVob(new ArraySegment<string>(chunks, 0, chunks.Length - 1), v =>
        //{
        //    v.MultiTyped().AsTypeOrCreateDefault<VobAliases>().Set(path, target);
        //});

        return services;
    }
}
