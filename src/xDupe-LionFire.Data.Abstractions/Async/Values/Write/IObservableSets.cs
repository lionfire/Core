using LionFire.Results;
using MorseCode.ITask;
using System;

namespace LionFire.Data.Sets;

public interface IObservableSets<in TValue> : ISets<TValue>
{
    IObservable<ITask<ITransferResult>> Sets { get; } 
}

