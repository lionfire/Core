#if UNUSED // Superceded by .WebHost() extension method calling IWebHostBuilder.UseUrls
using LionFire.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.AspNetCore;

public static class ConfigurationX
{
    public static ILionFireHostBuilder ConfigureWeb(this ILionFireHostBuilder lf)
    {
        lf.HostBuilder
            .ConfigureAppConfiguration((context, b) =>
            {
                var o = new WebHostConfig().Bind(context.Configuration);

                if (o.HttpEnabled && o.HttpPort.HasValue)
                {
                    b.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Kestrel:Endpoints:Http:Url"] = $"http://{o.HttpInterface}:{o.HttpPort}",
                    });
                }
                if (o.HttpsPort.HasValue)
                {
                    b.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Kestrel:Endpoints:HttpsDefaultCert:Url"] = $"https://{o.HttpsInterface}:{o.HttpsPort}",
                    });
                }
            });
        return lf;
    }
}

#endif