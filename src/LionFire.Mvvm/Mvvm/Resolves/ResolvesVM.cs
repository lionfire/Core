#nullable enable

using LionFire.Resolves;
using System.Reactive;
using MorseCode.ITask;
using System.Reactive.Linq;
using Splat;
using System.Reactive.Disposables;

namespace LionFire.Mvvm;

public class ResolvesVM<T> : ReactiveObject, IResolvesVM<T>, IResolves<T>
{
    #region Model

    public IResolves<T>? Source { get; set; }

    #endregion

    #region Parameters

    public bool ShowRefresh { get; set; } // = false;

    #endregion

    #region Lifecycle

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ResolvesVM()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        Resolve = ReactiveCommand.CreateFromTask(() => ((IResolves<T>)this).Resolve().AsTask());
        Resolve.ThrownExceptions.Subscribe(ex => this.Log().Error(ex, "Something went wrong"));

        this.WhenAnyValue(r => r.Source)
            .Subscribe(resolves => Resolve = ReactiveCommand.CreateFromTask<Unit, IResolveResult<T>>(
                _ => resolves!.Resolve().AsTask(), 
                canExecute: Observable.Create<bool>(o => { o.OnNext(resolves != null); o.OnCompleted(); return Disposable.Empty; })
            ));

        Resolve.IsExecuting.ToPropertyEx(this, vm => vm.IsResolving, initialValue: false);
        Resolve.CanExecute.ToPropertyEx(this, vm => vm.CanResolve, initialValue: false);
    }

    #endregion

    #region IResolves<T>

    ITask<IResolveResult<T>> IResolves<T>.Resolve() => Source.Resolve();

    #endregion

    #region Commands

    public ReactiveCommand<Unit, IResolveResult<T>> Resolve { get; private set; }

    [ObservableAsProperty]
    public bool CanResolve { get; }

    [ObservableAsProperty]
    public bool IsResolving { get; }

    //public bool IsResolving { get { return isResolving.Value; } }
    //readonly ObservableAsPropertyHelper<bool> isResolving;

    #endregion

    public IObservable<IResolveResult<T>> ResolveImpl()
        => Observable.StartAsync(async () => await ((IResolves<T>)this).Resolve());
    
}
