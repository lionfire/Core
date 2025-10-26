# Reactive MVVM Patterns

**Overview**: LionFire's MVVM architecture is fundamentally reactive, built on ReactiveUI for property change notifications, reactive commands, and observable transformations. This document covers the reactive programming patterns used throughout the MVVM stack.

---

## Table of Contents

1. [ReactiveObject Foundation](#reactiveobject-foundation)
2. [Reactive Properties](#reactive-properties)
3. [WhenAnyValue Patterns](#whenanyvalue-patterns)
4. [Reactive Commands](#reactive-commands)
5. [Observable Collections with DynamicData](#observable-collections-with-dynamicdata)
6. [Subscription Management](#subscription-management)
7. [Common Patterns](#common-patterns)

---

## ReactiveObject Foundation

### What is ReactiveObject?

**ReactiveObject** is the base class from ReactiveUI that provides `INotifyPropertyChanged` and reactive infrastructure.

```csharp
using ReactiveUI;

public partial class PersonVM : ReactiveObject
{
    // Reactive properties automatically notify changes
    // Observable streams for property changes
    // Command execution support
}
```

**Key Features**:
- Implements `INotifyPropertyChanged` and `INotifyPropertyChanging`
- `RaiseAndSetIfChanged` helper for manual properties
- `WhenAnyValue` for observing property changes
- `ObservableAsPropertyHelper` for derived properties
- Automatic change tracking for UI binding

### Alternative: ObservableObject (CommunityToolkit)

LionFire also supports **CommunityToolkit.Mvvm**:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

public partial class PersonVM : ObservableObject
{
    [ObservableProperty]
    private string? _name;  // Generates Name property with INPC
}
```

**Compatibility**: Can mix ReactiveUI and CommunityToolkit patterns.

---

## Reactive Properties

### Using [Reactive] Attribute

**Recommended Pattern** - ReactiveUI source generators:

```csharp
using ReactiveUI;
using ReactiveUI.SourceGenerators;

public partial class BotVM : ReactiveObject
{
    [Reactive]
    private string? _name;

    [Reactive]
    private bool _enabled;

    [Reactive]
    private double? _stopLoss;
}
```

**Generated Code** (automatic):
```csharp
// Source generator creates:
public string? Name
{
    get => _name;
    set => this.RaiseAndSetIfChanged(ref _name, value);
}

public bool Enabled
{
    get => _enabled;
    set => this.RaiseAndSetIfChanged(ref _enabled, value);
}
```

**Benefits**:
- Automatic `INotifyPropertyChanged`
- Less boilerplate
- Type-safe
- Observable streams available

### Manual Property Pattern

For complex properties:

```csharp
public partial class MyVM : ReactiveObject
{
    private string? _name;
    public string? Name
    {
        get => _name;
        set
        {
            // Custom validation
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name required");

            this.RaiseAndSetIfChanged(ref _name, value);
        }
    }
}
```

### Observable Properties (Derived)

**Pattern**: Properties derived from observables.

```csharp
public partial class PersonVM : ReactiveObject
{
    [Reactive] private string? _firstName;
    [Reactive] private string? _lastName;

    // Derived from other properties
    private readonly ObservableAsPropertyHelper<string> _fullName;
    public string FullName => _fullName.Value;

    public PersonVM()
    {
        // Compute FullName whenever FirstName or LastName changes
        _fullName = this.WhenAnyValue(
                x => x.FirstName,
                x => x.LastName,
                (first, last) => $"{first} {last}")
            .ToProperty(this, x => x.FullName);
    }
}
```

**Alternative Syntax**:
```csharp
// Extension method for computed properties
this.WhenAnyValue(x => x.FirstName, x => x.LastName)
    .Select(t => $"{t.Item1} {t.Item2}")
    .ToPropertyEx(this, x => x.FullName);
```

---

## WhenAnyValue Patterns

### Basic Usage

**Observe single property**:

```csharp
this.WhenAnyValue(x => x.Name)
    .Subscribe(name => Console.WriteLine($"Name: {name}"));
```

### Multiple Properties

**Observe multiple properties**:

```csharp
this.WhenAnyValue(
        x => x.FirstName,
        x => x.LastName,
        x => x.Age,
        (first, last, age) => new { first, last, age })
    .Subscribe(person => UpdateDisplay(person));
```

### Filtering and Transformation

```csharp
this.WhenAnyValue(x => x.SearchText)
    .Throttle(TimeSpan.FromMilliseconds(300))  // Debounce
    .Where(text => !string.IsNullOrEmpty(text))
    .DistinctUntilChanged()
    .Subscribe(text => PerformSearch(text));
```

### Binding to Commands

```csharp
// Enable Save command only when Name is not empty
var canSave = this.WhenAnyValue(x => x.Name)
    .Select(name => !string.IsNullOrEmpty(name));

SaveCommand = ReactiveCommand.CreateFromTask(Save, canSave);
```

### Subscription Management

**Use `DisposeWith` for automatic cleanup**:

```csharp
public MyVM()
{
    this.WhenActivated(disposables =>
    {
        this.WhenAnyValue(x => x.Name)
            .Subscribe(name => UpdateSomething(name))
            .DisposeWith(disposables);  // Auto-disposed when deactivated
    });
}
```

---

## Reactive Commands

### Basic Commands

```csharp
using ReactiveUI;
using System.Reactive;

public partial class BotVM : ReactiveObject
{
    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; }

    public BotVM()
    {
        StartCommand = ReactiveCommand.CreateFromTask(StartAsync);
        StopCommand = ReactiveCommand.CreateFromTask(StopAsync);
    }

    private async Task StartAsync()
    {
        // Async work
    }

    private async Task StopAsync()
    {
        // Async work
    }
}
```

### Commands with Parameters

```csharp
public ReactiveCommand<string, Unit> DeleteBotCommand { get; }

public BotVM()
{
    DeleteBotCommand = ReactiveCommand.CreateFromTask<string>(botId =>
        botService.Delete(botId));
}

// Execute with parameter
await DeleteBotCommand.Execute("bot-123");
```

### Commands with Results

```csharp
public ReactiveCommand<Unit, BotStatus> GetStatusCommand { get; }

public BotVM()
{
    GetStatusCommand = ReactiveCommand.CreateFromTask(GetStatusAsync);

    // Subscribe to results
    GetStatusCommand.Subscribe(status =>
    {
        CurrentStatus = status;
    });
}

private async Task<BotStatus> GetStatusAsync()
{
    return await botService.GetStatus();
}
```

### Commands with Conditions

```csharp
// Enable command only when conditions met
var canSave = this.WhenAnyValue(
    x => x.Name,
    x => x.IsValid,
    (name, valid) => !string.IsNullOrEmpty(name) && valid);

SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, canSave);

// Command automatically disables when conditions not met
```

### Command Execution State

```csharp
public ReactiveCommand<Unit, Unit> LoadCommand { get; }

[ObservableAsProperty]
public bool IsLoading { get; }

public MyVM()
{
    LoadCommand = ReactiveCommand.CreateFromTask(LoadAsync);

    // Bind execution state to property
    LoadCommand.IsExecuting
        .ToPropertyEx(this, x => x.IsLoading);

    // Handle errors
    LoadCommand.ThrownExceptions
        .Subscribe(ex => ErrorMessage = ex.Message);
}
```

---

## Observable Collections with DynamicData

### SourceCache Pattern

**Foundation**: `SourceCache<TObject, TKey>` is the mutable source.

```csharp
using DynamicData;

public class BotsVM : ReactiveObject
{
    // Private mutable cache
    private SourceCache<BotEntity, string> _bots = new(b => b.Id);

    // Public observable cache (read-only)
    public IObservableCache<BotEntity, string> Bots => _bots;

    // Modify cache
    public void AddBot(BotEntity bot) => _bots.AddOrUpdate(bot);
    public void RemoveBot(string id) => _bots.Remove(id);
}
```

### Transformations

**Transform entities to ViewModels**:

```csharp
private SourceCache<BotEntity, string> _bots = new(b => b.Id);

// Transform to ViewModels
public IObservable<IChangeSet<BotVM, string>> BotVMs =>
    _bots.Connect()
         .Transform(entity => new BotVM(entity))
         .Publish()
         .RefCount();

// Or as observable cache
public IObservableCache<BotVM, string> BotVMsCache { get; }

public BotsVM()
{
    BotVMsCache = _bots.Connect()
        .Transform(entity => new BotVM(entity))
        .AsObservableCache();
}
```

### Filtering

```csharp
// Filter for enabled bots only
var enabledBots = _bots.Connect()
    .Filter(bot => bot.Enabled);

// Dynamic filter (reacts to property changes)
var filterObservable = this.WhenAnyValue(x => x.ShowOnlyEnabled);

var dynamicFilter = _bots.Connect()
    .Filter(filterObservable.Select(showEnabled =>
        (Func<BotEntity, bool>)(bot => !showEnabled || bot.Enabled)));
```

### Sorting

```csharp
var sortedBots = _bots.Connect()
    .Sort(SortExpressionComparer<BotEntity>.Ascending(b => b.Name));

// Dynamic sort
var sortObservable = this.WhenAnyValue(x => x.SortColumn);

var dynamicSort = _bots.Connect()
    .Sort(sortObservable.Select(col => GetComparerForColumn(col)));
```

### Binding to UI Collections

```csharp
// In ViewModel
private readonly ReadOnlyObservableCollection<BotVM> _botList;
public ReadOnlyObservableCollection<BotVM> BotList => _botList;

public BotsVM()
{
    _bots.Connect()
         .Transform(entity => new BotVM(entity))
         .Bind(out _botList)  // Binds to observable collection
         .Subscribe();
}

// In UI (WPF/Avalonia)
<ListBox ItemsSource="{Binding BotList}" />

// In UI (Blazor)
@foreach (var bot in ViewModel.BotList)
{
    <div>@bot.Value.Name</div>
}
```

---

## Subscription Management

### The Problem

```csharp
// ❌ BAD - memory leak!
public MyVM()
{
    this.WhenAnyValue(x => x.Name)
        .Subscribe(name => { /* ... */ });
    // Subscription never disposed!
}
```

### Solution 1: WhenActivated

**Recommended** - Automatic disposal:

```csharp
public MyVM()
{
    this.WhenActivated(disposables =>
    {
        this.WhenAnyValue(x => x.Name)
            .Subscribe(name => { /* ... */ })
            .DisposeWith(disposables);

        LoadCommand.Subscribe(_ => { /* ... */ })
            .DisposeWith(disposables);
    });
}
```

**When activated**: Subscriptions created
**When deactivated**: All subscriptions disposed automatically

### Solution 2: CompositeDisposable

**Manual management**:

```csharp
private CompositeDisposable? disposables;

public MyVM()
{
    disposables = new CompositeDisposable();

    this.WhenAnyValue(x => x.Name)
        .Subscribe(name => { /* ... */ })
        .DisposeWith(disposables);
}

public void Dispose()
{
    disposables?.Dispose();
    disposables = null;
}
```

### Solution 3: Observable Lifetime Operators

**Automatic disposal** via operators:

```csharp
// TakeUntil: Subscribe until condition
this.WhenAnyValue(x => x.Name)
    .TakeUntil(this.WhenAnyValue(x => x.IsDisposed).Where(d => d))
    .Subscribe(name => { /* ... */ });

// Take: Only take N emissions
this.WhenAnyValue(x => x.Name)
    .Take(1)  // Only first value
    .Subscribe(name => InitialName = name);
```

---

## Common Patterns

### Pattern 1: Computed Property

```csharp
public partial class PersonVM : ReactiveObject
{
    [Reactive] private string? _firstName;
    [Reactive] private string? _lastName;

    private readonly ObservableAsPropertyHelper<string> _fullName;
    public string FullName => _fullName.Value;

    public PersonVM()
    {
        _fullName = this.WhenAnyValue(
                x => x.FirstName,
                x => x.LastName,
                (f, l) => $"{f} {l}".Trim())
            .ToProperty(this, x => x.FullName);
    }
}
```

### Pattern 2: Validation

```csharp
public partial class BotVM : ReactiveObject
{
    [Reactive] private string? _name;
    [Reactive] private double? _stopLoss;

    private readonly ObservableAsPropertyHelper<bool> _isValid;
    public bool IsValid => _isValid.Value;

    private readonly ObservableAsPropertyHelper<string?> _validationError;
    public string? ValidationError => _validationError.Value;

    public BotVM()
    {
        var validation = this.WhenAnyValue(
                x => x.Name,
                x => x.StopLoss,
                (name, sl) => Validate(name, sl))
            .Publish()
            .RefCount();

        _isValid = validation
            .Select(v => v.IsValid)
            .ToProperty(this, x => x.IsValid);

        _validationError = validation
            .Select(v => v.ErrorMessage)
            .ToProperty(this, x => x.ValidationError);
    }

    private (bool IsValid, string? ErrorMessage) Validate(string? name, double? stopLoss)
    {
        if (string.IsNullOrEmpty(name))
            return (false, "Name is required");
        if (stopLoss is <= 0 or > 10)
            return (false, "Stop loss must be between 0 and 10");
        return (true, null);
    }
}
```

### Pattern 3: Async Property Loading

```csharp
public partial class BotVM : ReactiveObject
{
    [Reactive] private BotStatus? _status;

    private readonly ObservableAsPropertyHelper<bool> _isLoadingStatus;
    public bool IsLoadingStatus => _isLoadingStatus.Value;

    public ReactiveCommand<Unit, BotStatus> LoadStatusCommand { get; }

    public BotVM(IBotService botService)
    {
        LoadStatusCommand = ReactiveCommand.CreateFromTask(() =>
            botService.GetStatus(BotId));

        // Bind command result to property
        LoadStatusCommand.Subscribe(status => Status = status);

        // Track loading state
        _isLoadingStatus = LoadStatusCommand.IsExecuting
            .ToProperty(this, x => x.IsLoadingStatus);

        // Handle errors
        LoadStatusCommand.ThrownExceptions
            .Subscribe(ex => Logger.LogError(ex, "Failed to load status"));
    }
}
```

### Pattern 4: Reactive Filtering

```csharp
public partial class BotsVM : ReactiveObject
{
    private SourceCache<BotEntity, string> _allBots = new(b => b.Id);

    [Reactive] private string? _searchText;
    [Reactive] private bool _showOnlyEnabled;

    public IObservableCache<BotVM, string> FilteredBots { get; }

    public BotsVM()
    {
        // Combine multiple filters
        var filter = this.WhenAnyValue(
                x => x.SearchText,
                x => x.ShowOnlyEnabled,
                (search, onlyEnabled) => (Func<BotEntity, bool>)(bot =>
                {
                    if (onlyEnabled && !bot.Enabled) return false;
                    if (!string.IsNullOrEmpty(search) && !bot.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                        return false;
                    return true;
                }));

        FilteredBots = _allBots.Connect()
            .Filter(filter)
            .Transform(bot => new BotVM(bot))
            .AsObservableCache();
    }
}
```

### Pattern 5: Master-Detail

```csharp
public partial class BotsPageVM : ReactiveObject
{
    private SourceCache<BotEntity, string> _bots = new(b => b.Id);

    [Reactive] private string? _selectedBotId;

    private readonly ObservableAsPropertyHelper<BotVM?> _selectedBot;
    public BotVM? SelectedBot => _selectedBot.Value;

    public BotsPageVM()
    {
        // Automatically update SelectedBot when SelectedBotId changes
        _selectedBot = this.WhenAnyValue(x => x.SelectedBotId)
            .Select(id => id == null ? null : _bots.Lookup(id).ValueOrDefault())
            .Select(entity => entity == null ? null : new BotVM(entity))
            .ToProperty(this, x => x.SelectedBot);
    }
}
```

---

## Observable Collections with DynamicData

### Core Pattern

```csharp
using DynamicData;

public class BotsVM
{
    // Source: Mutable
    private SourceCache<BotEntity, string> _bots = new(b => b.Id);

    // Observable: Read-only changes
    public IObservable<IChangeSet<BotEntity, string>> BotsChanges =>
        _bots.Connect();

    // Cache: Read-only current state
    public IObservableCache<BotEntity, string> Bots => _bots;

    // Modifications
    public void Add(BotEntity bot) => _bots.AddOrUpdate(bot);
    public void Remove(string id) => _bots.Remove(id);
    public void Update(BotEntity bot) => _bots.AddOrUpdate(bot);
}
```

### Transformation Pipeline

```csharp
var pipeline = _bots.Connect()
    .Filter(bot => bot.Enabled)                          // 1. Filter
    .Transform(bot => new BotVM(bot))                    // 2. To VM
    .Sort(SortExpressionComparer<BotVM>.Ascending(b => b.Value.Name))  // 3. Sort
    .Publish()                                           // 4. Publish
    .RefCount();                                         // 5. RefCount

// Subscribe
pipeline.Subscribe(changeSet =>
{
    foreach (var change in changeSet)
    {
        Console.WriteLine($"{change.Reason}: {change.Current.Value.Name}");
    }
});
```

### Change Set Operations

```csharp
_bots.Connect().Subscribe(changeSet =>
{
    foreach (var change in changeSet)
    {
        switch (change.Reason)
        {
            case ChangeReason.Add:
                OnBotAdded(change.Current);
                break;
            case ChangeReason.Update:
                OnBotUpdated(change.Current, change.Previous.Value);
                break;
            case ChangeReason.Remove:
                OnBotRemoved(change.Current);
                break;
        }
    }
});
```

### Aggregations

```csharp
// Count enabled bots
public IObservable<int> EnabledBotCount =>
    _bots.Connect()
         .Filter(bot => bot.Enabled)
         .Count();

// Average stop loss
public IObservable<double> AverageStopLoss =>
    _bots.Connect()
         .Filter(bot => bot.StopLoss.HasValue)
         .Transform(bot => bot.StopLoss!.Value)
         .QueryWhenChanged(bots => bots.Average());
```

---

## Performance Patterns

### RefCount for Shared Observables

```csharp
// Share expensive transformation across multiple subscribers
private IObservable<IChangeSet<BotVM, string>> sharedBots;

public BotsVM()
{
    sharedBots = _bots.Connect()
        .Transform(bot => new BotVM(bot))  // Expensive
        .Publish()    // Share upstream
        .RefCount();  // Dispose when no subscribers

    // Multiple subscriptions share one transformation
    EnabledBots = sharedBots.Filter(b => b.Value.Enabled);
    LiveBots = sharedBots.Filter(b => b.Value.Live);
}
```

### Throttle for Expensive Operations

```csharp
this.WhenAnyValue(x => x.SearchText)
    .Throttle(TimeSpan.FromMilliseconds(300))  // Wait 300ms after typing stops
    .DistinctUntilChanged()
    .Subscribe(text => PerformExpensiveSearch(text));
```

### Batch Updates

```csharp
// Batch multiple updates
_bots.Edit(updater =>
{
    updater.AddOrUpdate(bot1);
    updater.AddOrUpdate(bot2);
    updater.Remove("bot3");
    // Only one changeset emitted for all changes
});
```

---

## Summary

### Reactive MVVM Core Principles

1. **ReactiveObject Base** - Foundation for reactive properties
2. **[Reactive] Attribute** - Source-generated reactive properties
3. **WhenAnyValue** - Observe property changes
4. **ReactiveCommand** - Async commands with execution state
5. **DynamicData** - Reactive collections with transformations
6. **DisposeWith** - Automatic subscription cleanup

### Common Patterns

| Pattern | Use Case | Operator |
|---------|----------|----------|
| **Computed Property** | Derive from other properties | `WhenAnyValue + ToProperty` |
| **Validation** | Reactive validation | `WhenAnyValue + Select` |
| **Filtering** | Dynamic collection filtering | `Connect().Filter()` |
| **Transformation** | Entity → ViewModel | `Connect().Transform()` |
| **Debouncing** | Delay rapid changes | `Throttle()` |
| **Conditional Commands** | Enable/disable commands | `ReactiveCommand(..., canExecute)` |

### Benefits of Reactive Patterns

✅ **Declarative** - Describe what, not how
✅ **Composable** - Chain operations
✅ **Automatic** - UI updates without manual code
✅ **Memory-Safe** - Managed subscriptions
✅ **Testable** - No UI required
✅ **Performant** - Efficient change propagation

### Related Documentation

- **[MVVM Architecture Overview](README.md)** - High-level architecture
- **[MVVM Layers](mvvm-layers.md)** - Layer breakdown
- **[Data Binding](data-binding.md)** - Async data integration
- **[ReactiveUI Documentation](https://www.reactiveui.net/)**
- **[DynamicData Documentation](https://github.com/reactivemarbles/DynamicData)**
