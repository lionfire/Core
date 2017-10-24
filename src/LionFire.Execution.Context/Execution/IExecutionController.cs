using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{

    public interface IExecutionController : IExecutableEx, IStartable, IInitializable
    {
        [SetOnce]
        ExecutionContext ExecutionContext { get; set;  }
        
    }
}
