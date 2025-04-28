using LionFire.Deployment;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class ReleaseChannelsConfigurationX
{
    public static ReleaseChannel? GetReleaseChannel(this IHostApplicationBuilder builder) => builder.Configuration.GetReleaseChannel();

    public static ReleaseChannel? GetReleaseChannel(this IConfiguration configuration)
    {
        var value = configuration[ReleaseChannelKeys.ReleaseChannel];
        if (value == null) return null;
        return ReleaseChannels.TryParse(value);
    }

}
