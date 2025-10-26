# LionFire.Workspaces.UI.Blazor

## Overview

Blazor UI components for workspace management, including layout ViewModels, workspace selection, and grid-based workspace display. This library provides the UI layer for the LionFire workspace system, integrating with **MudBlazor** and **BlazorGridStack** for rich user experiences.

**Purpose**: Enable users to select, navigate, and visualize workspaces through Blazor components.

---

## Key Components

### WorkspaceLayoutVM

**Location**: `UI/Workspaces/Layouts/WorkspaceLayoutVM.cs`

**Purpose**: ViewModel managing workspace lifecycle, service scoping, and user-workspace relationships.

**Inheritance**: `UserLayoutVM` (from LionFire.Blazor.Components)

#### Responsibilities

1. **User-to-Workspace Mapping**: Sets up workspace directories based on logged-in user
2. **Workspace Service Provider Creation**: Builds workspace-scoped `IServiceProvider`
3. **Configurator Invocation**: Calls all registered `IWorkspaceServiceConfigurator` instances
4. **Service Cascading**: Provides `WorkspaceServices` to descendant components

#### Properties

```csharp
public class WorkspaceLayoutVM : UserLayoutVM
{
    // Dependencies
    public IServiceProvider ServiceProvider { get; }
    public AppInfo AppInfo { get; }
    public IEnumerable<IWorkspaceServiceConfigurator> WorkspaceServiceConfigurators { get; }

    // User Workspaces
    public string? WorkspacesDir { get; }  // Base directory for user's workspaces
    public bool WorkspacesAvailable { get; }

    // Active Workspace
    public string? WorkspaceId { get; set; }  // Currently selected workspace
    public string EffectiveWorkspaceName { get; }  // Display name

    // Workspace Services
    public IServiceProvider? WorkspaceServices { get; set; }  // Workspace-scoped provider
}
```

#### Lifecycle

```
User Logs In
    ↓
UserChanged event
    ↓
ConfigureUserServices()
    ↓ Sets UserWorkspacesDir based on user
    ↓ Registers IObservableReaderWriter<string, Workspace>
UserServices created
    ↓
User Selects Workspace
    ↓
WorkspaceId property set
    ↓
OnWorkspaceChanged()
    ↓
DoConfigureWorkspaceServices()
    ↓ Invokes all IWorkspaceServiceConfigurator instances
    ↓ Builds workspace-scoped IServiceProvider
WorkspaceServices created
    ↓ Cascaded to child components
Components receive workspace services
```

#### Usage Example

**Layout Component**:
```razor
@inherits LayoutComponentBase
@inject WorkspaceLayoutVM LayoutVM

<CascadingValue Name="UserServices" Value="@LayoutVM.UserServices">
    <CascadingValue Name="WorkspaceServices" Value="@LayoutVM.WorkspaceServices">

        <!-- Workspace Selector -->
        <WorkspaceSelector @bind-SelectedId="@LayoutVM.WorkspaceId"
                           VM="@LayoutVM" />

        <!-- Page Content -->
        @Body

    </CascadingValue>
</CascadingValue>

@code {
    protected override async Task OnInitializedAsync()
    {
        await LayoutVM.InitializeAsync();
    }
}
```

#### Workspace Directory Configuration

**Default Path** (configured automatically):
```
C:\ProgramData\{AppInfo.OrgDir}\{AppInfo.EffectiveDataDirName}\Users\{Username}\Workspaces\
```

**Example**:
```
C:\ProgramData\LionFire\Trading\Users\Alice\Workspaces\
├── workspace1.hjson       ← Workspace metadata
├── workspace1\            ← Workspace directory
│   ├── Bots\
│   └── Portfolios\
└── workspace2.hjson
    └── workspace2\
```

#### Extensibility

**Custom Workspace Path**:
```csharp
public class MyWorkspaceLayoutVM : WorkspaceLayoutVM
{
    protected override async ValueTask ConfigureUserServices(IServiceCollection services)
    {
        await base.ConfigureUserServices(services);

        // Override default path
        userWorkspacesService.UserWorkspacesDir = GetCustomWorkspacePath(EffectiveUserName);
    }

    private string GetCustomWorkspacePath(string username)
    {
        // Custom logic for workspace location
        return Path.Combine("D:\\MyWorkspaces", username);
    }
}
```

