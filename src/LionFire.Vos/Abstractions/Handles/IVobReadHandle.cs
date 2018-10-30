using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public interface IVobReadHandle<
#if !UNITY
out
#endif
 ObjectType>
#if !AOT
    : R<ObjectType>
#endif
 where ObjectType : class
    {
        IVob Vob { get; }
        Type Type { get; }
    }

}
