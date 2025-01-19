using LionFire.Data.Async.Gets;
using System.Reactive;
using Splat;
using LionFire.Data.Async;
using LionFire.Mvvm;
using System.Reactive.Threading.Tasks;

namespace LionFire.Data.Mvvm;

/// <summary>
/// A smart ViewModel for a single IGetter, which itself may or may not be smart/reactive.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// Source: mutable (REVIEW)
/// </remarks>
public partial class GetterVM<TValue>
: ReactiveObject
, IGetterVM<TValue>
, IViewModel<IGetterRxO<TValue>>
{
    #region Model

    #region Source

    /// <summary>
    /// Consider preferring to use FullFeaturedSource
    /// </summary>
    //[Reactive]
    public IGetter? Source
    {
        get => source;
        set
        {
            if (source == value) { return; }

            if (poller != null)
            {
                poller.Dispose();
                poller = null;
            }

            //        if (EqualityComparer<ILazilyGets<TValue>>.Default.Equals(source, value)) { return; }

            var fullFeaturedSource = value as IGetterRxO<TValue>;

            FullFeaturedSource = value == null ? null : GetterRxOUpscaler.Upscale<TValue>(value);
            this.RaiseAndSetIfChanged(ref source, value);
            TryStartPolling();
        }
    }
    private IGetter? source;

    [ReactiveUI.SourceGenerators.Reactive]
    private IGetterRxO<TValue>? _fullFeaturedSource;

    IGetterRxO<TValue>? IReadWrapper<IGetterRxO<TValue>>.Value => FullFeaturedSource;

    #endregion

    #endregion

    #region Parameters

    public bool ShowRefresh { get; set; } = true; // TEMP

    #endregion

    #region Polling

    AsyncPoller<TValue>? poller;
    public TimeSpan? PollDelay { get; set; }

    private void TryStartPolling()
    {
        if (!PollDelay.HasValue) return;
        if (FullFeaturedSource is IMightFireChangeEvents m && m.FiresChangeEvents == true) { return; }
        //else if (FullFeaturedSource != null) { return; }

        poller = new AsyncPoller<TValue>(() => GetCommand.Execute().ToTask(), PollDelay.Value);
    }

    #endregion

    #region Lifecycle

    public GetterVM(IGetter getter) : this()
    {
        Source = getter;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public GetterVM()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        //Get = ReactiveCommand.CreateFromTask(() => ((IGets<TValue>)this).Get().AsTask()
        //, canExecute: );
        //Get.ThrownExceptions.Subscribe(ex => this.Log().Error(ex, "Something went wrong"));

        this.WhenAnyValue(r => r.FullFeaturedSource).Select(s => s != null);

        #region GetCommand
        GetCommand = ReactiveCommand.CreateFromTask<Unit, IGetResult<TValue>>(
            _ => (FullFeaturedSource ?? throw new ArgumentNullException(nameof(Source))).Get().AsTask(),
            canExecute: this.WhenAnyValue(r => r.FullFeaturedSource).Select(s => s != null)
        //canExecute: Observable.Return(FullFeaturedSource != null)
        //Observable.Create<bool>(o => { o.OnNext(gets != null); o.OnCompleted(); return Disposable.Empty; })
        );
        GetCommand.ThrownExceptions.Subscribe(ex => this.Log().Error(ex, "Something went wrong"));

        _isGetting = GetCommand.IsExecuting.ToProperty(this, x => x.IsGetting, false, false);
        _canGet = GetCommand.CanExecute.ToProperty(this, x => x.CanGet, FullFeaturedSource != null, false);

        #endregion

        #region GetIfNeededCommand
        GetIfNeeded = ReactiveCommand.CreateFromTask<Unit, IGetResult<TValue>>(
                    _ => (FullFeaturedSource ?? throw new ArgumentNullException(nameof(FullFeaturedSource))).GetIfNeeded().AsTask(),
                    canExecute:
                    this.WhenAnyValue(r => r.FullFeaturedSource).Select(s => s != null)

                //Observable.Create<bool>(o => { o.OnNext(FullFeaturedSource != null); o.OnCompleted(); return Disposable.Empty; })
                );
        #endregion

        this.WhenAnyValue(r => r.FullFeaturedSource)
            .Subscribe(source =>
            {
                observableGetsSubscription?.Dispose();
                observableGetsSubscription = null;

                #region Bind: HasValue  (TODO: Value?)
                if (source is IObservableGetOperations<TValue> whenGets)
                {
                    // Subscribe to the IObservableResolves
                    observableGetsSubscription = whenGets.GetOperations.Subscribe(async resultTask =>
                    {
                        var result = await resultTask.ConfigureAwait(false);
                        HasValue = result.HasValue;
                    });
                }
                else
                {
                    // Subscribe to the ReactiveCommand in this
                    observableGetsSubscription = GetCommand.Subscribe(result => HasValue = result.HasValue);
                }
                HasValue = FullFeaturedSource?.HasValue == true;

                #endregion
            });
    }
    IDisposable? observableGetsSubscription;


    #endregion

    #region Gets

    public ReactiveCommand<Unit, IGetResult<TValue>> GetIfNeeded { get; private set; }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, IGetResult<TValue>> GetCommand { get; private set; }

    public bool CanGet => _canGet.Value;
    private ObservableAsPropertyHelper<bool> _canGet;

    public bool IsGetting => _isGetting.Value;
    private ObservableAsPropertyHelper<bool> _isGetting;


    #endregion

    [ReactiveUI.SourceGenerators.Reactive(SetModifier = ReactiveUI.SourceGenerators.AccessModifier.Protected)]
    private bool _hasValue;
}
