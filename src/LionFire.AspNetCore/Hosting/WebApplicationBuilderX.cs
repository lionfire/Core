using System;
using Microsoft.AspNetCore.Builder;

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
}
