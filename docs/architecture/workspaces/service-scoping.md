# Workspace-Scoped Services

**Why This Matters**: Understanding workspace service scoping prevents a common DI injection error that manifests as `InvalidOperationException: Unable to resolve service for type 'IObservableReader<TKey, TValue>'`. This document explains why workspace services are scoped separately from root services and how to use them correctly.

---

## Table of Contents

1. [The Problem](#the-problem)
2. [Why Workspace Scoping Exists](#why-workspace-scoping-exists)
3. [The Architecture](#the-architecture)
4. [Service Registration Flow](#service-registration-flow)
5. [Using Workspace Services in Blazor](#using-workspace-services-in-blazor)
6. [Common Pitfalls](#common-pitfalls)
7. [Complete Example](#complete-example)

---

## The Problem

### Symptom

You create a Blazor page that uses `ObservableReaderWriterItemVM<TKey, TValue, TVM>` and get this error:

```
InvalidOperationException: Unable to resolve service for type
'LionFire.Reactive.Persistence.IObservableReader`2[System.String,MyApp.MyEntity]'
while attempting to activate
'LionFire.Mvvm.ObservableReaderWriterItemVM`3[System.String,MyApp.MyEntity,MyApp.MyEntityVM]'.
```

### What It Means

The VM's constructor needs `IObservableReader<TKey, TValue>` and `IObservableWriter<TKey, TValue>`, but these services **don't exist in the root DI container**. They only exist in **workspace-scoped** service providers.

### Real-World Example

```csharp
// ❌ THIS FAILS - tries to inject from root container
@page "/bots/{BotId}"
@inherits ReactiveInjectableComponentBase<ObservableReaderWriterItemVM<string, BotEntity, BotVM>>

@code {
    // This component tries to inject the VM from root DI
    // But IObservableReader/Writer are only in workspace scope!
}
```

---

## Why Workspace Scoping Exists

### 1. Workspace Isolation

Each workspace is an **independent data container** with its own:
- File system directory
- Document instances
- Service registrations
- Observable readers/writers

**Example**: User has two workspaces:
- `/Users/Alice/Work` - work projects
- `/Users/Alice/Personal` - personal projects

Each workspace needs its own `IObservableReader<string, BotEntity>` pointing to different directories. If these were singleton services in the root container, **they couldn't point to different locations**.

### 2. Multi-Workspace Support

Applications can have multiple workspaces **open simultaneously**. Each needs isolated services:

```csharp
// Workspace 1: reads from C:\Work\Bots\
var workspace1Reader = workspace1Services.GetService<IObservableReader<string, BotEntity>>();

// Workspace 2: reads from C:\Personal\Bots\
var workspace2Reader = workspace2Services.GetService<IObservableReader<string, BotEntity>>();

// These MUST be different instances pointing to different directories!
```

### 3. Lifecycle Management

Workspace services are created when the workspace loads and disposed when it closes. This properly manages:
- File watchers
- Observable subscriptions
- Cache cleanup
- Background tasks

### 4. Configuration Flexibility

Different workspaces can have different configurations:
- Different serialization formats
- Different document types enabled
- Different persistence strategies

---

## The Architecture

### Service Provider Hierarchy

```
┌─────────────────────────────────────┐
│ Root Service Provider (Singleton)   │
│  - Application services             │
│  - Configuration                    │
│  - Logging                          │
│  - UserWorkspacesService            │
└────────────┬────────────────────────┘
             │
             │ Creates scoped providers
             ├──────────────────────┬──────────────────────┐
             │                      │                      │
    ┌────────▼────────┐   ┌────────▼────────┐   ┌────────▼────────┐
    │ Workspace 1     │   │ Workspace 2     │   │ Workspace 3     │
    │ Services        │   │ Services        │   │ Services        │
    │                 │   │                 │   │                 │
    │ - IObservable   │   │ - IObservable   │   │ - IObservable   │
    │   Reader/Writer │   │   Reader/Writer │   │   Reader/Writer │
    │   for this      │   │   for this      │   │   for this      │
    │   workspace's   │   │   workspace's   │   │   workspace's   │
    │   directory     │   │   directory     │   │   directory     │
    └─────────────────┘   └─────────────────┘   └─────────────────┘
```

### Key Classes

1. **`IWorkspaceServiceConfigurator`** - Configures services for a workspace
2. **`UserWorkspacesService`** - Manages workspace lifecycle
3. **`DirectoryWorkspaceDocumentService<TValue>`** - Creates reader/writer for document types
4. **`RegisterObservablesInDir<T>`** - Registers `IObservableReader/Writer<string, T>` for a directory

---

## Service Registration Flow

### Phase 1: Root Services (Application Startup)

In `Program.cs` or similar:

```csharp
services
    // Register workspace infrastructure
    .AddWorkspaces(configuration)

    // Declare document types that workspaces can contain
    .AddWorkspaceChildType<BotEntity>()
    .AddWorkspaceChildType<Portfolio>()

    // Register document service infrastructure
    .AddWorkspaceDocumentService<string, BotEntity>();
```

**What happens**:
- `AddWorkspaceChildType<T>` adds type to workspace's `MemberTypes` list
- `AddWorkspaceDocumentService<T>` registers `DirectoryWorkspaceDocumentService<T>` as hosted service
- NO `IObservableReader/Writer` registered yet!

### Phase 2: Workspace Creation (Runtime)

When a workspace is loaded:

```csharp
// UserWorkspacesService creates a workspace
var workspaceServices = new ServiceCollection();

// Calls all registered IWorkspaceServiceConfigurator instances
foreach (var configurator in rootServices.GetServices<IWorkspaceServiceConfigurator>())
{
    await configurator.ConfigureWorkspaceServices(workspaceServices, userWorkspaceService, workspaceId);
}

// Build the workspace-scoped provider
var workspaceProvider = workspaceServices.BuildServiceProvider();
```

**What happens** in `WorkspaceTypesConfigurator.ConfigureWorkspaceServices()`:

```csharp
public async ValueTask ConfigureWorkspaceServices(
    IServiceCollection services,
    UserWorkspacesService userWorkspacesService,
    string? workspaceId)
{
    // Get workspace directory (e.g., C:\Users\Alice\Work\)
    var workspaceReference = userWorkspacesService.UserWorkspaces.GetChild(workspaceId);

    var dirSelector = new DirectoryReferenceSelector(workspaceReference) { Recursive = true };

    // For each registered document type (BotEntity, Portfolio, etc.)
    foreach (var type in Options.MemberTypes)
    {
        // Call RegisterObservablesInSubDirForType<BotEntity>
        // This creates IObservableReader/Writer<string, BotEntity>
        // pointing to C:\Users\Alice\Work\Bots\
        services.RegisterObservablesInSubDirForType(
            type,
            ServiceProvider,
            workspaceReference,
            entitySubdir: null, // Uses plural type name (e.g., "Bots")
            recursive: true
        );
    }
}
```

### Phase 3: Service Usage

Now the workspace-scoped provider has:

```csharp
// These NOW exist in workspace services (but not root!)
IObservableReader<string, BotEntity>
IObservableWriter<string, BotEntity>
IObservableReaderWriter<string, BotEntity> // Composite of above
```

These are **singleton within the workspace scope**, created by:

```csharp
// From FsObservableCollectionFactoryX.RegisterObservablesInDir<T>
var reader = new HjsonFsDirectoryReaderRx<string, T>(dirSelector, options);
var writer = new HjsonFsDirectoryWriterRx<string, T>(dirSelector, options);
var composite = new ObservableReaderWriterFromComponents<string, T>(reader, writer);

services.AddSingleton<IObservableReaderWriter<string, T>>(composite);
```

---

## Using Workspace Services in Blazor

### The Right Way: CascadingParameter

Blazor components receive workspace services via **cascading parameter**:

```razor
@page "/bots/{BotId}"
@using LionFire.Mvvm
@using LionFire.Trading.Automation
@using LionFire.Reactive.Persistence
@using Microsoft.Extensions.DependencyInjection

@code {
    [Parameter]
    public string? BotId { get; set; }

    // ✅ Get workspace services via cascading parameter
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    private ObservableReaderWriterItemVM<string, BotEntity, BotVM>? VM { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (WorkspaceServices == null)
            throw new InvalidOperationException("WorkspaceServices not available");

        // ✅ Resolve services from workspace scope
        var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
        var writer = WorkspaceServices.GetService<IObservableWriter<string, BotEntity>>();

        if (reader == null || writer == null)
            throw new InvalidOperationException("Bot persistence services not registered");

        // ✅ Create VM manually with workspace-scoped services
        VM = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);
        VM.Id = BotId;

        await base.OnParametersSetAsync();
    }
}
```

### How WorkspaceServices Gets Set

The workspace layout component provides it:

```razor
<!-- In WorkspaceLayout.razor or similar -->
<CascadingValue Name="WorkspaceServices" Value="@WorkspaceServiceProvider">
    <CascadingValue Name="WorkspaceId" Value="@WorkspaceId">
        @Body
    </CascadingValue>
</CascadingValue>

@code {
    public IServiceProvider? WorkspaceServiceProvider { get; set; }
    public string? WorkspaceId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Get workspace-specific service provider
        WorkspaceServiceProvider = await UserWorkspacesService
            .GetWorkspaceServicesAsync(WorkspaceId);
    }
}
```

---

## Common Pitfalls

### ❌ Pitfall 1: Injecting from Root Container

```razor
@page "/bots/{BotId}"
@inherits ReactiveInjectableComponentBase<ObservableReaderWriterItemVM<string, BotEntity, BotVM>>

@code {
    // ❌ WRONG: Component base tries to inject VM from root container
    // But IObservableReader/Writer are only in workspace scope!
}
```

**Why it fails**: `ReactiveInjectableComponentBase` uses `@inject` which resolves from the root container, not workspace scope.

**Solution**: Use CascadingParameter and manual resolution (see above).

### ❌ Pitfall 2: Registering in Root Container

```csharp
// ❌ WRONG: Trying to register in root
services.AddSingleton<IObservableReader<string, BotEntity>>(sp => {
    var dir = "C:\\HardcodedPath\\Bots";
    return new HjsonFsDirectoryReaderRx<string, BotEntity>(sp, new DirectorySelector(dir));
});
```

**Why it fails**:
1. Breaks workspace isolation - all workspaces share one reader
2. Can't support multiple workspaces
3. Hardcoded path doesn't work for different users/machines

**Solution**: Let workspace configuration create readers per workspace.

### ❌ Pitfall 3: Forgetting AddWorkspaceChildType

```csharp
// ❌ WRONG: Registered document service but didn't add to member types
services
    .AddWorkspaceDocumentService<string, BotEntity>();  // This runs
    // Missing: .AddWorkspaceChildType<BotEntity>()
```

**Why it fails**: `WorkspaceTypesConfigurator` only registers readers/writers for types in `MemberTypes`. If you don't call `AddWorkspaceChildType<T>()`, the type won't be configured.

**Solution**:

```csharp
services
    .AddWorkspaceChildType<BotEntity>()              // ✅ Declares member type
    .AddWorkspaceDocumentService<string, BotEntity>(); // ✅ Registers service
```

### ❌ Pitfall 4: Using ObservableDataView Without DataServiceProvider

```razor
<!-- ❌ WRONG: ObservableDataView without DataServiceProvider -->
<ObservableDataView TKey="string" TValue="BotEntity" TValueVM="BotVM" />
```

**Why it might fail**: Component can't find workspace services.

**Solution**: Pass workspace services explicitly:

```razor
<!-- ✅ RIGHT: Explicitly provide workspace services -->
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices" />

@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }
}
```

---

## Complete Example

### 1. Define Entity and VM

```csharp
// Entity: The persisted data
[Alias("Bot")]
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;
    [Reactive] private string? _description;
}

// ViewModel: UI-friendly wrapper
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value) { }
}
```

### 2. Register in Root Services

```csharp
// In Program.cs or Startup.cs
services
    .AddWorkspaces(configuration)
    .AddWorkspaceChildType<BotEntity>()                     // Declare type
    .AddWorkspaceDocumentService<string, BotEntity>()       // Document service
    .AddReactivePersistenceMvvm();                          // VM support
