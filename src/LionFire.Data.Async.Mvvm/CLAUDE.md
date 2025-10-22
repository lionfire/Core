# LionFire.Data.Async.Mvvm

## Overview

ViewModel wrappers for async data access patterns. Provides reactive ViewModels that wrap `IGetter`, `ISetter`, and `IValue` instances, adding UI-friendly features like commands, polling, loading indicators, and observable state. Built on ReactiveUI and designed for direct UI binding.

**Layer**: Toolkit (ViewModel)
**Target**: .NET 9.0
**Root Namespace**: `LionFire`

## Key Dependencies

- **ReactiveUI** - Reactive MVVM framework
- **ReactiveUI.SourceGenerators** - `[Reactive]` attribute codegen
- **MorseCode.ITask** - Covariant task interfaces
- **LionFire.Data.Async.Reactive** - Reactive implementations
- **LionFire.Mvvm** - MVVM framework

## Core Purpose

This library creates a ViewModel layer over async data operations. Instead of exposing raw `IGetter<T>` or `IValue<T>` to the UI, you use `GetterVM<T>` or `ValueVM<T>` which provide:

- ReactiveCommands for Get/Set operations
- Observable loading/saving state
- Automatic polling support
- Error handling and display
- UI-friendly property notifications

## Key Classes

### 1. GetterVM<TValue>

ViewModel wrapper for `IGetter<TValue>`:

```csharp
public partial class GetterVM<TValue>
    : ReactiveObject
    , IGetterVM<TValue>
    , IViewModel<IGetterRxO<TValue>>
{
    // Model
    public IGetter? Source { get; set; }
    public IGetterRxO<TValue>? FullFeaturedSource { get; }

    // Commands
    public ReactiveCommand<Unit, IGetResult<TValue>> GetCommand { get; }
    public ReactiveCommand<Unit, IGetResult<TValue>> GetIfNeeded { get; }

    // State
    public bool CanGet { get; }        // Command can execute
    public bool IsGetting { get; }     // Operation in progress
    public bool HasValue { get; }      // Data is cached

    // Configuration
    public TimeSpan? PollDelay { get; set; }  // Auto-refresh interval
    public bool ShowRefresh { get; set; } = true;
}
```

**Features:**
- **Upscaling**: Automatically wraps simple `IGetter` in reactive adapter
- **Commands**: `GetCommand` and `GetIfNeeded` for UI binding
- **Polling**: Optional automatic refresh via `PollDelay`
- **Observable State**: `IsGetting`, `HasValue` for UI feedback
- **Smart Binding**: Subscribes to source's observable operations

**Usage:**
```csharp
// Create VM wrapping a getter
var getter = serviceProvider.GetRequiredService<IGetter<WeatherData>>();
var vm = new GetterVM<WeatherData>(getter)
{
    PollDelay = TimeSpan.FromMinutes(5)  // Auto-refresh every 5 minutes
};

// Bind to UI
<Button Command="{Binding vm.GetCommand}">Refresh</Button>
<ProgressBar IsVisible="{Binding vm.IsGetting}" />
<TextBlock Text="{Binding vm.FullFeaturedSource.ReadCacheValue.Temperature}" />
```

### 2. IStatelessGetterVM

Minimal ViewModel interface for stateless getters:

```csharp
public interface IStatelessGetterVM
{
    ReactiveCommand<Unit, IGetResult<object>> GetCommand { get; }
    bool IsGetting { get; }
}
```

### 3. ValueVM<TValue>

ViewModel wrapper for `IValue<TValue>` (read/write):

```csharp
public partial class ValueVM<TValue>
    : GetterVM<TValue>
    , IValueVM<TValue>
{
    // Inherits Get operations from GetterVM

    // Additional Set operations
    public ReactiveCommand<TValue, ISetResult<TValue>> SetCommand { get; }
    public bool CanSet { get; }
    public bool IsSetting { get; }

    // Combined state
    public bool IsBusy => IsGetting || IsSetting;
}
```

**Features:**
- All GetterVM features
- `SetCommand` for save operations
- `IsSetting` state tracking
- `IsBusy` combined indicator

