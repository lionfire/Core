# Async Data Access

## Overview

LionFire's **Async Data Access System** provides a comprehensive framework for asynchronous data operations with built-in support for caching, lazy loading, reactive updates, and observable operations. This system forms the foundation for all data access in LionFire applications.

**Key Philosophy**: Async-first, reactive-enabled, with explicit control over caching and operation lifecycle.

---

## Quick Start

### Basic Get Operation

```csharp
// Simple stateless getter
IStatelessGetter<WeatherData> getter = weatherService.GetCurrentWeather();

// Retrieve data
var result = await getter.Get();
if (result.IsSuccess)
{
    Console.WriteLine($"Temperature: {result.Value.Temperature}");
}
```

---

### Cached Get (Lazy Loading)

```csharp
// Lazy getter with caching
IGetter<UserProfile> profileGetter = userService.GetProfile(userId);

// First call: retrieves from source
var result1 = await profileGetter.GetIfNeeded();

// Second call: returns cached value (no retrieval)
var result2 = await profileGetter.GetIfNeeded();

// Force refresh
profileGetter.DiscardValue();
var result3 = await profileGetter.GetIfNeeded();  // Retrieves again
```

---

### Read/Write (IValue)

```csharp
// Combined read/write
IValue<AppSettings> settings = configService.GetSettings();

// Load
await settings.GetIfNeeded();
var currentTheme = settings.ReadCacheValue.Theme;

// Modify
settings.ReadCacheValue.Theme = "Dark";

// Save
await settings.Set(settings.ReadCacheValue);
```

---

## Core Concepts

### The Three Tiers

```
┌────────────────────────────────────────────────────────────┐
│                    Abstractions Tier                        │
│  LionFire.Data.Async.Abstractions                          │
│  - IGetter<T>, ISetter<T>, IValue<T>                       │
│  - Interface definitions only                              │
└─────────────────────┬──────────────────────────────────────┘
                      │
                      ↓ Implemented by
┌────────────────────────────────────────────────────────────┐
│                   Reactive Tier                             │
│  LionFire.Data.Async.Reactive                              │
│  - GetterRxO<T>, SetterRxO<T>, ValueRxO<T>                │
│  - ReactiveObject implementations                          │
│  - Observable properties and operations                    │
└─────────────────────┬──────────────────────────────────────┘
                      │
                      ↓ Wrapped by
┌────────────────────────────────────────────────────────────┐
│                    ViewModel Tier                           │
│  LionFire.Data.Async.Mvvm                                  │
│  - GetterVM<T>, ValueVM<T>                                 │
│  - Commands, loading indicators                            │
│  - UI-ready ViewModels                                     │
└────────────────────────────────────────────────────────────┘
```

---

### Interface Hierarchy

```
IGetter (marker)
    ├── IStatelessGetter<T>      ← Always retrieves, no caching
    └── IGetter<T>               ← Lazy loading with cache
            ├── IGetterRxO<T>    ← ReactiveObject variant
            └── IValue<T>        ← Read + Write
                    └── IValueRxO<T>  ← ReactiveObject variant
```

---

## Core Patterns

### 1. Getter Pattern (Read)

**Stateless** - Always retrieves:
```csharp
IStatelessGetter<Data> getter;
await getter.Get();  // Retrieves from source
await getter.Get();  // Retrieves again
```

**Lazy** - Caches result:
```csharp
IGetter<Data> lazyGetter;
await lazyGetter.GetIfNeeded();  // Retrieves from source
await lazyGetter.GetIfNeeded();  // Returns cached value

lazyGetter.DiscardValue();       // Clear cache
await lazyGetter.GetIfNeeded();  // Retrieves again
```

**See**: [Getters and Setters Guide](getters-setters.md)

---

### 2. Setter Pattern (Write)

```csharp
ISetter<Data> setter;
var result = await setter.Set(newValue);

if (result.IsSuccess)
{
    Console.WriteLine("Saved successfully");
}
```

**Features**:
- Async write operations
- Result tracking (success/failure)
- Observable write operations
- Trigger modes (Auto, Manual, AutoImmediate)

**See**: [Getters and Setters Guide](getters-setters.md)

---

### 3. Value Pattern (Read + Write)

```csharp
IValue<Config> config;

// Read
await config.GetIfNeeded();
var theme = config.ReadCacheValue.Theme;

// Write
config.ReadCacheValue.Theme = "Dark";
await config.Set(config.ReadCacheValue);
```

**Benefits**:
- Single interface for both read and write
- Cached reads
- Explicit saves
- Observable state

---

### 4. Observable Operations

All async operations are observable:

```csharp
IGetter<Data> getter;

// Subscribe to get operations
getter.GetOperations.Subscribe(task =>
{
    Console.WriteLine("Get operation started");
});

// Subscribe to results
getter.GetResults.Subscribe(result =>
{
    if (result.IsSuccess)
        Console.WriteLine($"Got: {result.Value}");
});

// Subscribe to loading state
getter.WhenAnyValue(g => g.IsLoading)
    .Subscribe(isLoading => ShowSpinner(isLoading));
```

