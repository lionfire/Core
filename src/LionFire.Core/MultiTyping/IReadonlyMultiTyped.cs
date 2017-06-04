using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.MultiTyping
{
    public interface IReadonlyMultiTyped
    {
        T AsType<T>() where T : class;

        //IEnumerable<Type> Types { get; }
    }
}
