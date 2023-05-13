#nullable enable

using System.Reactive.Disposables;
using LionFire.ExtensionMethods.Types;
using LionFire.Collections.Async;
using System.Reactive.Linq;

namespace LionFire.Mvvm;

public class AsyncDictionaryVM<TKey, TValue, TValueVM> : LazilyResolvesDictionaryVM<TKey, TValue>
    , ICreatesAsyncVM<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    #region Lifecycle

    public AsyncDictionaryVM() 
    {
        Create = ReactiveCommand.Create<ActivationParameters, Task<KeyValuePair<TKey, TValue>>>(p =>
            (Cache as ICreatesAsync<KeyValuePair<TKey, TValue>> ?? throw new ArgumentException($"{nameof(Cache)} must be ICreatesAsync<KeyValuePair<TKey, TValue>>")).Create(p.Type, p.Parameters)
        , Observable.Create<bool>(o =>
        {
            o.OnNext(Cache is ICreatesAsync<KeyValuePair<TKey, TValue>>);
            return Disposable.Empty;
        }));
    }

    #endregion

    #region ReadOnly

    public bool ReadOnly { get; set; }

    public IObservable<(string key, Type type, object[] parameters, Task result)> CreatesForKey => throw new NotImplementedException();

    void ValidateCanModify()
    {
        if (ReadOnly) { throw new InvalidOperationException($"ReadOnly is true"); }
        //if (Cache.IsReadOnly) { throw new InvalidOperationException($"Cache.ReadOnly is true"); }
    }

    #endregion

    #region User Input

    #region Create
    
    public ReactiveCommand<ActivationParameters, Task<KeyValuePair<TKey, TValue>>> Create { get; }


    // TODO Triage
    //public async void OnCreate(Type type)
    //{
    //    ValidateCanModify();

    //    try
    //    {
    //        if (Create != null)
    //        {
    //            await Create(type, null);
    //            return;
    //        }
    //        else if (Cache is ICreatesAsync<TValue> cc)
    //        {
    //            await cc.Create(type);
    //            return;
    //        }
    //        //else if (AddNew != null)
    //        //{
    //        //    await AddNew(type);
    //        //    return;
    //        //}
    //        //else if (Cache is IAddsAsync addsNew)
    //        //{
    //        //    await addsNew.AddNew(type);
    //        //    return;
    //        //}
    //        else if (Cache is IAddsAsync<TValue> adds)
    //        {
    //            await adds.Add(await instantiate(type));
    //            return;
    //        }
    //        else
    //        {
    //            throw new NotSupportedException();
    //        }
    //    }
    //    finally
    //    {
    //        //await (ResolvesVM?.Resolve() ?? Task.CompletedTask);
    //        //StateHasChanged();
    //    }

    //    async Task<TValue> instantiate(Type type)
    //    {
    //        object[] args = Array.Empty<object>();
    //        if (ItemConstructorParameters != null) { args = ItemConstructorParameters.Invoke(type); }

    //        if (Create != null) { return await Create(type, args); }
    //        else if (Cache is ICreatesAsync<TValue> cc && cc.CanCreate)
    //        {
    //            return await cc.Create(type, args);
    //        }

    //        return (TValue)ActivatorUtilities.CreateInstance(ServiceProvider, type, args)
    //            ?? (TValue?)Activator.CreateInstance(type, args)
    //            ?? throw new Exception($"Failed to create item of type {type.FullName}");
    //    }
    //}

    #endregion

    #endregion

    //#region ICreatesAsync<TValue>

    //public abstract IObservable<(Type, object[], Task<KeyValuePair<TKey, TValue>>)> Creates { get; }

    //public abstract Task<KeyValuePair<TKey, TValue>> Create(Type type, params object[] constructorParameters);

    //#endregion
}

//public class AsyncDictionaryVM<TKey, TValue, TValueVM> : AsyncCollectionVM<TKey, TValue>
//    //, ICreatesAsync<TKey, TValue>
//    where TKey : notnull
//{

//    //#region ICreatesAsync<TKey, TValue>

//    //public Task<TValue> CreateForKey(string key, Type type, params object[] constructorParameters)
//    //{
//    //    throw new NotImplementedException();
//    //}

//    //public Task<TValue> GetOrCreateForKey(string key, Type type, params object[] constructorParameters)
//    //{
//    //    throw new NotImplementedException();
//    //}

//    //#endregion
//}

public abstract class AsyncDictionaryVM<TKey, TValue> : AsyncDictionaryVM<TKey, TValue, object>
    where TKey : notnull
{
    
}

