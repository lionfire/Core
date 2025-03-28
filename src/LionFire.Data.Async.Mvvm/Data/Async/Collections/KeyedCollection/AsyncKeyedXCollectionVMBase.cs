﻿using LionFire.Data.Async.Sets;
using LionFire.Reflection;

namespace LionFire.Data.Mvvm;

public abstract partial class AsyncKeyedXCollectionVMBase<TKey, TValue, TValueVM>
    : LazilyGetsKeyedCollectionVM<TKey, TValue, TValueVM>
    , ICreatesAsyncVM<TValue>
    //, IActivatableViewModel
    where TKey : notnull
    where TValue : notnull
    where TValueVM : notnull//, IKeyed<TKey>
    //, IViewModel<TValue> // suggested only
{
    #region Lifecycle

    public AsyncKeyedXCollectionVMBase(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
        // TODO: map CanCreate, ReadOnly into this?
        Create = ReactiveCommand.Create<ActivationParameters, Task<TValue>>(
            execute: async p =>
            {
                var newValue = await (EffectiveCreator
                   ?? throw new ArgumentException($"{nameof(Source)} must be ICreatesAsync<TValue>"))
                    .Create(p.Type, p.Parameters);
                OnCreated(newValue);
                return newValue;
            },
            canExecute: Observable.Create<bool>(o =>
            {
                o.OnNext(EffectiveCreator is ICreatesAsync<TValue>);
                return Disposable.Empty;
            })
        );

        Create.CanExecute.ToProperty(this, nameof(CanCreate));
    }

    #endregion

    public virtual ICreatesAsync<TValue>? EffectiveCreator => Creator;
    public ICreatesAsync<TValue>? Creator { get; set; }

    #region ReadOnly

    public bool ReadOnly { get; set; }

    void ValidateCanModify()
    {
        if (ReadOnly) { throw new InvalidOperationException($"ReadOnly is true"); }
        //if (ObservableCache.IsReadOnly) { throw new InvalidOperationException($"ObservableCache.ReadOnly is true"); }
    }

    #endregion

    #region User Input

    #region Create

    public IObservable<(string key, Type type, object[] parameters, Task result)> CreatesForKey => throw new NotImplementedException();
    public ReactiveCommand<ActivationParameters, Task<TValue>> Create { get; }

    [ReactiveUI.SourceGenerators.Reactive]
    private bool _canCreate;

    public IEnumerable<Type> CreatableTypes { get; set; }

    protected virtual void OnCreated(TValue value) { }

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
    //        else if (ObservableCache is IObservableCreatesAsync<TValue> cc)
    //        {
    //            await cc.Create(type);
    //            return;
    //        }
    //        //else if (AddNew != null)
    //        //{
    //        //    await AddNew(type);
    //        //    return;
    //        //}
    //        //else if (ObservableCache is IAddsAsync addsNew)
    //        //{
    //        //    await addsNew.AddNew(type);
    //        //    return;
    //        //}
    //        else if (ObservableCache is IAddsAsync<TValue> adds)
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
    //        //await (ResolvesVM?.Get() ?? Task.CompletedTask);
    //        //StateHasChanged();
    //    }

    //    async Task<TValue> instantiate(Type type)
    //    {
    //        object[] args = Array.Empty<object>();
    //        if (ItemConstructorParameters != null) { args = ItemConstructorParameters.Invoke(type); }

    //        if (Create != null) { return await Create(type, args); }
    //        else if (ObservableCache is IObservableCreatesAsync<TValue> cc && cc.CanCreate)
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
}

#if UNUSED
//public class AsyncDictionaryVM<TKey, TValue, TValueVM> : AsyncCollectionVM<TKey, TValue>
//    //, IObservableCreatesAsync<TKey, TValue>
//    where TKey : notnull
//{

//    //#region IObservableCreatesAsync<TKey, TValue>

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

public abstract class AsyncDictionaryVM<TKey, TValue> : AsyncKeyedCollectionVM<TKey, TValue, object>
    where TKey : notnull
{
    #region Lifecycle

    public AsyncDictionaryVM(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
    }

    #endregion
}
#endif

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

#if OLD

if (VMFactory == null)
        {
            VMFactory = typeof(TValue) == typeof(TValueVM) 
                ? model => (TValueVM?)(object?)model // No ViewModel.  Use the model as the viewmodel.
                : model => ViewModelProvider.TryActivate<TValueVM, TValue>(model, ViewModel?.InitializerForChildViewModels, VMConstructorParameters(model));
        }

#endif
