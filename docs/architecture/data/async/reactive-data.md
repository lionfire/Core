# Reactive Data Patterns

**Overview**: Exploration of ReactiveUI-integrated async data patterns. The "RxO" (ReactiveObject) variants add property change notifications, observable state, and reactive integration to core async patterns.

---

## Table of Contents

1. [ReactiveUI Integration](#reactiveui-integration)
2. [RxO Marker Interfaces](#rxo-marker-interfaces)
3. [Property Change Notifications](#property-change-notifications)
4. [Observable Collections](#observable-collections)
5. [Upscaling Pattern](#upscaling-pattern)

---

## ReactiveUI Integration

### IReactiveObjectEx

**Foundation** for all reactive data interfaces:

```csharp
public interface IReactiveObjectEx
    : IReactiveObject
    , IReactiveNotifyPropertyChanged<IReactiveObject>
{
    // Ensures implementation derives from ReactiveObject
    // Provides INotifyPropertyChanged
    // Enables WhenAnyValue patterns
}
```

**Purpose**:
- Marker that implementation must use `ReactiveObject` base
- Guarantees property change notifications
- Enables ReactiveUI patterns (WhenAnyValue, etc.)

---

## RxO Marker Interfaces

### IGetterRxO\<T\>

**IGetter that is also a ReactiveObject**:

```csharp
public interface IGetterRxO<out TValue>
    : IReactiveObjectEx
    , IGetter<TValue>
{
    // No additional methods - just ensures ReactiveObject base
}
```

**What It Guarantees**:
- `IsLoading` is a reactive property
- `HasValue` is a reactive property
- `ReadCacheValue` raises PropertyChanged when cache updates
- Can use `WhenAnyValue(x => x.Property)`

**Usage**:
```csharp
IGetterRxO<WeatherData> getter = new GetterRxO<WeatherData>(...);

// React to loading state
getter.WhenAnyValue(g => g.IsLoading)
    .Subscribe(isLoading => UpdateUI(isLoading));

// React to value changes
getter.WhenAnyValue(g => g.ReadCacheValue)
    .WhereNotNull()
    .Subscribe(data => DisplayWeather(data));

// React to cache validity
getter.WhenAnyValue(g => g.HasValue)
    .Subscribe(hasValue => ShowEmptyState(!hasValue));
```

### ISetterRxO\<T\>

**ISetter that is also a ReactiveObject**:

```csharp
public interface ISetterRxO<TValue>
    : IReactiveObjectEx
    , ISetter<TValue>
{
    // Ensures IsSetting property is reactive
}
```

**Usage**:
```csharp
ISetterRxO<AppConfig> setter = ...;

// React to save state
setter.WhenAnyValue(s => s.IsSetting)
    .Subscribe(isSetting =>
    {
        SaveButton.IsEnabled = !isSetting;
        ProgressBar.IsVisible = isSetting;
    });

// React to save results
setter.SetResults
    .Subscribe(result =>
    {
        if (result.IsSuccess)
            ShowSuccessMessage();
        else
            ShowErrorMessage(result.ErrorMessage);
    });
```

### IValueRxO\<T\>

**IValue that is also a ReactiveObject**:

```csharp
public interface IValueRxO<T>
    : IReactiveObjectEx
    , IValue<T>
    , IGetterRxO<T>
    , ISetterRxO<T>
{
    // Full-featured read/write with reactive properties
}
```

**Complete Reactive Integration**:
```csharp
IValueRxO<UserProfile> profile = ...;

// React to any state change
var anyBusy = profile.WhenAnyValue(
    p => p.IsLoading,
    p => p.IsSetting,
    (loading, setting) => loading || setting);

anyBusy.Subscribe(isBusy => ProgressIndicator.IsVisible = isBusy);

// React to data availability
profile.WhenAnyValue(p => p.HasValue)
    .Subscribe(hasValue => ContentPanel.IsVisible = hasValue);

// React to cached value changes
profile.WhenAnyValue(p => p.ReadCacheValue)
    .WhereNotNull()
    .Subscribe(data => UpdateUI(data));
```

---

## Property Change Notifications

### Reactive Properties in Data Objects

**All state must be reactive**:

```csharp
public class GetterRxO<T> : ReactiveObject, IGetterRxO<T>
{
    // ✅ CORRECT: Reactive properties
    [Reactive] private bool _isLoading;
    [Reactive] private bool _hasValue;

    private T? _readCacheValue;
    public T? ReadCacheValue
    {
        get => _readCacheValue;
        private set => this.RaiseAndSetIfChanged(ref _readCacheValue, value);
    }

    // ❌ WRONG: Non-reactive property
    // public bool IsLoading { get; set; }  // Won't notify UI!
}
```

### Raising PropertyChanged for Cache Updates

**When cache updates, notify UI**:

```csharp
public async ITask<IGetResult<T>> Get(CancellationToken ct)
{
    IsLoading = true;  // Notifies UI automatically

    var data = await FetchData(ct);
    _readCacheValue = data;

    // CRITICAL: Raise PropertyChanged for cache value
    this.RaisePropertyChanged(nameof(ReadCacheValue));

    HasValue = true;  // Notifies UI automatically
    IsLoading = false;  // Notifies UI automatically

    return GetResult<T>.Success(data);
}
```

### WhenAnyValue Patterns

**Observe single property**:
```csharp
getter.WhenAnyValue(g => g.IsLoading)
    .Subscribe(isLoading => Console.WriteLine($"Loading: {isLoading}"));
```

**Observe multiple properties**:
```csharp
value.WhenAnyValue(
        v => v.IsLoading,
        v => v.IsSetting,
        v => v.HasValue,
        (loading, setting, hasValue) => new { loading, setting, hasValue })
    .Subscribe(state =>
    {
        IsBusy = state.loading || state.setting;
        CanEdit = state.hasValue && !IsBusy;
    });
```

**Derive computed properties**:
```csharp
public class MyViewModel : ReactiveObject
{
    public IGetterRxO<Data> DataGetter { get; }

    private readonly ObservableAsPropertyHelper<bool> _isBusy;
    public bool IsBusy => _isBusy.Value;

    public MyViewModel(IGetterRxO<Data> dataGetter)
    {
        DataGetter = dataGetter;

        _isBusy = DataGetter.WhenAnyValue(g => g.IsLoading)
            .ToProperty(this, x => x.IsBusy);
    }
}
```

---

## Observable Collections

### AsyncDynamicDataCollection\<T\>

**Base class** for async-loaded reactive collections:

```csharp
public abstract partial class AsyncDynamicDataCollection<TValue>
    : DynamicDataCollection<TValue>
    where TValue : notnull
{
    // Protected SourceCache
    protected SourceCache<TValue, int> sourceCache = new(x => x.GetHashCode());

    // Public observable cache
    public IObservableCache<TValue, int> Items => sourceCache.AsObservableCache();

    // Observable state
    [Reactive] private bool isLoading;
    [Reactive] private bool hasValue;

    // Abstract method to load items
    protected abstract ITask<IGetResult<IEnumerable<TValue>>> GetImpl(CancellationToken ct);

    // Thread-safe get with operation tracking
    public override async ITask<IGetResult<IEnumerable<TValue>>> Get(CancellationToken ct)
    {
        lock (getInProgressLock)
        {
            if (getOperations.Value != null && !IsCompleted)
                return getOperations.Value;  // Return in-progress operation

            IsLoading = true;
            var task = GetImpl(ct);
            getOperations.OnNext(task);
            return await task;
        }
    }
}
```

**Usage**:
```csharp
public class ProductsCollection : AsyncDynamicDataCollection<Product>
{
    private readonly IProductApi api;

    protected override async ITask<IGetResult<IEnumerable<Product>>> GetImpl(CancellationToken ct)
    {
        var products = await api.GetAllProducts(ct);
        return GetResult<IEnumerable<Product>>.Success(products);
    }
}

// Use
var products = new ProductsCollection(api);

// Load products
await products.Get();

// Subscribe to changes
products.Items
    .Connect()
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            Console.WriteLine($"{change.Reason}: {change.Current}");
        }
    });

// Items automatically added to SourceCache
// UI bound to Items.Connect() updates automatically
```

### AsyncKeyedCollection\<TKey, T\>

**Keyed collection** with reactive updates:

```csharp
public class BotCollection : AsyncKeyedCollection<string, BotEntity>
{
    private readonly IBotRepository repo;

    protected override async ITask<IGetResult<IEnumerable<BotEntity>>> GetImpl(CancellationToken ct)
    {
        var bots = await repo.GetAllBots(ct);
        return GetResult<IEnumerable<BotEntity>>.Success(bots);
    }

    public async Task<BotEntity?> GetBot(string id)
    {
        await EnsureLoaded();
        var lookup = Items.Lookup(id);
        return lookup.HasValue ? lookup.Value : null;
    }
}

// Usage
var bots = new BotCollection(repo);
await bots.Get();

// Access by key
var bot = await bots.GetBot("bot-alpha");

// Observe specific bot changes
bots.Items
    .Connect()
    .Filter(b => b.Id == "bot-alpha")
    .Subscribe(changeSet => UpdateUI(changeSet));
```

### Transforming Collections

**Transform source to derived collection**:

```csharp
public class UserVMCollection
{
    public UserVMCollection(IObservableCache<User, int> users)
    {
        // Transform users to ViewModels
        UserVMs = users
            .Connect()
            .Transform(user => new UserVM(user))
            .DisposeMany()  // Dispose VMs when users removed
            .AsObservableCache();
    }

    public IObservableCache<UserVM, int> UserVMs { get; }
}
```

---

## Upscaling Pattern

### Converting Non-Reactive to Reactive

**Problem**: You have `IGetter<T>` but need `IGetterRxO<T>`.

**Solution**: Upscaler automatically wraps in reactive adapter.

```csharp
public static class GetterRxOUpscaler
{
    public static IGetterRxO<T> Upscale<T>(IGetter getter)
    {
        // Already reactive? Return as-is
        if (getter is IGetterRxO<T> rxo)
            return rxo;

        // Wrap in reactive adapter
        return new GetterRxOAdapter<T>(getter);
    }
}
```

**Adapter Implementation**:
```csharp
public class GetterRxOAdapter<T> : ReactiveObject, IGetterRxO<T>
{
    private readonly IGetter<T> inner;

    public GetterRxOAdapter(IGetter<T> inner)
    {
        this.inner = inner;

        // Subscribe to inner's observable operations (if available)
        if (inner is IObservableGetOperations<T> obs)
        {
            obs.GetOperations.Subscribe(async task =>
            {
                IsLoading = true;
                var result = await task;
                IsLoading = false;
                HasValue = result.HasValue;
                this.RaisePropertyChanged(nameof(ReadCacheValue));
            });
        }
    }

    // Reactive properties
    [Reactive] private bool _isLoading;
    [Reactive] private bool _hasValue;

    // Delegate to inner
    public T? ReadCacheValue => inner.ReadCacheValue;
    public ITask<IGetResult<T>> Get(CancellationToken ct) => inner.Get(ct);
    public ITask<IGetResult<T>> GetIfNeeded(CancellationToken ct) => inner.GetIfNeeded(ct);

    // ... etc
}
```

**Automatic Upscaling** in GetterVM:
```csharp
public class GetterVM<T> : ReactiveObject
{
    private IGetter? source;
    public IGetter? Source
    {
        get => source;
        set
        {
            source = value;

            // Automatically upscale to IGetterRxO<T>
            FullFeaturedSource = value != null
                ? GetterRxOUpscaler.Upscale<T>(value)
                : null;
        }
    }

    public IGetterRxO<T>? FullFeaturedSource { get; private set; }
}
```

---

## Summary

### RxO Interface Benefits

1. **Reactive Properties** - IsLoading, HasValue, ReadCacheValue all reactive
2. **WhenAnyValue** - Can observe any property changes
3. **UI Binding** - Direct binding to XAML/Blazor
4. **Automatic Updates** - UI refreshes automatically
5. **Observable Chains** - Compose reactive pipelines

### Upscaling Benefits

1. **Compatibility** - Non-reactive getters work with reactive VMs
2. **Automatic** - GetterVM handles upscaling transparently
3. **Type-Safe** - Preserves generic types
4. **Zero-Cost** - If already reactive, no wrapping occurs

### When to Use RxO

**Use IGetterRxO/ISetterRxO/IValueRxO when**:
- Building UI components
- Need property change notifications
- Using ReactiveUI patterns
- Binding to XAML/Blazor

**Use base interfaces (IGetter/ISetter/IValue) when**:
- Building backend services
- No UI involvement
- Performance-critical (avoid ReactiveObject overhead)
- Testing (simpler mocks)

### Related Documentation

- **[Core Async Patterns](async-patterns.md)** - Base interfaces
- **[Async Data Overview](README.md)** - Architecture overview
- **[MVVM Data Binding](../mvvm/data-binding.md)** - ViewModel integration
- **Library References**:
  - [LionFire.Data.Async.Reactive.Abstractions](../../src/LionFire.Data.Async.Reactive.Abstractions/CLAUDE.md)
  - [LionFire.Data.Async.Reactive](../../src/LionFire.Data.Async.Reactive/CLAUDE.md)
