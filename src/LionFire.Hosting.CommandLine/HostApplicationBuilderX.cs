using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Extensions.Hosting;

public static class HostApplicationBuilderX2 // MOVE to LionFire.Extensions.Hosting
{
    public static IHostBuilder? AsHostBuilder(this HostApplicationBuilder hostApplicationBuilder)
    {
        var mi = hostApplicationBuilder.GetType().GetMethod("AsHostBuilder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (mi == null) throw new Exception("Could not find AsHostBuilder method on HostApplicationBuilder"); // Microsoft changed their internals.
        return mi.Invoke(hostApplicationBuilder, null) as IHostBuilder;
    }
    public static IDictionary<object, object>? Properties(this HostApplicationBuilder hostApplicationBuilder)
    {
        return hostApplicationBuilder.AsHostBuilder()?.Properties;
    }
}
