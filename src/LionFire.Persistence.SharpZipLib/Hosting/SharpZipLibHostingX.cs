using LionFire.ExtensionMethods;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Persisters.Expanders;
using LionFire.Persisters.SharpZipLib_;
using LionFire.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public static class SharpZipLibHostingX
{
    public const string SharpZipLibName = "SharpZipLib";

    //public static IServiceCollection AddSharpZipLib(this IServiceCollection services)
    //    => services
    //        .Configure<VosPersisterProviderOptions>(o =>
    //        {
    //            o.PersisterNamesToPersisterTypes.Add(SharpZipLibName, typeof(SharpZipLibExpander));
    //        });
    public static IServiceCollection AddSharpZipLib(this IServiceCollection services)
        => services
            .TryAddEnumerableSingleton<IExpander, SharpZipLibExpander>()
            .Configure<SharpZipLibExpanderOptions>(o =>
            {
                o.ConfigureDefault();
            })
        ;

}