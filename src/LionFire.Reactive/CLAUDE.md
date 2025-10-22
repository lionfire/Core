# LionFire.Reactive

## Overview

Reactive programming utilities and abstractions built on top of ReactiveUI and DynamicData. Provides reactive patterns for persistence, file system watching, observable collections, and lifecycle management with on-demand resource activation.

**Layer**: Toolkit
**Target**: .NET 9.0
**Root Namespace**: `LionFire`

## Key Dependencies

- **ReactiveUI** - Reactive extensions for .NET
- **DynamicData** - Reactive collections with observable change sets
- **LionFire.Data.Async.Abstractions** - Async data patterns

## Core Features

### 1. Reactive Persistence

Abstractions for observable key-value stores that notify subscribers of changes:

#### IObservableReader<TKey, TValue>

Observable read-only access to a key-value store:

```csharp
public interface IObservableReader<TKey, TValue>
{
    // Observable cache of keys (metadata only)
    IObservableCache<TMetadata, TKey> Keys { get; }

    // Observable cache of values (triggers deserialization)
    IObservableCache<Optional<TValue>, TKey> Values { get; }
    IObservableCache<Optional<TValue>, TKey> ObservableCache { get; }

    // Listen to all keys or values
    IDisposable ListenAllKeys();
    ValueTask<IDisposable> ListenAllValues();

    // Get observable for specific key
    IObservable<TValue?> GetValueObservable(TKey key);
}
```

**Key Concepts:**
- **Keys cache** - Lightweight metadata (e.g., file names) without loading content
- **Values cache** - Full objects, loaded/deserialized on demand
- **ListenAll** - Subscribe to all items (triggers loading)
- **On-demand** - Only load what's observed

#### IObservableWriter<TKey, TValue>

Observable write operations with synchronization support:

```csharp
public interface IObservableWriter<TKey, TValue>
{
    ValueTask Write(TKey key, TValue value);
    ValueTask<bool> Remove(TKey key);

    // Observable stream of write operations
    IObservable<WriteOperation<TKey, TValue>> WriteOperations { get; }

    // Auto-save patterns
    IDisposable Synchronize(IObservable<TValue> source, TKey key, ...);
    IDisposable Synchronize(IReactiveNotifyPropertyChanged<IReactiveObject> source, TKey key, ...);
}
```

**Features:**
- Observable write operations for UI feedback
- `WriteOperation<TKey, TValue>` with completion tracking
- Auto-save via `Synchronize()` methods
- Reactive property change → automatic persistence

#### IObservableReaderWriter<TKey, TValue>

Combined read/write interface:

```csharp
public interface IObservableReaderWriter<TKey, TValue>
    : IObservableReader<TKey, TValue>
    , IObservableWriter<TKey, TValue>
{
}
```

**Extension Methods:**
- `TryGet<TKey, TValue>()` - Synchronous lookup from cache

### 2. File System Watching

Reactive file system monitoring with polling:

#### ObservableFileInfos

Poll directory for file changes:

```csharp
// Create observable of FileInfo changes
var files = ObservableFileInfos.PollOnDemand(
    dir: "/path/to/directory",
    searchPattern: "*.json"
);

files.Subscribe(changeSet =>
{
    foreach (var change in changeSet)
    {
        // ChangeReason: Add, Update, Remove
        var file = change.Current;
    }
});
```

**Features:**
- Polls directory every 1 second
- Detects adds, updates (length/time changed), removes
- Only active when subscribed (on-demand pattern)
- Uses DynamicData's `IChangeSet<FileInfo, string>`

#### ObservableFsDocuments

Reactive file-based document collections:

```csharp
// Static factory
var docs = ObservableFsDocuments.Create<MyData>(
    dir: "/path/to/docs",
    deserialize: bytes => JsonSerializer.Deserialize<MyData>(bytes)
);

// Class-based
var docCollection = new ObservableFsDocuments<MyData>(
    dir: "/path",
    deserializeFile: async path => await LoadAsync(path)
);
var cache = docCollection.ObservableCache;
```

**Features:**
- Combines file watching with deserialization
- Returns `IObservable<IChangeSet<(string key, TValue value), string>>`
- Auto-deserializes files on change
- On-demand activation (polls only when subscribed)

**Note:** Currently uses polling; enhancement planned for `FileSystemWatcher`

### 3. DynamicData Extensions

Utilities for working with DynamicData's SourceCache:

#### SourceCacheX

```csharp
// Thread-safe get-or-add with async factory
var item = await cache.GetOrAddAsync(
    key: "mykey",
    factory: async key => await LoadItemAsync(key),
    semaphore: new SemaphoreSlim(1, 1)
);
```

#### SetAttribute

Helper attribute for marking properties that update SourceCache (implementation details in file).

### 4. Observable Extensions

Powerful extension methods for IObservable:

#### CreateConnectOnDemand

Create observable that manages resources on-demand:

