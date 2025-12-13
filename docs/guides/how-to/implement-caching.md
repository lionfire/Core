# How-To: Implement Caching Strategies

## Problem

You need to cache data to reduce API calls, database queries, or expensive computations while keeping data fresh when needed.

## Solution

Use LionFire's `IGetter<T>` with its built-in caching capabilities, or implement custom caching layers with `IMemoryCache`.

---

## Strategy 1: Built-in Getter Caching

**Use case**: Simple caching with manual invalidation.

### Basic Cached Getter

```csharp
using LionFire.Data.Async.Reactive;

public record UserData(string Name, string Email, DateTime LoadedAt);

// Getter with automatic caching
var userGetter = new GetterRxO<UserData>(async ct =>
{
    Console.WriteLine("🔄 Loading from database...");
    await Task.Delay(1000, ct);  // Simulate slow operation

    var user = new UserData("Alice", "alice@example.com", DateTime.Now);
    return GetResult.Success(user);
});

// First call: loads data
await userGetter.GetIfNeeded();
Console.WriteLine($"Loaded: {userGetter.ReadCacheValue.LoadedAt:HH:mm:ss}");

// Second call: uses cache (instant!)
await userGetter.GetIfNeeded();
Console.WriteLine($"Cached: {userGetter.ReadCacheValue.LoadedAt:HH:mm:ss}");

// Force reload
userGetter.DiscardValue();
await userGetter.GetIfNeeded();
Console.WriteLine($"Reloaded: {userGetter.ReadCacheValue.LoadedAt:HH:mm:ss}");

// Output:
// 🔄 Loading from database...
// Loaded: 10:30:15
// Cached: 10:30:15  (same time - from cache)
// 🔄 Loading from database...
// Reloaded: 10:30:17  (new time - reloaded)
```

### Cache State Checking

```csharp
// Check if value is cached
if (getter.HasValue)
{
    Console.WriteLine("Using cached value");
    var data = getter.ReadCacheValue;
}
else
{
    Console.WriteLine("Need to load");
    await getter.GetIfNeeded();
}

// Check read state
switch (getter.ReadState)
{
    case ReadState.NotStarted:
        Console.WriteLine("Never loaded");
        break;
    case ReadState.Loading:
        Console.WriteLine("Currently loading");
        break;
    case ReadState.Success:
        Console.WriteLine("Loaded successfully");
        break;
    case ReadState.Failed:
        Console.WriteLine("Load failed");
        break;
}
```

---

## Strategy 2: Time-Based Expiration

**Use case**: Cache expires after a certain time.

```csharp
public class TimedCacheGetter<T> : GetterRxO<T>
{
    private readonly TimeSpan cacheExpiry;
    private DateTime? lastLoadTime;

    public TimedCacheGetter(
        Func<CancellationToken, Task<IGetResult<T>>> loadFunc,
        TimeSpan cacheExpiry) : base(loadFunc)
    {
        this.cacheExpiry = cacheExpiry;
    }

    public async Task<IGetResult<T>> GetWithExpiry(CancellationToken ct = default)
    {
        // Check if cache expired
        if (lastLoadTime.HasValue &&
            DateTime.Now - lastLoadTime.Value > cacheExpiry)
        {
            Console.WriteLine("⏰ Cache expired, reloading...");
            DiscardValue();
        }

        var result = await GetIfNeeded(ct);

        if (result.IsSuccess)
        {
            lastLoadTime = DateTime.Now;
        }

        return result;
    }
}

// Usage
var getter = new TimedCacheGetter<WeatherData>(
    loadFunc: async ct =>
    {
        Console.WriteLine("📡 Fetching weather...");
        await Task.Delay(500, ct);
        return GetResult.Success(new WeatherData(75, "Sunny"));
    },
    cacheExpiry: TimeSpan.FromMinutes(5)
);

// First call: loads
await getter.GetWithExpiry();

// Within 5 minutes: uses cache
await getter.GetWithExpiry();

// After 5 minutes: reloads automatically
await Task.Delay(TimeSpan.FromMinutes(5));
await getter.GetWithExpiry();
```

