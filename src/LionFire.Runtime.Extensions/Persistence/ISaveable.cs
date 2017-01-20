using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public interface ISaveable
    {
        Task Save(object persistenceContext = null);
    }
}
