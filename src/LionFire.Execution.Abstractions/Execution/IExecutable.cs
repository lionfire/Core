using LionFire.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public interface IExecutable // RENAME to IReadOnlyExecutable?
    {
        // REVIEW - use a property + event here?
        //[Obsolete]
        //IBehaviorObservable<ExecutionState> State { get; }

        ExecutionState State { get; }
        event Action<ExecutionState, IExecutable> StateChangedToFor;

        //ExecutionState CurrentState { get; } // TODO: RENAME to State once State is gone
    }
    
}