**Usage:**
```csharp
var value = serviceProvider.GetRequiredService<IValue<UserProfile>>();
var vm = new ValueVM<UserProfile>(value);

// Bind to UI
<TextBox Text="{Binding vm.FullFeaturedSource.ReadCacheValue.Name}" />
<Button Command="{Binding vm.SetCommand}"
        CommandParameter="{Binding vm.FullFeaturedSource.ReadCacheValue}">
    Save
</Button>
<ProgressBar IsVisible="{Binding vm.IsBusy}" />
```

### 4. ValueMemberVM<TValue>

ViewModel for a member property within a larger object:

```csharp
public class ValueMemberVM<TValue> : ValueVM<TValue>
{
    public string MemberName { get; set; }
    public string? DisplayName { get; set; }
    public Type MemberType { get; }

    // Member-specific metadata
}
```

**Use Case:**
Property editors, inspection panels, form builders.

### 5. Collection ViewModels

ViewModels for async collections:

#### LazilyGetsCollectionVM<TValue>

```csharp
public class LazilyGetsCollectionVM<TValue>
    : ReactiveObject
    where TValue : notnull
{
    public IObservableCache<TValue, int>? Items { get; }
    public ReactiveCommand<Unit, IGetResult<IEnumerable<TValue>>> GetCommand { get; }

    public bool IsLoading { get; }
    public bool HasLoaded { get; }
}
```

#### LazilyGetsDictionaryVM<TKey, TValue>

```csharp
public class LazilyGetsDictionaryVM<TKey, TValue>
    : ReactiveObject
    where TKey : notnull
    where TValue : notnull
{
    public IObservableCache<TValue, TKey>? Items { get; }
    // Similar to collection VM
}
```

#### LazilyGetsKeyedCollectionVM<TKey, TValue>

Specialized for keyed collections with additional operations.

### 6. Observable Cache ViewModels

ViewModels wrapping DynamicData caches:

#### AsyncObservableCacheVM<TObject, TKey>

```csharp
public abstract class AsyncObservableCacheVM<TObject, TKey>
    : ReactiveObject
    where TObject : notnull
    where TKey : notnull
{
    public IObservableCache<TObject, TKey> ObservableCache { get; }
    public ReactiveCommand<Unit, IGetResult<IEnumerable<TObject>>> RefreshCommand { get; }

    protected abstract Task<IEnumerable<TObject>> LoadItems(CancellationToken ct);
}
```

#### AsyncKeyedCollectionVM<TKey, TValue>

Specific implementation for keyed collections with item access.

#### AsyncKeyedVMCollectionVM<TKey, TModel, TViewModel>

Transforms models to ViewModels:

```csharp
public class AsyncKeyedVMCollectionVM<TKey, TModel, TViewModel>
    : AsyncObservableCacheVM<TViewModel, TKey>
    where TKey : notnull
    where TModel : notnull
    where TViewModel : notnull
{
    public AsyncKeyedVMCollectionVM(
        IObservableCache<TModel, TKey> source,
        Func<TModel, TViewModel> createViewModel)
    {
        // Transforms source models to ViewModels
        ViewModels = source
            .Connect()
            .Transform(createViewModel)
            .DisposeMany()  // Auto-dispose VMs when removed
            .AsObservableCache();
    }

    public IObservableCache<TViewModel, TKey> ViewModels { get; }
}
```

### 7. Observable Reader ViewModels

ViewModels for reactive persistence (`IObservableReader` / `IObservableWriter` from LionFire.Reactive.Persistence):

#### ObservableReaderVM<TKey, TValue, TValueVM>

Collection-level ViewModel for `IObservableReader<TKey, TValue>`:

```csharp
public partial class ObservableReaderVM<TKey, TValue, TValueVM> : ReactiveObject
    where TKey : notnull
    where TValue : notnull
{
    public IObservableReader<TKey, TValue> Data { get; }

    public bool AutoLoadAll { get; set; }  // Auto-subscribe to all keys
}
```

**Features:**
- Wraps `IObservableReader<TKey, TValue>` from LionFire.Reactive.Persistence
- `AutoLoadAll` triggers `Data.ListenAllKeys()` when enabled
- Base class for collection-level reactive persistence access

#### ObservableReaderItemVM<TKey, TValue, TValueVM>

