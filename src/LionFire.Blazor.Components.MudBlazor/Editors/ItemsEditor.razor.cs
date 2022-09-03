#nullable enable

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
//using System.Net.Http;
//using Microsoft.AspNetCore.Components.Web;
//using Microsoft.AspNetCore.Components.Routing;
//using Microsoft.JSInterop;
//using Microsoft.Extensions.Logging;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.AspNetCore.Components.Forms;
//using Microsoft.AspNetCore.Components.Web.Virtualization;
//using Orleans;
//using MudBlazor;
//using MudBlazor.Dialog;
//using LionFire.Metaverse.Matchmaking;
//using Microsoft.Extensions.DependencyInjection;
//using static LionFire.Reflection.GetMethodEx;
using LionFire.Structures;
using ObservableCollections;
using LionFire.Structures.Keys;


namespace LionFire.Blazor.Components;

public partial class ItemsEditor<TItem, TView> : ComponentBase
{
    #region Parameters

    [Parameter]
    public bool ShowRefresh { get; set; }

    [Parameter]
    public ObservableList<TItem>? Items
    {
        get => items;
        set
        {
            if (Object.ReferenceEquals(items, value)) return;

            items = value;

            if (items == null)
            {
                View = null;
            }
            else
            {
                if (ViewComparer == null)
                {
                    
                    View = items.CreateSortedView<TItem, string, TView>(item => GetKey?.Invoke(item) ?? KeyProviderService.TryGetKey(item).key ?? ThrowNoKey<string>(item), item => GetDisplayValue(item), ItemComparer ?? DefaultItemComparer);
                }
                else
                {
                    View = items.CreateSortedView<TItem, string, TView>(item => GetKey?.Invoke(item) ?? KeyProviderService.TryGetKey(item).key ?? ThrowNoKey<string>(item), item => GetDisplayValue(item), ViewComparer);
                }
            }
        }
    }
    private ObservableList<TItem>? items;

    public T ThrowNoKey<T>(object item) => throw new ArgumentException("Failed to resolve Key for object of type " + item?.GetType()?.FullName);

    [Parameter]
    public Task<IEnumerable<Type>>? CreateableTypes { get; set; }

    [Parameter]
    public Func<Type, Task>? Create { get; set; }

    [Parameter]
    public Func<Task<IEnumerable<TItem>>>? RetrieveAction { get; set; }

    [Parameter]
    public RenderFragment<ISynchronizedView<TItem, TView>>? ChildContent { get; set; }

    [Parameter]
    public Func<TItem, TView> GetDisplayValue { get; set; } = DefaultGetDisplayValue;

    public static Func<TItem, TView> DefaultGetDisplayValue { get; set; } = item => (TView)Activator.CreateInstance(typeof(TView), item);

    [Parameter]
    public Func<TItem, string?>? GetKey { get; set; }

    public Func<TItem, string?>? DefaultGetKey => item =>
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
    public IComparer<TItem>? ItemComparer { get; set; }

    /// <summary>
    /// Default: compare based on IKeyed.Key.
    /// </summary>
    IComparer<TItem> DefaultItemComparer => defaultItemComparer ??= new KeyComparer<string, TItem>(KeyProviderService);
    IComparer<TItem>? defaultItemComparer;


    /// <summary>
    /// Optional. Overrides ItemComparer.
    /// </summary>
    [Parameter]
    public IComparer<TView>? ViewComparer { get; set; }

    [Parameter]
    public bool ExpandMultipleDetail { get; set; }

    #endregion

    #region State

    public bool IsRefreshing { get; set; }

    public HashSet<string> ShowDetailsFor { get; } = new();

    ISynchronizedView<TItem, TView>? View { get; set; }

    #endregion

    #region Initialization

    public ItemsEditor()
    {
        GetKey = DefaultGetKey;
        GlobalItemsChanged += ItemsEditor_GlobalItemsChanged;
    }

    private void ItemsEditor_GlobalItemsChanged(ItemsEditor<TItem,TView> obj, string key)
    {
        if(Object.ReferenceEquals(obj, this) || key != EffectiveKey) { return; }
        InvokeAsync(Retrieve).FireAndForget();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Items == null && RetrieveAction != null)
        {
            await Retrieve();
        }

        await base.OnParametersSetAsync();
    }

    #endregion

    public string Key { get; set; }
    public string EffectiveKey => Key ?? NavigationManager.Uri;

    public async Task Retrieve()
    {
        var refreshAction = RetrieveAction;
        if (refreshAction != null && !IsRefreshing)
        {
            try
            {
                IsRefreshing = true;
                StateHasChanged();
                var newItems = await refreshAction.Invoke();

                if (Items == null)
                {
                    Items = new ObservableList<TItem>(newItems);
                }
                else
                {
                    var additions = new List<TItem>();
                    var removals = new List<TItem>();
                    foreach (var item in newItems)
                    {
                        if (!Items.Contains(item)) additions.Add(item);

                    }
                    foreach (var item in Items)
                    {
                        if (!newItems.Contains(item))
                        {
                            removals.Add(item);
                        }
                    }
                    foreach (var item in removals) { Items.Remove(item); }
                    foreach (var item in additions) { Items.Add(item); }

                    if(additions.Count > 0 || removals.Count > 0)
                    {
                        GlobalItemsChanged(this, EffectiveKey);
                    }
                }

            }
            finally
            {
                IsRefreshing = false;
                StateHasChanged();
            }
        }
    }

    public static event Action<ItemsEditor<TItem, TView>, string> GlobalItemsChanged;

    public void ToggleShowDetail(string id)
    {
        if (!ShowDetailsFor.Remove(id))
        {
            if (!ExpandMultipleDetail) { ShowDetailsFor.Clear(); }
            ShowDetailsFor.Add(id);
        }
    }
    public void RaiseStateHasChanged() => StateHasChanged();


}