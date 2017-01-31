using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public interface INotifyOnLoaded
    {
        void OnLoaded(object persistenceContext = null);
    }
}
