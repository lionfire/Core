using LionFire.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire
{
    public interface IRequiresInjection
    {
        DependencyContext DependencyContext { get; set; }
    }
}
