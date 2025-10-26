# ViewModels Guide

## Overview

This comprehensive guide covers all ViewModel types in LionFire, from simple key-value wrappers to complex reactive data access ViewModels. Understanding which ViewModel to use and when is crucial for building maintainable MVVM applications.

**Key Principle**: ViewModels wrap data access patterns and add UI-specific concerns without duplicating business logic.

---

## Table of Contents

1. [ViewModel Hierarchy](#viewmodel-hierarchy)
2. [Basic ViewModels](#basic-viewmodels)
3. [Data Access ViewModels](#data-access-viewmodels)
4. [Collection ViewModels](#collection-viewmodels)
5. [Observable Reader/Writer ViewModels](#observable-readerwriter-viewmodels)
6. [Creating Custom ViewModels](#creating-custom-viewmodels)
7. [ViewModel Lifecycle](#viewmodel-lifecycle)
8. [Best Practices](#best-practices)

---

## ViewModel Hierarchy

```
IViewModel (marker)
    │
    ├── IViewModel<TModel>
    │       │
    │       ├── ViewModel<TModel>          ← Generic wrapper
    │       │
    │       └── KeyVM<TKey>                ← Has Key property
    │               │
    │               └── KeyValueVM<TKey, TValue>  ← Key + Value (entity)
    │
    ├── IGetterVM<TValue>
    │       │
    │       ├── GetterVM<TValue>           ← Wraps IGetter
    │       │       │
    │       │       └── ValueVM<TValue>    ← Wraps IValue (read/write)
    │       │               │
    │       │               └── ValueMemberVM<TValue>  ← Property editors
    │       │
    │       └── ObservableReaderItemVM<TKey, TValue, TValueVM>  ← Workspace item
    │               │
    │               └── ObservableReaderWriterItemVM<...>  ← Workspace item (R/W)
    │
    └── Collection VMs
            ├── LazilyGetsCollectionVM<TValue>
            ├── AsyncObservableCacheVM<TObject, TKey>
            ├── ObservableReaderVM<TKey, TValue, TValueVM>
            └── ObservableDataVM<TKey, TValue, TValueVM>
```

---

## Basic ViewModels

### ViewModel\<TModel\>

**Location**: `LionFire.Mvvm`

**Purpose**: Generic wrapper around any model type.

```csharp
public class ViewModel<TModel> : ReactiveObject, IViewModel<TModel>
{
    [Reactive] private TModel? _model;

    public ViewModel(TModel model)
    {
        Model = model;
    }
}
```

**Usage**:
```csharp
// Simple wrapping
var vm = new ViewModel<BotEntity>(myBot);
```

**When to Use**:
- Need minimal ViewModel functionality
- Just wrapping a model for UI binding
- Don't need key or data access features

---

### KeyVM\<TKey\>

**Location**: `LionFire.Mvvm`

**Purpose**: ViewModel with a Key property for keyed entities.

```csharp
public class KeyVM<TKey> : ReactiveObject
{
    [Reactive] private TKey? _key;

    public KeyVM(TKey key)
    {
        Key = key;
    }
}
```

**Usage**:
```csharp
var vm = new KeyVM<string>("bot-001");
```

**When to Use**:
- Entity has a key/id
- Need to track which entity this VM represents
- Building master-detail scenarios

---

### KeyValueVM\<TKey, TValue\>

**Location**: `LionFire.Mvvm`

**Purpose**: ViewModel combining a key and value (entity).

**This is the most common base class for custom ViewModels.**

```csharp
public class KeyValueVM<TKey, TValue> : KeyVM<TKey>
{
    [Reactive] private TValue? _value;

    public KeyValueVM(TKey key, TValue value) : base(key)
    {
        Value = value;
    }
}
```

**Usage**:
```csharp
// Custom ViewModel inheriting KeyValueVM
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value)
    {
        // Add UI-specific logic
    }

    // Computed properties
    public string DisplayName => $"{Value.Name} ({Key})";

    public string StatusText => Value.Enabled ? "Running" : "Stopped";

    // Commands
    public ReactiveCommand<Unit, Unit> ToggleCommand { get; }
}
```

**When to Use**:
- Most common base for custom ViewModels
- Entity has a key (file name, database ID, etc.)
- Need to add UI-specific properties or commands

**Constructor Requirement**:
```csharp
// ObservableDataView and similar components expect this constructor
public MyVM(TKey key, TValue value) : base(key, value) { }
```

---

## Data Access ViewModels

### GetterVM\<TValue\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Wraps `IGetter<TValue>` for async data loading.

```csharp
public partial class GetterVM<TValue> : ReactiveObject, IGetterVM<TValue>
{
    public IGetter? Source { get; set; }
    public IGetterRxO<TValue>? FullFeaturedSource { get; }

    // Commands
    public ReactiveCommand<Unit, IGetResult<TValue>> GetCommand { get; }
    public ReactiveCommand<Unit, IGetResult<TValue>> GetIfNeeded { get; }

    // State
    public bool CanGet { get; }
    public bool IsGetting { get; }
    public bool HasValue { get; }

    // Configuration
    public TimeSpan? PollDelay { get; set; }
    public bool ShowRefresh { get; set; } = true;
}
```

**Features**:
- **GetCommand**: Explicitly load data
- **GetIfNeeded**: Load only if not cached
- **IsGetting**: Loading indicator
- **PollDelay**: Auto-refresh interval

**Usage**:
```csharp
var getter = serviceProvider.GetRequiredService<IGetter<WeatherData>>();
var vm = new GetterVM<WeatherData>(getter)
{
    PollDelay = TimeSpan.FromMinutes(5)
};

// Bind to UI
<MudButton Command="@vm.GetCommand">Refresh</MudButton>
<MudProgressCircular Visible="@vm.IsGetting" />
<MudText>@vm.FullFeaturedSource.ReadCacheValue?.Temperature</MudText>
```

**When to Use**:
- Data loaded via `IGetter<T>`
- Need loading indicators
- Want automatic refresh/polling
- Read-only data access

---

### ValueVM\<TValue\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Wraps `IValue<TValue>` for read/write data access.

```csharp
public partial class ValueVM<TValue> : GetterVM<TValue>, IValueVM<TValue>
{
    // Inherits Get operations from GetterVM

    // Additional Set operations
    public ReactiveCommand<TValue, ISetResult<TValue>> SetCommand { get; }
    public bool CanSet { get; }
    public bool IsSetting { get; }

    public bool IsBusy => IsGetting || IsSetting;
}
```

**Features**:
- All GetterVM features
- **SetCommand**: Save data
- **IsSetting**: Saving indicator
- **IsBusy**: Combined loading/saving state

**Usage**:
```csharp
var value = serviceProvider.GetRequiredService<IValue<UserProfile>>();
var vm = new ValueVM<UserProfile>(value);

// Bind to UI
<MudTextField @bind-Value="vm.FullFeaturedSource.ReadCacheValue.Name" />
<MudButton Command="@vm.SetCommand"
           CommandParameter="@vm.FullFeaturedSource.ReadCacheValue"
           Disabled="@vm.IsBusy">
    Save
</MudButton>
<MudProgressLinear Indeterminate Visible="@vm.IsBusy" />
```

**When to Use**:
- Data loaded/saved via `IValue<T>`
- Need save operations with progress
- Want combined loading/saving indicators

---

### ValueMemberVM\<TValue\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: ViewModel for a single property within an object (property editors).

```csharp
public class ValueMemberVM<TValue> : ValueVM<TValue>
{
    public string MemberName { get; set; }
    public string? DisplayName { get; set; }
    public Type MemberType { get; }
}
```

**Usage**:
```csharp
// Property editor for configuration
public class ConfigEditorVM : ReactiveObject
{
    public ConfigEditorVM(Config config)
    {
        NameVM = new ValueMemberVM<string>
        {
            MemberName = nameof(Config.Name),
            DisplayName = "Configuration Name",
            // ... set value
        };

        TimeoutVM = new ValueMemberVM<int>
        {
            MemberName = nameof(Config.Timeout),
            DisplayName = "Timeout (seconds)",
            // ... set value
        };
    }

    public ValueMemberVM<string> NameVM { get; }
    public ValueMemberVM<int> TimeoutVM { get; }
}

// Bind to UI
<MudTextField Label="@vm.NameVM.DisplayName"
              @bind-Value="vm.NameVM.Value" />
<MudNumericField Label="@vm.TimeoutVM.DisplayName"
                 @bind-Value="vm.TimeoutVM.Value" />
```

**When to Use**:
- Property grid / inspector views
- Form builders
- Configuration editors
- Need metadata about properties

---

## Collection ViewModels

### LazilyGetsCollectionVM\<TValue\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Lazy-loading collection ViewModel.

```csharp
public class LazilyGetsCollectionVM<TValue> : ReactiveObject
    where TValue : notnull
{
    public IObservableCache<TValue, int> Items { get; }
    public ReactiveCommand<Unit, IGetResult<IEnumerable<TValue>>> GetCommand { get; }

    public bool IsLoading { get; }
    public bool HasLoaded { get; }
}
```

**Features**:
- Lazy loading via `GetCommand`
- Observable cache of items
- Loading state tracking

**Usage**:
```csharp
var collectionGetter = serviceProvider.GetRequiredService<IGetter<IEnumerable<Product>>>();
var vm = new LazilyGetsCollectionVM<Product>(collectionGetter);

// Load data
await vm.GetCommand.Execute();

// Bind to UI
@foreach (var item in vm.Items.Items)
{
    <MudCard>@item.Name</MudCard>
}
```

**When to Use**:
- Loading collections via `IGetter<IEnumerable<T>>`
- Don't need keyed access
- Simple list display

---

### LazilyGetsDictionaryVM\<TKey, TValue\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Lazy-loading dictionary/keyed collection.

```csharp
public class LazilyGetsDictionaryVM<TKey, TValue> : ReactiveObject
    where TKey : notnull
    where TValue : notnull
{
    public IObservableCache<TValue, TKey> Items { get; }
    // Similar to LazilyGetsCollectionVM
}
```

**When to Use**:
- Loading keyed collections
- Need dictionary-like access
- Items have natural keys

---

### AsyncObservableCacheVM\<TObject, TKey\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Base class for ViewModels wrapping DynamicData `SourceCache`.

```csharp
public abstract class AsyncObservableCacheVM<TObject, TKey> : ReactiveObject
    where TObject : notnull
    where TKey : notnull
{
    public IObservableCache<TObject, TKey> ObservableCache { get; }
    public ReactiveCommand<Unit, IGetResult<IEnumerable<TObject>>> RefreshCommand { get; }

    protected abstract Task<IEnumerable<TObject>> LoadItems(CancellationToken ct);
}
```

**Usage**:
```csharp
public class ProductCacheVM : AsyncObservableCacheVM<Product, string>
{
    private readonly IProductRepository repository;

    public ProductCacheVM(IProductRepository repository)
    {
        this.repository = repository;
    }

    protected override async Task<IEnumerable<Product>> LoadItems(CancellationToken ct)
    {
        return await repository.GetAllProducts(ct);
    }
}
```

**When to Use**:
- Need custom loading logic
- Working with DynamicData `SourceCache`
- Building collection ViewModels from scratch

---

### AsyncKeyedVMCollectionVM\<TKey, TModel, TViewModel\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Transforms model collection to ViewModel collection.

```csharp
public class AsyncKeyedVMCollectionVM<TKey, TModel, TViewModel>
    : AsyncObservableCacheVM<TViewModel, TKey>
    where TKey : notnull
    where TModel : notnull
    where TViewModel : notnull
{
    public AsyncKeyedVMCollectionVM(
        IObservableCache<TModel, TKey> source,
        Func<TModel, TViewModel> createViewModel)
    {
        ViewModels = source
            .Connect()
            .Transform(createViewModel)
            .DisposeMany()
            .AsObservableCache();
    }

    public IObservableCache<TViewModel, TKey> ViewModels { get; }
}
```

**Features**:
- Automatic VM creation
- Automatic VM disposal when removed
- Reactive transformations

**Usage**:
```csharp
var entityCache = GetEntityCache();
var vmCollection = new AsyncKeyedVMCollectionVM<string, BotEntity, BotVM>(
    entityCache,
    entity => new BotVM(entity.Key, entity)
);

// Subscribe to VM changes
vmCollection.ViewModels.Connect()
    .Subscribe(changeSet => {
        // React to VM additions/removals
    });
```

**When to Use**:
- Have observable cache of entities
- Need to wrap each entity in ViewModel
- Want automatic VM lifecycle management

---

## Observable Reader/Writer ViewModels

These ViewModels integrate with LionFire's reactive persistence layer (`IObservableReader/Writer`), which is used for workspace-scoped documents.

### ObservableReaderItemVM\<TKey, TValue, TValueVM\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: ViewModel for reading a single item from workspace.

```csharp
public partial class ObservableReaderItemVM<TKey, TValue, TValueVM> : ReactiveObject
    where TKey : notnull
    where TValue : notnull
{
    public IObservableReader<TKey, TValue> Data { get; }

    public TKey? Id { get; set; }          // Item key (auto-loads on change)
    public TValue? Value { get; }          // Current value
    public bool IsLoading { get; }         // Loading state
}
```

**Behavior**:
- When `Id` is set, automatically loads item
- Subscribes to item's observable for reactive updates
- Disposes previous subscription when `Id` changes

**Usage**:
```csharp
[CascadingParameter(Name = "WorkspaceServices")]
public IServiceProvider? WorkspaceServices { get; set; }

var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
var vm = new ObservableReaderItemVM<string, BotEntity, BotVM>(reader);

// Load bot-001
vm.Id = "bot-001";  // Automatically loads

// Value updates reactively if bot-001.hjson changes on disk
```

**When to Use**:
- Read-only item display
- Item from workspace/observable reader
- Want automatic reactive updates

---

### ObservableReaderWriterItemVM\<TKey, TValue, TValueVM\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: ViewModel for reading and writing a single workspace item.

**This is the most common ViewModel for workspace detail pages.**

```csharp
public partial class ObservableReaderWriterItemVM<TKey, TValue, TValueVM>
    : ObservableReaderItemVM<TKey, TValue, TValueVM>
    where TKey : notnull
    where TValue : notnull
{
    public IObservableWriter<TKey, TValue> Writer { get; }

    public ValueTask Write();  // Save current Value
}
```

**Usage**:
```csharp
@page "/bots/{BotId}"

<MudCard>
    <MudCardContent>
        <MudTextField Label="Name" @bind-Value="vm.Value.Name" />
        <MudTextField Label="Description" @bind-Value="vm.Value.Description" />
    </MudCardContent>
    <MudCardActions>
        <MudButton OnClick="Save">Save</MudButton>
    </MudCardActions>
</MudCard>

@code {
    [Parameter] public string? BotId { get; set; }

    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    ObservableReaderWriterItemVM<string, BotEntity, BotVM>? vm;

    protected override void OnInitialized()
    {
        var reader = WorkspaceServices.GetRequiredService<IObservableReader<string, BotEntity>>();
        var writer = WorkspaceServices.GetRequiredService<IObservableWriter<string, BotEntity>>();

        vm = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);
        vm.Id = BotId;  // Loads bot
    }

    private async Task Save()
    {
        await vm.Write();  // Saves to workspace1/Bots/bot-001.hjson
        NavigationManager.NavigateTo("/bots");
    }
}
```

**When to Use**:
- Detail pages with edit capability
- Item from workspace
- Need save functionality
- **This is the standard pattern for workspace detail views**

---

### ObservableReaderVM\<TKey, TValue, TValueVM\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Collection-level ViewModel for observable readers.

```csharp
public partial class ObservableReaderVM<TKey, TValue, TValueVM> : ReactiveObject
    where TKey : notnull
    where TValue : notnull
{
    public IObservableReader<TKey, TValue> Data { get; }
    public bool AutoLoadAll { get; set; }
}
```

**When to Use**:
- Managing collection of workspace items
- Usually used internally by `ObservableDataView`
- Rare to use directly in application code

---

### ObservableDataVM\<TKey, TValue, TValueVM\>

**Location**: `LionFire.Data.Async.Mvvm`

**Purpose**: Advanced collection ViewModel used by `ObservableDataView`.

```csharp
public class ObservableDataVM<TKey, TValue, TValueVM> : ReactiveObject
    where TKey : notnull
    where TValue : notnull
    where TValueVM : notnull
{
    public IObservableCache<TValueVM, TKey> Items { get; }
    public IObservable<IChangeSet<TValueVM, TKey>> ItemsChanged { get; }

    public bool CanCreate { get; }
    public bool CanDelete { get; }
    public EditMode AllowedEditModes { get; set; }
}
```

**Features**:
- Automatically creates VMs for entities
- Reactive collection updates
- CRUD capability flags
- Used internally by `ObservableDataView` component

**When to Use**:
- **Rarely used directly** (ObservableDataView uses it internally)
- Custom collection components
- Need fine control over VM lifecycle

---

## Creating Custom ViewModels

### Pattern 1: Simple Custom ViewModel

```csharp
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value)
    {
        // Constructor required for ObservableDataView
    }

    // Computed properties
    public string DisplayName => $"{Value.Name} ({Key})";

    public Color StatusColor => Value.Enabled ? Color.Success : Color.Default;
}
```

---

### Pattern 2: ViewModel with Commands

```csharp
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value)
    {
        // Commands
        StartCommand = ReactiveCommand.CreateFromTask(
            StartAsync,
            this.WhenAnyValue(x => x.Value.Enabled, enabled => !enabled)
        );

        StopCommand = ReactiveCommand.CreateFromTask(
            StopAsync,
            this.WhenAnyValue(x => x.Value.Enabled)
        );
    }

    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; }

    private async Task StartAsync()
    {
        Value.Enabled = true;
        Value.Start();
        // Optionally auto-save
    }

    private async Task StopAsync()
    {
        Value.Enabled = false;
        Value.Stop();
    }
}
```

---

### Pattern 3: ViewModel with Computed Collections

```csharp
public class PortfolioVM : KeyValueVM<string, Portfolio>
{
    public PortfolioVM(string key, Portfolio value) : base(key, value)
    {
        // Reactive computed collection
        ActivePositions = this.WhenAnyValue(x => x.Value.Positions)
            .Select(positions => positions?.Where(p => p.IsActive) ?? Enumerable.Empty<Position>())
            .ToProperty(this, x => x.ActivePositions);
    }

    // Computed property (observable)
    public ObservableAsPropertyHelper<IEnumerable<Position>> ActivePositions { get; }

    // Computed scalar
    public decimal TotalValue => Value.Positions?.Sum(p => p.Value) ?? 0;
}
```

---

### Pattern 4: Aggregating Multiple Data Sources

```csharp
public class TradingDashboardVM : ReactiveObject
{
    public TradingDashboardVM(
        IObservableReader<string, BotEntity> botReader,
        IObservableReader<string, Portfolio> portfolioReader)
    {
        // Collection of bots
        BotsVM = new ObservableDataVM<string, BotEntity, BotVM>(botReader);

        // Single portfolio
        PortfolioVM = new ObservableReaderItemVM<string, Portfolio, PortfolioVM>(portfolioReader);
        PortfolioVM.Id = "main";

        // Computed summary combining both
        Summary = this.WhenAnyValue(
                x => x.BotsVM.Items.Count,
                x => x.PortfolioVM.Value.TotalValue,
                (botCount, totalValue) => $"{botCount} active bots, ${totalValue:N2} total")
            .ToProperty(this, x => x.Summary);
    }

    public ObservableDataVM<string, BotEntity, BotVM> BotsVM { get; }
    public ObservableReaderItemVM<string, Portfolio, PortfolioVM> PortfolioVM { get; }
    public ObservableAsPropertyHelper<string> Summary { get; }
}
```

---

## ViewModel Lifecycle

### Creation

**Manual Creation**:
```csharp
var vm = new BotVM("bot-001", botEntity);
```

**Automatic Creation** (ObservableDataView):
```csharp
// Component automatically creates VMs using constructor:
foreach (var entity in entities)
{
    var vm = ActivatorUtilities.CreateInstance<BotVM>(
        serviceProvider,
        entity.Key,
        entity.Value
    );
}
```

---

### Disposal

**IDisposable**:
```csharp
public class MyVM : KeyValueVM<string, MyEntity>, IDisposable
{
    private readonly CompositeDisposable subscriptions = new();

    public MyVM(string key, MyEntity value) : base(key, value)
    {
        this.WhenAnyValue(x => x.Value.SomeProperty)
            .Subscribe(...)
            .DisposeWith(subscriptions);
    }

    public void Dispose()
    {
        subscriptions.Dispose();
    }
}
```

**IAsyncDisposable** (Blazor):
```csharp
public class MyVM : KeyValueVM<string, MyEntity>, IAsyncDisposable
{
    public async ValueTask DisposeAsync()
    {
        await CleanupAsync();
        subscriptions.Dispose();
    }
}
```

---

## Best Practices

### 1. Keep ViewModels Thin

```csharp
// ✅ Good
public class BotVM : KeyValueVM<string, BotEntity>
{
    public string DisplayName => $"{Value.Name} ({Key})";
}

// ❌ Avoid - Business logic belongs in entity
public class BotVM : KeyValueVM<string, BotEntity>
{
    public void ProcessTrade(Trade trade)
    {
        // Complex business logic - should be in BotEntity!
    }
}
```

---

### 2. Use Appropriate Base Class

```csharp
// ✅ Use KeyValueVM for simple keyed entities
public class BotVM : KeyValueVM<string, BotEntity> { }

// ✅ Use ObservableReaderWriterItemVM for workspace items
var vm = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);

// ❌ Don't inherit from wrong base
public class BotVM : GetterVM<BotEntity>  // Wrong - not using IGetter!
```

---

### 3. Dispose Subscriptions

```csharp
// ✅ Always dispose
private readonly CompositeDisposable subscriptions = new();

this.WhenAnyValue(x => x.Something)
    .Subscribe(...)
    .DisposeWith(subscriptions);

public void Dispose() => subscriptions.Dispose();
```

---

### 4. Provide Required Constructor

```csharp
// ✅ Required for ObservableDataView
public BotVM(string key, BotEntity value) : base(key, value) { }

// ❌ Component can't create VM without this constructor
public BotVM(BotEntity value) : base("", value) { }
```

---

## Related Documentation

- **[MVVM Overview](README.md)** - MVVM architecture overview
- **[Reactive Patterns](reactive-patterns.md)** - Reactive programming patterns
- **[Data Binding](data-binding.md)** - UI binding patterns
- **[Blazor MVVM Patterns](../ui/blazor-mvvm-patterns.md)** - When to use which pattern
- **[LionFire.Data.Async.Mvvm](../../src/LionFire.Data.Async.Mvvm/CLAUDE.md)** - Complete API reference

---

## Summary

**ViewModel Selection Guide**:

| Scenario | ViewModel | Base Class |
|----------|-----------|------------|
| **Simple entity with key** | Custom VM | `KeyValueVM<TKey, TValue>` |
| **Workspace item (read-only)** | Use directly | `ObservableReaderItemVM<...>` |
| **Workspace item (editable)** | Use directly | `ObservableReaderWriterItemVM<...>` |
| **Async getter** | Use directly | `GetterVM<TValue>` |
| **Async read/write** | Use directly | `ValueVM<TValue>` |
| **Collection** | Use directly | `LazilyGetsCollectionVM<TValue>` |
| **Property editor** | Use directly | `ValueMemberVM<TValue>` |

**Most Common**:
- 90% of custom ViewModels inherit from `KeyValueVM<TKey, TValue>`
- 90% of workspace detail pages use `ObservableReaderWriterItemVM<...>`
- 90% of workspace list pages use `ObservableDataView` (which uses `ObservableDataVM` internally)
