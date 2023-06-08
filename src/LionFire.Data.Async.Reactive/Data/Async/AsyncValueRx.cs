
namespace LionFire.Data.Async;

public abstract class AsyncValueRx<TValue>
    : LazilyGetsRx<TValue>
    , ISets
    , IStagesSet<TValue>
    , IObservableSets<TValue>
    , IHasNonNullSettable<AsyncValueOptions>
{
    #region Options

    #region (static)

    public new static AsyncValueOptions DefaultOptions { get; set; } = new();

    #endregion

    public new AsyncValueOptions Options
    {
        get => (AsyncValueOptions)base.Options;
        set => base.Options = value;
    }
    AsyncValueOptions IHasNonNullSettable<AsyncValueOptions>.Object { get => Options; set => Options = value; }

    AsyncValueOptions IHasNonNull<AsyncValueOptions>.Object => Options;

    #endregion

    #region Lifecycle

    public AsyncValueRx() : base(DefaultOptions) { }

    #endregion

}
