#nullable enable

using LionFire.Data.Gets;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;
using LionFire.Data.Gets.Mvvm;

namespace LionFire.Data.Mvvm;

public class VMOptions
{
    public bool ShowRefreshIfHasNoValue { get; set; } = true;
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Source: mutable (REVIEW)
/// </remarks>
public class GetsVM<T>
    : ReactiveObject
    , IGetsVM<T>
{
    #region Model

    #region Source

    [Reactive]
    public IGetsRx<T>? Source { get; set; }
    //{ OLD
    //    get => source;
    //    set
    //    {
    //        if (EqualityComparer<ILazilyGets<TValue>>.Default.Equals(source, value)) { return; }
    //        this.RaiseAndSetIfChanged(ref source, value);
    //        OnSourceChanged(value);
    //    }
    //}
    //private ILazilyGets<TValue>? source;
    //protected virtual void OnSourceChanged(IGets<TValue>? newValue) { }

    IStatelessGets<T>? IReadWrapper<IStatelessGets<T>>.Value => Source;
    IGets<T>? IReadWrapper<IGets<T>>.Value => Source;

    #endregion

    #endregion

    #region Parameters

    public bool ShowRefresh { get; set; } = true; // TEMP

    #endregion

    #region Lifecycle

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public GetsVM()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        //Get = ReactiveCommand.CreateFromTask(() => ((IGets<TValue>)this).Get().AsTask()
        //, canExecute: );
        //Get.ThrownExceptions.Subscribe(ex => this.Log().Error(ex, "Something went wrong"));

        this.WhenAnyValue(r => r.Source).Select(s => s != null);

        #region GetCommand
        GetCommand = ReactiveCommand.CreateFromTask<Unit, IGetResult<T>>(
            _ => (Source ?? throw new ArgumentNullException(nameof(Source))).Get().AsTask(),
            canExecute: this.WhenAnyValue(r => r.Source).Select(s => s != null)
            //canExecute: Observable.Return(Source != null)
        //Observable.Create<bool>(o => { o.OnNext(gets != null); o.OnCompleted(); return Disposable.Empty; })
        );
        GetCommand.ThrownExceptions.Subscribe(ex => this.Log().Error(ex, "Something went wrong"));
        GetCommand.IsExecuting.ToPropertyEx(this, vm => vm.IsGetting, initialValue: false);
        GetCommand.CanExecute.ToPropertyEx(this, vm => vm.CanGet, initialValue: Source != null);

        #endregion

        #region GetIfNeededCommand
        GetIfNeeded = ReactiveCommand.CreateFromTask<Unit, ILazyGetResult<T>>(
                    _ => (Source ?? throw new ArgumentNullException(nameof(Source))).GetIfNeeded().AsTask(),
                    canExecute: Observable.Create<bool>(o => { o.OnNext(Source != null); o.OnCompleted(); return Disposable.Empty; })
                );
        #endregion

        this.WhenAnyValue(r => r.Source)
            .Subscribe(source =>
            {
                observableGetsSubscription?.Dispose();
                observableGetsSubscription = null;

                #region Bind: HasValue  (TODO: Value?)
                if (source is IObservableGets<T> whenGets)
                {
                    // Subscribe to the IObservableResolves
                    observableGetsSubscription = whenGets.Gets.Subscribe(async resultTask =>
                    {
                        var result = await resultTask;
                        HasValue = result.HasValue;
                    });
                }
                else
                {
                    // Subscribe to the ReactiveCommand in this
                    observableGetsSubscription = GetCommand.Subscribe(result => HasValue = result.HasValue);
                }
                HasValue = Source?.HasValue == true;

                #endregion
            });
    }
    IDisposable? observableGetsSubscription;

    #endregion

    #region Gets

    public ReactiveCommand<Unit, ILazyGetResult<T>> GetIfNeeded { get; private set; }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, IGetResult<T>> GetCommand { get; private set; }

    [ObservableAsProperty]
    public bool CanGet { get; }

    [ObservableAsProperty]
    public bool IsGetting { get; }

    //public bool IsGetting { get { return isResolving.Value; } }
    //readonly ObservableAsPropertyHelper<bool> isResolving;

    #endregion

    [Reactive]
    public bool HasValue { get; protected set; }

    

}
