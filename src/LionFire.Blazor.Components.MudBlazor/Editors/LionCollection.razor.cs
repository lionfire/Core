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
    public string Key { get; set; }
    public string EffectiveKey => Key ?? NavigationManager.Uri;


    #region Parameters

    [Parameter]
    public Func<TItem, TItemVM> VMFactory { get; set; } = default;

    /// <summary>
    /// Recommended: ICollectionVM, or else set RefreshAction AddAction RemoveAction
    /// </summary>
    [Parameter]
    public IReadOnlyCollection<TItemVM> Items
    {
        get
        {
            return items ??= new FreezedList<TItemVM>(Enumerable.Empty<TItemVM>().ToList());
        }
        set
        {
            if (Object.ReferenceEquals(items, value)) return;
            items = value;
        }
    }
    private IReadOnlyCollection<TItemVM>? items;

    [Parameter]
    public bool ShowRefresh { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public bool AutoRetrieveOnInit { get; set; } = true;

        

    [Parameter]
    public IEnumerable<Type>? CreateableTypes { get; set; }

    [Parameter]
    public Func<Type, Task>? Create { get; set; }

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

    [Parameter]
    public ICollectionVM<TItem>? CollectionVM { get => collectionVM; set => collectionVM = value; }
    private ICollectionVM<TItem>? collectionVM { get; set; }

    #endregion

    #region State

    public bool IsRefreshing { get; set; }

    public HashSet<string> ShowDetailsFor { get; } = new();

    ISynchronizedView<TItem, TItemVM>? View { get; set; }

    #endregion

    ISynchronizedView<TItem, TItemVM>? view = default!;

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

        InitCollection();

        if (!CollectionVM.HasRetrieved && AutoRetrieveOnInit && CollectionVM.CanRetrieve)
        {
            await Retrieve();
        }
    }
        
    private void InitCollection()
    {
        CollectionVM ??= new ObservableListVM<TItem>();

        if (ChildContent == null) { return; }


        //view = observableCollection.CreateView(VMFactory);
        if (items == null) { view = null; }
        else
        {
            if (items is ICollectionVM<TItem> vm)
            {
                CollectionVM = vm;
                view = CollectionVM.Collection.CreateView(VMFactory);
            }
            else if (items is IObservableCollection<TItem> observableCollection)
            {
                view = observableCollection.CreateView(VMFactory);
            }
            else
            {
                // It is often the case that Items is not Observable.
                // In that case, FreezedList is provided to create a View with the same API for normal collections.
                var list = items as IReadOnlyList<TItem> ?? (IReadOnlyList<TItem>)items.ToList();
                var freezedList = new FreezedList<TItem>(list);
                view = freezedList.CreateView(VMFactory);
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

        if (view != null)
        {
            view.CollectionStateChanged += async _ =>
            {
                // TODO - does this get disposed?
                await InvokeAsync(StateHasChanged);
            };
        }
    }

    #endregion

    
    public Task Retrieve()
    {
        return CollectionVM.Retrieve();
    }

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

    public Func<Type, object[]>? ItemConstructorParameters { get; set; } = null;
    async Task Add(Type type)
    {
        var itemsObservable = Items as ICollectionVM<TItemVM>;

        if (itemsObservable == null) { throw new NotSupportedException($"{nameof(Items)} does not support modification"); }
        if (itemsObservable.IsReadOnly) { throw new NotSupportedException($"{nameof(Items)} is read only"); }

        Logger.LogInformation($"Adding {typeof(TItem).Name} type {type.Name}");

        var p = ItemConstructorParameters?.Invoke(type);
        var newItem = ActivatorUtilities.CreateInstance(ServiceProvider, type, p);

        var newItemVM = GetVM(newItem);

        await itemsObservable.Add(newItemVM);
    }

    public string DisplayNameForType(Type t) => t.Name.Replace("Item", "").TrimLeadingIOnInterfaceType();

    public void Dispose()
    {
        view?.Dispose();
    }

    #region Event Handling

    void RowClicked(DataGridRowClickEventArgs<TItemVM> args)
    {
        //_events.Insert(0, $"Event = RowClick, Index = {args.RowIndex}, Data = {System.Text.Json.JsonSerializer.Serialize(args.Item)}");
    }

    #endregion
}