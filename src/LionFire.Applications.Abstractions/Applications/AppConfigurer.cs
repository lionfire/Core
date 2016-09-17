using LionFire.Applications.Hosting;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Applications
{
    public class AppConfigurer : IAppConfigurer
    {
        public AppConfigurer(Action<IAppHost> configMethod) { this.ConfigMethod = configMethod; }

        public Action<IAppHost> ConfigMethod { get; set; }

        public void Config(IAppHost app)
        {
            ConfigMethod(ManualSingleton<IAppHost>.Instance);
        }
    }

}
