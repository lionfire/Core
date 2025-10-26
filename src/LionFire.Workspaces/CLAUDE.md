# LionFire.Workspaces

## Overview

Core workspace infrastructure providing directory-backed, scoped service containers for user-centric document organization. This library implements the workspace service model, document service pattern, and runner infrastructure that enables isolated, file-based data management.

**Key Concept**: Workspaces are **service scope containers** - each workspace gets its own `IServiceProvider` with workspace-specific `IObservableReader/Writer` instances pointing to that workspace's directory.

---

## Core Classes

### UserWorkspacesService

**Purpose**: Manages the workspace directory location and provides access to workspace references.

**Lifecycle**: Typically registered as singleton in root DI container, but stores per-user/per-workspace state.

```csharp
public class UserWorkspacesService
{
    // Root service provider
    public IServiceProvider ServiceProvider { get; }

    // User-scoped services (optional)
    public IServiceProvider? UserServices { get; set; }

    // Base directory for all workspaces
    public IReference? UserWorkspaces { get; set; }

    // Filesystem path (convenience accessor)
    public string? UserWorkspacesDir { get; set; }

    public UserWorkspacesService(IServiceProvider serviceProvider) { }
}
```

**Usage**:
```csharp
// Typically configured by UI layer
userWorkspacesService.UserWorkspacesDir = "C:\\Users\\Alice\\Trading\\Workspaces";

// Get specific workspace directory
var workspaceRef = userWorkspacesService.UserWorkspaces.GetChild("MyWorkspace");
// Result: C:\Users\Alice\Trading\Workspaces\MyWorkspace
```

**Notes**:
- Stores workspace base path, not individual workspace services
- Individual workspace service providers created on-demand
- Lifetime: Singleton in root container
- Configuration: Usually set by `WorkspaceLayoutVM` or similar UI component

---

### WorkspaceConfiguration

**Purpose**: Stores the list of document types that can live in workspaces.

```csharp
public class WorkspaceConfiguration
{
    // Types registered via AddWorkspaceChildType<T>()
    public List<Type> MemberTypes { get; set; } = new();
}
```

**Usage**:
```csharp
// Registration (in Program.cs)
services
    .AddWorkspaceChildType<BotEntity>()      // Adds to MemberTypes
    .AddWorkspaceChildType<Portfolio>()      // Adds to MemberTypes
    .Configure<WorkspaceConfiguration>(config => {
        // MemberTypes now contains [typeof(BotEntity), typeof(Portfolio)]
    });
```

**Used By**: `WorkspaceTypesConfigurator` to know which types to register readers/writers for.

---

### Workspace

**Purpose**: Represents a workspace instance (currently minimal, extensible via `FlexObject`).

```csharp
public class Workspace : FlexObject, IWorkspace
{
    // Currently just a marker type
    // Can be extended via FlexData property for workspace-specific settings
}
```

**Usage**:
```csharp
// Workspaces are persisted as HJSON files in parent directory
// C:\Users\Alice\Trading\Workspaces\workspace1.hjson

// Access via IObservableReader<string, Workspace>
var workspacesReader = services.GetService<IObservableReader<string, Workspace>>();
var workspace = await workspacesReader.TryGetValue("workspace1");
```

**Future**: May hold workspace-specific configuration, preferences, or metadata.

---

## Service Infrastructure

### IWorkspaceServiceConfigurator

**Interface** (from LionFire.Workspaces.Abstractions):
```csharp
public interface IWorkspaceServiceConfigurator
{
    ValueTask ConfigureWorkspaceServices(
        IServiceCollection services,
        UserWorkspacesService userWorkspacesService,
        string? workspaceId);
}
```

**Purpose**: Callback interface invoked when a workspace service provider is being built.

**Implementation**: `WorkspaceTypesConfigurator` (registers readers/writers for member types).

**Usage**:
```csharp
// Register configurator in root services
services.AddSingleton<IWorkspaceServiceConfigurator, WorkspaceTypesConfigurator>();
services.AddSingleton<IWorkspaceServiceConfigurator, MyCustomConfigurator>();

// When workspace is loaded, all configurators are invoked:
foreach (var configurator in serviceProvider.GetServices<IWorkspaceServiceConfigurator>())
{
    await configurator.ConfigureWorkspaceServices(workspaceServices, userWorkspacesService, "workspace1");
}
```

