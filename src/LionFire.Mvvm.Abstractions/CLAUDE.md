# LionFire.Mvvm.Abstractions

## Overview

Core abstractions for the LionFire MVVM (Model-View-ViewModel) framework. This library defines interfaces and base types for reactive MVVM patterns without providing concrete implementations.

**Layer**: Base/Toolkit
**Dependencies**: Minimal - DynamicData, ReactiveUI (abstractions only)

## Key Concepts

### ViewModel Pattern

The central abstraction is `IViewModel<TModel>` which wraps a model object:

```csharp
public interface IViewModel<out TModel> : IReadWrapper<TModel>
{
    // TModel Value { get; }
}
```

- ViewModels wrap domain models for presentation logic
- Implements `IReadWrapper<TModel>` for uniform access patterns
- Type-safe relationship between ViewModel and Model
- Extension methods for finding ViewModels by their Model reference

### ViewModel Provider

`IViewModelProvider` enables dynamic ViewModel creation:

```csharp
public interface IViewModelProvider
{
    bool CanTransform<TModel, TViewModel>();
    TViewModel? TryActivate<TViewModel, TModel>(TModel model, ...);
    object? TryActivate<TModel>(TModel model, ...);
}
```

- Factory pattern for creating ViewModels from Models
- Supports constructor parameter injection
- Optional initializer callbacks
- Type checking with `CanTransform<,>()`

## Object Inspection

Abstractions for runtime object inspection and editing:

### Interfaces

- `IInspectorNode` - Base node in inspection hierarchy
- `IInspectorMember` - Inspectable member (property/field)
- `IInspectorMemberVM` - ViewModel for a member
- `IInspectorObjectVM` - ViewModel for entire object inspection

Used for building property editors, object browsers, and debugging tools.

## Reactive Notifications

Interfaces for deep change tracking:

- `INotifiesChildChanged` - Notifies when direct child properties change
- `INotifiesChildDeeplyChanged` - Notifies when any descendant changes

Enables reactive UI updates for complex object graphs.

## Reflection Utilities

**ActivationParameters** - Helper for managing constructor parameters during ViewModel activation:
- Tracks required types for dependency injection
- Used by ViewModelProvider implementations

## Directory Structure

```
Mvvm/
  ObjectInspection/     - Object inspection interfaces
  Services/             - IViewModelProvider
  ViewModels/           - IViewModel<T>, ViewModel<T> base class
MOVE-Rx/                - Reactive notification interfaces
Reflection/             - ActivationParameters utilities
```

## Usage Pattern

This library is typically used indirectly through LionFire.Mvvm which provides implementations. However, you can reference this library directly if:

- Building custom ViewModel frameworks
- Creating ViewModel providers for specific domains
- Implementing alternative MVVM patterns
- Need abstractions without implementation dependencies

## Design Philosophy

**Unopinionated Abstractions:**
- Minimal interface contracts
- No framework lock-in
- Composable with other MVVM frameworks
- Focuses on the ViewModel-Model relationship

**Reactive-First:**
- Built with ReactiveUI patterns in mind
- Supports observable property graphs
- Change notification at multiple levels

## Related Projects

- **LionFire.Mvvm** - Concrete implementations of these abstractions
- **LionFire.Data.Async.Abstractions** - Async data patterns used by ViewModels
- **LionFire.Structures** - Structural types (e.g., `IReadWrapper<T>`)
- **LionFire.Metadata** - Metadata support for inspection

## See Also

- LionFire.Mvvm for implementation
- ReactiveUI documentation for reactive patterns
- DynamicData for reactive collections
