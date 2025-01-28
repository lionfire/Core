using LionFire.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class FrameworkHostingX
{
    public static string Prefix => LionFireWebHostBuilderX.prefix;
    public static LionFireWebHostBuilder Swagger(this LionFireWebHostBuilder lfw, bool enabled = true)
    { lfw.Builder.ConfigureDefaults([new($"{Prefix}:{nameof(WebFrameworkConfig.Swagger)}", enabled.ToString())]); return lfw; }

}
