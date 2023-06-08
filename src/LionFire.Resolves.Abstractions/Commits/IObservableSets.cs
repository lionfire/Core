using LionFire.Results;
using MorseCode.ITask;
using System;

namespace LionFire.Data.Async.Gets;

public interface IObservableSets<in TValue> : ISets<TValue>
{
    IObservable<ITask<ISuccessResult>> Sets { get; } 
}

