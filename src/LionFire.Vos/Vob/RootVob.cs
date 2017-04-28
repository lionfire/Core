using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    public class RootVob : Vob
    {
        internal RootVob(Vos vos)
            : base(vos, null, String.Empty)
        {
        }
    }
}
