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
        IBehaviorObservable<ExecutionState> State { get; }

    }

}
