# LionFire.Core Documentation

Welcome to the LionFire.Core documentation! This repository contains a collection of mini-frameworks and toolkits for .NET development, organized in a layered architecture.

## Documentation Structure

Our documentation is organized in three tiers to serve different needs:

### 1. 📚 In-Project Reference (`src/*/CLAUDE.md`)

Each library contains a `CLAUDE.md` file with technical reference, API documentation, and quick-start examples.

**Completed Libraries:**
- [LionFire.Mvvm.Abstractions](../src/LionFire.Mvvm.Abstractions/CLAUDE.md)
- [LionFire.Mvvm](../src/LionFire.Mvvm/CLAUDE.md)
- [LionFire.Data.Async.Abstractions](../src/LionFire.Data.Async.Abstractions/CLAUDE.md)
- [LionFire.Data.Async.Reactive.Abstractions](../src/LionFire.Data.Async.Reactive.Abstractions/CLAUDE.md)
- [LionFire.Data.Async.Reactive](../src/LionFire.Data.Async.Reactive/CLAUDE.md)
- [LionFire.Data.Async.Mvvm](../src/LionFire.Data.Async.Mvvm/CLAUDE.md)
- [LionFire.Reactive](../src/LionFire.Reactive/CLAUDE.md)

### 2. 🎯 Domain Documentation (`docs/<domain>/`)

Cross-library guides within a domain, with how-tos, tutorials, and integration patterns.

**Available Domains:**

#### **[Async Data](data/async/)** ✅ Complete (3,882 lines)
Foundational data access patterns with async/await, caching, and reactive features.
- [Overview](data/async/README.md) - Quick start and core concepts
- [Getters and Setters](data/async/getters-setters.md) - IGetter, ISetter, IValue patterns
- [Observable Operations](data/async/observable-operations.md) - Reactive operation tracking
- [Collections](data/async/collections.md) - Async collection patterns
- [Persistence](data/async/persistence.md) - File and database persistence
- [Caching Strategies](data/async/caching-strategies.md) - Cache management

#### **[MVVM](mvvm/)** ✅ Complete (5,053 lines)
Model-View-ViewModel patterns with ReactiveUI integration.
- [Overview](mvvm/README.md) - Quick start and core concepts ⭐ START HERE
- [ViewModels Guide](mvvm/viewmodels-guide.md) - All ViewModel types explained
- [Reactive Patterns](mvvm/reactive-patterns.md) - WhenAnyValue, commands, observables
- [Data Binding](mvvm/data-binding.md) - UI binding patterns
- [Collections](mvvm/collections.md) - Collection ViewModels
- [Inspection](mvvm/inspection.md) - Object inspection and property grids

#### **[UI / Blazor](ui/)** ✅ Complete (2,769 lines)
Blazor component patterns and reactive UI development.
- [Overview](ui/README.md) - UI patterns quick reference ⭐ START HERE
- [Blazor MVVM Patterns](ui/blazor-mvvm-patterns.md) - When to use automatic vs manual
- [Component Catalog](ui/component-catalog.md) - All available components
- [Reactive UI Updates](ui/reactive-ui-updates.md) - Update flow and performance

#### **[Reactive Programming](reactive/)** 🆕
Observable patterns with DynamicData, on-demand activation, and lifecycle management.
- [Overview](reactive/README.md) - Reactive patterns quick reference ⭐ START HERE

#### **[VOS (Virtual Object System)](data/vos/)**
Virtual filesystem with mounting, overlays, and dependency injection.
- [VOS Overview](data/vos/vos-overview.md)
- [VOS Core Concepts](data/vos/vos-core-concepts.md)
- [VOS Architecture](data/vos/vos-architecture.md)
- [VOS Examples](data/vos/vos-examples.md)

### 3. 🏛️ Architecture Documentation (`docs/architecture/`)

High-level system design, layer relationships, and design philosophy.

**Available Architecture Docs:**

#### **[Core Architecture](architecture/)** ✅ Complete
- [Overview](architecture/README.md) - Repository architecture and design philosophy
- [Layers](architecture/layers.md) - Layer breakdown and dependency rules
- [Dependency Graph](architecture/dependency-graph.md) - Visual dependency diagrams

#### **[Async Data Architecture](architecture/data/async/)** ✅ Complete
- [Overview](architecture/data/async/README.md) - Async-first data access philosophy
- [Async Patterns](architecture/data/async/async-patterns.md) - Core async patterns
- [Reactive Data](architecture/data/async/reactive-data.md) - ReactiveUI integration
- [Persistence Integration](architecture/data/async/persistence-integration.md) - Storage backends

