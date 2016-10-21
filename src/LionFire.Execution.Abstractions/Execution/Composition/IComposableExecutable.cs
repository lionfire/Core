using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution.Composition
{
    public interface IComposableExecutable<T>
    {
        T Add(IConfigures configurer);
        T Add(IInitializes initializer);
    }
}
