using MorseCode.ITask;
using System;

namespace LionFire.Resolves;

public interface IObservableResolves<out TValue> : IResolves<TValue>
{
    // REVIEW: Typical implementation should always return most recent resolve upon subscribe? Or Noop Result?
    IObservable<ITask<IResolveResult<TValue>>> Resolves { get; }
}
