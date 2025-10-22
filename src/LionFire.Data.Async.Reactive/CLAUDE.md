# LionFire.Data.Async.Reactive (LionFire.Data.Async.ReactiveUI)

## Overview

ReactiveUI-based implementations of async data access patterns. Provides concrete reactive classes for getters, setters, values, and collections built on DynamicData and ReactiveUI. This is the "RxO" (ReactiveObject) implementation layer.

**Layer**: Toolkit (Implementation)
**Target**: .NET 9.0
**Root Namespace**: `LionFire`
**Project Name**: LionFire.Data.Async.ReactiveUI.csproj

## Key Dependencies

- **ReactiveUI** - Reactive MVVM framework
- **ReactiveUI.SourceGenerators** - `[Reactive]` attribute codegen
- **System.Reactive** - Reactive Extensions
- **System.Reactive.Async** - Async observable support
- **DynamicData** - Reactive collections
- **Hjson** - Configuration file support
- **LionFire.Data.Async.Abstractions** - Core async abstractions
- **LionFire.Data.Async.Reactive.Abstractions** - ReactiveUI abstractions
- **LionFire.Reactive** - Additional reactive utilities
- **LionFire.Resolves** (Data.Async) - Data access implementations

## Core Features

### 1. Reactive Async Collections

Implementations of async-loaded collections with DynamicData integration:

#### AsyncDynamicDataCollection<TValue>

Abstract base for async collections backed by DynamicData:

```csharp
public abstract partial class AsyncDynamicDataCollection<TValue>
    : DynamicDataCollection<TValue>
    where TValue : notnull
{
    public override async ITask<IGetResult<IEnumerable<TValue>>> Get(CancellationToken ct)
    {
        // Thread-safe operation tracking
        lock (getInProgressLock)
        {
            if (getOperations.Value != null && !IsCompleted)
                return getOperations.Value; // Return existing operation

            var task = GetImpl(ct);
            getOperations.OnNext(task);
            return await task;
        }
    }

    protected abstract ITask<IGetResult<IEnumerable<TValue>>> GetImpl(CancellationToken ct);
}
```

**Features:**
- Prevents concurrent Get operations
- Publishes operations to observable stream
- Integrates with DynamicData's SourceCache
- Reactive property notifications

#### AsyncLazyDynamicDataCollection<TValue>

Lazy-loading variant with caching:

```csharp
public abstract class AsyncLazyDynamicDataCollection<TValue>
    : AsyncDynamicDataCollection<TValue>
{
    public async ITask<IGetResult<IEnumerable<TValue>>> GetIfNeeded(CancellationToken ct)
    {
        if (HasValue) return CachedResult;
        return await Get(ct);
    }
}
```

### 2. Keyed Collections

Collections with key-based access:

#### AsyncKeyedCollection<TKey, TValue>

```csharp
public class AsyncKeyedCollection<TKey, TValue>
    : IAsyncKeyedCollection<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    private readonly SourceCache<TValue, TKey> sourceCache;

    public IObservableCache<TValue, TKey> Items => sourceCache.AsObservableCache();

    public async Task<TValue?> GetItem(TKey key, CancellationToken ct)
    {
        await EnsureLoaded(ct);
        var lookup = sourceCache.Lookup(key);
        return lookup.HasValue ? lookup.Value : default;
    }
}
```

**Variants:**
- `AsyncKeyValueCollection<TKey, TValue>` - KeyValuePair-based
- `AsyncReadOnlyKeyedCollection<TKey, TValue>` - Read-only
- `AsyncReadOnlyKeyedFuncCollection<TKey, TValue>` - Function-based transforms

#### AsyncTransformingKeyedCollection<TKey, TSource, TValue>

Transforms source items on-the-fly:

```csharp
public class AsyncTransformingKeyedCollection<TKey, TSource, TValue>
    where TKey : notnull
{
    public AsyncTransformingKeyedCollection(
        IObservableCache<TSource, TKey> source,
        Func<TSource, TValue> transform)
    {
        TransformedCache = source
            .Connect()
            .Transform(transform)
            .AsObservableCache();
    }

    public IObservableCache<TValue, TKey> TransformedCache { get; }
}
```

**Use Cases:**
- Model → ViewModel transformation
- Data projection/mapping
- Filtering/enrichment pipelines

### 3. File System Collections

Async collections backed by file systems:

#### AsyncFileDictionary<TValue>

```csharp
public class AsyncFileDictionary<TValue>
    : AsyncKeyedCollection<string, TValue>
{
    public AsyncFileDictionary(
        string directoryPath,
        Func<string, Task<TValue>> deserialize,
        string searchPattern = "*")
    {
        // Watches directory, deserializes files
    }
}
```

#### SerializingFileDictionary<TValue>

Adds serialization support:

```csharp
public class SerializingFileDictionary<TValue>
    : AsyncFileDictionary<TValue>
    , IObservableWriter<string, TValue>
{
    public async ValueTask Write(string key, TValue value)
    {
        var path = Path.Combine(DirectoryPath, key);
        await File.WriteAllTextAsync(path, Serialize(value));
    }
}
```

