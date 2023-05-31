#nullable enable

using Microsoft.AspNetCore.Components;
using LionFire.Structures.Keys;
using LionFire.Reflection;
using LionFire.Reactive;
using LionFire.Resolves;
using static MudBlazor.CategoryTypes;
using DynamicData.Binding;
using LionFire.ExtensionMethods;
using LionFire.Mvvm;

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
public partial class KeyedCollectionView<TKey, TValue, TValueVM>
    : IAsyncDisposable
    , IComponentized<TKey, TValue, TValueVM>
    where TKey : notnull
{
    #region Parameters

    // Recommended:
    //  - IObservableCache<TValue, TItem>
    //     - see AsyncObservableCache<TItem, TValue> and
    //     - AsyncComposableObservableCache<TItem, TValue>)
    [Parameter]
    public IEnumerable<TValue>? Items { get; set; }
    IEnumerable<TValue>? oldItems { get; set; }
    //public IEnumerable<TValue>? Values => Items?.Select(x => x.Model);

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
    public IEnumerable<Type>? CreatableTypes { get; set; } // TODO: Move to ICreatesAsyncVM<T>

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
    public RenderFragment<AsyncKeyedCollectionVM<TKey, TValue, TValueVM>>? ChildContent { get; set; }

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

    public KeyedCollectionView()
    {
        this.WhenActivated(disposableRegistration =>
        {
            if (ViewModel == null) throw new ArgumentNullException(nameof(ViewModel));
            ViewModel.PropertyChanged += ViewModel_PropertyChanged; // MEMORYLEAK?

            

            //#if true // TODO: How to bind [Parameter] to ViewModel?  Set in OnParametersSetAsync?
            //            this.WhenAnyValue(v => v.Items)
            //                .BindTo(ViewModel, vm => vm.Model)
            //                .DisposeWith(disposableRegistration);
            //#else
            //            this.Bind(ViewModel,
            //                    viewModel => viewModel.Model,
            //                    view => view.Items)
            //                .DisposeWith(disposableRegistration);
            //#endif

            // I think this should work too?
            //this.OneWayBind(ViewModel,
            //        viewModel => viewModel.ValueVMs, // REVIEW Nullability: Source
            //        view => view.Children
            //        //,
            //        //vmToViewConverter: vm => vm?.Select(kvp => kvp.Model),
            //        //viewToVmConverter: v => null
            //        )
            //    .DisposeWith(disposableRegistration);


            //this.WhenAnyValue(v => v.ViewModel.ValueVMCollections)
            //    .ToProperty(this, nameof(ChildrenCollections))
            //    //viewModel => viewModel.ValueVMCollections,
            //    //view => view.ChildrenCollections)
            //    //.DisposeWith(disposableRegistration)
            //    ;

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
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        Logger.LogInformation($"{nameof(KeyedCollectionView<TKey, TValue, TValueVM>)}.ViewModel.PropertyChanged: {e.PropertyName}");
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        ArgumentNullException.ThrowIfNull(ViewModel);

        if (oldItems == null || !ReferenceEquals(oldItems, this.Items))
        {
            ViewModel.Source = Items == null ? null
                : Items as ILazilyResolves<IEnumerable<TValue>>
                    ?? new Preresolved<IEnumerable<TValue>>(Items);

            ViewModel.Source?.TryGetValue().AsTask().FireAndForget();


            ViewModel.ValueVMCollections.Subscribe(o =>
            {
                Debug.WriteLine($"KeyedCollectionView: VMCollectionChanged {ViewModel.ValueVMs != null}");
                o.Subscribe(_ => InvokeAsync(StateHasChanged));
                InvokeAsync(StateHasChanged);
            });

            ViewModel.ViewModelPropertyChanges.Subscribe(o =>
            {
                Debug.WriteLine($"KeyedCollectionView: ViewModelPropertyChanges {o}");
                InvokeAsync(StateHasChanged);
            });

            //this.BindCommand(ViewModel,
            //    viewModel => viewModel.Create,
            //    view => view.)
        }
        oldItems = Items;
    }


    public async ValueTask DisposeAsync()
    {
        var componentsCopy = Interlocked.Exchange(ref components, null);
        if (componentsCopy != null)
        {
            foreach (var disposableComponent in componentsCopy.OfType<IAsyncDisposable>())
            {
                await disposableComponent.DisposeAsync();
            }
            foreach (var disposableComponent in componentsCopy.OfType<IDisposable>())
            {
                disposableComponent.Dispose();
            }
        }

        var vm = ViewModel;
        if (vm != null)
        {
            vm = null;
            if (vm is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else { (vm as IDisposable)?.Dispose(); }
        }
    }

    #endregion

    public T ThrowNoKey<T>(object item) => throw new ArgumentException("Failed to resolve Key for object of type " + item?.GetType()?.FullName);

    //private void ItemsEditor_GlobalItemsChanged(KeyedCollectionView<TValue, TValueVM> obj, string key)
    //{
    //    if (Object.ReferenceEquals(obj, this) || key != EffectiveKey) { return; }
    //    InvokeAsync(Retrieve).FireAndForget();
    //}

    #region Retrieve


    //public bool CanResolve => AsyncCollectionCache != null;

    //public async Task<IResolveResult<IEnumerable<TValue>>> ResolveIfNeeded()
    //{
    //    if (Items is ILazilyResolves<IEnumerable<TValue>> lazily)
    //    {
    //        var result = await lazily.TryGetValue().ConfigureAwait(false);
    //        return (IResolveResult<IEnumerable<TValue>>)result;
    //    }

    //    if (Items is IResolves<IEnumerable<TValue>> resolves)
    //    {
    //        var result = await resolves.Resolve().ConfigureAwait(false);
    //        return result;
    //    }
    //    throw new NotSupportedException();

    //}
    //public async Task<IResolveResult<TValue>> Resolve()
    //{
    //    if (Items is IResolves<IEnumerable<TValue>> resolves)
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
    void RowClicked(DataGridRowClickEventArgs<TValueVM> args)
    {
        //_events.Insert(0, $"Event = RowClick, Index = {args.RowIndex}, Data = {System.Text.Json.JsonSerializer.Serialize(args.Item)}");
    }

    #endregion

    public T GetComponent<T>()
    {
        components ??= new();
        return (T)components.GetOrAdd(typeof(T), (type) => Activator.CreateInstance(type, ViewModel) ?? throw new Exception($"Failed to activate {typeof(T).FullName}"));
    }
    ConcurrentDictionary<Type, object>? components;

    [Reactive]
    public bool CanCreate { get; set; }
    public void OnCreate(Type t)
    {
        Console.WriteLine("Create: " + t);
        ViewModel.Create.Execute(t).Subscribe();
    }
}


//public partial class ListView<TItem, TValue, TValueVM> : IDisposable
//    where TItem : notnull
//{
//    [Parameter]
//    public IEnumerable<TValue>? Items { get; set; }
//}
