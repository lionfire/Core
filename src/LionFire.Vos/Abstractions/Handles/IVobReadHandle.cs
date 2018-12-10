using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public interface IVobReadHandle<out ObjectType> : RH<ObjectType>
    {
        IVob Vob { get; }
        Type Type { get; }
    }

}
