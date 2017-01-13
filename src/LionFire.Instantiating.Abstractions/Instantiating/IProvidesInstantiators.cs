using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public interface IProvidesInstantiators
    {
        IEnumerable<IInstantiator> Instantiators { get; }
    }
}
