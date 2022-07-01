using Microsoft.Extensions.Configuration;
using System;

namespace LionFire.AspNetCore;

public class LionFireStartup
{
    protected Uri? uri;
    public int DefaultPort => uri?.Port ?? 5000;

    public IConfiguration Configuration { get; }

    public LionFireStartup(IConfiguration configuration)
    {
        Configuration = configuration;
        uri = KestrelConfig.GetHttpUri(configuration);
    }
}
