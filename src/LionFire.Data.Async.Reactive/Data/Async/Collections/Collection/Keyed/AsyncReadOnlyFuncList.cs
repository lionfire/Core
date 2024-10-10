using MorseCode.ITask;
using LionFire.Data.Collections;
using LionFire.Data.Async.Gets;
using DynamicData;

namespace LionFire.Data.Collections;

public class AsyncReadOnlyFuncList<TItem> : AsyncReadOnlyList<TItem>
    , IEnumerable<TItem>
    where TItem : notnull
{
    #region Parameters

    public Func<ValueTask<IEnumerable<TItem>>> Func { get; }

    #endregion

    #region Lifecycle

    public AsyncReadOnlyFuncList(Func<ValueTask<IEnumerable<TItem>>> func)
    {
        Func = func;
    }

    #endregion

    #region State

    public override IEnumerable<TItem>? ReadCacheValue => readCacheValue;
    private IEnumerable<TItem>? readCacheValue;

    public override void DiscardValue() => readCacheValue = null;

    #endregion

    protected override async ITask<IGetResult<IEnumerable<TItem>>> GetImpl(CancellationToken cancellationToken = default)
        => GetResult<IEnumerable<TItem>>.Success(await Func());

    #region Event Handling

    public override void OnNext(IGetResult<IEnumerable<TItem>> result)
    {
        readCacheValue = result.Value;
        SourceList.EditDiff(readCacheValue ?? []);
    }

    #endregion
}
