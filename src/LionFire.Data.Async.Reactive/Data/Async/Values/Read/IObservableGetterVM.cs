using MorseCode.ITask;
using System;
using System.Collections.Generic;

namespace LionFire.Data.Async.Gets;

public interface IObservableGetterVM
{
    bool IsResolving { get; } // => !gets.Value.AsTask().IsCompleted;
}

public interface IObservableGets<out TValue> 
    : IStatelessGetter<TValue>
    , IObservableGetterVM
{
    // REVIEW: Typical implementation should always return most recent resolve upon subscribe? Or Noop Result?
    IObservable<ITask<IGetResult<TValue>>> Gets { get; }
}

