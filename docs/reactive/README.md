# Reactive Patterns in LionFire

## Overview

LionFire's **Reactive Programming** patterns provide observable, on-demand data flows built on **ReactiveUI** and **DynamicData**. These patterns enable building responsive applications where data changes automatically propagate through the system, resources activate only when needed, and lifecycle is managed reactively.

**Key Philosophy**: Subscribe to changes, not state. Resources activate on-demand and clean up automatically.

---

## Quick Start

### 1. Observable File Collection

```csharp
using LionFire.Reactive;
using DynamicData;

// Watch directory for file changes
var documents = ObservableFsDocuments.Create<Config>(
    dir: "./configs",
    deserialize: bytes => JsonSerializer.Deserialize<Config>(bytes)!
).AsObservableCache();

// Subscribe to changes
documents.Connect()
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            Console.WriteLine($"{change.Key}: {change.Reason}");
            if (change.Reason == ChangeReason.Add || change.Reason == ChangeReason.Update)
            {
                var config = change.Current;
                // React to new/updated config
            }
        }
    });

// Access current snapshot
var config = documents.Lookup("bot-001.json");
if (config.HasValue)
{
    Console.WriteLine($"Bot: {config.Value.Name}");
}
```

**Key Points**:
- Files automatically detected (polling every 1 second)
- Changes batched into `IChangeSet<T, TKey>`
- Deserialized on-demand when accessed
- Subscription activates file watching

---

### 2. Configuration-Driven Service (Runner Pattern)

```csharp
using LionFire.Execution;

public class BotRunner : Runner<BotConfig, BotRunner>
{
    private Bot? bot;

    // Define when configuration is "enabled"
    static bool IRunner<BotConfig>.IsEnabled(BotConfig config)
        => config.Enabled;

    // Start bot when enabled
    protected override async ValueTask<bool> Start(
        BotConfig config,
        Optional<BotConfig> oldConfig)
    {
        Console.WriteLine($"Starting bot: {config.Name}");
        bot = new Bot(config);
        await bot.StartAsync();
        return true;
    }

    // Handle config changes while running
    protected override void OnConfigurationChange(
        BotConfig newConfig,
        Optional<BotConfig> oldConfig)
    {
        Console.WriteLine($"Hot-reloading config: {newConfig.Name}");
        bot?.UpdateConfig(newConfig);
    }

    // Stop bot when disabled
    protected override void Stop(
        Optional<BotConfig> newConfig,
        Optional<BotConfig> oldConfig)
    {
        Console.WriteLine($"Stopping bot");
        bot?.Stop();
        bot = null;
    }
}

// Usage: Subscribe runner to config changes
var runner = new BotRunner();
configObservable.Subscribe(runner);

// When config.Enabled changes from false → true, bot starts
// When config.Enabled changes from true → false, bot stops
// When config changes while enabled, OnConfigurationChange called
```

**Key Points**:
- Implements `IObserver<BotConfig>`
- Automatically starts/stops based on `IsEnabled()`
- Hot-reload support via `OnConfigurationChange()`
- Fault tracking with `WorkerStatus`

---

### 3. On-Demand Resource Activation

```csharp
using LionFire.Reactive;

// Resource only created when someone subscribes
var onDemandData = IObservableX.CreateConnectOnDemand<Item, int>(
    keySelector: item => item.Id,
    resourceFactory: cache =>
    {
        Console.WriteLine("🟢 First subscriber - starting expensive operation");

        // Start polling, file watching, network connection, etc.
        var cts = new CancellationTokenSource();
        var task = Task.Run(async () =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                var items = await FetchItemsAsync();
                cache.AddOrUpdate(items);
                await Task.Delay(TimeSpan.FromSeconds(5), cts.Token);
            }
        });

        // Return cleanup function
        return Disposable.Create(() =>
        {
            Console.WriteLine("🔴 Last subscriber disposed - stopping operation");
            cts.Cancel();
        });
    }
);

// No resources allocated yet
var subscription = onDemandData.Subscribe(items => UpdateUI(items));
// 🟢 NOW resources start (logged above)

subscription.Dispose();
// 🔴 Resources cleaned up (logged above)
```

