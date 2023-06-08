#if UNUSED
using MorseCode.ITask;
using System;
using System.Collections.Generic;

namespace LionFire.Data.Async.Gets;

public interface IObservableResolves
{
    bool IsResolving { get; } // => !resolves.Value.AsTask().IsCompleted;
}

public interface IObservableResolves<out TValue> 
    : IGets<TValue>
    , IObservableResolves
{
    // REVIEW: Typical implementation should always return most recent resolve upon subscribe? Or Noop Result?
    IObservable<ITask<IResolveResult<TValue>>> Resolves { get; } // RENAME to Resolving? because it might be in progress
}
#endif