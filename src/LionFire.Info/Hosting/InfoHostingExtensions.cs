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
        public static LionFireHostBuilder Info(this LionFireHostBuilder b)
            => b.ForHostBuilder(b=>b.ConfigureServices((_,services) => services
                .AddSingleton<NamedSingletons<HierarchicalTagContext>>()));
    }
}
