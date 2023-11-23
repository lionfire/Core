using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting;

public static class WebApplicationBuilderX
{
    public static WebApplicationBuilder LionFire(this WebApplicationBuilder hostBuilder, Action<ILionFireHostBuilder>? action = null, bool useDefaults = true)
    {
        var lf = new LionFireWebApplicationBuilder(hostBuilder);

        if (useDefaults) { lf.Defaults(); }

        action?.Invoke(lf);

        return hostBuilder;
    }


    public static HostApplicationBuilder ToHostApplicationBuilder(this WebApplicationBuilder hostBuilder) 
        => (HostApplicationBuilder)typeof(WebApplicationBuilder).GetField("_hostApplicationBuilder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(hostBuilder) 
        ?? throw new Exception("Failed to get HostApplicationBuilder");

}
