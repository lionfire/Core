# LionFire.Data.Async.Reactive.Abstractions

## Overview

ReactiveUI-specific abstractions for async data access. Extends LionFire.Data.Async.Abstractions with ReactiveObject integration, providing marker interfaces that combine data access patterns with reactive property change notifications.

**Layer**: Toolkit (Abstractions)
**Target**: .NET 9.0
**Root Namespace**: `LionFire`

## Key Dependencies

- **ReactiveUI** - Reactive MVVM framework
- **LionFire.Data.Async.Abstractions** - Core async data abstractions
- **LionFire.Data.Abstractions** - Synchronous data patterns

## Core Purpose

This library defines "RxO" (ReactiveObject) variants of async data interfaces. These interfaces add `IReactiveObjectEx` as a base, ensuring implementations derive from ReactiveUI's `ReactiveObject` and provide property change notifications.

## Key Interfaces

### IReactiveObjectEx

```csharp
public interface IReactiveObjectEx
    : IReactiveObject
    , IReactiveNotifyPropertyChanged<IReactiveObject>
{
    // Combines ReactiveUI's IReactiveObject with property change notifications
}
```

**Purpose:**
- Marker for ReactiveUI integration
- Ensures `INotifyPropertyChanged` support
- Provides ReactiveUI-specific features (WhenAnyValue, etc.)

### IGetterRxO<TValue>

```csharp
public interface IGetterRxO<out TValue>
    : IReactiveObjectEx
    , IGetter<TValue>
{
    // IGetter that is also a ReactiveObject
}
```

**Characteristics:**
- All properties are reactive (can use `WhenAnyValue()`)
- `IsLoading`, `HasValue`, `ReadCacheValue` raise property changes
- Observable operations built-in
- Can be bound to UI directly

**Usage:**
```csharp
IGetterRxO<MyData> getter = ...;

// React to property changes
getter.WhenAnyValue(g => g.HasValue)
    .Subscribe(hasValue => UpdateUI(hasValue));

// React to loading state
getter.WhenAnyValue(g => g.IsLoading)
    .Subscribe(isLoading => ShowSpinner(isLoading));

// React to cached value changes
getter.WhenAnyValue(g => g.ReadCacheValue)
    .Subscribe(value => DisplayData(value));
```

### ISetterRxO<TValue>

```csharp
public interface ISetterRxO<TValue>
    : IReactiveObjectEx
    , ISetter<TValue>
{
    // ISetter that is also a ReactiveObject
}
```

**Characteristics:**
- `IsSetting` property is reactive
- Observable set operations
- Can track write progress in UI

**Usage:**
```csharp
ISetterRxO<MyData> setter = ...;

// React to save state
setter.WhenAnyValue(s => s.IsSetting)
    .Subscribe(isSetting => EnableSaveButton(!isSetting));
```

### IValueRxO<T>

```csharp
public interface IValueRxO<T>
    : IReactiveObjectEx
    , IValue<T>
    , IGetterRxO<T>
    , ISetterRxO<T>
{
    // Combined read/write that is also a ReactiveObject
}
```

**Full-Featured Interface:**
- All read/write operations
- All state reactive
- Complete observability
- Direct UI binding support

**Usage:**
```csharp
IValueRxO<UserProfile> profile = ...;

// Auto-load on first access
await profile.GetIfNeeded();

// React to any property changes
profile.WhenAnyValue(
    p => p.IsLoading,
    p => p.IsSetting,
    p => p.HasValue,
    (loading, setting, hasValue) => new { loading, setting, hasValue })
    .Subscribe(state => UpdateUI(state));

// Bind cached value to UI
this.WhenAnyValue(vm => vm.Profile.ReadCacheValue)
    .BindTo(this, vm => vm.ProfileDisplay.DataContext);
```

## Directory Structure

```
ReactiveUI_/
  IReactiveObjectEx.cs  - Base reactive interface
Values/
  Read/
    IGetterRxO.cs       - Reactive getter interface
  Write/
    ISetterRxO.cs       - Reactive setter interface
  ReadWrite/
    IValueRxO.cs        - Reactive value interface
Collections/
  (placeholder for future reactive collection interfaces)
```

## Design Philosophy

**Minimal Marker Interfaces:**
- These are primarily marker interfaces
- Combine existing async patterns with ReactiveUI
- No new methods - just ensure ReactiveObject base

**Type Safety:**
- Distinguishes reactive implementations from non-reactive
- Enables specialized implementations
- Supports type-based service registration

**UI-Oriented:**
- Designed for direct binding to XAML/Blazor/Avalonia
- All state observable for reactive UIs
- Property change notifications automatic

## Usage Patterns

### Pattern 1: ViewModel with Reactive Data

```csharp
public class MyViewModel : ReactiveObject
{
    public MyViewModel(IGetterRxO<UserData> userDataGetter)
    {
        UserDataGetter = userDataGetter;

        // Auto-refresh when user changes
        this.WhenAnyValue(vm => vm.SelectedUserId)
            .Subscribe(_ =>
            {
                UserDataGetter.DiscardValue();
                UserDataGetter.GetIfNeeded().FireAndForget();
            });

        // Update UI when data loads
        UserDataGetter.WhenAnyValue(g => g.ReadCacheValue)
            .WhereNotNull()
            .Subscribe(data => CurrentData = data);
    }

    public IGetterRxO<UserData> UserDataGetter { get; }

    [Reactive]
    private string selectedUserId;

    [Reactive]
    private UserData? currentData;
}
```

### Pattern 2: Upscaling Non-Reactive to Reactive

When you have a non-reactive `IGetter<T>` but need `IGetterRxO<T>`:

