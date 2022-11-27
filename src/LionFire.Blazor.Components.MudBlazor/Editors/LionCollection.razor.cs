#nullable enable

using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using ObservableCollections;
using LionFire.Structures.Keys;
using static LionFire.Reflection.GetMethodEx;
using System.Collections.ObjectModel;
using LionFire.Mvvm;
using Microsoft.Extensions.Logging;
using LionFire.Collections;
using System.Xml.Linq;
using LionFire.Reflection;
using MudBlazor;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Specialized;
using System.ComponentModel;
using LionFire.Reactive;
using System.Reactive.Subjects;
using LionFire.ExtensionMethods.Copying;

namespace LionFire.Blazor.Components;

/// <summary>
/// TODO:
/// 
///  - optional reorder support
///  - 
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class LionCollection<TItem, TItemVM> : ComponentBase, IDisposable,
    //IAsyncObserver<Collections.NotifyCollectionChangedEventArgs<TItem>>
    IObserver<Collections.NotifyCollectionChangedEventArgs<string>>
{

    private static readonly object[] EmptyArray = new object[] { };

    public string Key { get; set; }
    public string EffectiveKey => Key ?? NavigationManager.Uri;


    #region Parameters

    [Parameter]
    public Func<TItem, TItemVM>? VMFactory { get; set; } = null;

    [Parameter]
    public Func<TItem, object>? ModelSelector { get; set; }

    /// <summary>
    /// Recommended: ICollectionVM, or else set RefreshAction AddAction RemoveAction
    /// </summary>
    [Parameter]
    public IReadOnlyCollection<TItem> Items
    {
        get
        {
            return items ??= new FreezedList<TItem>(Enumerable.Empty<TItem>().ToList());
        }
        set
        {
            if (Object.ReferenceEquals(items, value)) return;
            items = value;
        }
    }
    private IReadOnlyCollection<TItem>? items;
    private IReadOnlyCollection<TItem> itemsForView;

    [Parameter]
    public bool ShowRefresh { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public bool AutoRetrieveOnInit { get; set; } = true;


    [Parameter]
    public IEnumerable<Type>? CreateableTypes { get; set; }

    [Parameter]
    public Func<Type /*type*/, object[]? /*parameters*/, Task<TItem> /*newObject*/>? Create { get; set; }

    [Parameter]
    public Func<TItem, Task>? Add { get; set; }

    [Parameter]
    public Func<Type, Task<TItem>>? AddNew { get; set; }

    //[Parameter]
    //public object[] CreateParameters { get; set; } = new object[] { };

    [Parameter]
    public Func<Type, object[]>? ItemConstructorParameters { get; set; } = null;

    /// <summary>
    /// Only used if VMFactory is not set
    /// </summary>
    [Parameter]
    public Func<TItem, object[]> VMConstructorParameters { get; set; } = _ => EmptyArray;

    [Parameter]
    public Func<Task<IEnumerable<TItem>>>? RetrieveAction { get; set; }

    [Parameter]
    public RenderFragment<ISynchronizedView<TItem, TItemVM>>? ChildContent { get; set; }

    [Parameter]
    public Func<TItem, TItemVM> GetDisplayValue { get; set; } = DefaultGetDisplayValue;

    public static Func<TItem, TItemVM> DefaultGetDisplayValue { get; set; } = item => (TItemVM)Activator.CreateInstance(typeof(TItemVM), item);

    [Parameter]
    public Func<TItemVM, string?>? GetKey { get; set; }
    public Func<TItem, string?>? GetKeyForItem { get; set; }

    public Func<TItemVM, string?>? DefaultGetKey => item =>
    {
        var keyed = (item as IKeyed);
        if (keyed != null) { return keyed.Key; }

        var result = KeyProviderService.TryGetKey(item);
        if (result.success) { return result.key; }

        Debug.WriteLine("[ItemsEditor] No key provider for object of type " + item?.GetType().FullName);
        return item!.GetHashCode().ToString();
    };
    public Func<TItem, string?>? DefaultGetKeyForItem => item =>
    {
        var keyed = (item as IKeyed);
        if (keyed != null) { return keyed.Key; }

        var result = KeyProviderService.TryGetKey(item);
        if (result.success) { return result.key; }

        Debug.WriteLine("[ItemsEditor] No key provider for object of type " + item?.GetType().FullName);
        return item!.GetHashCode().ToString();
    };

    /// <summary>
    /// Optional.  If none set, will compare using DefaultItemComparer
    /// </summary>
    [Parameter]
    public IComparer<TItemVM>? ItemComparer { get; set; }

    /// <summary>
    /// Default: compare based on IKeyed.Key.
    /// </summary>
    IComparer<TItemVM> DefaultItemComparer => defaultItemComparer ??= new KeyComparer<string, TItemVM>(KeyProviderService);
    IComparer<TItemVM>? defaultItemComparer;

    /// <summary>
    /// Optional. Overrides ItemComparer.
    /// </summary>
    [Parameter]
    public IComparer<TItemVM>? ViewComparer { get; set; }

    [Parameter]
    public bool ExpandMultipleDetail { get; set; }

    public IAsyncCollectionCache<TItem>? AsyncCollectionCache { get => asyncCollectionCache; set => asyncCollectionCache = value; }
    private IAsyncCollectionCache<TItem>? asyncCollectionCache;

    #endregion

    #region State

    public bool IsRefreshing { get; set; }

    /// <summary>
    /// A collection of Keys to show detail for
    /// </summary>
    public HashSet<string> ShowDetailsFor { get; } = new();

    public bool ShouldShow(TItem item) => ShowDetailsFor.Contains(GetKeyForItem(item));

    ISynchronizedView<TItem, TItemVM>? View
    {
        get => view;
        set
        {
            if (view != null)
            {
                view.CollectionStateChanged -= View_CollectionStateChanged;
                view.Dispose();
            }
            view = value;
            if (view != null)
            {
                view.CollectionStateChanged += View_CollectionStateChanged;
            }
        }
    }
    private ISynchronizedView<TItem, TItemVM>? view;

    #endregion

    public T ThrowNoKey<T>(object item) => throw new ArgumentException("Failed to resolve Key for object of type " + item?.GetType()?.FullName);

    //private void ItemsEditor_GlobalItemsChanged(LionCollection<TItem, TItemVM> obj, string key)
    //{
    //    if (Object.ReferenceEquals(obj, this) || key != EffectiveKey) { return; }
    //    InvokeAsync(Retrieve).FireAndForget();
    //}

    #region Lifecycle

    public LionCollection()
    {
        GetKey = DefaultGetKey;
        GetKeyForItem = DefaultGetKeyForItem;
        //GlobalItemsChanged += ItemsEditor_GlobalItemsChanged;
    }


    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        SetViewFromItems();

        if (AsyncCollectionCache != null && !AsyncCollectionCache.HasRetrieved && AutoRetrieveOnInit && AsyncCollectionCache.CanRetrieve)
        {
            await Retrieve();
        }
        await SubscribeAsync(); // TEMP
    }


    void SetViewFromItems()
    {
        //CollectionCache ??= new ObservableListCache<TItem>();

        if (ChildContent == null) { return; }

        if (items == null) { view = null; }
        else
        {
            if (!ReferenceEquals(itemsForView, items))
            {
                if (VMFactory == null)
                {
                    VMFactory = model => ViewModelProvider.Activate<TItemVM, TItem>(model, VMConstructorParameters(model));
                }

                if (items is IAsyncCollectionCache<TItem> vm)
                {
                    AsyncCollectionCache = vm;
                    View = AsyncCollectionCache.Collection.CreateView(VMFactory);
                }
                else if (items is IObservableCollection<TItem> observableCollection)
                {
                    View = observableCollection.CreateView(VMFactory);
                }
                else
                {
                    // It is often the case that Items is not Observable.
                    // In that case, FreezedList is provided to create a View with the same API for normal collections.
                    var list = items as IReadOnlyList<TItem> ?? (IReadOnlyList<TItem>)items.ToList();
                    var freezedList = new FreezedList<TItem>(list);
                    View = freezedList.CreateView(VMFactory);
                }
                itemsForView = items;
            }
        }

        //if (items == null)
        //{
        //    View = null;
        //}
        //else
        //{
        //    if (ViewComparer == null)
        //    {

        //        View = items.CreateSortedView<TItemVM, string, TItemVM>(item => GetKey?.Invoke(item) ?? KeyProviderService.TryGetKey(item).key ?? ThrowNoKey<string>(item), item => GetDisplayValue(item), ItemComparer ?? DefaultItemComparer);
        //    }
        //    else
        //    {
        //        View = items.CreateSortedView<TItemVM, string, TItemVM>(item => GetKey?.Invoke(item) ?? KeyProviderService.TryGetKey(item).key ?? ThrowNoKey<string>(item), item => GetDisplayValue(item), ViewComparer);
        //    }
        //}
    }

    public void Dispose()
    {
        View = null;
        UnsubscribeAsync();
    }

    #endregion


    #region Retrieve

    public bool CanRetrieve => AsyncCollectionCache != null;
    public Task Retrieve()
    {
        if (AsyncCollectionCache == null) throw new ArgumentNullException($"{nameof(Items)} is not of type {typeof(IAsyncCollectionCache<TItem>).FullName}, so {nameof(Retrieve)} is not possible.");
        return AsyncCollectionCache.Retrieve();
    }

    #endregion

    //public static event Action<LionCollection<TItem, TItemVM>, string> GlobalItemsChanged;

    public void ToggleShowDetail(TItem item)
    {
        ToggleShowDetail(GetKeyForItem(item));
    }

    public void ToggleShowDetail(string key)
    {
        if (!ShowDetailsFor.Remove(key))
        {
            if (!ExpandMultipleDetail) { ShowDetailsFor.Clear(); }
            ShowDetailsFor.Add(key);
        }
    }
    public void RaiseStateHasChanged() => StateHasChanged();

    void ValidateCanModify()
    {
        if (ReadOnly) { throw new InvalidOperationException($"ReadOnly is true"); }
        if (AsyncCollectionCache != null && AsyncCollectionCache.IsReadOnly) { throw new InvalidOperationException($"AsyncCollectionCache.ReadOnly is true"); }
    }

    public string DisplayNameForType(Type t) => t.Name.Replace("Item", "").TrimLeadingIOnInterfaceType();

    #region Event Handling

    private void View_CollectionStateChanged(NotifyCollectionChangedAction action)
    {
        InvokeAsync(StateHasChanged);
    }

    public async void OnCreate(Type type)
    {
        ValidateCanModify();

        try
        {
            if (Create != null)
            {
                await Create(type, null);
                return;
            }
            else if (AsyncCollectionCache is IAsyncCanCreate<TItem> cc && cc.CanCreate)
            {
                await cc.Create(type);
                return;
            }
            else if (AddNew != null)
            {
                await AddNew(type);
                return;
            }
            else if (AsyncCollectionCache != null && AsyncCollectionCache.CanAddNew)
            {
                await AsyncCollectionCache.AddNew(type);
                return;
            }
            else if (Add != null)
            {
                await Add(await create(type));
                return;
            }
            else if (AsyncCollectionCache != null && AsyncCollectionCache.CanAdd)
            {
                await AsyncCollectionCache.Add(await create(type));
                return;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        finally
        {
            await Retrieve();
            //StateHasChanged();
        }

        async Task<TItem> create(Type type)
        {
            object[] args = Array.Empty<object>();
            if (ItemConstructorParameters != null) { args = ItemConstructorParameters.Invoke(type); }

            if (Create != null) { return await Create(type, args); }
            else if (AsyncCollectionCache is IAsyncCanCreate<TItem> cc && cc.CanCreate)
            {
                return await cc.Create(type, args);
            }

            return (TItem)ActivatorUtilities.CreateInstance(ServiceProvider, type, args)
                ?? (TItem?)Activator.CreateInstance(type, args)
                ?? throw new Exception($"Failed to create item of type {type.FullName}");
        }
    }

    void RowClicked(DataGridRowClickEventArgs<(TItem, TItemVM)> args)
    {
        //_events.Insert(0, $"Event = RowClick, Index = {args.RowIndex}, Data = {System.Text.Json.JsonSerializer.Serialize(args.Item)}");
    }

    #endregion


    #region Subscription to Events

    //Items

    public bool CanSubscribe => Items is IAsyncObservable<Collections.NotifyCollectionChangedEventArgs<TItem>>;
    public bool IsSubscribed => sub != null;

    private IAsyncObservableForSyncObservers<Collections.NotifyCollectionChangedEventArgs<string>>? Subscribable => Items as IAsyncObservableForSyncObservers<Collections.NotifyCollectionChangedEventArgs<string>>;

    private bool IsSubscribing;

    Guid G = Guid.NewGuid();

    public async Task SubscribeAsync()
    {
        var subscribable = Subscribable;
        if (subscribable == null)
        {
            return;
            //throw new NotSupportedException(); 
        }
        if (!IsSubscribing)
        {
            try
            {
                IsSubscribing = true;
                if (sub != null) return;
                Debug.WriteLine($"SubscribeAsync {G}");
                sub = await subscribable.SubscribeAsync(this).ConfigureAwait(false);
            }
            finally
            {
                IsSubscribing = false;
            }
        }
    }

    public ValueTask UnsubscribeAsync()
    {
        var subCopy = sub;
        if (subCopy != null)
        {
            sub = null;
            return subCopy.DisposeAsync();
        }
        else { return ValueTask.CompletedTask; }
    }

    IAsyncDisposable? sub
    {
        get => _sub;
        set
        {
            if (ReferenceEquals(_sub, value)) return;
            if (_sub != null && value != null) { throw new AlreadySetException(); }
            _sub = value;
        }
    }
    IAsyncDisposable? _sub;

    #region IAsyncObserver

    //public Task OnNextAsync(Collections.NotifyCollectionChangedEventArgs<TItem> item)
    //{
    //    throw new NotImplementedException();
    //}

    //public Task OnCompletedAsync()
    //{
    //    throw new NotImplementedException();
    //}

    //public Task OnErrorAsync(Exception ex)
    //{
    //    throw new NotImplementedException();
    //}

    #endregion

    #region IObserver<>

    public void OnCompleted()
    {
        // TODO
    }

    public void OnError(Exception error)
    {
        // TODO
    }

    public void OnNext(Collections.NotifyCollectionChangedEventArgs<string> value)
    {
        Debug.WriteLine($"OnNext {G}");
        Task.Run(async () =>
        {
            await Retrieve();
            await InvokeAsync(StateHasChanged);
        });
    }

    #endregion

    #endregion
}

