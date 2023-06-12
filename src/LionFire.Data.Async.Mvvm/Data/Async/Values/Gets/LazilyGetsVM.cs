#nullable enable

using LionFire.Data.Async.Gets;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;

namespace LionFire.Data.Async.Mvvm;

public class VMOptions
{
    public bool ShowRefreshIfHasNoValue { get; set; } = true;
}

public abstract class LazilyGetsVM<T>
    : ReactiveObject
    , ILazilyGetsVM<T>
{
    #region Model

    #region Source

    [Reactive]
    public ILazilyGets<T>? Source { get; set; }
    //{ OLD
    //    get => source;
    //    set
    //    {
    //        if (EqualityComparer<ILazilyGets<T>>.Default.Equals(source, value)) { return; }
    //        this.RaiseAndSetIfChanged(ref source, value);
    //        OnSourceChanged(value);
    //    }
    //}
    //private ILazilyGets<T>? source;
    //protected virtual void OnSourceChanged(IGets<T>? newValue) { }

    IGets<T>? IReadWrapper<IGets<T>>.Value => Source;
    ILazilyGets<T>? IReadWrapper<ILazilyGets<T>>.Value => Source;

    #endregion

    #endregion

    #region Parameters

    public bool ShowRefresh { get; set; } = true; // TEMP

    #endregion

    #region Lifecycle

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public LazilyGetsVM()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        //Get = ReactiveCommand.CreateFromTask(() => ((IGets<T>)this).Get().AsTask()
        //, canExecute: );
        //Get.ThrownExceptions.Subscribe(ex => this.Log().Error(ex, "Something went wrong"));

        this.WhenAnyValue(r => r.Source)
            .Subscribe(resolves =>
            {
                // REVIEW - just making this up as I go.  Does this make any sense?
                var cmd = ReactiveCommand.CreateFromTask<Unit, IGetResult<T>>(
                    _ => resolves!.Get().AsTask(),
                    canExecute: Observable.Return(resolves != null)
                //Observable.Create<bool>(o => { o.OnNext(resolves != null); o.OnCompleted(); return Disposable.Empty; })
                );
                Resolve = cmd;
                cmd.ThrownExceptions.Subscribe(ex => this.Log().Error(ex, "Something went wrong"));
                cmd.IsExecuting.ToPropertyEx(this, vm => vm.IsResolving, initialValue: false);
                cmd.CanExecute.ToPropertyEx(this, vm => vm.CanResolve, initialValue: false);
            });

        this.WhenAnyValue(r => r.Source)
            .Subscribe(resolves =>
            {
                GetIfNeeded = ReactiveCommand.CreateFromTask<Unit, ILazyGetResult<T>>(
                    _ => resolves.TryGetValue().AsTask(),
                    canExecute: Observable.Create<bool>(o => { o.OnNext(resolves != null); o.OnCompleted(); return Disposable.Empty; })
                );

                #region NeedsLazilyResolve

                if (resolves is IObservableGets<T> whenGets)
                {
                    // Subscribe to the IObservableResolves
                    whenGets.Gets.Subscribe(async resultTask =>
                    {
                        var result = await resultTask;
                        HasValue = result.HasValue;
                    });
                }
                else
                {
                    // Subscribe to the ReactiveCommand in this
                    Resolve.Subscribe(result => HasValue = result.HasValue);
                }
                HasValue = Source?.HasValue == true;

                #endregion
            });
    }

    #endregion

    #region Gets

    public ReactiveCommand<Unit, ILazyGetResult<T>> GetIfNeeded { get; private set; }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, IGetResult<T>> Resolve { get; private set; }

    [ObservableAsProperty]
    public bool CanResolve { get; }

    [ObservableAsProperty]
    public bool IsResolving { get; }

    //public bool IsResolving { get { return isResolving.Value; } }
    //readonly ObservableAsPropertyHelper<bool> isResolving;

    #endregion

    public IObservable<IGetResult<T>> GetImpl()
        => Observable.StartAsync(async () => await ((IGets<T>)this).Get());

}