```

### 3. Create List View (Bots.razor)

```razor
@page "/bots"
@using LionFire.Trading.Automation

<h3>Bots</h3>

<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices"
                    AllowedEditModes="EditMode.All">
    <Columns>
        <PropertyColumn Property="x => x.Value.Name" Title="Name" />
        <PropertyColumn Property="x => x.Value.Description" Title="Description" />
        <TemplateColumn>
            <CellTemplate>
                <MudButton Href="@($"/bots/{context.Item.Key}")">Edit</MudButton>
            </CellTemplate>
        </TemplateColumn>
    </Columns>
</ObservableDataView>

@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }
}
```

### 4. Create Detail View (Bot.razor)

```razor
@page "/bots/{BotId}"
@using LionFire.Mvvm
@using LionFire.Trading.Automation
@using LionFire.Reactive.Persistence
@using Microsoft.Extensions.DependencyInjection
@inject ILogger<Bot> Logger

<h3>Bot @BotId</h3>

@if (VM?.Value != null)
{
    <MudTextField @bind-Value="VM.Value.Name" Label="Name" />
    <MudTextField @bind-Value="VM.Value.Description" Label="Description" />
    <MudButton OnClick="Save" Color="Color.Primary">Save</MudButton>
}
else
{
    <MudProgressCircular Indeterminate="true" />
}

