#nullable enable

using Microsoft.AspNetCore.Components;
using LionFire.Structures.Keys;
using LionFire.Reflection;
using LionFire.Reactive;
using LionFire.Data.Async.Gets;
using static MudBlazor.CategoryTypes;
using DynamicData.Binding;
using LionFire.ExtensionMethods;
using LionFire.Mvvm;
using LionFire.Data.Mvvm;
using LionFire.FlexObjects;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using DynamicData;
using LionFire.Data.Collections;
using Orleans.Runtime;
using LionFire.Reactive.Persistence;
using DynamicData.Kernel;

namespace LionFire.Blazor.Components;

// TODO: Make this class as dumb as possible, moving testable logic to the AsyncKeyedCollectionVM / TValueVM classes

/// <summary>
/// How to use:
/// - without a ViewModel type: TValueVM same as TValue
/// - with a ViewModel type: TValueVM different than TValue
/// 
/// TODO:
///  - optional ViewModel type: ctor injection, or IViewModelProvider
///  - optional reorder support
///
///  RENAME: AsyncDictionaryView
/// </summary>
/// <typeparam name="TValue"></typeparam>
//[CascadingTypeParameter(nameof(T))]
public partial class ObservableDataView<TKey, TValue, TValueVM>
    : IAsyncDisposable
    , IComponentized
    , IGetsOrCreatesByType
    where TKey : notnull
    where TValue : notnull
    where TValueVM : notnull
{
    #region Aspects

    #region IComponentized

    Components Components { get; set; } = new();

    public T TryGetComponent<T>() where T : class
    {
        return ((IComponentized)Components).TryGetComponent<T>();
    }
    #endregion

    #endregion

    #region Parameters

    #region Items

    ///// <summary>
    ///// Recommended:
    /////  - IObservableCache<TValue, TItem>
    /////     - see AsyncObservableCache<TItem, TValue> and
    /////     - AsyncComposableObservableCache<TItem, TValue>)
    ///// </summary>
    //[Parameter]
    //public IEnumerable<TValue>? Items { get; set; }
    //public IEnumerable<TValue>? EffectiveItems { get; private set; }

    //[Parameter]
    //public bool AllowCreate { get; set; }
    //[Parameter]
    //public bool AllowDelete { get; set; }
    //[Parameter]
    //public bool AllowEdit { get; set; }

    [Parameter]
    public EditMode AllowedEditModes { get; set; }


    #region MudDataGrid passthrough

    [Parameter]
    public DataGridEditMode EditMode { get; set; } = DataGridEditMode.Form;

    #endregion


    [Parameter]
    public Func<TKey, Optional<TValue>, TValueVM>? VMFactory { get; set; }

    /// <summary>
    /// If writable, supply IObservableReaderWriter here
    /// </summary>
    [Parameter]
    public IObservableReader<TKey, TValue>? Data { get; set; }

    public IObservableReader<TKey, TValue>? EffectiveData { get; private set; }
    public IObservableReaderWriter<TKey, TValue>? EffectiveDataWriter { get; private set; }

    [Parameter]
    public TimeSpan? PollDelay { get; set; }


    //#region Derived

    ///// <summary>
    ///// Previous value of Items.
    ///// Used for change detection.
    ///// </summary>
    //private IEnumerable<TValue>? oldItems { get; set; }

    //#endregion

    #endregion

    #region Override: ChildContent

    [Parameter]
    public RenderFragment<ObservableDataVM<TKey, TValue, TValueVM>>? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? Columns { get; set; }

    #endregion

    #region ViewModel

    #region ViewModelOptions 

    public VMOptions ViewModelOptions { get; set; }

    #region Derived

    public bool HasVM => typeof(TValue) != typeof(TValueVM);

    #region TODO: Pass-thru to ViewModelOptions

    //public Func<TValueVM, object>? ValueSelector { get;set;} = null;

    [Parameter]
    public IEnumerable<Type>? CreatableTypes { get; set; }

    [Parameter]
    public bool CanCreateValueType { get; set; } = true;



    //[Parameter]
    //public bool ShowRefresh { get; set; }

    //[Parameter]
    //public bool ReadOnly { get; set; }

    //[Parameter]
    //public bool AutoRetrieveOnInit { get; set; } = true; // TODO

    #region TODO:  Pass-thru to ICollectionViewModelOptions

    //[Parameter]
    //public Func<TValue, TValueVM?> CreateViewModel { get; set; } = DefaultGetDisplayValue;

    //public static Func<TValue, TValueVM?> DefaultCreateViewModel{ get; set; } = item => (TValueVM?)Activator.CreateInstance(typeof(TValueVM), item);
    //[Parameter]
    //public object[] CreateParameters { get; set; } = new object[] { };

    //[Parameter]
    //public Func<Type, object[]>? ItemConstructorParameters { get; set; } = null;

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

    #region Triage

    ///// <summary>
    ///// Optional.  If none set, will compare using DefaultItemComparer
    ///// </summary>
    //[Parameter]
    //public IComparer<TValueVM>? ViewModelComparer { get; set; }

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

    #endregion

    #endregion

    #endregion

    #endregion

    #endregion

    #endregion

    #region Refs

    public MudDataGrid<TValueVM> MudDataGrid { get; set; }

    #endregion

    #region Lifecycle

    public ObservableDataView()
    {
        this.WhenActivated(disposableRegistration =>
        {
            if (ViewModel == null) throw new ArgumentNullException(nameof(ViewModel));
            ViewModel.PropertyChanged += ViewModel_PropertyChanged; // No Memory leak


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

            // I think this should work too?
            //this.OneWayBind(ViewModel,
            //        viewModel => viewModel.ValueVMs, // REVIEW Nullability: Source
            //        view => view.Children
            //        //,
            //        //vmToViewConverter: vm => vm?.Select(kvp => kvp.Value),
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

    IDisposable? itemsSubscription = null;
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        ViewModel!.AllowedEditModes = AllowedEditModes;
        ViewModel!.CreatableTypes = CreatableTypes ?? [];
        ViewModel!.CanCreateValueType = CanCreateValueType;


        ArgumentNullException.ThrowIfNull(ViewModel);

        ViewModel!.VMFactory = VMFactory;
        ViewModel!.Data = Data;

        ViewModel.ItemsChanged.Subscribe(_ => InvokeAsync(StateHasChanged));
        //ViewModel.WhenAnyValue(vm => vm.Items).Subscribe(items => // MOVE this as an IObservable to VM class
        //{
        //    itemsSubscription?.Dispose();
        //    itemsSubscription = items?.Connect().Subscribe(_ => InvokeAsync(StateHasChanged));
        //});

        //if (EffectiveData == null || !ReferenceEquals(EffectiveData, Data))
        //{
            //EffectiveData = Data;
            //EffectiveDataWriter = EffectiveData as IObservableReaderWriter<TKey, TValue>;
//#if true // NEW
//            ViewModel.Data?.ObservableCache.Connect().Subscribe(o =>
//            {
//                //Debug.WriteLine($"ObservableDataView: VMCollectionChanged {ViewModel.ValueVMs != null}");
//                Debug.WriteLine($"ObservableDataView: Data changed");
//                InvokeAsync(StateHasChanged);
//            });
//#else // OLD
//            ViewModel.ValueVMCollections.Subscribe(o =>
//            {
//                Debug.WriteLine($"ObservableDataView: VMCollectionChanged {ViewModel.ValueVMs != null}");
//                o.Subscribe(_ => InvokeAsync(StateHasChanged));
//                InvokeAsync(StateHasChanged);
//            });
//#endif
        //}

        //if (oldItems == null || !ReferenceEquals(oldItems, this.Items))
        {

            //ViewModel.PollDelay = PollDelay;
            //ViewModel.Source = Items == null ? null
            //    : Items as IGetter<IEnumerable<TValue>>
            //        ?? new PreresolvedGetter<IEnumerable<TValue>>(Items);
            //ViewModel.FullFeaturedSource?.GetIfNeeded().AsTask().FireAndForget();

            //if (Items is IHasObservableCache<TValue, TKey> hoc)
            //{
            //    hoc.ObservableCache.Connect().Subscribe(changeSet =>
            //    {
            //        InvokeAsync(StateHasChanged);
            //    });
            //}


            //ViewModel.ViewModelPropertyChanges.Subscribe(o =>
            //{
            //    Debug.WriteLine($"ObservableDataView: ViewModelPropertyChanges {o}");
            //    InvokeAsync(StateHasChanged);
            //});

            //this.BindCommand(ViewModel,
            //    viewModel => viewModel.Create,
            //    view => view.)
        }
        //oldItems = Items;


        //#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
        //        if (MudDataGrid != null)
        //        {
        //            MudDataGrid.Columns ??= CreateColumns();
        //        }
        //#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
    }

    private RenderFragment CreateColumns => builder =>
    {
        var properties = typeof(TValueVM).GetProperties();

        foreach (var prop in properties)
        {
            MudDataGridUtilities.BuildPropertyColumn<TValueVM, TValue>(builder, prop);

            //if (prop.PropertyType == typeof(string))
            //{
            //    BuildPropertyColumn<string>(builder, prop);
            //}
            //else if (prop.PropertyType == typeof(int))
            //{
            //    BuildPropertyColumn<int>(builder, prop);
            //}
        }
        properties = typeof(TValue).GetProperties();
        foreach (var prop in properties)
        {
            MudDataGridUtilities.BuildPropertyColumn<TValueVM, TValue>(builder, prop, "Value");
            //    if (prop.PropertyType == typeof(string))
            //    {
            //        //BuildPropertyColumn(builder, prop, "p => p.Value.");
            //    }
        }

        /*
         * 
         *        <TemplateColumn>
                <CellTemplate>
                    <MudToggleIconButton Toggled="UniverseClient.UniverseInfo.Key == context.Item.Key"
                                         ToggledIcon="@Icons.Material.Filled.CheckCircle"
                                         Icon="@Icons.Material.Outlined.Circle" />
                </CellTemplate>
            </TemplateColumn>
            <PropertyColumn Property="p => p.Name" />
        */

        static void NewMethod(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder, System.Reflection.PropertyInfo prop, string? subProperty = null)
        {

        }
    };

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        //    new List<MudDataGridColumn<TValueVM>>
        //{
        //    new MudDataGridColumn<YourDataType>
        //    {
        //        Field = nameof(YourDataType.Id),
        //        Title = "ID",
        //        Width = 100
        //    },
        //    new MudDataGridColumn<YourDataType>
        //    {
        //        Field = nameof(YourDataType.Name),
        //        Title = "Name",
        //        Width = 200
        //    },
        //    // Add more columns as needed
        //};
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

    //private void ItemsEditor_GlobalItemsChanged(ObservableDataView<TValue, TValueVM> obj, string key)
    //{
    //    if (Object.ReferenceEquals(obj, this) || key != EffectiveKey) { return; }
    //    InvokeAsync(Retrieve).FireAndForget();
    //}

    #region Retrieve

    //public bool CanResolve => AsyncCollectionCache != null;

    //public async Task<IGetResult<IEnumerable<TValue>>> GetIfNeeded()
    //{
    //    if (Items is ILazilyGets<IEnumerable<TValue>> lazily)
    //    {
    //        var result = await lazily.TryGetValue().ConfigureAwait(false);
    //        return (IGetResult<IEnumerable<TValue>>)result;
    //    }

    //    if (Items is IGets<IEnumerable<TValue>> gets)
    //    {
    //        var result = await gets.Get().ConfigureAwait(false);
    //        return result;
    //    }
    //    throw new NotSupportedException();

    //}
    //public async Task<IGetResult<TValue>> Get()
    //{
    //    if (Items is IGets<IEnumerable<TValue>> gets)
    //    {
    //        var result = await gets.Get().ConfigureAwait(false);
    //        return result;
    //    }
    //    throw new NotSupportedException();
    //}

    #endregion

    #region Event Handling

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        Logger.LogInformation($"{nameof(ObservableDataView<TKey, TValue, TValueVM>)}.ViewModel.PropertyChanged: {e.PropertyName}");
    }

    //void RowClicked(DataGridRowClickEventArgs<(TValue, TValueVM)> args)
    //{
    //    //_events.Insert(0, $"Event = RowClick, Index = {args.RowIndex}, Data = {System.Text.Json.JsonSerializer.Serialize(args.Item)}");
    //}
    //void RowClicked(DataGridRowClickEventArgs<TValue> args)
    //{
    //    //_events.Insert(0, $"Event = RowClick, Index = {args.RowIndex}, Data = {System.Text.Json.JsonSerializer.Serialize(args.Item)}");
    //}
    void RowClicked(DataGridRowClickEventArgs<TValueVM> args)
    {
        //_events.Insert(0, $"Event = RowClick, Index = {args.RowIndex}, Data = {System.Text.Json.JsonSerializer.Serialize(args.Item)}");
    }

    #endregion

    public T GetOrCreate<T>() // TODO:  abstract this to IGetsOrCreates<T>
        where T : class
    {
        components ??= new();
        return (T)components.GetOrAdd(typeof(T), (type) => Activator.CreateInstance(type, ViewModel) ?? throw new Exception($"Failed to activate {typeof(T).FullName}"));
    }
    ConcurrentDictionary<Type, object>? components;

    public void OnCreate(Type t)
    {
        Console.WriteLine("Create: " + t);
        throw new NotImplementedException();
        //ViewModel.Create.Execute(t).Subscribe();
    }
}

//public partial class ListView<TItem, TValue, TValueVM> : IDisposable
//    where TItem : notnull
//{
//    [Parameter]
//    public IEnumerable<TValue>? Items { get; set; }
//}