**Features:**
- Auto-watch file system changes
- Lazy deserialization
- Write support with serialization
- DynamicData change notifications

### 4. Collection Configuration

#### AsyncObservableCollectionOptions

```csharp
public class AsyncObservableCollectionOptions
{
    public bool AutoInstantiateCollection { get; set; } = true;
    public bool AutoSync { get; set; } = false;
    public bool AlwaysRetrieveOnEnableSync { get; set; } = true;
}
```

**Options:**
- `AutoInstantiateCollection` - Create backing collection automatically
- `AutoSync` - Enable automatic synchronization
- `AlwaysRetrieveOnEnableSync` - Force retrieval when sync enabled

#### AsyncValueState & AsyncValueStatus

State tracking for async values:

```csharp
[Flags]
public enum AsyncValueState
{
    Uninitialized = 0,
    Loading = 1 << 0,
    Loaded = 1 << 1,
    Error = 1 << 2,
    Synchronized = 1 << 3
}

public class AsyncValueStatus : ReactiveObject
{
    [Reactive] private AsyncValueState state;
    [Reactive] private Exception? error;
    [Reactive] private DateTimeOffset? lastUpdated;
}
```

### 5. Observable Creation

#### IObservableCreatesAsync<TValue>

```csharp
public interface IObservableCreatesAsync<TValue>
{
    IObservable<TValue> CreatedItems { get; }
    Task<TValue> CreateAsync(CancellationToken ct = default);
}
```

Stream of newly created items for reactive UIs.

### 6. Collection Interfaces

#### IAsyncCollection<TValue>

```csharp
public interface IAsyncCollection<TValue>
    : IStatelessGetter<IEnumerable<TValue>>
{
    IObservableCache<TValue, int> Items { get; }  // Observable collection
    Task Add(TValue item);
    Task Remove(TValue item);
}
```

Combines async loading with reactive collections.

## Directory Structure

```
Data/Async/
  Collections/
    AsyncDynamicDataCollection.cs
    AsyncLazyDynamicDataCollection.cs
    AsyncObservableCollectionCacheBaseBase.cs
    AsyncObservableCollectionOptions.cs
    AsyncValueState.cs
    AsyncValueStatus.cs

    Collection/
      IAsyncCollection.cs
      IObservableCreatesAsync.cs

      Keyed/
        AsyncKeyedCollection.cs
        AsyncKeyValueCollection.cs
        AsyncReadOnlyKeyedCollection.cs
        AsyncReadOnlyKeyedFuncCollection.cs
        AsyncReadOnlyKeyValuePairCollection.cs
        AsyncTransformingKeyedCollection.cs
        AsyncTransformingDictionary.cs
        AsyncTransformingReadOnlyKeyedCollection.cs
        AsyncTransformingReadOnlyDictionary.cs

        Filesystem/
          AsyncFileDictionary.cs
          SerializingFileDictionary.cs

Values/
  Read/     (placeholder)
  Write/    (placeholder)
```

## Usage Patterns

### Pattern 1: Async File-Based Collection

```csharp
// Create dictionary backed by JSON files
var configFiles = new SerializingFileDictionary<AppConfig>(
    directoryPath: "/configs",
    deserialize: async path => JsonSerializer.Deserialize<AppConfig>(
        await File.ReadAllTextAsync(path)),
    serialize: config => JsonSerializer.Serialize(config),
    searchPattern: "*.json"
);

// Observe changes
configFiles.Items
    .Connect()
    .Subscribe(changes =>
    {
        foreach (var change in changes)
        {
            Console.WriteLine($"{change.Reason}: {change.Key}");
        }
    });

// Add new config (saves to disk)
await configFiles.Write("app.json", new AppConfig { ... });

// Access by key
var appConfig = configFiles.Items.Lookup("app.json");
```

### Pattern 2: Transforming Collections

```csharp
// Source: Models from database
var userModels = new AsyncKeyedCollection<int, UserModel>(...);

// Transform: Model → ViewModel
var userViewModels = new AsyncTransformingKeyedCollection<int, UserModel, UserViewModel>(
    source: userModels.Items,
    transform: model => new UserViewModel(model)
);

// Bind to UI
DataGrid.ItemsSource = userViewModels.TransformedCache
    .Connect()
    .Bind(out var boundCollection)
    .DisposeMany()  // Dispose ViewModels when removed
    .Subscribe();
```

### Pattern 3: Lazy-Loading Collection

```csharp
public class UserCollection : AsyncLazyDynamicDataCollection<User>
{
    protected override async ITask<IGetResult<IEnumerable<User>>> GetImpl(CancellationToken ct)
    {
        var users = await _api.GetUsers(ct);
        return GetResult<IEnumerable<User>>.Success(users);
    }
}

// Usage
var users = new UserCollection();

// First access triggers load
await users.GetIfNeeded();

// Subsequent access returns cached
await users.GetIfNeeded();  // No API call

// Force refresh
users.DiscardValue();
await users.GetIfNeeded();  // API call again
```

