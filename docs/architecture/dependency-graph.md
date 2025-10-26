# LionFire.Core Dependency Graph

**Purpose**: Visual dependency diagrams, project dependency matrix, and guidelines for managing dependencies across the repository.

---

## Table of Contents

1. [Layer Dependencies](#layer-dependencies)
2. [Domain Dependencies](#domain-dependencies)
3. [Complete Async Data Stack](#complete-async-data-stack)
4. [MVVM Stack](#mvvm-stack)
5. [Workspaces Integration](#workspaces-integration)
6. [VOS Dependencies](#vos-dependencies)
7. [Dependency Matrix](#dependency-matrix)
8. [Managing Dependencies](#managing-dependencies)

---

## Layer Dependencies

### High-Level Layer Diagram

```mermaid
graph TB
    subgraph Framework["Layer 3: Framework"]
        LF[LionFire.Framework]
        ACF[AspNetCore.Framework]
        VAF[Vos.App.Framework]
        FE[Core.Extras]
    end

    subgraph Toolkits["Layer 2: Toolkits"]
        subgraph MVVM["MVVM Domain"]
            MA[Mvvm.Abstractions]
            M[Mvvm]
            DAM[Data.Async.Mvvm]
        end

        subgraph Data["Data Domain"]
            DA[Data.Async.Abstractions]
            DRA[Data.Async.Reactive.Abs]
            DR[Data.Async.Reactive]
            R[Reactive]
        end

        subgraph Persistence["Persistence Domain"]
            PA[Persistence.Abstractions]
            P[Persistence]
            PFS[Persistence.Filesystem]
            PR[Persistence.Redis]
        end

        subgraph VOS["VOS Domain"]
            VA[Vos.Abstractions]
            V[Vos]
            VF[Vos.Framework]
        end

        subgraph Workspaces["Workspaces Domain"]
            WA[Workspaces.Abstractions]
            W[Workspaces]
        end

        subgraph Serialization["Serialization"]
            S[Serialization]
            SJ[Serialization.Json]
        end

        subgraph Hosting["Hosting"]
            HA[Hosting.Abstractions]
            H[Hosting]
        end
    end

    subgraph Base["Layer 1: Base"]
        B[LionFire.Base]
        F[LionFire.Flex]
        ST[LionFire.Structures]
    end

    %% Framework to Toolkit dependencies
    LF --> M
    LF --> P
    LF --> V
    ACF --> M
    VAF --> V
    FE --> MA

    %% MVVM dependencies
    M --> MA
    MA --> B
    DAM --> M
    DAM --> DR
    DAM --> DRA

    %% Data dependencies
    DR --> DRA
    DRA --> DA
    DA --> B
    R --> B

    %% Persistence dependencies
    P --> PA
    PA --> B
    PFS --> P
    PR --> P

    %% VOS dependencies
    V --> VA
    VA --> PA
    VF --> V
    V --> P

    %% Workspaces dependencies
    W --> WA
    WA --> B
    W --> DR
    W --> P

    %% Serialization dependencies
    SJ --> S
    S --> B

    %% Hosting dependencies
    H --> HA
    HA --> B

    %% Base dependencies
    F --> B
    ST --> B
    ST --> F

    classDef framework fill:#e1f5ff,stroke:#0288d1
    classDef toolkit fill:#fff9c4,stroke:#f57c00
    classDef base fill:#c8e6c9,stroke:#388e3c

    class LF,ACF,VAF,FE framework
    class MA,M,DAM,DA,DRA,DR,R,PA,P,PFS,PR,VA,V,VF,WA,W,S,SJ,HA,H toolkit
    class B,F,ST base
```

### Layer Dependency Rules

**✅ Allowed**:
- Framework → Toolkits
- Framework → Base
- Toolkits → Toolkits (carefully)
- Toolkits → Base
- Base → Base

**❌ Forbidden**:
- Base → Toolkits
- Base → Framework
- Toolkits → Framework
- Circular dependencies

---

## Domain Dependencies

### MVVM Domain Detailed

```mermaid
graph TB
    subgraph Application["Application Layer"]
        Blazor[Blazor Components]
        WPF[WPF Views]
    end

    subgraph MVVMLayer["MVVM Layer"]
        DAM[Data.Async.Mvvm]
        M[Mvvm]
        MA[Mvvm.Abstractions]
    end

    subgraph DataLayer["Data Layer"]
        DR[Data.Async.Reactive]
        DRA[Data.Async.Reactive.Abs]
        DA[Data.Async.Abstractions]
    end

    subgraph ReactiveLayer["Reactive Layer"]
        RUI[ReactiveUI]
        DD[DynamicData]
        R[LionFire.Reactive]
    end

    %% Application dependencies
    Blazor --> DAM
    WPF --> DAM
    Blazor --> M
    WPF --> M

    %% MVVM layer dependencies
    DAM --> M
    DAM --> DR
    DAM --> DRA
    M --> MA
    M --> RUI

    %% Data layer dependencies
    DR --> DRA
    DR --> R
    DR --> DD
    DRA --> DA

    %% Reactive layer dependencies
    R --> RUI
    R --> DD

    classDef app fill:#ffe0b2,stroke:#e65100
    classDef mvvm fill:#e1bee7,stroke:#7b1fa2
    classDef data fill:#b2dfdb,stroke:#00897b
    classDef reactive fill:#ffccbc,stroke:#d84315

    class Blazor,WPF app
    class DAM,M,MA mvvm
    class DR,DRA,DA data
    class RUI,DD,R reactive
```

### Data Domain Detailed

```mermaid
graph TB
    subgraph UILayer["UI Layer (ViewModels)"]
        GetterVM[GetterVM&lt;T&gt;]
        ValueVM[ValueVM&lt;T&gt;]
        ObsReaderVM[ObservableReaderVM]
    end

    subgraph ReactiveImpl["Reactive Implementation"]
        GetterRxO[GetterRxO&lt;T&gt;]
        ValueRxO[ValueRxO&lt;T&gt;]
        AsyncCollection[AsyncDynamicDataCollection]
    end

    subgraph ReactiveAbs["Reactive Abstractions"]
        IGetterRxO[IGetterRxO&lt;T&gt;]
        IValueRxO[IValueRxO&lt;T&gt;]
        IReactiveObjectEx[IReactiveObjectEx]
    end

    subgraph CoreAbs["Core Abstractions"]
        IGetter[IGetter&lt;T&gt;]
        ISetter[ISetter&lt;T&gt;]
        IValue[IValue&lt;T&gt;]
        IObsReader[IObservableReader&lt;K,V&gt;]
    end

    subgraph DataSource["Data Sources"]
        Files[File System]
        DB[Databases]
        API[APIs]
        VOS[VOS]
    end

    %% UI to Reactive Implementation
    GetterVM --> GetterRxO
    ValueVM --> ValueRxO
    ObsReaderVM --> AsyncCollection

    %% Reactive Implementation to Reactive Abstractions
    GetterRxO -.implements.-> IGetterRxO
    ValueRxO -.implements.-> IValueRxO

    %% Reactive Abstractions to Core Abstractions
    IGetterRxO --> IGetter
    IValueRxO --> IValue
    IReactiveObjectEx --> IGetter

    %% Core Abstractions to Data Sources
    IGetter --> Files
    ISetter --> Files
    IObsReader --> Files
    IGetter --> DB
    IValue --> DB
    IObsReader --> VOS

    classDef ui fill:#fff9c4,stroke:#f57c00
    classDef reactive fill:#e1bee7,stroke:#7b1fa2
    classDef abs fill:#b2dfdb,stroke:#00897b
    classDef source fill:#ffccbc,stroke:#d84315

    class GetterVM,ValueVM,ObsReaderVM ui
    class GetterRxO,ValueRxO,AsyncCollection reactive
    class IGetterRxO,IValueRxO,IReactiveObjectEx reactive
    class IGetter,ISetter,IValue,IObsReader abs
    class Files,DB,API,VOS source
```

---

## Complete Async Data Stack

### Full Stack from Data Source to UI

```mermaid
graph TB
    subgraph UI["UI Layer"]
        View[Blazor/WPF View]
    end

    subgraph VM["ViewModel Layer"]
        ObsReaderItemVM[ObservableReaderWriterItemVM]
        ValueVM[ValueVM&lt;T&gt;]
    end

    subgraph Reactive["Reactive Layer"]
        AsyncCollection[AsyncDynamicDataCollection]
        ValueRxO[ValueRxO&lt;T&gt;]
    end

    subgraph Persistence["Persistence Layer"]
        ObsReader[IObservableReader&lt;K,V&gt;]
        ObsWriter[IObservableWriter&lt;K,V&gt;]
    end

    subgraph Storage["Storage Layer"]
        FileWatcher[FileSystemWatcher]
        Serializer[Serialization]
        Files[(Files on Disk)]
    end

    %% UI to VM
    View -- binds to --> ObsReaderItemVM
    View -- binds to --> ValueVM

    %% VM to Reactive
    ObsReaderItemVM -- wraps --> AsyncCollection
    ObsReaderItemVM -- uses --> ValueRxO
    ValueVM -- wraps --> ValueRxO

    %% Reactive to Persistence
    AsyncCollection -- observes --> ObsReader
    ValueRxO -- reads from --> ObsReader
    ValueRxO -- writes to --> ObsWriter

    %% Persistence to Storage
    ObsReader -- watches --> FileWatcher
    ObsReader -- deserializes --> Serializer
    ObsWriter -- serializes --> Serializer
    Serializer -- reads/writes --> Files
    FileWatcher -- monitors --> Files

    classDef ui fill:#e3f2fd,stroke:#1976d2
    classDef vm fill:#f3e5f5,stroke:#7b1fa2
    classDef reactive fill:#e8f5e9,stroke:#388e3c
    classDef persist fill:#fff3e0,stroke:#f57c00
    classDef storage fill:#fce4ec,stroke:#c2185b

    class View ui
    class ObsReaderItemVM,ValueVM vm
    class AsyncCollection,ValueRxO reactive
    class ObsReader,ObsWriter persist
    class FileWatcher,Serializer,Files storage
```

### Data Flow Example

**User saves entity in UI:**

```mermaid
sequenceDiagram
    participant UI as Blazor Component
    participant VM as ObservableReaderWriterItemVM
    participant RxO as ValueRxO
    participant Writer as IObservableWriter
    participant Ser as Serializer
    participant FS as File System

    UI->>VM: User edits entity
    UI->>VM: Calls SaveCommand
    VM->>RxO: Set Value
    RxO->>Writer: Write(key, value)
    Writer->>Ser: Serialize(value)
    Ser->>FS: Write file
    FS-->>Ser: Success
    Ser-->>Writer: Success
    Writer-->>RxO: Success
    RxO-->>VM: Value updated
    VM-->>UI: UI refreshes
```

**File changes externally:**

```mermaid
sequenceDiagram
    participant FS as File System
    participant FW as FileSystemWatcher
    participant Reader as IObservableReader
    participant Coll as AsyncDynamicDataCollection
    participant VM as ObservableReaderItemVM
    participant UI as Blazor Component

    FS->>FW: File changed event
    FW->>Reader: Notify file changed
    Reader->>Reader: Deserialize file
    Reader->>Coll: Emit changeset
    Coll->>VM: Item updated
    VM->>UI: Property change notification
    UI->>UI: Re-render
```

---

## MVVM Stack

### MVVM Component Dependencies

```mermaid
graph TB
    subgraph BlazorComponents["Blazor Components"]
        ODV[ObservableDataView]
        BotRazor[Bot.razor]
        BotsRazor[Bots.razor]
    end

    subgraph MudBlazor["LionFire.Blazor.Components.MudBlazor"]
        ObsDataView[ObservableDataView Component]
    end

    subgraph ViewModels["Data.Async.Mvvm"]
        ObsDataVM[ObservableDataVM]
        ObsReaderItemVM[ObservableReaderWriterItemVM]
        GetterVM[GetterVM]
        ValueVM[ValueVM]
    end

    subgraph MVVM["Mvvm Core"]
        VMProvider[ViewModelProvider]
        IVM[IViewModel&lt;T&gt;]
    end

    subgraph ReactiveData["Data.Async.Reactive"]
        AsyncColl[AsyncDynamicDataCollection]
        ValueRxO[ValueRxO]
    end

    %% Blazor to MudBlazor
    BotRazor --> ObsDataView
    BotsRazor --> ObsDataView

    %% MudBlazor to ViewModels
    ObsDataView --> ObsDataVM
    BotRazor --> ObsReaderItemVM

    %% ViewModels to MVVM
    ObsDataVM --> VMProvider
    ObsReaderItemVM -.implements.-> IVM
    GetterVM -.implements.-> IVM
    ValueVM -.implements.-> IVM

    %% ViewModels to Reactive Data
    ObsDataVM --> AsyncColl
    ObsReaderItemVM --> ValueRxO
    GetterVM --> ValueRxO

    classDef blazor fill:#e3f2fd,stroke:#1976d2
    classDef mudblazor fill:#f3e5f5,stroke:#7b1fa2
    classDef vm fill:#e8f5e9,stroke:#388e3c
    classDef mvvm fill:#fff3e0,stroke:#f57c00
    classDef reactive fill:#fce4ec,stroke:#c2185b

    class ODV,BotRazor,BotsRazor blazor
    class ObsDataView mudblazor
    class ObsDataVM,ObsReaderItemVM,GetterVM,ValueVM vm
    class VMProvider,IVM mvvm
    class AsyncColl,ValueRxO reactive
```

---

## Workspaces Integration

### Workspaces Service Scoping

```mermaid
graph TB
    subgraph App["Application"]
        RootSP[Root ServiceProvider]
    end

    subgraph WS1["Workspace 1"]
        WS1SP[Workspace1 ServiceProvider]
        WS1Reader[IObservableReader&lt;Bot&gt;]
        WS1Writer[IObservableWriter&lt;Bot&gt;]
        WS1Dir[(Workspace1/bots/)]
    end

    subgraph WS2["Workspace 2"]
        WS2SP[Workspace2 ServiceProvider]
        WS2Reader[IObservableReader&lt;Bot&gt;]
        WS2Writer[IObservableWriter&lt;Bot&gt;]
        WS2Dir[(Workspace2/bots/)]
    end

    subgraph Configurator["Workspace Configuration"]
        WSConfig[IWorkspaceServiceConfigurator]
    end

    %% Root to Workspace scopes
    RootSP --> WSConfig
    WSConfig --> WS1SP
    WSConfig --> WS2SP

    %% Workspace services
    WS1SP --> WS1Reader
    WS1SP --> WS1Writer
    WS1Reader -.watches.-> WS1Dir
    WS1Writer -.writes to.-> WS1Dir

    WS2SP --> WS2Reader
    WS2SP --> WS2Writer
    WS2Reader -.watches.-> WS2Dir
    WS2Writer -.writes to.-> WS2Dir

    classDef root fill:#e3f2fd,stroke:#1976d2
    classDef workspace fill:#e8f5e9,stroke:#388e3c
    classDef config fill:#fff3e0,stroke:#f57c00
    classDef storage fill:#fce4ec,stroke:#c2185b

    class RootSP root
    class WS1SP,WS1Reader,WS1Writer,WS2SP,WS2Reader,WS2Writer workspace
    class WSConfig config
    class WS1Dir,WS2Dir storage
```

**Key Point**: Each workspace has its own scoped `ServiceProvider` with isolated `IObservableReader/Writer` instances pointing to different directories.

### Workspace Component Stack

```mermaid
graph TB
    subgraph UI["UI Layer"]
        BlazorPage[Blazor Page]
    end

    subgraph Workspaces["Workspaces Layer"]
        WSManager[WorkspaceManager]
        WSServices[WorkspaceServices]
        WSConfig[WorkspaceServiceConfigurator]
    end

    subgraph Persistence["Persistence Layer"]
        DirDocService[DirectoryWorkspaceDocumentService]
        ObsReader[IObservableReader]
        ObsWriter[IObservableWriter]
    end

    subgraph Storage["Storage Layer"]
        WSDir[(Workspace Directory)]
        BotsDir[(bots/ subdirectory)]
    end

    %% UI to Workspaces
    BlazorPage -- CascadingParameter --> WSServices

    %% Workspaces layer
    WSManager --> WSServices
    WSServices --> WSConfig
    WSConfig -- configures --> DirDocService

    %% Persistence layer
    DirDocService --> ObsReader
    DirDocService --> ObsWriter
    ObsReader -.monitors.-> BotsDir
    ObsWriter -.writes to.-> BotsDir
    BotsDir -.subdirectory of.-> WSDir

    classDef ui fill:#e3f2fd,stroke:#1976d2
    classDef workspace fill:#e8f5e9,stroke:#388e3c
    classDef persist fill:#fff3e0,stroke:#f57c00
    classDef storage fill:#fce4ec,stroke:#c2185b

    class BlazorPage ui
    class WSManager,WSServices,WSConfig workspace
    class DirDocService,ObsReader,ObsWriter persist
    class WSDir,BotsDir storage
```

---

## VOS Dependencies

### VOS Core Architecture

```mermaid
graph TB
    subgraph Application["Application"]
        App[VOS Application]
    end

    subgraph VOS["VOS Layer"]
        Vob[Vob - Virtual Object]
        VRef[VosReference]
        Mount[Mount]
        Overlay[Overlay]
    end

    subgraph Persistence["Persistence Layer"]
        PersistenceProvider[IPersistenceProvider]
        FileProvider[Filesystem Provider]
        RedisProvider[Redis Provider]
    end

    subgraph Handles["Handles Layer"]
        Handle[IHandle]
        ReadHandle[IReadHandle]
        WriteHandle[IWriteHandle]
    end

    subgraph Referencing["Referencing Layer"]
        IRef[IReference]
        RefResolver[Reference Resolver]
    end

    %% Application to VOS
    App --> Vob
    App --> VRef

    %% VOS dependencies
    Vob --> Mount
    Mount --> Overlay
    Vob --> Handle
    VRef -.resolves to.-> Vob

    %% Handles to Persistence
    Handle --> PersistenceProvider
    ReadHandle --> PersistenceProvider
    WriteHandle --> PersistenceProvider
    PersistenceProvider --> FileProvider
    PersistenceProvider --> RedisProvider

    %% Referencing
    VRef -.implements.-> IRef
    RefResolver --> IRef

    classDef app fill:#e3f2fd,stroke:#1976d2
    classDef vos fill:#e8f5e9,stroke:#388e3c
    classDef persist fill:#fff3e0,stroke:#f57c00
    classDef handles fill:#f3e5f5,stroke:#7b1fa2
    classDef ref fill:#fce4ec,stroke:#c2185b

    class App app
    class Vob,VRef,Mount,Overlay vos
    class PersistenceProvider,FileProvider,RedisProvider persist
    class Handle,ReadHandle,WriteHandle handles
    class IRef,RefResolver ref
```

### VOS Mount Stack

```mermaid
graph TB
    subgraph VirtualTree["Virtual Object Tree"]
        Root[/]
        App[/app]
        Config[/app/config]
        Data[/app/data]
    end

    subgraph Mounts["Mounts"]
        FSMount[Filesystem Mount]
        RedisMount[Redis Mount]
        OverlayMount[Overlay Mount]
    end

    subgraph Storage["Physical Storage"]
        FSDir[(Filesystem Directory)]
        RedisDB[(Redis Database)]
    end

    %% Virtual tree structure
    Root --> App
    App --> Config
    App --> Data

    %% Mounts to virtual paths
    FSMount -.mounted at.-> App
    RedisMount -.mounted at.-> Data
    OverlayMount -.overlays.-> App

    %% Mounts to storage
    FSMount --> FSDir
    RedisMount --> RedisDB

    classDef vtree fill:#e3f2fd,stroke:#1976d2
    classDef mounts fill:#e8f5e9,stroke:#388e3c
    classDef storage fill:#fce4ec,stroke:#c2185b

    class Root,App,Config,Data vtree
    class FSMount,RedisMount,OverlayMount mounts
    class FSDir,RedisDB storage
```

---

## Dependency Matrix

### Core Library Dependencies

| Library | Base | Flex | Structures | Data.Async.Abs | MVVM.Abs | Persistence.Abs |
|---------|------|------|------------|----------------|----------|-----------------|
| **Base** | - | - | - | - | - | - |
| **Flex** | ✅ | - | - | - | - | - |
| **Structures** | ✅ | ✅ | - | - | - | - |
| **Data.Async.Abs** | ✅ | - | - | - | - | - |
| **Data.Async.Reactive.Abs** | ✅ | - | - | ✅ | - | - |
| **Data.Async.Reactive** | ✅ | - | - | ✅ | - | - |
| **MVVM.Abstractions** | ✅ | - | - | - | - | - |
| **MVVM** | ✅ | - | - | - | ✅ | - |
| **Data.Async.Mvvm** | ✅ | - | - | ✅ | ✅ | - |
| **Persistence.Abs** | ✅ | - | - | - | - | - |
| **Persistence** | ✅ | - | - | - | - | ✅ |
| **Vos.Abstractions** | ✅ | - | - | - | - | ✅ |
| **Vos** | ✅ | ✅ | - | - | - | ✅ |
| **Workspaces.Abs** | ✅ | - | - | - | - | - |
| **Workspaces** | ✅ | - | - | ✅ | - | ✅ |

### External Package Dependencies

| Library | ReactiveUI | DynamicData | CommunityToolkit.Mvvm | Newtonsoft.Json |
|---------|-----------|-------------|----------------------|-----------------|
| **Data.Async.Reactive** | ✅ | ✅ | - | - |
| **MVVM** | ✅ | - | - | - |
| **Data.Async.Mvvm** | ✅ | ✅ | ✅ | - |
| **Reactive** | ✅ | ✅ | - | - |
| **Serialization.Json** | - | - | - | ✅ |

---

## Managing Dependencies

### Circular Dependency Detection

**Check for circular dependencies:**

```bash
# Using dotnet list package with --include-transitive
dotnet-win list package --include-transitive | grep "LionFire"

# Check for circular references in a specific project
dotnet-win list src/LionFire.Mvvm/LionFire.Mvvm.csproj reference
```

**Common circular dependency scenarios to avoid:**

❌ **Bad: Toolkit depends on Framework**
```
LionFire.Mvvm → LionFire.Framework
LionFire.Framework → LionFire.Mvvm  // Circular!
```

❌ **Bad: Base depends on Toolkit**
```
LionFire.Base → LionFire.Mvvm  // Base cannot depend on Toolkit!
```

### Adding New Dependencies

**Checklist before adding a dependency:**

1. **Check layer rules**:
   - [ ] Am I in the right layer?
   - [ ] Does the dependency respect layer hierarchy?

2. **Prefer abstractions**:
   - [ ] Can I depend on the abstraction instead of implementation?
   - [ ] Will this create tight coupling?

3. **External packages**:
   - [ ] Is this package absolutely necessary?
   - [ ] Is it maintained and compatible with .NET 9?
   - [ ] Add to `Directory.Packages.props` (Central Package Management)

4. **Document the dependency**:
   - [ ] Update this dependency graph if it's a significant architectural dependency
   - [ ] Add comments in `.csproj` if the dependency is non-obvious

### Dependency Guidelines

**DO**:
- ✅ Depend on abstractions over implementations
- ✅ Keep Base layer free of external dependencies
- ✅ Use Central Package Management for version consistency
- ✅ Document architectural dependencies
- ✅ Minimize inter-toolkit coupling

**DON'T**:
- ❌ Create circular dependencies
- ❌ Make Base layer depend on Toolkits
- ❌ Make Toolkits depend on Frameworks
- ❌ Add external packages without consideration
- ❌ Tightly couple toolkits without abstractions

### Refactoring Dependencies

**When you find a problematic dependency:**

1. **Identify the issue**:
   - Is it circular?
   - Is it violating layer rules?
   - Is it creating tight coupling?

2. **Choose a refactoring strategy**:
   - Extract abstraction to separate assembly
   - Move functionality to appropriate layer
   - Use dependency injection to invert dependency
   - Create adapter/bridge pattern

3. **Example refactoring**:

**Before (Bad)**:
```
LionFire.Mvvm (Toolkit)
  → LionFire.Data.Async.Reactive (Toolkit Implementation)  // Tight coupling
```

**After (Good)**:
```
LionFire.Mvvm (Toolkit)
  → LionFire.Data.Async.Reactive.Abstractions (Toolkit Abstraction)  // Loose coupling
```

### Dependency Visualization Tools

**Recommended tools for visualizing dependencies:**

1. **MSBuild Binary Log Viewer**:
   ```bash
   dotnet-win build -bl
   # Open .binlog file in viewer
   ```

2. **Visual Studio Dependency Diagrams** (Enterprise edition)

3. **dotnet-depends** (NuGet package):
   ```bash
   dotnet tool install -g dotnet-depends
   dotnet depends src/LionFire.Mvvm/LionFire.Mvvm.csproj
   ```

4. **Custom script** (for this repository):
   ```bash
   # List all LionFire project references
   find src -name "*.csproj" -exec grep -H "LionFire" {} \;
   ```

---

## Guidelines for New Projects

### Pre-Flight Dependency Checklist

Before creating a new project:

1. **Determine layer**:
   - [ ] Base (no external deps)
   - [ ] Toolkit (domain-specific)
   - [ ] Framework (integrated)

2. **Plan dependencies**:
   - [ ] List required LionFire projects
   - [ ] List required external packages
   - [ ] Verify no circular dependencies
   - [ ] Verify layer rules respected

3. **Create abstractions if needed**:
   - [ ] Will other projects need to depend on this?
   - [ ] Should I create a separate `.Abstractions` project?

4. **Document architecture**:
   - [ ] Update this dependency graph if significant
   - [ ] Add to appropriate domain documentation

### Example: Adding a New Toolkit

**Scenario**: Create a new "LionFire.Caching" toolkit

**Step 1: Plan dependencies**
```
LionFire.Caching.Abstractions
  → LionFire.Base (✅ allowed)

LionFire.Caching
  → LionFire.Caching.Abstractions (✅ allowed)
  → LionFire.Base (✅ allowed)
  → Microsoft.Extensions.Caching.Memory (✅ external package OK)
```

**Step 2: Add to Directory.Packages.props**
```xml
<PackageVersion Include="Microsoft.Extensions.Caching.Memory" Version="9.0.0" />
```

**Step 3: Create projects**
```bash
dotnet new classlib -n LionFire.Caching.Abstractions -o src/LionFire.Caching.Abstractions
dotnet new classlib -n LionFire.Caching -o src/LionFire.Caching
```

**Step 4: Add dependencies**
```bash
# Abstractions
dotnet add src/LionFire.Caching.Abstractions reference src/LionFire.Base

# Implementation
dotnet add src/LionFire.Caching reference src/LionFire.Caching.Abstractions
dotnet add src/LionFire.Caching reference src/LionFire.Base
dotnet add src/LionFire.Caching package Microsoft.Extensions.Caching.Memory
```

**Step 5: Update documentation**
- Add to this dependency graph (Toolkit section)
- Create `src/LionFire.Caching/CLAUDE.md`
- Update `docs/README.md` if appropriate

---

## Summary

### Dependency Principles

1. **Respect layer hierarchy**: Framework → Toolkit → Base
2. **Prefer abstractions**: Depend on interfaces, not implementations
3. **Avoid circular dependencies**: Use dependency inversion
4. **Minimize coupling**: Toolkits should be loosely coupled
5. **Document dependencies**: Keep this graph updated

### Quick Reference

| From Layer | Can Depend On |
|------------|---------------|
| **Framework** | Toolkits, Base, External Packages |
| **Toolkit** | Other Toolkits (carefully), Base, External Packages |
| **Base** | Other Base libraries, BCL only |

### Tools for Managing Dependencies

- **Central Package Management**: `Directory.Packages.props`
- **MSBuild Binary Log**: `dotnet-win build -bl`
- **Reference Listing**: `dotnet-win list reference`
- **Package Listing**: `dotnet-win list package`

---

**Related Documentation**:
- [Architecture Overview](README.md)
- [Layer Architecture](layers.md)
- [Main Documentation](../README.md)
- [Repository CLAUDE.md](../../CLAUDE.md)