Item-level ViewModel for accessing individual items:

```csharp
public partial class ObservableReaderItemVM<TKey, TValue, TValueVM> : ReactiveObject
    where TKey : notnull
    where TValue : notnull
{
    public IObservableReader<TKey, TValue> Data { get; }

    public TKey? Id { get; set; }          // Current item key
    public TValue? Value { get; }          // Current item value
    public bool IsLoading { get; }         // Loading state
}
```

**Features:**
- Automatically subscribes to item observable when `Id` changes
- Reactive value updates via `Data.GetValueObservableIfExists()`
- Loading state tracking
- Auto-cleanup of subscriptions on Id change

#### ObservableReaderWriterVM<TKey, TValue, TValueVM>

Collection-level read/write ViewModel:

```csharp
public partial class ObservableReaderWriterVM<TKey, TValue, TValueVM>
    : ObservableReaderVM<TKey, TValue, TValueVM>
    where TKey : notnull
    where TValue : notnull
{
    public IObservableWriter<TKey, TValue> Writer { get; }
}
```

Extends `ObservableReaderVM` with write capabilities.

#### ObservableReaderWriterItemVM<TKey, TValue, TValueVM>

Item-level read/write ViewModel:

```csharp
public partial class ObservableReaderWriterItemVM<TKey, TValue, TValueVM>
    : ObservableReaderItemVM<TKey, TValue, TValueVM>
    where TKey : notnull
    where TValue : notnull
{
    public IObservableWriter<TKey, TValue> Writer { get; }

    public ValueTask Write();  // Write current value
}
```

**Features:**
- All read features from `ObservableReaderItemVM`
- `Write()` method persists current `Value` to storage
- Suitable for master-detail scenarios with inline editing

**Usage Example:**
```csharp
// Collection-level VM
var reader = serviceProvider.GetRequiredService<IObservableReader<string, Config>>();
var collectionVM = new ObservableReaderVM<string, Config, ConfigVM>(reader)
{
    AutoLoadAll = true  // Auto-load all config files
};

// Item-level VM for editing
var writer = serviceProvider.GetRequiredService<IObservableWriter<string, Config>>();
var itemVM = new ObservableReaderWriterItemVM<string, Config, ConfigVM>(reader, writer);

// Bind to specific item
itemVM.Id = "app-config.json";
// Value automatically loads and updates reactively

// Edit and save
itemVM.Value.Setting = "new value";
await itemVM.Write();  // Persists to storage
```

### 8. Observable Data ViewModel

#### ObservableDataVM<TKey, TValue, TMetadata>

Advanced ViewModel for `IObservableReader<TKey, TValue, TMetadata>`:

```csharp
public class ObservableDataVM<TKey, TValue, TMetadata>
    : ReactiveObject
    where TKey : notnull
    where TValue : notnull
    where TMetadata : notnull
{
    public IObservableCache<TMetadata, TKey> Keys { get; }
    public IObservableCache<Optional<TValue>, TKey> Values { get; }

    public ReactiveCommand ListenAllKeysCommand { get; }
    public ReactiveCommand ListenAllValuesCommand { get; }
}
```

Wraps reactive persistence layer (see LionFire.Reactive).

## Configuration

### VMOptions

```csharp
public class VMOptions
{
    public bool AutoGet { get; set; }           // Auto-load on creation
    public bool AutoGetIfNeeded { get; set; }   // Auto-load if not cached
    public TimeSpan? PollInterval { get; set; } // Auto-refresh interval
}
```

## Directory Structure

```
Data/Async/
  Collections/
    Collection/
      IGetsCollectionVM.cs
      IGetsKeyedCollectionVM.cs
      LazilyGetsCollectionVM.cs

    Dictionary/
      LazilyGetsDictionaryVM.cs

    DisplayNameUtilsX.cs

    DynamicData_/
      ObservableDataVM.cs

    KeyedCollection/
      AsyncKeyedCollectionVM.cs
      AsyncKeyedVMCollectionVM.cs
      AsyncKeyedXCollectionVMBase.cs
      LazilyGetsKeyedCollectionVM.cs

    ObservableCache/
      AsyncObservableCacheVM.cs
      AsyncObservableXCacheVMBase.cs

    ObservableReader/
      ObservableReaderVM.cs           - Collection & item-level read VMs
      ObservableReaderWriterVM.cs     - Collection & item-level read/write VMs

  Values/
    Read/
      GetterVM.cs
      IGetterVM.cs
      IObservableGetterVM.cs
      IStatelessGetterVM.cs
      VMOptions.cs

    ReadWrite/
      IValueVM.cs
      ValueMemberVM.cs
      ValueVM.cs
```