// vvv TODO: Triage vvv
#if TODO // Triage
public class DictionaryVM<TKey, TValue, TValueVM> : CollectionVMBase<KeyValuePair<TKey, TValue>>, IViewModel<IEnumerable<KeyValuePair<TKey, TValue>>>, IActivatableViewModel
    where TKey : notnull
{
    #region Dependencies

    #region Optional

    public IKeyProvider<TKey, TValue>? KeyProvider { get; set; }

    #endregion

    #endregion

    public ICache<TValue, TKey>? Cache { get => asyncCollectionCache; set => asyncCollectionCache = value; }
    private ICache<TValue, TKey>? asyncCollectionCache;


    #region Extensions

    public IMutableDictionaryVM<TKey, TValue, TValueVM>? ActiveVM { get; set; } // TODO REVIEW

    #endregion

    #region Parameters

    #endregion

    #region Configuration

    public Func<TValue, TValueVM?>? VMFactory { get; set; } = null;

    public Func<TValue, TKey?> ValueKeyProvider { get; set; } = DefaultValueKeyProvider;
    public static Func<TValue, TKey?> DefaultValueKeyProvider = v => (v as IKeyed<TKey> ?? throw new NotSupportedException()).Key;

    public Func<TValueVM, TKey?> ValueVMKeyProvider { get; set; } = DefaultValueVMKeyProvider;
    public static Func<TValueVM, TKey?> DefaultValueVMKeyProvider = v => (v as IKeyed<TKey> ?? throw new NotSupportedException()).Key;

    #endregion

    #region Lifecycle

    public DictionaryVM(IServiceProvider serviceProvider)
    {
        ViewModelProvider = serviceProvider.GetService<IViewModelProvider>();
        KeyProvider = serviceProvider.GetService<IKeyProvider<TKey, TValue>>();

        FallbackKeyProvider = serviceProvider.GetService<IKeyProvider<TKey, object>>();

        InitializerForChildViewModels = vm =>
        {
            if (vm is IParentable<DictionaryVM<TKey, TValue, TValueVM>> parentable) parentable.Parent = this;
        };

        this.WhenActivated(async disposable =>
        {
            this.WhenAnyValue(vm => vm.Value)
                .Subscribe(v =>
                {
                    BindToChildren(v);
                })
                .DisposeWith(disposable);

            // TODO REVIEW
            if (Cache != null && !Cache.HasRetrieved && AutoRetrieveOnInit && Cache.CanRetrieve)
            {
                //Resolve..Execute()

                await (ResolveC() ?? Task.CompletedTask);
            }
            await SubscribeAsync(); // TEMP
        });

    }

    #endregion

    #region Model

    [Reactive]
    public IEnumerable<KeyValuePair<TKey, TValue>>? Value { get; set; }

    //public IObservableCache<TValueVM, TItem>? ViewModels { get; set; }
    //public IObservableCache<TValue, TItem>? Values { get; set; }

    //public IEnumerable<TValue> Items
    //{
    //    get => items ?? (emptyItems ??= new FreezedList<TValue>(Enumerable.Empty<TValue>().ToList()));
    //    set
    //    {
    //        if (Object.ReferenceEquals(items, value)) return;
    //        items = value;


    //    }
    //}
    //private IEnumerable<TValue>? items;
    //private IEnumerable<TValue>? itemsForView;
    //private static IEnumerable<TValue>? emptyItems;

    #endregion

    #region State

    public extern bool IsRefreshing { [ObservableAsProperty] get; }

    //.ToPropertyEx(this, x => x.PersonInfo);
    //public bool IsRefreshing { get => isRefreshing.Value; set { isRefreshing.OnNext(value); } }
    //private BehaviorSubject<bool> isRefreshing = new BehaviorSubject<bool>(false);

    #endregion

    #region Children

    public IEnumerable<TValueVM>? Children => childViewModels;
    //IEnumerable<KeyValuePair<TItem, TValueVM>>? children;
    protected ReadOnlyObservableCollection<TValueVM>? childViewModels;

    public Func<TValue, object[]>? ConstructorParametersForChildViewModels { get; set; } = v => Array.Empty<object>();
    public Action<TValueVM>? InitializerForChildViewModels { get; set; } // Init in ctor

    protected virtual void BindToChildren(IEnumerable<KeyValuePair<TKey, TValue>>? value)
    {
        if (value is IObservableCache<TValue, TKey> cacheValues)
        {
            if (ViewModelProvider != null && ViewModelProvider.CanTransform<TValue, TValueVM>())
            {
                cacheValues.Connect().Transform(modelItem =>
                        ViewModelProvider.Activate(
                            modelItem,
                            InitializerForChildViewModels,
                            ConstructorParametersForChildViewModels?.Invoke(modelItem)
                        )
                    )
                    .Bind(out childViewModels)
                    .DisposeMany();
            }
        }
        else if (value is IObservableList<TValue> listValues)
        {
            if (ViewModelProvider != null && ViewModelProvider.CanTransform<TValue, TValueVM>())
            {
                listValues.Connect().Transform(modelItem =>
                        ViewModelProvider.Activate(
                            modelItem,
                            InitializerForChildViewModels,
                            ConstructorParametersForChildViewModels?.Invoke(modelItem)
                        )
                    )
                    .Bind(out childViewModels)
                    .DisposeMany();
            }
        }
        else
        {
            childViewModels = null;
        }

#if OLD
if (items is ICache<TValue> vm)
                {
                    Cache = vm;
                    View = Cache.Collection.CreateView(VMFactory);
                }
                else if (items is IObservableCollection<TValue> observableCollection)
                {
                    View = observableCollection.CreateView(VMFactory);
                }
                else
                {
                    // It is often the case that Items is not Observable.
                    // In that case, FreezedList is provided to create a View with the same API for normal collections.
                    var list = items as IReadOnlyList<TValue> ?? (IReadOnlyList<TValue>)items.ToList();
                    var freezedList = new FreezedList<TValue>(list);
                    View = freezedList.CreateView(VMFactory);
                }
        //    if (ViewComparer == null)
        //    {

        //        View = items.CreateSortedView<TValueVM, string, TValueVM>(item => GetKey?.Invoke(item) ?? KeyProviderService.TryGetKey(item).key ?? ThrowNoKey<string>(item), item => GetDisplayValue(item), ItemComparer ?? DefaultItemComparer);
        //    }
        //    else
        //    {
        //        View = items.CreateSortedView<TValueVM, string, TValueVM>(item => GetKey?.Invoke(item) ?? KeyProviderService.TryGetKey(item).key ?? ThrowNoKey<string>(item), item => GetDisplayValue(item), ViewComparer);
        //    }
#endif
    }

    #endregion

    #region IActivatableViewModel

    public ViewModelActivator Activator => activator.Value;
    Lazy<ViewModelActivator> activator = new();

    #endregion

    #region Key Resolution

    #region Key for TValueVM

    public Func<TValueVM, TKey?>? GetKey { get; set; }

    public Func<TValueVM, TKey?>? DefaultGetKey => item => AmbientKeyProviderX.GetKey<TKey, TValueVM>(item);

    #endregion

    #region Key for TValue

    public Func<TValue, TKey?>? GetKeyForItem { get; set; }
    public Func<TValue, TKey?>? DefaultGetKeyForItem => item => AmbientKeyProviderX.GetKey<TKey, TValue>(item);

    #endregion

    #endregion

    #region ShowDetails

    ShowDetailsVM<TKey, TValue, TValueVM>? ShowDetailsVM { get; set; } // TODO REVIEW

    #endregion

}
#endif
public static class DisplayNameUtilsX {
    public static string DisplayNameForType(Type t) => t.Name.Replace("Item", "").TrimLeadingIOnInterfaceType();
}
public class ShowDetailsVM<TKey, TValue, TValueVM>
    where TKey : notnull
{
    public AsyncDictionaryVM<TKey, TValue, TValueVM> Parent { get; }

    #region Parameters

    public bool ExpandMultipleDetail { get; set; }

    #endregion

    public ShowDetailsVM(AsyncDictionaryVM<TKey, TValue, TValueVM> parent)
    {
        Parent = parent;
    }
    /// <summary>
    /// A collection of Keys to show detail for
    /// </summary>
    public HashSet<TKey> ShowDetailsFor { get; } = new();

    public bool ShouldShow(TKey key) => ShowDetailsFor.Contains(key);

    public void ToggleShowDetail(TKey key)
    {
        if (!ShowDetailsFor.Remove(key))
        {
            if (!ExpandMultipleDetail) { ShowDetailsFor.Clear(); }
            ShowDetailsFor.Add(key);
        }
    }

    //public void ToggleShowDetail(TValue item)
    //{
    //    ToggleShowDetail(Parent.GetKeyForItem(item));
    //}

}


#if OLD

if (VMFactory == null)
        {
            VMFactory = typeof(TValue) == typeof(TValueVM) 
                ? model => (TValueVM?)(object?)model // No ViewModel.  Use the model as the viewmodel.
                : model => ViewModelProvider.TryActivate<TValueVM, TValue>(model, ViewModel?.InitializerForChildViewModels, VMConstructorParameters(model));
        }

#endif