---

## Strategy 3: Multi-Level Caching

**Use case**: Memory cache → Redis → Database.

```csharp
using Microsoft.Extensions.Caching.Memory;

public class MultiLevelCache<T>
{
    private readonly string key;
    private readonly IMemoryCache memoryCache;
    private readonly Func<Task<T>> databaseLoad;

    public MultiLevelCache(
        string key,
        IMemoryCache memoryCache,
        Func<Task<T>> databaseLoad)
    {
        this.key = key;
        this.memoryCache = memoryCache;
        this.databaseLoad = databaseLoad;
    }

    public async Task<T> GetAsync()
    {
        // Level 1: Memory cache
        if (memoryCache.TryGetValue(key, out T? cachedValue))
        {
            Console.WriteLine("✅ Memory cache hit");
            return cachedValue!;
        }

        // Level 2: Redis (simulated)
        var redisValue = await TryGetFromRedis(key);
        if (redisValue != null)
        {
            Console.WriteLine("✅ Redis cache hit");

            // Backfill memory cache
            memoryCache.Set(key, redisValue, TimeSpan.FromMinutes(5));

            return redisValue;
        }

        // Level 3: Database
        Console.WriteLine("💾 Loading from database");
        var dbValue = await databaseLoad();

        // Backfill both caches
        memoryCache.Set(key, dbValue, TimeSpan.FromMinutes(5));
        await SetInRedis(key, dbValue);

        return dbValue;
    }

    public void Invalidate()
    {
        memoryCache.Remove(key);
        InvalidateRedis(key);
    }

    private async Task<T?> TryGetFromRedis(string key)
    {
        // Simulate Redis lookup
        await Task.Delay(50);
        return default;
    }

    private async Task SetInRedis(string key, T value)
    {
        // Simulate Redis set
        await Task.Delay(10);
    }

    private void InvalidateRedis(string key)
    {
        // Simulate Redis delete
    }
}

// Usage
var memoryCache = new MemoryCache(new MemoryCacheOptions());

var userCache = new MultiLevelCache<User>(
    key: "user:123",
    memoryCache: memoryCache,
    databaseLoad: async () =>
    {
        await Task.Delay(500);
        return new User { Id = "123", Name = "Alice" };
    }
);

var user = await userCache.GetAsync();  // Loads from database
var user2 = await userCache.GetAsync(); // Memory cache hit
```

---

## Strategy 4: Cache Aside Pattern

**Use case**: Application controls when to cache.

```csharp
public class CacheAsideRepository<TKey, TValue>
    where TKey : notnull
{
    private readonly IMemoryCache cache;
    private readonly Func<TKey, Task<TValue?>> loadFunc;
    private readonly TimeSpan cacheExpiry;

    public CacheAsideRepository(
        IMemoryCache cache,
        Func<TKey, Task<TValue?>> loadFunc,
        TimeSpan cacheExpiry)
    {
        this.cache = cache;
        this.loadFunc = loadFunc;
        this.cacheExpiry = cacheExpiry;
    }

    public async Task<TValue?> GetAsync(TKey key)
    {
        var cacheKey = $"cache:{typeof(TValue).Name}:{key}";

        // Try cache first
        if (cache.TryGetValue(cacheKey, out TValue? cachedValue))
        {
            return cachedValue;
        }

        // Load from source
        var value = await loadFunc(key);

        // Store in cache
        if (value != null)
        {
            cache.Set(cacheKey, value, cacheExpiry);
        }

        return value;
    }

    public async Task SetAsync(TKey key, TValue value)
    {
        var cacheKey = $"cache:{typeof(TValue).Name}:{key}";

        // Update cache
        cache.Set(cacheKey, value, cacheExpiry);

        // Persist to database
        await PersistToDatabase(key, value);
    }

    public void Invalidate(TKey key)
    {
        var cacheKey = $"cache:{typeof(TValue).Name}:{key}";
        cache.Remove(cacheKey);
    }

    private async Task PersistToDatabase(TKey key, TValue value)
    {
        // Database write logic
        await Task.Delay(100);
    }
}

// Usage
var memoryCache = new MemoryCache(new MemoryCacheOptions());

var productRepo = new CacheAsideRepository<string, Product>(
    cache: memoryCache,
    loadFunc: async productId =>
    {
        Console.WriteLine($"Loading product {productId} from database");
        await Task.Delay(500);
        return new Product { Id = productId, Name = $"Product {productId}" };
    },
    cacheExpiry: TimeSpan.FromMinutes(10)
);

// First call: database
var product1 = await productRepo.GetAsync("PROD-001");

// Second call: cache
var product2 = await productRepo.GetAsync("PROD-001");

// Update (invalidates cache)
product1.Name = "Updated Product";
await productRepo.SetAsync("PROD-001", product1);
```

