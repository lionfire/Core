# MVVM Architecture

**Overview**: LionFire's Model-View-ViewModel (MVVM) architecture provides a reactive, layered approach to separating presentation logic from domain models. Built on ReactiveUI and DynamicData, it integrates seamlessly with async data patterns, workspace-scoped services, and multiple UI frameworks.

---

## Table of Contents

1. [Why MVVM in LionFire?](#why-mvvm-in-lionfire)
2. [Architecture Layers](#architecture-layers)
3. [Framework Integration](#framework-integration)
4. [Core Patterns](#core-patterns)
5. [Relationship to Data Access](#relationship-to-data-access)
6. [Design Philosophy](#design-philosophy)

---

## Why MVVM in LionFire?

### The Challenge

Modern .NET applications require:
- **Reactive UIs** that update automatically when data changes
- **Async data access** with loading states, caching, and error handling
- **Testable presentation logic** separate from domain models
- **Cross-platform support** (Blazor, WPF, Avalonia, etc.)
- **Observable collections** with transformations and filtering
- **Deep integration with DI** for service injection

### The Solution

LionFire's MVVM architecture provides:

✅ **Reactive-First**: Built on ReactiveUI for automatic change propagation
✅ **Async-Native**: Integrates with `IGetter/ISetter/IValue` async patterns
✅ **Observable Collections**: DynamicData for reactive lists
✅ **Framework-Agnostic Abstractions**: Works with Blazor, WPF, Avalonia
✅ **DI-Integrated**: ViewModels created via dependency injection
✅ **Layered**: Abstractions → Implementation → Specialized ViewModels

---

## Architecture Layers

### Layer Diagram

```
┌────────────────────────────────────────────────────────────────┐
│                     Application Layer                          │
│  - UI Components (Blazor Razor, WPF XAML)                     │
│  - Page ViewModels                                             │
└───────────────────────┬────────────────────────────────────────┘
                        │
                        ↓ Uses
┌────────────────────────────────────────────────────────────────┐
│              Specialized ViewModel Layer                       │
│  LionFire.Data.Async.Mvvm                                     │
│  - ObservableReaderWriterItemVM                               │
│  - ObservableDataVM                                            │
│  - GetterVM, ValueVM, etc.                                     │
└───────────────────────┬────────────────────────────────────────┘
                        │
                        ↓ Built on
┌────────────────────────────────────────────────────────────────┐
│                  MVVM Implementation Layer                     │
│  LionFire.Mvvm                                                │
│  - ViewModelProvider (factory)                                │
│  - Object Inspection                                           │
│  - ReactiveUI integration                                      │
│  - Hosting extensions                                          │
└───────────────────────┬────────────────────────────────────────┘
                        │
                        ↓ Implements
┌────────────────────────────────────────────────────────────────┐
│                MVVM Abstractions Layer                         │
│  LionFire.Mvvm.Abstractions                                   │
│  - IViewModel<T>                                               │
│  - IViewModelProvider                                          │
│  - IInspector* interfaces                                      │
└────────────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

**Abstractions Layer** (`LionFire.Mvvm.Abstractions`):
- Defines `IViewModel<TModel>` interface
- `IViewModelProvider` for ViewModel factory pattern
- Object inspection interfaces
- No concrete implementations
- Minimal dependencies

**Implementation Layer** (`LionFire.Mvvm`):
- Concrete `ViewModelProvider` using Microsoft.Extensions.DI
- ReactiveUI integration (Splat bridge)
- Object inspection system implementation
- Hosting extensions (`AddMvvm()`)
- ViewModel registry and scanning

**Specialized ViewModel Layer** (`LionFire.Data.Async.Mvvm`):
- ViewModels for async data patterns:
  - `GetterVM<T>` wraps `IGetter<T>`
  - `ValueVM<T>` wraps `IValue<T>`
  - `ObservableReaderItemVM` wraps `IObservableReader`
- Collection ViewModels
- Async operation ViewModels
- Polling and caching support

**Application Layer**:
- Consumes ViewModels in UI components
- Implements page/screen ViewModels
- Business logic specific to application

---

## Framework Integration

### ReactiveUI Integration

**Foundation**: ReactiveUI provides the reactive programming model.

```csharp
using ReactiveUI;
using ReactiveUI.SourceGenerators;

public partial class MyViewModel : ReactiveObject
{
    [Reactive]  // Source generator creates property with INPC
    private string? _name;

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public MyViewModel()
    {
        SaveCommand = ReactiveCommand.CreateFromTask(Save);

        // React to name changes
        this.WhenAnyValue(x => x.Name)
            .Subscribe(name => Console.WriteLine($"Name changed to: {name}"));
    }

    private async Task Save() { /* ... */ }
}
```

**Key Features Used**:
- `ReactiveObject` - Base class with `INotifyPropertyChanged`
- `[Reactive]` - Source generator for properties
- `ReactiveCommand` - Commands with async support
- `WhenAnyValue` - Property change observables
- `IObservableCache` - Reactive collections

### CommunityToolkit.Mvvm Integration

**Modern Features**: Adds source generators and additional patterns.

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class MyViewModel : ObservableObject
{
    [ObservableProperty]  // Generates property with INPC
    private string? _name;

    [RelayCommand]  // Generates ICommand implementation
    private async Task Save() { /* ... */ }
}
```

**Compatibility**: Can mix ReactiveUI and CommunityToolkit patterns.

### DynamicData Integration

**Observable Collections**: All collection patterns use DynamicData.

```csharp
using DynamicData;

public class MyViewModel
{
    private SourceCache<Item, string> _items = new(x => x.Id);
    public IObservableCache<Item, string> Items => _items;

    // Transformations
    public IObservable<IChangeSet<ItemVM, string>> ItemVMs =>
        _items.Connect()
              .Transform(item => new ItemVM(item));
}
```

### Microsoft.Extensions.DependencyInjection

**DI Integration**: ViewModels are created via DI container.

```csharp
// Registration
services.AddMvvm(typeof(MyViewModelsAssembly).Assembly);
services.AddTransient<MyViewModel>();

// Usage
public class MyService
{
    private readonly IViewModelProvider vmProvider;
    private readonly IServiceProvider serviceProvider;

    // ViewModels created with full DI support
    var vm = vmProvider.Activate<MyViewModel, MyModel>(model);
    // Or: ActivatorUtilities.CreateInstance<MyViewModel>(serviceProvider, model);
}
```

---

## Core Patterns

### 1. ViewModel Wraps Model

```csharp
// Model: Domain entity
public class BotEntity
{
    public string Name { get; set; }
    public bool Enabled { get; set; }
}

// ViewModel: Presentation logic
public class BotVM : ReactiveObject, IViewModel<BotEntity>
{
    public BotVM(BotEntity value)
    {
        Value = value;
    }

    public BotEntity Value { get; }

    // UI-specific computed property
    public string DisplayName => $"Bot: {Value.Name}";

    // UI-specific state (not in model)
    [Reactive]
    private bool _isRunning;

    // Commands
    public ReactiveCommand<Unit, Unit> StartCommand { get; }
}
```

### 2. ViewModel Provider Factory

```csharp
// Get VM for model via factory
var vm = viewModelProvider.Activate<BotVM, BotEntity>(botEntity);

// Provider handles:
// - Finding/creating VM type
// - Constructor injection (model + services)
// - Initialization
```

### 3. Reactive Properties

```csharp
public partial class MyViewModel : ReactiveObject
{
    [Reactive] private string? _name;

    // React to changes
    this.WhenAnyValue(x => x.Name)
        .Where(name => !string.IsNullOrEmpty(name))
        .Subscribe(name => UpdateSomething(name));
}
```

### 4. Observable Collections

```csharp
private SourceCache<Item, string> _items = new(x => x.Id);

// Public observable interface
public IObservableCache<Item, string> Items => _items;

// Transform to VMs
public IObservable<IChangeSet<ItemVM, string>> ItemVMs =>
    _items.Connect()
          .Transform(item => new ItemVM(item))
          .Publish()
          .RefCount();
```

### 5. Async Commands

```csharp
public ReactiveCommand<Unit, MyResult> LoadCommand { get; }

public MyViewModel()
{
    LoadCommand = ReactiveCommand.CreateFromTask(LoadAsync);

    // Bind to loading state
    LoadCommand.IsExecuting
        .ToPropertyEx(this, x => x.IsLoading);
}

private async Task<MyResult> LoadAsync() { /* ... */ }
```

---

## Relationship to Data Access

### Integration with Async Data Patterns

LionFire MVVM **wraps** async data access patterns with ViewModels:

```
Domain Entity (BotEntity)
    ↓ Accessed via
IObservableReader<string, BotEntity>
    ↓ Wrapped by
ObservableReaderItemVM<string, BotEntity, BotVM>
    ↓ Creates
BotVM (ViewModel)
    ↓ Bound to
UI Component (Blazor/WPF)
```

### ViewModel Types for Data Access

```csharp
// Wrap IGetter<T>
GetterVM<T>

// Wrap IValue<T> (read/write)
ValueVM<T>

// Wrap IObservableReader<TKey, TValue>
ObservableReaderVM<TKey, TValue, TValueVM>

// Wrap IObservableReader + Writer
ObservableReaderWriterVM<TKey, TValue, TValueVM>

// Single item from reader/writer
ObservableReaderWriterItemVM<TKey, TValue, TValueVM>
```

**See**: [Data Binding Integration](data-binding.md) for details.

### Workspace Integration

ViewModels integrate with workspace-scoped services:

```csharp
// Workspace services provide IObservableReader/Writer
var reader = workspaceServices.GetService<IObservableReader<string, BotEntity>>();
var writer = workspaceServices.GetService<IObservableWriter<string, BotEntity>>();

// VM wraps workspace services
var vm = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);
vm.Id = "my-bot";  // Triggers load from workspace

// UI binds to VM
<MudTextField @bind-Value="vm.Value.Name" />
```

**See**: [Workspace Architecture](../workspaces/README.md) for workspace patterns.

---

## Design Philosophy

### 1. Reactive-First

**Everything is observable**:
- Properties notify changes via `INotifyPropertyChanged`
- Collections emit changesets via DynamicData
- Commands expose execution state
- Async operations return `IObservable<T>`

**Benefits**:
- Automatic UI updates
- Declarative reactive pipelines
- Composable transformations
- Memory-efficient subscriptions (RefCount)

### 2. Async-Native

**Async operations are first-class**:
- `ReactiveCommand.CreateFromTask` for async commands
- `ToProperty` for deriving properties from observables
- Loading/error states built-in
- Cancellation support

**Example**:
```csharp
LoadCommand = ReactiveCommand.CreateFromTask(LoadAsync);
LoadCommand.IsExecuting.ToPropertyEx(this, x => x.IsLoading);
LoadCommand.ThrownExceptions.Subscribe(ex => ErrorMessage = ex.Message);
```

### 3. Layered Abstractions

**Separation of concerns**:
- Abstractions define contracts
- Implementation provides framework integration
- Specialized VMs add domain-specific features
- Applications compose as needed

**Benefits**:
- Testable (mock abstractions)
- Swappable implementations
- No framework lock-in
- Clear dependencies

### 4. DI-Integrated

**ViewModels are services**:
- Created via `IViewModelProvider` or `ActivatorUtilities`
- Constructor injection for dependencies
- Scoped/Transient/Singleton lifetimes
- Integrates with Microsoft.Extensions.* ecosystem

### 5. Inspection-Oriented

**Runtime introspection**:
- Object inspection system
- Build generic property editors
- Debugging and diagnostic tools
- Custom inspection strategies

---

## MVVM Stack Overview

### Complete Stack Diagram

```
Application
    ↓
Page ViewModels (e.g., BotsPageVM)
    ↓
Specialized ViewModels (e.g., ObservableReaderWriterItemVM)
    ↓
Base ViewModels (e.g., KeyValueVM, ReactiveObject)
    ↓
MVVM Framework (LionFire.Mvvm)
    ↓
MVVM Abstractions (LionFire.Mvvm.Abstractions)
    ↓
ReactiveUI + DynamicData + CommunityToolkit.Mvvm
```

### Data Flow

```
User Input (UI)
    ↓
ViewModel Command
    ↓
ViewModel Property Change
    ↓
Model Update
    ↓
Persistence (IObservableWriter)
    ↓
File/Database
    ↓ (File watcher detects change)
IObservableReader (emits changeset)
    ↓
ViewModel (receives update)
    ↓
UI (automatic refresh via reactive binding)
```

---

## Key Components

### 1. IViewModel\<TModel\>

**Core abstraction** linking ViewModels to Models:

```csharp
public interface IViewModel<out TModel> : IReadWrapper<TModel>
{
    // TModel Value { get; }
}
```

### 2. IViewModelProvider

**Factory** for creating ViewModels:

```csharp
public interface IViewModelProvider
{
    TViewModel? Activate<TViewModel, TModel>(TModel model, ...);
    object? Activate<TModel>(TModel model, ...);
}
```

### 3. ReactiveObject Base

**Foundation** for reactive properties:

```csharp
public partial class MyVM : ReactiveObject
{
    [Reactive] private string? _name;  // Auto-generates INPC
}
```

### 4. ObservableDataVM

**Specialized VM** for workspace document collections:

```csharp
public class ObservableDataVM<TKey, TValue, TValueVM>
{
    public IObservableReader<TKey, TValue>? Data { get; set; }
    public IObservableCache<TValueVM, TKey> Items { get; }
    // CRUD commands, edit mode, etc.
}
```

### 5. KeyValueVM

**Base class** for key-value pair ViewModels:

```csharp
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value) { }
}
```

---

## Framework Integration Summary

| Framework | Integration | Status |
|-----------|-------------|--------|
| **ReactiveUI** | Core foundation - `ReactiveObject`, reactive commands, observables | ✅ Primary |
| **CommunityToolkit.Mvvm** | Source generators (`[ObservableProperty]`, `[RelayCommand]`) | ✅ Supported |
| **DynamicData** | Observable collections, transformations, filtering | ✅ Primary |
| **Microsoft.Extensions.DI** | ViewModel creation, service injection | ✅ Primary |
| **Blazor** | Via `LionFire.Blazor.Components.MudBlazor` | ✅ Full Support |
| **WPF/Avalonia** | Via `LionFire.Avalon` | ⚠️ Legacy Support |

---

## Common Scenarios

### Scenario 1: Simple ViewModel

```csharp
public partial class PersonVM : ReactiveObject, IViewModel<Person>
{
    public PersonVM(Person value)
    {
        Value = value;
    }

    public Person Value { get; }

    [Reactive] private bool _isSelected;

    // Computed property
    public string DisplayName => $"{Value.FirstName} {Value.LastName}";
}
```

### Scenario 2: ViewModel with Commands

```csharp
public partial class PersonVM : ReactiveObject, IViewModel<Person>
{
    private readonly IPersonService personService;

    public PersonVM(Person value, IPersonService personService)
    {
        Value = value;
        this.personService = personService;

        SaveCommand = ReactiveCommand.CreateFromTask(Save);
        DeleteCommand = ReactiveCommand.CreateFromTask(Delete);
    }

    public Person Value { get; }

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    private async Task Save() => await personService.Save(Value);
    private async Task Delete() => await personService.Delete(Value);
}
```

### Scenario 3: Collection ViewModel

```csharp
public class PeopleVM : ReactiveObject
{
    private SourceCache<Person, int> _people = new(p => p.Id);

    public IObservableCache<PersonVM, int> PersonVMs { get; }

    public PeopleVM()
    {
        PersonVMs = _people
            .Connect()
            .Transform(person => new PersonVM(person))
            .AsObservableCache();
    }

    public void AddPerson(Person person) => _people.AddOrUpdate(person);
}
```

### Scenario 4: Async Data ViewModel

```csharp
public class PersonDetailVM : ReactiveObject
{
    public ObservableReaderWriterItemVM<int, Person, PersonVM> PersonVM { get; }

    public PersonDetailVM(
        IObservableReader<int, Person> reader,
        IObservableWriter<int, Person> writer)
    {
        PersonVM = new ObservableReaderWriterItemVM<int, Person, PersonVM>(reader, writer);
    }

    public async Task LoadPerson(int id)
    {
        PersonVM.Id = id;  // Triggers async load
        // PersonVM.Value will be set when loaded
    }

    public async Task Save()
    {
        await PersonVM.Write();  // Persists changes
    }
}
```

---

## Architecture Principles

### Separation of Concerns

**Model**: Domain logic and data
```csharp
public class BotEntity  // Pure data, no UI concerns
{
    public string Name { get; set; }
    public bool Enabled { get; set; }
}
```

**ViewModel**: Presentation logic
```csharp
public class BotVM : KeyValueVM<string, BotEntity>  // UI-specific logic
{
    public string DisplayName => $"Bot: {Value.Name}";
    public bool IsRunning { get; set; }  // UI state
    public ReactiveCommand StartCommand { get; }  // UI commands
}
```

**View**: UI markup/components (Razor, XAML)
```razor
<MudTextField @bind-Value="BotVM.Value.Name" />
<MudButton OnClick="() => BotVM.StartCommand.Execute()">Start</MudButton>
```

### Testability

**ViewModels can be tested** without UI:

```csharp
[Fact]
public async Task SaveCommand_UpdatesModel()
{
    // Arrange
    var model = new BotEntity { Name = "Test" };
    var mockService = new Mock<IBotService>();
    var vm = new BotVM(model, mockService.Object);

    // Act
    vm.Value.Name = "Updated";
    await vm.SaveCommand.Execute();

    // Assert
    mockService.Verify(s => s.Save(It.IsAny<BotEntity>()), Times.Once);
}
```

### Reactive Pipelines

**Declarative transformations**:

```csharp
// Filter enabled bots
var enabledBots = _bots
    .Connect()
    .Filter(bot => bot.Value.Enabled)
    .Transform(bot => new BotVM(bot.Key, bot.Value));

// Sort by name
var sortedBots = enabledBots
    .Sort(SortExpressionComparer<BotVM>.Ascending(b => b.Value.Name));

// Bind to UI
sortedBots.Bind(out ReadOnlyObservableCollection<BotVM> botList);
```

---

## Summary

### Why LionFire MVVM?

1. **Reactive-First**: Automatic UI updates via ReactiveUI
2. **Async-Native**: First-class async data support
3. **Observable Collections**: DynamicData for reactive lists
4. **DI-Integrated**: Full Microsoft.Extensions.DependencyInjection support
5. **Layered**: Clear separation (Abstractions → Implementation → Specialized)
6. **Framework-Agnostic**: Works with Blazor, WPF, Avalonia
7. **Testable**: ViewModels testable without UI
8. **Inspection-Oriented**: Runtime object inspection for tools

### Architecture Highlights

- **3 Layers**: Abstractions → Implementation → Specialized ViewModels
- **3 Frameworks**: ReactiveUI + DynamicData + CommunityToolkit.Mvvm
- **Workspace Integration**: ViewModels work with workspace-scoped services
- **Data Access Integration**: ViewModels wrap `IGetter/IValue/IObservableReader`

### Related Documentation

- **[MVVM Layers](mvvm-layers.md)** - Detailed layer breakdown
- **[Reactive MVVM](reactive-mvvm.md)** - ReactiveUI patterns
- **[Data Binding](data-binding.md)** - Integration with async data
- **[UI Frameworks](ui-frameworks.md)** - Blazor/WPF integration
- **Library References**:
  - [LionFire.Mvvm.Abstractions](../../src/LionFire.Mvvm.Abstractions/CLAUDE.md)
  - [LionFire.Mvvm](../../src/LionFire.Mvvm/CLAUDE.md)
  - [LionFire.Data.Async.Mvvm](../../src/LionFire.Data.Async.Mvvm/CLAUDE.md)
