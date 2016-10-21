using LionFire.Applications.Hosting;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.MultiTyping;
using LionFire.Execution.Composition;

namespace LionFire.Applications
{
    public class AppConfigurer : Configurer<IAppHost>
    {
        public AppConfigurer(Action<IAppHost> configMethod) : base(configMethod) { }
    }
}
