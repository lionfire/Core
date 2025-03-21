﻿using DynamicData;
using DynamicData.Kernel;
using LionFire.Data.Collections;
using LionFire.Ontology;
using LionFire.Reactive.Persistence;
using LionFire.Structures.Keys;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace LionFire.Data.Mvvm;

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
public partial class ObservableDataVM<TKey, TValue, TValueVM> : ReactiveObject
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

    public Func<TKey, Optional<TValue>, TValueVM>? VMFactory { get; set; }
    public Func<TKey, Optional<TValue>, TValueVM> EffectiveVMFactory => VMFactory ?? DefaultFactory;

    static Func<TKey, Optional<TValue>, TValueVM> DefaultFactory = (k, v) => (TValueVM)Activator.CreateInstance(typeof(TValueVM), k, v.HasValue ? v.Value : null)!;

    #endregion

    #region Lifecycle

    public ObservableDataVM()
    {
    }

    #endregion

    #region Reader and Writer

    /// <summary>
    /// Set via Reader
    /// </summary>
    public IObservableReaderWriter<TKey, TValue>? DataWriter => Data as IObservableReaderWriter<TKey, TValue>;
    //{ get => dataWriter; set => Data = dataWriter = value; }
    // private IObservableReaderWriter<TKey, TValue>? dataWriter;

    public IObservableReader<TKey, TValue>? Data
    {
        get => data;
        set
        {
            if (ReferenceEquals(data, value)) return;
            readerDisposables?.Dispose();
            data = value;
            readerDisposables = new();

            if (data != null)
            {
                //dataWriter = reader as IObservableReaderWriter<TKey, TValue>;

                Items = data.Values.Connect()
                    .Transform((val, key) =>
                    {
                        try
                        {
                            return EffectiveVMFactory(key, val);
                        }
                        catch(Exception ex)
                        {
                            Debug.WriteLine("ERROR: VMFactory failed: " + ex.ToString()); // TODO Log or throw somehow
                            throw;
                        }
                    })
                    .AsObservableCache()
                    .DisposeWith(readerDisposables);

                itemsChangedSubject.OnNext(Unit.Default);
                data.ListenAll().DisposeWith(readerDisposables);
            }
        }
    }
    private IObservableReader<TKey, TValue>? data;
    

    CompositeDisposable readerDisposables = new();

    #endregion

    #region New IObservable<Unit> for Items changes

    private readonly Subject<Unit> itemsChangedSubject = new();
    public IObservable<Unit> ItemsChanged => itemsChangedSubject.AsObservable();

    #endregion

    public IObservableCache<TValueVM, TKey>? Items
    {
        get => _items;
        set
        {
            this.RaiseAndSetIfChanged(ref _items, value);
            // Notify subscribers that Items have changed
            itemsChangedSubject.OnNext(Unit.Default);

            // Subscribe to changes in the Items collection
            if (_items != null)
            {
                _items.Connect()
                    .Subscribe(_ => itemsChangedSubject.OnNext(Unit.Default))
                    .DisposeWith(readerDisposables);
            }
        }
    }
    private IObservableCache<TValueVM, TKey>? _items;

    public bool ShowRefresh { get; set; }
}

