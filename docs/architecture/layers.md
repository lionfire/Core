# LionFire.Core Layer Architecture

**Purpose**: Detailed breakdown of the layered architecture, dependency rules, and guidelines for adding new components.

---

## Table of Contents

1. [Layer Overview](#layer-overview)
2. [Layer 1: Base](#layer-1-base)
3. [Layer 2: Toolkits](#layer-2-toolkits)
4. [Layer 3: Frameworks](#layer-3-frameworks)
5. [Dependency Rules](#dependency-rules)
6. [Adding New Components](#adding-new-components)
7. [Layer-Specific Patterns](#layer-specific-patterns)

---

## Layer Overview

### The Three-Layer Model

```
┌────────────────────────────────────────────────────────────┐
│                    Layer 3: Frameworks                     │
│                  (Opinionated Integration)                 │
│                                                             │
│  Purpose: Bring toolkits together for rapid development   │
│  Dependencies: Multiple toolkits, external packages        │
│  Examples: LionFire.Framework, AspNetCore.Framework        │
└────────────────────────────────────────────────────────────┘
                            ↑
                   depends on
                            ↑
┌────────────────────────────────────────────────────────────┐
│                     Layer 2: Toolkits                      │
│                (Unopinionated, A la carte)                 │
│                                                             │
│  Purpose: Provide specific functionality domains           │
│  Dependencies: May depend on other toolkits and Base       │
│  Examples: MVVM, Persistence, VOS, Serialization           │
└────────────────────────────────────────────────────────────┘
                            ↑
                   depends on
                            ↑
┌────────────────────────────────────────────────────────────┐
│                      Layer 1: Base                         │
│                (Minimal Dependencies, BCL+)                │
│                                                             │
│  Purpose: Augment .NET BCL with foundational utilities     │
│  Dependencies: Only .NET BCL (no external packages)        │
│  Examples: LionFire.Base, LionFire.Flex                    │
└────────────────────────────────────────────────────────────┘
```

### Layer Characteristics

| Layer | Dependencies | Opinions | Stability | Use Case |
|-------|-------------|----------|-----------|----------|
| **Base** | BCL only | None | Stable | Universal utilities |
| **Toolkits** | Base + other toolkits | Minimal | Evolving | Domain-specific features |
| **Frameworks** | Multiple toolkits | Strong | Flexible | Rapid app development |

---

## Layer 1: Base

### Purpose

**Augment the .NET BCL with minimal dependencies**

The Base layer provides foundational utilities that can be used anywhere:
- Extension methods for common types (collections, strings, dates)
- Core data structures
- Patterns that don't require external dependencies
- Strongly-typed dynamic object pattern

### Key Principle

**Zero External Dependencies**: Base layer libraries should only depend on:
- .NET BCL (System.*, Microsoft.Extensions.* that ship with .NET)
- Other Base layer libraries
- NO external NuGet packages

### Projects in Base Layer

#### LionFire.Base
**Path**: `src/LionFire.Base/`
**Purpose**: Core BCL augmentation
**Dependencies**: None (BCL only)

**Contents**:
- **Collections**: Extension methods for `IEnumerable<T>`, `List<T>`, `Dictionary<TKey, TValue>`
  - `ForEach()`, `AddRange()`, `TryAdd()`, `OrEmpty()`, etc.
- **DateTime**: Date/time utilities, parsing, formatting
- **Reflection**: Type inspection, attribute helpers
- **Strings**: String manipulation, parsing, formatting
- **Comparison**: Equality comparers, comparison utilities
- **Conversion**: Type conversion helpers
- **Validation**: Basic validation utilities

**Example**:
```csharp
// Collection extensions
var items = GetItems().OrEmpty();  // Never returns null
items.ForEach(item => Process(item));

// String extensions
var value = input.TrimOrNull();  // Trims and returns null if empty
```

#### LionFire.Flex
**Path**: `src/LionFire.Flex/`
**Purpose**: Strongly-typed dynamic object pattern
**Dependencies**: LionFire.Base

**Core Pattern**:
```csharp
public interface IFlex
{
    object? FlexData { get; set; }
}

// Usage: Add any strongly-typed data to an object at runtime
public class MyEntity : IFlex
{
    public string Name { get; set; }
    public object? FlexData { get; set; }  // Can hold any type
}

var entity = new MyEntity();
entity.As<ISerializable>()  // Retrieve as different interface
```

**Use Cases**:
- Runtime type extension without inheritance
- Multi-typing pattern (object implements multiple unrelated interfaces)
- Adding capabilities to objects dynamically

#### LionFire.Structures
**Path**: `src/LionFire.Structures/`
**Purpose**: Data structures and collection types
**Dependencies**: LionFire.Base, LionFire.Flex

**Contents**:
- Advanced collection types
- Tree structures
- Graph structures
- Specialized data structures

#### LionFire.Binding
**Path**: `src/LionFire.Binding/`
**Purpose**: Data binding from one object to another
**Dependencies**: LionFire.Base

**Use Cases**:
- Synchronize properties between objects
- One-way or two-way binding
- Independent of UI frameworks

#### LionFire.Behaviors
**Path**: `src/LionFire.Behaviors/`
**Purpose**: Composable behaviors for workflows or AI
**Dependencies**: LionFire.Base

### What Belongs in Base Layer?

✅ **Include**:
- Extension methods for BCL types
- Pure utility classes with no external dependencies
- Core patterns used across multiple toolkits
- Data structures with universal applicability

❌ **Exclude**:
- Anything requiring external NuGet packages
- Domain-specific logic (belongs in Toolkits)
- Opinionated frameworks
- UI-specific code
- Persistence or I/O operations

### Base Layer Patterns

**Extension Methods**:
```csharp
public static class CollectionExtensions
{
    public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? source)
        => source ?? Enumerable.Empty<T>();
}
```

**Utility Classes**:
```csharp
public static class DateTimeUtils
{
    public static DateTime? TryParse(string? input) { /* ... */ }
}
```

**Minimal Interfaces**:
```csharp
public interface IHas<out T>
{
    T Value { get; }
}
```

---

## Layer 2: Toolkits

### Purpose

**Provide specific functionality domains with minimal opinions**

Toolkits offer specialized capabilities that can be used independently or composed together:
- Each toolkit focuses on a specific domain (MVVM, Persistence, Serialization)
- Unopinionated: provide mechanisms, not policies
- Can depend on other toolkits (but should minimize coupling)
- Open for extension

### Key Principle

**A La Carte**: Toolkits should be usable independently. Consumers can pick and choose which toolkits they need.

### Toolkit Categories

#### 1. Data Toolkits

**Serialization**
- **LionFire.Serialization** - Core abstractions
- **LionFire.Serialization.Json.Newtonsoft** - JSON.NET implementation
- **LionFire.Serialization.Json.System** - System.Text.Json implementation
- **LionFire.Serialization.Hjson** - HJSON format
- **LionFire.Serialization.YamlDotNet** - YAML format

**Pattern**: Pluggable serializers with format detection

**Referencing**
- **LionFire.Referencing.Abstractions** - Reference abstractions
- **LionFire.Referencing** - URL/URI handling, custom schemas

**Pattern**: URLs for object addressing

**Resolves (Data Access Abstractions)**
- **LionFire.Resolves.Abstractions** - Core interfaces (synchronous)
- **LionFire.Data.Async.Abstractions** - Async data access (`IGetter<T>`, `ISetter<T>`, `IValue<T>`)
- **LionFire.Data.Async.Reactive.Abstractions** - Reactive marker interfaces
- **LionFire.Data.Async.Reactive** - ReactiveUI implementations
- **LionFire.Reactive** - Observable persistence, file watching

**Pattern**: Layered async data access (Abstractions → Reactive Abstractions → Reactive → MVVM)

**Persistence**
- **LionFire.Persistence.Abstractions** - Core interfaces
- **LionFire.Persistence** - Base implementation
- **LionFire.Persistence.Filesystem** - File system backend
- **LionFire.Persistence.Redis**, **LionFire.Persistence.LiteDB**, etc. - Various backends

**Pattern**: Pluggable persistence providers

**Handles**
- **LionFire.Persistence.Handles.Abstractions** - Handle abstractions
- **LionFire.Persistence.Handles** - Object handles with reference support

**Pattern**: Handles wrap references and provide read/write capabilities

**VOS (Virtual Object System)**
- **LionFire.Vos.Abstractions** - Core VOS interfaces
- **LionFire.Vos** - VOS implementation
- **LionFire.Vos.Framework** - VOS application framework
- **LionFire.Vos.Overlays** - Overlay mounts

**Pattern**: Virtual filesystem with mounts, overlays, hierarchical DI

**Assets/Instantiating**
- **LionFire.Assets.Abstractions** - Entity/asset abstractions
- **LionFire.Assets** - Primary key-based entity access
- **LionFire.Instantiating.Abstractions** - Template abstractions
- **LionFire.Instantiating** - Template-based object instantiation

**Pattern**: Simplified data access via primary keys

#### 2. MVVM Toolkits

**Core MVVM**
- **LionFire.Mvvm.Abstractions** - `IViewModel<T>`, `IViewModelProvider`
- **LionFire.Mvvm** - ReactiveUI integration, ViewModel provider

**Pattern**: Reactive ViewModels wrapping models

**Data Access ViewModels**
- **LionFire.Data.Async.Mvvm** - ViewModels for async data (`GetterVM`, `ValueVM`, `ObservableReaderVM`)

**Pattern**: ViewModels wrap async data access patterns

#### 3. UI Toolkits

**Blazor**
- **LionFire.Blazor.Components** - Base Blazor components
- **LionFire.Blazor.Components.MudBlazor** - MudBlazor reactive components
- **LionFire.Blazor.Components.UI** - UI utilities

**WPF/Avalonia**
- **LionFire.Avalon** - WPF/Avalonia support
- **LionFire.UI.Wpf** - WPF utilities

#### 4. Workspaces Toolkits

**Core Workspaces**
- **LionFire.Workspaces.Abstractions** - Workspace abstractions
- **LionFire.Workspaces** - Workspace implementation, DI

**UI Integration**
- **LionFire.Workspaces.UI** - Base UI abstractions
- **LionFire.Workspaces.UI.Blazor** - Blazor workspace components

**Pattern**: Workspace-scoped services for isolation

#### 5. Hosting Toolkits

**Application Hosting**
- **LionFire.Hosting.Abstractions** - Hosting abstractions
- **LionFire.Hosting** - Extensions to Microsoft.Extensions.Hosting
- **LionFire.Hosting.CommandLine** - Command-line application support

**Pattern**: Wrapper around `IHostBuilder` and `HostApplicationBuilder`

**Configuration**
- **LionFire.Extensions.Configuration** - Configuration extensions
- **LionFire.Extensions.DependencyInjection** - DI utilities
- **LionFire.Extensions.Logging** - Logging extensions

#### 6. Reactive Toolkits

**Reactive Patterns**
- **LionFire.Reactive** - File watching, observable persistence, lifecycle
- **LionFire.Reactive.Framework** - Framework integration

**Pattern**: ReactiveUI + DynamicData integration

### What Belongs in Toolkit Layer?

✅ **Include**:
- Domain-specific functionality (MVVM, Persistence, etc.)
- Pluggable/extensible systems
- Integration with external libraries (ReactiveUI, etc.)
- Abstractions + implementations (separate assemblies)

❌ **Exclude**:
- Universal utilities (belongs in Base)
- Opinionated integration of multiple toolkits (belongs in Framework)
- Application-specific code

### Toolkit Dependency Rules

**Allowed Dependencies**:
- ✅ Base layer libraries
- ✅ Other toolkit abstractions
- ✅ External NuGet packages (with restraint)
- ⚠️ Other toolkit implementations (use sparingly, prefer abstractions)

**Discouraged Dependencies**:
- ❌ Framework layer libraries (circular dependency)
- ❌ Tight coupling to specific implementations

**Example: Good Dependency Structure**
```
LionFire.Data.Async.Mvvm
  ├─> LionFire.Mvvm.Abstractions (toolkit abstraction)
  ├─> LionFire.Data.Async.Reactive.Abstractions (toolkit abstraction)
  └─> LionFire.Base (base layer)
```

### Toolkit Patterns

**Abstractions + Implementation**:
```
LionFire.Mvvm.Abstractions/
  ├─ IViewModel<T>
  └─ IViewModelProvider

LionFire.Mvvm/
  ├─ ViewModelProvider : IViewModelProvider
  └─ MvvmServiceExtensions
```

**Hosting Extensions**:
```csharp
public static class MvvmServiceExtensions
{
    public static IServiceCollection AddMvvm(this IServiceCollection services, ...)
    {
        services.AddSingleton<IViewModelProvider, ViewModelProvider>();
        // ...
        return services;
    }
}
```

**Options Pattern**:
```csharp
public class MvvmOptions
{
    public bool AutoScanViewModels { get; set; } = true;
}

services.AddMvvm(options => {
    options.AutoScanViewModels = false;
});
```

---

## Layer 3: Frameworks

### Purpose

**Opinionated integration of multiple toolkits for rapid application development**

Frameworks bring toolkits together with sensible defaults:
- Compose multiple toolkits
- Provide opinionated configuration
- Handle common integration scenarios
- Enable "one-line" application setup

### Key Principle

**Convention over Configuration**: Frameworks make decisions for you, with escape hatches for customization.

### Projects in Framework Layer

#### LionFire.Core.Extras
**Path**: `src/LionFire.Core.Extras/`
**Purpose**: General framework utilities
**Dependencies**: LionFire.Core + multiple toolkit abstractions

**Contents**:
- Multi-typing implementation
- Common framework patterns
- Utilities used across framework components

**Note**: May be renamed to `LionFire.Framework.Minimal`

#### LionFire.AspNetCore.Framework
**Path**: `src/LionFire.AspNetCore.Framework/`
**Purpose**: Opinionated ASP.NET Core setup
**Dependencies**: Multiple ASP.NET Core toolkits

**Pattern**: Best-practice ASP.NET Core application structure

#### LionFire.Vos.VosApp (or LionFire.Vos.App.Framework)
**Path**: `src/LionFire.Vos.Application/`
**Purpose**: VOS application framework
**Dependencies**: VOS toolkit + related toolkits

**Pattern**: Opinionated default capabilities for VOS applications (configuration, persistence, etc.)

#### LionFire.Framework
**Path**: `src/LionFire.Framework/`
**Purpose**: Complete integration of all toolkits
**Dependencies**: All major toolkits

**Vision**:
```csharp
// One line to get everything
new HostApplicationBuilder().LionFire();
```

### What Belongs in Framework Layer?

✅ **Include**:
- Integration code combining multiple toolkits
- Opinionated defaults and configuration
- "Batteries included" functionality
- Pre-configured application templates

❌ **Exclude**:
- New core functionality (belongs in Toolkits)
- Universal utilities (belongs in Base)
- Single-toolkit functionality

### Framework Dependency Rules

**Allowed Dependencies**:
- ✅ Multiple toolkit implementations
- ✅ External packages for integration
- ✅ Configuration and setup code

**Pattern**: Frameworks pull everything together

---

## Dependency Rules

### Strict Layer Hierarchy

```
Framework → Toolkit → Base
   (3)        (2)      (1)
```

**Rule 1: Higher layers can depend on lower layers**
- Framework can depend on Toolkits and Base
- Toolkits can depend on Base and other Toolkits (carefully)
- Base can only depend on BCL

**Rule 2: Lower layers CANNOT depend on higher layers**
- Base cannot depend on Toolkits or Framework
- Toolkits cannot depend on Framework

**Rule 3: Minimize inter-toolkit dependencies**
- Prefer depending on abstractions over implementations
- Avoid circular dependencies between toolkits
- Document when tight coupling is necessary

### Dependency Examples

**✅ Good**:
```
LionFire.Data.Async.Mvvm (Toolkit)
  → LionFire.Mvvm.Abstractions (Toolkit Abstraction)
  → LionFire.Data.Async.Reactive.Abstractions (Toolkit Abstraction)
  → LionFire.Base (Base)
```

**⚠️ Acceptable** (with caution):
```
LionFire.Vos (Toolkit)
  → LionFire.Persistence (Toolkit Implementation)
  → LionFire.Persistence.Handles (Toolkit Implementation)
  → LionFire.Referencing (Toolkit Implementation)
```
*Note: VOS requires tight integration with these toolkits*

**❌ Bad**:
```
LionFire.Base (Base)
  → LionFire.Mvvm (Toolkit)  // Base cannot depend on Toolkit!
```

**❌ Bad**:
```
LionFire.Mvvm (Toolkit)
  → LionFire.Framework (Framework)  // Circular dependency!
```

### Abstractions vs Implementations

**Pattern**: Separate abstractions from implementations

```
LionFire.<Component>.Abstractions/
  ├─ Interfaces
  └─ Core types

LionFire.<Component>/
  ├─ Implementations
  └─ Depends on <Component>.Abstractions
```

**Benefits**:
- Consumers can depend on abstractions without pulling in implementations
- Easier testing (mock abstractions)
- Multiple implementations possible

**Example**:
```
LionFire.Serialization.Abstractions
  → ISerializer, ISerializationProvider

LionFire.Serialization.Json.Newtonsoft
  → JsonNetSerializer : ISerializer
  → Depends on LionFire.Serialization.Abstractions
  → Depends on Newtonsoft.Json
```

---

## Adding New Components

### Where Does My Component Belong?

Use this decision tree:

```
Does it only use BCL types with no external packages?
│
├─ Yes → Base Layer
│   └─ Is it universally useful across domains?
│       ├─ Yes → LionFire.Base (or new Base project)
│       └─ No → Reconsider (might be Toolkit)
│
└─ No → Does it integrate multiple toolkits with opinions?
    │
    ├─ Yes → Framework Layer
    │   └─ LionFire.Framework or specialized framework
    │
    └─ No → Toolkit Layer
        └─ Create new toolkit or add to existing
```

### Guidelines for New Projects

#### Adding to Base Layer

**Checklist**:
- [ ] Zero external NuGet dependencies
- [ ] Universal utility (not domain-specific)
- [ ] No I/O operations (pure utility)
- [ ] Stable interface (won't change frequently)

**Naming**: `LionFire.<Utility>`
**Example**: `LionFire.Text`, `LionFire.Collections`

#### Adding to Toolkit Layer

**Checklist**:
- [ ] Focused on specific domain (MVVM, Persistence, etc.)
- [ ] Unopinionated (provides mechanisms, not policies)
- [ ] Can be used independently
- [ ] Minimal dependencies on other toolkit implementations

**Naming**: `LionFire.<Domain>[.<SubDomain>]`
**Example**: `LionFire.Persistence.MongoDB`

**Structure**:
```
LionFire.<Domain>.Abstractions/  (if abstractions needed)
LionFire.<Domain>/               (implementation)
```

#### Adding to Framework Layer

**Checklist**:
- [ ] Integrates multiple toolkits
- [ ] Provides opinionated defaults
- [ ] Targets specific application scenario
- [ ] Reduces boilerplate for common use cases

**Naming**: `LionFire.<Domain>.Framework`
**Example**: `LionFire.Blazor.Framework`

### Naming Conventions

**Projects**:
- `LionFire.<Component>` - Main implementation
- `LionFire.<Component>.Abstractions` - Interfaces and abstractions
- `LionFire.<Component>.<SubComponent>` - Sub-component
- `LionFire.<Component>.Tests` - Tests

**Namespaces**:
- Match project names
- Example: `namespace LionFire.Mvvm.Abstractions`

**Assemblies**:
- Match project names
- Example: `LionFire.Mvvm.Abstractions.dll`

---

## Layer-Specific Patterns

### Base Layer Patterns

**Extension Methods**:
```csharp
namespace LionFire.Extensions;

public static class StringExtensions
{
    public static string? TrimOrNull(this string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
```

**Null-Safe Collections**:
```csharp
public static class EnumerableExtensions
{
    public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? source)
        => source ?? Enumerable.Empty<T>();
}
```

**Minimal Interfaces**:
```csharp
public interface IReadWrapper<out T>
{
    T Value { get; }
}

public interface IWriteWrapper<in T>
{
    T Value { set; }
}
```

### Toolkit Layer Patterns

**Abstractions + Implementation**:
```csharp
// Abstractions assembly
public interface IViewModel<out TModel> : IReadWrapper<TModel> { }

// Implementation assembly
public class ViewModelProvider : IViewModelProvider
{
    private readonly IServiceProvider serviceProvider;
    // ...
}
```

**Hosting Extensions**:
```csharp
public static class PersistenceServiceExtensions
{
    public static IServiceCollection AddLionFirePersistence(
        this IServiceCollection services,
        Action<PersistenceOptions>? configure = null)
    {
        services.AddSingleton<IPersistenceProvider, PersistenceProvider>();
        if (configure != null)
        {
            services.Configure(configure);
        }
        return services;
    }
}
```

**Options Pattern**:
```csharp
public class PersistenceOptions
{
    public string DefaultProvider { get; set; } = "Filesystem";
    public bool EnableCaching { get; set; } = true;
}
```

**Plugin/Provider Pattern**:
```csharp
public interface ISerializationProvider
{
    ISerializer GetSerializer(string format);
    void RegisterSerializer(string format, ISerializer serializer);
}
```

### Framework Layer Patterns

**Fluent Configuration**:
```csharp
public static IServiceCollection AddLionFire(
    this IServiceCollection services,
    Action<LionFireOptions>? configure = null)
{
    services.AddLionFireBase();
    services.AddMvvm();
    services.AddLionFirePersistence();
    services.AddVos();
    // ... all the things

    if (configure != null)
    {
        services.Configure(configure);
    }

    return services;
}
```

**Convention-Based Setup**:
```csharp
// Framework makes decisions
services.AddLionFire(options =>
{
    options.UseDefaultConfiguration = true;  // Opinionated defaults
    options.EnableAutoDiscovery = true;      // Convention over configuration
});
```

---

## Summary

### Layer Quick Reference

| Layer | Purpose | Dependencies | Example Projects |
|-------|---------|--------------|------------------|
| **Base** | Universal utilities | BCL only | LionFire.Base, LionFire.Flex |
| **Toolkits** | Domain functionality | Base + other toolkits | LionFire.Mvvm, LionFire.Persistence |
| **Frameworks** | Integrated applications | Multiple toolkits | LionFire.Framework |

### Key Principles

1. **Base**: Zero external dependencies, universal utilities
2. **Toolkits**: Unopinionated, composable, domain-specific
3. **Frameworks**: Opinionated, integrated, rapid development

4. **Dependencies**: Higher → Lower only (never circular)
5. **Abstractions**: Separate from implementations
6. **Naming**: Consistent conventions across repository

### Decision Guidelines

**Adding a new component?**
- No external dependencies + universal → **Base**
- Domain-specific + unopinionated → **Toolkit**
- Multiple toolkits + opinionated → **Framework**

**Adding a dependency?**
- Can only depend on lower layers
- Prefer abstractions over implementations
- Minimize inter-toolkit coupling

---

**Related Documentation**:
- [Architecture Overview](README.md)
- [Dependency Graph](dependency-graph.md)
- [Main Documentation](../README.md)
