using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Structures
{
    public interface IComposable<T> 
        : IEnumerable<object>
        // : IFreezable // ?
        // : ICollection<object> // ?
    {
        T Add(object component);
        //T Add(IConfigures configurer);
        //T Add(IInitializes initializer);
    }
}
