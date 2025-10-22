# LionFire.Data.Async.Abstractions

## Overview

Core abstractions for asynchronous data access patterns. Defines interfaces for getting, setting, and managing async values with support for caching, lazy loading, polling, and observable operations. This is the foundation layer for LionFire's async data access system.

**Layer**: Base/Toolkit (Abstractions)
**Target**: .NET 9.0
**Root Namespace**: `LionFire`

## Key Dependencies

- **MorseCode.ITask** - Covariant `ITask<T>` interface (alternative to `Task<T>`)
- **System.Reactive** - IObservable support
- **LionFire.Data.Abstractions** (LionFire.Resolves.Abstractions project) - Synchronous data abstractions

## Core Concepts

This library defines a comprehensive set of interfaces for async data operations, organized into several key patterns:

### 1. Getter Pattern (Read Operations)

Interfaces for reading/retrieving data asynchronously:

#### IGetter (Marker Interface)
```csharp
public interface IGetter { }  // Marker for objects that can be retrieved
```

#### IStatelessGetter<TValue>
```csharp
public interface IStatelessGetter<out TValue> : IGetter
{
    ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default);
}
```

**Characteristics:**
- Stateless: Always fetches fresh data
- No caching: Each call performs a new retrieval
- Covariant: `out TValue` for flexible usage
- Returns `IGetResult<TValue>` with success/error/value information

#### ILazyGetter
```csharp
public interface ILazyGetter : IDefaultable, IDiscardableValue, IDiscardable
{
    // Lazily gets the object - avoids retrieval if already known
    // To force retrieval: DiscardValue() then Get()
}
```

**Characteristics:**
- Stateful: Caches retrieved value
- Lazy loading: Only retrieves when needed
- Explicit discard to force refresh
- Implements `IDiscardableValue` for cache invalidation

#### IGetter<TValue> (Extended Interface)

Combines multiple getter patterns:
```csharp
public interface IGetter<out TValue>
    : IStatelessGetter<TValue>     // Always retrieves
    , ILazyGetter<TValue>           // Cached lazy retrieval
    , IObservableGetOperations<TValue>  // Observable operations
    , IObservableGetState           // Observable state (IsLoading, etc.)
    , IObservableGetResults<TValue> // Observable results
{
    TValue? ReadCacheValue { get; }  // Synchronous cache access
    bool HasValue { get; }            // Cache validity

    ITask<IGetResult<TValue>> GetIfNeeded(CancellationToken = default);
}
```

**Key Methods:**
- `Get()` - Always retrieves (stateless)
- `GetIfNeeded()` - Retrieves only if not cached (lazy)
- `ReadCacheValue` - Synchronous cache read
- `DiscardValue()` - Invalidate cache

#### Read State Tracking

```csharp
[Flags]
public enum ReadState
{
    Unspecified = 0,
    Unloaded = 1 << 0,      // Not yet loaded
    Loading = 1 << 1,       // Retrieval in progress
    Loaded = 1 << 2,        // Retrieved once
    Synchronized = 1 << 3   // Kept in sync (e.g., polling)
}
```

### 2. Setter Pattern (Write Operations)

Interfaces for writing data asynchronously:

#### ISetter<TValue>
```csharp
public interface ISetter<TValue>
{
    Task<ISetResult<TValue>> Set(TValue? value, CancellationToken cancellationToken = default);
}
```

**Features:**
- Async write operation
- Returns `ISetResult<TValue>` with success/error information
- May trigger persistence to underlying data store

#### Observable Write Operations

```csharp
public interface IObservableSetOperations
{
    IObservable<ITask> SetOperations { get; }  // Stream of write operations
}

public interface IObservableSetState
{
    bool IsSetting { get; }  // Write in progress
}

public interface IObservableSetResults<out TValue>
{
    IObservable<ISetResult<TValue>> SetResults { get; }  // Stream of results
}
```

### 3. Value Pattern (Combined Read/Write)

#### IStatelessAsyncValue<T>
```csharp
public interface IStatelessAsyncValue<T>
    : IStatelessGetter<T>
    , ISetter<T>
{
    // Combined synchronous read/write
}
```

#### IValue<T>
```csharp
public interface IValue<T> : IStatelessAsyncValue<T>
{
    // Full-featured read/write with caching, observables, etc.
}
```

### 4. Observable Patterns

Interfaces for reactive data access:

#### IObservableGetOperations<TValue>
```csharp
public interface IObservableGetOperations<out TValue>
{
    IObservable<ITask<IGetResult<TValue>>> GetOperations { get; }
}
```

Stream of get operations as they occur - useful for UI loading indicators.

