# UI Patterns & Components

## Overview

LionFire provides a comprehensive Blazor UI toolkit built on **MudBlazor** that integrates seamlessly with reactive data patterns, MVVM architecture, and workspace-scoped services. This documentation covers UI patterns, component usage, and best practices for building reactive Blazor applications.

**Key Philosophy**: Minimize boilerplate while maintaining flexibility. Components should work automatically with reactive data sources, but allow manual control when needed.

---

## Documentation Structure

### 1. **[Blazor MVVM Patterns](blazor-mvvm-patterns.md)** ⭐ START HERE
**Essential reading** for understanding when to use automatic vs. manual patterns.

**Topics**:
- **ObservableDataView Pattern** (Automatic) - Zero-boilerplate data grids
- **Manual ViewModel Pattern** - Full control for detail pages
- **Decision flowchart** - Which pattern to use?
- **Complete examples** - Both patterns side-by-side

**When to Use**:
- Building list or detail views
- Deciding between automatic and manual approaches
- Understanding reactive binding

---

### 2. **[Component Catalog](component-catalog.md)**
**Reference guide** for all available UI components.

**Contents**:
- `ObservableDataView` - Reactive data grid with CRUD
- `InspectorView` - Property grid inspector
- `WorkspaceSelector` - Workspace selection UI
- `CascadingT` - Type-safe cascading values
- Utility components

**When to Use**:
- Finding components for common scenarios
- Understanding component parameters
- Exploring advanced features

---

### 3. **[Reactive UI Updates](reactive-ui-updates.md)**
**Deep dive** into how reactive updates flow through the UI.

**Topics**:
- Change detection mechanisms
- Observable subscriptions
- StateHasChanged optimization
- Performance considerations

**When to Use**:
- Debugging update issues
- Optimizing performance
- Understanding internals

---

## Quick Start

### Pattern 1: Automatic List View (Recommended for Lists)

**Use when**: Displaying a list of workspace documents with standard CRUD operations.

```razor
@page "/bots"

<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices"
                    ReadOnly=false>
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

**What you get**:
- ✅ Automatic data loading from workspace
- ✅ Built-in toolbar (Add, Edit, Delete)
- ✅ Reactive updates when files change
- ✅ Sorting, filtering, pagination
- ✅ ~20 lines of code total

---

### Pattern 2: Manual Detail View (Recommended for Details)

**Use when**: Displaying/editing a single document with custom layout.

```razor
@page "/bots/{BotId}"

<MudCard>
    <MudCardContent>
        <MudTextField Label="Name" @bind-Value="vm.Value.Name" />
        <MudTextField Label="Description" @bind-Value="vm.Value.Description" />
        <MudSwitch @bind-Checked="vm.Value.Enabled" Label="Enabled" />
    </MudCardContent>
    <MudCardActions>
        <MudButton OnClick="Save" Color="Color.Primary">Save</MudButton>
    </MudCardActions>
</MudCard>

@code {
    [Parameter]
    public string? BotId { get; set; }

    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    ObservableReaderWriterItemVM<string, BotEntity, BotVM>? vm;

    protected override void OnInitialized()
    {
        var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
        var writer = WorkspaceServices.GetService<IObservableWriter<string, BotEntity>>();
        vm = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);
        vm.Id = BotId;  // Automatically loads
    }

    private async Task Save()
    {
        await vm.Write();  // Save to file
    }
}
```

**What you get**:
- ✅ Full layout control
- ✅ Automatic data loading/saving
- ✅ Reactive updates from file changes
- ✅ Custom validation and commands
- ✅ ~40 lines of code total

---

## Core Concepts

### 1. Workspace-Scoped Services

**Problem**: UI components need access to workspace-specific data readers/writers.

**Solution**: Cascade `WorkspaceServices` (IServiceProvider) from layout to descendants.

```razor
<!-- Layout Component -->
<CascadingValue Name="WorkspaceServices" Value="@WorkspaceServices">
    @Body
</CascadingValue>

<!-- Child Component -->
@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    // Now can resolve workspace services
    var reader = WorkspaceServices.GetService<IObservableReader<string, MyEntity>>();
}
```

**Why**: Each workspace has its own service provider with readers/writers pointing to that workspace's directories. See [Service Scoping](../architecture/workspaces/service-scoping.md).

---

### 2. Reactive Data Binding

**LionFire components automatically update** when:
- Entity properties change (via `INotifyPropertyChanged`)
- Files are added/removed from workspace
- Observable collections emit changes

**Requirements**:
1. Entity must implement `INotifyPropertyChanged`
2. Use `ReactiveObject` or `[ObservableProperty]`
3. Component subscribes to observables

**Example**:
```csharp
// ✅ Reactive Entity
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;  // Automatically notifies changes
}

