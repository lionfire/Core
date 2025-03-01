using DynamicData;
using LionFire.Data.Collections;
using LionFire.Ontology;
using LionFire.Reactive.Persistence;
using LionFire.Structures.Keys;
using ReactiveUI;
using System.Reactive.Disposables;

namespace LionFire.Data.Async.Collections.DynamicData_;

/// <summary>
/// An async collection (read-write or readonly) that automatically creates ViewModel objects for the items in the collection.
/// </summary>
/// <remarks>
/// Differences from EntitiesVM:
/// - Don't have to implement the abstract write methods
/// - Writing is only available if the IObservableReaderWriter ReaderWriter is set
/// - The ReaderWriter itself knows how to write
/// </remarks>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TValueVM"></typeparam>
public partial class AsyncVMSourceCacheVM<TKey, TValue, TValueVM> : ReactiveObject
    //, IInjectable<Func<TKey, TValue, TValueVM>>
    where TKey : notnull
    where TValue : notnull
    where TValueVM : notnull

    // REVIEW: Should I add any interfaces from any of these?:
    // AsyncReadOnlyKeyedCollection<TKey, TValue>
    // , IAsyncKeyedCollection<TKey, TValue>
    // , AsyncKeyedCollection<TKey, TValueVM>
    // , IInjectable<IKeyProvider<TKey, TValue>>
{
    #region Parameters

    public Func<TKey, TValue, TValueVM> VMFactory { get; set; } = DefaultFactory;
    static Func<TKey, TValue, TValueVM> DefaultFactory = (k, v) => (TValueVM)Activator.CreateInstance(typeof(TValueVM), k, v)!;

    #endregion

    #region Lifecycle

    public AsyncVMSourceCacheVM()
    {
    }

    #endregion

    #region Reader and Writer

    /// <summary>
    /// Set via Reader
    /// </summary>
    public IObservableReaderWriter<TKey, TValue>? ReaderWriter { get => readerWriter; set => Reader = value; }
    private IObservableReaderWriter<TKey, TValue>? readerWriter;

    public IObservableReader<TKey, TValue>? Reader
    {
        get => reader;
        set
        {
            if (reader == value) return;
            readerDisposables?.Dispose();
            reader = value;
            readerDisposables = new();

            if (reader != null)
            {
                readerWriter = reader as IObservableReaderWriter<TKey, TValue>;

                Items = reader.ObservableCache.Connect()
                    .Transform((val, key) => VMFactory(key, val))
                    .AsObservableCache()
                    .DisposeWith(readerDisposables);
            }
        }
    }
    private IObservableReader<TKey, TValue>? reader;

    CompositeDisposable readerDisposables = new();

    #endregion

    /// <summary>
    /// Set via Reader
    /// </summary>
    [Reactive]
    IObservableCache<TValueVM, TKey>? _items;
}


