using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Structures;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using LionFire.Info;

namespace LionFire.Hosting
{
    public static class InfoHostingExtensions
    {
        public static ILionFireHostBuilder Info(this ILionFireHostBuilder b)
            => b.ForHostBuilder(b=>b.ConfigureServices((_,services) => services
                .AddSingleton<NamedSingletons<HierarchicalTagContext>>()));
    }
}