**Key Points**:
- Resources only created when first subscriber connects
- Automatic cleanup when last subscriber disconnects
- Shared among multiple subscribers (RefCount pattern)
- Perfect for file watchers, network connections, polling

---

## Core Concepts

### 1. Reactive vs. Imperative

**Imperative (Push-based, Manual)**:
```csharp
// ❌ Manual change detection
while (true)
{
    var files = Directory.GetFiles("./data");
    foreach (var file in files)
    {
        if (HasChanged(file))
        {
            var data = LoadFile(file);
            UpdateUI(data);
        }
    }
    await Task.Delay(1000);
}
```

**Reactive (Pull-based, Automatic)**:
```csharp
// ✅ Automatic change detection
fileWatcher.Connect()
    .Subscribe(changeSet => UpdateUI(changeSet));
```

**Benefits**:
- Changes flow automatically
- No polling loops
- Composable transformations
- Automatic cleanup

---

### 2. Observable Collections (DynamicData)

LionFire uses **DynamicData** for all observable collections:

```csharp
// IObservableCache<TValue, TKey> - cached collection with change notifications
IObservableCache<BotConfig, string> configs = ...;

// Subscribe to changes
configs.Connect()
    .Filter(c => c.Enabled)           // Only enabled bots
    .Transform(c => new BotVM(c))     // Transform to ViewModels
    .DisposeMany()                    // Dispose VMs when removed
    .Bind(out var botVMs)             // Bind to ObservableCollection
    .Subscribe();

// botVMs is ReadOnlyObservableCollection<BotVM>
// Automatically updates when files change!
```

**Key Features**:
- Batched change notifications (`IChangeSet<T, TKey>`)
- Rich operators: Filter, Transform, Sort, Group, Merge
- Efficient (only processes changes, not entire collection)
- Integrates with ReactiveUI

---

### 3. On-Demand Activation Pattern

```
No Subscribers
    ↓ State: Inactive (no resources allocated)
First Subscriber
    ↓ onFirstSubscribe() → Start file watcher, network connection, etc.
    ↓ State: Active
Data Flows to All Subscribers
    ↓ RefCount = N (shared subscription)
Last Subscriber Disposes
    ↓ onLastDispose() → Stop file watcher, close connections
    ↓ State: Inactive
```

**Implementation**: `PublishRefCountWithEvents`, `ConnectOnDemand`

