using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting;

public static class HostApplicationBuilderX_CommandLine 
{
    public static HostApplicationBuilder ConfigureServices(this HostApplicationBuilder hostApplicationBuilder, Action<IServiceCollection> configure)
    {
        configure(hostApplicationBuilder.Services);
        return hostApplicationBuilder;
    }

    public static HostApplicationBuilder ConfigureServices(this HostApplicationBuilder hostApplicationBuilder, Action<HostApplicationBuilder, IServiceCollection> configure)
    {
        configure(hostApplicationBuilder, hostApplicationBuilder.Services);
        return hostApplicationBuilder;
    }

    /// <summary>
    /// Uses reflection to get the IHostBuilder from a HostApplicationBuilder.
    /// </summary>
    /// <param name="hostApplicationBuilder"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// TODO - document what this is all about. Where does AsHostBuilder come from?
    public static IHostBuilder? AsHostBuilder(this HostApplicationBuilder hostApplicationBuilder)
    {
        var mi = hostApplicationBuilder.GetType().GetMethod("AsHostBuilder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (mi == null) throw new Exception("Could not find AsHostBuilder method on HostApplicationBuilder"); // Microsoft changed their internals.
        return mi.Invoke(hostApplicationBuilder, null) as IHostBuilder;
    }

#if OLD // .NET 8 added this for me :) 
    //public static IDictionary<object, object>? Properties(this HostApplicationBuilder hostApplicationBuilder)
    //{
    //    return hostApplicationBuilder.AsHostBuilder()?.Properties;
    //}

    public static IDictionary<object, object>? Properties(this HostApplicationBuilder hostApplicationBuilder)
    {
        return hostApplicationBuilder.Properties();
    }
#endif


}
