using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public class MExecutionStateChanged<T>
        where T : IExecutable
    {
        public T Source { get; set; }
    }
}
