#nullable enable

using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using ObservableCollections;
using LionFire.Structures.Keys;
using LionFire.Reflection;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Reactive;
using LionFire.Resolves;
using DynamicData;
using Microsoft.Extensions.Logging;
using static MudBlazor.CategoryTypes;
using System.Reactive;

namespace LionFire.Blazor.Components;

// TODO: Make this class as dumb as possible, moving testable logic to DictionaryVM

/// <summary>
/// TODO:
/// 
///  - optional ViewModel type: ctor injection, or IViewModelProvider
///  - optional reorder support
///  RENAME: AsyncDictionaryView
/// </summary>
/// <typeparam name="TValue"></typeparam>
public partial class DictionaryView<TKey, TValue, TValueVM> : IAsyncDisposable
    where TKey : notnull
{
    #region Parameters

    // Recommended:
    //  - IObservableCache<TValue, TItem>
    //     - see AsyncObservableCache<TItem, TValue> and
    //     - AsyncComposableObservableCache<TItem, TValue>)
    [Parameter]
    public IEnumerable<KeyValuePair<TKey, TValue>>? Items { get; set; }
    public IEnumerable<TValue>? Values => Items?.Select(x => x.Value);

    #region TODO: Pass-thru to VM

    //[Parameter]
    //public bool ShowRefresh { get; set; }

    //[Parameter]
    //public bool ReadOnly { get; set; }

    #endregion

    // TODO
    //[Parameter]
    //public bool AutoRetrieveOnInit { get; set; } = true; // TODO

    [Parameter]
    public IEnumerable<Type>? CreateableTypes { get; set; }

    #region Collection Parameters

    //[Parameter]
    //public object[] CreateParameters { get; set; } = new object[] { };

    //[Parameter]
    //public Func<Type, object[]>? ItemConstructorParameters { get; set; } = null;

    #endregion

    #region OLD - collection implementation - move to a composable AsyncCollection type

    //[Parameter]
    //public Func<Type /*type*/, object[]? /*parameters*/, Task<TValue> /*newObject*/>? Create { get; set; }
    //[Parameter]
    //public Func<TValue, Task>? Add { get; set; }

    //[Parameter]
    //public Func<Type, Task<TValue>>? AddNew { get; set; }
    //[Parameter]
    //public Func<Task<IEnumerable<TValue>>>? RetrieveAction { get; set; }

    #endregion

    [Parameter]
    public RenderFragment<AsyncDictionaryVM<TKey, TValue, TValueVM>>? ChildContent { get; set; }

    //[Parameter]
    //public Func<TValue, TValueVM?> GetDisplayValue { get; set; } = DefaultGetDisplayValue;

    //public static Func<TValue, TValueVM?> DefaultGetDisplayValue { get; set; } = item => (TValueVM?)Activator.CreateInstance(typeof(TValueVM), item);

    ///// <summary>
    ///// Optional.  If none set, will compare using DefaultItemComparer
    ///// </summary>
    //[Parameter]
    //public IComparer<TValueVM>? ItemComparer { get; set; }

    ///// <summary>
    ///// Default: compare based on IKeyed.Key.
    ///// </summary>
    //IComparer<TValueVM> DefaultItemComparer => defaultItemComparer ??= new KeyComparer<string, TValueVM>(KeyProviderService<TItem>);
    //IComparer<TValueVM>? defaultItemComparer;

    ///// <summary>
    ///// Optional. Overrides ItemComparer.
    ///// </summary>
    //[Parameter]
    //public IComparer<TValueVM>? ViewComparer { get; set; }

    #endregion

    #region Lifecycle

    public DictionaryView()
    {
        this.WhenActivated(disposableRegistration =>
        {
            //#if true // TODO: How to bind [Parameter] to ViewModel?  Set in OnParametersSetAsync?
            //            this.WhenAnyValue(v => v.Items)
            //                .BindTo(ViewModel, vm => vm.Value)
            //                .DisposeWith(disposableRegistration);
            //#else
            //            this.Bind(ViewModel,
            //                    viewModel => viewModel.Value,
            //                    view => view.Items)
            //                .DisposeWith(disposableRegistration);
            //#endif

            //< TValueVM, TValue, IEnu ,>
            this.Bind(ViewModel,
                    viewModel => viewModel.Source.Value,
                    view => view.Children
                    //,
                    //vmToViewConverter: vm => vm?.Select(kvp => kvp.Value),
                    //viewToVmConverter: v => null
                    )
                .DisposeWith(disposableRegistration);

            // sample
            //this.BindCommand(ViewModel,
            //    viewModel => viewModel.OpenPage,
            //    view => view.OpenButton)
            //.DisposeWith(disposableRegistration);


            //this.OneWayBind(ViewModel,
            //        viewModel => viewModel.Test,
            //        view => view.Test)
            //    .DisposeWith(disposableRegistration);
        });

        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        Logger.LogInformation($"DictionaryView.ViewModel.PropertyChanged: {e.PropertyName}");
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (ViewModel != null) { ViewModel.Source = Items == null ? null : new Preresolved<IEnumerable<KeyValuePair<TKey,TValue>>>(Items); }       
    }

    //protected override Task OnInitializedAsync()
    //{
    //    return base.OnInitializedAsync();
    //}

    public async ValueTask DisposeAsync()
    {
        await UnsubscribeAsync();
        var vm = ViewModel;
        if (vm != null)
        {
            vm = null;
            if (vm is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                (vm as IDisposable)?.Dispose();
            }
        }
    }

    #endregion

    #region State

    #region Bound to ViewModel

    public IEnumerable<TValueVM>? Children { get; set; }

    #endregion

    #endregion     

    public T ThrowNoKey<T>(object item) => throw new ArgumentException("Failed to resolve Key for object of type " + item?.GetType()?.FullName);

    //private void ItemsEditor_GlobalItemsChanged(DictionaryView<TValue, TValueVM> obj, string key)
    //{
    //    if (Object.ReferenceEquals(obj, this) || key != EffectiveKey) { return; }
    //    InvokeAsync(Retrieve).FireAndForget();
    //}

    #region Retrieve


    //public bool CanResolve => AsyncCollectionCache != null;

    //public async Task<IResolveResult<IEnumerable<TValue>>> ResolveIfNeeded()
    //{
    //    if (Items is ILazilyResolves<IEnumerable<T>> lazily)
    //    {
    //        var result = await lazily.TryGetValue().ConfigureAwait(false);
    //        return (IResolveResult<IEnumerable<TValue>>)result;
    //    }

    //    if (Items is IResolves<IEnumerable<T>> resolves)
    //    {
    //        var result = await resolves.Resolve().ConfigureAwait(false);
    //        return result;
    //    }
    //    throw new NotSupportedException();

    //}
    //public async Task<IResolveResult<TValue>> Resolve()
    //{
    //    if (Items is IResolves<IEnumerable<T>> resolves)
    //    {
    //        var result = await resolves.Resolve().ConfigureAwait(false);
    //        return result;
    //    }
    //    throw new NotSupportedException();
    //}

    #endregion

    #region Event Handling

    void RowClicked(DataGridRowClickEventArgs<(TValue, TValueVM)> args)
    {
        //_events.Insert(0, $"Event = RowClick, Index = {args.RowIndex}, Data = {System.Text.Json.JsonSerializer.Serialize(args.Item)}");
    }
    void RowClicked(DataGridRowClickEventArgs<TValue> args)
    {
        //_events.Insert(0, $"Event = RowClick, Index = {args.RowIndex}, Data = {System.Text.Json.JsonSerializer.Serialize(args.Item)}");
    }

    #endregion

}

//public partial class ListView<TItem, TValue, TValueVM> : IDisposable
//    where TItem : notnull
//{
//    [Parameter]
//    public IEnumerable<TValue>? Items { get; set; }
//}