```csharp
public static class GetterRxOUpscaler
{
    public static IGetterRxO<T> Upscale<T>(IGetter getter)
    {
        if (getter is IGetterRxO<T> rxo) return rxo;

        // Wrap in a reactive adapter
        return new GetterRxOAdapter<T>(getter);
    }
}

// Adapter implementation
public class GetterRxOAdapter<T> : ReactiveObject, IGetterRxO<T>
{
    private readonly IGetter<T> inner;

    public GetterRxOAdapter(IGetter<T> inner)
    {
        this.inner = inner;

        // Subscribe to inner operations and update reactive properties
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

    [Reactive] private bool isLoading;
    [Reactive] private bool hasValue;

    public T? ReadCacheValue => inner.ReadCacheValue;

    // Delegate all IGetter<T> methods to inner
    public ITask<IGetResult<T>> Get(CancellationToken ct) => inner.Get(ct);
    public ITask<IGetResult<T>> GetIfNeeded(CancellationToken ct) => inner.GetIfNeeded(ct);
    // ... etc
}
```

### Pattern 3: Direct UI Binding

**Avalonia/WPF:**
```xml
<TextBlock Text="{Binding DataGetter.ReadCacheValue.Name}" />
<ProgressBar IsVisible="{Binding DataGetter.IsLoading}" />
<Button Command="{Binding DataGetter.GetCommand}"
        IsEnabled="{Binding DataGetter.CanGet}">
    Refresh
</Button>
```

**Blazor with ReactiveUI:**
```csharp
@inject IGetterRxO<WeatherData> WeatherGetter

<div>
    @if (WeatherGetter.IsLoading)
    {
        <p>Loading...</p>
    }
    else if (WeatherGetter.HasValue)
    {
        <p>Temperature: @WeatherGetter.ReadCacheValue.Temp</p>
    }
</div>

@code {
    protected override void OnInitialized()
    {
        // React to changes
        WeatherGetter.WhenAnyValue(g => g.ReadCacheValue)
            .Subscribe(_ => InvokeAsync(StateHasChanged));
    }
}
```

## When to Use These Interfaces

### Use IGetterRxO/ISetterRxO/IValueRxO When:

✓ Building ViewModels for UI
✓ Need property change notifications
✓ Binding to XAML/Blazor components
✓ Using ReactiveUI's WhenAnyValue/WhenAny patterns
✓ Want reactive state tracking (IsLoading, IsSetting)

### Use Base Interfaces (IGetter/ISetter/IValue) When:

✓ Building backend services
✓ Console applications
✓ No UI involvement
✓ Want minimal dependencies
✓ Performance-critical paths (avoid ReactiveObject overhead)

## Relationship to Other Projects

```
┌─────────────────────────────────────┐
│ LionFire.Data.Async.Abstractions    │  Core async patterns
│   IGetter, ISetter, IValue          │
└────────────────┬────────────────────┘
                 │ extends
┌────────────────▼────────────────────┐
│ LionFire.Data.Async.Reactive.Abstractions │  ReactiveUI markers
│   IGetterRxO, ISetterRxO, IValueRxO      │
└────────────────┬────────────────────┘
                 │ implemented by
┌────────────────▼────────────────────┐
│ LionFire.Data.Async.Reactive        │  Concrete implementations
│   GetterRxO, ValueRxO classes       │
└────────────────┬────────────────────┘
                 │ wrapped by
┌────────────────▼────────────────────┐
│ LionFire.Data.Async.Mvvm            │  ViewModel wrappers
│   GetterVM, ValueVM                 │
└─────────────────────────────────────┘
```

## Implementation Notes

### Minimal Interface

These interfaces are intentionally minimal - they don't add methods, only ensure:
1. Derives from `ReactiveObject`
2. Implements `IReactiveObjectEx`
3. Provides property change notifications

### State Properties

Implementations should ensure these properties are reactive:
- `IsLoading` / `IsSetting` - Operation in progress
- `HasValue` - Cache validity
- `ReadCacheValue` - Cached value (raise PropertyChanged on cache update)

### Observable Patterns

All `IObservable<T>` members should be hot observables that replay state:
```csharp
// Good: Share subscription, replay current state
GetOperations = getOperationsSubject
    .Replay(1)
    .RefCount();

// Bad: Cold observable, no replay
GetOperations = getOperationsSubject;
```

## Testing

When testing with these interfaces:
```csharp
// Create test double
var mockGetter = new Mock<IGetterRxO<MyData>>();
mockGetter.SetupGet(g => g.HasValue).Returns(true);
mockGetter.SetupGet(g => g.ReadCacheValue).Returns(testData);

// Or use actual ReactiveObject implementation
var getter = new GetterRxO<MyData>(mockSource.Object);
```

## Performance Considerations

**ReactiveObject Overhead:**
- Property change notifications have small cost
- Subscription management uses memory
- Observable chains can accumulate

**When to Avoid:**
- High-frequency updates (>100Hz)
- Large collections (use DynamicData instead)
- Performance-critical paths
- Backend services without UI

**Optimization:**
```csharp
// Throttle high-frequency changes
getter.WhenAnyValue(g => g.ReadCacheValue)
    .Throttle(TimeSpan.FromMilliseconds(100))
    .Subscribe(UpdateUI);
```

## Related Projects

- **LionFire.Data.Async.Abstractions** - Base interfaces extended here
- **LionFire.Data.Async.Reactive** - Concrete implementations
- **LionFire.Data.Async.Mvvm** - ViewModel wrappers using these
- **LionFire.Reactive** - Additional reactive utilities
- **LionFire.Mvvm** - MVVM framework integration

## See Also

- ReactiveUI documentation: https://www.reactiveui.net/
- WhenAnyValue patterns
- ReactiveCommand usage
- Property change notification best practices
