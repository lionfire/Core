# Workspaces Domain Documentation

**Overview**: Comprehensive guide to the LionFire Workspaces ecosystem - a system for organizing user-centric, file-backed documents with isolated service scopes and reactive updates.

---

## Table of Contents

1. [What Are Workspaces?](#what-are-workspaces)
2. [When to Use Workspaces](#when-to-use-workspaces)
3. [Library Ecosystem](#library-ecosystem)
4. [Quick Start](#quick-start)
5. [Common Scenarios](#common-scenarios)
6. [Best Practices](#best-practices)
7. [Troubleshooting](#troubleshooting)

---

## What Are Workspaces?

### Definition

**Workspaces** are user-centric, directory-backed containers for heterogeneous documents. Think of them as "project folders" that:
- Store multiple types of related documents (Bots, Portfolios, Strategies, etc.)
- Provide isolated service scopes (each workspace has its own `IObservableReader/Writer` instances)
- Persist as HJSON files in subdirectories
- Support reactive updates (file changes automatically propagate to UI)
- Enable multi-workspace scenarios (multiple workspaces can be open simultaneously)

### Real-World Analogy

Like Visual Studio **Solutions** or IntelliJ **Projects**, but:
- More flexible (any document types, not just code)
- File-based (human-readable HJSON, version-controllable)
- Service-scoped (isolated data access per workspace)
- UI-integrated (common workspace management components)

### Example Structure

```
C:\Users\Alice\Trading\Workspaces\
├── DayTrading\                         ← Workspace 1
│   ├── Bots\
│   │   ├── scalper-btc.hjson
│   │   └── momentum-eth.hjson
│   ├── Portfolios\
│   │   └── main-portfolio.hjson
│   └── Strategies\
│       └── mean-reversion.hjson
└── SwingTrading\                       ← Workspace 2
    ├── Bots\
    │   └── swing-bot.hjson
    └── Portfolios\
        └── swing-portfolio.hjson
```

---

## When to Use Workspaces

### ✅ Use Workspaces When:

1. **User Organization**: Users need to group related documents into "projects" or "contexts"
2. **Multiple Document Types**: Application has several entity types that belong together (not just one type)
3. **File-Based Storage**: Want human-readable files that can be version-controlled
4. **Isolation**: Different workspaces should have independent data and services
5. **Multi-Workspace**: Users might work with multiple workspaces (switching or multiple open)
6. **UI Integration**: Want standard workspace management UI (selector, properties, etc.)

**Examples**:
- **Trading Application**: Workspaces for different trading strategies, each with bots, portfolios, and configurations
- **Content Management**: Workspaces for different projects, each with articles, media, and settings
- **Game Development**: Workspaces for different game projects, each with levels, assets, and configurations
- **Data Analysis**: Workspaces for different analyses, each with datasets, notebooks, and results

### ❌ Don't Use Workspaces When:

1. **Single Global Store**: Application has one shared data context for all users
2. **Database-Only**: All data lives in a database (consider Ided/Assets pattern instead)
3. **No User Organization**: Data organization is purely technical, not user-driven
4. **Simple Config Files**: Just need to read/write a few config files (use `IObservableReader` directly)
5. **No Isolation Needed**: All contexts share the same data

**Alternatives**:
| Need | Alternative | Example |
|------|-------------|---------|
| Database entities | **Ided/Assets** | Game inventory items |
| Complex virtual filesystem | **VOS** | Overlay configs from multiple sources |
| Single config file | **Direct IObservableReader** | App settings |
| Custom storage backend | **Persistence Layer** | MongoDB, Redis |

---

## Library Ecosystem

### Core Libraries

```
┌──────────────────────────────────────────────────────┐
│ LionFire.Workspaces.Abstractions                    │
│ - IWorkspace, IWorkspaceServiceConfigurator         │
│ - UserWorkspacesService                             │
└─────────────────┬────────────────────────────────────┘
                  │
                  ↓ implements
┌──────────────────────────────────────────────────────┐
│ LionFire.Workspaces                                  │
│ - WorkspaceTypesConfigurator                        │
│ - DirectoryWorkspaceDocumentService                 │
│ - WorkspaceDocumentRunner pattern                   │
└─────────────────┬────────────────────────────────────┘
                  │
                  ↓ used by
┌──────────────────────────────────────────────────────┐
│ LionFire.Workspaces.UI                              │
│ - WorkspaceGridVM (ViewModel)                       │
└─────────────────┬────────────────────────────────────┘
                  │
                  ↓ used by
┌──────────────────────────────────────────────────────┐
│ LionFire.Workspaces.UI.Blazor                       │
│ - WorkspaceLayoutVM (provides WorkspaceServices)    │
│ - WorkspaceSelector, WorkspaceGrid                  │
└──────────────────────────────────────────────────────┘
```

### Related Libraries

- **LionFire.Reactive**: Provides `IObservableReader/Writer` interfaces
- **LionFire.IO.Reactive.Hjson**: HJSON file-based implementations
- **LionFire.Data.Async.Mvvm**: ViewModels for workspace documents
- **LionFire.Blazor.Components.MudBlazor**: `ObservableDataView` component

### Documentation

- **[Workspace Architecture](../architecture/workspaces/README.md)** - High-level design
- **[Service Scoping Deep Dive](../architecture/workspaces/service-scoping.md)** - Critical for understanding DI
- **[Blazor MVVM Patterns](../ui/blazor-mvvm-patterns.md)** - UI patterns
- **Library References**:
  - [LionFire.Workspaces](../../src/LionFire.Workspaces/CLAUDE.md)
  - [LionFire.Workspaces.Abstractions](../../src/LionFire.Workspaces.Abstractions/CLAUDE.md)

---

## Quick Start

### 1. Install Packages

```bash
dotnet add package LionFire.Workspaces
dotnet add package LionFire.Workspaces.UI.Blazor  # If using Blazor
dotnet add package LionFire.Blazor.Components.MudBlazor  # For ObservableDataView
```

### 2. Define Entity and ViewModel

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

### 3. Register in Application Startup

```csharp
// In Program.cs
builder.Services
    // Core workspace infrastructure
    .AddWorkspacesModel()

    // Declare document types
    .AddWorkspaceChildType<BotEntity>()
    .AddWorkspaceChildType<Portfolio>()

    // Register document services
    .AddWorkspaceDocumentService<string, BotEntity>()
    .AddWorkspaceDocumentService<string, Portfolio>()

    // Register configurator
    .AddSingleton<IWorkspaceServiceConfigurator, WorkspaceTypesConfigurator>();
```

### 4. Create Blazor Pages

**List Page** (Bots.razor):
```razor
@page "/bots"

<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices">
    <Columns>
        <PropertyColumn Property="x => x.Value.Name" />
        <PropertyColumn Property="x => x.Value.Description" />
    </Columns>
</ObservableDataView>

@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }
}
```

**Detail Page** (Bot.razor):
```razor
@page "/bots/{BotId}"
@using Microsoft.Extensions.DependencyInjection

<MudTextField @bind-Value="VM.Value.Name" Label="Name" />
<MudButton OnClick="Save">Save</MudButton>

@code {
    [Parameter]
    public string? BotId { get; set; }

    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    private ObservableReaderWriterItemVM<string, BotEntity, BotVM>? VM { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
        var writer = WorkspaceServices.GetService<IObservableWriter<string, BotEntity>>();

        VM = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);
        VM.Id = BotId;
    }

    private async Task Save() => await VM.Write();
}
```

### 5. Run and Use

1. Application creates workspace directory (e.g., `C:\ProgramData\MyApp\Users\Alice\Workspaces\workspace1\`)
2. User creates bots via UI ("Add" button in ObservableDataView)
3. Files created: `workspace1\Bots\bot-alpha.hjson`
4. User edits bot via detail page
5. Changes saved to file
6. File watching automatically updates UI in list page

---

## Common Scenarios

### Scenario 1: Adding a New Document Type

**Goal**: Add "Strategy" document type to existing workspace application.

**Steps**:

1. **Define Entity**:
```csharp
[Alias("Strategy")]
public partial class StrategyEntity : ReactiveObject
{
    [Reactive] private string? _name;
    [Reactive] private string? _rules;
}
```

2. **Define ViewModel**:
```csharp
public class StrategyVM : KeyValueVM<string, StrategyEntity>
{
    public StrategyVM(string key, StrategyEntity value) : base(key, value) { }
}
```

3. **Register**:
```csharp
services
    .AddWorkspaceChildType<StrategyEntity>()
    .AddWorkspaceDocumentService<string, StrategyEntity>();
```

4. **Create Pages** (same as Bots example above)

5. **Done** - Strategies now available in workspaces!

---

### Scenario 2: Custom Workspace Configuration

**Goal**: Add application-specific services to workspace scope.

**Create Custom Configurator**:
```csharp
public class MyAppWorkspaceConfigurator : IWorkspaceServiceConfigurator
{
    public async ValueTask ConfigureWorkspaceServices(
        IServiceCollection services,
        UserWorkspacesService userWorkspacesService,
        string? workspaceId)
    {
        // Add custom services
        services.AddSingleton<IMyAppService>(sp => {
            var workspaceDir = userWorkspacesService.UserWorkspaces?.GetChild(workspaceId);
            return new MyAppService(workspaceDir);
        });
    }
}

// Register
services.AddSingleton<IWorkspaceServiceConfigurator, MyAppWorkspaceConfigurator>();
```

---

### Scenario 3: Document Runner (Active Documents)

**Goal**: Automatically start/stop bots when documents change.

**Create Runner**:
```csharp
public class BotRunner :
    IWorkspaceDocumentRunner<string, BotEntity>,
    IObserver<BotEntity>
{
    public Type RunnerType => typeof(BotRunner);

    public void OnNext(BotEntity bot)
    {
        // React to bot document changes
        if (bot.Enabled && !IsRunning(bot))
            StartBot(bot);
        else if (!bot.Enabled && IsRunning(bot))
            StopBot(bot);
    }

    public void OnError(Exception error) { }
    public void OnCompleted() { }

    private bool IsRunning(BotEntity bot) { /* ... */ }
    private void StartBot(BotEntity bot) { /* ... */ }
    private void StopBot(BotEntity bot) { /* ... */ }
}
```

**Register**:
```csharp
services.TryAddEnumerable(
    ServiceDescriptor.Singleton<IWorkspaceDocumentRunner<string, BotEntity>, BotRunner>()
);
```

**Result**: When user enables bot in UI, file updates, runner automatically starts bot.

---

### Scenario 4: Multiple Workspaces

**Goal**: Allow users to switch between workspaces or have multiple open.

**UI Component**:
```razor
<!-- Workspace selector -->
<MudSelect T="string" @bind-Value="CurrentWorkspaceId" Label="Workspace">
    @foreach (var workspace in AvailableWorkspaces)
    {
        <MudSelectItem Value="@workspace">@workspace</MudSelectItem>
    }
</MudSelect>

@code {
    private string CurrentWorkspaceId { get; set; }
    private List<string> AvailableWorkspaces { get; set; }

    protected override void OnInitialized()
    {
        // Load available workspaces
        var workspacesDir = userWorkspacesService.UserWorkspacesDir;
        AvailableWorkspaces = Directory.GetDirectories(workspacesDir)
            .Select(Path.GetFileName)
            .ToList();
    }
}
```

**Service Management**:
```csharp
// When workspace changes
async Task OnWorkspaceChanged(string workspaceId)
{
    // Dispose old workspace services
    oldWorkspaceServices?.Dispose();

    // Create new workspace services
    var services = new ServiceCollection();
    foreach (var configurator in workspaceServiceConfigurators)
    {
        await configurator.ConfigureWorkspaceServices(services, userWorkspacesService, workspaceId);
    }
    WorkspaceServices = services.BuildServiceProvider();

    // Update cascading value
    StateHasChanged();
}
```

---

## Best Practices

### 1. Service Scoping

✅ **DO**: Use `CascadingParameter` for workspace services in Blazor
```razor
[CascadingParameter(Name = "WorkspaceServices")]
public IServiceProvider? WorkspaceServices { get; set; }
```

❌ **DON'T**: Try to inject workspace services via `@inject`
```razor
@inject IObservableReader<string, BotEntity> Reader  ❌ Won't work!
```

### 2. File Naming

✅ **DO**: Use kebab-case for file names
```
bot-alpha.hjson  ✅
bot_beta.hjson   ✅
BotGamma.hjson   ⚠️ Works but inconsistent
```

❌ **DON'T**: Use special characters or spaces
```
bot#1.hjson      ❌ Special characters
my bot.hjson     ❌ Spaces cause issues
```

### 3. Entity Properties

✅ **DO**: Use `ReactiveObject` and `[Reactive]` for change notifications
```csharp
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;  ✅
}
```

❌ **DON'T**: Use plain properties without change notifications
```csharp
public class BotEntity
{
    public string Name { get; set; }  ❌ UI won't update
}
```

### 4. Directory Structure

✅ **DO**: Let workspace system create subdirectories automatically
```
workspace1\
├── Bots\          ← Created automatically
├── Portfolios\    ← Created automatically
└── Strategies\    ← Created automatically
```

❌ **DON'T**: Create subdirectories manually or use non-standard names

### 5. Error Handling

✅ **DO**: Check for null services and log errors
```csharp
if (WorkspaceServices == null)
{
    Logger.LogError("WorkspaceServices not available");
    return;
}

var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
if (reader == null)
{
    Logger.LogError("Reader not registered for BotEntity");
    return;
}
```

❌ **DON'T**: Assume services are always available

---

## Troubleshooting

### Issue: "Unable to resolve service for type 'IObservableReader'"

**Symptom**: `InvalidOperationException` when trying to create VM or use workspace services.

**Causes**:
1. Trying to inject from root container instead of workspace services
2. Document type not registered with `AddWorkspaceChildType`
3. Workspace services not built yet

**Solutions**:
- ✅ Use `CascadingParameter` and resolve from `WorkspaceServices`
- ✅ Verify `AddWorkspaceChildType<T>()` was called
- ✅ Check that workspace layout provides `WorkspaceServices`

**See**: [Service Scoping Deep Dive](../architecture/workspaces/service-scoping.md)

---

### Issue: Files not appearing in UI

**Causes**:
1. Wrong directory
2. Wrong file extension
3. Serialization error
4. Not subscribed to observable

**Debugging**:
```csharp
var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
Logger.LogInformation("Keys: {Keys}", string.Join(", ", reader?.Keys.Items ?? []));
// Expected: bot-alpha, bot-beta
```

---

### Issue: Changes not saving

**Causes**:
1. Didn't call `VM.Write()`
2. File permissions issue
3. Entity not implementing `INotifyPropertyChanged`

**Solution**:
```csharp
// Ensure you call Write()
await VM.Write();

// Check file permissions on workspace directory
// Ensure entity uses ReactiveObject
```

---

## Summary

### Key Concepts

1. **Workspaces = Isolated Service Scopes** for user-organized documents
2. **File-Based Storage** with HJSON format
3. **Reactive Updates** via `IObservableReader/Writer`
4. **Extensible** via `IWorkspaceServiceConfigurator`
5. **UI-Integrated** with Blazor components

### Getting Started Checklist

- [ ] Define entity types (`ReactiveObject`)
- [ ] Define ViewModels (`KeyValueVM`)
- [ ] Register with `AddWorkspaceChildType<T>()`
- [ ] Register document services
- [ ] Create Blazor list page (`ObservableDataView`)
- [ ] Create Blazor detail page (manual VM)
- [ ] Test with workspace layout providing `WorkspaceServices`

### Next Steps

- **[How-To: Create Blazor Workspace Page](../guides/how-to/create-blazor-workspace-page.md)** - Step-by-step tutorial
- **[Document Types Deep Dive](../architecture/workspaces/document-types.md)** - Advanced document patterns
- **[Blazor MVVM Patterns](../ui/blazor-mvvm-patterns.md)** - UI implementation details
