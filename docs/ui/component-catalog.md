# Component Catalog

## Overview

Comprehensive reference for all LionFire Blazor UI components. This catalog covers components from **LionFire.Blazor.Components.MudBlazor**, **LionFire.Blazor.Components**, and workspace-related UI libraries.

**Organization**: Components are grouped by purpose (data display, workspace management, utilities, etc.).

---

## Table of Contents

1. [Data Display Components](#data-display-components)
2. [Workspace Components](#workspace-components)
3. [Property Grid Components](#property-grid-components)
4. [Utility Components](#utility-components)
5. [Layout Components](#layout-components)

---

## Data Display Components

### ObservableDataView\<TKey, TValue, TValueVM\>

**Location**: `LionFire.Blazor.Components.MudBlazor`

**Purpose**: Reactive data grid with automatic CRUD operations for workspace documents.

**When to Use**:
- Displaying lists of workspace documents
- Need built-in toolbar (Add, Edit, Delete)
- Want minimal code for standard CRUD
- Table/grid layout is acceptable

#### Parameters

**Type Parameters**:
```csharp
TKey             // Key type (usually string)
TValue           // Entity type (e.g., BotEntity)
TValueVM         // ViewModel type (e.g., BotVM)
```

**Essential**:
```csharp
[Parameter] public IServiceProvider? DataServiceProvider { get; set; }
// Pass WorkspaceServices to resolve IObservableReader/Writer
```

**CRUD Control**:
```csharp
[Parameter] public bool ReadOnly { get; set; } = true
[Parameter] public EditMode AllowedEditModes { get; set; }
// EditMode.None, .Cell, .Form, .All
[Parameter] public IEnumerable<Type>? CreatableTypes { get; set; }
// Enable Add button for these types
```

**UI Customization**:
```csharp
[Parameter] public RenderFragment? Columns { get; set; }
// Column definitions
[Parameter] public RenderFragment? EditingColumns { get; set; }
// Columns when editing (overrides Columns)
[Parameter] public RenderFragment<CellContext<TValueVM>>? ChildRowContent { get; set; }
// Expandable row content
[Parameter] public RenderFragment<TValueVM>? ContextMenu { get; set; }
// Right-click menu
[Parameter] public RenderFragment<ObservableDataVM<TKey, TValue, TValueVM>>? ChildContent { get; set; }
// Complete custom rendering
```

**ViewModel Factory**:
```csharp
[Parameter] public Func<TKey, Optional<TValue>, TValueVM>? VMFactory { get; set; }
// Custom VM creation logic
```

**Events**:
```csharp
[Parameter] public EventHandler<DataGridRowClickEventArgs<TValueVM>>? RowClick { get; set; }
```

**Auto-Columns**:
```csharp
[Parameter] public Func<PropertyInfo, bool>? IsAutoColumn { get; set; }
// Determines which properties become columns
[Parameter] public Func<PropertyInfo, bool>? IsAutoEditColumn { get; set; }
// Determines which properties are editable
```

#### Usage Examples

**Basic List**:
```razor
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices">
    <Columns>
        <PropertyColumn Property="x => x.Value.Name" />
        <PropertyColumn Property="x => x.Value.Description" />
    </Columns>
</ObservableDataView>
```

**With CRUD Operations**:
```razor
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices"
                    ReadOnly=false
                    CreatableTypes="@(new[] { typeof(BotEntity) })">
    <Columns>
        <PropertyColumn Property="x => x.Value.Name" />
        <TemplateColumn>
            <CellTemplate>
                <MudSwitch @bind-Checked="context.Item.Value.Enabled" />
            </CellTemplate>
        </TemplateColumn>
    </Columns>
</ObservableDataView>
```

**With Expandable Rows**:
```razor
<ObservableDataView ...>
    <ChildRowContent>
        <MudCard>
            <MudCardContent>
                <MudText>@context.Item.Value.Description</MudText>
                <!-- Additional details -->
            </MudCardContent>
        </MudCard>
    </ChildRowContent>
</ObservableDataView>
```

**See**: [LionFire.Blazor.Components.MudBlazor/CLAUDE.md](../../src/LionFire.Blazor.Components.MudBlazor/CLAUDE.md) for complete documentation.

---

### AsyncVMSourceCacheView\<TKey, TValue, TValueVM\>

**Location**: `LionFire.Blazor.Components.MudBlazor`

**Purpose**: Similar to ObservableDataView but works with `SourceCache<TValue, TKey>` directly.

**When to Use**:
- Data is already in a DynamicData `SourceCache`
- Not using `IObservableReader/Writer` pattern
- Need reactive collection without file persistence

#### Parameters

```csharp
[Parameter] public SourceCache<TValue, TKey>? SourceCache { get; set; }
// Direct cache binding (instead of DataServiceProvider)
```

Other parameters similar to `ObservableDataView`.

#### Usage

```razor
<AsyncVMSourceCacheView TKey="string"
                        TValue="BotEntity"
                        TValueVM="BotVM"
                        SourceCache="@mySourceCache">
    <Columns>
        <PropertyColumn Property="x => x.Value.Name" />
    </Columns>
</AsyncVMSourceCacheView>
```

---

### KeyedCollectionView\<TKey, TValue\>

**Location**: `LionFire.Blazor.Components.MudBlazor`

**Purpose**: Display keyed collections without ViewModels.

**When to Use**:
- Simple read-only display
- Don't need ViewModel features
- Don't want VM overhead

---

### KeyedVMCollectionView\<TKey, TValue, TValueVM\>

**Location**: `LionFire.Blazor.Components.MudBlazor`

**Purpose**: Display collections with explicit ViewModels (not auto-created).

**When to Use**:
- Have pre-created VMs
- Manual VM lifecycle management
- Don't need observable reader/writer

---

## Workspace Components

### WorkspaceSelector

**Location**: `LionFire.Workspaces.UI.Blazor`

**Purpose**: Dropdown selector for choosing active workspace.

#### Parameters

```csharp
[Parameter] public string? SelectedId { get; set; }
[Parameter] public EventCallback<string?> SelectedIdChanged { get; set; }
[Parameter] public WorkspaceLayoutVM? VM { get; set; }
```

#### Usage

```razor
<WorkspaceSelector @bind-SelectedId="currentWorkspace"
                   VM="@workspaceLayoutVM" />

@code {
    string? currentWorkspace;
    WorkspaceLayoutVM workspaceLayoutVM;
}
```

**Cascading Parameter**:
```csharp
[CascadingParameter(Name = "UserServices")]
public IServiceProvider? UserServices { get; set; }
```

**Internally**:
- Resolves `IObservableReaderWriter<string, Workspace>` from `UserServices`
- Displays all available workspaces from user's workspace directory
- Fires `SelectedIdChanged` when user selects workspace

---

### WorkspaceGrid

**Location**: `LionFire.Workspaces.UI.Blazor`

**Purpose**: Grid display of workspace documents (alternative to ObservableDataView).

---

### WorkspaceNavMenu

**Location**: `LionFire.Workspaces.UI.Blazor`

**Purpose**: Navigation menu for workspace-related routes.

---

## Property Grid Components

### InspectorView

**Location**: `LionFire.Blazor.Components.MudBlazor`

**Purpose**: Property grid inspector for viewing/editing object properties.

**When to Use**:
- Need property grid view of an object
- Want automatic property editors
- Building configuration/settings UI

#### Parameters

```csharp
[Parameter] public object? InspectedObject { get; set; }
[Parameter] public bool ReadOnly { get; set; }
```

#### Usage

```razor
<InspectorView InspectedObject="@myEntity"
               ReadOnly="false" />

@code {
    BotEntity myEntity = new BotEntity { Name = "Bot1" };
}
```

**Features**:
- Automatic property detection
- Type-appropriate editors (text, number, select, date, etc.)
- Nested object support
- Custom editors via templates

---

### InspectorRow

**Location**: `LionFire.Blazor.Components.MudBlazor`

**Purpose**: Single row in property grid.

---

### InspectorValueCell

**Location**: `LionFire.Blazor.Components.MudBlazor`

**Purpose**: Value cell with type-appropriate editor.

---

### Cell Editors

**Location**: `LionFire.Blazor.Components.MudBlazor/PropertyGrid/CellEditors/`

- **MudTextInspector** - Text property editor
- **MudNumericInspector** - Numeric property editor
- **MudSelectInspector** - Enum/select property editor
- **MudDateInspector** - Date property editor

---

## Utility Components

### CascadingT\<T\>

**Location**: `LionFire.Blazor.Components`

**Purpose**: Type-safe cascading value wrapper.

#### Parameters

```csharp
[Parameter] public T? Value { get; set; }
[Parameter] public RenderFragment? ChildContent { get; set; }
```

#### Usage

```razor
<CascadingT T="IServiceProvider" Value="@WorkspaceServices">
    <ChildComponent />
</CascadingT>

<!-- In ChildComponent -->
@code {
    [CascadingParameter]
    public IServiceProvider? Services { get; set; }
}
```

**Benefit**: Type-safe cascading without string names.

---

### Async\<T\>

**Location**: `LionFire.Blazor.Components`

**Purpose**: Renders async content with loading/error states.

#### Parameters

```csharp
[Parameter] public Task<T>? Task { get; set; }
[Parameter] public RenderFragment<T>? ChildContent { get; set; }
[Parameter] public RenderFragment? Loading { get; set; }
[Parameter] public RenderFragment<Exception>? Error { get; set; }
```

#### Usage

```razor
<Async Task="@LoadDataAsync()">
    <Loading>
        <MudProgressCircular Indeterminate />
    </Loading>
    <Error>
        <MudAlert Severity="Severity.Error">@context.Message</MudAlert>
    </Error>
    <ChildContent>
        <MudText>Loaded: @context.Name</MudText>
    </ChildContent>
</Async>

@code {
    private async Task<MyData> LoadDataAsync()
    {
        await Task.Delay(1000);
        return new MyData { Name = "Test" };
    }
}
```

---

### NavMenuItem

**Location**: `LionFire.Blazor.Components`

**Purpose**: Navigation menu item with reactive active state.

---

### LionMudTextField

**Location**: `LionFire.Blazor.Components.MudBlazor`

**Purpose**: Extended MudTextField with additional features.

---

## Layout Components

### UserLayoutVM

**Location**: `LionFire.Blazor.Components`

**Purpose**: Base ViewModel for user-scoped layouts.

**Features**:
- User authentication state management
- User-scoped service provider creation
- User workspace directory setup

**Usage**:
```csharp
public class MyLayoutVM : UserLayoutVM
{
    public MyLayoutVM(AuthenticationStateProvider authProvider)
        : base(authProvider)
    {
    }

    protected override async ValueTask ConfigureUserServices(IServiceCollection services)
    {
        await base.ConfigureUserServices(services);
        // Register user-scoped services
    }
}
```

---

### WorkspaceLayoutVM

**Location**: `LionFire.Workspaces.UI.Blazor`

**Purpose**: ViewModel managing workspace lifecycle and service scopes.

**Features**:
- Workspace selection
- Workspace service provider creation
- Cascades `WorkspaceServices` to descendants
- Invokes `IWorkspaceServiceConfigurator[]`

**Usage**:
```razor
@inherits LayoutComponentBase
@inject WorkspaceLayoutVM VM

<CascadingValue Name="WorkspaceServices" Value="@VM.WorkspaceServices">
    <WorkspaceSelector @bind-SelectedId="@VM.WorkspaceId" VM="@VM" />
    @Body
</CascadingValue>
```

**See**: [Workspace Architecture](../architecture/workspaces/README.md) for details.

---

## Terminal Components

### TerminalView

**Location**: `LionFire.Blazor.Components`

**Purpose**: Terminal/console view component.

---

### OutputPane

**Location**: `LionFire.Blazor.Components`

**Purpose**: Output pane for displaying terminal output.

---

### ConsoleActivity

**Location**: `LionFire.Blazor.Components`

**Purpose**: Console activity indicator.

---

## Component Usage Patterns

### Pattern: List + Detail Navigation

**List Page** (uses ObservableDataView):
```razor
@page "/bots"

<ObservableDataView TKey="string" TValue="BotEntity" TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices">
    <Columns>
        <PropertyColumn Property="x => x.Value.Name" />
        <TemplateColumn>
            <CellTemplate>
                <MudButton Href="@($"/bots/{context.Item.Key}")">
                    Edit
                </MudButton>
            </CellTemplate>
        </TemplateColumn>
    </Columns>
</ObservableDataView>
```

**Detail Page** (uses manual VM):
```razor
@page "/bots/{BotId}"

<MudCard>
    <MudCardContent>
        <MudTextField @bind-Value="vm.Value.Name" />
    </MudCardContent>
    <MudCardActions>
        <MudButton OnClick="Save">Save</MudButton>
        <MudButton Href="/bots">Back</MudButton>
    </MudCardActions>
</MudCard>

@code {
    [Parameter] public string? BotId { get; set; }
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    ObservableReaderWriterItemVM<string, BotEntity, BotVM>? vm;

    protected override void OnInitialized()
    {
        var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
        var writer = WorkspaceServices.GetService<IObservableWriter<string, BotEntity>>();
        vm = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);
        vm.Id = BotId;
    }

    private async Task Save() => await vm.Write();
}
```

---

### Pattern: Master-Detail (Same Page)

```razor
@page "/bots-manager"

<MudGrid>
    <!-- Master -->
    <MudItem xs="12" md="6">
        <ObservableDataView TKey="string" TValue="BotEntity" TValueVM="BotVM"
                            DataServiceProvider="@WorkspaceServices"
                            RowClick="@OnRowClick">
            <Columns>
                <PropertyColumn Property="x => x.Value.Name" />
            </Columns>
        </ObservableDataView>
    </MudItem>

    <!-- Detail -->
    <MudItem xs="12" md="6">
        @if (selectedId != null)
        {
            <BotDetail BotId="@selectedId" WorkspaceServices="@WorkspaceServices" />
        }
    </MudItem>
</MudGrid>

@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    string? selectedId;

    void OnRowClick(object sender, DataGridRowClickEventArgs<BotVM> e)
    {
        selectedId = e.Item.Key;
    }
}
```

---

### Pattern: Dashboard with Multiple Sources

```razor
@page "/dashboard"

<MudGrid>
    <!-- Bots Card -->
    <MudItem xs="12" md="4">
        <MudCard>
            <MudCardHeader>Active Bots</MudCardHeader>
            <MudCardContent>
                @if (botsVM != null)
                {
                    foreach (var bot in botsVM.Items.Items.Where(b => b.Value.Enabled))
                    {
                        <MudChip>@bot.Value.Name</MudChip>
                    }
                }
            </MudCardContent>
        </MudCard>
    </MudItem>

    <!-- Portfolio Card -->
    <MudItem xs="12" md="4">
        <MudCard>
            <MudCardHeader>Portfolio</MudCardHeader>
            <MudCardContent>
                @if (portfolioVM?.Value != null)
                {
                    <MudText>Total: @portfolioVM.Value.TotalValue.ToString("C2")</MudText>
                }
            </MudCardContent>
        </MudCard>
    </MudItem>
</MudGrid>

@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    ObservableDataVM<string, BotEntity, BotVM>? botsVM;
    ObservableReaderItemVM<string, Portfolio, PortfolioVM>? portfolioVM;

    protected override void OnInitialized()
    {
        var botReader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
        botsVM = new ObservableDataVM<string, BotEntity, BotVM>(botReader);

        var portfolioReader = WorkspaceServices.GetService<IObservableReader<string, Portfolio>>();
        portfolioVM = new ObservableReaderItemVM<string, Portfolio, PortfolioVM>(portfolioReader);
        portfolioVM.Id = "main";
    }
}
```

---

## Component Selection Guide

| Need | Component | Alternative |
|------|-----------|-------------|
| **List of items** | `ObservableDataView` | `AsyncVMSourceCacheView` |
| **Single item detail** | Manual VM | N/A |
| **Property grid** | `InspectorView` | Manual form |
| **Workspace selector** | `WorkspaceSelector` | Custom dropdown |
| **Cascading values** | `CascadingT<T>` | `CascadingValue` |
| **Async content** | `Async<T>` | Manual state management |
| **Navigation menu** | `NavMenuItem` | `MudNavLink` |
| **Terminal** | `TerminalView` | Custom component |

---

## Related Documentation

- **[Blazor MVVM Patterns](blazor-mvvm-patterns.md)** - When to use each pattern
- **[Reactive UI Updates](reactive-ui-updates.md)** - How reactive updates work
- **[LionFire.Blazor.Components.MudBlazor](../../src/LionFire.Blazor.Components.MudBlazor/CLAUDE.md)** - Deep dive on ObservableDataView
- **[LionFire.Data.Async.Mvvm](../../src/LionFire.Data.Async.Mvvm/CLAUDE.md)** - ViewModels documentation
- **[Workspace Architecture](../architecture/workspaces/README.md)** - Workspace concepts

---

## Summary

LionFire provides a comprehensive component library for Blazor UI:

**Primary Components**:
- **ObservableDataView** - Automatic reactive data grids
- **InspectorView** - Property grid inspector
- **WorkspaceSelector** - Workspace selection UI
- **Async\<T\>** - Async content rendering

**Key Benefit**: Components integrate seamlessly with workspace-scoped services and reactive data persistence, eliminating boilerplate and manual subscription management.

**Most Common**: 90% of scenarios use either `ObservableDataView` (for lists) or manual VM pattern (for details).
