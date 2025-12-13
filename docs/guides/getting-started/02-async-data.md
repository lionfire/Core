# Getting Started: Async Data Access

## Overview

This guide introduces **LionFire's Async Data Access System** - a powerful framework for working with asynchronous data using the `IGetter`, `ISetter`, and `IValue` patterns. These patterns provide explicit control over caching, lazy loading, and observable operations.

**What You'll Learn**:
- Your first `IGetter` for async reads
- Implementing lazy loading with caching
- Using `IValue` for read/write operations
- Observable operations and loading states
- Practical patterns and best practices

**Prerequisites**:
- .NET 9.0+ SDK
- Basic understanding of async/await
- Completed [01-base-layer.md](01-base-layer.md) (recommended)

---

## Setup

### 1. Create a New Console Project

```bash
dotnet new console -n LionFireAsyncData
cd LionFireAsyncData
```

### 2. Add Required Packages

```bash
dotnet add package LionFire.Data.Async.Abstractions
dotnet add package LionFire.Data.Async.Reactive
```

### 3. Update Program.cs

Replace the contents with:

```csharp
using LionFire.Data.Async;

Console.WriteLine("LionFire Async Data Getting Started");
Console.WriteLine("=====================================\n");

// Your code here
```

---

## Your First Getter

Let's start with a simple stateless getter that fetches data from an API.

### Example: Weather Data Getter

```csharp
using LionFire.Data.Async;
using LionFire.Data.Async.Reactive;

// Step 1: Define your data model
public record WeatherData(string City, double Temperature, string Condition);

// Step 2: Create a stateless getter
public class WeatherGetter : IStatelessGetter<WeatherData>
{
    private readonly string city;

    public WeatherGetter(string city)
    {
        this.city = city;
    }

    public async Task<IGetResult<WeatherData>> Get(CancellationToken cancellationToken = default)
    {
        try
        {
            // Simulate API call
            await Task.Delay(500, cancellationToken);

            // In real app, call actual weather API
            var weather = new WeatherData(
                City: city,
                Temperature: Random.Shared.Next(60, 90),
                Condition: "Sunny"
            );

            return GetResult.Success(weather);
        }
        catch (Exception ex)
        {
            return GetResult.Failure<WeatherData>(ex.Message);
        }
    }
}

// Step 3: Use the getter
var weatherGetter = new WeatherGetter("San Francisco");

var result = await weatherGetter.Get();
if (result.IsSuccess)
{
    var weather = result.Value!;
    Console.WriteLine($"Weather in {weather.City}:");
    Console.WriteLine($"  Temperature: {weather.Temperature}°F");
    Console.WriteLine($"  Condition: {weather.Condition}");
}
else
{
    Console.WriteLine($"Error: {result.Error}");
}
```

**Key Points**:
- `IStatelessGetter<T>` always retrieves fresh data (no caching)
- Returns `IGetResult<T>` with success/failure information
- Supports cancellation tokens
- Explicit error handling

---

## Lazy Loading with Caching

For data that doesn't change frequently, use `IGetter<T>` with caching.

### Example: User Profile with Caching

```csharp
using LionFire.Data.Async;
using LionFire.Data.Async.Reactive;

public record UserProfile(string UserId, string Name, string Email);

public class UserProfileGetter : GetterRxO<UserProfile>
{
    private readonly string userId;

    public UserProfileGetter(string userId) : base(LoadProfile)
    {
        this.userId = userId;
    }

    private static async Task<IGetResult<UserProfile>> LoadProfile(CancellationToken ct)
    {
        // In real app, load from database or API
        await Task.Delay(1000, ct);

        var profile = new UserProfile(
            UserId: "user123",
            Name: "Alice Johnson",
            Email: "alice@example.com"
        );

        return GetResult.Success(profile);
    }
}

// Usage
var profileGetter = new UserProfileGetter("user123");

// First call: loads from source
Console.WriteLine("Loading profile...");
await profileGetter.GetIfNeeded();
var profile1 = profileGetter.ReadCacheValue;
Console.WriteLine($"Name: {profile1.Name}");

// Second call: uses cached value (instant!)
Console.WriteLine("\nAccessing cached profile...");
await profileGetter.GetIfNeeded();
var profile2 = profileGetter.ReadCacheValue;
Console.WriteLine($"Name: {profile2.Name}");

// Clear cache and reload
Console.WriteLine("\nClearing cache and reloading...");
profileGetter.DiscardValue();
await profileGetter.GetIfNeeded();
var profile3 = profileGetter.ReadCacheValue;
Console.WriteLine($"Name: {profile3.Name}");
```

**Key Concepts**:
- `GetIfNeeded()` - Only loads if not cached
- `ReadCacheValue` - Synchronous access to cached value
- `DiscardValue()` - Clear cache to force reload
- `HasValue` - Check if value is cached

---

## Read/Write with IValue

For data you need to both read and modify, use `IValue<T>`.

### Example: Application Settings

