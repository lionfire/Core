
using DynamicData;
using LionFire.ExtensionMethods.Cloning;

namespace LionFire.Data.Collections;

public class OneShotSyncKeyedCollection<TKey, TValue>
    : AsyncReadOnlyKeyedCollection<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    #region Parameters

    private Func<IEnumerable<TValue>>? getImpl;
    private readonly Func<TValue, TKey> keySelector;

    #endregion

    #region Lifecycle

    public OneShotSyncKeyedCollection(Func<IEnumerable<TValue>> getImpl, Func<TValue, TKey> keySelector) : base(keySelector)
    {
        this.getImpl = getImpl;
        this.keySelector = keySelector;
    }

    #endregion

    #region Implementation: GetImpl

    protected override ITask<IGetResult<IEnumerable<TValue>>> GetImpl(CancellationToken cancellationToken = default)
    {
        var getImplCopy = getImpl;
        // THREADSAFETY - could be executed more than once
        if (getImplCopy == null)
        {
            if (HasValue) { return Task.FromResult<IGetResult<IEnumerable<TValue>>>(new NoopGetResult<IEnumerable<TValue>>(HasValue, ReadCacheValue)).AsITask(); }
            else throw new InvalidOperationException("This is a one shot getter and it has already been done.  Do not discard the value if you wish to access it again.");
        }
        else
        {
            var result = Task.FromResult<IGetResult<IEnumerable<TValue>>>(
            GetResult<IEnumerable<TValue>>.SyncSuccess(getImplCopy())
            ).AsITask();
            getImpl = null;

            SourceCache.Edit(updater =>
            {
                var changes = result.Result.Value?.Select(v => new Change<TValue, TKey>(ChangeReason.Add, keySelector(v), v));
                if (changes != null)
                {
                    updater.Clone(new ChangeSet<TValue, TKey>(changes));
                }
            });
            return result;
        }
    }
    public override void OnNext(IGetResult<IEnumerable<TValue>> value) { }

    #endregion
}