// ❌ Non-Reactive Entity
public class BotEntity
{
    public string? Name { get; set; }  // No notifications!
}
```

---

### 3. ViewModel Layer

ViewModels wrap entities and provide UI-specific functionality:

```csharp
// Entity (data model)
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;
    [Reactive] private decimal _profitLoss;
}

// ViewModel (adds UI logic)
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value) { }

    // Computed property for UI
    public string DisplayName => $"{Value.Name} ({Key})";

    // UI-specific formatting
    public string ProfitLossFormatted => Value.ProfitLoss.ToString("C2");

    // Commands
    public ReactiveCommand<Unit, Unit> ToggleEnabled { get; }
}
```

**When to Use VMs**:
- Need computed properties for display
- Need commands for UI actions
- Want to keep entities pure (no UI logic)

---

### 4. Component Hierarchy

```
Application Root
    ↓
WorkspaceLayoutVM
    ↓ Cascades WorkspaceServices
Blazor Pages (@page "/bots")
    ↓ Uses
ObservableDataView (automatic)
    OR
Manual ViewModel Pattern
    ↓ Both resolve
IObservableReader/Writer
    ↓ Backed by
File System (HJSON files)
```

---

## Patterns Summary

### When to Use Each Pattern

| Scenario | Pattern | Component/VM | Boilerplate |
|----------|---------|--------------|-------------|
| **List View** | Automatic | `ObservableDataView` | Minimal (~20 lines) |
| **Detail View** | Manual | `ObservableReaderWriterItemVM` | Medium (~40 lines) |
| **Master-Detail** | Hybrid | List uses `ObservableDataView`, detail uses manual VM | Mixed |
| **Read-Only Display** | Manual | `ObservableReaderItemVM` | Minimal |
| **Custom Layout** | Manual | Custom VM with direct reader/writer access | High (full control) |

---

## Architecture Integration

### Layer Stack

```
UI Layer (Blazor Components)
    ↓ uses
ViewModel Layer (KeyValueVM, ObservableReaderWriterItemVM)
    ↓ wraps
Reactive Persistence Layer (IObservableReader/Writer)
    ↓ backed by
File System (HJSON files in workspace)
```

### Service Resolution Flow

```
Blazor Component
    ↓ CascadingParameter
WorkspaceServices (IServiceProvider)
    ↓ GetService<T>()
IObservableReader<TKey, TValue>
    ↓ Points to
workspace1/Bots/ directory
```

---

## Best Practices

### 1. Prefer ObservableDataView for Lists

```razor
<!-- ✅ Good - Minimal code -->
<ObservableDataView TKey="string" TValue="Bot" TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices" />

<!-- ❌ Avoid - Manual list management -->
@code {
    List<BotVM> bots;
    protected override async Task OnInitializedAsync()
    {
        var reader = WorkspaceServices.GetService<IObservableReader<string, Bot>>();
        // Manual subscription, disposal, updates...
    }
}
```

---

### 2. Use Manual Pattern for Detail Views

```razor
<!-- ✅ Good - Full control over layout -->
<MudCard>
    <MudTextField @bind-Value="vm.Value.Name" />
    <MudTextField @bind-Value="vm.Value.Description" />
</MudCard>

<!-- ❌ Avoid - ObservableDataView for single item -->
<ObservableDataView ... />  <!-- Overkill for one item -->
```

---

### 3. Cascade WorkspaceServices, Not Individual Services

```razor
<!-- ✅ Good - Cascade provider -->
<CascadingValue Name="WorkspaceServices" Value="@WorkspaceServices">
    @Body
</CascadingValue>

<!-- ❌ Avoid - Cascading individual services -->
<CascadingValue Value="@BotReader">
<CascadingValue Value="@BotWriter">
<CascadingValue Value="@PortfolioReader">
    <!-- Too many cascades! -->
</CascadingValue>
```

---

### 4. Use ReactiveObject for Entities

```csharp
// ✅ Good - Reactive notifications
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;
}

// ❌ Avoid - No change notifications
public class BotEntity
{
    public string? Name { get; set; }
}
```

---

### 5. Dispose Subscriptions Properly

```razor
@implements IAsyncDisposable

@code {
    IDisposable? subscription;

    protected override void OnInitialized()
    {
        subscription = reader.Values.Connect().Subscribe(changes => {
            // Handle changes
        });
    }

    public async ValueTask DisposeAsync()
    {
        subscription?.Dispose();
    }
}
```

---

## Common Scenarios

### Scenario 1: Simple Read-Only List

```razor
<ObservableDataView TKey="string"
                    TValue="Config"
                    TValueVM="ConfigVM"
                    DataServiceProvider="@WorkspaceServices"
                    ReadOnly="true">
    <Columns>
        <PropertyColumn Property="x => x.Value.Name" />
    </Columns>
