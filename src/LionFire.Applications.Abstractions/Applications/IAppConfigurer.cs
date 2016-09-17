using LionFire.Applications.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Applications
{
    public interface IAppConfigurer : IAppComponent
    {
        void Config(IAppHost app);
    }
   
}