**Additional Workspace Services**:
```csharp
protected override async ValueTask ConfigureWorkspaceServices(
    IServiceCollection services,
    string? workspaceId)
{
    await base.ConfigureWorkspaceServices(services, workspaceId);

    // Register custom workspace-scoped services
    services.AddScoped<IMyWorkspaceService, MyWorkspaceService>();
}
```

---

## UI Components

### WorkspaceSelector

**Location**: `UI/Workspaces/WorkspaceSelector.razor`

**Purpose**: Dropdown selector for choosing the active workspace from available user workspaces.

#### Parameters

```csharp
[CascadingParameter(Name = "UserServices")]
public IServiceProvider? UserServices { get; set; }

[Parameter]
public string? SelectedId { get; set; }

[Parameter]
public EventCallback<string?> SelectedIdChanged { get; set; }

[Parameter]
public WorkspaceLayoutVM? VM { get; set; }
```

#### Usage

```razor
<WorkspaceSelector @bind-SelectedId="@currentWorkspaceId" />

@code {
    string? currentWorkspaceId;
}
```

#### Features

- **Automatic Workspace Discovery**: Reads available workspaces from `UserServices`
- **Two-Way Binding**: Updates `SelectedId` when user changes selection
- **Reactive Updates**: Automatically updates when workspaces are added/removed
- **MudBlazor Integration**: Uses `MudSelect` component

#### Implementation Details

```csharp
// Resolves workspace reader from UserServices
Workspaces = UserServices?.GetRequiredService<IObservableReaderWriter<string, Workspace>>();

// Displays all workspace keys
foreach (var key in Workspaces.Keys.Keys)
{
    <MudSelectItem Value="@key">@key</MudSelectItem>
}

// Fires SelectedIdChanged event
private string? internalSelectedId
{
    set
    {
        currentWorkspace = value;
        await SelectedIdChanged.InvokeAsync(currentWorkspace);
    }
}
```

#### Auto-Selection

If no workspace selected and workspaces available, automatically selects first workspace:

```csharp
protected override async Task OnParametersSetAsync()
{
    if (SelectedId == null && Workspaces?.Keys.Count > 0)
    {
        internalSelectedId = Workspaces.Keys.Keys.First();
    }
}
```

---

### WorkspaceGrid

**Location**: `UI/Workspaces/WorkspaceGrid.razor`

**Purpose**: Grid-based layout for displaying workspace widgets using BlazorGridStack.

**Status**: Experimental / Work in Progress

#### Features

- **Grid-Based Layout**: Uses BlazorGridStack for draggable, resizable widgets
- **Dynamic Widgets**: Add widgets at runtime
- **Persistent Layout**: (Future) Save/restore grid layout

#### Usage

```razor
@page "/workspaces/{WorkspaceName}"

<WorkspaceGrid WorkspaceName="@WorkspaceName" />
```

#### Current Implementation

```razor
<BlazorGridStackBody GridStackOptions="@(new() { Class = "DashboardGrid" })">
    <BlazorGridStackWidget WidgetOptions="@(new() {X = 3, Y = 2})">
        <div class="tile">
            <!-- Widget content -->
        </div>
    </BlazorGridStackWidget>
</BlazorGridStackBody>
```

**Note**: This component is experimental and may undergo significant changes.

---

### WorkspaceNavMenu

**Location**: `UI/Workspaces/WorkspaceNavMenu.razor`

**Purpose**: Navigation menu for workspace-related routes.

**Typical Usage**: Sidebar navigation in workspace layout.

---

### Workspaces.razor

**Location**: `UI/Workspaces/Workspaces.razor`

**Purpose**: Main workspaces page/component.

---

### DocumentSelector

**Location**: `UI/Documents/DocumentSelector.razor`

**Purpose**: Selector component for choosing documents within a workspace.

---

## Integration Patterns

### Pattern 1: Full Workspace Layout

**Complete layout with user and workspace scoping:**

```razor
<!-- MainLayout.razor -->
@inherits LayoutComponentBase
@inject WorkspaceLayoutVM LayoutVM
@inject AuthenticationStateProvider AuthProvider

<MudThemeProvider />
<MudDialogProvider />
<MudSnackbarProvider />

<MudLayout>
    <!-- Top Bar -->
    <MudAppBar Elevation="1">
        <MudText Typo="Typo.h6">My Application</MudText>
        <MudSpacer />
        <WorkspaceSelector @bind-SelectedId="@LayoutVM.WorkspaceId"
                           VM="@LayoutVM" />
    </MudAppBar>

    <!-- Sidebar -->
    <MudDrawer Open="true" Elevation="1">
        <WorkspaceNavMenu />
    </MudDrawer>

    <!-- Main Content -->
    <MudMainContent Class="pa-4">
        <CascadingValue Name="UserServices" Value="@LayoutVM.UserServices">
            <CascadingValue Name="WorkspaceServices" Value="@LayoutVM.WorkspaceServices">
                @Body
            </CascadingValue>
        </CascadingValue>
    </MudMainContent>
</MudLayout>

@code {
    protected override async Task OnInitializedAsync()
    {
        // Initialize layout VM
        await LayoutVM.InitializeAsync();
    }
}
```