---

### WorkspaceTypesConfigurator

**Purpose**: Main configurator that registers `IObservableReader/Writer` for each member type.

**What It Does**:
1. Gets workspace directory from `UserWorkspacesService`
2. For each type in `WorkspaceConfiguration.MemberTypes`:
   - Calls `RegisterObservablesInSubDirForType<T>()`
   - Registers `IObservableReader<string, T>`
   - Registers `IObservableWriter<string, T>`
   - Points to `{WorkspaceDir}/{PluralTypeName}/` subdirectory

**Example**:
```csharp
// Given:
//   WorkspaceDir: C:\Users\Alice\Trading\Workspaces\workspace1
//   MemberTypes: [BotEntity, Portfolio]

// Configurator registers:
//   IObservableReader<string, BotEntity>
//     -> Points to C:\Users\Alice\Trading\Workspaces\workspace1\Bots\
//   IObservableWriter<string, BotEntity>
//     -> Points to C:\Users\Alice\Trading\Workspaces\workspace1\Bots\
//   IObservableReader<string, Portfolio>
//     -> Points to C:\Users\Alice\Trading\Workspaces\workspace1\Portfolios\
//   IObservableWriter<string, Portfolio>
//     -> Points to C:\Users\Alice\Trading\Workspaces\workspace1\Portfolios\
```

**Source** (simplified):
```csharp
public class WorkspaceTypesConfigurator : IWorkspaceServiceConfigurator
{
    public WorkspaceConfiguration Options { get; }
    public IServiceProvider ServiceProvider { get; }

    public async ValueTask ConfigureWorkspaceServices(
        IServiceCollection services,
        UserWorkspacesService userWorkspacesService,
        string? workspaceId)
    {
        if (userWorkspacesService.UserWorkspaces == null) return;

        var workspaceReference = userWorkspacesService.UserWorkspaces.GetChild(workspaceId);

        await WorkspaceSchemas.InitFilesystemSchemas(userWorkspacesService.UserWorkspaces);

        var dir = new DirectoryReferenceSelector(userWorkspacesService.UserWorkspaces) { Recursive = true };

        // For each registered member type
        foreach (var type in Options.MemberTypes)
        {
            // Register IObservableReader/Writer for this type
            var method = typeof(FsObservableCollectionFactoryX)
                .GetMethod(nameof(FsObservableCollectionFactoryX.RegisterObservablesInSubDirForType))!;
            var genericMethod = method.MakeGenericMethod(type);
            genericMethod.Invoke(this, [services, ServiceProvider, dir.Path, null]);
        }
    }
}
```

---

## Document Service Pattern

### DirectoryWorkspaceDocumentService\<TValue\>

**Purpose**: Hosted service that watches workspace documents and invokes runners when documents change.

**Inheritance**: `WorkspaceDocumentService<string, TValue>` → `IHostedService`

**What It Does**:
1. Subscribes to `IObservableReader<string, TValue>.Values.Connect()`
2. When documents are added/modified/removed:
   - Finds registered `IWorkspaceDocumentRunner<string, TValue>` instances
   - Creates runner instances per document key
   - Calls `runner.OnNext(entity)` to notify of changes

**Registration**:
```csharp
services.AddWorkspaceDocumentService<string, BotEntity>();

// Internally registers:
services.AddHostedSingleton<DirectoryWorkspaceDocumentService<BotEntity>>();
```

**Hardcoded Directory** (temporary):
```csharp
static string dir = "C:\\ProgramData\\LionFire\\Trading\\Users"; // TEMP HARDCODE
```

**Note**: Currently uses a hardcoded path - should be refactored to use workspace directories.

---

### WorkspaceDocumentService\<TKey, TValue\> (Base Class)

**Purpose**: Base class for document services, manages runner lifecycle.

**Key Features**:
- Subscribes to `IObservableReader.Values.Connect()` on startup
- Maintains `ConcurrentDictionary<TKey, IObserver<TValue>>` per runner type
- Creates runner instances on-demand using `ActivatorUtilities`
- Calls `runner.OnNext(value)` when documents change
- Disposes subscriptions on shutdown

