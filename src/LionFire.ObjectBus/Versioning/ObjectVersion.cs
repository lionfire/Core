using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    public struct ObjectVersion
    {
        [Ignore(LionSerializeContext.Persistence)]
        public short Major;
        [Ignore(LionSerializeContext.Persistence)]
        public short Minor;
        [Ignore(LionSerializeContext.Persistence)]
        public short Build;
        [Ignore(LionSerializeContext.Persistence)]
        public short Revision;
    }
}