---

### Pattern 2: Workspace Switcher

**Quick workspace switching in header:**

```razor
<MudMenu Icon="@Icons.Material.Filled.Workspaces" Label="@currentWorkspaceName">
    @if (workspaces != null)
    {
        foreach (var workspace in workspaces)
        {
            <MudMenuItem OnClick="@(() => SwitchWorkspace(workspace.Key))">
                @workspace.Key
            </MudMenuItem>
        }
    }
    <MudDivider />
    <MudMenuItem OnClick="@CreateNewWorkspace">
        <MudIcon Icon="@Icons.Material.Filled.Add" /> New Workspace
    </MudMenuItem>
</MudMenu>

@code {
    IEnumerable<KeyValuePair<string, Workspace>>? workspaces;

    private void SwitchWorkspace(string workspaceId)
    {
        LayoutVM.WorkspaceId = workspaceId;
    }
}
```

---

### Pattern 3: Workspace Dashboard

**Dashboard showing workspace info:**

```razor
@page "/dashboard"

<MudGrid>
    <MudItem xs="12" md="6">
        <MudCard>
            <MudCardHeader>
                <MudText Typo="Typo.h6">Active Workspace</MudText>
            </MudCardHeader>
            <MudCardContent>
                <MudText>Workspace: @LayoutVM.EffectiveWorkspaceName</MudText>
                <MudText>Path: @LayoutVM.WorkspacesDir</MudText>
            </MudCardContent>
        </MudCard>
    </MudItem>

    <MudItem xs="12" md="6">
        <MudCard>
            <MudCardHeader>
                <MudText Typo="Typo.h6">Workspace Documents</MudText>
            </MudCardHeader>
            <MudCardContent>
                <!-- Show document counts, recent items, etc. -->
            </MudCardContent>
        </MudCard>
    </MudItem>
</MudGrid>

@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    [Inject]
    public WorkspaceLayoutVM LayoutVM { get; set; }
}
```

---

## Architecture Integration

### Service Flow Diagram

```
┌──────────────────────────────────────────────────────────────┐
│                       MainLayout.razor                        │
│  - Injects WorkspaceLayoutVM                                 │
└───────────────────────┬──────────────────────────────────────┘
                        │
                        ↓ Cascades
┌──────────────────────────────────────────────────────────────┐
│                  CascadingValue: UserServices                 │
│  - IObservableReaderWriter<string, Workspace>                │
│  - User-scoped services                                      │
└───────────────────────┬──────────────────────────────────────┘
                        │
                        ↓ Cascades
┌──────────────────────────────────────────────────────────────┐
│               CascadingValue: WorkspaceServices               │
│  - IObservableReader<string, BotEntity>                      │
│  - IObservableWriter<string, BotEntity>                      │
│  - Workspace-scoped services                                 │
└───────────────────────┬──────────────────────────────────────┘
                        │
                        ↓ Available to
┌──────────────────────────────────────────────────────────────┐
│                     Page Components                           │
│  - Receive services via CascadingParameter                   │
│  - Use ObservableDataView or manual VM patterns             │
└──────────────────────────────────────────────────────────────┘
```

### Workspace Lifecycle

```
Application Start
    ↓
WorkspaceLayoutVM created (injected)
    ↓
User Authenticates
    ↓
OnUserChanged()
    ↓ ConfigureUserServices()
    ↓ Set UserWorkspacesDir
    ↓ Register IObservableReaderWriter<Workspace>
UserServices = serviceCollection.BuildServiceProvider()
    ↓
WorkspaceSelector displays available workspaces
    ↓
User Selects Workspace
    ↓
WorkspaceId property set
    ↓
OnWorkspaceChanged()
    ↓ DoConfigureWorkspaceServices()
    ↓ Invoke IWorkspaceServiceConfigurator[]
    ↓ Register workspace-scoped services
WorkspaceServices = serviceCollection.BuildServiceProvider()
    ↓
Components receive WorkspaceServices
    ↓
Components resolve IObservableReader/Writer
    ↓
Data loaded and displayed
```