```csharp
using LionFire.Data.Async;
using LionFire.Data.Async.Reactive;
using System.Text.Json;

public record AppSettings(string Theme, int FontSize, bool NotificationsEnabled);

public class SettingsValue : ValueRxO<AppSettings>
{
    private readonly string filePath;

    public SettingsValue(string filePath) : base(
        loadFunc: ct => LoadSettings(filePath, ct),
        saveFunc: (settings, ct) => SaveSettings(filePath, settings, ct))
    {
        this.filePath = filePath;
    }

    private static async Task<IGetResult<AppSettings>> LoadSettings(
        string path,
        CancellationToken ct)
    {
        try
        {
            if (File.Exists(path))
            {
                var json = await File.ReadAllTextAsync(path, ct);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                return GetResult.Success(settings!);
            }

            // Return defaults
            var defaults = new AppSettings("Light", 14, true);
            return GetResult.Success(defaults);
        }
        catch (Exception ex)
        {
            return GetResult.Failure<AppSettings>(ex.Message);
        }
    }

    private static async Task<ISetResult> SaveSettings(
        string path,
        AppSettings settings,
        CancellationToken ct)
    {
        try
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(path, json, ct);
            return SetResult.Success;
        }
        catch (Exception ex)
        {
            return SetResult.Failure(ex.Message);
        }
    }
}

// Usage
var settingsPath = "appsettings.json";
var settings = new SettingsValue(settingsPath);

// Load settings
await settings.GetIfNeeded();
Console.WriteLine($"Theme: {settings.ReadCacheValue.Theme}");
Console.WriteLine($"Font Size: {settings.ReadCacheValue.FontSize}");

// Modify settings
var updated = settings.ReadCacheValue with { Theme = "Dark", FontSize = 16 };
var saveResult = await settings.Set(updated);

if (saveResult.IsSuccess)
{
    Console.WriteLine("Settings saved successfully!");
}

// Verify changes
settings.DiscardValue();
await settings.GetIfNeeded();
Console.WriteLine($"\nUpdated Theme: {settings.ReadCacheValue.Theme}");
Console.WriteLine($"Updated Font Size: {settings.ReadCacheValue.FontSize}");
```

**Key Features**:
- `Get()` / `GetIfNeeded()` - Load data
- `Set()` - Save data
- `ReadCacheValue` - Access current value
- Both load and save functions are customizable

---

## Observable Operations

Track loading state and operations reactively.

### Example: Data Loader with Progress

```csharp
using LionFire.Data.Async;
using LionFire.Data.Async.Reactive;
using ReactiveUI;

public record LargeDataSet(int RecordCount, string[] Records);

public class DataSetGetter : GetterRxO<LargeDataSet>
{
    public DataSetGetter() : base(LoadDataSet) { }

    private static async Task<IGetResult<LargeDataSet>> LoadDataSet(CancellationToken ct)
    {
        // Simulate slow load
        await Task.Delay(2000, ct);

        var records = Enumerable.Range(1, 1000)
            .Select(i => $"Record {i}")
            .ToArray();

        return GetResult.Success(new LargeDataSet(records.Length, records));
    }
}

// Usage with observable operations
var dataGetter = new DataSetGetter();

// Subscribe to loading state
dataGetter.WhenAnyValue(g => g.IsLoading)
    .Subscribe(isLoading =>
    {
        if (isLoading)
            Console.WriteLine("⏳ Loading data...");
        else
            Console.WriteLine("✅ Loading complete");
    });

// Subscribe to results
dataGetter.GetResults
    .Subscribe(result =>
    {
        if (result.IsSuccess && result.Value != null)
        {
            Console.WriteLine($"📊 Loaded {result.Value.RecordCount} records");
        }
    });

// Trigger load
await dataGetter.Get();

// Access the data
if (dataGetter.HasValue)
{
    var data = dataGetter.ReadCacheValue;
    Console.WriteLine($"First record: {data.Records[0]}");
}
```

**Observable Properties**:
- `IsLoading` - Boolean indicating if operation is in progress
- `GetResults` - Observable stream of results
- `GetOperations` - Observable stream of operations
- `ReadState` - Current state (NotStarted, Loading, Success, Failed)

---

## Practical Example: Simple Cache Service

Let's build a cache service using async data patterns:

