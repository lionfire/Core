# LionFire.Mvvm

## Overview

Concrete implementation of the LionFire MVVM framework integrating ReactiveUI, CommunityToolkit.Mvvm, and DynamicData. Provides hosting extensions, dependency injection support, and a powerful object inspection system.

**Layer**: Toolkit
**Target**: .NET 9.0
**Root Namespace**: `LionFire`

## Key Dependencies

- **CommunityToolkit.Mvvm** - Modern MVVM toolkit (source generators, commands, etc.)
- **ReactiveUI** - Reactive extensions for MVVM
- **DynamicData** - Reactive collections
- **Microsoft.Extensions.DependencyInjection** - DI container integration
- **Microsoft.Extensions.Hosting** - Application lifetime management
- **Splat** - ReactiveUI's service locator (bridged to Microsoft.Extensions.DI)

Internal Dependencies:
- LionFire.Core
- LionFire.Mvvm.Abstractions
- LionFire.Data.Async.Abstractions
- LionFire.Data.Async.ReactiveUI
- LionFire.Hosting
- LionFire.Inspection
- LionFire.Metadata

## Getting Started

### Basic Setup

```csharp
var builder = Host.CreateApplicationBuilder();

// Add MVVM with ReactiveUI integration
builder.Services.AddMvvm(typeof(MyViewModelsAssembly).Assembly);

var app = builder.Build();
await app.RunAsync();
```

This registers:
- ReactiveUI with Microsoft.Extensions.DependencyInjection
- ViewModel type registry and scanning
- Object inspector system
- ViewModelProvider for ViewModel activation
- Summarizer for object descriptions

## Core Features

### 1. Hosting Extensions

**MvvmHostingX.AddMvvm()**
```csharp
services.AddMvvm(useDefaults: true, viewModelAssemblies);
```

Sets up:
- ReactiveUI integration with Microsoft DI (via Splat)
- ViewModel type registry (scans assemblies for `IViewModel<>` implementations)
- Object inspection system
- Compound ViewModelProvider

**ReactiveUIHostingX.UseMicrosoftDIForReactiveUI()**
- Bridges ReactiveUI's Splat service locator to Microsoft.Extensions.DependencyInjection
- Enables using MS DI for ReactiveUI dependency resolution

### 2. ViewModel Provider System

**CompoundViewModelProvider** - Main implementation of `IViewModelProvider`:
- Activates ViewModels using Microsoft.Extensions.DependencyInjection
- Supports constructor injection of models and services
- Can create ViewModels generically from model types
- Delegates to specialized providers

### 3. Object Inspection

Runtime object inspection for building property editors and debugging UIs:

**Components:**
- `InspectedObjectType` - Cached type metadata
- `TypeInteractionModel` - Interaction model for a type's members
- `InspectedObjectItem` - Represents an inspectable object instance
- `CustomInspector` - User-defined inspection strategies

**ViewModels:**
- `AsyncPropertyVM` - ViewModel for async-loaded properties
- `ObservableVM` - Wraps IObservable<T> for UI binding
- `AsyncEnumerableVM` - Wraps IAsyncEnumerable<T>
- `ObjectInspectionAdapter<T>` - Base adapter using ReactiveUI

**Features:**
- Type scanning with caching
- Member filtering (MemberScanOptions, TypeScanOptions)
- Custom inspection via attributes or registrations
- Async data support

### 4. Dependency Injection Utilities

**InjectionReflectionCache<T, TInput>**
- Caches reflection metadata for efficient ViewModel construction
- Determines whether model should be:
  - Passed as constructor parameter
  - Set via property injection
  - Set via field injection

### 5. DynamicData Extensions

**DynamicDataX** - Extension methods for DynamicData patterns
**AsyncSourceCache<TItem, TValue>** (commented/experimental) - Async-loaded reactive caches

### 6. Subscription Patterns

**SubscribeCommands** - Helper for subscribing to reactive commands and observables in a structured way

### 7. ReactiveUI Base Classes

**DisposableBaseViewModel** - Base class for ViewModels with proper disposal patterns

## Directory Structure