**Use Cases**:
- File system watchers (don't watch when UI closed)
- Network connections (connect only when needed)
- Database queries (poll only when data displayed)
- Expensive computations (compute only when observed)

---

### 4. Runner Pattern for Lifecycle

Runners observe configuration and manage stateful components:

```
Configuration Observable (IObservable<TConfig>)
    ↓ OnNext(config)
Check IsEnabled(config)
    ↓ If true and not running
Start(config)
    ↓ Running
OnNext(newConfig) again
    ↓ If still enabled
OnConfigurationChange(newConfig)
    ↓ If now disabled
Stop()
    ↓ Stopped
```

**Key Types**:
- `Runner<TValue, TRunner>` - Base class
- `WorkerStatus` - Reactive status tracking
- `RunnerState` - Unspecified, Starting, Running, Faulted

**See**: [Runner Pattern Architecture](../architecture/reactive/runner-pattern.md)

---

### 5. Reactive Persistence

Observable key-value stores that react to underlying data changes:

```csharp
public interface IObservableReader<TKey, TValue>
{
    // Metadata cache (lightweight)
    IObservableCache<TMetadata, TKey> Keys { get; }

    // Value cache (triggers deserialization)
    IObservableCache<Optional<TValue>, TKey> Values { get; }

    // Start watching
    IDisposable ListenAllKeys();
    ValueTask<IDisposable> ListenAllValues();
}

public interface IObservableWriter<TKey, TValue>
{
    ValueTask Write(TKey key, TValue value);
    ValueTask<bool> Remove(TKey key);

    // Auto-save patterns
    IDisposable Synchronize(IReactiveObject source, TKey key, ...);
}
```

**Key Features**:
- Keys ≠ Values (lazy loading)
- File system watching
- Auto-save via `Synchronize()`
- Observable write operations

**See**: [Reactive Persistence Architecture](../architecture/reactive/reactive-persistence.md)

---

## Documentation Structure

### **[DynamicData Guide](dynamic-data.md)** (Planned)
Deep dive into DynamicData usage in LionFire.

**Topics**:
- `IObservableCache<T, TKey>` fundamentals
- Operators: Filter, Transform, Sort, Group
- `IChangeSet<T, TKey>` change notifications
- Bind() to UI collections
- Performance optimization

---

### **[File Watching Guide](file-watching.md)** (Planned)
File system watching patterns.

**Topics**:
- `ObservableFileInfos` - File metadata watching
- `ObservableFsDocuments` - Document collections
- Polling vs. FileSystemWatcher
- Custom deserialization
- Error handling

---

### **[On-Demand Resources](on-demand-resources.md)** (Planned)
On-demand resource activation patterns.

**Topics**:
- RefCount pattern
- `PublishRefCountWithEvents`
- `ConnectOnDemand`
- Resource lifecycle
- Performance benefits

---

### **[Runner Lifecycle](runner-lifecycle.md)** (Planned)
Runner pattern for configuration-driven components.

**Topics**:
- Implementing `Runner<TValue, TRunner>`
- `IsEnabled()` predicate
- Start/Stop lifecycle
- Hot-reload support
- Fault tracking
- Integration with workspace documents

---

## Common Patterns

### Pattern 1: Reactive File-Backed Collection

```csharp
// Directory of JSON config files
var configs = ObservableFsDocuments.Create<BotConfig>(
    dir: "./bots",
    deserialize: bytes => JsonSerializer.Deserialize<BotConfig>(bytes)!
).AsObservableCache();

// React to changes
configs.Connect()
    .Filter(c => c.Enabled)
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            if (change.Reason == ChangeReason.Add)
                Console.WriteLine($"New bot: {change.Current.Name}");
            else if (change.Reason == ChangeReason.Remove)
                Console.WriteLine($"Removed bot: {change.Key}");
        }
    });
```

---

### Pattern 2: Transform to ViewModels

```csharp
// Observable entity collection
IObservableReader<string, BotEntity> reader = ...;

// Transform to ViewModels
var botVMs = reader.Values.Connect()
    .Transform(optional => optional.HasValue
        ? new BotVM(optional.Value)
        : null)
    .Filter(vm => vm != null)
    .DisposeMany()  // Dispose VMs when removed
    .Bind(out ReadOnlyObservableCollection<BotVM> collection)
    .Subscribe();

// collection is ObservableCollection that auto-updates!
```

---

### Pattern 3: Auto-Save ViewModel

```csharp
// Writer implementation
IObservableWriter<string, BotConfig> writer = ...;

// Create ViewModel
var botVM = new BotVM(config);

// Auto-save on property changes
var autoSave = writer.Synchronize(
    source: botVM,  // IReactiveObject
    key: "bot-001",
    options: new WritingOptions
    {
        Debounce = TimeSpan.FromSeconds(1)  // Debounce writes
    }
);

// Any change to botVM.Property triggers save after 1 second
botVM.Name = "New Name";  // Automatically saved!

// Cleanup
autoSave.Dispose();
```

---

### Pattern 4: Workspace Document Runner

```csharp
// Reactive document collection
IObservableReader<string, BotConfig> configs = workspaceServices
    .GetService<IObservableReader<string, BotConfig>>();

// Subscribe runner to each config
configs.Values.Connect()
    .Filter(opt => opt.HasValue)
    .Transform(opt => opt.Value!)
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            var config = change.Current;

            if (change.Reason == ChangeReason.Add)
            {
                // Create runner for new config
                var runner = new BotRunner();
                Observable.Return(config)
                    .Merge(GetConfigObservable(change.Key))
                    .Subscribe(runner);
            }
        }
    });
```

---

### Pattern 5: On-Demand API Polling

```csharp
var liveData = IObservableX.CreateConnectOnDemand<StockPrice, string>(
    keySelector: price => price.Symbol,
    resourceFactory: cache =>
    {
        // Only poll when someone is watching
        var cts = new CancellationTokenSource();

        Task.Run(async () =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                var prices = await FetchPricesAsync();
                cache.AddOrUpdate(prices);
                await Task.Delay(TimeSpan.FromSeconds(5), cts.Token);
            }
        });

        return Disposable.Create(() => cts.Cancel());
    }
);

// Subscribe to observe (activates polling)
liveData.Subscribe(prices => UpdatePriceDisplay(prices));
```

---

## Best Practices

### 1. Use On-Demand for Expensive Resources

```csharp
// ✅ Good - Only active when observed
var onDemandFiles = ObservableFileInfos.PollOnDemand(dir);

// ❌ Avoid - Always polling
var timer = Observable.Interval(TimeSpan.FromSeconds(1))
    .SelectMany(_ => PollFiles());
```

---

### 2. Dispose Subscriptions

```csharp
// ✅ Good - Proper cleanup
var subscription = observable.Subscribe(...);
// Later:
subscription.Dispose();

// ✅ Good - Automatic cleanup
var compositeDisposable = new CompositeDisposable();

observable1.Subscribe(...).DisposeWith(compositeDisposable);
observable2.Subscribe(...).DisposeWith(compositeDisposable);

// Dispose all at once
compositeDisposable.Dispose();
```

---

### 3. Use DynamicData Operators

```csharp
// ✅ Good - Efficient change tracking
collection.Connect()
    .Filter(item => item.IsActive)
    .Transform(item => new ItemVM(item))
    .Subscribe(...);

// ❌ Avoid - No change tracking
collection.Connect()
    .Select(changeSet => changeSet
        .Where(c => c.Current.IsActive)
        .Select(c => new ItemVM(c.Current)))
    .Subscribe(...);
```

---

### 4. Handle Errors in Observables

```csharp
// ✅ Good - Graceful error handling
observable
    .Catch<Data, Exception>(ex =>
    {
        LogError(ex);
        return Observable.Empty<Data>();
    })
    .Subscribe(...);

// ✅ Good - Retry on failure
observable
    .Retry(3)
    .Subscribe(...);
```

---

### 5. Use RefCount for Shared Subscriptions

```csharp
// ✅ Good - One subscription shared by many
var shared = expensiveObservable
    .PublishRefCountWithEvents(
        onFirstSubscribe: StartExpensiveOperation,
        onLastDispose: StopExpensiveOperation
    );

// Multiple subscribers share one operation
shared.Subscribe(sub1);
shared.Subscribe(sub2);
shared.Subscribe(sub3);
```

---

## Reactive Layers in LionFire

### LionFire.Reactive

**Purpose**: Reactive utilities and patterns.

**Key Components**:
- `IObservableReader/Writer` - Reactive persistence
- `ObservableFileInfos` - File watching
- `ObservableFsDocuments` - Document collections
- `Runner<TValue, TRunner>` - Lifecycle management
- Observable extensions

**See**: [LionFire.Reactive/CLAUDE.md](../../src/LionFire.Reactive/CLAUDE.md)

---

### ReactiveUI Integration

**Purpose**: Foundation reactive framework.

**Key Features**:
- `ReactiveObject` - Property change notifications
- `[Reactive]` attribute - Source-generated properties
- `WhenAnyValue` - Property observables
- `ReactiveCommand` - Commands with can-execute

**Documentation**: https://www.reactiveui.net/

---

### DynamicData Integration

**Purpose**: Reactive collections.

**Key Features**:
- `IObservableCache<T, TKey>` - Observable cache
- `IChangeSet<T, TKey>` - Change notifications
- Transform, Filter, Sort operators
- RefCount patterns

**Documentation**: https://github.com/reactivemarbles/DynamicData

---

## Integration with Other Systems

### With Workspaces

```
Workspace Document File (bot-001.json)
    ↓ File system watcher (ObservableFsDocuments)
IObservableReader<string, BotConfig>
    ↓ Values.Connect()
WorkspaceDocumentService
    ↓ For each config
BotRunner
    ↓ Start/Stop based on config.Enabled
Bot Instance
```

---

### With MVVM

```
IObservableReader<TKey, TValue>
    ↓ Wrapped by
ObservableReaderItemVM<TKey, TValue, TVM>
    ↓ Used by
Blazor Component
    ↓ Subscribes
vm.WhenAnyValue(x => x.Value)
    ↓ Triggers
StateHasChanged()
```

---

### With Async Data

```
IValue<T> / IGetter<T>
    ↓ Reactive Implementation
ValueRxO<T> / GetterRxO<T>
    ↓ Provides
IObservableGetOperations
IObservableGetResults
    ↓ Consumed by
Subscribers
```

---

## When to Use Reactive Patterns

| Scenario | Pattern | Why |
|----------|---------|-----|
| **File-backed collection** | `ObservableFsDocuments` | Auto-updates on file changes |
| **Config-driven service** | `Runner<T, TR>` | Start/stop based on config |
| **Expensive operation** | `ConnectOnDemand` | Only active when observed |
| **Transform entities to VMs** | `Transform()` | Automatic VM lifecycle |
| **Filter active items** | `Filter()` | Efficient change tracking |
| **Auto-save ViewModel** | `Synchronize()` | Property change → persist |
| **Shared subscription** | `PublishRefCount` | One source, many subscribers |
| **File watching** | `ObservableFileInfos` | Detect file adds/updates/removes |

---

## Related Documentation

### Architecture
- **[Reactive Architecture](../architecture/reactive/README.md)** - High-level reactive architecture
- **[Reactive Persistence](../architecture/reactive/reactive-persistence.md)** - IObservableReader/Writer
- **[Runner Pattern](../architecture/reactive/runner-pattern.md)** - Lifecycle management

### Domain Guides
- **[MVVM Reactive Patterns](../mvvm/reactive-patterns.md)** - Reactive patterns in MVVM
- **[Reactive UI Updates](../ui/reactive-ui-updates.md)** - UI update flow
- **[Observable Operations](../data/async/observable-operations.md)** - Observable async operations

### Project Documentation
- **[LionFire.Reactive](../../src/LionFire.Reactive/CLAUDE.md)** - Complete API reference
- **[LionFire.Data.Async.Reactive](../../src/LionFire.Data.Async.Reactive/CLAUDE.md)** - ReactiveUI implementations

---

## Summary

**Reactive Patterns** in LionFire provide:

**Core Patterns**:
- **Reactive Persistence** - `IObservableReader/Writer` for file-backed data
- **On-Demand Activation** - Resources created only when subscribed
- **Runner Pattern** - Configuration-driven lifecycle management
- **File Watching** - `ObservableFileInfos`, `ObservableFsDocuments`
- **DynamicData Collections** - Change-tracked observable collections

**Key Benefits**:
- Automatic propagation of changes
- Optimal performance (on-demand resources)
- Clean lifecycle management (automatic cleanup)
- Composable transformations
- Efficient change tracking (batched changesets)

**Foundation**: Built on **ReactiveUI** and **DynamicData** for battle-tested reactive primitives.

**Most Common Use Cases**:
1. File-backed workspace documents with auto-reload
2. Configuration-driven service start/stop (runners)
3. Transform entity collections to ViewModels
4. Auto-save ViewModels on property change

**Next Steps**:
1. Explore [Reactive Architecture](../architecture/reactive/README.md) for design decisions
2. Read [Reactive Persistence](../architecture/reactive/reactive-persistence.md) for IObservableReader/Writer
3. Study [Runner Pattern](../architecture/reactive/runner-pattern.md) for lifecycle management