```csharp
using LionFire.Data.Async;
using LionFire.Data.Async.Reactive;
using System.Collections.Concurrent;

public class CacheService<TKey, TValue>
    where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, IGetter<TValue>> cache = new();
    private readonly Func<TKey, Task<TValue>> loadFunc;
    private readonly TimeSpan cacheExpiry;

    public CacheService(Func<TKey, Task<TValue>> loadFunc, TimeSpan cacheExpiry)
    {
        this.loadFunc = loadFunc;
        this.cacheExpiry = cacheExpiry;
    }

    public async Task<TValue?> GetAsync(TKey key)
    {
        var getter = cache.GetOrAdd(key, k => CreateGetter(k));

        await getter.GetIfNeeded();

        return getter.HasValue ? getter.ReadCacheValue : default;
    }

    public void Invalidate(TKey key)
    {
        if (cache.TryGetValue(key, out var getter))
        {
            getter.DiscardValue();
        }
    }

    public void InvalidateAll()
    {
        foreach (var getter in cache.Values)
        {
            getter.DiscardValue();
        }
    }

    private IGetter<TValue> CreateGetter(TKey key)
    {
        return new GetterRxO<TValue>(async ct =>
        {
            var value = await loadFunc(key);
            return GetResult.Success(value);
        });
    }
}

// Usage
var userCache = new CacheService<string, UserProfile>(
    loadFunc: async userId =>
    {
        Console.WriteLine($"Loading user {userId} from database...");
        await Task.Delay(500);
        return new UserProfile(userId, $"User {userId}", $"{userId}@example.com");
    },
    cacheExpiry: TimeSpan.FromMinutes(5)
);

// First access: loads from database
var user1 = await userCache.GetAsync("user123");
Console.WriteLine($"User: {user1.Name}");

// Second access: returns cached value (instant)
var user2 = await userCache.GetAsync("user123");
Console.WriteLine($"User: {user2.Name}");

// Invalidate and reload
userCache.Invalidate("user123");
var user3 = await userCache.GetAsync("user123");
Console.WriteLine($"User: {user3.Name}");
```

---

## Best Practices

### 1. Choose the Right Interface

```csharp
// ✅ Use IStatelessGetter for always-fresh data
IStatelessGetter<StockPrice> stockPrices;  // API call every time

// ✅ Use IGetter for cacheable data
IGetter<UserProfile> userProfile;  // Cache until invalidated

// ✅ Use IValue for read/write data
IValue<AppConfig> configuration;  // Cache + save capability
```

### 2. Always Check Results

```csharp
// ✅ Good - Check for success
var result = await getter.Get();
if (result.IsSuccess && result.Value != null)
{
    UseData(result.Value);
}
else
{
    LogError(result.Error);
}

// ❌ Avoid - Assuming success
var value = (await getter.Get()).Value!;  // May throw!
```

### 3. Manage Cache Lifecycle

```csharp
// ✅ Good - Clear cache after update
await UpdateUserProfile(userId, newData);
profileGetter.DiscardValue();  // Clear stale cache

// ✅ Good - Use GetIfNeeded for cached reads
if (!getter.HasValue)
{
    await getter.GetIfNeeded();
}
```

### 4. Handle Cancellation

```csharp
// ✅ Good - Support cancellation
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

try
{
    var result = await getter.Get(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation timed out");
}
```

---

## Common Patterns

### Pattern 1: Retry on Failure

```csharp
public async Task<TValue?> GetWithRetry<TValue>(
    IGetter<TValue> getter,
    int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        getter.DiscardValue();
        var result = await getter.Get();

        if (result.IsSuccess)
            return result.Value;

        if (i < maxRetries - 1)
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
    }

    return default;
}
```

### Pattern 2: Fallback Chain

```csharp
public async Task<TValue?> GetWithFallback<TValue>(
    params IGetter<TValue>[] getters)
{
    foreach (var getter in getters)
    {
        var result = await getter.Get();
        if (result.IsSuccess)
            return result.Value;
    }

    return default;
}

// Usage
var value = await GetWithFallback(
    cacheGetter,      // Try cache first
    apiGetter,        // Then API
    defaultGetter     // Finally, defaults
);
```

### Pattern 3: Auto-Refresh

```csharp
public class AutoRefreshGetter<T> : IDisposable
{
    private readonly IGetter<T> getter;
    private readonly Timer timer;

    public AutoRefreshGetter(IGetter<T> getter, TimeSpan interval)
    {
        this.getter = getter;
        this.timer = new Timer(_ => Refresh(), null, interval, interval);
    }

    private async void Refresh()
    {
        getter.DiscardValue();
        await getter.GetIfNeeded();
    }

    public void Dispose() => timer?.Dispose();
}
```

---

## Summary

**LionFire Async Data Access** provides:

**Core Patterns**:
- `IStatelessGetter<T>` - Always-fresh data
- `IGetter<T>` - Lazy loading with caching
- `IValue<T>` - Read/write operations
- Observable operations for loading states

**Benefits**:
- Explicit cache control
- Observable operation lifecycle
- Type-safe results
- Cancellation support

**Next Steps**:
1. Explore [03-mvvm-basics.md](03-mvvm-basics.md) to wrap async data in ViewModels
2. Read [Async Data Domain Docs](../../data/async/README.md) for advanced patterns
3. Learn about [Observable Operations](../../data/async/observable-operations.md)

---

## Exercise

Build a simple news reader that:
1. Fetches articles from an API (use `IStatelessGetter`)
2. Caches user preferences (use `IValue`)
3. Implements retry logic for failed requests
4. Shows loading state while fetching

Use the patterns from this guide!
