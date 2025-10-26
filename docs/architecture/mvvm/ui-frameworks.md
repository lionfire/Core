# UI Framework Support

**Overview**: LionFire's MVVM architecture is designed to work with multiple UI frameworks. While the core MVVM patterns are framework-agnostic, specific libraries provide integration for Blazor and WPF/Avalonia.

---

## Table of Contents

1. [Framework-Agnostic Core](#framework-agnostic-core)
2. [Blazor Integration](#blazor-integration)
3. [WPF/Avalonia Integration](#wpfavalonia-integration)
4. [Cross-Platform Considerations](#cross-platform-considerations)

---

## Framework-Agnostic Core

### Core MVVM is UI-Independent

The core MVVM stack works with **any UI framework**:

```
LionFire.Mvvm.Abstractions     ← No UI dependencies
    ↓
LionFire.Mvvm                  ← No UI dependencies
    ↓
LionFire.Data.Async.Mvvm       ← No UI dependencies
```

**ViewModels are pure C#**:
```csharp
// This ViewModel works in Blazor, WPF, Avalonia, Console, Tests, etc.
public partial class BotVM : ReactiveObject, IViewModel<BotEntity>
{
    public BotVM(string key, BotEntity value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; }
    public BotEntity Value { get; }

    [Reactive] private bool _isRunning;

    public ReactiveCommand<Unit, Unit> StartCommand { get; }
}
```

**UI frameworks bind to ViewModels** using their native binding systems.

---

## Blazor Integration

### Library: LionFire.Blazor.Components.MudBlazor

**Purpose**: Blazor-specific components that integrate with LionFire MVVM.

**Key Components**:
- `ObservableDataView<TKey, TValue, TValueVM>` - Reactive MudDataGrid
- `AsyncVMSourceCacheView` - DynamicData cache binding
- `KeyedCollectionView` - Simple keyed collections

### Component-Based Pattern

**Blazor uses components**, not data binding:

```razor
<!-- WPF/Avalonia: Data binding -->
<ListBox ItemsSource="{Binding Items}" />

<!-- Blazor: Component parameters and @foreach -->
<MudDataGrid Items="@ViewModel.Items" />
@foreach (var item in ViewModel.Items)
{
    <MudListItem>@item.Name</MudListItem>
}
```

### ReactiveUI Integration in Blazor

**ReactiveUI.Blazor** provides base component:

```csharp
using ReactiveUI.Blazor;

public partial class MyComponent : ReactiveComponentBase<MyViewModel>
{
    // ViewModel property automatically injected
    // Automatic activation/deactivation
}
```

**However**: LionFire patterns typically **don't use** `ReactiveComponentBase` for workspace documents. Instead:

```razor
@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    // Create VM manually with workspace services
    private MyVM? VM { get; set; }
}
```

**Reason**: Workspace services are scoped, not injectable from root container.

### ObservableDataView Pattern

**Recommended** for lists in Blazor:

```razor
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices">
    <Columns>
        <PropertyColumn Property="x => x.Value.Name" />
    </Columns>
</ObservableDataView>
```

**Features**:
- Automatic reactive updates
- Built-in CRUD toolbar
- MudBlazor styling
- DynamicData integration
- Workspace service resolution

**See**: [Blazor MVVM Patterns](../../ui/blazor-mvvm-patterns.md) for complete guide.

### Manual VM Pattern

**For detail views**:

```razor
@page "/bots/{BotId}"

@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    private ObservableReaderWriterItemVM<string, BotEntity, BotVM>? VM { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
        var writer = WorkspaceServices.GetService<IObservableWriter<string, BotEntity>>();

        VM = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);
        VM.Id = BotId;
    }
}
```

### Reactive Updates in Blazor

**Challenge**: Blazor needs explicit `StateHasChanged()` calls.

**Solution**: Components subscribe to VM changes:

```razor
@implements IDisposable

@code {
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        // Subscribe to VM property changes
        subscription = ViewModel.WhenAnyValue(x => x.SomeProperty)
            .Subscribe(_ => InvokeAsync(StateHasChanged));
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

**ObservableDataView handles this automatically**:
```csharp
// Internally subscribes to ViewModel.ItemsChanged
ViewModel.ItemsChanged.Subscribe(_ => InvokeAsync(StateHasChanged));
```

---

## WPF/Avalonia Integration

### Library: LionFire.Avalon

**Purpose**: WPF/Avalonia-specific MVVM support (legacy).

**Status**: ⚠️ Legacy support - less actively maintained than Blazor.

### XAML Data Binding

**WPF/Avalonia use declarative data binding**:

```xml
<Window xmlns:vm="clr-namespace:MyApp.ViewModels">
    <Window.DataContext>
        <vm:BotViewModel />
    </Window.DataContext>

    <!-- Bind to ViewModel properties -->
    <TextBox Text="{Binding Value.Name}" />
    <CheckBox IsChecked="{Binding Value.Enabled}" />

    <!-- Bind to commands -->
    <Button Command="{Binding StartCommand}">Start</Button>

    <!-- Bind to state -->
    <ProgressBar IsIndeterminate="True"
                 Visibility="{Binding IsRunning, Converter={StaticResource BoolToVisibility}}" />
</Window>
```

### ReactiveUI with WPF

**ReactiveUI provides WPF integration**:

```csharp
using ReactiveUI;

public partial class BotView : ReactiveUserControl<BotViewModel>
{
    public BotView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            // Bind ViewModel to View
            this.OneWayBind(ViewModel, vm => vm.Value.Name, v => v.NameTextBox.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.StartCommand, v => v.StartButton)
                .DisposeWith(disposables);
        });
    }
}
```

### Observable Collections in WPF

**DynamicData to ObservableCollection**:

```csharp
using System.Collections.ObjectModel;
using DynamicData.Binding;

public class BotsVM : ReactiveObject
{
    private SourceCache<BotEntity, string> _bots = new(b => b.Id);

    private ReadOnlyObservableCollection<BotVM> _botList;
    public ReadOnlyObservableCollection<BotVM> BotList => _botList;

    public BotsVM()
    {
        _bots.Connect()
             .Transform(entity => new BotVM(entity))
             .Bind(out _botList)  // Bind to ObservableCollection
             .Subscribe();
    }
}
```

**XAML Binding**:
```xml
<ListBox ItemsSource="{Binding BotList}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Value.Name}" />
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

---

## Cross-Platform Considerations

### Shared ViewModels

**ViewModels work across all platforms**:

```csharp
// Single ViewModel
public class WeatherViewModel : ReactiveObject
{
    [Reactive] private WeatherData? _currentWeather;
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
}

// Used in Blazor
@inject WeatherViewModel ViewModel

// Used in WPF
<Window xmlns:vm="..." DataContext="{StaticResource WeatherVM}">

// Used in Console app
var vm = serviceProvider.GetRequiredService<WeatherViewModel>();
await vm.RefreshCommand.Execute();
```

### Platform-Specific Patterns

**Blazor**:
- Component parameters (no data binding)
- `@bind-Value` for two-way binding
- `InvokeAsync(StateHasChanged)` for updates
- `CascadingParameter` for workspace services

**WPF/Avalonia**:
- `DataContext` and `{Binding}`
- Automatic change notifications via `INotifyPropertyChanged`
- `DependencyProperty` for custom controls
- `ICommand` for buttons

### Dependency Injection Differences

**Blazor**:
```csharp
// Services injected via @inject or [Inject]
@inject IViewModelProvider VMProvider
@inject IObservableReader<string, BotEntity> Reader  // If in root scope

[CascadingParameter(Name = "WorkspaceServices")]  // Workspace services
public IServiceProvider? WorkspaceServices { get; set; }
```

**WPF**:
```csharp
// ViewModels created via ViewLocator or manual
public MainWindow()
{
    InitializeComponent();
    DataContext = App.ServiceProvider.GetRequiredService<MainWindowVM>();
}
```

### Reactive Binding Differences

**Blazor**:
```razor
<!-- Manual subscription to trigger StateHasChanged -->
@code {
    protected override void OnInitialized()
    {
        ViewModel.WhenAnyValue(x => x.Items)
            .Subscribe(_ => InvokeAsync(StateHasChanged));
    }
}
```

**WPF/Avalonia**:
```csharp
// Automatic via INotifyPropertyChanged
// No manual StateHasChanged needed
```

---

## Summary

### Framework Support Matrix

| Framework | Library | Status | Recommended For |
|-----------|---------|--------|-----------------|
| **Blazor** | LionFire.Blazor.Components.MudBlazor | ✅ Active | New projects |
| **WPF** | LionFire.Avalon | ⚠️ Legacy | Existing WPF apps |
| **Avalonia** | LionFire.Avalon (partial) | ⚠️ Limited | Cross-platform desktop |

### Key Differences

**Blazor**:
- Component-based (parameters, not data binding)
- Manual `StateHasChanged` calls
- `ObservableDataView` component for automatic reactive grids
- Workspace service via `CascadingParameter`

**WPF/Avalonia**:
- Declarative data binding via XAML
- Automatic change propagation via `INotifyPropertyChanged`
- `ReactiveUserControl<TVM>` for ReactiveUI integration
- Traditional DI patterns

### Core Principle

**ViewModels are portable** - Write once, use in any UI framework. Only the View layer changes.

### Related Documentation

- **[Blazor MVVM Patterns](../../ui/blazor-mvvm-patterns.md)** - Blazor-specific patterns
- **[MVVM Architecture Overview](README.md)** - Framework-agnostic architecture
- **[Reactive MVVM](reactive-mvvm.md)** - ReactiveUI patterns (all frameworks)
- **Library References**:
  - [LionFire.Blazor.Components.MudBlazor](../../src/LionFire.Blazor.Components.MudBlazor/CLAUDE.md)
  - [LionFire.Mvvm](../../src/LionFire.Mvvm/CLAUDE.md)