@code {
    [Parameter]
    public string? BotId { get; set; }

    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    private ObservableReaderWriterItemVM<string, BotEntity, BotVM>? VM { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (WorkspaceServices == null)
        {
            Logger.LogError("WorkspaceServices not available");
            return;
        }

        // Resolve workspace-scoped services
        var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
        var writer = WorkspaceServices.GetService<IObservableWriter<string, BotEntity>>();

        if (reader == null || writer == null)
        {
            Logger.LogError("Bot persistence services not registered");
            return;
        }

        // Create VM with workspace services
        VM = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);
        VM.Id = BotId;

        await base.OnParametersSetAsync();
    }

    private async Task Save()
    {
        if (VM?.Value != null)
        {
            await VM.Write();
        }
    }
}
```

---

## Summary

### Key Principles

1. **Workspace services are scoped** to support isolation and multi-workspace scenarios
2. **`IObservableReader/Writer` live in workspace scope**, not root scope
3. **Use CascadingParameter** to access workspace services in Blazor
4. **Resolve services manually** when creating VMs for single-item views
5. **ObservableDataView handles it automatically** for list views via DataServiceProvider

### Decision Tree

**Need to display a list of workspace documents?**
→ Use `ObservableDataView` with `DataServiceProvider="@WorkspaceServices"`

**Need to display/edit a single workspace document?**
→ Use CascadingParameter + manual `IObservableReader/Writer` resolution

**Getting `Unable to resolve service` errors?**
→ Check if you're trying to inject workspace-scoped services from root container

**Workspace services not found?**
→ Verify `AddWorkspaceChildType<T>()` was called for your entity type

---

## Related Documentation

- [Workspace Architecture Overview](README.md)
- [Blazor MVVM Patterns](../../ui/blazor-mvvm-patterns.md)
- [How-To: Create Blazor Workspace Page](../../guides/how-to/create-blazor-workspace-page.md)
- [LionFire.Workspaces CLAUDE.md](../../../src/LionFire.Workspaces/CLAUDE.md)
- [LionFire.Blazor.Components.MudBlazor CLAUDE.md](../../../src/LionFire.Blazor.Components.MudBlazor/CLAUDE.md)
