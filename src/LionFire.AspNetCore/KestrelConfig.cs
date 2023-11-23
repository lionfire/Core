using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.AspNetCore;

public class KestrelConfig
{
    public static Uri? GetHttpUri(IConfiguration configuration)
    {
        var urlString = configuration.GetSection("Kestrel").GetSection("Endpoints").GetSection("Http")["Url"];
        return urlString == null ? null : new Uri(urlString);
    }
}

