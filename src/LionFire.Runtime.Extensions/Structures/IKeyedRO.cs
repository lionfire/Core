using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Structures
{
    public interface IKeyedRO<TKey>
    {
        TKey Key { get; }
    }
}