## Usage Patterns

### Pattern 1: Simple Data Display

```csharp
public class WeatherViewModel : ReactiveObject
{
    public WeatherViewModel(IGetter<WeatherData> weatherGetter)
    {
        WeatherVM = new GetterVM<WeatherData>(weatherGetter);

        // Auto-load on creation
        WeatherVM.GetIfNeeded.Execute().Subscribe();
    }

    public GetterVM<WeatherData> WeatherVM { get; }
}

// View (Avalonia/WPF)
<StackPanel>
    <Button Command="{Binding WeatherVM.GetCommand}">
        Refresh Weather
    </Button>
    <ProgressBar IsVisible="{Binding WeatherVM.IsGetting}"
                 IsIndeterminate="True" />
    <TextBlock Text="{Binding WeatherVM.FullFeaturedSource.ReadCacheValue.Temperature}"
               IsVisible="{Binding WeatherVM.HasValue}" />
</StackPanel>
```

### Pattern 2: Auto-Polling Data

```csharp
public class StockTickerViewModel : ReactiveObject
{
    public StockTickerViewModel(IGetter<StockPrice> priceGetter)
    {
        PriceVM = new GetterVM<StockPrice>(priceGetter)
        {
            PollDelay = TimeSpan.FromSeconds(10)  // Refresh every 10 seconds
        };
    }

    public GetterVM<StockPrice> PriceVM { get; }
}
```

### Pattern 3: Editable Form

```csharp
public class UserProfileViewModel : ReactiveObject
{
    public UserProfileViewModel(IValue<UserProfile> profileValue)
    {
        ProfileVM = new ValueVM<UserProfile>(profileValue);

        // Load profile
        ProfileVM.GetIfNeeded.Execute().Subscribe();

        // Save on command
        SaveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await ProfileVM.SetCommand.Execute(
                ProfileVM.FullFeaturedSource.ReadCacheValue);
        });
    }

    public ValueVM<UserProfile> ProfileVM { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
}

// View
<StackPanel>
    <TextBox Text="{Binding ProfileVM.FullFeaturedSource.ReadCacheValue.Name}" />
    <TextBox Text="{Binding ProfileVM.FullFeaturedSource.ReadCacheValue.Email}" />
    <Button Command="{Binding SaveCommand}"
            IsEnabled="{Binding !ProfileVM.IsBusy}">
        Save
    </Button>
</StackPanel>
```

### Pattern 4: Master-Detail with Collection

```csharp
public class DocumentBrowserViewModel : ReactiveObject
{
    public DocumentBrowserViewModel(
        IGetter<IEnumerable<DocumentSummary>> summariesGetter,
        IViewModelProvider vmProvider)
    {
        DocumentsVM = new LazilyGetsCollectionVM<DocumentSummary>(summariesGetter);

        // Transform summaries to ViewModels
        DocumentViewModels = new AsyncKeyedVMCollectionVM<int, DocumentSummary, DocumentSummaryVM>(
            source: DocumentsVM.Items,
            createViewModel: summary => new DocumentSummaryVM(summary)
        );

        // Load documents
        DocumentsVM.GetCommand.Execute().Subscribe();

        // Selection
        this.WhenAnyValue(vm => vm.SelectedDocument)
            .WhereNotNull()
            .Subscribe(doc => LoadDetail(doc));
    }

    public LazilyGetsCollectionVM<DocumentSummary> DocumentsVM { get; }
    public AsyncKeyedVMCollectionVM<int, DocumentSummary, DocumentSummaryVM> DocumentViewModels { get; }

    [Reactive] private DocumentSummaryVM? selectedDocument;
}
```

### Pattern 5: Reactive Persistence with ObservableReader

