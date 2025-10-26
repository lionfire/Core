# Collection ViewModels

## Overview

This guide covers ViewModels for working with collections of data in LionFire. Collection ViewModels wrap async data sources, provide reactive updates, and automatically create ViewModels for individual items.

**Key Concept**: Collection ViewModels transform **collections of entities** into **collections of ViewModels**, handling lifecycle, subscriptions, and reactive updates.

---

## Table of Contents

1. [Collection ViewModel Types](#collection-viewmodel-types)
2. [Simple Collections](#simple-collections)
3. [Keyed Collections](#keyed-collections)
4. [Observable Cache ViewModels](#observable-cache-viewmodels)
5. [Workspace Collection ViewModels](#workspace-collection-viewmodels)
6. [Creating Custom Collection ViewModels](#creating-custom-collection-viewmodels)
7. [Master-Detail Patterns](#master-detail-patterns)
8. [Performance Considerations](#performance-considerations)

---

## Collection ViewModel Types

### Hierarchy

```
Collection ViewModels
│
├── Simple Collections (IEnumerable)
│   └── LazilyGetsCollectionVM<TValue, TValueVM>
│
├── Keyed Collections (Dictionary-like)
│   ├── LazilyGetsDictionaryVM<TKey, TValue>
│   └── LazilyGetsKeyedCollectionVM<TKey, TValue>
│
├── Observable Cache (DynamicData)
│   ├── AsyncObservableCacheVM<TKey, TValue, TValueVM>
│   ├── AsyncKeyedCollectionVM<TKey, TValue>
│   └── AsyncKeyedVMCollectionVM<TKey, TValue, TValueVM>
│
└── Workspace Collections (File-Backed)
    ├── ObservableReaderVM<TKey, TValue, TValueVM>
    ├── ObservableReaderWriterVM<TKey, TValue, TValueVM>
    └── ObservableDataVM<TKey, TValue, TValueVM>
```

---

## Simple Collections

### LazilyGetsCollectionVM\<TValue, TValueVM\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Lazy-loading collection ViewModel for `IGetter<IEnumerable<T>>`.

**When to Use**:
- Loading collections via `IGetter<IEnumerable<T>>`
- Don't need keyed access
- Simple list display without keys

#### Properties

```csharp
public class LazilyGetsCollectionVM<TValue, TValueVM> : GetterVM<IEnumerable<TValue>>
{
    // ViewModel provider for creating item VMs
    public IViewModelProvider ViewModelProvider { get; }

    // Inherits from GetterVM:
    // - GetCommand
    // - GetIfNeeded
    // - IsGetting, HasValue
    // - PollDelay
}
```

#### Usage

```csharp
// Data source
public class ProductRepository
{
    public IStatelessGetter<IEnumerable<Product>> GetAllProducts { get; }
}

// ViewModel
var getter = repository.GetAllProducts;
var vm = new LazilyGetsCollectionVM<Product, ProductVM>(viewModelProvider);
vm.Source = getter;

// Load data
await vm.GetCommand.Execute();

// Access items (as IEnumerable<TValue>)
var products = vm.FullFeaturedSource.ReadCacheValue;
foreach (var product in products)
{
    // ...
}
```

#### UI Binding

```razor
<MudButton Command="@vm.GetCommand" Disabled="@vm.IsGetting">
    @(vm.IsGetting ? "Loading..." : "Load Products")
</MudButton>

@if (vm.HasValue && vm.FullFeaturedSource.ReadCacheValue != null)
{
    <MudList>
        @foreach (var product in vm.FullFeaturedSource.ReadCacheValue)
        {
            <MudListItem>@product.Name - @product.Price.ToString("C")</MudListItem>
        }
    </MudList>
}
```

**Limitations**:
- No automatic VM creation for items
- No keyed access
- Manual iteration required

**Better Alternative**: For most scenarios, use `ObservableDataVM` or `AsyncObservableCacheVM` instead.

---

## Keyed Collections

### LazilyGetsDictionaryVM\<TKey, TValue\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Lazy-loading dictionary ViewModel with DynamicData cache.

**When to Use**:
- Loading keyed collections via `IGetter<IDictionary<TKey, TValue>>`
- Need dictionary-like access
- Want DynamicData reactive features

#### Properties

```csharp
public class LazilyGetsDictionaryVM<TKey, TValue> : ReactiveObject
    where TKey : notnull
    where TValue : notnull
{
    // DynamicData cache
    public IObservableCache<TValue, TKey> Items { get; }

    // Commands
    public ReactiveCommand<Unit, IGetResult<IDictionary<TKey, TValue>>> GetCommand { get; }

    // State
    public bool IsLoading { get; }
    public bool HasLoaded { get; }
}
```

#### Usage

```csharp
// Data source returning dictionary
var dictionaryGetter = serviceProvider.GetRequiredService<IGetter<IDictionary<string, Config>>>();

// ViewModel
var vm = new LazilyGetsDictionaryVM<string, Config>(dictionaryGetter);

// Load data
await vm.GetCommand.Execute();

// Access items
var config = vm.Items.Lookup("app-config");
if (config.HasValue)
{
    Console.WriteLine(config.Value.Name);
}

// Subscribe to changes
vm.Items.Connect()
    .Subscribe(changeSet => {
        // React to additions/removals
    });
```

#### UI Binding

```razor
<MudButton Command="@vm.GetCommand">Load Configs</MudButton>

@if (vm.HasLoaded)
{
    <MudDataGrid Items="@vm.Items.Items">
        <Columns>
            <PropertyColumn Property="x => x.Name" />
            <PropertyColumn Property="x => x.Value" />
        </Columns>
    </MudDataGrid>
}
```

---

### LazilyGetsKeyedCollectionVM\<TKey, TValue\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Similar to LazilyGetsDictionaryVM but for keyed collections.

**When to Use**: Similar scenarios to dictionary VM.

---

## Observable Cache ViewModels

These ViewModels work with **DynamicData** `SourceCache` for reactive collections.

### AsyncObservableCacheVM\<TKey, TValue, TValueVM\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Base class for ViewModels that manage a DynamicData cache and automatically create item ViewModels.

**When to Use**:
- Need custom loading logic
- Working with DynamicData `SourceCache`
- Want automatic VM creation and disposal

#### Abstract Members

```csharp
public abstract class AsyncObservableCacheVM<TKey, TValue, TValueVM> : ReactiveObject
    where TKey : notnull
    where TValue : notnull
    where TValueVM : notnull
{
    // Dependencies
    public IViewModelProvider ViewModelProvider { get; }

    // Observable cache of items (entities)
    protected IObservableCache<TValue, TKey> Source { get; set; }

    // Observable cache of ViewModels
    public IObservableCache<TValueVM, TKey> Items { get; }

    // VM factory
    public Func<TKey, Optional<TValue>, TValueVM>? VMFactory { get; set; }

    // Creation support
    public ICreatesAsync<TValue>? Creator { get; set; }
    public ReactiveCommand<ActivationParameters, Task<TValue>> Create { get; }
}
```

#### Creating a Custom Collection VM

```csharp
public class ProductCacheVM : AsyncObservableCacheVM<string, Product, ProductVM>
{
    private readonly IProductRepository repository;

    public ProductCacheVM(IViewModelProvider vmProvider, IProductRepository repository)
        : base(vmProvider)
    {
        this.repository = repository;

        // Load from repository and populate cache
        LoadCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var products = await repository.GetAllAsync();

            Source.Edit(updater =>
            {
                updater.Clear();
                foreach (var product in products)
                {
                    updater.AddOrUpdate(product, product.Id);
                }
            });
        });
    }

    public ReactiveCommand<Unit, Unit> LoadCommand { get; }
}
```

#### Usage

```csharp
var vm = new ProductCacheVM(vmProvider, repository);
await vm.LoadCommand.Execute();

// Access ViewModels
foreach (var productVM in vm.Items.Items)
{
    Console.WriteLine(productVM.Value.Name);
}

// Subscribe to changes
vm.Items.Connect()
    .Subscribe(changeSet => {
        // Handle adds, updates, removes
    });
```

---

### AsyncKeyedVMCollectionVM\<TKey, TValue, TValueVM\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Transforms a source cache of entities into a cache of ViewModels.

**When to Use**:
- Already have `IObservableCache<TValue, TKey>`
- Need to wrap each entity in ViewModel
- Want automatic VM lifecycle (creation/disposal)

#### Features

```csharp
public class AsyncKeyedVMCollectionVM<TKey, TModel, TViewModel>
    : AsyncObservableCacheVM<TKey, TViewModel, TKey>
    where TKey : notnull
    where TModel : notnull
    where TViewModel : notnull
{
    public AsyncKeyedVMCollectionVM(
        IObservableCache<TModel, TKey> source,
        Func<TModel, TViewModel> createViewModel)
    {
        // Automatically transforms models to ViewModels
        ViewModels = source
            .Connect()
            .Transform(createViewModel)
            .DisposeMany()  // Auto-dispose VMs when removed from cache
            .AsObservableCache();
    }

    public IObservableCache<TViewModel, TKey> ViewModels { get; }
}
```

#### Usage

```csharp
// Source cache of entities
var entityCache = new SourceCache<BotEntity, string>(e => e.Id);

// Transform to ViewModels
var vmCollection = new AsyncKeyedVMCollectionVM<string, BotEntity, BotVM>(
    entityCache,
    entity => new BotVM(entity.Id, entity)
);

// Add entity to source
entityCache.AddOrUpdate(new BotEntity { Id = "bot1", Name = "Bot 1" });

// VM automatically created and available
var botVM = vmCollection.ViewModels.Lookup("bot1");

// Remove entity from source
entityCache.Remove("bot1");
// VM automatically disposed
```

---

## Workspace Collection ViewModels

These ViewModels work with `IObservableReader/Writer` from LionFire's reactive persistence layer.

### ObservableReaderVM\<TKey, TValue, TValueVM\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Collection-level ViewModel for `IObservableReader`.

```csharp
public partial class ObservableReaderVM<TKey, TValue, TValueVM> : ReactiveObject
    where TKey : notnull
    where TValue : notnull
{
    public IObservableReader<TKey, TValue> Data { get; }
    public bool AutoLoadAll { get; set; }
}
```

**Features**:
- Wraps `IObservableReader<TKey, TValue>`
- `AutoLoadAll` triggers `Data.ListenAllKeys()`
- Base class for workspace collection VMs

**When to Use**: Rarely used directly - use `ObservableDataVM` instead.

---

### ObservableDataVM\<TKey, TValue, TValueVM\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Advanced collection ViewModel used by `ObservableDataView` component.

**This is the primary workspace collection ViewModel.**

#### Features

```csharp
public partial class ObservableDataVM<TKey, TValue, TValueVM> : ReactiveObject
    where TKey : notnull
    where TValue : notnull
    where TValueVM : notnull
{
    // Data source
    public IObservableReader<TKey, TValue>? Data { get; set; }
    public IObservableReaderWriter<TKey, TValue>? DataWriter { get; }

    // Items (ViewModels)
    public IObservableCache<TValueVM, TKey>? Items { get; }
    public IObservable<Unit> ItemsChanged { get; }

    // VM Factory
    public Func<TKey, Optional<TValue>, TValueVM>? VMFactory { get; set; }

    // CRUD capabilities
    public bool CanCreate { get; }
    public bool CanDelete { get; }
    public bool CanUpdate { get; }
    public bool CanRename { get; }
    public EditMode AllowedEditModes { get; set; }

    // Creation
    public IEnumerable<Type> CreatableTypes { get; set; }
    public ReactiveCommand<ActivationParameters, Task<TValue>> Create { get; }

    // Deletion
    public Task Delete(TValueVM value);

    // UI State
    public bool ShowRefresh { get; set; }
    public bool ShowDeleteColumn { get; set; }
}
```

#### Automatic VM Creation

When `Data` is set, the VM automatically:
1. Subscribes to `Data.Values.Connect()`
2. Transforms entities to ViewModels using `VMFactory` or default factory
3. Manages VM lifecycle (creation/disposal)
4. Updates `Items` cache reactively

**Default VM Factory**:
```csharp
// Automatically calls: new TValueVM(key, value)
static Func<TKey, Optional<TValue>, TValueVM> DefaultFactory =
    (k, v) => (TValueVM)Activator.CreateInstance(typeof(TValueVM), k, v.Value)!;
```

#### Usage Pattern 1: ObservableDataView (Automatic)

```razor
<!-- Component uses ObservableDataVM internally -->
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices">
    <Columns>
        <PropertyColumn Property="x => x.Value.Name" />
    </Columns>
</ObservableDataView>
```

**Behind the scenes**:
```csharp
// Component creates ObservableDataVM
var reader = DataServiceProvider.GetService<IObservableReader<TKey, TValue>>();
var vm = new ObservableDataVM<TKey, TValue, TValueVM>();
vm.Data = reader;  // Triggers automatic subscription and VM creation
```

---

#### Usage Pattern 2: Manual Collection Management

```csharp
@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    ObservableDataVM<string, BotEntity, BotVM>? botsVM;

    protected override void OnInitialized()
    {
        var reader = WorkspaceServices.GetRequiredService<IObservableReader<string, BotEntity>>();

        botsVM = new ObservableDataVM<string, BotEntity, BotVM>();
        botsVM.Data = reader;
        botsVM.AllowedEditModes = EditMode.All;
        botsVM.CreatableTypes = new[] { typeof(BotEntity) };
    }

    private async Task CreateBot()
    {
        await botsVM.Create.Execute(new ActivationParameters(typeof(BotEntity)));
    }

    private async Task DeleteBot(BotVM bot)
    {
        await botsVM.Delete(bot);
    }
}
```

#### Custom VM Factory

```csharp
botsVM.VMFactory = (key, valueOptional) =>
{
    if (!valueOptional.HasValue) return null;

    var vm = new BotVM(key, valueOptional.Value);

    // Custom initialization
    vm.LoadAdditionalData();
    vm.InitializeCommands();

    return vm;
};
```

---

### ObservableReaderWriterVM\<TKey, TValue, TValueVM\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Collection-level read/write ViewModel.

```csharp
public partial class ObservableReaderWriterVM<TKey, TValue, TValueVM>
    : ObservableReaderVM<TKey, TValue, TValueVM>
{
    public IObservableWriter<TKey, TValue> Writer { get; }
}
```

**When to Use**: Rarely - `ObservableDataVM` is more feature-rich.

---

## Master-Detail Patterns

### Pattern 1: List with Selection

```csharp
public class BotsPageVM : ReactiveObject
{
    public BotsPageVM(IObservableReader<string, BotEntity> reader)
    {
        // Collection VM
        CollectionVM = new ObservableDataVM<string, BotEntity, BotVM>();
        CollectionVM.Data = reader;

        // Selected item (reactive)
        this.WhenAnyValue(x => x.SelectedId)
            .Where(id => id != null)
            .Subscribe(id =>
            {
                var lookup = CollectionVM.Items.Lookup(id);
                SelectedBot = lookup.HasValue ? lookup.Value : null;
            });
    }

    public ObservableDataVM<string, BotEntity, BotVM> CollectionVM { get; }

    [Reactive] private string? _selectedId;
    [Reactive] private BotVM? _selectedBot;
}
```

**UI**:
```razor
<MudGrid>
    <!-- List -->
    <MudItem xs="6">
        <MudDataGrid Items="@vm.CollectionVM.Items.Items"
                     SelectedItemChanged="@(bot => vm.SelectedId = bot.Key)">
            <Columns>
                <PropertyColumn Property="x => x.Value.Name" />
            </Columns>
        </MudDataGrid>
    </MudItem>

    <!-- Detail -->
    <MudItem xs="6">
        @if (vm.SelectedBot != null)
        {
            <MudCard>
                <MudCardContent>
                    <MudTextField @bind-Value="vm.SelectedBot.Value.Name" />
                </MudCardContent>
            </MudCard>
        }
    </MudItem>
</MudGrid>
```

---

### Pattern 2: Multiple Collections

```csharp
public class DashboardVM : ReactiveObject
{
    public DashboardVM(
        IObservableReader<string, BotEntity> botReader,
        IObservableReader<string, Portfolio> portfolioReader,
        IObservableReader<string, Strategy> strategyReader)
    {
        // Multiple collection VMs
        BotsVM = new ObservableDataVM<string, BotEntity, BotVM>();
        BotsVM.Data = botReader;

        PortfoliosVM = new ObservableDataVM<string, Portfolio, PortfolioVM>();
        PortfoliosVM.Data = portfolioReader;

        StrategiesVM = new ObservableDataVM<string, Strategy, StrategyVM>();
        StrategiesVM.Data = strategyReader;

        // Computed properties combining collections
        TotalBots = this.WhenAnyValue(x => x.BotsVM.Items.Count)
            .ToProperty(this, x => x.TotalBots);

        ActiveBots = BotsVM.Items.Connect()
            .Filter(bot => bot.Value.Enabled)
            .Count()
            .ToProperty(this, x => x.ActiveBots);
    }

    public ObservableDataVM<string, BotEntity, BotVM> BotsVM { get; }
    public ObservableDataVM<string, Portfolio, PortfolioVM> PortfoliosVM { get; }
    public ObservableDataVM<string, Strategy, StrategyVM> StrategiesVM { get; }

    public int TotalBots => totalBots.Value;
    private readonly ObservableAsPropertyHelper<int> totalBots;

    public int ActiveBots => activeBots.Value;
    private readonly ObservableAsPropertyHelper<int> activeBots;
}
```

---

### Pattern 3: Filtered Collections

```csharp
public class BotListVM : ReactiveObject
{
    public BotListVM(IObservableReader<string, BotEntity> reader)
    {
        // Main collection
        AllBotsVM = new ObservableDataVM<string, BotEntity, BotVM>();
        AllBotsVM.Data = reader;

        // Filtered collection (active bots only)
        ActiveBots = AllBotsVM.Items.Connect()
            .Filter(bot => bot.Value.Enabled)
            .AsObservableCache();

        // Filtered collection (inactive bots)
        InactiveBots = AllBotsVM.Items.Connect()
            .Filter(bot => !bot.Value.Enabled)
            .AsObservableCache();
    }

    public ObservableDataVM<string, BotEntity, BotVM> AllBotsVM { get; }
    public IObservableCache<BotVM, string> ActiveBots { get; }
    public IObservableCache<BotVM, string> InactiveBots { get; }
}
```

**UI**:
```razor
<MudTabs>
    <MudTabPanel Text="All Bots">
        <MudDataGrid Items="@vm.AllBotsVM.Items.Items" />
    </MudTabPanel>
    <MudTabPanel Text="Active">
        <MudDataGrid Items="@vm.ActiveBots.Items" />
    </MudTabPanel>
    <MudTabPanel Text="Inactive">
        <MudDataGrid Items="@vm.InactiveBots.Items" />
    </MudTabPanel>
</MudTabs>
```

---

## Creating Custom Collection ViewModels

### Custom Loading Logic

```csharp
public class CustomProductCollectionVM : AsyncObservableCacheVM<string, Product, ProductVM>
{
    private readonly IProductService service;

    public CustomProductCollectionVM(IViewModelProvider vmProvider, IProductService service)
        : base(vmProvider)
    {
        this.service = service;

        // Custom load command with filtering
        LoadActiveProductsCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var products = await service.GetActiveProducts();
            UpdateSourceCache(products);
        });

        LoadAllProductsCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var products = await service.GetAllProducts();
            UpdateSourceCache(products);
        });
    }

    private void UpdateSourceCache(IEnumerable<Product> products)
    {
        Source.Edit(updater =>
        {
            updater.Clear();
            updater.AddOrUpdate(products);
        });
    }

    public ReactiveCommand<Unit, Unit> LoadActiveProductsCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadAllProductsCommand { get; }
}
```

---

### Sorted Collections

```csharp
public class SortedBotsVM : ReactiveObject
{
    public SortedBotsVM(IObservableReader<string, BotEntity> reader)
    {
        var baseVM = new ObservableDataVM<string, BotEntity, BotVM>();
        baseVM.Data = reader;

        // Sorted by name
        SortedByName = baseVM.Items.Connect()
            .Sort(SortExpressionComparer<BotVM>.Ascending(bot => bot.Value.Name))
            .AsObservableCache();

        // Sorted by profit/loss
        SortedByPL = baseVM.Items.Connect()
            .Sort(SortExpressionComparer<BotVM>.Descending(bot => bot.Value.ProfitLoss))
            .AsObservableCache();
    }

    public IObservableCache<BotVM, string> SortedByName { get; }
    public IObservableCache<BotVM, string> SortedByPL { get; }
}
```

---

### Grouped Collections

```csharp
public class GroupedBotsVM : ReactiveObject
{
    public GroupedBotsVM(IObservableReader<string, BotEntity> reader)
    {
        var baseVM = new ObservableDataVM<string, BotEntity, BotVM>();
        baseVM.Data = reader;

        // Group by exchange
        GroupedByExchange = baseVM.Items.Connect()
            .Group(bot => bot.Value.Exchange)
            .Transform(group => new ExchangeGroupVM(group.Key, group.Cache))
            .AsObservableCache();
    }

    public IObservableCache<ExchangeGroupVM, string> GroupedByExchange { get; }
}

public class ExchangeGroupVM
{
    public string Exchange { get; }
    public IObservableCache<BotVM, string> Bots { get; }

    public ExchangeGroupVM(string exchange, IObservableCache<BotVM, string> bots)
    {
        Exchange = exchange;
        Bots = bots;
    }
}
```

**UI**:
```razor
@foreach (var group in vm.GroupedByExchange.Items)
{
    <MudCard Class="mb-4">
        <MudCardHeader>
            <MudText Typo="Typo.h6">@group.Exchange (@group.Bots.Count bots)</MudText>
        </MudCardHeader>
        <MudCardContent>
            <MudDataGrid Items="@group.Bots.Items">
                <Columns>
                    <PropertyColumn Property="x => x.Value.Name" />
                </Columns>
            </MudDataGrid>
        </MudCardContent>
    </MudCard>
}
```

---

## Performance Considerations

### Large Collections

**Problem**: 10,000+ items slow to render.

**Solution 1**: Virtualization

```razor
<MudDataGrid Items="@vm.Items.Items"
             Virtualize="true"
             ItemSize="48" />
```

**Solution 2**: Pagination

```csharp
public class PaginatedBotsVM : ReactiveObject
{
    [Reactive] private int _currentPage;
    [Reactive] private int _pageSize = 25;

    public PaginatedBotsVM(IObservableReader<string, BotEntity> reader)
    {
        var baseVM = new ObservableDataVM<string, BotEntity, BotVM>();
        baseVM.Data = reader;

        // Paginated view
        CurrentPageItems = this.WhenAnyValue(
                x => x.CurrentPage,
                x => x.PageSize)
            .Select(_ => baseVM.Items.Items
                .Skip(CurrentPage * PageSize)
                .Take(PageSize))
            .ToProperty(this, x => x.CurrentPageItems);
    }

    public IEnumerable<BotVM> CurrentPageItems => currentPageItems.Value;
    private readonly ObservableAsPropertyHelper<IEnumerable<BotVM>> currentPageItems;
}
```

---

### Memory Management

**Dispose VMs Automatically**:

```csharp
// ✅ DisposeMany() automatically disposes removed VMs
source.Connect()
    .Transform(entity => new BotVM(entity.Id, entity))
    .DisposeMany()  // Critical for memory cleanup!
    .AsObservableCache();
```

**Without DisposeMany**:
```csharp
// ❌ Memory leak - VMs never disposed when removed
source.Connect()
    .Transform(entity => new BotVM(entity.Id, entity))
    .AsObservableCache();  // VMs leak!
```

---

### Subscription Management

```csharp
public class CollectionVM : ReactiveObject, IDisposable
{
    private readonly CompositeDisposable subscriptions = new();

    public CollectionVM(IObservableReader<string, MyEntity> reader)
    {
        // All subscriptions in composite
        reader.Values.Connect()
            .Subscribe(...)
            .DisposeWith(subscriptions);

        Items.Connect()
            .Subscribe(...)
            .DisposeWith(subscriptions);
    }

    public void Dispose()
    {
        subscriptions.Dispose();
    }
}
```

---

## Common Patterns

### Pattern: Search/Filter

```csharp
public class SearchableBotsVM : ReactiveObject
{
    [Reactive] private string? _searchText;

    public SearchableBotsVM(IObservableReader<string, BotEntity> reader)
    {
        var baseVM = new ObservableDataVM<string, BotEntity, BotVM>();
        baseVM.Data = reader;

        // Filter based on search text
        FilteredItems = this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Select(searchText =>
                baseVM.Items.Connect()
                    .Filter(bot => string.IsNullOrEmpty(searchText) ||
                                   bot.Value.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            )
            .Switch()  // Switch to new filtered observable
            .AsObservableCache();
    }

    public IObservableCache<BotVM, string> FilteredItems { get; }
}
```

---

### Pattern: Aggregations

```csharp
public class BotStatsVM : ReactiveObject
{
    public BotStatsVM(ObservableDataVM<string, BotEntity, BotVM> botsVM)
    {
        // Total count
        TotalCount = botsVM.Items.CountChanged
            .ToProperty(this, x => x.TotalCount);

        // Active count
        ActiveCount = botsVM.Items.Connect()
            .Filter(bot => bot.Value.Enabled)
            .Count()
            .ToProperty(this, x => x.ActiveCount);

        // Total profit/loss
        TotalPL = botsVM.Items.Connect()
            .Select(_ => botsVM.Items.Items.Sum(bot => bot.Value.ProfitLoss))
            .ToProperty(this, x => x.TotalPL);
    }

    public int TotalCount => totalCount.Value;
    private readonly ObservableAsPropertyHelper<int> totalCount;

    public int ActiveCount => activeCount.Value;
    private readonly ObservableAsPropertyHelper<int> activeCount;

    public decimal TotalPL => totalPL.Value;
    private readonly ObservableAsPropertyHelper<decimal> totalPL;
}
```

---

### Pattern: Hierarchical Collections

```csharp
public class HierarchicalBotsVM : ReactiveObject
{
    public HierarchicalBotsVM(IObservableReader<string, BotEntity> reader)
    {
        var baseVM = new ObservableDataVM<string, BotEntity, BotVM>();
        baseVM.Data = reader;

        // Group by exchange, then by strategy
        ExchangeGroups = baseVM.Items.Connect()
            .Group(bot => bot.Value.Exchange)
            .Transform(exchangeGroup =>
            {
                var strategyGroups = exchangeGroup.Cache.Connect()
                    .Group(bot => bot.Value.Strategy)
                    .Transform(strategyGroup => new StrategyGroupVM(
                        strategyGroup.Key,
                        strategyGroup.Cache))
                    .AsObservableCache();

                return new ExchangeGroupVM(exchangeGroup.Key, strategyGroups);
            })
            .AsObservableCache();
    }

    public IObservableCache<ExchangeGroupVM, string> ExchangeGroups { get; }
}
```

---

## Related Documentation

- **[ViewModels Guide](viewmodels-guide.md)** - Individual item ViewModels
- **[Reactive Patterns](reactive-patterns.md)** - Reactive programming
- **[Data Binding](data-binding.md)** - UI binding
- **[Blazor MVVM Patterns](../ui/blazor-mvvm-patterns.md)** - UI patterns
- **[LionFire.Data.Async.Mvvm](../../src/LionFire.Data.Async.Mvvm/CLAUDE.md)** - Complete API reference
- **[DynamicData Documentation](https://github.com/reactivemarbles/DynamicData)** - Reactive collections

---

## Summary

**Collection ViewModel Selection Guide**:

| Scenario | ViewModel | Why |
|----------|-----------|-----|
| **Workspace documents** | `ObservableDataVM` | File-backed, reactive, CRUD |
| **Simple list** | `LazilyGetsCollectionVM` | Basic lazy loading |
| **Keyed list** | `LazilyGetsDictionaryVM` | Dictionary access |
| **Custom loading** | `AsyncObservableCacheVM` | Full control |
| **Transform entities to VMs** | `AsyncKeyedVMCollectionVM` | Auto VM lifecycle |
| **Filtered/sorted** | DynamicData operators on base VM | Reactive transformations |

**Most Common**:
- **90% of workspace scenarios**: Use `ObservableDataView` component (uses `ObservableDataVM` internally)
- **Custom scenarios**: Inherit from `AsyncObservableCacheVM` for full control
- **Simple loading**: `LazilyGetsCollectionVM` or `LazilyGetsDictionaryVM`

**Key Pattern**: Collection ViewModels automatically transform entity collections to ViewModel collections, managing lifecycle and reactive updates.