```csharp
var observable = IObservableX.CreateConnectOnDemand<Item, string>(
    keySelector: item => item.Id,
    resourceFactory: cache =>
    {
        // Start polling/loading when first subscriber connects
        var subscription = StartDataSource(cache);

        // Cleanup when last subscriber disconnects
        return Disposable.Create(() => subscription.Dispose());
    }
);
```

**Use Case:** Database connections, file watchers, network streams that should only run when needed.

#### PublishRefCountWithEvents

RefCount with lifecycle callbacks:

```csharp
var shared = source
    .PublishRefCountWithEvents(
        onFirstSubscribe: () => Console.WriteLine("First subscriber!"),
        onLastDispose: () => Console.WriteLine("All unsubscribed!")
    );
```

#### OnAttachEvents

Add subscription tracking to any observable:

```csharp
var tracked = observable.OnAttachEvents(
    onFirstSubscribe: StartResource,
    onLastDispose: StopResource
);
```

**Features:**
- Thread-safe subscription counting
- Lifecycle hooks for resource management
- Useful for lazy initialization and cleanup

### 5. On-Demand Connection Patterns

Extensions for SourceCache with resource management:

#### ConnectOnDemand

```csharp
sourceCache.ConnectOnDemand(
    resourceFactory: cache =>
    {
        // Resource setup when first connection
        return disposeAction;
    },
    predicate: item => item.IsActive,
    suppressEmptyChangeSets: true
);
```

**Key Concepts:**
- **RefCount pattern** - Shares one subscription among many observers
- **Resource lifecycle** - Setup on first subscriber, cleanup on last unsubscribe
- **Observable.Using** - Automatic resource disposal
- **Lazy activation** - Resources only created when needed

### 6. Execution Framework

Reactive lifecycle management for start/stop operations:

#### Runner<TValue, TRunner>

Abstract base for managing stateful, start/stop components:

```csharp
public class MyRunner : Runner<MyConfig, MyRunner>
{
    // Define when configuration is "enabled"
    static bool IRunner<MyConfig>.IsEnabled(MyConfig config)
        => config.Enabled;

    // Start logic
    protected override async ValueTask<bool> Start(
        MyConfig config,
        Optional<MyConfig> oldConfig)
    {
        // Initialize resources with config
        return true;
    }

    // Handle config changes while running
    protected override void OnConfigurationChange(
        MyConfig newConfig,
        Optional<MyConfig> oldConfig)
    {
        // Hot-reload configuration
    }

    // Stop logic
    protected override void Stop(
        Optional<MyConfig> newConfig,
        Optional<MyConfig> oldConfig)
    {
        // Cleanup resources
    }
}

// Usage as IObserver<MyConfig>
var runner = new MyRunner();
configChanges.Subscribe(runner);
```

**Features:**
- Implements `IObserver<TValue>` - feed config changes via observable
- Automatic start/stop based on `IsEnabled()` predicate
- Hot-reload support via `OnConfigurationChange()`
- Fault tracking with `WorkerStatus`
- Error observable: `runner.Errors`

#### WorkerStatus

Reactive status tracking:

```csharp
public class WorkerStatus : ReactiveObject
{
    RunnerState RunnerState { get; set; } // Unspecified, Starting, Running, Faulted

    IObservableCache<(string key, object value), string> Faults { get; }
}
```

**States:**
- `Unspecified` - Not started
- `Starting` - Start() in progress
- `Running` - Successfully started
- `Faulted` - Start() threw exception

**Use Cases:**
- Server processes that start/stop based on configuration
- Feature toggles with hot-reload
- Reactive service management
- Monitoring and diagnostics

## Directory Structure

```
DynamicData_/           - DynamicData extensions
  SourceCacheX.cs       - GetOrAddAsync, etc.
  SetAttribute.cs       - Attribute helpers

Execution/              - Runner framework
  Runner.cs             - Base runner class
  WorkerStatus.cs       - Status tracking

IO/Reactive/            - File system watching
  FileInfoWatchers.cs   - ObservableFileInfos
  ObservableFsDocuments.cs - Document collections

Reactive/
  Observable/           - IObservable extensions
    IObservableX.cs     - CreateConnectOnDemand, PublishRefCountWithEvents
    IConnectableObservableX.cs - Connectable extensions
    OnDemandX.cs        - ConnectOnDemand for SourceCache

  Persistence/          - Reactive persistence abstractions
    Read/               - IObservableReader
    Write/              - IObservableWriter, WriteOperation, WritingOptions
    ReadWrite/          - IObservableReaderWriter
```

## Usage Patterns

### Pattern 1: Reactive File Store

```csharp
// Watch directory, deserialize files, expose as observable cache
var documents = ObservableFsDocuments.Create<JsonDoc>(
    dir: "./data",
    deserialize: bytes => JsonSerializer.Deserialize<JsonDoc>(bytes)!
).AsObservableCache();

// Subscribe to changes
documents.Connect()
    .Subscribe(changes =>
    {
        // React to file adds/updates/deletes
    });

// Access current snapshot
var currentDoc = documents.Lookup("doc1.json");
```

### Pattern 2: Auto-Save ViewModels