**See**: [Observable Operations](observable-operations.md)

---

### 5. Caching Strategies

```csharp
IGetter<Data> getter;

// Manual cache control
await getter.GetIfNeeded();     // Use cache if available
getter.DiscardValue();          // Clear cache
await getter.Get();             // Force retrieval

// Check cache state
if (getter.HasValue)
{
    var cached = getter.ReadCacheValue;  // Synchronous cache access
}
```

**See**: [Caching Strategies](caching-strategies.md)

---

## Library Ecosystem

### LionFire.Data.Async.Abstractions

**Purpose**: Core interface definitions

**Key Types**:
- `IGetter<T>`, `ISetter<T>`, `IValue<T>`
- `IStatelessGetter<T>`, `ILazyGetter<T>`
- `IGetResult<T>`, `ISetResult<T>`
- `ReadState`, `WriteState`, `TriggerMode`

**See**: [Project Documentation](../../../src/LionFire.Data.Async.Abstractions/CLAUDE.md)

---

### LionFire.Data.Async.Reactive.Abstractions

**Purpose**: ReactiveUI marker interfaces

**Key Types**:
- `IGetterRxO<T>`, `ISetterRxO<T>`, `IValueRxO<T>`
- `IReactiveObjectEx`

**See**: [Project Documentation](../../../src/LionFire.Data.Async.Reactive.Abstractions/CLAUDE.md)

---

### LionFire.Data.Async.Reactive

**Purpose**: ReactiveUI implementations

**Key Types**:
- `GetterRxO<T>`, `SetterRxO<T>`, `ValueRxO<T>`
- `AsyncDynamicDataCollection<T>`
- `AsyncKeyedCollection<TKey, T>`
- File system collections

**See**: [Project Documentation](../../../src/LionFire.Data.Async.Reactive/CLAUDE.md)

---

### LionFire.Data.Async.Mvvm

**Purpose**: ViewModel wrappers

**Key Types**:
- `GetterVM<T>`, `ValueVM<T>`
- `ObservableReaderItemVM<TKey, T, TVM>`
- `ObservableDataVM<TKey, T, TVM>`

**See**: [Project Documentation](../../../src/LionFire.Data.Async.Mvvm/CLAUDE.md)

---

## Common Scenarios

### Scenario 1: Load User Profile

```csharp
// Service provides getter
public class UserService
{
    public IGetter<UserProfile> GetProfile(string userId)
    {
        return new GetterRxO<UserProfile>(
            async ct =>
            {
                var profile = await database.LoadProfile(userId, ct);
                return GetResult.Success(profile);
            });
    }
}

// Usage
var profileGetter = userService.GetProfile("user123");
await profileGetter.GetIfNeeded();

// Display in UI
Console.WriteLine($"Name: {profileGetter.ReadCacheValue.Name}");
```

---

### Scenario 2: Save Configuration

```csharp
// Service provides value
public class ConfigService
{
    public IValue<AppSettings> GetSettings()
    {
        return new ValueRxO<AppSettings>(
            loadFunc: async ct => await File.ReadAsync("settings.json", ct),
            saveFunc: async (value, ct) => await File.WriteAsync("settings.json", value, ct)
        );
    }
}

// Usage
var settings = configService.GetSettings();
await settings.GetIfNeeded();

// Modify
settings.ReadCacheValue.Theme = "Dark";

// Save
await settings.Set(settings.ReadCacheValue);
```

---

### Scenario 3: File-Backed Collection

```csharp
// Workspace documents
IObservableReader<string, BotEntity> botReader = GetBotReader();

// Subscribe to changes
botReader.Values.Connect()
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            Console.WriteLine($"{change.Key}: {change.Reason}");
        }
    });

// Read item
var result = await botReader.TryGetValue("bot-001");
if (result.HasValue)
{
    Console.WriteLine($"Bot: {result.Value.Name}");
}
```

**See**: [Persistence Patterns](persistence.md)

---

## When to Use What

| Scenario | Interface | Why |
|----------|-----------|-----|
| **API call** | `IStatelessGetter<T>` | Fresh data every time |
| **Config file** | `IGetter<T>` | Cache, reload on change |
| **User profile** | `IValue<T>` | Read + Write with cache |
| **Real-time data** | `IStatelessGetter<T>` + polling | Always fresh |
| **File watching** | `IObservableReader<TKey, T>` | File system changes |
| **Collection** | `AsyncKeyedCollection<TKey, T>` | Multiple items with keys |
| **Workspace docs** | `IObservableReader<TKey, T>` | File-backed, reactive |

---

## Documentation Structure

### **[Getters and Setters Guide](getters-setters.md)**
Comprehensive guide to `IGetter`, `ISetter`, and `IValue` patterns.

**Topics**:
- Stateless vs lazy getters
- Cache management
- Write operations
- Result handling

