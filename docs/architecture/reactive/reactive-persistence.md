# Reactive Persistence Patterns

## Overview

This document covers **IObservableReader/Writer** patterns - LionFire's reactive persistence abstraction that provides observable, cached access to key-value stores with automatic change detection.

**Key Concept**: Persistent data (files, databases) exposed as observable caches that automatically update when underlying data changes.

---

## Table of Contents

1. [IObservableReader Pattern](#iobservablereader-pattern)
2. [IObservableWriter Pattern](#iobservablewriter-pattern)
3. [On-Demand Loading](#on-demand-loading)
4. [File System Implementation](#file-system-implementation)
5. [Database Implementation](#database-implementation)
6. [RefCount and Resource Management](#refcount-and-resource-management)
7. [Performance Characteristics](#performance-characteristics)

---

## IObservableReader Pattern

### Interface Definition

```csharp
public interface IObservableReader<TKey, TValue>
    where TKey : notnull
{
    // Lightweight key metadata
    IObservableCache<TMetadata, TKey> Keys { get; }

    // Full values (triggers deserialization)
    IObservableCache<Optional<TValue>, TKey> Values { get; }
    IObservableCache<Optional<TValue>, TKey> ObservableCache { get; }  // Alias

    // Start watching
    IDisposable ListenAllKeys();
    ValueTask<IDisposable> ListenAllValues();

    // Get specific value
    ValueTask<Optional<TValue>> TryGetValue(TKey key);

    // Get observable for specific key
    IObservable<TValue?> GetValueObservable(TKey key);
    IObservable<Optional<TValue>>? GetValueObservableIfExists(TKey key);
}
```

---

### Key Concepts

#### Keys vs Values

**Keys Cache** (Lightweight):
- Metadata only (e.g., file names, modification times)
- No deserialization
- Fast enumeration
- Always loaded

**Values Cache** (On-Demand):
- Full entities after deserialization
- Loaded only when observed
- Memory-intensive
- Lazy by default

**Example**:
```csharp
IObservableReader<string, BotEntity> reader;

// Keys are available immediately (file names)
Console.WriteLine($"Found {reader.Keys.Count} bot files");
foreach (var key in reader.Keys.Items)
{
    Console.WriteLine($"  - {key}");  // Just file names
}

// Values loaded on demand
reader.ListenAllValues();  // Triggers deserialization

foreach (var valueOptional in reader.Values.Items)
{
    if (valueOptional.HasValue)
    {
        Console.WriteLine($"Bot: {valueOptional.Value.Name}");  // Full entity
    }
}
```

---

### Usage Patterns

#### Pattern 1: List All Keys

```csharp
IObservableReader<string, Config> configReader;

// Subscribe to keys (no value loading)
configReader.Keys.Connect()
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            Console.WriteLine($"Config file: {change.Key} ({change.Reason})");
        }
    });

// Keys are available immediately
var configFiles = configReader.Keys.Items;  // ["app.json", "user.json"]
```

---

#### Pattern 2: Load Specific Value

```csharp
// Get single value
var result = await reader.TryGetValue("bot-001");

if (result.HasValue)
{
    Console.WriteLine($"Bot: {result.Value.Name}");
}
else
{
    Console.WriteLine("Bot not found");
}
```

---

#### Pattern 3: Subscribe to Specific Value

```csharp
// Watch specific item for changes
var observable = reader.GetValueObservable("bot-001");

observable
    .WhereNotNull()
    .Subscribe(bot =>
    {
        Console.WriteLine($"Bot updated: {bot.Name}");
    });
```

---

#### Pattern 4: Subscribe to All Values

```csharp
// Load and watch all values
reader.ListenAllValues();

reader.Values.Connect()
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            if (change.Current.HasValue)
            {
                var entity = change.Current.Value;
                Console.WriteLine($"{change.Key}: {change.Reason} - {entity.Name}");
            }
        }
    });
```

---

## IObservableWriter Pattern

### Interface Definition

```csharp
public interface IObservableWriter<TKey, TValue>
    where TKey : notnull
{
    // Write operations
    ValueTask Write(TKey key, TValue value);
    ValueTask<bool> Remove(TKey key);

    // Observable write operations
    IObservable<WriteOperation<TKey, TValue>> WriteOperations { get; }

    // Auto-save patterns
    IDisposable Synchronize(
        IObservable<TValue> source,
        TKey key,
        TimeSpan? throttle = null);

    IDisposable Synchronize(
        IReactiveNotifyPropertyChanged<IReactiveObject> source,
        TKey key,
        TimeSpan? throttle = null);
}
```

---

### Write Operations

#### Basic Write

```csharp
IObservableWriter<string, BotEntity> writer;

var bot = new BotEntity { Name = "New Bot", Enabled = true };

// Write to storage
await writer.Write("bot-001", bot);
// File created: workspace/Bots/bot-001.hjson
```

---

#### Delete

```csharp
// Remove from storage
bool removed = await writer.Remove("bot-001");
// File deleted: workspace/Bots/bot-001.hjson
```

---

### Auto-Save Patterns

#### Synchronize Observable Stream

```csharp
// Auto-save when observable emits
IObservable<BotEntity> botChanges = ...;

var subscription = writer.Synchronize(
    source: botChanges,
    key: "bot-001",
    throttle: TimeSpan.FromSeconds(2)  // Debounce saves
);

// Bot changes automatically saved with 2-second debounce
```

---

#### Synchronize ReactiveObject

```csharp
// Auto-save reactive entity
BotEntity bot = new BotEntity();  // Must be ReactiveObject

var subscription = writer.Synchronize(
    source: bot,
    key: "bot-001",
    throttle: TimeSpan.FromSeconds(1)
);

// Any property change triggers save (after 1 second throttle)
bot.Name = "Updated Name";  // Auto-saves after 1 second
bot.Enabled = true;          // Auto-saves after 1 second
```

**Benefits**:
- Zero boilerplate auto-save
- Automatic debouncing
- Reactive property tracking
- Disposal stops auto-save

---

### Write Operation Tracking

```csharp
writer.WriteOperations.Subscribe(op =>
{
    Console.WriteLine($"Writing {op.Key}...");

    op.Completion.Subscribe(
        onNext: _ => Console.WriteLine($"Write completed: {op.Key}"),
        onError: ex => Console.WriteLine($"Write failed: {ex.Message}")
    );
});

// Trigger write
await writer.Write("config", myConfig);
```

---

## On-Demand Loading

### The Problem

**Naive Approach** - Load everything upfront:
```csharp
// ❌ Loads ALL files immediately
var allBots = await LoadAllBotsFromDirectory();  // Slow!
```

**Issues**:
- Slow startup (deserialize all files)
- High memory usage
- Wasted resources (unused entities loaded)

---

### The Solution - On-Demand

**Only load what's observed**:

```csharp
IObservableReader<string, BotEntity> reader;

// Keys available immediately (file names only)
var keys = reader.Keys.Items;  // Fast!

// Values loaded only when subscribed
reader.Values.Connect()  // NOW deserialization starts
    .Subscribe(changeSet => ...);
```

**Benefits**:
- Fast startup (keys only)
- Low memory (values on-demand)
- Efficient (only deserialize observed items)

---

### Implementation: ConnectOnDemand

```csharp
// Create observable that activates on subscription
var observable = sourceCache.ConnectOnDemand(
    resourceFactory: cache =>
    {
        // START file watching when first subscriber connects
        var watcher = new FileSystemWatcher(directory);
        watcher.Changed += (s, e) => HandleFileChange(e, cache);
        watcher.EnableRaisingEvents = true;

        // CLEANUP when last subscriber disconnects
        return Disposable.Create(() =>
        {
            watcher.Dispose();
            Console.WriteLine("File watching stopped");
        });
    }
);

// File watching starts ONLY when someone subscribes
var subscription = observable.Subscribe(...);

// File watching stops when subscription disposed
subscription.Dispose();
```

---

## File System Implementation

### HjsonFsDirectoryReaderRx\<TKey, TValue\>

**Purpose**: File system-backed `IObservableReader` with HJSON deserialization.

**Architecture**:

```
Directory: workspace1/Bots/
    ↓
FileSystemWatcher / Polling
    ↓ Detects file changes
FileInfo Observable
    ↓ Filter *.hjson files
HjsonFsDirectoryReaderRx
    ↓ Lazy deserialization
SourceCache<Optional<BotEntity>, string>
    ↓ Connect()
Subscribers
```

**Key Features**:
- On-demand file watching
- Lazy deserialization
- Automatic cache updates
- RefCount pattern (stops watching when no subscribers)

---

### Implementation Details

```csharp
public class HjsonFsDirectoryReaderRx<TKey, TValue> : IObservableReader<TKey, TValue>
{
    private readonly SourceCache<Optional<TValue>, TKey> valueCache;
    private readonly SourceCache<FileMetadata, TKey> keyCache;

    public IObservableCache<FileMetadata, TKey> Keys => keyCache.AsObservableCache();
    public IObservableCache<Optional<TValue>, TKey> Values => valueCache.AsObservableCache();

    public HjsonFsDirectoryReaderRx(
        IServiceProvider serviceProvider,
        DirectoryReferenceSelector dirSelector)
    {
        // Set up keys (file names) - immediate
        var fileInfos = ObservableFileInfos.PollOnDemand(
            dirSelector.Path,
            searchPattern: "*.hjson"
        );

        fileInfos
            .Transform(fi => new FileMetadata(fi.Name, fi.LastWriteTime))
            .PopulateInto(keyCache);

        // Set up values (deserialized) - on-demand
        // Only loads when Values.Connect() called
    }

    public async ValueTask<Optional<TValue>> TryGetValue(TKey key)
    {
        // Check cache first
        var cached = valueCache.Lookup(key);
        if (cached.HasValue)
            return cached.Value;

        // Load from file
        var filePath = GetFilePath(key);
        if (!File.Exists(filePath))
            return Optional.None<TValue>();

        var hjson = await File.ReadAllTextAsync(filePath);
        var entity = HjsonValue.Load(hjson).ToObject<TValue>();

        // Update cache
        valueCache.AddOrUpdate(Optional.Some(entity), key);

        return Optional.Some(entity);
    }
}
```

---

## RefCount and Resource Management

### RefCount Pattern

**Problem**: Multiple components subscribe to same observable.

**Naive Solution**: Each creates own subscription.
```csharp
// ❌ Creates 3 file watchers!
component1.Subscribe(reader.Values.Connect());
component2.Subscribe(reader.Values.Connect());
component3.Subscribe(reader.Values.Connect());
```

**RefCount Solution**: Share one subscription.
```csharp
var shared = reader.Values.Connect()
    .RefCount();  // Shares subscription

// All share same file watcher
component1.Subscribe(shared);
component2.Subscribe(shared);
component3.Subscribe(shared);
```

**Timeline**:
```
No Subscribers
    ↓
Component1 subscribes → File watching starts
    ↓
Component2 subscribes → (reuses same file watcher)
    ↓
Component3 subscribes → (reuses same file watcher)
    ↓
Component1 unsubscribes → (file watcher continues)
    ↓
Component2 unsubscribes → (file watcher continues)
    ↓
Component3 unsubscribes → File watching stops
```

---

### PublishRefCountWithEvents

**Enhanced RefCount with lifecycle callbacks**:

```csharp
var shared = reader.Values.Connect()
    .PublishRefCountWithEvents(
        onFirstSubscribe: () =>
        {
            Console.WriteLine("First subscriber - activating resources");
            StartExpensivePolling();
        },
        onLastDispose: () =>
        {
            Console.WriteLine("Last subscriber - cleanup");
            StopExpensivePolling();
        }
    );
```

**Use Cases**:
- Start/stop database connections
- Enable/disable polling
- Activate/deactivate file watchers
- Track subscription count

---

## Performance Characteristics

### Memory Usage

**Keys Cache**:
- ~100 bytes per key (file name + metadata)
- 10,000 files = ~1 MB

**Values Cache**:
- Depends on entity size
- Example: 1 KB per entity
- 10,000 entities = ~10 MB
- **Only loaded when subscribed**

**Recommendation**: Use Keys for enumeration, Values only when needed.

---

### Latency

**First Load** (cold cache):
```
File system read:      ~1-5ms per file
Deserialization:       ~1-10ms per file (depends on size)
Cache update:          ~0.1ms
Notification:          ~0.1ms

Total for 100 files:   ~200-1,500ms
```

**Cached Access**:
```
reader.TryGetValue("key"):  ~0.1-1ms (from cache)
```

**Change Detection**:
```
File modified:          0ms
FileSystemWatcher:      ~10-50ms
Deserialization:        ~1-10ms
Cache update:           ~0.1ms
Subscriber notified:    ~0.1ms

Total latency:          ~11-60ms
```

---

## Related Documentation

- **[Reactive Architecture](README.md)** - Overview
- **[Runner Pattern](runner-pattern.md)** - Lifecycle management
- **[Persistence Patterns](../../data/async/persistence.md)** - Domain guide
- **[Workspace Architecture](../workspaces/README.md)** - Workspace usage
- **[LionFire.Reactive](../../../src/LionFire.Reactive/CLAUDE.md)** - Implementation details

---

## Summary

**IObservableReader/Writer** provides:

**Key Features**:
- **Observable Caches** - DynamicData for reactive updates
- **On-Demand Loading** - Keys immediate, values lazy
- **File System Watching** - Automatic change detection
- **RefCount Pattern** - Shared subscriptions
- **Write-Through Caching** - Immediate cache updates

**Performance**:
- Keys: ~1 MB for 10,000 files
- Values: Lazy-loaded, ~10 MB for 10,000 entities
- Change detection: ~11-60ms latency
- Cached access: <1ms

**Most Common Use**: Workspace documents with file-based HJSON persistence.
