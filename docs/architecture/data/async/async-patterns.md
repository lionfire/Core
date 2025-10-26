# Core Async Patterns

**Overview**: Detailed exploration of LionFire's fundamental async data access patterns - Getter (read), Setter (write), and Value (read/write). Covers stateless vs stateful operations, caching strategies, observable operations, and TriggerMode configuration.

---

## Table of Contents

1. [Getter Pattern](#getter-pattern)
2. [Setter Pattern](#setter-pattern)
3. [Value Pattern](#value-pattern)
4. [Observable Operations](#observable-operations)
5. [TriggerMode Configuration](#triggermode-configuration)
6. [Result Types](#result-types)

---

## Getter Pattern

### Purpose

**IGetter** represents any **read operation** - fetching data from an external source asynchronously.

### Interface Hierarchy

```
IGetter (marker)
    ├── IStatelessGetter<T>     → Always fetches
    └── ILazyGetter             → Caches results
            ↓
        IGetter<T>              → Full-featured (stateless + lazy + observable)
```

### IStatelessGetter\<T\> - Always Fresh

**Contract**: Every call fetches fresh data.

```csharp
public interface IStatelessGetter<out TValue> : IGetter
{
    ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default);
}
```

**Usage**:
```csharp
public class CurrentTimeGetter : IStatelessGetter<DateTime>
{
    public async ITask<IGetResult<DateTime>> Get(CancellationToken ct)
    {
        // Always returns fresh time
        return GetResult<DateTime>.Success(DateTime.UtcNow);
    }
}

// Every call gets fresh value
var result1 = await getter.Get(); // 10:00:00
await Task.Delay(1000);
var result2 = await getter.Get(); // 10:00:01 (different!)
```

**Use Cases**:
- Time/timestamps
- Random numbers
- Always-changing data
- No caching desired

### ILazyGetter - Cached Results

**Contract**: Caches retrieved value, provides invalidation.

```csharp
public interface ILazyGetter : IDefaultable, IDiscardableValue, IDiscardable
{
    // Implicit: GetIfNeeded() retrieves only if not cached
    // DiscardValue() invalidates cache
}
```

**Characteristics**:
- **Stateful**: Remembers last retrieved value
- **Explicit invalidation**: Call `DiscardValue()` to force refresh
- **Performance**: Avoids redundant fetches

**Interfaces**:
```csharp
public interface IDiscardableValue
{
    void DiscardValue();  // Invalidate cache
}

public interface IDefaultable
{
    bool HasValue { get; }  // Is cache valid?
}
```

### IGetter\<T\> - Full-Featured

**Contract**: Combines stateless + lazy + observable + state.

```csharp
public interface IGetter<out TValue>
    : IStatelessGetter<TValue>            // Get()
    , ILazyGetter<TValue>                  // GetIfNeeded(), DiscardValue()
    , IObservableGetOperations<TValue>     // Observable operations
    , IObservableGetState                  // Observable state
    , IObservableGetResults<TValue>        // Observable results
{
    // Synchronous cache access
    TValue? ReadCacheValue { get; }

    // Cache validity
    bool HasValue { get; }

    // Lazy retrieval
    ITask<IGetResult<TValue>> GetIfNeeded(CancellationToken = default);
}
```

**Complete Example**:
```csharp
public class UserProfileGetter : IGetter<UserProfile>
{
    private UserProfile? cache;
    private ReadState state = ReadState.Unloaded;

    // Lazy property
    public bool HasValue => state == ReadState.Loaded;
    public UserProfile? ReadCacheValue => cache;

    // Stateless: Always fetch
    public async ITask<IGetResult<UserProfile>> Get(CancellationToken ct)
    {
        state = ReadState.Loading;
        cache = await FetchFromAPI(ct);
        state = ReadState.Loaded;
        return GetResult<UserProfile>.Success(cache);
    }

    // Lazy: Fetch only if needed
    public async ITask<IGetResult<UserProfile>> GetIfNeeded(CancellationToken ct)
    {
        if (HasValue)
            return GetResult<UserProfile>.Success(cache!);

        return await Get(ct);
    }

    // Invalidation
    public void DiscardValue()
    {
        cache = null;
        state = ReadState.Unloaded;
    }

    // Observable operations
    public IObservable<ITask<IGetResult<UserProfile>>> GetOperations { get; }
    public IObservable<IGetResult<UserProfile>?> GetResults { get; }
    public bool IsLoading => state == ReadState.Loading;
}
```

### Read State Tracking

```csharp
[Flags]
public enum ReadState
{
    Unspecified = 0,
    Unloaded = 1 << 0,      // Never loaded
    Loading = 1 << 1,       // Retrieval in progress
    Loaded = 1 << 2,        // Retrieved successfully
    Synchronized = 1 << 3   // Kept in sync (e.g., polling/watching)
}
```

**Usage**:
```csharp
public class MyGetter : IGetter<Data>
{
    private ReadState state = ReadState.Unloaded;

    public bool HasValue => state.HasFlag(ReadState.Loaded);
    public bool IsLoading => state.HasFlag(ReadState.Loading);
    public bool IsSynchronized => state.HasFlag(ReadState.Synchronized);

    public async ITask<IGetResult<Data>> Get(CancellationToken ct)
    {
        state = ReadState.Loading;
        try
        {
            var data = await FetchData(ct);
            state = ReadState.Loaded;
            return GetResult<Data>.Success(data);
        }
        catch (Exception ex)
        {
            state = ReadState.Unloaded;
            return GetResult<Data>.Exception(ex);
        }
    }
}
```

---

## Setter Pattern

### ISetter\<T\> - Write Operations

**Contract**: Asynchronous write operation.

```csharp
public interface ISetter<TValue>
{
    Task<ISetResult<TValue>> Set(TValue? value, CancellationToken cancellationToken = default);
}
```

**Implementation**:
```csharp
public class ConfigurationSetter : ISetter<AppConfig>
{
    private readonly string configPath;

    public async Task<ISetResult<AppConfig>> Set(AppConfig? value, CancellationToken ct)
    {
        try
        {
            if (value == null)
                return SetResult<AppConfig>.SuccessWithNoValue;

            var json = JsonSerializer.Serialize(value);
            await File.WriteAllTextAsync(configPath, json, ct);

            return SetResult<AppConfig>.Success(value);
        }
        catch (Exception ex)
        {
            return SetResult<AppConfig>.Exception(ex);
        }
    }
}
```

### Observable Set Operations

**Track write operations**:

```csharp
public interface IObservableSetOperations
{
    IObservable<ITask> SetOperations { get; }
}

public interface IObservableSetState
{
    bool IsSetting { get; }
}

public interface IObservableSetResults<out TValue>
{
    IObservable<ISetResult<TValue>> SetResults { get; }
}
```

**Usage**:
```csharp
// Subscribe to all set operations
setter.SetOperations.Subscribe(async task =>
{
    Console.WriteLine("Write started");
    await task;
    Console.WriteLine("Write completed");
});

// Subscribe to results
setter.SetResults.Subscribe(result =>
{
    if (result.IsSuccess)
        Console.WriteLine("Saved successfully");
    else
        Console.WriteLine($"Error: {result.Error}");
});

// Track state
setter.WhenAnyValue(s => s.IsSetting)
    .Subscribe(isSetting => UpdateUI(isSetting));
```

---

## Value Pattern

### IValue\<T\> - Combined Read/Write

**Contract**: Full-featured read and write operations.

```csharp
public interface IValue<T>
    : IStatelessGetter<T>
    , ISetter<T>
    , ILazyGetter
    , IObservableGetOperations<T>
    , IObservableSetOperations
    // ... all observable interfaces
{
    TValue? ReadCacheValue { get; }
    bool HasValue { get; }

    ITask<IGetResult<T>> GetIfNeeded(CancellationToken = default);
}
```

**Complete Implementation**:
```csharp
public class FileValue<T> : ReactiveObject, IValueRxO<T>
{
    private readonly string filePath;
    private T? cache;
    private ReadState readState = ReadState.Unloaded;

    [Reactive] private bool isLoading;
    [Reactive] private bool isSetting;

    public bool HasValue => readState == ReadState.Loaded;
    public T? ReadCacheValue => cache;
    public bool IsLoading => isLoading;
    public bool IsSetting => isSetting;

    // Read
    public async ITask<IGetResult<T>> Get(CancellationToken ct)
    {
        IsLoading = true;
        readState = ReadState.Loading;

        try
        {
            var json = await File.ReadAllTextAsync(filePath, ct);
            cache = JsonSerializer.Deserialize<T>(json);
            readState = ReadState.Loaded;
            this.RaisePropertyChanged(nameof(ReadCacheValue));
            return GetResult<T>.Success(cache!);
        }
        catch (Exception ex)
        {
            readState = ReadState.Unloaded;
            return GetResult<T>.Exception(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async ITask<IGetResult<T>> GetIfNeeded(CancellationToken ct)
    {
        if (HasValue)
            return GetResult<T>.Success(cache!);

        return await Get(ct);
    }

    // Write
    public async Task<ISetResult<T>> Set(T? value, CancellationToken ct)
    {
        IsSetting = true;

        try
        {
            if (value == null)
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                cache = default;
                readState = ReadState.Unloaded;
                return SetResult<T>.SuccessWithNoValue;
            }

            var json = JsonSerializer.Serialize(value);
            await File.WriteAllTextAsync(filePath, json, ct);
            cache = value;
            readState = ReadState.Loaded;
            this.RaisePropertyChanged(nameof(ReadCacheValue));

            return SetResult<T>.Success(value);
        }
        catch (Exception ex)
        {
            return SetResult<T>.Exception(ex);
        }
        finally
        {
            IsSetting = false;
        }
    }

    // Invalidation
    public void DiscardValue()
    {
        cache = default;
        readState = ReadState.Unloaded;
        this.RaisePropertyChanged(nameof(HasValue));
        this.RaisePropertyChanged(nameof(ReadCacheValue));
    }
}
```

**Usage**:
```csharp
var configValue = new FileValue<AppConfig>("/app/config.json");

// Load
var result = await configValue.GetIfNeeded();
if (result.HasValue)
{
    var config = result.Value;
    Console.WriteLine(config.Setting);
}

// Modify and save
config.Setting = "new value";
await configValue.Set(config);

// Access cache synchronously
var cached = configValue.ReadCacheValue;
```

---

## Observable Operations

### Get Operations Observable

**Track all get operations as they occur**:

```csharp
public interface IObservableGetOperations<out TValue>
{
    IObservable<ITask<IGetResult<TValue>>> GetOperations { get; }
}
```

**Usage**:
```csharp
// Log all get operations
getter.GetOperations.Subscribe(async task =>
{
    var sw = Stopwatch.StartNew();
    var result = await task;
    Console.WriteLine($"Get completed in {sw.ElapsedMilliseconds}ms: {result.HasValue}");
});

// Show loading indicator
getter.GetOperations
    .Select(_ => true)  // Started
    .Merge(getter.GetResults.Select(_ => false))  // Completed
    .Subscribe(isLoading => ShowSpinner(isLoading));
```

### Get Results Observable

**Subscribe to results**:

```csharp
public interface IObservableGetResults<out TValue>
{
    IObservable<IGetResult<TValue>?> GetResults { get; }
}
```

**Usage**:
```csharp
// Update UI when data loads
getter.GetResults
    .Where(r => r?.HasValue == true)
    .Subscribe(result => UpdateDisplay(result!.Value));

// Handle errors
getter.GetResults
    .Where(r => r?.IsSuccess == false)
    .Subscribe(result => ShowError(result!.Error));
```

### Get State Observable

**Track loading state**:

```csharp
public interface IObservableGetState
{
    bool IsLoading { get; }
}
```

**Usage**:
```csharp
// Bind to progress indicator
this.WhenAnyValue(x => x.DataGetter.IsLoading)
    .Subscribe(isLoading => ProgressBar.IsVisible = isLoading);
```

---

## TriggerMode Configuration

### TriggerMode Enum

**Controls when operations automatically execute**:

```csharp
[Flags]
public enum TriggerMode
{
    Disabled = 1 << 0,          // No automatic operations
    Once = 1 << 1,              // Trigger once (on creation)
    Manual = 1 << 2,            // Manual only (via commands)
    Auto = 1 << 3,              // Auto with throttling/batching
    AutoImmediate = 1 << 4      // Auto without delays
}
```

### GetterOptions

```csharp
public class GetterOptions
{
    public TriggerMode AutoGetMode { get; set; } = TriggerMode.Manual;
    // Additional caching, throttling options
}
```

**Examples**:

**Manual** (default):
```csharp
var getter = new MyGetter
{
    Options = new GetterOptions { AutoGetMode = TriggerMode.Manual }
};

// Must explicitly call Get
await getter.Get();
```

**Once** (auto-load on creation):
```csharp
var getter = new MyGetter
{
    Options = new GetterOptions { AutoGetMode = TriggerMode.Once }
};
// Automatically calls Get() on creation
```

**Auto** (with throttling):
```csharp
var getter = new MyGetter
{
    Options = new GetterOptions
    {
        AutoGetMode = TriggerMode.Auto,
        ThrottleInterval = TimeSpan.FromSeconds(1)
    }
};
// Throttles rapid Get() calls
```

### SetterOptions

```csharp
public class SetterOptions
{
    public TriggerMode AutoSetMode { get; set; } = TriggerMode.Manual;
    // Debouncing, batching options
}
```

**Examples**:

**Manual** (explicit save):
```csharp
var setter = new ConfigSetter
{
    Options = new SetterOptions { AutoSetMode = TriggerMode.Manual }
};

// Must explicitly call Set
await setter.Set(config);
```

**AutoImmediate** (auto-save on change):
```csharp
var setter = new ConfigSetter
{
    Options = new SetterOptions { AutoSetMode = TriggerMode.AutoImmediate }
};

// Automatically saves when property changes
config.Property = "new value";  // Triggers Set() immediately
```

**Auto** (debounced save):
```csharp
var setter = new ConfigSetter
{
    Options = new SetterOptions
    {
        AutoSetMode = TriggerMode.Auto,
        DebounceInterval = TimeSpan.FromSeconds(2)
    }
};

// Saves 2 seconds after last change
config.Property = "value1";  // Starts timer
config.Property = "value2";  // Resets timer
// ... 2 seconds later: Set() called with "value2"
```

### ValueOptions

**Combines get and set options**:

```csharp
public class ValueOptions
{
    public GetterOptions Get { get; set; } = new();
    public SetterOptions Set { get; set; } = new();
}
```

**Usage**:
```csharp
var value = new FileValue<AppConfig>("/config.json")
{
    Options = new ValueOptions
    {
        Get = new GetterOptions
        {
            AutoGetMode = TriggerMode.Once  // Load on creation
        },
        Set = new SetterOptions
        {
            AutoSetMode = TriggerMode.Auto,  // Debounced auto-save
            DebounceInterval = TimeSpan.FromSeconds(2)
        }
    }
};

// Automatically loads on creation
// Auto-saves 2 seconds after changes stop
```

---

## Result Types

### IGetResult\<T\>

**Represents the result of a get operation**:

```csharp
public interface IGetResult<out T>
{
    bool HasValue { get; }
    bool IsSuccess { get; }
    T Value { get; }
    Exception? Error { get; }
    string? ErrorMessage { get; }
}
```

**Creating Results**:
```csharp
// Success with value
return GetResult<MyData>.Success(data);

// Success with no value (not found)
return GetResult<MyData>.SuccessWithNoValue;

// Failure
return GetResult<MyData>.Failure("Not found");

// Exception
return GetResult<MyData>.Exception(ex);
```

**Handling Results**:
```csharp
var result = await getter.Get();

if (result.HasValue)
{
    var data = result.Value;
    // Use data
}
else if (result.IsSuccess)
{
    // Success but no value (e.g., not found)
}
else
{
    Console.WriteLine($"Error: {result.ErrorMessage}");
    if (result.Error != null)
        Logger.LogError(result.Error, "Get failed");
}
```

### ISetResult\<T\>

**Represents the result of a set operation**:

```csharp
public interface ISetResult<out T>
{
    bool IsSuccess { get; }
    T? Value { get; }
    Exception? Error { get; }
    string? ErrorMessage { get; }
}
```

**Creating Results**:
```csharp
// Success
return SetResult<MyData>.Success(data);

// Success with no value
return SetResult<MyData>.SuccessWithNoValue;

// Failure
return SetResult<MyData>.Failure("Permission denied");

// Exception
return SetResult<MyData>.Exception(ex);
```

---

## Common Patterns

### Pattern 1: HTTP API Getter

```csharp
public class WeatherGetter : IStatelessGetter<WeatherData>
{
    private readonly HttpClient httpClient;

    public async ITask<IGetResult<WeatherData>> Get(CancellationToken ct)
    {
        try
        {
            var data = await httpClient.GetFromJsonAsync<WeatherData>(
                "https://api.weather.com/current", ct);

            return data != null
                ? GetResult<WeatherData>.Success(data)
                : GetResult<WeatherData>.SuccessWithNoValue;
        }
        catch (HttpRequestException ex)
        {
            return GetResult<WeatherData>.Failure($"HTTP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return GetResult<WeatherData>.Exception(ex);
        }
    }
}
```

### Pattern 2: Cached File Getter

```csharp
public class CachedFileGetter<T> : IGetter<T>
{
    private readonly string filePath;
    private T? cache;
    private DateTime? cacheTime;
    private readonly TimeSpan cacheExpiry = TimeSpan.FromMinutes(5);

    public bool HasValue => cache != null && CacheIsValid();
    public T? ReadCacheValue => cache;

    private bool CacheIsValid()
        => cacheTime.HasValue &&
           DateTime.UtcNow - cacheTime.Value < cacheExpiry;

    public async ITask<IGetResult<T>> GetIfNeeded(CancellationToken ct)
    {
        if (HasValue)
            return GetResult<T>.Success(cache!);

        return await Get(ct);
    }

    public async ITask<IGetResult<T>> Get(CancellationToken ct)
    {
        var json = await File.ReadAllTextAsync(filePath, ct);
        cache = JsonSerializer.Deserialize<T>(json);
        cacheTime = DateTime.UtcNow;
        return GetResult<T>.Success(cache!);
    }

    public void DiscardValue()
    {
        cache = default;
        cacheTime = null;
    }
}
```

### Pattern 3: Database Value

```csharp
public class DbValue<T> : IValue<T>
{
    private readonly DbContext db;
    private readonly int entityId;
    private T? cache;

    public async ITask<IGetResult<T>> Get(CancellationToken ct)
    {
        cache = await db.Set<T>().FindAsync(new object[] { entityId }, ct);
        return cache != null
            ? GetResult<T>.Success(cache)
            : GetResult<T>.SuccessWithNoValue;
    }

    public async Task<ISetResult<T>> Set(T? value, CancellationToken ct)
    {
        if (value == null)
        {
            var entity = await db.Set<T>().FindAsync(new object[] { entityId }, ct);
            if (entity != null)
                db.Set<T>().Remove(entity);
        }
        else
        {
            db.Set<T>().Update(value);
        }

        await db.SaveChangesAsync(ct);
        cache = value;

        return SetResult<T>.Success(value);
    }

    public async ITask<IGetResult<T>> GetIfNeeded(CancellationToken ct)
        => HasValue ? GetResult<T>.Success(cache!) : await Get(ct);

    public bool HasValue => cache != null;
    public T? ReadCacheValue => cache;
    public void DiscardValue() => cache = default;
}
```

### Pattern 4: Computed Getter

```csharp
public class ComputedGetter : IStatelessGetter<Summary>
{
    private readonly IGetter<DataSet> dataGetter;

    public async ITask<IGetResult<Summary>> Get(CancellationToken ct)
    {
        // Get underlying data
        var dataResult = await dataGetter.Get(ct);
        if (!dataResult.HasValue)
            return GetResult<Summary>.Failure("No data");

        // Compute summary
        var summary = new Summary
        {
            Total = dataResult.Value.Items.Count,
            Average = dataResult.Value.Items.Average(x => x.Value)
        };

        return GetResult<Summary>.Success(summary);
    }
}
```

---

## Summary

### Key Interfaces

| Interface | Read | Write | Caching | Observable |
|-----------|------|-------|---------|------------|
| `IStatelessGetter<T>` | ✅ Always | ❌ | ❌ | Optional |
| `IGetter<T>` | ✅ Lazy | ❌ | ✅ | ✅ |
| `ISetter<T>` | ❌ | ✅ | ❌ | Optional |
| `IValue<T>` | ✅ Lazy | ✅ | ✅ | ✅ |

### Key Methods

**Read**:
- `Get()` - Always fetch
- `GetIfNeeded()` - Fetch if not cached
- `ReadCacheValue` - Sync cache access
- `DiscardValue()` - Invalidate cache

**Write**:
- `Set(value)` - Write operation

**State**:
- `HasValue` - Cache validity
- `IsLoading` / `IsSetting` - Operation in progress

### When to Use

**IStatelessGetter**:
- Time-sensitive data
- Always need fresh values
- No caching desired

**IGetter** (lazy):
- Expensive operations
- Data doesn't change frequently
- Want caching

**ISetter**:
- Write-only operations
- Configuration updates
- Command execution

**IValue**:
- Read/write entities
- Configuration files
- User profiles
- Any data that needs both read and write

### Related Documentation

- **[Reactive Data Patterns](reactive-data.md)** - ReactiveUI integration
- **[Persistence Integration](persistence-integration.md)** - IObservableReader/Writer
- **[MVVM Data Binding](../mvvm/data-binding.md)** - ViewModel wrappers
- **[Async Data Overview](README.md)** - Architecture overview
