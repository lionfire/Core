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

public static class SharpZipLibPersisterHostingX
{
    public const string SharpZipLibName = "SharpZipLib";
    
    public static IServiceCollection AddSharpZipLibPersistence(this IServiceCollection services)
        => services
            .AddSingleton<SharpZipLibSerializer>()
            .TryAddEnumerableSingleton<ISerializationStrategy, SharpZipLibSerializer>()

            .AddSingleton<IReadHandleProvider<IReference<ZipFile>>, SharpZipLibHandleProvider>()
            .Configure<ReferenceToHandleOptions>(o=>
            {
                o.TypesRequiringTransform.Add(typeof(ZipFile));
            })
            //.Configure<TypeTransformerOptions>(o =>
            //{
            //    o
            //        .RegisterSourceForTarget<ZipFile, byte[]>()
            //        .RegisterSourceForTarget<ZipFile, Stream>()
            //        ;
            //})
        ;
}