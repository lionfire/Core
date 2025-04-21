using DynamicData;
using DynamicData.Kernel;
using LionFire.Data.Collections;
using LionFire.Ontology;
using LionFire.Reactive.Persistence;
using LionFire.Reflection;
using LionFire.Structures.Keys;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

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
    , ICreatesAsyncVM<TValue>
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

    #region State

    public bool ShowRefresh { get; set; }
    public bool ShowDeleteColumn { get; set; }

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
                        catch (Exception ex)
                        {
                            Debug.WriteLine("ERROR: VMFactory failed: " + ex.ToString()); // TODO Log or throw somehow
                            throw;
                        }
                    })
                    .AsObservableCache()
                    .DisposeWith(readerDisposables);

                data.ListenAllKeys().DisposeWith(readerDisposables);
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


    #region Creation

    public IEnumerable<Type> CreatableTypes
    {
        get => [.. (creatableTypes ?? (Type[])[]), .. (CanCreateValueType ? (Type[])[typeof(TValue)] : [])];
        set => creatableTypes = value;
    }
    private IEnumerable<Type>? creatableTypes;

    public bool CanCreateValueType { get; set; }

    public ReactiveCommand<ActivationParameters, Task<TValue>> Create
            => ReactiveCommand.Create<ActivationParameters, Task<TValue>>(async ap =>
            {
                var key = "~New~" + GenerateRandomName();
                var value = InstantiateNew();
                await DataWriter!.Write((TKey)(object)key, value);
                return value;
            }, canExecute: Observable.Return(DataWriter != null && typeof(TKey) == typeof(string)));

    public string GenerateRandomName(int length = 10)
    {
        string allowedCharacters = "ABCDEFGHJKLMNPQRTUVWXYZ2346789";

        var sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            sb.Append(allowedCharacters[Random.Shared.Next(0, allowedCharacters.Length)]);
        }
        return sb.ToString();
    }

    public TValue InstantiateNew()
    {
        return Activator.CreateInstance<TValue>();
    }

    #endregion

    #region AllowedEditModes

    public EditMode AllowedEditModes { get; set; }

    public bool CanCreate => AllowedEditModes.CanCreate();
    public bool CanRename => AllowedEditModes.CanRename();
    public bool CanDelete => AllowedEditModes.CanDelete();
    public bool CanUpdate => AllowedEditModes.CanUpdate();

    #endregion

    public async Task Delete(TValueVM value)
    {
        if (!CanDelete || DataWriter == null) return;

        if (value is IKeyed<TKey> keyed)
        {
            Debug.WriteLine("Deleting " + value);
            await DataWriter.Remove(keyed.Key);
        }
        else
        {
            var kv = Items.KeyValues.Where(kv => ReferenceEquals(value, kv.Value)).FirstOrDefault();

            if (kv.Key == null)
            {
                Debug.WriteLine("ERROR: Delete: Could not find key for value " + value);
                return;
            }
            else
            {
                Debug.WriteLine("Deleting " + value);
                await DataWriter.Remove(kv.Key);
            }
        }
    }
}
