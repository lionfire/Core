# MVVM in LionFire

## Overview

Model-View-ViewModel (MVVM) is a core architectural pattern in LionFire, providing clean separation between UI and business logic. LionFire's MVVM implementation integrates **ReactiveUI**, **CommunityToolkit.Mvvm**, and custom patterns optimized for reactive data persistence and workspace-scoped services.

**Key Philosophy**: ViewModels are thin wrappers around data access patterns, adding UI-specific concerns without duplicating business logic.

---

## Quick Start

### 1. Define Your Entity (Model)

```csharp
using ReactiveUI;
using ReactiveUI.SourceGenerators;

[Alias("Bot")]
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;
    [Reactive] private string? _description;
    [Reactive] private bool _enabled;

    // Business logic methods
    public void Start() { /* ... */ }
    public void Stop() { /* ... */ }
}
```

**Key Points**:
- Inherit from `ReactiveObject` (ReactiveUI)
- Use `[Reactive]` attribute for properties
- Implements `INotifyPropertyChanged` automatically

---

### 2. Create a ViewModel

```csharp
using LionFire.Mvvm;

public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value)
    {
        // Initialize commands
        ToggleCommand = ReactiveCommand.Create(Toggle);
    }

    // UI-specific computed properties
    public string DisplayName => $"{Value.Name} ({Key})";

    public string StatusText => Value.Enabled ? "Running" : "Stopped";

    public Color StatusColor => Value.Enabled ? Color.Success : Color.Default;

    // UI commands
    public ReactiveCommand<Unit, Unit> ToggleCommand { get; }

    private void Toggle()
    {
        Value.Enabled = !Value.Enabled;
        if (Value.Enabled)
            Value.Start();
        else
            Value.Stop();
    }
}
```

**Key Points**:
- Inherit from `KeyValueVM<TKey, TValue>` for keyed entities
- Add UI-specific computed properties
- Add commands for user actions
- Don't duplicate business logic from entity

---

### 3. Use in Blazor UI

**Pattern A: List View (Automatic)**

```razor
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices">
    <Columns>
        <PropertyColumn Property="x => x.DisplayName" />
        <PropertyColumn Property="x => x.StatusText" />
        <TemplateColumn>
            <CellTemplate>
                <MudButton Command="@context.Item.ToggleCommand">
                    Toggle
                </MudButton>
            </CellTemplate>
        </TemplateColumn>
    </Columns>
</ObservableDataView>
```

**Pattern B: Detail View (Manual)**

```razor
<MudCard>
    <MudCardContent>
        <MudTextField Label="Name" @bind-Value="vm.Value.Name" />
        <MudSwitch Label="Enabled" @bind-Checked="vm.Value.Enabled" />
        <MudText Color="@vm.StatusColor">@vm.StatusText</MudText>
    </MudCardContent>
    <MudCardActions>
        <MudButton Command="@vm.ToggleCommand">Toggle</MudButton>
        <MudButton OnClick="Save">Save</MudButton>
    </MudCardActions>
</MudCard>

@code {
    [Parameter] public string? BotId { get; set; }
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    ObservableReaderWriterItemVM<string, BotEntity, BotVM>? vm;

    protected override void OnInitialized()
    {
        var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
        var writer = WorkspaceServices.GetService<IObservableWriter<string, BotEntity>>();
        vm = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);
        vm.Id = BotId;
    }

    private async Task Save() => await vm.Write();
}
```

---

## Core Concepts

### 1. The MVVM Layers