---

### **[Observable Operations](observable-operations.md)**
How to work with observable async operations.

**Topics**:
- `IObservableGetOperations`
- Loading state tracking
- Result streams
- Reactive patterns

---

### **[Collections](collections.md)**
Working with async collections.

**Topics**:
- `AsyncDynamicDataCollection<T>`
- `AsyncKeyedCollection<TKey, T>`
- Keyed vs unkeyed collections
- DynamicData integration

---

### **[Persistence Patterns](persistence.md)**
File-based and database persistence.

**Topics**:
- `IObservableReader/Writer`
- File system watching
- HJSON serialization
- Write-through caching

---

### **[Caching Strategies](caching-strategies.md)**
Cache management and invalidation.

**Topics**:
- When to cache
- `DiscardValue()` patterns
- `ReadState` management
- Polling and auto-refresh

---

## Architecture Integration

### Async Data in the Stack

```
Application Code
    ↓ Uses
ViewModels (GetterVM, ValueVM)
    ↓ Wraps
Reactive Implementations (GetterRxO, ValueRxO)
    ↓ Implements
Async Abstractions (IGetter, IValue)
    ↓ Backed by
Data Sources (Files, Database, APIs)
```

### Integration with Other Systems

**MVVM**:
- ViewModels wrap async data objects
- Commands expose Get/Set operations
- Observable properties for UI binding

**Workspaces**:
- `IObservableReader/Writer` for workspace documents
- File-backed collections with reactive updates

**VOS**:
- Handles backed by async data access
- Mount points provide getters/setters
- Lazy loading throughout VOS tree

---

## Best Practices

### 1. Use Appropriate Interface

```csharp
// ✅ Stateless for always-fresh data
IStatelessGetter<StockPrice> priceGetter;  // API call every time

// ✅ Lazy for cacheable data
IGetter<UserProfile> profileGetter;  // Cache until discarded

// ✅ Value for read/write
IValue<Config> configValue;  // Cache + save capability
```

---

### 2. Handle Results Properly

```csharp
// ✅ Check result status
var result = await getter.Get();
if (result.IsSuccess)
{
    UseData(result.Value);
}
else
{
    LogError(result.Error);
}

// ❌ Don't assume success
var value = (await getter.Get()).Value;  // May be null/throw!
```

---

### 3. Manage Cache Lifecycle

```csharp
// ✅ Discard when data changes
await userService.UpdateProfile(newProfile);
profileGetter.DiscardValue();  // Clear stale cache

// ✅ Check cache before expensive operation
if (!getter.HasValue)
{
    await getter.GetIfNeeded();
}
```

---

### 4. Use Observable Operations for UI

```csharp
// ✅ Subscribe to loading state
getter.WhenAnyValue(g => g.IsLoading)
    .Subscribe(isLoading => ShowSpinner(isLoading));

// ✅ Subscribe to results
getter.GetResults
    .Where(r => r.IsSuccess)
    .Subscribe(result => UpdateUI(result.Value));
```

---

## Related Documentation

### Architecture
- **[Async Data Architecture](../../architecture/data/async/README.md)** - High-level architecture
- **[Reactive Data Patterns](../../architecture/data/async/reactive-data.md)** - Reactive integration

### Domain Guides
- **[Getters and Setters](getters-setters.md)** - Core patterns ⭐ START HERE
- **[Observable Operations](observable-operations.md)** - Reactive features
- **[Collections](collections.md)** - Working with collections
- **[Persistence](persistence.md)** - File and database persistence
- **[Caching Strategies](caching-strategies.md)** - Cache management

### Project Documentation
- **[LionFire.Data.Async.Abstractions](../../../src/LionFire.Data.Async.Abstractions/CLAUDE.md)** - Interface reference
- **[LionFire.Data.Async.Reactive](../../../src/LionFire.Data.Async.Reactive/CLAUDE.md)** - ReactiveUI implementations
- **[LionFire.Data.Async.Mvvm](../../../src/LionFire.Data.Async.Mvvm/CLAUDE.md)** - ViewModel wrappers

---

## Summary

**Async Data Access System** provides:

**Core Patterns**:
- **IGetter<T>** - Async read with caching
- **ISetter<T>** - Async write operations
- **IValue<T>** - Combined read/write
- **Observable Operations** - Reactive streams
- **Collections** - Async keyed/unkeyed collections

**Key Benefits**:
- Explicit cache control
- Observable operation lifecycle
- ReactiveUI integration
- Lazy loading support
- Polling and auto-refresh

**Most Common**:
- 80% of scenarios use `IGetter<T>` (lazy, cached reads)
- 15% use `IValue<T>` (read/write config files)
- 5% use `IStatelessGetter<T>` (always-fresh API calls)

**Next Steps**:
1. Read [Getters and Setters Guide](getters-setters.md) for detailed patterns
2. Learn [Observable Operations](observable-operations.md) for reactive features
3. Explore [Collections](collections.md) for multi-item scenarios