```csharp
// Writer implementation
IObservableWriter<string, MyModel> writer = ...;

// Auto-save on property changes
var autoSave = writer.Synchronize(
    source: myViewModel,  // IReactiveObject
    key: "my-model-key",
    options: new WritingOptions { Debounce = TimeSpan.FromSeconds(1) }
);

// Changes to myViewModel properties automatically trigger writes
// Dispose autoSave to stop synchronization
```

### Pattern 3: On-Demand Data Loading

```csharp
// Only load data when someone subscribes
var onDemandData = IObservableX.CreateConnectOnDemand<Item, int>(
    keySelector: item => item.Id,
    resourceFactory: cache =>
    {
        Console.WriteLine("Loading data...");

        // Start background loading
        var cts = new CancellationTokenSource();
        Task.Run(() => LoadDataIntoCache(cache, cts.Token));

        // Return cleanup
        return Disposable.Create(() =>
        {
            Console.WriteLine("Stopping data load");
            cts.Cancel();
        });
    }
);

// No data loaded yet
var subscription = onDemandData.Subscribe(...); // NOW data starts loading
subscription.Dispose(); // Cleanup triggered
```

### Pattern 4: Configuration-Based Service

```csharp
public class WebServerRunner : Runner<WebServerConfig, WebServerRunner>
{
    private WebApplication? app;

    static bool IRunner<WebServerConfig>.IsEnabled(WebServerConfig c)
        => c.Port > 0;

    protected override async ValueTask<bool> Start(
        WebServerConfig config,
        Optional<WebServerConfig> oldConfig)
    {
        app = WebApplication.CreateBuilder().Build();
        app.Urls.Add($"http://localhost:{config.Port}");
        await app.StartAsync();
        return true;
    }

    protected override void OnConfigurationChange(
        WebServerConfig newConfig,
        Optional<WebServerConfig> oldConfig)
    {
        // Can't hot-reload port, would need restart
    }

    protected override void Stop(...)
    {
        app?.StopAsync().Wait();
        app = null;
    }
}

// Use with configuration observable
configChanges.Subscribe(new WebServerRunner());
```

## Design Philosophy

**On-Demand Activation:**
- Resources (file watchers, network connections) only active when observed
- RefCount pattern for shared subscriptions
- Automatic cleanup when unsubscribed

**Reactive Persistence:**
- Observable key-value stores
- Lazy loading (keys ≠ values)
- Auto-save patterns for ViewModels

**DynamicData-First:**
- All collections use `IObservableCache<TObject, TKey>`
- Change sets (`IChangeSet<T, K>`) for efficient updates
- Integrates seamlessly with ReactiveUI

**Lifecycle Management:**
- Runner framework for start/stop logic
- Configuration-driven activation
- Fault tracking and error observables

## Common Patterns

### Lazy Resource Pattern

```csharp
// Resource only created when first subscriber connects
var lazy = Observable.Defer(() => ExpensiveOperation())
    .PublishRefCountWithEvents(
        onFirstSubscribe: () => Console.WriteLine("Initializing..."),
        onLastDispose: () => Console.WriteLine("Cleaning up...")
    );
```

### Cache Synchronization

```csharp
// Keep two caches in sync
var derived = source.Connect()
    .Transform(item => new DerivedItem(item))
    .DisposeMany() // Dispose items on removal
    .AsObservableCache();
```

### Polling with Backoff

File polling currently hardcoded at 1 second. For custom intervals:

```csharp
// Custom polling observable
Observable.Interval(TimeSpan.FromSeconds(5))
    .SelectMany(_ => Observable.FromAsync(PollDataAsync))
    .Subscribe(cache.AddOrUpdate);
```

## Performance Considerations

- File polling every 1 second per directory (customization pending)
- `EditDiff()` efficiently compares FileInfo by length/time
- On-demand pattern prevents unnecessary resource allocation
- Thread-safe GetOrAddAsync prevents duplicate async operations
- RefCount pattern shares subscriptions

## Known Limitations

- File watching uses polling, not `FileSystemWatcher` (enhancement planned)
- ObservableFsDocuments uses `.Result` for async deserialization (blocking)
- WritingOptions debouncing not fully implemented
- No built-in retry logic for failed writes

## Testing

When testing reactive components:
- Use `TestScheduler` from ReactiveUI for time-based testing
- Mock `IObservableReader/Writer` interfaces
- Verify `onFirstSubscribe`/`onLastDispose` called correctly
- Test error paths via `OnError()` and fault tracking

## Related Projects

- **LionFire.Mvvm** - Uses reactive patterns for ViewModels
- **LionFire.Data.Async.ReactiveUI** - Reactive handles and persistence
- **LionFire.Vos** - VOS uses reactive patterns for virtual file system
- **LionFire.Execution** - Execution state machines using runners

## See Also

- ReactiveUI: https://www.reactiveui.net/
- DynamicData: https://github.com/reactivemarbles/DynamicData
- Reactive Extensions: http://reactivex.io/
- LionFire persistence abstractions in main CLAUDE.md
