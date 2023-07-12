using LionFire.Results;
using ReactiveUI;
using System.Reactive;

namespace LionFire.Data.Sets.Mvvm;

public interface ISetsVM<T>
    : ISetsRx<T>
{
    ReactiveCommand<Unit, ITransferResult> SetCommand { get; } // REVIEW - does this match impl?
}