#### **[MVVM Architecture](architecture/mvvm/)** ✅ Complete
- [Overview](architecture/mvvm/README.md) - MVVM philosophy and integration
- [MVVM Layers](architecture/mvvm/mvvm-layers.md) - Layer architecture
- [Reactive MVVM](architecture/mvvm/reactive-mvvm.md) - ReactiveUI patterns
- [Data Binding](architecture/mvvm/data-binding.md) - Data access + MVVM integration
- [UI Frameworks](architecture/mvvm/ui-frameworks.md) - WPF, Blazor, cross-platform

#### **[Workspaces Architecture](architecture/workspaces/)** ✅ Complete
- [Overview](architecture/workspaces/README.md) - User-centric data containers ⭐
- [Service Scoping](architecture/workspaces/service-scoping.md) - DI architecture (CRITICAL)
- [Document Types](architecture/workspaces/document-types.md) - Document type system
- [Integration Diagram](architecture/workspaces/integration-diagram.md) - Complete flow diagrams

#### **[VOS Architecture](architecture/data/vos/)**
- [Analysis](architecture/data/vos/analysis/) - Architectural critique and recommendations

## Quick Navigation

### 🚀 New to LionFire? Start Here

**Building Blazor Apps?**
1. [UI Patterns Overview](ui/README.md) - Quick start for Blazor
2. [Blazor MVVM Patterns](ui/blazor-mvvm-patterns.md) - List vs detail views
3. [Workspace Architecture](architecture/workspaces/README.md) - Multi-workspace apps

**Working with Data?**
1. [Async Data Overview](data/async/README.md) - Data access quick start
2. [Getters and Setters](data/async/getters-setters.md) - Core patterns
3. [MVVM Overview](mvvm/README.md) - ViewModels and reactive binding

**Understanding Architecture?**
1. [Repository Architecture](architecture/README.md) - Overall design
2. [Layers Guide](architecture/layers.md) - Layer boundaries
3. [Dependency Graph](architecture/dependency-graph.md) - Visual dependencies

---

### 📖 Key Topics

#### MVVM & UI Development
- **[MVVM Domain Docs](mvvm/)** - Complete MVVM guide (5,053 lines)
  - [Quick Start](mvvm/README.md)
  - [ViewModels Guide](mvvm/viewmodels-guide.md) - All VM types
  - [Reactive Patterns](mvvm/reactive-patterns.md) - WhenAnyValue, commands
- **[UI Domain Docs](ui/)** - Blazor patterns (2,769 lines)
  - [UI Patterns](ui/README.md)
  - [Blazor MVVM Patterns](ui/blazor-mvvm-patterns.md) - Decision guide
  - [Component Catalog](ui/component-catalog.md) - ObservableDataView, etc.
- **[Blazor Components CLAUDE.md](../src/LionFire.Blazor.Components.MudBlazor/CLAUDE.md)** - Component reference

#### Async Data Access
- **[Async Data Domain Docs](data/async/)** - Complete data access guide (3,882 lines)
  - [Overview](data/async/README.md) - Quick start
  - [Getters and Setters](data/async/getters-setters.md) - Core patterns
  - [Observable Operations](data/async/observable-operations.md) - Reactive features
  - [Collections](data/async/collections.md) - Collection patterns
  - [Caching Strategies](data/async/caching-strategies.md) - Cache management
- **[Async Data Architecture](architecture/data/async/)** - Architecture deep dive
- **Project CLAUDE.md Files**:
  - [LionFire.Data.Async.Abstractions](../src/LionFire.Data.Async.Abstractions/CLAUDE.md)
  - [LionFire.Data.Async.Reactive](../src/LionFire.Data.Async.Reactive/CLAUDE.md)
  - [LionFire.Data.Async.Mvvm](../src/LionFire.Data.Async.Mvvm/CLAUDE.md)

#### Workspace System
- **[Workspace Architecture](architecture/workspaces/)** - Complete workspace guide (2,673 lines)
  - [Overview](architecture/workspaces/README.md)
  - [Service Scoping](architecture/workspaces/service-scoping.md) - CRITICAL for DI
  - [Document Types](architecture/workspaces/document-types.md)
  - [Integration Diagram](architecture/workspaces/integration-diagram.md) - Visual flows
- **Project CLAUDE.md Files**:
  - [LionFire.Workspaces](../src/LionFire.Workspaces/CLAUDE.md)
  - [LionFire.Workspaces.Abstractions](../src/LionFire.Workspaces.Abstractions/CLAUDE.md)
  - [LionFire.Workspaces.UI.Blazor](../src/LionFire.Workspaces.UI.Blazor/CLAUDE.md)