---

## Strategy 5: Write-Through Caching

**Use case**: All writes go through cache, keeping it consistent.

```csharp
public class WriteThroughCache<TKey, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> cache = new();
    private readonly Func<TKey, Task<TValue?>> loadFunc;
    private readonly Func<TKey, TValue, Task> saveFunc;

    public WriteThroughCache(
        Func<TKey, Task<TValue?>> loadFunc,
        Func<TKey, TValue, Task> saveFunc)
    {
        this.loadFunc = loadFunc;
        this.saveFunc = saveFunc;
    }

    public async Task<TValue?> GetAsync(TKey key)
    {
        // Check cache
        if (cache.TryGetValue(key, out var value))
        {
            Console.WriteLine($"✅ Cache hit: {key}");
            return value;
        }

        // Load from storage
        Console.WriteLine($"💾 Cache miss: {key}");
        var loaded = await loadFunc(key);

        if (loaded != null)
        {
            cache[key] = loaded;
        }

        return loaded;
    }

    public async Task SetAsync(TKey key, TValue value)
    {
        Console.WriteLine($"💾 Writing: {key}");

        // Write to storage first (write-through)
        await saveFunc(key, value);

        // Then update cache
        cache[key] = value;

        Console.WriteLine($"✅ Cache updated: {key}");
    }

    public void Remove(TKey key)
    {
        cache.Remove(key);
    }
}

// Usage
var cache = new WriteThroughCache<string, Document>(
    loadFunc: async id =>
    {
        await Task.Delay(500);
        return new Document { Id = id, Title = $"Document {id}" };
    },
    saveFunc: async (id, doc) =>
    {
        await Task.Delay(300);
        Console.WriteLine($"Saved to database: {doc.Title}");
    }
);

// Load document (caches)
var doc = await cache.GetAsync("DOC-001");

// Update document (writes through)
doc.Title = "Updated Title";
await cache.SetAsync("DOC-001", doc);

// Read again (from cache)
var doc2 = await cache.GetAsync("DOC-001");
```

---

## Strategy 6: Invalidation Patterns

### Pattern A: Time-Based Invalidation

```csharp
public class TimeBasedCache<T>
{
    private T? cachedValue;
    private DateTime? lastUpdate;
    private readonly TimeSpan expiry;
    private readonly Func<Task<T>> loadFunc;

    public TimeBasedCache(Func<Task<T>> loadFunc, TimeSpan expiry)
    {
        this.loadFunc = loadFunc;
        this.expiry = expiry;
    }

    public async Task<T> GetAsync()
    {
        if (cachedValue != null &&
            lastUpdate.HasValue &&
            DateTime.Now - lastUpdate.Value < expiry)
        {
            return cachedValue;
        }

        cachedValue = await loadFunc();
        lastUpdate = DateTime.Now;
        return cachedValue;
    }
}
```

### Pattern B: Event-Based Invalidation

