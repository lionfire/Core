using ICSharpCode.SharpZipLib.Zip;
using LionFire.ExtensionMethods;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Persisters.Expanders;
using LionFire.Persisters.SharpZipLib_;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Services;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Serialization.SharpZipLib;

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
            .AddSharpZipLibPersistence()
            .TryAddEnumerableSingleton<IExpander, SharpZipLibExpander>()


            .Configure<SharpZipLibExpanderOptions>(o =>
            {
                o.ConfigureDefault();
            })

        ;

}