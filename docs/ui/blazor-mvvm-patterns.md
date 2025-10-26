# Blazor MVVM Patterns

**Overview**: This document describes the two primary patterns for building Blazor components that work with workspace-scoped MVVM data in LionFire. Both patterns leverage reactive persistence (`IObservableReader/Writer`) and workspace service scoping.

---

## Table of Contents

1. [Pattern Overview](#pattern-overview)
2. [Pattern 1: ObservableDataView (List Views)](#pattern-1-observabledataview-list-views)
3. [Pattern 2: Manual VM Creation (Single-Item Views)](#pattern-2-manual-vm-creation-single-item-views)
4. [When to Use Which Pattern](#when-to-use-which-pattern)
5. [Navigation Between List and Detail](#navigation-between-list-and-detail)
6. [Common Scenarios](#common-scenarios)
7. [Troubleshooting](#troubleshooting)

---

## Pattern Overview

### The Challenge

You have workspace-scoped services (`IObservableReader/Writer<TKey, TValue>`) that need to be consumed by Blazor components to display and edit data. The services are **not in the root DI container** - they're in a workspace-specific service provider.

### The Two Patterns

| Pattern | Use Case | Complexity | Flexibility |
|---------|----------|------------|-------------|
| **ObservableDataView** | List of items with CRUD | Low - Component handles everything | Medium - Configure via parameters |
| **Manual VM Creation** | Single item detail view | Medium - Manual setup required | High - Full control |

### Common Foundation

Both patterns use:
- **CascadingParameter** to receive `WorkspaceServices`
- **IObservableReader/Writer** for reactive persistence
- **ViewModels** to wrap entities for UI binding
- **MudBlazor** components for UI

---

## Pattern 1: ObservableDataView (List Views)

### When to Use

✅ **Use ObservableDataView when:**
- Displaying a **list/grid** of items
- Need standard CRUD operations (Create, Read, Update, Delete)
- Want built-in toolbar with Add/Edit/Delete buttons
- Data is workspace-scoped

### How It Works

`ObservableDataView` is a pre-built component that:
1. Accepts workspace services via `DataServiceProvider` parameter
2. Automatically resolves `IObservableReader/Writer<TKey, TValue>`
3. Creates ViewModels for each item using factory
4. Renders a MudDataGrid with reactive updates
5. Provides toolbar with CRUD operations

### Complete Example

**Bots.razor** - List of bots in workspace:

```razor
@page "/bots"
@using LionFire.Mvvm
@using LionFire.Trading.Automation

<div class="pa-6">
    <ObservableDataView @ref=ItemsEditor
                        DataServiceProvider="WorkspaceServices"
                        TKey="string"
                        TValue="BotEntity"
                        TValueVM="BotVM"
                        AllowedEditModes="EditMode.All"
                        ReadOnly=false>
        <Columns>
            <!-- Status icon with navigation -->
            <TemplateColumn T="BotVM">
                <HeaderTemplate>Status</HeaderTemplate>
                <CellTemplate>
                    <MudLink Href="@($"/bots/{context.Item.Key}")">
                        <MudIcon Icon="@Icons.Material.Outlined.Circle" />
                    </MudLink>
                </CellTemplate>
            </TemplateColumn>

            <!-- Enabled toggle -->
            <TemplateColumn T="BotVM">
                <HeaderTemplate>Enabled</HeaderTemplate>
                <CellTemplate>
                    @if (context.Item?.Value != null)
                    {
                        <MudSwitch T="bool"
                                   @bind-Value="context.Item.Value.Enabled"
                                   Color="Color.Primary"
                                   Size="Size.Small" />
                    }
                </CellTemplate>
            </TemplateColumn>

            <!-- Live toggle (changes color based on account type) -->
            <TemplateColumn T="BotVM">
                <HeaderTemplate>Live</HeaderTemplate>
                <CellTemplate>
                    @if (context.Item?.Value != null)
                    {
                        <MudSwitch T="bool"
                                   @bind-Value="context.Item.Value.Live"
                                   Color="@(context.Item.IsLive ? Color.Secondary : Color.Default)"
                                   Size="Size.Small" />
                    }
                </CellTemplate>
            </TemplateColumn>

            <!-- Standard property columns -->
            <PropertyColumn Property="x => x.Value.Exchange" Title="Exchange" />
            <PropertyColumn Property="x => x.Value.ExchangeArea" Title="Area" />
            <PropertyColumn Property="x => x.Value.Symbol" Title="Symbol" />
            <PropertyColumn Property="x => x.Value.TimeFrame" Title="TimeFrame" />
            <PropertyColumn Property="x => x.Value.BotTypeName" Title="Type" />
            <PropertyColumn Property="x => x.Value.Name" />
            <PropertyColumn Property="x => x.AD" Title="AD" />
            <PropertyColumn Property="x => x.Value.Comments" />
        </Columns>

        <!-- Optional: Expandable row content -->
        <ChildRowContent>
            <MudCard>
                <MudCardHeader>
                    <CardHeaderContent>
                        <MudText Typo="Typo.h6">@context.Item.Value.Name</MudText>
                    </CardHeaderContent>
                </MudCardHeader>
                <MudCardContent>
                    <MudText>Enabled: <MudSwitch T="bool" @bind-Value="context.Item.Value.Enabled" /></MudText>
                    <MudText>Comments: @context.Item.Value.Comments</MudText>
                </MudCardContent>
            </MudCard>
        </ChildRowContent>

        <!-- Optional: Context menu -->
        <ContextMenu>
            <MudMenuItem Icon="@Icons.Material.Filled.Delete">
                Delete @context.Value.Name
            </MudMenuItem>
        </ContextMenu>
    </ObservableDataView>
</div>

@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    ObservableDataView<string, BotEntity, BotVM>? ItemsEditor { get; set; }
}
```

### ViewModel Definition

```csharp
// Simple ViewModel wrapping the entity
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value)
    {
    }

    // UI-specific computed properties
    public bool IsLive => Value.Live && AccountIsRealMoney();

    public double? AD => CalculateAverageDrawdown();

    // Event handlers for toolbar actions
    public ValueTask OnStart() => StartBotAsync();
    public ValueTask OnStop() => StopBotAsync();

    private bool AccountIsRealMoney() { /* ... */ }
    private double? CalculateAverageDrawdown() { /* ... */ }
}
```

### Key Features

**Automatic CRUD Toolbar**:
- Add button (if `CreatableTypes` specified)
- Edit mode toggle
- Delete toggle
- Refresh button (if `ShowRefresh = true`)

**Automatic VM Creation**:
```csharp
// Component automatically creates VMs for each entity
// using constructor: new BotVM(key, entity)
```

**Reactive Updates**:
```csharp
// When files change on disk or items are added/removed,
// the grid automatically updates via DynamicData observables
```

**Built-in Features**:
- Sorting (multi-column)
- Filtering
- Grouping
- Pagination
- Cell/Row editing
- Context menus
- Expandable rows

### Parameters

```csharp
// Required
TKey               // Key type (usually string)
TValue             // Entity type
TValueVM           // ViewModel type

// Recommended
DataServiceProvider   // Pass WorkspaceServices here

// Optional
AllowedEditModes     // EditMode.None, .Cell, .Form, .All
ReadOnly             // Disable editing
CreatableTypes       // Enable "Add" button
VMFactory            // Custom VM creation
Columns              // Custom column definitions
ChildRowContent      // Expandable row template
ContextMenu          // Right-click menu
```

---

## Pattern 2: Manual VM Creation (Single-Item Views)

### When to Use

✅ **Use Manual VM Creation when:**
- Displaying/editing a **single item** detail view
- Need custom layout (not a grid)
- Want fine-grained control over VM lifecycle
- Implementing master-detail pattern

### How It Works

1. Receive workspace services via `CascadingParameter`
2. Manually resolve `IObservableReader/Writer`
3. Create `ObservableReaderWriterItemVM` with services
4. Set VM's `Id` property to load specific item
5. Bind UI to VM properties
6. Call `VM.Write()` to save changes

### Complete Example

**Bot.razor** - Single bot detail page:

```razor
@page "/bots/{BotId}"
@using LionFire.Mvvm
@using LionFire.Trading.Automation
@using LionFire.Reactive.Persistence
@using Microsoft.Extensions.DependencyInjection
@using Microsoft.Extensions.Logging
@inject ILogger<Bot> Logger
@inject IServiceProvider ServiceProvider

<h3>Bot @BotId</h3>

@if (VM?.Value != null)
{
    <MudTextField @bind-Value="VM.Value.Name" Label="Name" />
    <MudTextField @bind-Value="VM.Value.Comments" Label="Comments" />
    <MudTextField @bind-Value="VM.Value.Description" Label="Description" />

    <MudButton OnClick="Save" Color="Color.Primary">Save</MudButton>
}
else if (VM != null)
{
    <MudProgressCircular Indeterminate="true" />
    <p>Loading bot...</p>
}
else
{
    <MudAlert Severity="Severity.Error">
        Unable to load bot services. Check logs for details.
    </MudAlert>
}

@code {
    [Parameter]
    public string? BotId { get; set; }

    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    private ObservableReaderWriterItemVM<string, BotEntity, BotVM>? VM { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        // Try workspace services first, fall back to root for debugging/testing
        var effectiveServices = WorkspaceServices ?? ServiceProvider;

        if (WorkspaceServices == null)
        {
            Logger.LogWarning(
                "WorkspaceServices cascading parameter not found. " +
                "Falling back to root ServiceProvider. " +
                "For production, this page should be rendered within a workspace layout."
            );
        }

        // Resolve reader/writer from workspace services
        var reader = effectiveServices.GetService<IObservableReader<string, BotEntity>>();
        var writer = effectiveServices.GetService<IObservableWriter<string, BotEntity>>();

        if (reader == null || writer == null)
        {
            Logger.LogError(
                "Bot persistence services not registered. " +
                "Reader: {ReaderAvailable}, Writer: {WriterAvailable}, " +
                "Source: {ServiceSource}",
                reader != null,
                writer != null,
                WorkspaceServices != null ? "Workspace" : "Root"
            );

            Logger.LogError(
                "This means either: " +
                "1) Workspace layout is not being used, or " +
                "2) Workspace configurators haven't run yet, or " +
                "3) BotEntity not registered with AddWorkspaceChildType"
            );
            return;
        }

        Logger.LogInformation(
            "Loaded Bot persistence services from {ServiceSource}",
            WorkspaceServices != null ? "Workspace" : "Root"
        );

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
            Logger.LogInformation("Saved bot {BotId}", BotId);
            // Optional: Show success snackbar
        }
    }
}
```

### Key Points

**Service Resolution**:
```csharp
// Get services from workspace scope
var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
var writer = WorkspaceServices.GetService<IObservableWriter<string, BotEntity>>();
```

**VM Creation**:
```csharp
// Create VM manually, passing workspace-scoped services
VM = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);
VM.Id = BotId;  // Triggers load from persistence
```

**Reactive Loading**:
```csharp
// When VM.Id is set, it automatically:
// 1. Calls reader.TryGetValue(BotId)
// 2. Creates BotVM if entity found
// 3. Notifies UI via PropertyChanged
// 4. UI re-renders with @if (VM?.Value != null)
```

**Saving Changes**:
```csharp
// Call VM.Write() to persist changes
await VM.Write();

// Internally calls: writer.Write(VM.Id, VM.Value.Value)
```

### Fallback Pattern (Development)

The example above includes a fallback for development:

```csharp
// Try workspace services first, fall back to root
var effectiveServices = WorkspaceServices ?? ServiceProvider;

if (WorkspaceServices == null)
{
    Logger.LogWarning("Using root services - workspace layout not found");
}
```

**When to use fallback**:
- ✅ During development/testing
- ✅ When testing pages in isolation
- ❌ In production (should fail fast)

**Production pattern** (no fallback):
```csharp
if (WorkspaceServices == null)
{
    throw new InvalidOperationException(
        "This page must be rendered within a workspace layout"
    );
}

var reader = WorkspaceServices.GetRequiredService<IObservableReader<string, BotEntity>>();
```

---

## When to Use Which Pattern

### Decision Tree

```
Do you need to display multiple items?
├─ Yes
│  └─ Do you need custom layout/grid?
│     ├─ No  → Use ObservableDataView ✅
│     └─ Yes → Consider ObservableDataView with custom columns,
│              or manual pattern if very custom
└─ No (single item)
   └─ Use Manual VM Creation ✅
```

### Comparison Table

| Aspect | ObservableDataView | Manual VM Creation |
|--------|-------------------|-------------------|
| **Lines of Code** | ~20-30 (mostly column definitions) | ~60-80 (full setup) |
| **Control** | Medium (parameter-driven) | High (full control) |
| **Complexity** | Low (component handles DI) | Medium (manual DI) |
| **CRUD** | Built-in toolbar | Implement yourself |
| **Layout** | Grid/Table only | Any layout |
| **Learning Curve** | Easy | Moderate |
| **Best For** | Lists, grids, collections | Details, forms, custom UI |

### Real-World Examples

**ObservableDataView**:
- Bot list page ✅
- Portfolio list ✅
- Strategy list ✅
- Settings categories list ✅
- File browser ✅

**Manual VM Creation**:
- Bot detail/edit page ✅
- Portfolio dashboard ✅
- Strategy builder ✅
- Settings detail page ✅
- Custom wizard/form ✅

---

## Navigation Between List and Detail

### Master-Detail Pattern

**List Page** (Master):
```razor
<ObservableDataView ...>
    <Columns>
        <TemplateColumn>
            <CellTemplate>
                <!-- Navigate to detail page -->
                <MudButton Href="@($"/bots/{context.Item.Key}")">
                    Edit
                </MudButton>
            </CellTemplate>
        </TemplateColumn>
    </Columns>
</ObservableDataView>
```

**Detail Page**:
```razor
@page "/bots/{BotId}"

<!-- Back button -->
<MudButton Href="/bots" StartIcon="@Icons.Material.Filled.ArrowBack">
    Back to List
</MudButton>

<!-- Rest of detail view -->
```

### URL Routing

```
/bots              → Bots.razor (List View - ObservableDataView)
/bots/bot-alpha    → Bot.razor (Detail View - Manual VM)
/bots/bot-beta     → Bot.razor (Detail View - Manual VM)
```

### Passing Context

**Via URL Parameter**:
```razor
@page "/bots/{BotId}"

@code {
    [Parameter]
    public string? BotId { get; set; }
}
```

**Via Navigation State** (if needed):
```csharp
NavigationManager.NavigateTo("/bots/bot-alpha", new NavigationOptions {
    State = new { OpenInEditMode = true }
});
```

---

## Common Scenarios

### Scenario 1: Read-Only List

```razor
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices"
                    ReadOnly="true"
                    AllowedEditModes="EditMode.None">
    <!-- Columns -->
</ObservableDataView>
```

### Scenario 2: List with Add Button

```razor
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices"
                    CreatableTypes="@(new[] { typeof(BotEntity) })"
                    CanCreateValueType="true">
    <!-- Columns -->
</ObservableDataView>
```

### Scenario 3: Custom VM Factory

```razor
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices"
                    VMFactory="@CreateBotVM">
    <!-- Columns -->
</ObservableDataView>

@code {
    private BotVM CreateBotVM(string key, Optional<BotEntity> entity)
    {
        if (entity.HasValue)
        {
            var vm = new BotVM(key, entity.Value);
            vm.Initialize(someService); // Custom initialization
            return vm;
        }
        return null;
    }
}
```

### Scenario 4: Detail Page with Tabs

```razor
@page "/bots/{BotId}"

@if (VM?.Value != null)
{
    <MudTabs>
        <MudTabPanel Text="General">
            <MudTextField @bind-Value="VM.Value.Name" Label="Name" />
            <MudTextField @bind-Value="VM.Value.Description" Label="Description" />
        </MudTabPanel>

        <MudTabPanel Text="Parameters">
            <MudTextField @bind-Value="VM.Value.Parameters.StopLoss" Label="Stop Loss" />
            <MudTextField @bind-Value="VM.Value.Parameters.TakeProfit" Label="Take Profit" />
        </MudTabPanel>

        <MudTabPanel Text="Advanced">
            <!-- Advanced settings -->
        </MudTabPanel>
    </MudTabs>

    <MudButton OnClick="Save">Save All</MudButton>
}

@code {
    // Same setup as before
}
```

### Scenario 5: Dirty State Tracking

```razor
@code {
    private bool IsDirty => VM?.Value?.IsChanged ?? false;

    private async Task SaveWithConfirmation()
    {
        if (!IsDirty)
        {
            Snackbar.Add("No changes to save", Severity.Info);
            return;
        }

        await VM.Write();
        Snackbar.Add("Saved successfully", Severity.Success);
    }

    // Warn on navigation if dirty
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            NavigationManager.LocationChanged += CheckDirtyOnNavigation;
        }
    }

    private void CheckDirtyOnNavigation(object sender, LocationChangedEventArgs e)
    {
        if (IsDirty)
        {
            // Show confirmation dialog
        }
    }
}
```

---

## Troubleshooting

### Issue: "Unable to resolve service for type 'IObservableReader'"

**Cause**: Trying to inject workspace-scoped services from root container.

**Solution**: Use `CascadingParameter` and manual resolution:
```razor
[CascadingParameter(Name = "WorkspaceServices")]
public IServiceProvider? WorkspaceServices { get; set; }

var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
```

**See**: [Service Scoping Deep Dive](../architecture/workspaces/service-scoping.md)

### Issue: Services are null even with WorkspaceServices

**Cause**: Document type not registered with `AddWorkspaceChildType`.

**Solution**:
```csharp
services
    .AddWorkspaceChildType<BotEntity>()              // ← Must call this!
    .AddWorkspaceDocumentService<string, BotEntity>();
```

### Issue: ObservableDataView shows empty grid

**Check**:
1. Is `DataServiceProvider` set? (Should be `WorkspaceServices`)
2. Are there files in the workspace directory?
3. Check browser console for errors
4. Verify `IObservableReader` has data:
   ```csharp
   var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
   Logger.LogInformation("Keys: {Keys}", string.Join(", ", reader.Keys.Items));
   ```

### Issue: VM not updating when files change

**Cause**: Not subscribed to observable changes.

**Solution**: For manual pattern, ensure VM subscribes to reader's observable:
```csharp
// ObservableReaderWriterItemVM does this automatically
// But if rolling your own:
reader.Values.Connect()
    .Subscribe(changeSet => {
        // Handle updates
    });
```

### Issue: Changes not persisting

**Check**:
1. Did you call `VM.Write()`?
2. Does the entity implement `INotifyPropertyChanged` (or use `ReactiveObject`)?
3. Check file permissions on workspace directory
4. Look for exceptions in logs

---

## Summary

### Pattern Selection

**Use ObservableDataView for**:
- Lists and grids
- Standard CRUD
- Quick implementation

**Use Manual VM Creation for**:
- Single-item details
- Custom layouts
- Fine-grained control

### Best Practices

1. ✅ Always use `CascadingParameter` for workspace services
2. ✅ Check for null services and log errors
3. ✅ Use `ObservableReaderWriterItemVM` for single items
4. ✅ Call `VM.Write()` to save changes
5. ✅ Register types with `AddWorkspaceChildType`
6. ✅ Implement reactive properties in entities (`ReactiveObject`)
7. ❌ Don't try to inject workspace services via `@inject`
8. ❌ Don't create singleton readers/writers in root container

### Related Documentation

- [Workspace Service Scoping](../architecture/workspaces/service-scoping.md)
- [Workspace Architecture](../architecture/workspaces/README.md)
- [How-To: Create Blazor Workspace Page](../guides/how-to/create-blazor-workspace-page.md)
- [LionFire.Blazor.Components.MudBlazor CLAUDE.md](../../src/LionFire.Blazor.Components.MudBlazor/CLAUDE.md)
- [LionFire.Data.Async.Mvvm CLAUDE.md](../../src/LionFire.Data.Async.Mvvm/CLAUDE.md)
