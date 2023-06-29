using LionFire.Data.Sets;
using Microsoft.Extensions.Options;

namespace LionFire.Data;

public class AsyncSets<TValue>
    : ReactiveObject
    , ISetsRx<TValue>
{
    #region Parameters

    #region (static)

    public static AsyncSetOptions DefaultOptions => AsyncSetOptions<TValue>.Default;

    #endregion

    public AsyncSetOptions Options { get; }

    AsyncGetOptions IHasNonNullSettable<AsyncSetOptions>.Object { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public virtual IEqualityComparer<TValue> EqualityComparer => EqualityComparerOptions<TValue>.Default;

    #endregion

    #region Lifecycle

    public AsyncSets() : this(null) { }

    public AsyncSets(AsyncSetOptions? options)
    {
        Options = options ?? AsyncSetOptions<TValue>.Default;
    }

    #endregion

    public IObservable<ITask<ITransferResult>> Sets => throw new NotImplementedException();

    public TValue? StagedValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool HasStagedValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public AsyncValueOptions Object { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    AsyncValueOptions IHasNonNull<AsyncValueOptions>.Object => throw new NotImplementedException();
    public Task<ITransferResult> Set(TValue? value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void DiscardStagedValue()
    {
        throw new NotImplementedException();
    }

    public Task<ITransferResult> Set(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    
}