```
┌────────────────────────────────────────────────────────────┐
│                        View Layer                           │
│  Blazor Components (.razor files)                          │
│  - MudDataGrid, MudTextField, etc.                         │
│  - Binds to ViewModels                                     │
└─────────────────────┬──────────────────────────────────────┘
                      │ Data Binding
                      ↓
┌────────────────────────────────────────────────────────────┐
│                    ViewModel Layer                          │
│  ViewModels (BotVM, PortfolioVM, etc.)                    │
│  - UI-specific properties (DisplayName, StatusColor)       │
│  - Commands (ToggleCommand, SaveCommand)                   │
│  - Wraps Entity                                            │
└─────────────────────┬──────────────────────────────────────┘
                      │ Wraps
                      ↓
┌────────────────────────────────────────────────────────────┐
│                      Model Layer                            │
│  Entities (BotEntity, Portfolio, etc.)                     │
│  - Business logic (Start(), Stop())                        │
│  - Domain properties (Name, Description)                   │
│  - No UI concerns                                          │
└─────────────────────┬──────────────────────────────────────┘
                      │ Persisted by
                      ↓
┌────────────────────────────────────────────────────────────┐
│                  Data Access Layer                          │
│  IObservableReader/Writer                                  │
│  - File-based persistence (HJSON)                          │
│  - Observable change notifications                         │
└────────────────────────────────────────────────────────────┘
```

---

### 2. Reactive Properties

**ReactiveUI's [Reactive] Attribute**:

```csharp
public partial class BotEntity : ReactiveObject
{
    // Source generator creates:
    // - Public property: public string? Name { get; set; }
    // - Property change notifications
    // - Observable for WhenAnyValue(x => x.Name)

    [Reactive] private string? _name;
}
```

**Benefits**:
- Automatic `INotifyPropertyChanged` implementation
- Zero boilerplate
- Type-safe reactive subscriptions
- Blazor automatically detects changes

---

### 3. ViewModel Hierarchy

```
IViewModel (marker interface)
    ↓
IViewModel<TModel>
    ↓ implements
KeyVM<TKey>                     ← Has Key property
    ↓ extends
KeyValueVM<TKey, TValue>        ← Has Key + Value (entity)
    ↓ used in
ObservableReaderItemVM          ← Reads from IObservableReader
    ↓ extends
ObservableReaderWriterItemVM    ← Reads + Writes
```

**When to Use Each**:
- `KeyValueVM` - Simple key/value pair with UI logic
- `ObservableReaderItemVM` - Read entity from workspace (read-only)
- `ObservableReaderWriterItemVM` - Read/write entity (full CRUD)

---

### 4. Commands

**ReactiveCommand** provides command pattern with reactive extensions:

```csharp
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value)
    {
        // Command with enable/disable logic
        StartCommand = ReactiveCommand.CreateFromTask(
            StartAsync,
            this.WhenAnyValue(x => x.Value.Enabled, enabled => !enabled)
        );

        // Disabled when bot is not enabled
        StopCommand = ReactiveCommand.CreateFromTask(
            StopAsync,
            this.WhenAnyValue(x => x.Value.Enabled)
        );
    }

    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; }

    private async Task StartAsync()
    {
        Value.Enabled = true;
        Value.Start();
        await SaveChanges();
    }

    private async Task StopAsync()
    {
        Value.Enabled = false;
        Value.Stop();
        await SaveChanges();
    }
}
```

**Binding to UI**:
```razor
<MudButton Command="@vm.StartCommand" Disabled="@(!vm.StartCommand.CanExecute.Value)">
    Start
</MudButton>
```

---

## Documentation Structure

### **[ViewModels Guide](viewmodels-guide.md)**
Comprehensive guide to creating and using ViewModels in LionFire.

**Topics**:
- ViewModel base classes
- Creating custom ViewModels
- ViewModel lifecycle
- Commands and validation

---

### **[Reactive Patterns](reactive-patterns.md)**
Deep dive into reactive programming patterns used in MVVM.

**Topics**:
- WhenAnyValue and observables
- Reactive subscriptions
- Throttling and debouncing
- Combining observables

---

### **[Data Binding](data-binding.md)**
How ViewModels bind to Blazor UI components.

**Topics**:
- One-way vs. two-way binding
- Binding to collections
- Custom binding patterns
- Performance optimization

---

## MVVM Layers in LionFire

### Abstractions Layer

