# Async Collections

## Overview

This guide covers **async collections** in LionFire, including keyed and unkeyed collections, DynamicData integration, and file-backed collections. Async collections provide lazy loading, reactive updates, and efficient change tracking.

**Key Concept**: Collections can be loaded asynchronously and updated reactively using DynamicData's observable caches.

---

## Table of Contents

1. [Collection Types](#collection-types)
2. [Unkeyed Collections](#unkeyed-collections)
3. [Keyed Collections](#keyed-collections)
4. [DynamicData Integration](#dynamicdata-integration)
5. [File System Collections](#file-system-collections)
6. [Transformation Pipelines](#transformation-pipelines)
7. [Common Patterns](#common-patterns)

---

## Collection Types

### Hierarchy

```
Async Collections
│
├── Unkeyed (IEnumerable-based)
│   └── AsyncDynamicDataCollection<TValue>
│       └── Indexed by integer
│
├── Keyed (Dictionary-based)
│   ├── AsyncKeyedCollection<TKey, TValue>
│   ├── AsyncKeyValueCollection<TKey, TValue>
│   └── AsyncReadOnlyKeyedCollection<TKey, TValue>
│
└── File-Backed
    ├── AsyncFileDictionary<TValue>
    ├── SerializingFileDictionary<TValue>
    └── IObservableReader<TKey, TValue>  (workspace pattern)
```

---

## Unkeyed Collections

### AsyncDynamicDataCollection\<TValue\>

**Location**: `LionFire.Data.Async.Reactive`

**Purpose**: Base class for async-loaded collections backed by DynamicData.

```csharp
public abstract partial class AsyncDynamicDataCollection<TValue> : ReactiveObject
    where TValue : notnull
{
    // DynamicData cache (indexed by integer)
    protected SourceCache<TValue, int> sourceCache;

    // Observable cache for consumers
    public IObservableCache<TValue, int> Items { get; }

    // Get operation
    public abstract ITask<IGetResult<IEnumerable<TValue>>> Get(CancellationToken ct);

    // State
    public bool IsLoading { get; }
    public bool HasValue { get; }
}
```

#### Creating Custom Collection

```csharp
public class ProductCollection : AsyncDynamicDataCollection<Product>
{
    private readonly IProductRepository repository;

    public ProductCollection(IProductRepository repository)
    {
        this.repository = repository;
    }

    protected override async ITask<IGetResult<IEnumerable<Product>>> GetImpl(
        CancellationToken ct)
    {
        try
        {
            var products = await repository.GetAllAsync(ct);

            // Update cache
            sourceCache.Edit(updater =>
            {
                updater.Clear();
                int index = 0;
                foreach (var product in products)
                {
                    updater.AddOrUpdate(product, index++);
                }
            });

            return GetResult.Success(products);
        }
        catch (Exception ex)
        {
            return GetResult.Failure<IEnumerable<Product>>(ex);
        }
    }
}
```

**Usage**:
```csharp
var collection = new ProductCollection(repository);
await collection.Get();

// Access items
foreach (var product in collection.Items.Items)
{
    Console.WriteLine(product.Name);
}

// Subscribe to changes
collection.Items.Connect()
    .Subscribe(changeSet =>
    {
        // React to additions/removals
    });
```

---

### AsyncLazyDynamicDataCollection\<TValue\>

**Purpose**: Lazy-loading variant with `GetIfNeeded()` support.

```csharp
public abstract class AsyncLazyDynamicDataCollection<TValue>
    : AsyncDynamicDataCollection<TValue>
{
    public async ITask<IGetResult<IEnumerable<TValue>>> GetIfNeeded(CancellationToken ct)
    {
        if (HasValue)
            return CachedResult;  // Return cached

        return await Get(ct);  // Load if needed
    }
}
```

---

## Keyed Collections

### AsyncKeyedCollection\<TKey, TValue\>

**Location**: `LionFire.Data.Async.Reactive`

**Purpose**: Async collection with dictionary-like key-based access.

**This is the primary keyed collection type.**

```csharp
public class AsyncKeyedCollection<TKey, TValue> : ReactiveObject
    where TKey : notnull
    where TValue : notnull
{
    protected SourceCache<TValue, TKey> sourceCache;

    public IObservableCache<TValue, TKey> Items { get; }

    // Get single item
    public async Task<TValue?> GetItem(TKey key, CancellationToken ct)
    {
        await EnsureLoaded(ct);
        var lookup = sourceCache.Lookup(key);
        return lookup.HasValue ? lookup.Value : default;
    }

    // Get all items
    public abstract Task<IEnumerable<TValue>> LoadItems(CancellationToken ct);
}
```

#### Usage

```csharp
public class BotCollection : AsyncKeyedCollection<string, BotEntity>
{
    private readonly IBotRepository repository;

    public BotCollection(IBotRepository repository)
    {
        this.repository = repository;
    }

    protected override async Task<IEnumerable<BotEntity>> LoadItems(CancellationToken ct)
    {
        return await repository.GetAllBots(ct);
    }
}

// Usage
var bots = new BotCollection(repository);

// Get specific bot
var bot1 = await bots.GetItem("bot-001");

// Get all
await bots.EnsureLoaded();
foreach (var bot in bots.Items.Items)
{
    Console.WriteLine(bot.Name);
}

// Subscribe to changes
bots.Items.Connect()
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            Console.WriteLine($"{change.Key}: {change.Reason}");
        }
    });
```

---

### AsyncKeyValueCollection\<TKey, TValue\>

**Purpose**: Collection of KeyValuePair items.

```csharp
public class AsyncKeyValueCollection<TKey, TValue>
    : AsyncKeyedCollection<TKey, KeyValuePair<TKey, TValue>>
{
    // Items are KeyValuePair<TKey, TValue>
}
```

---

### AsyncReadOnlyKeyedCollection\<TKey, TValue\>

**Purpose**: Read-only variant of keyed collection.

**When to Use**: Collections that shouldn't be modified.

---

## DynamicData Integration

### SourceCache Basics

```csharp
// Create source cache
var cache = new SourceCache<Product, string>(p => p.Id);

// Add items
cache.AddOrUpdate(new Product { Id = "prod1", Name = "Product 1" });

// Remove items
cache.Remove("prod1");

// Batch edits
cache.Edit(updater =>
{
    updater.AddOrUpdate(product1);
    updater.AddOrUpdate(product2);
    updater.Remove("old-id");
});

// Subscribe to changes
cache.Connect()
    .Subscribe(changeSet =>
    {
        // Process changes
    });
```

---

### Transforming Collections

```csharp
// Source collection
AsyncKeyedCollection<string, BotEntity> bots;

// Transform to ViewModels
var botVMs = bots.Items.Connect()
    .Transform(bot => new BotVM(bot.Id, bot))
    .DisposeMany()  // Auto-dispose VMs
    .AsObservableCache();

// Filtered collection
var activeBots = bots.Items.Connect()
    .Filter(bot => bot.Enabled)
    .AsObservableCache();

// Sorted collection
var sortedBots = bots.Items.Connect()
    .Sort(SortExpressionComparer<BotEntity>.Ascending(b => b.Name))
    .AsObservableCache();
```

---

### Chaining Operations

```csharp
var processedData = sourceCache.Connect()
    .Filter(item => item.IsActive)        // Filter
    .Transform(item => item.ToDTO())       // Transform
    .Sort(SortExpressionComparer<DTO>.Ascending(d => d.Name))  // Sort
    .Top(10)                               // Take top 10
    .AsObservableCache();
```

---

## File System Collections

### AsyncFileDictionary\<TValue\>

**Location**: `LionFire.Data.Async.Reactive`

**Purpose**: File system-backed collection with automatic file watching.

```csharp
public class AsyncFileDictionary<TValue> : AsyncKeyedCollection<string, TValue>
{
    public AsyncFileDictionary(
        string directoryPath,
        Func<string, Task<TValue>> deserialize,
        string searchPattern = "*")
    {
        // Watches directory
        // Deserializes files on-demand
        // Updates cache when files change
    }
}
```

#### Usage

```csharp
var configFiles = new AsyncFileDictionary<Config>(
    directoryPath: "C:\\Configs",
    deserialize: async path =>
    {
        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<Config>(json);
    },
    searchPattern: "*.json"
);

// Load all configs
await configFiles.EnsureLoaded();

// Access specific config
var appConfig = await configFiles.GetItem("app-config.json");

// Subscribe to file changes
configFiles.Items.Connect()
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            Console.WriteLine($"Config {change.Key} {change.Reason}");
        }
    });
```

---

### SerializingFileDictionary\<TValue\>

**Purpose**: File collection with write support.

```csharp
public class SerializingFileDictionary<TValue>
    : AsyncFileDictionary<TValue>
    , IObservableWriter<string, TValue>
{
    public async ValueTask Write(string key, TValue value)
    {
        var filePath = Path.Combine(DirectoryPath, key);
        var json = JsonSerializer.Serialize(value);
        await File.WriteAllTextAsync(filePath, json);

        // Cache automatically updated via file watcher
    }

    public async ValueTask Remove(string key)
    {
        var filePath = Path.Combine(DirectoryPath, key);
        File.Delete(filePath);

        // Cache automatically updated via file watcher
    }
}
```

**Usage**:
```csharp
var configs = new SerializingFileDictionary<Config>(
    directoryPath: "C:\\Configs",
    serialize: config => JsonSerializer.Serialize(config),
    deserialize: json => JsonSerializer.Deserialize<Config>(json)
);

// Read
var config = await configs.GetItem("app.json");

// Write
await configs.Write("app.json", new Config { Theme = "Dark" });

// Delete
await configs.Remove("old-config.json");
```

**See**: [Persistence Patterns](persistence.md) for more file-backed patterns.

---

## Transformation Pipelines

### Filter → Transform → Sort

```csharp
public class ProcessedProductsCollection
{
    public ProcessedProductsCollection(AsyncKeyedCollection<string, Product> source)
    {
        ProcessedProducts = source.Items.Connect()
            .Filter(p => p.IsActive && p.Price > 0)  // Active with price
            .Transform(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                PriceFormatted = p.Price.ToString("C2")
            })
            .Sort(SortExpressionComparer<ProductDTO>.Ascending(dto => dto.Name))
            .AsObservableCache();
    }

    public IObservableCache<ProductDTO, string> ProcessedProducts { get; }
}
```

---

### Group by Category

```csharp
var groupedProducts = products.Items.Connect()
    .Group(p => p.Category)
    .Transform(group => new CategoryGroup
    {
        Category = group.Key,
        Products = group.Cache
    })
    .AsObservableCache();

// Usage
foreach (var categoryGroup in groupedProducts.Items)
{
    Console.WriteLine($"Category: {categoryGroup.Category}");
    foreach (var product in categoryGroup.Products.Items)
    {
        Console.WriteLine($"  - {product.Name}");
    }
}
```

---

### Merge Multiple Sources

```csharp
var allBots = Observable.Merge(
        collection1.Items.Connect(),
        collection2.Items.Connect(),
        collection3.Items.Connect())
    .AsObservableCache();
```

---

## Common Patterns

### Pattern 1: Lazy-Loading Collection

```csharp
public class LazyProductCollection : AsyncLazyDynamicDataCollection<Product>
{
    protected override async Task<IGetResult<IEnumerable<Product>>> GetImpl(
        CancellationToken ct)
    {
        var products = await LoadProductsFromAPI(ct);

        sourceCache.Edit(updater =>
        {
            updater.Clear();
            updater.AddOrUpdate(products);
        });

        return GetResult.Success(products);
    }
}

// Usage
var products = new LazyProductCollection();

// Load on demand
if (!products.HasValue)
{
    await products.GetIfNeeded();
}

// Use cached items
foreach (var product in products.Items.Items)
{
    Console.WriteLine(product.Name);
}
```

---

### Pattern 2: Live-Updating Collection

```csharp
public class LiveStockPrices : AsyncKeyedCollection<string, StockPrice>
{
    private readonly IStockPriceService priceService;

    public LiveStockPrices(IStockPriceService priceService)
    {
        this.priceService = priceService;

        // Poll for updates every 5 seconds
        Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(5))
            .Subscribe(async _ => await RefreshPrices());
    }

    private async Task RefreshPrices()
    {
        var prices = await priceService.GetLatestPrices();

        sourceCache.Edit(updater =>
        {
            foreach (var price in prices)
            {
                updater.AddOrUpdate(price, price.Symbol);
            }
        });
    }

    protected override async Task<IEnumerable<StockPrice>> LoadItems(CancellationToken ct)
    {
        return await priceService.GetLatestPrices();
    }
}
```

---

### Pattern 3: Filtered View of Collection

```csharp
public class FilteredBotsCollection
{
    private readonly AsyncKeyedCollection<string, BotEntity> allBots;

    [Reactive] private string? _exchangeFilter;

    public FilteredBotsCollection(AsyncKeyedCollection<string, BotEntity> allBots)
    {
        this.allBots = allBots;

        // Reactive filtered view
        FilteredBots = this.WhenAnyValue(x => x.ExchangeFilter)
            .Select(exchange =>
                allBots.Items.Connect()
                    .Filter(bot =>
                        string.IsNullOrEmpty(exchange) ||
                        bot.Exchange == exchange)
            )
            .Switch()  // Switch to new filtered observable
            .AsObservableCache();
    }

    public IObservableCache<BotEntity, string> FilteredBots { get; }
}
```

---

## Related Documentation

- **[Async Data Overview](README.md)** - Overview and quick start
- **[Persistence Patterns](persistence.md)** - File-backed collections
- **[Collection ViewModels](../../mvvm/collections.md)** - ViewModel wrappers
- **[LionFire.Data.Async.Reactive](../../../src/LionFire.Data.Async.Reactive/CLAUDE.md)** - Implementation details
- **[DynamicData Documentation](https://github.com/reactivemarbles/DynamicData)** - Reactive collections library

---

## Summary

**Collection Types**:
- **AsyncDynamicDataCollection<T>** - Unkeyed, integer-indexed
- **AsyncKeyedCollection<TKey, T>** - Keyed, dictionary-like
- **AsyncFileDictionary<T>** - File-backed, auto-watching

**Key Features**:
- Lazy loading with `GetIfNeeded()`
- DynamicData reactive caches
- Observable change notifications
- Transformation pipelines (Filter, Transform, Sort)
- File system watching

**Most Common**: `AsyncKeyedCollection<TKey, T>` for in-memory keyed data, `IObservableReader<TKey, T>` for workspace file-backed data.
