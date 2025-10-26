# Reactive Architecture

## Overview

LionFire's **Reactive Architecture** provides observable patterns for data access, file system watching, and lifecycle management. Built on **ReactiveUI** and **DynamicData**, this architecture enables building responsive applications that react to changes automatically.

**Key Philosophy**: Resources are activated on-demand, changes flow through observable streams, and lifecycle is managed reactively.

---

## Table of Contents

1. [Reactive Stack Overview](#reactive-stack-overview)
2. [Core Libraries](#core-libraries)
3. [Key Patterns](#key-patterns)
4. [Integration Points](#integration-points)
5. [Architecture Decisions](#architecture-decisions)

---

## Reactive Stack Overview

### The Reactive Layers

```
┌────────────────────────────────────────────────────────────┐
│                    Application Layer                        │
│  Blazor Components, ViewModels                             │
└─────────────────────┬──────────────────────────────────────┘
                      │ Subscribes to
                      ↓
┌────────────────────────────────────────────────────────────┐
│                  ViewModel Layer                            │
│  LionFire.Data.Async.Mvvm                                  │
│  - ObservableDataVM                                        │
│  - ObservableReaderWriterItemVM                           │
└─────────────────────┬──────────────────────────────────────┘
                      │ Uses
                      ↓
┌────────────────────────────────────────────────────────────┐
│               Reactive Persistence Layer                    │
│  LionFire.Reactive                                         │
│  - IObservableReader/Writer                                │
│  - HjsonFsDirectoryReaderRx                                │
│  - Runner pattern                                          │
└─────────────────────┬──────────────────────────────────────┘
                      │ Built on
                      ↓
┌────────────────────────────────────────────────────────────┐
│                  Reactive Foundation                        │
│  ReactiveUI + DynamicData                                  │
│  - IObservable<T>                                          │
│  - SourceCache<T, TKey>                                    │
│  - ReactiveObject                                          │
└────────────────────────────────────────────────────────────┘
```

---

## Core Libraries

### LionFire.Reactive

**Purpose**: Reactive utilities and patterns for LionFire.

**Key Components**:
- **IObservableReader/Writer** - Reactive persistence abstractions
- **ObservableFileInfos** - File system watching
- **Runner<TValue, TRunner>** - Lifecycle management pattern
- **On-Demand Patterns** - Resource activation on subscription

**Documentation**: [LionFire.Reactive/CLAUDE.md](../../../src/LionFire.Reactive/CLAUDE.md)

---

### ReactiveUI Integration

**Purpose**: Foundation reactive framework.

**Key Features**:
- `ReactiveObject` - INotifyPropertyChanged implementation
- `[Reactive]` attribute - Source-generated properties
- `WhenAnyValue` - Property change observables
- `ReactiveCommand` - Commands with reactive can-execute
- Observable extensions (Throttle, DistinctUntilChanged, etc.)

**Documentation**: https://www.reactiveui.net/

---

### DynamicData Integration

**Purpose**: Reactive collections framework.

**Key Features**:
- `SourceCache<TValue, TKey>` - Observable cache with change notifications
- `IChangeSet<TValue, TKey>` - Batched change notifications
- Transform, Filter, Sort operators
- Group, Merge, Join operators
- RefCount and on-demand connection patterns

**Documentation**: https://github.com/reactivemarbles/DynamicData

---

## Key Patterns

### 1. Reactive Persistence Pattern

```
File System
    ↓ FileSystemWatcher / Polling
IObservableReader<TKey, TValue>
    ↓ Values.Connect()
SourceCache<TValue, TKey>
    ↓ IChangeSet<TValue, TKey>
Subscribers (VMs, Components)
    ↓
UI Updates
```

**Flow**:
1. Files change on disk
2. File watcher detects changes
3. Files deserialized to entities
4. SourceCache updated
5. IChangeSet emitted
6. Subscribers notified
7. UI updates reactively

**See**: [Reactive Persistence](reactive-persistence.md)

---

### 2. On-Demand Activation Pattern

```
No Subscribers
    ↓ State: Inactive
First Subscriber Connects
    ↓ OnFirstSubscribe
Resources Activated (file watcher, polling, etc.)
    ↓ State: Active
Data Flows to Subscribers
    ↓
Last Subscriber Disconnects
    ↓ OnLastDispose
Resources Deactivated
    ↓ State: Inactive
```

**Benefits**:
- Resources only created when needed
- Automatic cleanup
- Optimal performance
- Memory efficient

**Implementation**: `CreateConnectOnDemand`, `PublishRefCountWithEvents`

---

### 3. Runner Pattern

```
Configuration Observable
    ↓ Subscribe
Runner<TConfig, TRunner>
    ↓ OnNext(config)
Check IsEnabled(config)
    ↓ If true
Start(config)
    ↓ Running
OnConfigurationChange(newConfig)  (if config changes)
    ↓ If disabled
Stop()
    ↓ Stopped
```

**Use Cases**:
- Start/stop bots based on config files
- Enable/disable services based on settings
- Hot-reload configurations
- Lifecycle-managed components

**See**: [Runner Pattern](runner-pattern.md)

---

## Integration Points

### With Workspaces

```
Workspace Document File
    ↓ File system watcher
IObservableReader<string, BotEntity>
    ↓ Values.Connect()
WorkspaceDocumentService<BotEntity>
    ↓ For each runner
IWorkspaceDocumentRunner<string, BotEntity>
    ↓ IObserver<BotEntity>.OnNext()
BotRunner starts/stops bot
```

**Integration**: Workspace documents trigger runners via reactive patterns.

---

### With MVVM

```
IObservableReader<TKey, TValue>
    ↓ Wrapped by
ObservableReaderItemVM<TKey, TValue, TVM>
    ↓ Used by
Blazor Component
    ↓ Subscribes to
vm.WhenAnyValue(x => x.Value)
    ↓ Triggers
StateHasChanged()
```

**Integration**: Observable data sources wrapped in ViewModels for UI binding.

---

### With Async Data

```
IGetter<T> / IValue<T>
    ↓ Reactive Implementation
GetterRxO<T> / ValueRxO<T>
    ↓ Provides
IObservableGetOperations
IObservableGetResults
    ↓ Consumed by
ViewModels and UI
```

**Integration**: Async data operations exposed as observables.

---

## Architecture Decisions

### ADR 1: Why ReactiveUI?

**Context**: Need reactive MVVM framework.

**Decision**: Use ReactiveUI as foundation.

**Rationale**:
- Mature, well-tested library
- Excellent `WhenAnyValue` and observable property support
- `[Reactive]` source generator reduces boilerplate
- Cross-platform (WPF, Blazor, Avalonia, etc.)
- Strong community and ecosystem

**Consequence**: Slight learning curve, but massive productivity gains.

---

### ADR 2: Why DynamicData?

**Context**: Need reactive collections with change tracking.

**Decision**: Use DynamicData for all observable collections.

**Rationale**:
- Efficient change notifications (batched changesets)
- Rich transformation operators
- Excellent performance
- Integrates with ReactiveUI
- On-demand patterns built-in

**Consequence**: Learning curve for DynamicData API, but powerful capabilities.

---

### ADR 3: Why On-Demand Activation?

**Context**: File watchers and polling are expensive.

**Decision**: Activate resources only when subscribed to.

**Rationale**:
- Don't watch files nobody is viewing
- Don't poll APIs nobody is using
- Automatic cleanup when UI closes
- Optimal performance

**Consequence**: More complex observable setup, but significant performance gains.

---

## Related Documentation

### Architecture Deep Dives
- **[Reactive Persistence](reactive-persistence.md)** - IObservableReader/Writer architecture
- **[Runner Pattern](runner-pattern.md)** - Lifecycle management

### Domain Documentation
- **[Reactive Patterns](../../mvvm/reactive-patterns.md)** - Reactive programming in MVVM
- **[Reactive UI Updates](../../ui/reactive-ui-updates.md)** - UI update flow
- **[Observable Operations](../../data/async/observable-operations.md)** - Observable async operations

### Project Documentation
- **[LionFire.Reactive](../../../src/LionFire.Reactive/CLAUDE.md)** - Complete API reference
- **[LionFire.Data.Async.Reactive](../../../src/LionFire.Data.Async.Reactive/CLAUDE.md)** - ReactiveUI implementations

---

## Summary

**Reactive Architecture** provides:

**Core Patterns**:
- **Reactive Persistence** - IObservableReader/Writer for file-backed data
- **On-Demand Activation** - Resources created only when subscribed
- **Runner Pattern** - Configuration-driven lifecycle management
- **File System Watching** - Reactive file monitoring

**Key Benefits**:
- Automatic UI updates when data changes
- Optimal performance (resources only when needed)
- Clean lifecycle management
- Reactive composition

**Foundation**: Built on ReactiveUI and DynamicData for battle-tested reactive primitives.

**Integration**: Seamlessly integrates with Workspaces (file watching), MVVM (ViewModels), and Async Data (observable operations).