**Library**: `LionFire.Mvvm.Abstractions`

**Purpose**: Core interfaces and contracts.

**Key Types**:
- `IViewModel`, `IViewModel<TModel>`
- `IViewModelProvider`
- `IItemPageVM`

---

### Core MVVM Layer

**Library**: `LionFire.Mvvm`

**Purpose**: Base ViewModel implementations.

**Key Types**:
- `KeyVM<TKey>`
- `KeyValueVM<TKey, TValue>`
- `ViewModel<TModel>`

---

### Async Data MVVM Layer

**Library**: `LionFire.Data.Async.Mvvm`

**Purpose**: ViewModels wrapping async data access patterns.

**Key Types**:
- `GetterVM<TValue>` - Wraps `IGetter`
- `ValueVM<TValue>` - Wraps `IValue` (read/write)
- `ObservableReaderItemVM<TKey, TValue, TValueVM>` - Wraps `IObservableReader`
- `ObservableReaderWriterItemVM<TKey, TValue, TValueVM>` - Wraps `IObservableReader/Writer`
- `ObservableDataVM<TKey, TValue, TValueVM>` - Collection VM

**See**: [LionFire.Data.Async.Mvvm/CLAUDE.md](../../src/LionFire.Data.Async.Mvvm/CLAUDE.md)

---

## Common Patterns

### Pattern 1: Simple Entity ViewModel

**Use Case**: Display/edit simple entity with UI-specific formatting.

```csharp
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value) { }

    // Computed properties
    public string DisplayName => $"{Value.Name} ({Key})";
    public string StatusText => Value.Enabled ? "Active" : "Inactive";
}
```

---

### Pattern 2: ViewModel with Commands

**Use Case**: Add user actions to entity.

```csharp
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value)
    {
        DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync);
        CloneCommand = ReactiveCommand.CreateFromTask(CloneAsync);
    }

    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    public ReactiveCommand<Unit, Unit> CloneCommand { get; }

    private async Task DeleteAsync()
    {
        // Delete logic
    }

    private async Task CloneAsync()
    {
        // Clone logic
    }
}
```

---

### Pattern 3: ViewModel with Validation

**Use Case**: Add validation rules for UI.

```csharp
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value)
    {
        // Validate name
        this.ValidationRule(
            vm => vm.Value.Name,
            name => !string.IsNullOrWhiteSpace(name),
            "Name is required"
        );

        // Validate symbol
        this.ValidationRule(
            vm => vm.Value.Symbol,
            symbol => IsValidSymbol(symbol),
            "Invalid trading symbol"
        );
    }

    private bool IsValidSymbol(string? symbol)
    {
        return !string.IsNullOrEmpty(symbol) && symbol.Length >= 3;
    }
}
```

---

### Pattern 4: Aggregating ViewModels

**Use Case**: Dashboard combining multiple data sources.

```csharp
public class DashboardVM : ReactiveObject
{
    public DashboardVM(
        IObservableReader<string, BotEntity> botReader,
        IObservableReader<string, Portfolio> portfolioReader)
    {
        BotsVM = new ObservableDataVM<string, BotEntity, BotVM>(botReader);
        PortfolioVM = new ObservableReaderItemVM<string, Portfolio, PortfolioVM>(portfolioReader);
        PortfolioVM.Id = "main";

        // Computed properties combining data
        this.WhenAnyValue(
                x => x.BotsVM.Items.Count,
                x => x.PortfolioVM.Value.TotalValue,
                (botCount, totalValue) => new { botCount, totalValue })
            .Subscribe(x => UpdateSummary(x.botCount, x.totalValue));
    }

    public ObservableDataVM<string, BotEntity, BotVM> BotsVM { get; }
    public ObservableReaderItemVM<string, Portfolio, PortfolioVM> PortfolioVM { get; }

    [Reactive] private string? _summaryText;
}
```

---

## Best Practices

### 1. Keep ViewModels Thin