#### IObservableGetState
```csharp
public interface IObservableGetState
{
    bool IsLoading { get; }  // Read operation in progress
}
```

#### IObservableGetResults<TValue>
```csharp
public interface IObservableGetResults<out TValue>
{
    IObservable<IGetResult<TValue>?> GetResults { get; }
}
```

Stream of get results - useful for reactive UIs.

### 5. Configuration & Options

#### GetterOptions
```csharp
public class GetterOptions
{
    public TriggerMode AutoGetMode { get; set; }
    // Caching, throttling, batching options
}
```

#### ValueOptions
```csharp
public class ValueOptions
{
    public GetterOptions Get { get; set; }
    public SetterOptions Set { get; set; }
    // Combined read/write configuration
}
```

#### TriggerMode
```csharp
[Flags]
public enum TriggerMode
{
    Disabled = 1 << 0,          // No automatic operations
    Once = 1 << 1,              // Trigger once
    Manual = 1 << 2,            // Manual triggering only
    Auto = 1 << 3,              // Auto with buffering/throttling
    AutoImmediate = 1 << 4      // Auto without delays
}
```

**Usage Examples:**
- `Auto` for Get: Buffered/throttled retrieval
- `AutoImmediate` for Set: Immediate save on change (e.g., typing)
- `Manual` for Set: Explicit save button required

### 6. Specialized Interfaces

#### ICachingGets<TValue>
```csharp
public interface ICachingGets<TValue> : IStatelessGetter<TValue>
{
    // Explicit caching support
}
```

#### IDetects (Existence Check)
```csharp
public interface IDetects
{
    Task<bool?> Exists(CancellationToken cancellationToken = default);
}
```

Check if data exists without retrieving full value.

#### IGetsOrInstantiates<TValue>
```csharp
public interface IGetsOrInstantiates<TValue>
    : IGetter<TValue>
{
    // Get existing value or create default instance
}
```

Useful for ensuring non-null values in UI scenarios.

### 7. Async Objects

#### IAsyncObject
```csharp
public interface IAsyncObject
{
    ObjectOptions Options { get; }
}
```

Base interface for configurable async data objects.

#### ObjectOptions
```csharp
public class ObjectOptions
{
    public ValueOptions Value { get; set; }
    // Collection options, polling options, etc.
}
```

### 8. Polling Support

#### AsyncPoller<TValue>
```csharp
public class AsyncPoller<TValue> : IDisposable
{
    public AsyncPoller(Func<Task<TValue>> poll, TimeSpan interval);
    // Periodically invokes poll function
}
```

**Use Cases:**
- Keep data synchronized with external changes
- Implement refresh for non-reactive data sources
- Fallback when change notifications unavailable

### 9. Collection Interfaces

Interfaces for async collections (keyed/unkeyed):

- `IGetsSync<TKey, TValue>` - Synchronous collection access
- `IGetsOrCreatesByType` - Type-based instantiation
- `ICreatesAsync<TKey, TValue>` - Async item creation
- `IObservableCreatesAsync` - Observable creation operations

## Directory Structure

```
Data/Async/
  Objects/              - IAsyncObject base
  Polling/              - AsyncPoller
  Read/                 - Getter interfaces
    Lazy/               - ILazyGetter, ReadState
  Write/                - Setter interfaces
    Creates/            - Creation interfaces
    Instantiates/       - Instantiation for Set
  ReadWrite/            - Combined IValue interfaces
  Values/               - Single-value interfaces
    Read/               - IGetter, IStatelessGetter, IDetects
    Write/              - ISetter, observable write interfaces
    ReadWrite/          - IValue, IStatelessAsyncValue
  Sync/                 - Synchronous variants
  TriggerMode.cs        - Trigger configuration enum
  GetterOrSetterOptions.cs
```

## Design Patterns

### Lazy vs Stateless

**Stateless (IStatelessGetter):**
```csharp
// Always retrieves fresh data
var result = await getter.Get();
```

**Lazy (ILazyGetter via IGetter):**
```csharp
// First call retrieves, subsequent calls use cache
var result1 = await getter.GetIfNeeded(); // Retrieves
var result2 = await getter.GetIfNeeded(); // Returns cached

// Force refresh
getter.DiscardValue();
var result3 = await getter.GetIfNeeded(); // Retrieves again
```

### Observable Operations

```csharp
// Subscribe to all get operations
getter.GetOperations.Subscribe(async task =>
{
    var result = await task;
    Console.WriteLine($"Retrieved: {result.Value}");
});

// Subscribe to state changes
getter.WhenAnyValue(g => g.IsLoading)
    .Subscribe(isLoading => UpdateUI(isLoading));
```

