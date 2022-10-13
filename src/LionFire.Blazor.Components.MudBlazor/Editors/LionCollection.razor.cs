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

namespace LionFire.Blazor.Components;

/// <summary>
/// TODO:
/// 
///  - optional reorder support
///  - 
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class LionCollection<TItem, TItemVM> : ComponentBase, IDisposable
{
    private static readonly object[] EmptyArray = new object[] { };

    public string Key { get; set; }
    public string EffectiveKey => Key ?? NavigationManager.Uri;


    #region Parameters

    [Parameter]
    public Func<TItem, TItemVM>? VMFactory { get; set; } = null;

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

    public Func<TItemVM, string?>? DefaultGetKey => item =>
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

    public HashSet<string> ShowDetailsFor { get; } = new();

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

    public void ToggleShowDetail(string id)
    {
        if (!ShowDetailsFor.Remove(id))
        {
            if (!ExpandMultipleDetail) { ShowDetailsFor.Clear(); }
            ShowDetailsFor.Add(id);
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

        if(Create != null)
        {
            await Create(type, null);
            return;
        }
        else if (AsyncCollectionCache != null && AsyncCollectionCache.CanCreate)
        {
            await AsyncCollectionCache.Create(type);
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
        
        async Task<TItem> create(Type type)
        {
            object[] args = Array.Empty<object>();
            if (ItemConstructorParameters != null) { args = ItemConstructorParameters.Invoke(type); }

            if (Create != null) { return await Create(type, args); }
            else if (AsyncCollectionCache != null && AsyncCollectionCache.CanCreate) {
                return await AsyncCollectionCache.Create(type, args);
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
}