Master-detail with file-based reactive persistence:

```csharp
public class ConfigEditorViewModel : ReactiveObject
{
    public ConfigEditorViewModel(
        IObservableReader<string, AppConfig> reader,
        IObservableWriter<string, AppConfig> writer)
    {
        // Collection-level: Show all config files
        CollectionVM = new ObservableReaderVM<string, AppConfig, AppConfigVM>(reader)
        {
            AutoLoadAll = true  // Auto-subscribe to all keys (file changes)
        };

        // Item-level: Edit selected config
        ItemVM = new ObservableReaderWriterItemVM<string, AppConfig, AppConfigVM>(reader, writer);

        // Wire up selection
        this.WhenAnyValue(vm => vm.SelectedConfigKey)
            .Subscribe(key =>
            {
                ItemVM.Id = key;  // Automatically loads selected item
            });

        // Save command
        SaveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await ItemVM.Write();  // Persists changes
        });
    }

    public ObservableReaderVM<string, AppConfig, AppConfigVM> CollectionVM { get; }
    public ObservableReaderWriterItemVM<string, AppConfig, AppConfigVM> ItemVM { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    [Reactive] private string? selectedConfigKey;
}

// View (Avalonia/WPF)
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="200" />
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>

    <!-- Master: Config list -->
    <ListBox Grid.Column="0"
             ItemsSource="{Binding CollectionVM.Data.Keys}"
             SelectedItem="{Binding SelectedConfigKey}" />

    <!-- Detail: Config editor -->
    <StackPanel Grid.Column="1" IsVisible="{Binding ItemVM.Id, Converter={x:Static ObjectConverters.IsNotNull}}">
        <ProgressBar IsVisible="{Binding ItemVM.IsLoading}" />
        <TextBox Text="{Binding ItemVM.Value.Setting1}" />
        <TextBox Text="{Binding ItemVM.Value.Setting2}" />
        <Button Command="{Binding SaveCommand}">Save</Button>
    </StackPanel>
</Grid>
```

**Key Benefits:**
- Automatic reactive updates when files change on disk
- Lazy loading - only deserializes when `Id` is set
- Clean master-detail pattern
- Auto-cleanup of subscriptions

### Pattern 6: Upscaling Non-Reactive Sources

When your source `IGetter` doesn't implement `IGetterRxO`:

```csharp
var simpleGetter = new MySimpleGetter(); // Just implements IStatelessGetter<T>
var vm = new GetterVM<MyData>(simpleGetter);

// GetterVM automatically upscales to IGetterRxO<T>
// Now you have reactive properties even though source isn't reactive
vm.WhenAnyValue(g => g.HasValue)
    .Subscribe(hasValue => Console.WriteLine($"Has value: {hasValue}"));
```

## Design Philosophy

**Separation of Concerns:**
- Data access logic in `IGetter`/`IValue` implementations
- UI concerns (commands, state, polling) in ViewModels
- ViewModels never implement business logic

**Command Pattern:**
- All operations exposed as `ReactiveCommand`
- UI binds to commands, not methods
- Automatic CanExecute management

**Observable State:**
- All ViewModel state is reactive
- UI automatically updates on changes
- No manual `INotifyPropertyChanged` raising needed

**Wrapping, Not Replacing:**
- ViewModels wrap existing data access objects
- Original interfaces still accessible
- Can swap implementations without changing VM code

## Advanced Features

### GetterRxOUpscaler

Automatically converts non-reactive getters to reactive:

```csharp
public static class GetterRxOUpscaler
{
    public static IGetterRxO<T> Upscale<T>(IGetter getter)
    {
        if (getter is IGetterRxO<T> rxo) return rxo;

        // Wrap in reactive adapter
        return new GetterRxOAdapter<T>(getter);
    }
}
```

Used internally by `GetterVM` when `Source` is set.

### Polling Support

```csharp
// GetterVM internally creates AsyncPoller if PollDelay is set
private void TryStartPolling()
{
    if (!PollDelay.HasValue) return;
    if (FullFeaturedSource?.FiresChangeEvents == true) return; // Don't poll if already reactive

    poller = new AsyncPoller<TValue>(
        () => GetCommand.Execute().ToTask(),
        PollDelay.Value
    );
}
```

