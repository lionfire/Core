
using DynamicData;
using LionFire.ExtensionMethods.Cloning;

namespace LionFire.Data.Collections;

public class OneShotSyncDictionary<TKey, TValue>
    : AsyncReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    #region Parameters

    private Func<IEnumerable<KeyValuePair<TKey, TValue>>>? getImpl;

    #endregion

    #region Lifecycle

    public OneShotSyncDictionary(Func<IEnumerable<KeyValuePair<TKey, TValue>>> getImpl)
    {
        this.getImpl = getImpl;
    }

    #endregion

    #region Implementation: GetImpl

    protected override ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>> GetImpl(CancellationToken cancellationToken = default)
    {
        var getImplCopy = getImpl;
        // THREADSAFETY - could be executed more than once
        if (getImplCopy == null)
        {
            if (HasValue) { return Task.FromResult<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>>(new NoopGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>(HasValue, ReadCacheValue)).AsITask(); }
            else throw new InvalidOperationException("This is a one shot getter and it has already been done.  Do not discard the value if you wish to access it again.");
        }
        else
        {
            var result = Task.FromResult<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>>(
            GetResult<IEnumerable<KeyValuePair<TKey, TValue>>>.SyncSuccess(getImplCopy())
            ).AsITask();
            getImpl = null;

            SourceCache.Edit(updater =>
            {
                var changes = result.Result.Value?.Select(kvp => new Change<KeyValuePair<TKey, TValue>, TKey>(ChangeReason.Add, kvp.Key, new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value)));
                if (changes != null)
                {
                    updater.Clone(new ChangeSet<KeyValuePair<TKey, TValue>, TKey>(changes));
                }
            });
            return result;
        }
    }
    public override void OnNext(IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>> value) { }

    #endregion
}