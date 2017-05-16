using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Composables
{
    public interface IComposable<T> 
        : IEnumerable<object>
         //: IReadOnlyCollection<object> // ? Adds count
    {
        T Add(object component);
    }

    
}