**Flow**:
```
Document Added/Modified (bot1.hjson)
    ↓
IObservableReader.Values emits changeset
    ↓
WorkspaceDocumentService.OnValue(key, entity)
    ↓
For each IWorkspaceDocumentRunner<string, BotEntity>:
    Get or create runner instance for "bot1"
    Call runner.OnNext(entity)
```

---

### IWorkspaceDocumentRunner\<TKey, TValue\>

**Purpose**: Marker interface for runner services that react to document changes.

```csharp
public interface IWorkspaceDocumentRunner<TKey, TValue>
{
    Type RunnerType { get; }
}
```

**Implementation Pattern**:
```csharp
public class BotRunner :
    IWorkspaceDocumentRunner<string, BotEntity>,
    IObserver<BotEntity>
{
    public Type RunnerType => typeof(BotRunner);

    public void OnNext(BotEntity bot)
    {
        // React to bot document changes
        if (bot.Enabled)
            StartBot(bot);
        else
            StopBot(bot);
    }

    public void OnError(Exception error) { }
    public void OnCompleted() { }
}
```

**Registration**:
```csharp
services.TryAddEnumerable(
    ServiceDescriptor.Singleton<IWorkspaceDocumentRunner<string, BotEntity>, BotRunner>()
);
```

**Result**: When bot HJSON files are created/modified, `BotRunner.OnNext()` is automatically called.

---

### WorkspaceDocumentRunner\<TValue, TRunner\>

**Purpose**: Base class helper for implementing runners (optional convenience).

---

## Schemas and Conventions

### WorkspaceSchemas

**Purpose**: Defines directory schemas for workspace organization.

**What It Does**:
- Defines schema for workspace directory structure
- Initializes schema metadata files in filesystem
- Ensures proper directory conventions

**Usage**:
```csharp
// Called when workspace services are configured
await WorkspaceSchemas.InitFilesystemSchemas(userWorkspacesService.UserWorkspaces);

// Creates schema files in workspace directory
// Defines conventions for workspace structure
```

**Schema Namespace**: `https://schemas.lionfire.ca/2025/ui`

---

## Hosting Extensions

### WorkspacesHostingX

**Methods**:
```csharp
public static IServiceCollection AddWorkspacesModel(this IServiceCollection services)
```

**Purpose**: Registers workspace model types (currently minimal).

**Usage**:
```csharp
services.AddWorkspacesModel();
```

---

### WorkspaceDocumentServiceX

**Methods**:
```csharp
public static IServiceCollection AddWorkspaceDocumentService<TKey, TValue>(
    this IServiceCollection services)
    where TKey : notnull
    where TValue : notnull
```

**Purpose**: Registers `DirectoryWorkspaceDocumentService<TValue>` as hosted service.

**Usage**:
```csharp
services
    .AddWorkspaceChildType<BotEntity>()                     // Declare type
    .AddWorkspaceDocumentService<string, BotEntity>();      // Register service
```

**What It Does**:
```csharp
services.AddHostedSingleton<DirectoryWorkspaceDocumentService<TValue>>();
```

---

## Complete Setup Example

### 1. Application Startup (Program.cs)

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    // Basic workspace infrastructure
    .AddWorkspacesModel()

    // Declare document types
    .AddWorkspaceChildType<BotEntity>()
    .AddWorkspaceChildType<Portfolio>()

    // Register document services (hosted services)
    .AddWorkspaceDocumentService<string, BotEntity>()
    .AddWorkspaceDocumentService<string, Portfolio>()

    // Register workspace configurators
    .AddSingleton<IWorkspaceServiceConfigurator, WorkspaceTypesConfigurator>()

    // Optional: Register runners
    .TryAddEnumerable(ServiceDescriptor.Singleton<
        IWorkspaceDocumentRunner<string, BotEntity>, BotRunner>());
```

### 2. UI Layer (WorkspaceLayoutVM or Similar)

```csharp
// Set workspace base directory
userWorkspacesService.UserWorkspacesDir = "C:\\Users\\Alice\\Trading\\Workspaces";

// Create workspace-scoped services
var workspaceServices = new ServiceCollection();

// Invoke all configurators
foreach (var configurator in serviceProvider.GetServices<IWorkspaceServiceConfigurator>())
{
    await configurator.ConfigureWorkspaceServices(
        workspaceServices,
        userWorkspacesService,
        "workspace1"
    );
}

// Build workspace service provider
var workspaceProvider = workspaceServices.BuildServiceProvider();

