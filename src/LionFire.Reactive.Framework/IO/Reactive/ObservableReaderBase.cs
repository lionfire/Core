using DynamicData.Kernel;
using LionFire.Reactive;
using LionFire.Reactive.Persistence;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LionFire.IO.Reactive;

public abstract class ObservableReaderBase<TKey, TValue> //: IObservableReader<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{

    #region Relationships

    public IObservableWriter<TKey, TValue>? PairedWriter
    {
        get => pairedWriter;
        set
        {
            if (ReferenceEquals(pairedWriter, value)) return;
            if (pairedWriter != null) { throw new AlreadySetException(); }

            pairedWriter = value;

            writerSubscription = pairedWriter!.WriteOperations.Subscribe(o => OnWrite(o));
        }
    }
    private IObservableWriter<TKey, TValue>? pairedWriter;
    private IDisposable? writerSubscription;

    protected int millisecondsToWaitForWriteCompletion = 500;


    private void OnWrite(WriteOperation<TKey, TValue> writeOperation)
    {
        // REVIEW - lock(pendingWritesLock) needed in more places?
        var originalExpiry = DateTime.UtcNow + TimeSpan.FromMilliseconds(millisecondsToWaitForWriteCompletion);
        pendingWrites?.AddOrUpdate(writeOperation.Key, originalExpiry, (_, _) => originalExpiry);

        Task.Run(async () =>
        {
            try
            {
                await writeOperation.Completion;

                _ = Task.Delay(millisecondsToWaitForWriteCompletion).ContinueWith(_ =>
                {
                    lock (pendingWritesLock)
                    {
                        pendingWrites.TryRemove(writeOperation.Key, out var currentExpiry);
                        if (currentExpiry != originalExpiry && currentExpiry > originalExpiry)
                        {
                            // Put it back.  Another Task should be waiting on it.
                            pendingWrites.AddOrUpdate(writeOperation.Key, currentExpiry, (_, _) => currentExpiry);
                        }
                    }
                }, TaskScheduler.Default).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                pendingWrites.TryRemove(writeOperation.Key, out _);
            }
        });
    }
    protected bool IsPendingWrite(TKey key) => pendingWrites?.ContainsKey(key) == true;

    #endregion

    #region Lifecycle

    public ObservableReaderBase()
    {
        Values = values.Connect().Transform(x => x.optional).AsObservableCache();
        StartExpirationCleanup();
    }

    protected virtual void Dispose()
    {
        writerSubscription?.Dispose();
        pendingWrites?.Clear();
        pendingWrites = null;
    }

    #endregion

    private void StartExpirationCleanup()
    {
        _ = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(1000).ConfigureAwait(false);
                var pendingWritesCopy = pendingWrites;
                if (pendingWritesCopy == null) return;

                var now = DateTime.UtcNow;
                foreach (var key in pendingWritesCopy.Keys)
                {
                    if (pendingWritesCopy.TryGetValue(key, out var timestamp) && now - timestamp > _expirationTime)
                    {
                        pendingWritesCopy.TryRemove(key, out _);
                    }
                }
            }
        });
    }
    private readonly TimeSpan _expirationTime = TimeSpan.FromSeconds(5); 

    #region State

    protected IObservableCache<(TKey key, Optional<TValue> optional), TKey> KeyedValues => values.AsObservableCache();
    protected SourceCache<(TKey key, Optional<TValue> optional), TKey> values = new(x => x.key);

    private ConcurrentDictionary<TKey, DateTime>? pendingWrites = new();
    private object pendingWritesLock = new();



    #region Derived

    public IObservableCache<Optional<TValue>, TKey> Values { get; }
    //    valuesWithSubscribeEvents ??=
    //        values
    //            .ConnectOnDemand(v => v)
    //            .PublishRefCountWithEvents(() => EnableKeysWatcher(), () => DisableKeysWatcher())
    //            .AsObservableCache();
    //private IObservableCache<Optional<TValue>, TKey>? valuesWithSubscribeEvents;

    public abstract IObservableCache<Optional<TValue>, TKey> ObservableCache { get; }

    #endregion

    #endregion
}