Polling only starts if:
- `PollDelay` is set
- Source doesn't already fire change events

### Display Name Utilities

```csharp
public static class DisplayNameUtilsX
{
    public static string GetDisplayName(MemberInfo member)
    {
        // Check for [Display] attribute
        // Check for [DisplayName] attribute
        // Fallback to member name with spacing
    }
}
```

Used by member ViewModels for UI labels.

## Testing

Testing ViewModels:

```csharp
[Fact]
public async Task GetterVM_Executes_GetCommand()
{
    // Arrange
    var mockGetter = new Mock<IGetter<string>>();
    mockGetter.Setup(g => g.Get(default))
        .Returns(Task.FromResult(GetResult<string>.Success("test")));

    var vm = new GetterVM<string>(mockGetter.Object);

    // Act
    await vm.GetCommand.Execute();

    // Assert
    Assert.True(vm.HasValue);
    Assert.Equal("test", vm.FullFeaturedSource.ReadCacheValue);
}

[Fact]
public void GetterVM_IsGetting_Updates_During_Operation()
{
    // Arrange
    var tcs = new TaskCompletionSource<IGetResult<string>>();
    var mockGetter = new Mock<IGetter<string>>();
    mockGetter.Setup(g => g.Get(default))
        .Returns(tcs.Task.AsITask());

    var vm = new GetterVM<string>(mockGetter.Object);

    var isGettingValues = new List<bool>();
    vm.WhenAnyValue(g => g.IsGetting).Subscribe(isGettingValues.Add);

    // Act
    var task = vm.GetCommand.Execute();

    // Assert - while executing
    Assert.True(vm.IsGetting);
    Assert.Contains(true, isGettingValues);

    // Complete
    tcs.SetResult(GetResult<string>.Success("done"));
    task.Wait();

    // Assert - after complete
    Assert.False(vm.IsGetting);
}
```

## Performance Considerations

**ViewModel Overhead:**
- Each VM adds ReactiveCommand overhead
- Polling adds background task
- Observable subscriptions use memory

**When to Use:**
✓ UI-bound data
✓ User-initiated operations
✓ Infrequent updates

**When to Avoid:**
✗ High-frequency data streams (>10Hz)
✗ Backend services
✗ Performance-critical paths

**Optimization:**
```csharp
// Good: Single VM, multiple UI bindings
var vm = new GetterVM<Data>(getter);
view1.DataContext = vm;
view2.DataContext = vm;

// Bad: Multiple VMs for same data
var vm1 = new GetterVM<Data>(getter);  // Creates polling
var vm2 = new GetterVM<Data>(getter);  // Creates duplicate polling!
```

## Common Pitfalls

**1. Not Disposing Polling:**
```csharp
// VM with PollDelay should be disposed
var vm = new GetterVM<Data>(getter) { PollDelay = TimeSpan.FromSeconds(5) };

// Later:
vm.Dispose(); // Stops polling
```

**2. Accessing ReadCacheValue Before Loading:**
```csharp
// Bad: ReadCacheValue may be null
var value = vm.FullFeaturedSource.ReadCacheValue.Property; // NullReferenceException!

// Good: Check HasValue first
if (vm.HasValue)
{
    var value = vm.FullFeaturedSource.ReadCacheValue.Property;
}
```

**3. Forgetting to Execute Initial Load:**
```csharp
// Bad: Data never loads
var vm = new GetterVM<Data>(getter);
// HasValue stays false

// Good: Execute initial load
var vm = new GetterVM<Data>(getter);
vm.GetIfNeeded.Execute().Subscribe();
```

## Related Projects

- **LionFire.Data.Async.Abstractions** - Interfaces being wrapped
- **LionFire.Data.Async.Reactive** - Reactive implementations
- **LionFire.Data.Async.Reactive.Abstractions** - RxO interfaces
- **LionFire.Mvvm** - MVVM framework base
- **LionFire.Reactive** - Additional reactive utilities

## See Also

- ReactiveCommand documentation
- ReactiveUI ViewModel patterns
- MVVM best practices
- WhenAnyValue usage
- Binding patterns for XAML/Avalonia/Blazor
