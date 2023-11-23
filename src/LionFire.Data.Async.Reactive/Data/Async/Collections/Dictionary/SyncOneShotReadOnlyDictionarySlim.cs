using System.Reactive.Subjects;

namespace LionFire.Data.Collections;

#if OPTIMIZE
// override Get
//  - fire getOperations.OnCompleted
// implement getOperations here instead of base class, as a Lazy, so it never gets fired if nobody subscribes
public class SyncOneShotReadOnlyDictionarySlim<TKey, TValue>
    : AsyncReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    public override async ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>> Get(CancellationToken cancellationToken = default)
    {
        try
        {
            var resultTask = GetImpl(cancellationToken);
            hasValue = true;
            if (getOperations.IsValueCreated)
            {
                getOperations.Value.OnNext(resultTask);
                getOperations.Value.OnCompleted();
            }

            return await resultTask.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return GetResult<IEnumerable<KeyValuePair<TKey, TValue>>>.Exception(ex);
        }
    }

    protected override ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>> GetImpl(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    #region GetOperations

    public override IObservable<ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>>> GetOperations => throw new NotImplementedException();
    protected Lazy<Subject<ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>>>> getOperations = new Lazy<Subject<ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>>>>();

    #endregion
}

#endif