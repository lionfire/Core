# Async Data Architecture

**Overview**: LionFire's async data access layer provides a comprehensive, reactive-first approach to reading and writing data asynchronously. Built on interfaces like `IGetter`, `ISetter`, and `IValue`, it supports caching, lazy loading, observable operations, and seamless integration with MVVM and workspace patterns.

---

## Table of Contents

1. [Why Async-First Data Access?](#why-async-first-data-access)
2. [Architecture Layers](#architecture-layers)
3. [Core Patterns](#core-patterns)
4. [Integration Points](#integration-points)
5. [Design Philosophy](#design-philosophy)

---

## Why Async-First Data Access?

### The Challenge

Modern applications require:
- **Async operations** for I/O (files, HTTP, databases)
- **Caching** to avoid redundant fetches
- **Lazy loading** to defer expensive operations
- **Reactive UIs** that update automatically when data changes
- **Observable state** (is loading, has value, errors)
- **Separation** between stateless and stateful operations

### The Solution

LionFire's async data architecture provides:

✅ **Unified Interfaces**: `IGetter`, `ISetter`, `IValue` for all data operations
✅ **Reactive Integration**: ReactiveObject base with observable state
✅ **Caching Support**: `GetIfNeeded()` vs `Get()` for lazy loading
✅ **Observable Operations**: Subscribe to get/set operations as they occur
✅ **MVVM-Ready**: ViewModels wrap these interfaces for UI binding
✅ **Workspace Integration**: Powers workspace document persistence

---

## Architecture Layers

### Four-Layer Stack

```
┌────────────────────────────────────────────────────────────────┐
│  Layer 4: MVVM ViewModels                                      │
│  LionFire.Data.Async.Mvvm                                     │
│  - GetterVM, ValueVM                                           │
│  - ObservableReaderWriterItemVM                               │
└───────────────────────┬────────────────────────────────────────┘
                        │ Wraps
┌───────────────────────▼────────────────────────────────────────┐
│  Layer 3: Reactive Implementations                             │
│  LionFire.Data.Async.Reactive                                 │
│  - GetterRxO, ValueRxO                                         │
│  - AsyncDynamicDataCollection                                  │
└───────────────────────┬────────────────────────────────────────┘
                        │ Implements
┌───────────────────────▼────────────────────────────────────────┐
│  Layer 2: Reactive Abstractions                                │
│  LionFire.Data.Async.Reactive.Abstractions                    │
│  - IGetterRxO, ISetterRxO, IValueRxO                          │
└───────────────────────┬────────────────────────────────────────┘
                        │ Extends
┌───────────────────────▼────────────────────────────────────────┐
│  Layer 1: Core Abstractions                                    │
│  LionFire.Data.Async.Abstractions                             │
│  - IGetter, ISetter, IValue                                    │
│  - IStatelessGetter, ILazyGetter                              │
└────────────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

**Layer 1** (Core Abstractions):
- Define fundamental async data contracts
- Stateless vs stateful operations
- Observable operations interfaces
- TriggerMode and options
- Framework-agnostic

**Layer 2** (Reactive Abstractions):
- Add ReactiveObject requirement
- "RxO" marker interfaces
- Ensure property change notifications
- ReactiveUI integration contracts

**Layer 3** (Reactive Implementations):
- Concrete classes implementing Layer 2
- DynamicData collections
- File system-backed collections
- Async operation tracking

**Layer 4** (MVVM ViewModels):
- Wrap Layer 3 for UI binding
- Commands for get/set operations
- Loading/saving state properties
- Polling support

---

## Core Patterns

### 1. Getter Pattern (Read)

**Three variants**:

```csharp
// Stateless: Always fetches fresh
IStatelessGetter<T>
    .Get()  → Always retrieves

// Lazy: Caches result
ILazyGetter + IGetter<T>
    .GetIfNeeded()  → Retrieves only if not cached
    .Get()          → Always retrieves (invalidates cache)
    .DiscardValue() → Invalidate cache

// Reactive: Observable state
IGetterRxO<T> (extends IGetter)
    .IsLoading      → Reactive property
    .HasValue       → Reactive property
    .ReadCacheValue → Reactive property
```

### 2. Setter Pattern (Write)

```csharp
// Basic write
ISetter<T>
    .Set(value)  → Async write operation

// Reactive write
ISetterRxO<T>
    .IsSetting   → Reactive property
    .SetResults  → Observable stream of results
```

### 3. Value Pattern (Read + Write)

```csharp
// Combined operations
IValue<T> = IGetter<T> + ISetter<T>
    .GetIfNeeded()  → Lazy read
    .Set(value)     → Write
    .ReadCacheValue → Sync cache access

// Reactive combined
IValueRxO<T> = IGetterRxO<T> + ISetterRxO<T>
    .IsLoading   → Reactive
    .IsSetting   → Reactive
    .HasValue    → Reactive
```

### 4. Observable Collections

```csharp
// Async-loaded collection
AsyncDynamicDataCollection<T>
    .Items          → IObservableCache<T, int>
    .Get()          → Load items
    .GetIfNeeded()  → Load if not cached

// Keyed collection
AsyncKeyedCollection<TKey, T>
    .Items          → IObservableCache<T, TKey>
    .GetItem(key)   → Get specific item
```

### 5. Observable Persistence

```csharp
// From LionFire.Reactive
IObservableReader<TKey, TValue>
    .Keys           → IObservableCache<TKey>
    .Values         → IObservableCache<Optional<TValue>, TKey>
    .TryGetValue(key) → Async read

IObservableWriter<TKey, TValue>
    .Write(key, value) → Async write
```

**See**: [Persistence Integration](persistence-integration.md) for details.

---

## Integration Points

### 1. MVVM Integration

**ViewModels wrap data access**:

```
IGetter<WeatherData>
    ↓ Wrapped by
GetterVM<WeatherData>
    ↓ Bound to
Blazor/WPF UI
```

**Example**:
```csharp
var getter = new WeatherGetter(); // IGetter<WeatherData>
var vm = new GetterVM<WeatherData>(getter);

// UI binding
@bind-Value="vm.FullFeaturedSource.ReadCacheValue.Temperature"
```

**See**: [MVVM Data Binding](../mvvm/data-binding.md)

### 2. Workspace Integration

**Workspaces use `IObservableReader/Writer`**:

```
Workspace Service Provider
    ↓ Provides
IObservableReader<string, BotEntity>
IObservableWriter<string, BotEntity>
    ↓ Wrapped by
ObservableReaderWriterItemVM<string, BotEntity, BotVM>
    ↓ Bound to
Blazor UI
```

**See**: [Workspace Architecture](../workspaces/README.md)

### 3. Persistence Layer

**Persistence implementations use async interfaces**:

```csharp
// File-based persistence
public class FileValue<T> : IValue<T>
{
    public async Task<IGetResult<T>> Get(CancellationToken ct)
    {
        var json = await File.ReadAllTextAsync(path, ct);
        var value = JsonSerializer.Deserialize<T>(json);
        return GetResult<T>.Success(value);
    }

    public async Task<ISetResult<T>> Set(T value, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(value);
        await File.WriteAllTextAsync(path, json, ct);
        return SetResult<T>.Success(value);
    }
}
```

### 4. VOS Integration

**Virtual Object System uses handles with async access**:

```csharp
// Get handle to VOS object
var handle = vos.GetHandle<MyData>("vos://app/config");

// Handle implements IValue<T>
var data = await handle.GetIfNeeded();
data.Value.Setting = "new value";
await handle.Set(data.Value);
```

---

## Design Philosophy

### 1. Async-First

**All operations async by default**:
- I/O operations don't block
- Supports cancellation
- Integrates with async/await patterns
- CancellationToken throughout

### 2. Reactive-Friendly

**Designed for reactive UIs**:
- Observable operations
- Property change notifications
- Reactive state (IsLoading, HasValue)
- DynamicData collections

### 3. Layered Abstractions

**Progressive enhancement**:
```
Core → Reactive → Implementation → ViewModel
```

Each layer adds capabilities without breaking previous layers.

### 4. Caching Support

**Built-in caching**:
- `GetIfNeeded()` avoids redundant fetches
- `DiscardValue()` invalidates cache
- `HasValue` and `ReadCacheValue` for sync access
- `ReadState` tracking

### 5. Observable Everything

**All state is observable**:
- Get/Set operations → `IObservable<ITask>`
- Results → `IObservable<IGetResult<T>>`
- State → Reactive properties (IsLoading, IsSetting)
- Collections → DynamicData change sets

### 6. Separation of Concerns

**Read vs Write**:
- `IGetter` for read-only data
- `ISetter` for write-only operations
- `IValue` combines both

**Stateless vs Stateful**:
- `IStatelessGetter` always fetches
- `ILazyGetter` caches results
- Explicit cache invalidation

---

## Complete Stack Example

### Data Layer

```csharp
// Implement IGetter
public class WeatherGetter : IStatelessGetter<WeatherData>
{
    public async ITask<IGetResult<WeatherData>> Get(CancellationToken ct)
    {
        var data = await httpClient.GetFromJsonAsync<WeatherData>("...", ct);
        return GetResult<WeatherData>.Success(data);
    }
}
```

### Reactive Layer

```csharp
// Wrap in reactive implementation (automatic via upscaler)
IGetterRxO<WeatherData> reactiveGetter = GetterRxOUpscaler.Upscale(weatherGetter);

// Now has reactive properties
reactiveGetter.IsLoading  // Reactive property
reactiveGetter.HasValue   // Reactive property
```

### ViewModel Layer

```csharp
// Wrap in ViewModel
var vm = new GetterVM<WeatherData>(weatherGetter);

// Commands for UI
vm.GetCommand      // ReactiveCommand
vm.GetIfNeeded     // ReactiveCommand

// State for UI
vm.IsGetting       // Bool property
vm.HasValue        // Bool property
```

### UI Layer (Blazor)

```razor
@inject GetterVM<WeatherData> WeatherVM

@if (WeatherVM.IsGetting)
{
    <MudProgressCircular Indeterminate="true" />
}
else if (WeatherVM.HasValue)
{
    <div>Temperature: @WeatherVM.FullFeaturedSource.ReadCacheValue.Temp°</div>
}

<MudButton OnClick="@(() => WeatherVM.GetCommand.Execute())">
    Refresh
</MudButton>
```

---

## Summary

### Key Interfaces

| Interface | Purpose | Layer |
|-----------|---------|-------|
| `IStatelessGetter<T>` | Always fetch | Core |
| `IGetter<T>` | Fetch with caching | Core |
| `ISetter<T>` | Write operation | Core |
| `IValue<T>` | Read + Write | Core |
| `IGetterRxO<T>` | Reactive getter | Reactive |
| `IValueRxO<T>` | Reactive read/write | Reactive |
| `IObservableReader<TKey, T>` | Reactive persistence | Reactive (LionFire.Reactive) |

### Key Patterns

1. **Getter/Setter/Value** - Fundamental async operations
2. **Stateless vs Lazy** - Caching strategies
3. **Observable Operations** - Subscribe to operations and results
4. **ReactiveObject Integration** - Property change notifications
5. **DynamicData Collections** - Reactive collections
6. **Upscaling** - Convert non-reactive to reactive automatically

### Why This Architecture?

✅ **Flexibility**: Multiple strategies (stateless, lazy, reactive)
✅ **Composability**: Interfaces combine logically
✅ **Testability**: Mock interfaces for testing
✅ **UI-Friendly**: Observable state and reactive properties
✅ **Performance**: Caching and lazy loading built-in
✅ **Extensibility**: Implement interfaces for custom backends

### Related Documentation

- **[Core Async Patterns](async-patterns.md)** - Getter/Setter/Value details
- **[Reactive Data Patterns](reactive-data.md)** - RxO and ReactiveUI integration
- **[Persistence Integration](persistence-integration.md)** - IObservableReader/Writer
- **[MVVM Data Binding](../mvvm/data-binding.md)** - ViewModel integration
- **Library References**:
  - [LionFire.Data.Async.Abstractions](../../src/LionFire.Data.Async.Abstractions/CLAUDE.md)
  - [LionFire.Data.Async.Reactive.Abstractions](../../src/LionFire.Data.Async.Reactive.Abstractions/CLAUDE.md)
  - [LionFire.Data.Async.Reactive](../../src/LionFire.Data.Async.Reactive/CLAUDE.md)
  - [LionFire.Data.Async.Mvvm](../../src/LionFire.Data.Async.Mvvm/CLAUDE.md)
