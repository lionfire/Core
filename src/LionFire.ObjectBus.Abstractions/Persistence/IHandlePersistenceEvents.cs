using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    internal interface IHandlePersistenceEvents
    {
        void OnSaved();
        void OnDeleted();
    }
}
