# Workspaces Architecture

**Overview**: Workspaces are user-centric data containers that provide isolated, file-based storage for different sets of documents. They enable multi-workspace scenarios, proper service scoping, and flexible UI organization.

---

## Table of Contents

1. [What Are Workspaces?](#what-are-workspaces)
2. [Core Concepts](#core-concepts)
3. [Architecture Layers](#architecture-layers)
4. [Service Scoping Model](#service-scoping-model)
5. [Document Type System](#document-type-system)
6. [Persistence Strategy](#persistence-strategy)
7. [Lifecycle Management](#lifecycle-management)
8. [Integration Points](#integration-points)
9. [When to Use Workspaces](#when-to-use-workspaces)

---

## What Are Workspaces?

### Definition

A **workspace** is a directory-backed, user-centric container for heterogeneous documents. Think of it as a "project folder" that can contain multiple types of related data with a unified UI for management.

### Example Scenarios

**Trading Application**:
```
C:\Users\Alice\Trading\
├── WorkA/                      ← Workspace 1
│   ├── Bots\
│   │   ├── bot1.hjson
│   │   └── bot2.hjson
│   └── Portfolios\
│       └── portfolio1.hjson
└── WorkB/                      ← Workspace 2
    ├── Bots\
    │   └── bot3.hjson
    └── Portfolios\
        └── portfolio2.hjson
```

Each workspace is **independent**:
- Separate file storage
- Separate service instances
- Separate UI contexts
- Can be opened/closed independently

### Key Characteristics

1. **User-Centric**: Organized around user workflows, not technical boundaries
2. **Heterogeneous**: Can contain multiple document types (Bots, Portfolios, Strategies, etc.)
3. **File-Backed**: Persisted as HJSON files in a directory structure
4. **Isolated**: Each workspace has its own service scope and data
5. **UI-Integrated**: Common workspace management UI components
6. **Runtime-Configurable**: Workspaces can be created/opened at runtime

---

## Core Concepts

### 1. Workspace Identity

```csharp
public class UserWorkspacesService
{
    // Get workspace by ID
    public IReference UserWorkspaces { get; }  // Base directory: C:\Users\Alice\Trading\

    // Get specific workspace
    var workspaceRef = UserWorkspaces.GetChild("WorkA");  // C:\Users\Alice\Trading\WorkA\
}
```

### 2. Document Types

**Member Types** are entity types that can live in workspaces:

```csharp
services
    .AddWorkspaceChildType<BotEntity>()      // Bots subdirectory
    .AddWorkspaceChildType<Portfolio>()       // Portfolios subdirectory
    .AddWorkspaceChildType<Strategy>();       // Strategies subdirectory
```

Each type gets:
- A subdirectory (plural name by convention)
- File-based persistence (HJSON by default)
- Observable reader/writer services
- Optional runner for active documents

### 3. Workspace Services

Each workspace has its own `IServiceProvider` with workspace-specific services:

```csharp
// Root services (singleton, application-wide)
services.GetService<ILogger>();                    // ✅ Available
services.GetService<UserWorkspacesService>();      // ✅ Available

// Workspace services (scoped per workspace)
workspaceServices.GetService<IObservableReader<string, BotEntity>>();  // ✅ Points to this workspace's Bots\
```

### 4. Service Configurators

`IWorkspaceServiceConfigurator` implementations set up workspace services:

```csharp
public class WorkspaceTypesConfigurator : IWorkspaceServiceConfigurator
{
    public async ValueTask ConfigureWorkspaceServices(
        IServiceCollection services,
        UserWorkspacesService userWorkspacesService,
        string? workspaceId)
    {
        // Called when workspace is created/loaded
        // Registers IObservableReader/Writer for each member type
    }
}
```

---

## Architecture Layers

### Layer Diagram

```
┌────────────────────────────────────────────────────────────────┐
│                     Application Layer                          │
│  - Blazor Pages/Components                                     │
│  - ViewModels (BotVM, PortfolioVM)                            │
└───────────────────────┬────────────────────────────────────────┘
                        │
                        ↓ Uses
┌────────────────────────────────────────────────────────────────┐
│                   Workspace UI Layer                           │
│  LionFire.Workspaces.UI.Blazor                                │
│  - WorkspaceLayout                                             │
│  - Workspace selector components                               │
│  - Provides CascadingValue WorkspaceServices                  │
└───────────────────────┬────────────────────────────────────────┘
                        │
                        ↓ Manages
┌────────────────────────────────────────────────────────────────┐
│                    Workspace Core Layer                        │
│  LionFire.Workspaces                                          │
│  - UserWorkspacesService                                       │
│  - WorkspaceServiceConfigurator                               │
│  - DirectoryWorkspaceDocumentService<T>                       │
│  - WorkspaceDocumentRunner<T>                                 │
└───────────────────────┬────────────────────────────────────────┘
                        │
                        ↓ Uses
┌────────────────────────────────────────────────────────────────┐
│              Reactive Persistence Layer                        │
│  LionFire.Reactive                                            │
│  - IObservableReader<TKey, TValue>                            │
│  - IObservableWriter<TKey, TValue>                            │
│  - HjsonFsDirectoryReaderRx / WriterRx                        │
└───────────────────────┬────────────────────────────────────────┘
                        │
                        ↓ Reads/Writes
┌────────────────────────────────────────────────────────────────┐
│                      File System                               │
│  C:\Users\Alice\Trading\WorkA\Bots\bot1.hjson                │
└────────────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

**Application Layer**:
- Consumes workspace services via CascadingParameter
- Uses VMs and components
- Implements business logic

**Workspace UI Layer**:
- Provides workspace layout and navigation
- Manages workspace lifecycle (open/close)
- Cascades WorkspaceServices to child components

**Workspace Core Layer**:
- Manages workspace registration and configuration
- Creates workspace-scoped service providers
- Coordinates document services and runners

**Reactive Persistence Layer**:
- Provides file-based observable collections
- Handles serialization (HJSON)
- Monitors file system changes

---

## Service Scoping Model

### The Problem Workspaces Solve

**Without workspace scoping**:
```csharp
// ❌ Single global service - can only point to one directory!
services.AddSingleton<IObservableReader<string, BotEntity>>(sp => {
    return new HjsonFsDirectoryReaderRx<string, BotEntity>(
        sp, new DirectorySelector("C:\\HardcodedPath\\Bots"));
});
```

Problems:
- Can't have multiple workspaces open
- Hardcoded paths
- No isolation between workspaces
- Can't support per-workspace configuration

**With workspace scoping**:
```csharp
// ✅ Each workspace gets its own reader/writer
foreach (var workspace in activeWorkspaces)
{
    var workspaceServices = CreateWorkspaceServices(workspace);

    // This workspace's reader points to C:\Users\Alice\WorkA\Bots\
    var readerA = workspaceServices.GetService<IObservableReader<string, BotEntity>>();

    // Different workspace's reader points to C:\Users\Alice\WorkB\Bots\
    var readerB = otherWorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
}
```

### Service Provider Hierarchy

```
Root Services (Application Lifetime)
├── ILogger (singleton)
├── IConfiguration (singleton)
├── UserWorkspacesService (singleton)
├── IWorkspaceServiceConfigurator[] (collection)
└── DirectoryWorkspaceDocumentService<T> (hosted service)

Workspace Services (Per Workspace)
├── IObservableReader<string, BotEntity>    ← Points to WorkA/Bots/
├── IObservableWriter<string, BotEntity>    ← Points to WorkA/Bots/
├── IObservableReader<string, Portfolio>    ← Points to WorkA/Portfolios/
└── IObservableWriter<string, Portfolio>    ← Points to WorkA/Portfolios/
```

**See [Service Scoping Deep Dive](service-scoping.md) for detailed explanation.**

---

## Document Type System

### Defining a Document Type

**Step 1: Create Entity**

```csharp
[Alias("Bot")]  // Used in file paths
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;
    [Reactive] private string? _description;

    // Entity logic here
}
```

**Step 2: Create ViewModel (Optional)**

```csharp
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value) { }

    // UI-specific properties and commands
}
```

**Step 3: Register with Workspace**

```csharp
services
    // Declares this type can live in workspaces
    .AddWorkspaceChildType<BotEntity>()

    // Sets up document service and runner
    .AddWorkspaceDocumentService<string, BotEntity>();
```

### File Naming Convention

```
WorkspaceDirectory/
└── {PluralTypeName}/        ← "Bots" for BotEntity
    ├── bot-alpha.hjson      ← Key: "bot-alpha"
    ├── bot-beta.hjson       ← Key: "bot-beta"
    └── advanced-bot.hjson   ← Key: "advanced-bot"
```

**Plural name** is derived from `[Alias]` attribute or type name:
- `BotEntity` → `Bots`
- `Portfolio` → `Portfolios`
- `Strategy` → `Strategies`

### File Format (HJSON)

```hjson
// WorkA/Bots/my-bot.hjson
// Root braces OMITTED per HJSON convention
name: My Trading Bot
description: Scalping strategy for BTCUSDT
parameters: {
  timeframe: 15m
  stopLoss: 1.5
  takeProfit: 3.0
}
enabled: true
```

---

## Persistence Strategy

### Read/Write Flow

```
UI Component
    ↓ Uses
ObservableReaderWriterItemVM<string, BotEntity, BotVM>
    ↓ Wraps
IObservableReader<string, BotEntity> + IObservableWriter<string, BotEntity>
    ↓ Backed by
HjsonFsDirectoryReaderRx / HjsonFsDirectoryWriterRx
    ↓ Reads/Writes
C:\Users\Alice\WorkA\Bots\my-bot.hjson
```

### Read Operations

```csharp
var reader = workspaceServices.GetRequiredService<IObservableReader<string, BotEntity>>();

// Subscribe to all bots in workspace
reader.Values.Connect().Subscribe(changeSet => {
    foreach (var change in changeSet)
    {
        Console.WriteLine($"Bot {change.Key}: {change.Reason}");
    }
});

// Get specific bot
var result = await reader.TryGetValue("my-bot");
if (result.HasValue)
{
    var bot = result.Value;
}
```

### Write Operations

```csharp
var writer = workspaceServices.GetRequiredService<IObservableWriter<string, BotEntity>>();

var bot = new BotEntity { Name = "New Bot", Description = "Test" };
await writer.Write("new-bot", bot);

// File created: WorkA/Bots/new-bot.hjson
```

### Automatic File Watching

When files change externally:

```
1. User edits WorkA/Bots/my-bot.hjson in text editor
2. HjsonFsDirectoryReaderRx detects file change
3. Reloads entity from disk
4. Publishes update to Observable
5. UI components automatically refresh via reactive bindings
```

---

## Lifecycle Management

### Workspace Lifecycle

```
┌─────────────┐
│   Create    │  - Directory created
│  Workspace  │  - Services configured
└──────┬──────┘
       ↓
┌─────────────┐
│    Open     │  - Service provider built
│  Workspace  │  - Document services started
└──────┬──────┘  - Runners activated
       ↓
┌─────────────┐
│   Active    │  - User works with documents
│  Workspace  │  - File watching active
└──────┬──────┘  - Services available
       ↓
┌─────────────┐
│   Close     │  - Runners stopped
│  Workspace  │  - Subscriptions disposed
└──────┬──────┘  - Services cleaned up
       ↓
┌─────────────┐
│   Dispose   │  - Service provider disposed
│             │  - Resources released
└─────────────┘
```

### Document Runner Pattern

Some documents are "active" and need lifecycle management:

```csharp
public class BotRunner : IWorkspaceDocumentRunner<string, BotEntity>, IObserver<BotEntity>
{
    public void OnNext(BotEntity bot)
    {
        // Document changed - react to updates
        if (bot.Enabled)
        {
            StartBot(bot);
        }
        else
        {
            StopBot(bot);
        }
    }
}
```

Register runner:

```csharp
services.TryAddEnumerable(
    ServiceDescriptor.Singleton<IWorkspaceDocumentRunner<string, BotEntity>, BotRunner>()
);
```

**Result**: When bot entity files are created/modified, runners automatically start/stop bots.

---

## Integration Points

### 1. MVVM Integration

**ViewModels** wrap workspace documents:

```csharp
// Workspace-scoped services
var reader = workspaceServices.GetRequiredService<IObservableReader<string, BotEntity>>();
var writer = workspaceServices.GetRequiredService<IObservableWriter<string, BotEntity>>();

// Create VM
var vm = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);
vm.Id = "my-bot";

// VM automatically loads from workspace, saves on changes
```

### 2. Blazor UI Integration

**List View** (automatic):

```razor
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices" />
```

**Detail View** (manual):

```razor
@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
        // Use reader...
    }
}
```

### 3. VOS Integration

Workspaces can integrate with the Virtual Object System:

```csharp
// Mount workspace as VOS path
vos.Mount("workspace://WorkA", new FileReference("C:\\Users\\Alice\\WorkA"));

// Access via VOS
var botHandle = vos.GetHandle<BotEntity>("workspace://WorkA/Bots/my-bot");
var bot = await botHandle.Get();
```

### 4. Dependency Injection

```csharp
// Root services
builder.Services
    .AddWorkspaces(configuration)
    .AddWorkspaceChildType<BotEntity>()
    .AddWorkspaceDocumentService<string, BotEntity>();

// Workspace services (automatically configured)
workspaceServices
    .GetService<IObservableReader<string, BotEntity>>()   // ✅ Automatic
    .GetService<IObservableWriter<string, BotEntity>>();  // ✅ Automatic
```

---

## When to Use Workspaces

### ✅ Use Workspaces When:

1. **User-Centric Organization**: Users organize data into "projects" or "contexts"
2. **Multiple Document Types**: Need to group heterogeneous entities (Bots + Portfolios + Strategies)
3. **File-Based Storage**: Want human-readable, version-controllable files
4. **Multi-Workspace Scenarios**: Users might have multiple workspaces open
5. **Isolation Required**: Different workspaces should have independent data/services
6. **UI Management**: Want common workspace management UI (selector, properties, etc.)

### ❌ Don't Use Workspaces When:

1. **Single Global Data Store**: Application has one shared data context
2. **Database-Only**: All data lives in a database (use Ided/Assets instead)
3. **No User Organization**: Data organization is purely technical, not user-driven
4. **Simple Single Files**: Just need to read/write a config file (use IObservableReader directly)
5. **No Isolation Needed**: All users/contexts share the same data

### Alternative Patterns

| Pattern | Use When | Example |
|---------|----------|---------|
| **Workspaces** | User-centric, file-based, multi-type | Trading workspace with bots, portfolios, strategies |
| **VOS** | Complex virtual filesystem with mounts | Overlay configs, zip files, databases into unified tree |
| **Ided/Assets** | Database entities with primary keys | Game assets, inventory items |
| **Direct IObservableReader** | Simple file watching | Watch a single config file |
| **Persistence Layer** | Custom storage backend | Non-file persistence (Redis, MongoDB) |

---

## Example: Trading Workspace

### Workspace Structure

```
C:\Trading\Users\Alice\
├── DayTrading\                         ← Workspace
│   ├── Bots\
│   │   ├── scalper-btc.hjson
│   │   └── momentum-eth.hjson
│   ├── Portfolios\
│   │   └── main-portfolio.hjson
│   └── Strategies\
│       └── mean-reversion.hjson
└── SwingTrading\                       ← Another Workspace
    ├── Bots\
    │   └── swing-bot.hjson
    └── Portfolios\
        └── swing-portfolio.hjson
```

### Setup

```csharp
// Program.cs
services
    .AddWorkspaces(configuration)
    .AddWorkspaceChildType<BotEntity>()
    .AddWorkspaceChildType<Portfolio>()
    .AddWorkspaceChildType<Strategy>()
    .AddWorkspaceDocumentService<string, BotEntity>()
    .AddWorkspaceDocumentService<string, Portfolio>()
    .AddWorkspaceDocumentService<string, Strategy>();
```

### Usage

```csharp
// User opens "DayTrading" workspace
var workspaceServices = await userWorkspacesService
    .GetWorkspaceServicesAsync("DayTrading");

// Access bots from this workspace
var botReader = workspaceServices
    .GetRequiredService<IObservableReader<string, BotEntity>>();

// All bots in DayTrading workspace
botReader.Keys.Items  // ["scalper-btc", "momentum-eth"]

// Switch to "SwingTrading" workspace
var swingServices = await userWorkspacesService
    .GetWorkspaceServicesAsync("SwingTrading");

var swingBotReader = swingServices
    .GetRequiredService<IObservableReader<string, BotEntity>>();

swingBotReader.Keys.Items  // ["swing-bot"]
```

---

## Summary

### Key Principles

1. **Workspaces are user-centric data containers**
2. **Each workspace has isolated services** (IObservableReader/Writer per workspace)
3. **Services are scoped, not singleton**
4. **File-based persistence with HJSON**
5. **Observable changes for reactive UI**
6. **Lifecycle-managed with runners**

### Architecture Patterns

- **Separation of Concerns**: Root services vs workspace services
- **Dependency Injection**: Workspace-scoped service providers
- **Observer Pattern**: File watching and reactive updates
- **Factory Pattern**: Service configurators create workspace services
- **Repository Pattern**: IObservableReader/Writer abstract persistence

### Integration

```
Workspaces ← → MVVM ← → Blazor UI
    ↓
Reactive Persistence ← → File System
    ↓
VOS (optional)
```

---

## Related Documentation

- **[Service Scoping Deep Dive](service-scoping.md)** - Detailed DI and service resolution
- **[Document Types](document-types.md)** - Creating and registering document types
- **[Blazor MVVM Patterns](../../ui/blazor-mvvm-patterns.md)** - UI patterns for workspaces
- **[How-To: Create Workspace Page](../../guides/how-to/create-blazor-workspace-page.md)** - Step-by-step guide
- **Library References**:
  - [LionFire.Workspaces CLAUDE.md](../../../src/LionFire.Workspaces/CLAUDE.md)
  - [LionFire.Workspaces.UI.Blazor CLAUDE.md](../../../src/LionFire.Workspaces.UI.Blazor/CLAUDE.md)
  - [LionFire.Blazor.Components.MudBlazor CLAUDE.md](../../../src/LionFire.Blazor.Components.MudBlazor/CLAUDE.md)