### Pattern 4: Keyed Collection with Demand Loading

```csharp
public class DocumentCollection : AsyncKeyedCollection<Guid, Document>
{
    private readonly IDocumentApi api;

    protected override async ITask<IGetResult<IEnumerable<Document>>> GetImpl(CancellationToken ct)
    {
        // Load all document metadata
        var docs = await api.GetAllDocuments(ct);
        return GetResult<IEnumerable<Document>>.Success(docs);
    }

    public async Task<Document?> GetDocumentContent(Guid id, CancellationToken ct)
    {
        // Lazy-load full content for specific document
        var existing = Items.Lookup(id);
        if (existing.HasValue && existing.Value.HasContent)
            return existing.Value;

        var fullDoc = await api.GetDocumentWithContent(id, ct);
        sourceCache.AddOrUpdate(fullDoc);
        return fullDoc;
    }
}
```

### Pattern 5: Read-Only Projections

```csharp
// Source collection
IObservableCache<Product, int> products = ...;

// Read-only transformed view
var productNames = new AsyncReadOnlyKeyedFuncCollection<int, Product, string>(
    source: products,
    transform: product => product.Name
);

// Reactive binding
productNames.Items
    .Connect()
    .Bind(out var namesList)
    .Subscribe();
```

## Design Philosophy

**DynamicData-First:**
- All collections use `IObservableCache<T, TKey>` or `IObservableList<T>`
- Change notifications via `IChangeSet<T, TKey>`
- Efficient incremental updates

**Async-Friendly:**
- All loading operations async
- Concurrent operation protection
- CancellationToken support throughout

**Reactive State:**
- All state changes observable
- ReactiveObject base for property notifications
- Integration with ReactiveUI commands

**Composable:**
- Transform collections without reloading
- Chain multiple transformations
- Combine with DynamicData operators

## Performance Considerations

**Memory:**
- SourceCache holds all items in memory
- Transformations create additional caches
- Use lazy loading for large datasets

**Concurrency:**
- Get operations locked to prevent duplicates
- Thread-safe SourceCache operations
- Async operations don't block UI

**Optimization Tips:**
```csharp
// Good: Single load, multiple transforms
var source = new AsyncKeyedCollection<int, Model>(...);
var transform1 = new AsyncTransformingKeyedCollection<int, Model, VM1>(source.Items, ...);
var transform2 = new AsyncTransformingKeyedCollection<int, Model, VM2>(source.Items, ...);

// Bad: Multiple loads
var source1 = new AsyncKeyedCollection<int, Model>(...);
var source2 = new AsyncKeyedCollection<int, Model>(...); // Duplicate load!
```

## Testing

When testing async collections:
```csharp
// Create test collection
var testCollection = new AsyncDynamicDataCollection<MyData>
{
    GetImpl = ct => Task.FromResult(
        GetResult<IEnumerable<MyData>>.Success(testData))
};

// Test loading
await testCollection.Get();
Assert.True(testCollection.HasValue);

// Test observable
var changes = new List<IChangeSet<MyData>>();
testCollection.Items.Connect().Subscribe(changes.Add);

// Trigger change
await testCollection.Get();
Assert.Equal(1, changes.Count);
```

## Common Pitfalls

**1. Forgetting to Dispose:**
```csharp
// Bad: Memory leak
collection.Items.Connect().Subscribe(...);

// Good: Dispose subscription
var subscription = collection.Items.Connect().Subscribe(...);
// Later:
subscription.Dispose();

// Best: Use DisposeWith
collection.Items.Connect()
    .Subscribe(...)
    .DisposeWith(disposables);
```

**2. Blocking Async:**
```csharp
// Bad: Deadlock risk
var result = collection.Get().AsTask().Result;

// Good: Async all the way
var result = await collection.Get();
```

**3. Not Handling Exceptions:**
```csharp
// Bad: Unhandled exception crashes app
await collection.Get();

// Good: Handle errors
try
{
    await collection.Get();
}
catch (Exception ex)
{
    Logger.LogError(ex, "Failed to load collection");
}
```

## Known Limitations

- No built-in paging support (implement in GetImpl)
- SourceCache holds all items (not suitable for very large datasets)
- File system watching uses polling (from LionFire.Reactive)
- No built-in conflict resolution for concurrent writes

## Related Projects

- **LionFire.Data.Async.Abstractions** - Interfaces implemented here
- **LionFire.Data.Async.Reactive.Abstractions** - RxO marker interfaces
- **LionFire.Data.Async.Mvvm** - ViewModel wrappers for these collections
- **LionFire.Reactive** - Additional reactive utilities (ObservableFsDocuments, etc.)
- **DynamicData** - Underlying reactive collection library

## See Also

- DynamicData documentation: https://github.com/reactivemarbles/DynamicData
- ReactiveUI collections patterns
- IObservableCache usage
- SourceCache vs SourceList
- Transform, Filter, Sort operators