#### Reactive Programming
- **[Reactive Domain Docs](reactive/)** 🆕 - Complete reactive patterns guide (805 lines)
  - [Overview](reactive/README.md) - DynamicData, on-demand, runners
- **[Reactive Architecture](architecture/reactive/)** - Architecture deep dive
  - [Reactive Persistence](architecture/reactive/reactive-persistence.md) - IObservableReader/Writer
  - [Runner Pattern](architecture/reactive/runner-pattern.md) - Lifecycle management
- [Reactive Utilities](../src/LionFire.Reactive/CLAUDE.md) - API reference
- [Reactive UI Updates](ui/reactive-ui-updates.md) - UI update flow

#### Virtual Object System (VOS)
- [VOS Overview](data/vos/vos-overview.md)
- [VOS Core Concepts](data/vos/vos-core-concepts.md)
- [VOS Architecture](data/vos/vos-architecture.md)
- [VOS Examples](data/vos/vos-examples.md)

### 🚀 Getting Started Guides

**Complete Series** (6 guides):
- [01. Base Layer Utilities](guides/getting-started/01-base-layer.md) - Extension methods and common utilities
- [02. Async Data Access](guides/getting-started/02-async-data.md) - IGetter, IValue, and lazy loading
- [03. MVVM Basics](guides/getting-started/03-mvvm-basics.md) - ViewModels with ReactiveUI
- [04. Reactive Collections](guides/getting-started/04-reactive-collections.md) 🆕 - DynamicData and observable collections
- [05. File-Based Persistence](guides/getting-started/05-persistence.md) 🆕 - IObservableReader/Writer patterns
- [06. VOS Introduction](guides/getting-started/06-vos-introduction.md) 🆕 - Virtual Object System basics

### 🛠️ How-To Guides

**Available** (10 guides) ✅ **MILESTONE COMPLETE**:

**Data Access:**
- [Implement Custom Getter](guides/how-to/implement-custom-getter.md) - 5 approaches to fetching data
- [Implement Caching Strategies](guides/how-to/implement-caching.md) - 6 caching patterns

**MVVM & UI:**
- [Create Custom ViewModels](guides/how-to/create-custom-viewmodel.md) - 5 ViewModel patterns
- [Create Custom Commands](guides/how-to/create-custom-commands.md) - 6 command patterns
- [Implement Validation](guides/how-to/implement-validation.md) - 5 validation strategies
- [Bind Async Data to UI](guides/how-to/bind-async-data-to-ui.md) - 5 strategies for reactive binding
- [Handle Loading States](guides/how-to/handle-loading-states.md) - 6 loading state strategies
- [Implement Master-Detail Pattern](guides/how-to/implement-master-detail.md) - 4 layout patterns

**Collections & Reactive:**
- [Work with Observable Collections](guides/how-to/work-with-observable-collections.md) - 7 collection patterns

**Blazor:**
- [Create Blazor Workspace Page](guides/how-to/create-blazor-workspace-page.md) - Step-by-step list and detail pages

**Coming Soon:**
- Build property editors
- Integrate with dependency injection
- Create custom serializers

### 📋 Complete CLAUDE.md Index

**Completed Libraries** (11 files):
- [LionFire.Mvvm.Abstractions](../src/LionFire.Mvvm.Abstractions/CLAUDE.md)
- [LionFire.Mvvm](../src/LionFire.Mvvm/CLAUDE.md)
- [LionFire.Data.Async.Abstractions](../src/LionFire.Data.Async.Abstractions/CLAUDE.md)
- [LionFire.Data.Async.Reactive.Abstractions](../src/LionFire.Data.Async.Reactive.Abstractions/CLAUDE.md)
- [LionFire.Data.Async.Reactive](../src/LionFire.Data.Async.Reactive/CLAUDE.md)
- [LionFire.Data.Async.Mvvm](../src/LionFire.Data.Async.Mvvm/CLAUDE.md)
- [LionFire.Reactive](../src/LionFire.Reactive/CLAUDE.md)
- [LionFire.Workspaces](../src/LionFire.Workspaces/CLAUDE.md)
- [LionFire.Workspaces.Abstractions](../src/LionFire.Workspaces.Abstractions/CLAUDE.md)
- [LionFire.Workspaces.UI.Blazor](../src/LionFire.Workspaces.UI.Blazor/CLAUDE.md)
- [LionFire.Blazor.Components.MudBlazor](../src/LionFire.Blazor.Components.MudBlazor/CLAUDE.md)

## Repository Architecture

