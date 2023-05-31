using MorseCode.ITask;
using System;
using System.Collections.Generic;

namespace LionFire.Resolves;

public interface IObservableResolves
{
    bool IsResolving { get; } // => !resolves.Value.AsTask().IsCompleted;
}

public interface IObservableResolves<out TValue> 
    : IResolves<TValue>
    , IObservableResolves
{
    // REVIEW: Typical implementation should always return most recent resolve upon subscribe? Or Noop Result?
    IObservable<ITask<IResolveResult<TValue>>> Resolves { get; } // RENAME to Resolving? because it might be in progress
}