</ObservableDataView>
```

---

### Scenario 2: Editable List with Add/Delete

```razor
<ObservableDataView TKey="string"
                    TValue="Bot"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices"
                    ReadOnly="false"
                    CreatableTypes="@(new[] { typeof(Bot) })">
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

---

### Scenario 3: Master-Detail Navigation

**List Page**:
```razor
@page "/bots"

<ObservableDataView ...>
    <Columns>
        <TemplateColumn>
            <CellTemplate>
                <MudButton Href="@($"/bots/{context.Item.Key}")">Edit</MudButton>
            </CellTemplate>
        </TemplateColumn>
    </Columns>
</ObservableDataView>
```

**Detail Page**:
```razor
@page "/bots/{BotId}"

<MudCard>
    <MudCardContent>
        <MudTextField @bind-Value="vm.Value.Name" />
        <!-- Custom layout -->
    </MudCardContent>
</MudCard>

@code {
    [Parameter] public string? BotId { get; set; }
    ObservableReaderWriterItemVM<string, Bot, BotVM>? vm;
}
```

---

### Scenario 4: Custom Column Templates

```razor
<ObservableDataView ...>
    <Columns>
        <!-- Status indicator -->
        <TemplateColumn>
            <CellTemplate>
                <MudIcon Icon="@Icons.Material.Filled.Circle"
                         Color="@(context.Item.Value.Enabled ? Color.Success : Color.Default)" />
            </CellTemplate>
        </TemplateColumn>

        <!-- Action buttons -->
        <TemplateColumn>
            <CellTemplate>
                <MudIconButton Icon="@Icons.Material.Filled.PlayArrow"
                               OnClick="@(() => StartBot(context.Item))" />
                <MudIconButton Icon="@Icons.Material.Filled.Stop"
                               OnClick="@(() => StopBot(context.Item))" />
            </CellTemplate>
        </TemplateColumn>
    </Columns>
</ObservableDataView>
```

---

## Troubleshooting

### Issue: "Unable to resolve service IObservableReader"

**Cause**: Using root DI container instead of workspace services.

**Fix**:
```razor
<!-- ❌ Wrong -->
<ObservableDataView DataServiceProvider="@ServiceProvider" />

<!-- ✅ Correct -->
<ObservableDataView DataServiceProvider="@WorkspaceServices" />
```

---

### Issue: UI not updating when entity changes

**Cause**: Entity doesn't implement `INotifyPropertyChanged`.

**Fix**:
```csharp
// ❌ Wrong
public class Bot { public string Name { get; set; } }

// ✅ Correct
public partial class Bot : ReactiveObject
{
    [Reactive] private string? _name;
}
```

---

### Issue: Component shows empty grid

**Check**:
1. Is `DataServiceProvider` set to `WorkspaceServices`?
2. Are files present in workspace directory?
3. Is entity type registered with `AddWorkspaceChildType<T>()`?
4. Check console for errors

**Debug**:
```csharp
var reader = WorkspaceServices.GetService<IObservableReader<string, Bot>>();
Console.WriteLine($"Keys: {string.Join(", ", reader?.Keys.Items)}");
```

---

## Related Documentation

### Architecture
- **[Workspace Architecture](../architecture/workspaces/README.md)** - High-level workspace concepts
- **[Service Scoping](../architecture/workspaces/service-scoping.md)** - Understanding workspace services
- **[MVVM Architecture](../architecture/mvvm/README.md)** - ViewModel patterns

### Project Documentation
- **[LionFire.Blazor.Components.MudBlazor](../../src/LionFire.Blazor.Components.MudBlazor/CLAUDE.md)** - ObservableDataView deep dive
- **[LionFire.Data.Async.Mvvm](../../src/LionFire.Data.Async.Mvvm/CLAUDE.md)** - ViewModels and reactive patterns
- **[LionFire.Reactive](../../src/LionFire.Reactive/CLAUDE.md)** - Observable readers/writers

### Guides
- **[How-To: Create Blazor Workspace Page](../guides/how-to/create-blazor-workspace-page.md)** - Step-by-step tutorial

---

## Summary

LionFire's Blazor UI toolkit provides **two primary patterns**:

### Automatic Pattern (Lists)
- **Component**: `ObservableDataView`
- **Boilerplate**: Minimal (~20 lines)
- **Use for**: Lists, grids, CRUD

### Manual Pattern (Details)
- **ViewModel**: `ObservableReaderWriterItemVM`
- **Boilerplate**: Medium (~40 lines)
- **Use for**: Detail views, custom layouts

**Key Benefit**: Both patterns integrate seamlessly with workspace-scoped services and reactive data persistence, eliminating manual subscription management and state synchronization.

**Next Steps**:
1. Read **[Blazor MVVM Patterns](blazor-mvvm-patterns.md)** for detailed examples
2. Browse **[Component Catalog](component-catalog.md)** for available components
3. Check **[Reactive UI Updates](reactive-ui-updates.md)** for performance optimization
