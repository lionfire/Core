using Microsoft.Extensions.DependencyInjection;
using LionFire.Vos;
using LionFire.Dependencies;
using LionFire.Referencing;
using LionFire.Vos.Mounts;
using LionFire.FlexObjects;

namespace LionFire.Hosting;

public static class VosMountServiceCollectionExtensions
{
    // TODO ENH: resolve urls, and support mounting to strings, such as file:///c:/temp/abc, or default to parsing from local filesystem: /dev/null, c:/temp/abc.


    /// <summary>
    /// Defaults to Read mount if no options specified or both ReadPriority and WritePriority are null
    /// </summary>
    public static IServiceCollection VosMount(this IServiceCollection services, VobReference vobReference, IReference reference, IMountOptions options = null)
        => services.VosMount((IVobReference)vobReference, reference, options);

    /// <summary>
    /// Defaults to Read mount if no options specified or both ReadPriority and WritePriority are null
    /// </summary>
    public static IServiceCollection VosMount(this IServiceCollection services, IVobReference vobReference, IReference reference, IMountOptions options = null)
    {
        if (options == null) options = new MountOptions();
        if (options.ReadPriority == null && options.WritePriority == null)
        {
            options.ReadPriority = MountOptions.DefaultReadPriority;
        }

        return services.Configure<VobRootOptions>(options?.RootName ?? VosConstants.DefaultRootName, o => o.Mounts.Add(new TMount(vobReference, reference, options)));
    }

    /// <summary>
    /// Defaults to Read mount if no options specified
    /// </summary>
    /// <param name="services"></param>
    /// <param name="mountPoint"></param>
    /// <param name="reference"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection VosMountRead(this IServiceCollection services, VobReference mountPoint, IReference reference)
       => VosMount(services, mountPoint, reference, MountOptions.DefaultRead);
    public static IServiceCollection VosMountReadWrite(this IServiceCollection services, VobReference mountPoint, IReference reference)
        => VosMount(services, mountPoint, reference, MountOptions.DefaultReadWrite);
    public static IServiceCollection VosMountWrite(this IServiceCollection services, VobReference mountPoint, IReference reference)
        => VosMount(services, mountPoint, reference, MountOptions.DefaultWrite);

}