```csharp
public class EventBasedCache<T>
{
    private T? cachedValue;
    private readonly Func<Task<T>> loadFunc;

    public event EventHandler? Invalidated;

    public EventBasedCache(Func<Task<T>> loadFunc)
    {
        this.loadFunc = loadFunc;
    }

    public async Task<T> GetAsync()
    {
        if (cachedValue == null)
        {
            cachedValue = await loadFunc();
        }

        return cachedValue;
    }

    public void Invalidate()
    {
        cachedValue = default;
        Invalidated?.Invoke(this, EventArgs.Empty);
    }
}

// Usage with multiple dependent caches
var userCache = new EventBasedCache<User>(LoadUser);
var profileCache = new EventBasedCache<Profile>(LoadProfile);

// When user changes, invalidate profile too
userCache.Invalidated += (s, e) => profileCache.Invalidate();
```

### Pattern C: Dependency-Based Invalidation

```csharp
public class DependentCache<T>
{
    private readonly List<Action> invalidators = new();

    public void DependsOn(DependentCache<object> other)
    {
        other.AddInvalidator(() => Invalidate());
    }

    public void AddInvalidator(Action invalidator)
    {
        invalidators.Add(invalidator);
    }

    public void Invalidate()
    {
        // Clear this cache
        Clear();

        // Cascade invalidation
        foreach (var invalidator in invalidators)
        {
            invalidator();
        }
    }

    private void Clear()
    {
        // Clear cache logic
    }
}
```

---

## Best Practices

### 1. Choose the Right Strategy

```csharp
// ✅ Time-based for semi-static data
var configCache = new TimedCacheGetter<Config>(loadFunc, TimeSpan.FromHours(1));

// ✅ Manual invalidation for mutable data
var userCache = new GetterRxO<User>(loadFunc);
// Invalidate after update
await UpdateUser(user);
userCache.DiscardValue();

// ✅ No caching for real-time data
var priceGetter = new StatelessGetter<StockPrice>(loadFunc);
```

### 2. Set Appropriate Expiration Times

```csharp
// ✅ Good - Match data volatility
var configCache = TimeSpan.FromHours(24);    // Config changes rarely
var userCache = TimeSpan.FromMinutes(5);     // User data changes occasionally
var priceCache = TimeSpan.FromSeconds(10);   // Prices change frequently

// ❌ Avoid - Too long for volatile data
var priceCache = TimeSpan.FromHours(24);  // Stale prices!

// ❌ Avoid - Too short for stable data
var configCache = TimeSpan.FromSeconds(1);  // Excessive reloading!
```

### 3. Handle Cache Failures Gracefully

```csharp
// ✅ Good - Fallback on cache failure
try
{
    return cache.Get(key);
}
catch (CacheException)
{
    // Cache unavailable, load directly
    return await LoadFromDatabase(key);
}
```

### 4. Monitor Cache Performance

```csharp
public class MonitoredCache<T>
{
    private int hits;
    private int misses;

    public async Task<T> GetAsync(string key)
    {
        if (cache.TryGetValue(key, out T? value))
        {
            hits++;
            return value;
        }

        misses++;
        var loaded = await LoadAsync(key);
        cache.Set(key, loaded);
        return loaded;
    }

    public double HitRate => hits + misses == 0 ? 0 : (double)hits / (hits + misses);
}
```

---

## Summary

**Caching Strategies:**

1. **Built-in Getter Caching** - Simple, manual invalidation
2. **Time-Based Expiration** - Automatic expiry after time
3. **Multi-Level Caching** - Memory → Redis → Database
4. **Cache Aside** - Application manages cache
5. **Write-Through** - Consistent cache writes
6. **Invalidation Patterns** - Time, event, dependency-based

**Key Decisions:**
- Cache duration based on data volatility
- Invalidation strategy (manual, time, event)
- Single vs. multi-level caching
- Write-through vs. write-behind

**Related Guides:**
- [Implement Custom Getter](implement-custom-getter.md)
- [Async Data Domain Docs](../../data/async/README.md)
- [Caching Strategies Reference](../../data/async/caching-strategies.md)
