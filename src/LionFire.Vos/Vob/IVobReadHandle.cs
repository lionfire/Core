using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    public interface IVobReadHandle<
#if !UNITY
out
#endif
 ObjectType>
#if !AOT
 : IReadHandle<ObjectType>
#endif
 where ObjectType : class
    {
        Vob Vob { get; }
        Type Type { get; }
    }

}
