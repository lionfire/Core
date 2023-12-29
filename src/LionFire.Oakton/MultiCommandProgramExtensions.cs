using LionFire.Oakton;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting;

public static class MultiCommandProgramExtensions
{
    /// <summary>
    /// Does not use any IServiceProvider.  Oakton Commands must not depend on DI for construction.
    /// To use DI, first call .Build() on the IHostBuilder, and use the constructor that accepts an IHost.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static MultiCommandProgramBuilder MultiCommandProgram(this IHostBuilder builder)
    {
        var b = new MultiCommandProgramBuilder(builder);
        return b;
    }

    ///// <summary>
    ///// Does not use any IServiceProvider.  Oakton Commands must not depend on DI for construction.
    ///// To use DI, first call .Build() on the IHostBuilder, and use the constructor that accepts an IHost.
    ///// </summary>
    ///// <param name="builder"></param>
    ///// <returns></returns>
    //public static MultiCommandProgramBuilder MultiCommandProgram(this HostApplicationBuilder builder)
    //{
    //    var b = new MultiCommandProgramBuilder(builder);
    //    return b;
    //}

    /// <summary>
    /// Use the IServiceProvider from IHost.Services
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public static MultiCommandProgramBuilder MultiCommandProgram(this IHost host)
    {
        var b = new MultiCommandProgramBuilder(host);
        return b;
    }
}