```csharp
// ✅ Good - VM adds UI concerns only
public class BotVM : KeyValueVM<string, BotEntity>
{
    public string DisplayName => $"{Value.Name} ({Key})";
    public ReactiveCommand<Unit, Unit> ToggleCommand { get; }
}

// ❌ Avoid - Business logic belongs in entity
public class BotVM : KeyValueVM<string, BotEntity>
{
    public void ExecuteTrade() { /* complex trading logic */ }
}
```

---

### 2. Use Reactive Properties

```csharp
// ✅ Good - Reactive property
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;
}

// ❌ Avoid - No change notifications
public class BotEntity
{
    public string? Name { get; set; }
}
```

---

### 3. Dispose Subscriptions

```csharp
// ✅ Good - Proper disposal
public class MyVM : ReactiveObject, IDisposable
{
    private readonly CompositeDisposable subscriptions = new();

    public MyVM()
    {
        this.WhenAnyValue(x => x.SomeProperty)
            .Subscribe(...)
            .DisposeWith(subscriptions);
    }

    public void Dispose() => subscriptions.Dispose();
}
```

---

### 4. Don't Duplicate Business Logic

```csharp
// ✅ Good - Delegate to entity
public class BotVM : KeyValueVM<string, BotEntity>
{
    private async Task StartAsync()
    {
        Value.Start();  // Entity method
        await SaveChanges();
    }
}

// ❌ Avoid - Duplicating logic
public class BotVM : KeyValueVM<string, BotEntity>
{
    private async Task StartAsync()
    {
        // Duplicating what Value.Start() should do!
        Value.IsRunning = true;
        Value.StartTime = DateTime.Now;
        // ...
    }
}
```

---

## Troubleshooting

### Issue: UI Not Updating

**Cause**: Entity doesn't implement `INotifyPropertyChanged`.

**Fix**:
```csharp
// Add ReactiveObject and [Reactive]
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;
}
```

---

### Issue: Command Not Executing

**Cause**: Command's `CanExecute` returns false.

**Debug**:
```csharp
Console.WriteLine($"Can execute: {command.CanExecute.Value}");
```

---

### Issue: Memory Leak

**Cause**: Subscriptions not disposed.

**Fix**: Use `CompositeDisposable` and `DisposeWith()`.

---

## Related Documentation

### Architecture
- **[MVVM Architecture](../architecture/mvvm/README.md)** - High-level MVVM architecture

### Domain Guides
- **[ViewModels Guide](viewmodels-guide.md)** - Comprehensive ViewModel documentation
- **[Reactive Patterns](reactive-patterns.md)** - Reactive programming patterns
- **[Data Binding](data-binding.md)** - UI binding patterns

### UI Documentation
- **[Blazor MVVM Patterns](../ui/blazor-mvvm-patterns.md)** - Blazor-specific MVVM patterns

### Project Documentation
- **[LionFire.Mvvm](../../src/LionFire.Mvvm/CLAUDE.md)** - Core MVVM library *(pending)*
- **[LionFire.Data.Async.Mvvm](../../src/LionFire.Data.Async.Mvvm/CLAUDE.md)** - Async data ViewModels

---

## Summary

LionFire's MVVM implementation provides:

**Key Features**:
- **ReactiveUI Integration** - Reactive properties and commands
- **Thin ViewModels** - UI concerns only, business logic in entities
- **Data Access Wrappers** - ViewModels wrap IObservableReader/Writer
- **Workspace Integration** - Seamless integration with workspace-scoped services

**Primary Use Case**: Building reactive Blazor UIs with clean separation between UI and business logic.

**Next Steps**:
1. Read [ViewModels Guide](viewmodels-guide.md) for detailed ViewModel patterns
2. Study [Reactive Patterns](reactive-patterns.md) for reactive programming
3. Review [Data Binding](data-binding.md) for UI integration

**Most Common Pattern**: `KeyValueVM<TKey, TValue>` for simple entity ViewModels.
