# MVVM Layer Architecture

**Overview**: LionFire's MVVM system is organized into three distinct layers, each with specific responsibilities and dependencies. This layered approach provides flexibility, testability, and clear separation of concerns.

---

## Table of Contents

1. [Layer Overview](#layer-overview)
2. [Abstractions Layer](#abstractions-layer)
3. [Implementation Layer](#implementation-layer)
4. [Specialized ViewModels Layer](#specialized-viewmodels-layer)
5. [Layer Dependencies](#layer-dependencies)
6. [Design Patterns](#design-patterns)

---

## Layer Overview

### Three-Layer Architecture

```
┌─────────────────────────────────────────────────────────┐
│  Layer 3: Specialized ViewModels                        │
│  LionFire.Data.Async.Mvvm                              │
│  - Domain-specific ViewModels                           │
│  - Async data integration                               │
│  - Collection ViewModels                                │
└────────────────┬────────────────────────────────────────┘
                 │ Depends on
┌────────────────▼────────────────────────────────────────┐
│  Layer 2: Implementation                                │
│  LionFire.Mvvm                                         │
│  - ViewModelProvider                                    │
│  - ReactiveUI integration                               │
│  - Object inspection                                    │
│  - Hosting extensions                                   │
└────────────────┬────────────────────────────────────────┘
                 │ Implements
┌────────────────▼────────────────────────────────────────┐
│  Layer 1: Abstractions                                  │
│  LionFire.Mvvm.Abstractions                            │
│  - IViewModel<T>                                        │
│  - IViewModelProvider                                   │
│  - Inspector interfaces                                 │
└─────────────────────────────────────────────────────────┘
```

### Layer Principles

**Layer 1 (Abstractions)**:
- Minimal dependencies (only essential abstractions)
- Defines contracts, no implementations
- Framework-agnostic
- Stable APIs

**Layer 2 (Implementation)**:
- Concrete implementations of Layer 1
- Framework integration (ReactiveUI, DI)
- Hosting and configuration
- Reusable infrastructure

**Layer 3 (Specialized)**:
- Domain-specific ViewModels
- Builds on Layer 2
- Async data integration
- Application-ready components

---

## Abstractions Layer

### Library: LionFire.Mvvm.Abstractions

**Purpose**: Define core MVVM contracts without implementation dependencies.

**Location**: `/src/LionFire.Mvvm.Abstractions/`

**Dependencies**: Minimal
- ReactiveUI (abstractions only)
- DynamicData (abstractions only)
- LionFire.Structures

### Core Interfaces

#### IViewModel\<TModel\>

**The central abstraction** linking ViewModels to Models:

```csharp
namespace LionFire.Mvvm;

public interface IViewModel<out TModel> : IReadWrapper<TModel>
{
    // TModel Value { get; }  (inherited from IReadWrapper)
}
```

**Usage**:
```csharp
public class PersonVM : ReactiveObject, IViewModel<Person>
{
    public PersonVM(Person value)
    {
        Value = value;
    }

    public Person Value { get; }  // Satisfies IViewModel<Person>
}
```

**Benefits**:
- Type-safe ViewModel-Model relationship
- Enables finding ViewModels by Model
- Uniform access to underlying Model

#### IViewModelProvider

**Factory interface** for creating ViewModels:

```csharp
public interface IViewModelProvider
{
    bool CanTransform<TModel, TViewModel>();

    TViewModel? TryActivate<TViewModel, TModel>(
        TModel model,
        object[]? parameters = null,
        Action<TViewModel>? initializer = null);

    object? TryActivate<TModel>(
        TModel model,
        object[]? parameters = null,
        Action<object>? initializer = null);
}
```

**Usage**:
```csharp
var provider = services.GetRequiredService<IViewModelProvider>();

// Create specific VM type
var personVM = provider.TryActivate<PersonVM, Person>(person);

// Create generic (finds appropriate VM)
var vm = provider.TryActivate<Person>(person);  // Returns PersonVM if registered
```

**Benefits**:
- Decouples ViewModel creation from consumers
- Supports DI and constructor injection
- Extensible via custom providers

### Object Inspection Interfaces

**Purpose**: Runtime object introspection for building property editors.

```csharp
// Base node in inspection tree
public interface IInspectorNode { }

// Inspectable member (property/field)
public interface IInspectorMember : IInspectorNode
{
    string Name { get; }
    Type MemberType { get; }
    object? GetValue(object instance);
    void SetValue(object instance, object? value);
}

// ViewModel for a member
public interface IInspectorMemberVM { }

// ViewModel for object inspection
public interface IInspectorObjectVM { }
```

**Use Cases**:
- Property grids
- Object browsers
- Debugging tools
- Generic editors

### Reactive Notification Interfaces

**Deep change tracking**:

```csharp
// Notifies when direct child properties change
public interface INotifiesChildChanged
{
    IObservable<string> ChildChanged { get; }
}

// Notifies when any descendant changes
public interface INotifiesChildDeeplyChanged
{
    IObservable<string> ChildDeeplyChanged { get; }
}
```

---

## Implementation Layer

### Library: LionFire.Mvvm

**Purpose**: Concrete MVVM infrastructure integrating ReactiveUI with Microsoft.Extensions.DI.

**Location**: `/src/LionFire.Mvvm/`

**Dependencies**:
- LionFire.Mvvm.Abstractions (implements)
- ReactiveUI
- CommunityToolkit.Mvvm
- DynamicData
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Hosting
- Splat (ReactiveUI's service locator)

### Key Components

#### 1. ViewModelProvider (CompoundViewModelProvider)

**Concrete implementation** of `IViewModelProvider`:

```csharp
public class CompoundViewModelProvider : IViewModelProvider
{
    private readonly IServiceProvider serviceProvider;

    public TViewModel? TryActivate<TViewModel, TModel>(TModel model, ...)
    {
        // Uses ActivatorUtilities.CreateInstance with DI
        return ActivatorUtilities.CreateInstance<TViewModel>(serviceProvider, model);
    }
}
```

**Features**:
- Uses Microsoft.Extensions.DependencyInjection
- Constructor injection for models and services
- Caches reflection metadata for performance
- Delegates to specialized providers

#### 2. ReactiveUI Integration

**Splat Bridge** (`ReactiveUIHostingX`):

```csharp
public static IServiceCollection UseMicrosoftDIForReactiveUI(
    this IServiceCollection services)
{
    // Bridge ReactiveUI's Splat to Microsoft.Extensions.DI
    Locator.SetLocator(new MicrosoftDependencyResolver(serviceProvider));
}
```

**Result**: ReactiveUI can resolve dependencies from Microsoft.Extensions.DI container.

#### 3. ViewModel Registry

**Automatic scanning** for ViewModels:

```csharp
services.AddMvvm(typeof(MyViewModelsAssembly).Assembly);

// Scans assembly for IViewModel<T> implementations
// Registers in ViewModelRegistry
// Enables generic ViewModel lookup
```

#### 4. Object Inspection System

**Runtime introspection** infrastructure:

Components:
- `InspectedObjectType` - Cached type metadata
- `TypeInteractionModel` - Member interaction models
- `InspectedObjectItem` - Inspectable object instance
- `CustomInspector` - User-defined inspection

ViewModels:
- `AsyncPropertyVM` - Async-loaded property
- `ObservableVM` - IObservable<T> wrapper
- `AsyncEnumerableVM` - IAsyncEnumerable<T> wrapper

**Usage**:
```csharp
var inspector = services.GetRequiredService<IObjectInspector>();
var inspection = inspector.Inspect(myObject);

foreach (var member in inspection.Members)
{
    Console.WriteLine($"{member.Name}: {member.Value}");
}
```

#### 5. Hosting Extensions

**MvvmHostingX.AddMvvm()**:

```csharp
public static IServiceCollection AddMvvm(
    this IServiceCollection services,
    bool useDefaults = true,
    params Assembly[] viewModelAssemblies)
{
    return services
        .UseMicrosoftDIForReactiveUI()          // Splat bridge
        .AddSingleton<ViewModelRegistry>()       // VM registry
        .AddSingleton<IViewModelProvider, CompoundViewModelProvider>()
        .AddObjectInspection()                   // Inspection system
        .ScanForViewModels(viewModelAssemblies); // Assembly scanning
}
```

---

## Specialized ViewModels Layer

### Library: LionFire.Data.Async.Mvvm

**Purpose**: ViewModels that integrate MVVM with async data access patterns.

**Location**: `/src/LionFire.Data.Async.Mvvm/`

**Dependencies**:
- LionFire.Mvvm (builds on implementation)
- LionFire.Data.Async.Abstractions (`IGetter`, `IValue`, etc.)
- LionFire.Data.Async.Reactive (`IGetterRxO`, etc.)
- LionFire.Reactive (`IObservableReader/Writer`)

### ViewModel Types

#### For Async Data Access

**GetterVM\<T\>**: Wraps `IGetter<T>`
```csharp
public class GetterVM<T> : ReactiveObject
{
    public IGetter<T> Getter { get; }
    public ReactiveCommand<Unit, T> GetCommand { get; }
    [Reactive] private T? _value;
}
```

**ValueVM\<T\>**: Wraps `IValue<T>` (read/write)
```csharp
public class ValueVM<T> : ReactiveObject
{
    public IValue<T> Value { get; }
    public ReactiveCommand<Unit, T> GetCommand { get; }
    public ReactiveCommand<T, Unit> SetCommand { get; }
}
```

#### For Observable Collections

**ObservableReaderVM\<TKey, TValue, TValueVM\>**: Wraps `IObservableReader`
```csharp
public class ObservableReaderVM<TKey, TValue, TValueVM> : ReactiveObject
{
    public IObservableReader<TKey, TValue> Reader { get; }
    public IObservableCache<TValueVM, TKey> Items { get; }

    // Automatically subscribes to Reader.Values.Connect()
    // Creates TValueVM for each TValue
}
```

**ObservableReaderWriterVM\<TKey, TValue, TValueVM\>**: Read + write
```csharp
public class ObservableReaderWriterVM<TKey, TValue, TValueVM>
    : ObservableReaderVM<TKey, TValue, TValueVM>
{
    public IObservableWriter<TKey, TValue> Writer { get; }
    // Adds write capabilities
}
```

**ObservableReaderWriterItemVM\<TKey, TValue, TValueVM\>**: Single item
```csharp
public class ObservableReaderWriterItemVM<TKey, TValue, TValueVM>
{
    public TKey? Id { get; set; }              // Set to load
    public TValueVM? Value { get; }            // Loaded entity as VM
    public ValueTask Write() { }               // Persist changes
}
```

#### For Data Collections

**ObservableDataVM\<TKey, TValue, TValueVM\>**: Complete collection management
```csharp
public class ObservableDataVM<TKey, TValue, TValueVM>
{
    public IObservableReader<TKey, TValue>? Data { get; set; }
    public IObservableCache<TValueVM, TKey> Items { get; }

    // CRUD commands
    public ReactiveCommand Create { get; }
    public ReactiveCommand Delete { get; }

    // Edit mode
    public EditMode AllowedEditModes { get; set; }
    public bool CanCreate { get; }
    public bool CanDelete { get; }
}
```

Used by `ObservableDataView` Blazor component.

### Hosting Extensions

**AsyncHostingX.AddReactivePersistenceMvvm()**:

```csharp
services.AddReactivePersistenceMvvm()
    .AddTransient(typeof(ObservableDataVM<,,>))
    .AddTransient(typeof(ObservableReaderVM<,,>))
    .AddTransient(typeof(ObservableReaderItemVM<,,>))
    .AddTransient(typeof(ObservableReaderWriterVM<,,>))
    .AddTransient(typeof(ObservableReaderWriterItemVM<,,>));
```

---

## Layer Dependencies

### Dependency Graph

```
Application ViewModels
    ↓ depends on
LionFire.Data.Async.Mvvm (Layer 3)
    ↓ depends on
LionFire.Mvvm (Layer 2)
    ↓ depends on
LionFire.Mvvm.Abstractions (Layer 1)
    ↓ depends on
ReactiveUI + DynamicData (External)
```

### Dependency Rules

**Layer 1** may depend on:
- ✅ External abstractions (ReactiveUI interfaces, DynamicData interfaces)
- ✅ LionFire.Structures (basic types)
- ❌ No concrete implementations
- ❌ No UI frameworks

**Layer 2** may depend on:
- ✅ Layer 1 (Abstractions)
- ✅ ReactiveUI (concrete)
- ✅ DynamicData (concrete)
- ✅ Microsoft.Extensions.* (DI, Hosting)
- ❌ No UI framework specifics (Blazor, WPF)

**Layer 3** may depend on:
- ✅ Layer 1 and 2
- ✅ LionFire.Data.Async.* (data access)
- ✅ LionFire.Reactive (persistence)
- ❌ No UI framework specifics

### Cross-Cutting Libraries

**UI Framework Integration** (separate libraries):
- `LionFire.Blazor.Components.MudBlazor` - Blazor components
- `LionFire.Avalon` - WPF components

These depend on all MVVM layers but are **not part of the core MVVM stack**.

---

## Design Patterns

### Pattern 1: Abstract → Implement → Specialize

```csharp
// Layer 1: Abstract interface
public interface IViewModel<out TModel> { }

// Layer 2: Base implementation (optional)
public abstract class ViewModel<TModel> : ReactiveObject, IViewModel<TModel>
{
    protected ViewModel(TModel value) { Value = value; }
    public TModel Value { get; }
}

// Layer 3: Specialized for async data
public class ObservableReaderItemVM<TKey, TValue, TValueVM> : ReactiveObject
{
    public ObservableReaderItemVM(IObservableReader<TKey, TValue> reader) { }
}

// Application: Domain-specific
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value) { }
    // Application-specific logic
}
```

### Pattern 2: Progressive Enhancement

Each layer adds capabilities:

```csharp
// Layer 1: Just the contract
IViewModel<Person>

// Layer 2: Adds infrastructure
+ ViewModelProvider (factory)
+ DI integration
+ Inspection support

// Layer 3: Adds data access
+ IGetter/IValue wrapping
+ Observable collection support
+ Async operations

// Application: Adds domain logic
+ Business rules
+ Commands
+ Computed properties
```

### Pattern 3: Dependency Inversion

Higher layers depend on abstractions, not concrete implementations:

```csharp
// Layer 3 depends on Layer 1 abstraction, not Layer 2 implementation
public class ObservableDataVM<TKey, TValue, TValueVM>
{
    // Uses abstract IViewModelProvider, not CompoundViewModelProvider
    public ObservableDataVM(IViewModelProvider vmProvider) { }
}
```

---

## Complete Layer Breakdown

### Layer 1: LionFire.Mvvm.Abstractions

**Namespaces**:
- `LionFire.Mvvm` - Core ViewModel interfaces
- `LionFire.Mvvm.ObjectInspection` - Inspection interfaces
- `LionFire.Mvvm.Services` - IViewModelProvider

**Key Types**:
- `IViewModel<TModel>` - ViewModel-Model relationship
- `IViewModelProvider` - ViewModel factory
- `IInspectorNode` - Inspection hierarchy
- `IInspectorMember` - Inspectable member
- `INotifiesChildChanged` - Change notifications

**When to Use**:
- Building custom MVVM frameworks
- Need abstractions without implementation
- Defining ViewModel contracts for libraries

### Layer 2: LionFire.Mvvm

**Namespaces**:
- `LionFire.Hosting` - Hosting extensions
- `LionFire.Mvvm` - Core implementations
- `LionFire.Mvvm.Inspection` - Inspection system

**Key Types**:
- `CompoundViewModelProvider` - Factory implementation
- `ViewModelRegistry` - Type registry
- `InspectedObjectType` - Type metadata cache
- `TypeInteractionModel` - Member interaction
- `DisposableBaseViewModel` - Base class

**When to Use**:
- Standard MVVM application setup
- Need ViewModel provider with DI
- Want object inspection features
- Building on ReactiveUI

### Layer 3: LionFire.Data.Async.Mvvm

**Namespaces**:
- `LionFire.Mvvm` - Specialized ViewModels
- `LionFire.Data.Mvvm` - Data-oriented ViewModels
- `LionFire.Hosting` - Registration extensions

**Key Types**:
- `GetterVM<T>`, `SetterVM<T>`, `ValueVM<T>`
- `ObservableReaderVM<TKey, TValue, TValueVM>`
- `ObservableReaderWriterVM<TKey, TValue, TValueVM>`
- `ObservableReaderWriterItemVM<TKey, TValue, TValueVM>`
- `ObservableDataVM<TKey, TValue, TValueVM>`
- `AsyncKeyedCollectionVM<TKey, TValue, TValueVM>`

**When to Use**:
- Wrapping async data access in ViewModels
- Working with observable collections
- Integrating with workspace documents
- Building reactive UIs for data-driven apps

---

## Usage by Layer

### Application Uses All Layers

```csharp
// Application code
public class BotsPageVM
{
    public BotsPageVM(
        IViewModelProvider vmProvider,              // Layer 2
        IObservableReader<string, BotEntity> reader // From workspace
    )
    {
        // Create Layer 3 specialized VM
        BotsVM = new ObservableReaderVM<string, BotEntity, BotVM>(reader);

        // BotsVM implements IViewModel<...> from Layer 1
        // Created via provider from Layer 2
    }

    public ObservableReaderVM<string, BotEntity, BotVM> BotsVM { get; }
}
```

### Library Only Uses Lower Layers

```csharp
// Layer 3 library code
public class ObservableReaderVM<TKey, TValue, TValueVM>
{
    public ObservableReaderVM(
        IViewModelProvider vmProvider  // Layer 1 abstraction (not Layer 2!)
    )
    {
        // Only depends on abstraction
        // Doesn't care about CompoundViewModelProvider
    }
}
```

---

## Summary

### Layer Comparison

| Aspect | Layer 1 (Abstractions) | Layer 2 (Implementation) | Layer 3 (Specialized) |
|--------|----------------------|------------------------|---------------------|
| **Purpose** | Define contracts | Provide infrastructure | Domain-specific VMs |
| **Dependencies** | Minimal | ReactiveUI, DI | Layers 1-2 + Data patterns |
| **Stability** | Very stable | Stable | Evolving |
| **When to reference** | Building frameworks | Standard apps | Data-driven apps |
| **Examples** | IViewModel<T> | CompoundViewModelProvider | ObservableReaderItemVM |

### Key Takeaways

1. **Layered architecture** enables flexibility and testability
2. **Each layer has clear responsibilities** and dependencies
3. **Abstractions** make the system extensible and testable
4. **Implementation** provides ReactiveUI + DI integration
5. **Specialized ViewModels** add async data access capabilities
6. **Applications** consume all layers but depend on abstractions where possible

### Next Steps

- **[Reactive MVVM Patterns](reactive-mvvm.md)** - ReactiveUI usage details
- **[Data Binding Integration](data-binding.md)** - Async data + MVVM
- **[UI Framework Support](ui-frameworks.md)** - Blazor and WPF integration
- **Library References**:
  - [LionFire.Mvvm.Abstractions](../../src/LionFire.Mvvm.Abstractions/CLAUDE.md)
  - [LionFire.Mvvm](../../src/LionFire.Mvvm/CLAUDE.md)
  - [LionFire.Data.Async.Mvvm](../../src/LionFire.Data.Async.Mvvm/CLAUDE.md)