// Now workspace provider has:
//   IObservableReader<string, BotEntity>    -> Bots/ subdirectory
//   IObservableWriter<string, BotEntity>    -> Bots/ subdirectory
//   IObservableReader<string, Portfolio>    -> Portfolios/ subdirectory
//   IObservableWriter<string, Portfolio>    -> Portfolios/ subdirectory
```

### 3. Blazor Component Usage

```razor
<CascadingValue Name="WorkspaceServices" Value="@WorkspaceProvider">
    @Body
</CascadingValue>

@code {
    public IServiceProvider? WorkspaceProvider { get; set; }
}
```

### 4. Component Consumes Workspace Services

```razor
@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    protected override void OnInitialized()
    {
        var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
        // Use reader to access bots in this workspace
    }
}
```

---

## Directory Structure

When set up, workspace directories look like:

```
C:\Users\Alice\Trading\Workspaces\
├── workspace1.hjson              ← Workspace metadata
├── workspace1\                   ← Workspace directory
│   ├── Bots\
│   │   ├── bot-alpha.hjson
│   │   └── bot-beta.hjson
│   ├── Portfolios\
│   │   └── portfolio1.hjson
│   └── Strategies\
│       └── strategy1.hjson
└── workspace2.hjson              ← Another workspace
    └── workspace2\
        ├── Bots\
        └── Portfolios\
```

---

## Key Design Patterns

### 1. Service Scoping

Each workspace gets isolated services:
```
Root Container
├── UserWorkspacesService (singleton)
├── IWorkspaceServiceConfigurator[] (collection)
└── DirectoryWorkspaceDocumentService<T> (hosted service)

Workspace Container (per workspace)
├── IObservableReader<string, BotEntity>    ← Workspace-specific
├── IObservableWriter<string, BotEntity>    ← Workspace-specific
└── ... (other member types)
```

### 2. Configurator Pattern

Extensible configuration via `IWorkspaceServiceConfigurator`:
- Core framework provides `WorkspaceTypesConfigurator`
- Applications can add custom configurators
- All invoked when workspace services are built

### 3. Observer Pattern (Runners)

Document changes trigger runners:
```
Document Change → IObservableReader → WorkspaceDocumentService → Runner.OnNext()
```

### 4. Hosted Service Pattern

`DirectoryWorkspaceDocumentService<T>` runs as background service:
- Starts when application starts
- Subscribes to document changes
- Manages runner lifecycle
- Stops when application stops

---

## Dependencies

### NuGet Packages

```xml
<PackageReference Include="Microsoft.Extensions.Hosting" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" />
<PackageReference Include="Microsoft.Extensions.Options" />
```

### LionFire Dependencies

```xml
<ProjectReference Include="..\LionFire.Workspaces.Abstractions\" />
<ProjectReference Include="..\LionFire.Reactive\" />
<ProjectReference Include="..\LionFire.IO.Reactive.Hjson\" />
<ProjectReference Include="..\LionFire.Referencing\" />
<ProjectReference Include="..\LionFire.FlexObjects\" />
<ProjectReference Include="..\LionFire.Schemas\" />
```

---

## Related Documentation

- **[Workspace Architecture](../../../docs/architecture/workspaces/README.md)** - High-level overview
- **[Service Scoping Deep Dive](../../../docs/architecture/workspaces/service-scoping.md)** - DI and scoping details
- **[LionFire.Workspaces.Abstractions](../LionFire.Workspaces.Abstractions/CLAUDE.md)** - Interfaces
- **[LionFire.Workspaces.UI.Blazor](../LionFire.Workspaces.UI.Blazor/CLAUDE.md)** - Blazor UI components
- **[LionFire.Reactive](../LionFire.Reactive/CLAUDE.md)** - Observable readers/writers

---

## Summary

**LionFire.Workspaces** provides the **core workspace infrastructure**:

✅ **Service Scoping**: Workspace-scoped service providers
✅ **Configuration**: Extensible configurator pattern
✅ **Document Services**: Hosted services watching for changes
✅ **Runner Pattern**: React to document changes with lifecycle management
✅ **Directory Conventions**: HJSON file-based storage with subdirectories per type

**Use This Library To**: Build applications where users organize documents into isolated workspaces with file-based persistence and reactive updates.