---

## Dependencies

### NuGet Packages

```xml
<PackageReference Include="BlazorGridStack" />
<PackageReference Include="Microsoft.AspNetCore.Components.Web" />
<PackageReference Include="ReactiveUI.SourceGenerators" />
```

### LionFire Dependencies

```xml
<ProjectReference Include="..\LionFire.Blazor.Components.MudBlazor\" />
<ProjectReference Include="..\LionFire.Blazor.Components.UI\" />
<ProjectReference Include="..\LionFire.Reactive\" />
<ProjectReference Include="..\LionFire.Structures\" />
<ProjectReference Include="..\LionFire.Workspaces.UI\" />
```

---

## Related Documentation

### Architecture
- **[Workspace Architecture](../../../docs/architecture/workspaces/README.md)** - Overall workspace concepts
- **[Service Scoping](../../../docs/architecture/workspaces/service-scoping.md)** - Service provider hierarchy
- **[Integration Diagram](../../../docs/architecture/workspaces/integration-diagram.md)** - Complete flow diagrams

### UI Documentation
- **[Blazor MVVM Patterns](../../../docs/ui/blazor-mvvm-patterns.md)** - When to use which UI pattern
- **[Component Catalog](../../../docs/ui/component-catalog.md)** - All available components

### Project Documentation
- **[LionFire.Workspaces](../LionFire.Workspaces/CLAUDE.md)** - Core workspace implementation
- **[LionFire.Workspaces.Abstractions](../LionFire.Workspaces.Abstractions/CLAUDE.md)** - Workspace interfaces
- **[LionFire.Blazor.Components.MudBlazor](../LionFire.Blazor.Components.MudBlazor/CLAUDE.md)** - ObservableDataView and components

---

## Common Scenarios

### Scenario 1: Basic Workspace Application

**Startup Configuration**:
```csharp
// Program.cs
builder.Services
    .AddWorkspacesModel()
    .AddWorkspaceChildType<BotEntity>()
    .AddWorkspaceChildType<Portfolio>()
    .AddSingleton<WorkspaceLayoutVM>();
```

**Layout**:
```razor
<!-- MainLayout.razor -->
<CascadingValue Name="UserServices" Value="@LayoutVM.UserServices">
    <CascadingValue Name="WorkspaceServices" Value="@LayoutVM.WorkspaceServices">
        <WorkspaceSelector @bind-SelectedId="@LayoutVM.WorkspaceId" />
        @Body
    </CascadingValue>
</CascadingValue>
```

---

### Scenario 2: Multi-User Application

**Custom Layout VM**:
```csharp
public class MultiUserWorkspaceLayoutVM : WorkspaceLayoutVM
{
    protected override async ValueTask OnUserChanged()
    {
        await base.OnUserChanged();

        // Custom per-user initialization
        await LoadUserPreferences(EffectiveUserName);
        await InitializeUserWorkspaces(EffectiveUserName);
    }
}
```

---

### Scenario 3: Workspace Templates

**Create from Template**:
```csharp
public class WorkspaceTemplateService
{
    public async Task CreateWorkspaceFromTemplate(
        string workspaceName,
        string templateName)
    {
        var templatePath = GetTemplatePath(templateName);
        var workspacePath = GetWorkspacePath(workspaceName);

        // Copy template structure
        await CopyDirectory(templatePath, workspacePath);

        // Initialize workspace metadata
        var workspace = new Workspace { Name = workspaceName };
        await SaveWorkspaceMetadata(workspace);
    }
}
```

---

## Summary

**LionFire.Workspaces.UI.Blazor** provides the **Blazor UI layer** for workspace management:

**Key Components**:
- **WorkspaceLayoutVM** - Manages workspace lifecycle and service scoping
- **WorkspaceSelector** - UI for selecting active workspace
- **WorkspaceGrid** - Grid-based workspace layout (experimental)

**Key Features**:
- User-to-workspace directory mapping
- Workspace service provider creation and cascading
- Integration with IWorkspaceServiceConfigurator pattern
- MudBlazor and BlazorGridStack integration

**Use This Library**: When building Blazor applications with multi-workspace support and workspace-scoped services.

**Integration**: Works seamlessly with LionFire.Workspaces (core) and LionFire.Blazor.Components.MudBlazor (components).