### Auto-Get Pattern

```csharp
var getter = new SomeGetter<MyData>
{
    Options = new GetterOptions
    {
        AutoGetMode = TriggerMode.Once  // Auto-load on creation
    }
};
// Data automatically retrieved
```

### Combined Read/Write

```csharp
IValue<MyData> value = ...;

// Read (lazy)
var data = await value.GetIfNeeded();

// Modify
data.Value.Name = "New Name";

// Write
await value.Set(data.Value);

// Observe changes
value.GetResults.Subscribe(result =>
{
    if (result.HasValue)
        Console.WriteLine($"Loaded: {result.Value}");
});
```

## ITask vs Task

This library uses `MorseCode.ITask<T>` for covariant interfaces:

```csharp
// ITask<T> is covariant (out T)
ITask<IGetResult<Animal>> task = GetAnimal();
ITask<IGetResult<object>> covariant = task; // Valid!

// Task<T> is invariant
Task<Animal> regularTask = GetAnimal();
// Task<object> invalid = regularTask; // Compilation error
```

**Conversion:**
```csharp
ITask<T> itask = ...;
Task<T> task = itask.AsTask();
```

## Common Usage Patterns

### Pattern 1: Simple Async Property

```csharp
public class UserProfileGetter : IStatelessGetter<UserProfile>
{
    public async ITask<IGetResult<UserProfile>> Get(CancellationToken ct)
    {
        try
        {
            var profile = await _api.GetUserProfile(ct);
            return GetResult<UserProfile>.Success(profile);
        }
        catch (Exception ex)
        {
            return GetResult<UserProfile>.Exception(ex);
        }
    }
}
```

### Pattern 2: Cached Lazy Loading

```csharp
public class CachedDocumentGetter : IGetter<Document>
{
    private Document? cachedValue;
    private ReadState state = ReadState.Unloaded;

    public bool HasValue => state == ReadState.Loaded;
    public Document? ReadCacheValue => cachedValue;

    public async ITask<IGetResult<Document>> GetIfNeeded(CancellationToken ct)
    {
        if (HasValue) return GetResult<Document>.Success(cachedValue!);
        return await Get(ct);
    }

    public async ITask<IGetResult<Document>> Get(CancellationToken ct)
    {
        state = ReadState.Loading;
        cachedValue = await FetchDocument(ct);
        state = ReadState.Loaded;
        return GetResult<Document>.Success(cachedValue);
    }

    public void DiscardValue()
    {
        cachedValue = null;
        state = ReadState.Unloaded;
    }
}
```

### Pattern 3: Observable Async Value

```csharp
public class ReactiveValue<T> : ReactiveObject, IValue<T>
{
    [Reactive] private bool isLoading;
    private readonly Subject<ITask<IGetResult<T>>> getOps = new();

    public IObservable<ITask<IGetResult<T>>> GetOperations => getOps;

    public async ITask<IGetResult<T>> Get(CancellationToken ct)
    {
        IsLoading = true;
        var task = FetchAsync(ct);
        getOps.OnNext(task);
        var result = await task;
        IsLoading = false;
        return result;
    }
}
```

## Design Philosophy

**Separation of Concerns:**
- Read operations (IGetter) separate from write (ISetter)
- Stateless vs stateful operations explicit
- Observable operations opt-in

**Flexibility:**
- Interfaces compose together (IValue = IGetter + ISetter)
- Multiple retrieval strategies (lazy, stateless, cached)
- Configuration via options objects

**Reactive-Friendly:**
- All operations observable
- State changes trackable
- Integrates with ReactiveUI patterns

**Async-First:**
- All operations async by default
- Synchronous cache access where appropriate
- CancellationToken support throughout

## Testing Considerations

When testing async data access:
- Mock `IStatelessGetter<T>` for simple scenarios
- Mock `IGetter<T>` for stateful/caching scenarios
- Verify `HasValue` and `ReadCacheValue` behavior
- Test `DiscardValue()` cache invalidation
- Use `TestScheduler` for observable operations

## Related Projects

- **LionFire.Data.Abstractions** (Resolves.Abstractions) - Synchronous data patterns
- **LionFire.Data.Async.Reactive** - ReactiveUI implementations
- **LionFire.Data.Async.Mvvm** - ViewModel wrappers
- **LionFire.Resolves** (Data.Async) - Concrete implementations
- **LionFire.Persistence** - Persistence layer using these abstractions

## See Also

- MorseCode.ITask: Covariant task interfaces
- System.Reactive for IObservable patterns
- LionFire Handle system (built on these abstractions)
- LionFire Persistence framework
