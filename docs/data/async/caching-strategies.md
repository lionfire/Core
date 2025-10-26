# Caching Strategies

## Overview

This guide covers **caching and invalidation strategies** for LionFire's async data access system. Proper cache management is critical for performance and data consistency.

**Key Concept**: Cache data when appropriate, invalidate when stale, and balance between performance and freshness.

---

## Table of Contents

1. [Cache Fundamentals](#cache-fundamentals)
2. [Cache Control Methods](#cache-control-methods)
3. [ReadState Management](#readstate-management)
4. [Polling and Auto-Refresh](#polling-and-auto-refresh)
5. [Cache Invalidation Patterns](#cache-invalidation-patterns)
6. [When to Cache](#when-to-cache)
7. [Performance Optimization](#performance-optimization)

---

## Cache Fundamentals

### How Caching Works

```
First Call: GetIfNeeded()
    ↓
HasValue == false
    ↓
Retrieve from source
    ↓
Cache value
    ↓
ReadState = Loaded
    ↓
Return result

Second Call: GetIfNeeded()
    ↓
HasValue == true
    ↓
Return cached value (no retrieval)
```

---

### Cache Properties

```csharp
public interface IGetter<out TValue>
{
    // Cache status
    bool HasValue { get; }           // Is value cached?
    TValue? ReadCacheValue { get; }  // Cached value (synchronous)
    ReadState ReadState { get; }     // Detailed state

    // Cache operations
    void DiscardValue();             // Invalidate cache
    Task<IGetResult<TValue>> Get();           // Force retrieve (bypass cache)
    Task<IGetResult<TValue>> GetIfNeeded();   // Use cache if available
}
```

---

## Cache Control Methods

### DiscardValue()

**Purpose**: Invalidate cached value, force next `GetIfNeeded()` to retrieve.

```csharp
IGetter<UserProfile> profileGetter = ...;

// Load and cache
await profileGetter.GetIfNeeded();  // Retrieves
Console.WriteLine(profileGetter.ReadCacheValue.Name);

// Invalidate
profileGetter.DiscardValue();

// Next call retrieves fresh
await profileGetter.GetIfNeeded();  // Retrieves again
```

**When to Use**:
- Data modified externally
- Known staleness
- User-initiated refresh
- After write operations

---

### Get() vs GetIfNeeded()

**Get()** - Force retrieval:
```csharp
// Always retrieves from source (bypasses cache)
var result = await getter.Get();
```

**GetIfNeeded()** - Use cache if available:
```csharp
// Retrieves only if not cached
var result = await getter.GetIfNeeded();
```

**Comparison**:
```csharp
// Scenario: Cache is valid
await getter.Get();         // Retrieves from source (slow)
await getter.GetIfNeeded(); // Returns cached (fast)

// Scenario: Cache is invalid
await getter.Get();         // Retrieves from source
await getter.GetIfNeeded(); // Retrieves from source (same)
```

---

### ReadCacheValue

**Purpose**: Synchronous cache access (no await).

```csharp
// Load asynchronously
await profileGetter.GetIfNeeded();

// Access synchronously multiple times
var name = profileGetter.ReadCacheValue.Name;
var email = profileGetter.ReadCacheValue.Email;
var phone = profileGetter.ReadCacheValue.Phone;

// No additional async calls!
```

**Use Case**: Accessing cached data in loops or computations.

---

## ReadState Management

### ReadState Enum

```csharp
[Flags]
public enum ReadState
{
    Unspecified = 0,
    Unloaded = 1 << 0,      // Never loaded
    Loading = 1 << 1,       // Retrieval in progress
    Loaded = 1 << 2,        // Retrieved successfully
    Synchronized = 1 << 3   // Kept in sync (polling/watching)
}
```

---

### State Transitions

```
Initial State: Unloaded
    ↓
Call GetIfNeeded()
    ↓
State: Loading
    ↓
Retrieval completes
    ↓
State: Loaded
    ↓
Enable polling
    ↓
State: Synchronized
    ↓
Call DiscardValue()
    ↓
State: Unloaded
```

---

### Using ReadState

```csharp
IGetter<Data> getter = ...;

// Check state before operation
if (getter.ReadState == ReadState.Unloaded)
{
    await getter.GetIfNeeded();
}

// React to state changes
getter.WhenAnyValue(g => g.ReadState)
    .Subscribe(state =>
    {
        switch (state)
        {
            case ReadState.Unloaded:
                StatusText = "Not loaded";
                break;
            case ReadState.Loading:
                StatusText = "Loading...";
                break;
            case ReadState.Loaded:
                StatusText = "Ready";
                break;
            case ReadState.Synchronized:
                StatusText = "Live";
                break;
        }
    });
```

---

## Polling and Auto-Refresh

### Manual Polling

```csharp
public class PollingDataService
{
    private readonly IGetter<LiveData> dataGetter;
    private readonly Timer pollTimer;

    public PollingDataService(IGetter<LiveData> dataGetter)
    {
        this.dataGetter = dataGetter;

        // Poll every 5 seconds
        pollTimer = new Timer(
            callback: async _ => await Refresh(),
            state: null,
            dueTime: TimeSpan.Zero,
            period: TimeSpan.FromSeconds(5)
        );
    }

    private async Task Refresh()
    {
        dataGetter.DiscardValue();
        await dataGetter.GetIfNeeded();
    }

    public void Dispose()
    {
        pollTimer?.Dispose();
    }
}
```

---

### Reactive Polling

```csharp
public class ReactivePollingVM : ReactiveObject, IDisposable
{
    private readonly IGetter<LiveData> dataGetter;
    private readonly IDisposable pollSubscription;

    public ReactivePollingVM(IGetter<LiveData> dataGetter)
    {
        this.dataGetter = dataGetter;

        // Poll using Observable.Timer
        pollSubscription = Observable
            .Timer(TimeSpan.Zero, TimeSpan.FromSeconds(5))
            .Subscribe(async _ =>
            {
                dataGetter.DiscardValue();
                await dataGetter.GetIfNeeded();
            });

        // Update UI when data changes
        dataGetter.WhenAnyValue(g => g.ReadCacheValue)
            .WhereNotNull()
            .Subscribe(data => CurrentData = data);
    }

    [Reactive] private LiveData? _currentData;

    public void Dispose()
    {
        pollSubscription?.Dispose();
    }
}
```

---

### Conditional Polling

```csharp
public class SmartPollingVM : ReactiveObject
{
    [Reactive] private bool _isPollingEnabled;

    public SmartPollingVM(IGetter<Data> getter)
    {
        // Only poll when enabled
        this.WhenAnyValue(vm => vm.IsPollingEnabled)
            .Subscribe(enabled =>
            {
                if (enabled)
                    StartPolling();
                else
                    StopPolling();
            });
    }

    private IDisposable? pollSubscription;

    private void StartPolling()
    {
        pollSubscription = Observable
            .Timer(TimeSpan.Zero, TimeSpan.FromSeconds(5))
            .Subscribe(async _ =>
            {
                getter.DiscardValue();
                await getter.GetIfNeeded();
            });
    }

    private void StopPolling()
    {
        pollSubscription?.Dispose();
        pollSubscription = null;
    }
}
```

---

## Cache Invalidation Patterns

### Pattern 1: Invalidate on Write

```csharp
public async Task UpdateUserProfile(UserProfile profile)
{
    // Save changes
    await profileSetter.Set(profile);

    // Invalidate read cache
    profileGetter.DiscardValue();

    // Next read will be fresh
}
```

---

### Pattern 2: Invalidate Related Caches

```csharp
public async Task DeleteBot(string botId)
{
    // Delete bot
    await botWriter.Remove(botId);

    // Invalidate related caches
    botGetter.DiscardValue();
    botStatisticsGetter.DiscardValue();
    dashboardGetter.DiscardValue();
}
```

---

### Pattern 3: Time-Based Invalidation

```csharp
public class TimeCachedGetter<T>
{
    private readonly IGetter<T> innerGetter;
    private readonly TimeSpan cacheLifetime;
    private DateTime lastLoadTime;

    public async Task<IGetResult<T>> GetWithTimeInvalidation()
    {
        var age = DateTime.UtcNow - lastLoadTime;

        if (age > cacheLifetime)
        {
            innerGetter.DiscardValue();
        }

        var result = await innerGetter.GetIfNeeded();
        lastLoadTime = DateTime.UtcNow;
        return result;
    }
}
```

**Usage**:
```csharp
var cachedGetter = new TimeCachedGetter<WeatherData>(
    weatherGetter,
    cacheLifetime: TimeSpan.FromMinutes(15)
);

// Cache expires after 15 minutes
await cachedGetter.GetWithTimeInvalidation();
```

---

### Pattern 4: Event-Based Invalidation

```csharp
public class EventInvalidationService
{
    private readonly IGetter<Config> configGetter;

    public EventInvalidationService(
        IGetter<Config> configGetter,
        IEventBus eventBus)
    {
        this.configGetter = configGetter;

        // Invalidate when config changed event received
        eventBus.Subscribe<ConfigChangedEvent>(e =>
        {
            configGetter.DiscardValue();
        });
    }
}
```

---

## When to Cache

### ✅ Cache When:

1. **Data changes infrequently**
   ```csharp
   IGetter<AppSettings> settings;  // Changes rarely
   ```

2. **Expensive to retrieve**
   ```csharp
   IGetter<Report> expensiveReport;  // Complex database query
   ```

3. **Used multiple times**
   ```csharp
   var name = getter.ReadCacheValue.Name;
   var email = getter.ReadCacheValue.Email;
   var phone = getter.ReadCacheValue.Phone;
   ```

4. **Network latency matters**
   ```csharp
   IGetter<UserProfile> remoteProfile;  // API call
   ```

---

### ❌ Don't Cache When:

1. **Data changes constantly**
   ```csharp
   IStatelessGetter<StockPrice> price;  // Use stateless
   ```

2. **Single use only**
   ```csharp
   var result = await statelessGetter.Get();  // One-time call
   ```

3. **Real-time requirements**
   ```csharp
   IStatelessGetter<SensorData> sensor;  // Always fresh
   ```

4. **Memory constrained**
   ```csharp
   // Large data, don't cache
   IStatelessGetter<LargeDataset> data;
   ```

---

## Performance Optimization

### Cache Hit Rate

**Measure cache effectiveness**:
```csharp
public class CacheMetrics
{
    private int cacheHits;
    private int cacheMisses;

    public async Task<T> GetWithMetrics<T>(IGetter<T> getter)
    {
        if (getter.HasValue)
        {
            cacheHits++;
            return getter.ReadCacheValue;
        }
        else
        {
            cacheMisses++;
            await getter.GetIfNeeded();
            return getter.ReadCacheValue;
        }
    }

    public double CacheHitRate =>
        (double)cacheHits / (cacheHits + cacheMisses);
}
```

**Target**: > 80% hit rate for cacheable data.

---

### Memory Management

```csharp
// For large datasets, discard when not needed
public class MemoryAwareCache
{
    public async Task LoadLargeDataset()
    {
        await largeDataGetter.GetIfNeeded();

        // Use data
        ProcessData(largeDataGetter.ReadCacheValue);

        // Discard to free memory
        largeDataGetter.DiscardValue();
    }
}
```

---

### Preloading Strategies

**Eager Loading**:
```csharp
// Load critical data upfront
public async Task InitializeApplication()
{
    await configGetter.GetIfNeeded();
    await userProfileGetter.GetIfNeeded();
    await preferencesGetter.GetIfNeeded();

    // All cached for fast access
}
```

**Lazy Loading**:
```csharp
// Load on first access
public async Task<Report> GetReport(string reportId)
{
    var getter = reportGetters[reportId];
    await getter.GetIfNeeded();  // Only loads if not cached
    return getter.ReadCacheValue;
}
```

---

## Related Documentation

- **[Async Data Overview](README.md)** - Overview and quick start
- **[Getters and Setters Guide](getters-setters.md)** - Core patterns
- **[Observable Operations](observable-operations.md)** - Operation tracking
- **[Collections](collections.md)** - Collection caching
- **[Persistence Patterns](persistence.md)** - Persistent caching

---

## Summary

**Cache Control Methods**:
- **DiscardValue()** - Invalidate cache
- **Get()** - Force retrieval (bypass cache)
- **GetIfNeeded()** - Use cache if available
- **ReadCacheValue** - Synchronous cache access
- **HasValue** - Check cache status

**Cache Strategies**:
- **Lazy Loading** - Load on first access
- **Eager Loading** - Preload critical data
- **Polling** - Periodic refresh
- **Event-Based** - Invalidate on events
- **Time-Based** - Expire after duration

**Key Metrics**:
- **Cache Hit Rate** - Target > 80%
- **Cache Lifetime** - Balance freshness vs performance
- **Memory Usage** - Discard large cached data when not needed

**Best Practice**: Use `GetIfNeeded()` by default, `DiscardValue()` when data changes, `Get()` only when guaranteed fresh data required.