LionFire.Core follows a layered architecture philosophy:

### Layer 1: Base (Minimal Dependencies)
- **LionFire.Base** - BCL augmentation with no external dependencies
- **LionFire.Flex** - Strongly-typed dynamic object pattern
- **LionFire.Structures** - Data structures and collection types

### Layer 2: Toolkits (Unopinionated, A la carte)
- **Data Access** - Async patterns, persistence, VOS
- **MVVM** - ViewModels, commands, reactive patterns
- **Serialization** - Multiple serializer implementations
- **Hosting** - Application hosting extensions
- **Referencing** - URL/URI handling and custom schemas

### Layer 3: Frameworks (Opinionated)
- **LionFire.Core.Extras** - General-purpose framework utilities
- **LionFire.AspNetCore.Framework** - ASP.NET Core best practices
- **LionFire.Framework** - Complete integrated framework

## Key Concepts

### Async Data Access Layers

```
Abstractions → Reactive.Abstractions → Reactive → Mvvm
     ↓              ↓                      ↓          ↓
  IGetter      IGetterRxO            GetterRxO   GetterVM
  ISetter      ISetterRxO            SetterRxO   ValueVM
  IValue       IValueRxO             ValueRxO    ObservableReaderVM
```

**Abstractions** - Core interfaces for async operations
**Reactive.Abstractions** - ReactiveObject marker interfaces
**Reactive** - Concrete implementations with DynamicData
**Mvvm** - ViewModel wrappers with commands

### MVVM Architecture

- **Abstractions**: `IViewModel<T>`, `IViewModelProvider`, object inspection
- **Implementation**: ReactiveUI integration, ViewModel registry
- **ViewModels**: `GetterVM`, `ValueVM`, collection VMs, ObservableReader VMs

### Reactive Patterns

- **Observable Data**: `IObservableReader<TKey, TValue>`, `IObservableWriter<TKey, TValue>`
- **File System**: Reactive file watching and document collections
- **Lifecycle**: Runner pattern for configuration-driven start/stop
- **On-Demand**: Resource management with RefCount patterns

## Documentation Statistics

**Current Status** (November 2025):

| Domain | Files | Lines | Status |
|--------|-------|-------|--------|
| Async Data (Domain) | 6 | 3,882 | ✅ Complete |
| MVVM (Domain) | 6 | 5,053 | ✅ Complete |
| UI/Blazor (Domain) | 4 | 2,769 | ✅ Complete |
| Reactive (Domain) | 1 | 805 | 🚧 In Progress |
| Workspaces (Architecture) | 4 | 2,673 | ✅ Complete |
| MVVM (Architecture) | 5 | 2,500+ | ✅ Complete |
| Async Data (Architecture) | 4 | 2,000+ | ✅ Complete |
| Reactive (Architecture) | 3 | 1,000+ | ✅ Complete |
| Core (Architecture) | 3 | 1,500+ | ✅ Complete |
| **TOTAL** | **36+** | **22,000+** | **In Progress** |

**CLAUDE.md Files**: 11+ completed (out of 100+ libraries)

## Documentation Tasks

See [TASKS.md](TASKS.md) for the comprehensive documentation project plan, including:
- ✅ Core architecture documentation (complete)
- ✅ MVVM domain and architecture docs (complete)
- ✅ Async Data domain and architecture docs (complete)
- ✅ UI/Blazor domain docs (complete)
- ✅ Workspace architecture docs (complete)
- ✅ Reactive architecture docs (complete)
- 🚧 Reactive domain docs (README complete, optional deep-dive docs pending)
- 📋 Getting started guides (planned)
- 📋 How-to guides (planned)
- 📋 Completing CLAUDE.md coverage for all libraries (ongoing)

## Contributing

### Adding Documentation

When adding new libraries or features:

1. **Create CLAUDE.md** in the project root with:
   - Overview and purpose
   - Key concepts and patterns
   - Usage examples
   - Design philosophy

2. **Update Domain Docs** if introducing new patterns

3. **Update Architecture Docs** if changing layer relationships

### Documentation Standards

- Use clear, concise language
- Provide code examples for all concepts
- Link to related documentation
- Keep CLAUDE.md files focused on the library
- Use domain docs for cross-library integration
- Use architecture docs for design rationale

## Questions or Feedback?

- 📝 See [TASKS.md](TASKS.md) for documentation project status
- 🐛 Report documentation issues in the main repository issue tracker
- 💡 Suggest improvements or new guides

---

**Note**: This documentation is under active development. See [TASKS.md](TASKS.md) for current progress and upcoming additions.
