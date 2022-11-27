using LionFire.ExtensionMethods;
using LionFire.Persistence.Persisters.SharpZipLib_;
using LionFire.Persistence.Persisters.Vos;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public static class SharpZipLibVosHostingExtensions
{
    public const string SharpZipLibName = "SharpZipLib";

    public static IServiceCollection AddSharpZipLib(this IServiceCollection services)
    {
        return services
            .Configure<ArchivePersisterOptions>("SharpZipLib", o =>
            {
                o.PersisterType = typeof(SharpZipLibPersister);
            })
            .Configure<VosPersisterProviderOptions>(o =>
            {
                o.PersisterNamesToPersisterTypes.Add(SharpZipLibName, typeof(SharpZipLibPersister));
            });
    }

}