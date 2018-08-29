using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    public interface IHandlePersistenceEvents // REVIEW
    {
        void OnSaved();
        void OnDeleted();
    }
}
