using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Vos
{
    public class RootVob : Vob
    {
        internal RootVob(VBase vos)
            : base(vos, null, String.Empty)
        {
        }
    }
}
