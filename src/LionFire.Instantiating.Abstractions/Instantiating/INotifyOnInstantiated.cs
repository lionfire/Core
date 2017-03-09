using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public interface INotifyOnInstantiated
    {
        void OnInstantiated(object instantiationContext = null);
    }
}
