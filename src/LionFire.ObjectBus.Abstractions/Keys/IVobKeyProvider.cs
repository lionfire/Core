using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    //[SlotName ("KeyProvider")] // FUTURE
    public interface IVobKeyProvider
    {
        void ReturnKey(object key);
        string GetNextKeyAsString();
        object GetNextKey();
    }
}
