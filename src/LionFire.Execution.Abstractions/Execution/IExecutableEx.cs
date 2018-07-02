using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution
{

    [Obsolete("Migrate to new IExecutable2")]
    public interface IExecutableEx //: IStateful<ExecutionStateEx>
    {
        ExecutionStateEx State { get; }
        event Action<ExecutionStateEx, object> StateChangedToFor;
    }

}