```
DependencyInjection/    - DI utilities (InjectionReflectionCache)
DynamicData_/           - DynamicData extensions and async caches
Hosting/                - AddMvvm() and ReactiveUI hosting setup
Inspection/             - Object inspection system
  CustomInspector/      - Custom inspection strategies
  TypeInfo/             - Type metadata and interaction models
  ViewModels/           - Inspector ViewModels (AsyncPropertyVM, etc.)
    Adapters/           - ObjectInspectionAdapter and built-in adapters
    AsyncData/          - Async property ViewModels
Mvvm/                   - Core MVVM utilities
  Async/Collections/    - Async collection ViewModels (placeholders)
ReactiveUI/             - ReactiveUI base classes
Reflection/             - Reflection utilities for MVVM
Subscribing/            - Subscription helpers
```

## Usage Patterns

### Creating ViewModels

**Via IViewModelProvider:**
```csharp
public class MyService
{
    private readonly IViewModelProvider vmProvider;

    public MyService(IViewModelProvider vmProvider)
    {
        this.vmProvider = vmProvider;
    }

    public void ShowModel(MyModel model)
    {
        var vm = vmProvider.Activate<MyViewModel, MyModel>(model);
        // Use vm...
    }
}
```

**With Constructor Injection:**
```csharp
public class MyViewModel : ReactiveObject, IViewModel<MyModel>
{
    public MyViewModel(MyModel model, ISomeService service)
    {
        Value = model;
        // service is injected from DI container
    }

    public MyModel Value { get; }
}
```

### Object Inspection

```csharp
// Setup
services.AddMvvm(useDefaults: true);

// Inspect an object
var inspector = serviceProvider.GetRequiredService<IObjectInspector>();
var inspection = inspector.Inspect(myObject);

// Access members
foreach (var member in inspection.Members)
{
    Console.WriteLine($"{member.Name}: {member.Value}");
}
```

### ReactiveUI Integration

```csharp
// ReactiveUI ViewModels work seamlessly with DI
public class MyViewModel : ReactiveObject
{
    [Reactive]  // From ReactiveUI.SourceGenerators
    private string? _name;

    public MyViewModel(IMyService service)
    {
        // Constructor injection works
    }
}
```

## Configuration

**ViewModelConfiguration** - Configure ViewModel scanning:
```csharp
services.Configure<ViewModelConfiguration>(c =>
{
    c.TypeScanOptions.AssemblyWhitelist = new[] { typeof(MyVM).Assembly };
});
```

**MemberScanOptions** - Control which members are inspected
**TypeScanOptions** - Control which types are scanned

## Design Philosophy

**Framework Integration:**
- Bridges multiple MVVM frameworks (ReactiveUI, CommunityToolkit.Mvvm)
- Leverages Microsoft.Extensions.* patterns
- Unified ViewModel provider abstraction

**Reactive-First:**
- All collections use DynamicData
- ViewModels typically derive from ReactiveObject
- Async operations via IObservable<T>

**Inspection-Oriented:**
- Rich runtime inspection capabilities
- Supports building generic property editors
- Custom inspection strategies via attributes/registrations

## Common Patterns

### ViewModel Registration

ViewModels are auto-discovered if they implement `IViewModel<TModel>` and are in scanned assemblies. Manual registration:

```csharp
services.AddTransient<MyViewModel>();
```

### Async Properties

Use `AsyncPropertyVM` for properties that load asynchronously:
```csharp
public AsyncPropertyVM<MyData> DataProperty { get; }
```

### Observable Collections

Use DynamicData's reactive collections:
```csharp
private SourceCache<Item, int> items = new(x => x.Id);
public IObservableCache<Item, int> Items => items;
```

## Testing

When testing ViewModels:
- Mock `IViewModelProvider` for ViewModel creation
- Use `TestScheduler` from ReactiveUI for time-based testing
- Register test services in DI container

## Performance Considerations

- Type scanning happens at startup (cached)
- InjectionReflectionCache uses static caching per type pair
- Object inspection metadata is cached per type

## Known Limitations

- Some async collection ViewModels are placeholders (see folder comments)
- AsyncSourceCache is experimental (commented out)
- Generic ViewModel support in ViewModelProvider is partial

## Related Projects

- **LionFire.Mvvm.Abstractions** - Interfaces implemented here
- **LionFire.Blazor.Mvvm** - Blazor-specific MVVM components
- **LionFire.Avalon** - WPF/Avalon MVVM support

## See Also

- ReactiveUI: https://www.reactiveui.net/
- CommunityToolkit.Mvvm: https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/
- DynamicData: https://github.com/reactivemarbles/DynamicData
- Microsoft.Extensions.Hosting patterns in main CLAUDE.md
