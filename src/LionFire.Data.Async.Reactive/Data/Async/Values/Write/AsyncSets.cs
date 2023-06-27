namespace LionFire.Data.Async;

public class AsyncSets<TValue>
{
    #region Parameters

    #region (static)

    public static AsyncGetOptions DefaultOptions { get; set; } = new();

    #endregion

    public AsyncGetOptions Options { get; set; }

    AsyncGetOptions IHasNonNullSettable<AsyncGetOptions>.Object { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public virtual IEqualityComparer<TValue> EqualityComparer => AsyncGetOptions<TValue>.DefaultEqualityComparer;

    #endregion

    #region Lifecycle

    public AsyncSets() : base(DefaultOptions)
    {
        asyncSets = new();
    }

    public AsyncValue(AsyncValueOptions options) : base(options)
    {
        asyncSets = new(options);

        this.ObservableForProperty(t => t.Value)
            .Subscribe(t =>
            {
                if (HasStagedValue)
                {
                    ValueChangedWhileValueStaged = true;
                }
            });
    }

    #endregion
}
