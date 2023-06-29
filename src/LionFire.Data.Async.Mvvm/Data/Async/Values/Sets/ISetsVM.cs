using LionFire.Results;
using ReactiveUI;
using System.Reactive;

namespace LionFire.Data.Sets.Mvvm;

public interface ISetsVM<T>
    : ISetsRx<T>
{
    ReactiveCommand<Unit, ISuccessResult> SetCommand { get; } // REVIEW - does this match impl?
}
