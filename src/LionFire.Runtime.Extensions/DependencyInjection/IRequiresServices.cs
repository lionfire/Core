using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire
{
    public interface IRequiresServices
    {
        IServiceProvider ServiceProvider { set; }
    }
}
