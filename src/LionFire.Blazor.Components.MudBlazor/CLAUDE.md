# LionFire.Blazor.Components.MudBlazor

## Overview

Reactive Blazor components built on MudBlazor that integrate with LionFire's MVVM and reactive persistence patterns. These components provide high-level, pre-built UI elements for working with observable data collections, particularly workspace-scoped documents.

**Key Feature**: Automatic integration with `IObservableReader/Writer` for file-backed, reactive data grids.

---

## Key Components

### ObservableDataView\<TKey, TValue, TValueVM\>

**Purpose**: Displays a reactive MudDataGrid bound to an `IObservableReader/Writer` collection with automatic CRUD operations, toolbar, and reactive updates.

**When to Use**:
- Displaying lists of workspace documents
- Need standard CRUD operations (Create, Read, Update, Delete)
- Want built-in toolbar
- Data is backed by `IObservableReader/Writer`

**Primary Use Case**: Master list views in workspace-based applications.

#### Basic Usage

```razor
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices"
                    AllowedEditModes="EditMode.All"
                    ReadOnly=false>
    <Columns>
        <PropertyColumn Property="x => x.Value.Name" Title="Name" />
        <PropertyColumn Property="x => x.Value.Description" Title="Description" />
    </Columns>
</ObservableDataView>

@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }
}
```

#### How It Works

**Service Resolution Flow**:
```
DataServiceProvider (WorkspaceServices)
    ↓
EffectiveDataServiceProvider
    ↓ GetService<IObservableReader<TKey, TValue>>()
    ↓ GetService<IObservableWriter<TKey, TValue>>()
ViewModel.Data = reader/writer
    ↓
ObservableDataVM<TKey, TValue, TValueVM>
    ↓ Subscribes to reader.Values.Connect()
Items Observable Collection
    ↓
MudDataGrid<TValueVM>
```

**Automatic VM Creation**:
```csharp
// Component automatically creates VMs for each entity using constructor injection:
foreach (var kvp in reader.Values)
{
    var vm = VMFactory?.Invoke(kvp.Key, kvp.Value)
          ?? ActivatorUtilities.CreateInstance<TValueVM>(serviceProvider, kvp.Key, kvp.Value);
}
```

#### Parameters

**Required**:
```csharp
TKey               // Key type (usually string for file names)
TValue             // Entity type (e.g., BotEntity)
TValueVM           // ViewModel type (e.g., BotVM)
```

**Data Source**:
```csharp
[Parameter]
public IServiceProvider? DataServiceProvider { get; set; }
// Pass workspace services here to resolve IObservableReader/Writer

[Parameter]
public IObservableReader<TKey, TValue>? Data { get; set; }
// Alternative: Provide reader/writer directly
```

**CRUD Control**:
```csharp
[Parameter]
public EditMode AllowedEditModes { get; set; }
// EditMode.None, .Cell, .Form, .All

[Parameter]
public bool ReadOnly { get; set; } = true

[Parameter]
public IEnumerable<Type>? CreatableTypes { get; set; }
// Types that can be created via "Add" button

[Parameter]
public bool CanCreateValueType { get; set; } = true
```

**UI Customization**:
```csharp
[Parameter]
public RenderFragment<ObservableDataVM<TKey, TValue, TValueVM>>? ChildContent { get; set; }
// Custom rendering (instead of default MudDataGrid)

[Parameter]
public RenderFragment? Columns { get; set; }
// Column definitions

[Parameter]
public RenderFragment? EditingColumns { get; set; }
// Columns when in edit mode (overrides Columns)

[Parameter]
public RenderFragment<CellContext<TValueVM>>? ChildRowContent { get; set; }
// Expandable row content

[Parameter]
public RenderFragment<TValueVM>? ContextMenu { get; set; }
// Right-click context menu
```

**VM Factory**:
```csharp
[Parameter]
public Func<TKey, Optional<TValue>, TValueVM>? VMFactory { get; set; }
// Custom ViewModel creation
```

**Events**:
```csharp
[Parameter]
public EventHandler<DataGridRowClickEventArgs<TValueVM>>? RowClick { get; set; }
```

#### Built-In Toolbar

The component renders a toolbar with:
- **Add Button** (if `CreatableTypes` specified)
  - Dropdown if multiple types
  - Single button if one type
- **Edit Toggle** - Switches between view and edit mode
- **Delete Toggle** - Shows/hides delete column
- **Refresh Button** (if `ShowRefresh = true`)

#### Auto-Columns

If no `Columns` specified, the component generates columns automatically:

```csharp
[Parameter]
public Func<PropertyInfo, bool>? IsAutoColumn { get; set; }
// Determines which properties become columns
// Default: public, readable, standard types (string, int, etc.)

[Parameter]
public Func<PropertyInfo, bool>? IsAutoEditColumn { get; set; }
// Determines which properties are editable
// Default: public, writeable
```

#### Reactive Updates

The component automatically updates when:
- Files are added/removed from workspace directory
- Entity properties change (via `INotifyPropertyChanged`)
- Observable collections emit changes

**Under the Hood**:
```csharp
// Component subscribes to DynamicData observables
ViewModel.ItemsChanged.Subscribe(_ => InvokeAsync(StateHasChanged));

// VM subscribes to reader's observable cache
reader.Values.Connect().Subscribe(changeSet => {
    // Process adds, updates, removes
    // Update Items collection
    // Trigger UI refresh
});
```

#### Complete Example

```razor
@page "/bots"
@using LionFire.Trading.Automation

<div class="pa-6">
    <ObservableDataView @ref=ItemsEditor
                        DataServiceProvider="WorkspaceServices"
                        TKey="string"
                        TValue="BotEntity"
                        TValueVM="BotVM"
                        AllowedEditModes=EditMode.All
                        ReadOnly=false
                        CreatableTypes="@(new[] { typeof(BotEntity) })"
                        RowClick="@OnRowClick">
        <Columns>
            <!-- Status column with navigation -->
            <TemplateColumn T="BotVM">
                <HeaderTemplate>Status</HeaderTemplate>
                <CellTemplate>
                    <MudLink Href="@($"/bots/{context.Item.Key}")">
                        <MudIcon Icon="@Icons.Material.Outlined.Circle"
                                 Color="@GetStatusColor(context.Item)" />
                    </MudLink>
                </CellTemplate>
            </TemplateColumn>

            <!-- Toggle switches -->
            <TemplateColumn T="BotVM">
                <HeaderTemplate>Enabled</HeaderTemplate>
                <CellTemplate>
                    <MudSwitch T="bool"
                               @bind-Value="context.Item.Value.Enabled"
                               Color="Color.Primary"
                               Size="Size.Small" />
                </CellTemplate>
            </TemplateColumn>

            <!-- Standard columns -->
            <PropertyColumn Property="x => x.Value.Exchange" />
            <PropertyColumn Property="x => x.Value.Symbol" />
            <PropertyColumn Property="x => x.Value.Name" />

            <!-- Computed column from VM -->
            <PropertyColumn Property="x => x.AD" Title="Avg Drawdown" />
        </Columns>

        <!-- Expandable row details -->
        <ChildRowContent>
            <MudCard>
                <MudCardHeader>
                    <MudText Typo="Typo.h6">@context.Item.Value.Name</MudText>
                </MudCardHeader>
                <MudCardContent>
                    <MudText>@context.Item.Value.Description</MudText>
                    <MudText>Comments: @context.Item.Value.Comments</MudText>
                </MudCardContent>
            </MudCard>
        </ChildRowContent>

        <!-- Right-click menu -->
        <ContextMenu>
            <MudMenuItem Icon="@Icons.Material.Filled.Delete"
                         OnClick="@(() => DeleteBot(context))">
                Delete @context.Value.Name
            </MudMenuItem>
            <MudMenuItem Icon="@Icons.Material.Filled.Edit"
                         OnClick="@(() => EditBot(context))">
                Edit @context.Value.Name
            </MudMenuItem>
        </ContextMenu>
    </ObservableDataView>
</div>

@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    ObservableDataView<string, BotEntity, BotVM>? ItemsEditor { get; set; }

    private Color GetStatusColor(BotVM bot)
        => bot.Value.Enabled ? Color.Success : Color.Default;

    private void OnRowClick(object sender, DataGridRowClickEventArgs<BotVM> e)
    {
        NavigationManager.NavigateTo($"/bots/{e.Item.Key}");
    }

    private void DeleteBot(BotVM bot) { /* ... */ }
    private void EditBot(BotVM bot) { /* ... */ }
}
```

---

### AsyncVMSourceCacheView\<TKey, TValue, TValueVM\>

**Purpose**: Similar to ObservableDataView but works directly with `SourceCache<TValue, TKey>` from DynamicData.

**When to Use**:
- Data is already in a `SourceCache`
- Not using `IObservableReader/Writer` pattern
- Need reactive collection without file persistence

**Usage**:
```razor
<AsyncVMSourceCacheView TKey="string"
                        TValue="BotEntity"
                        TValueVM="BotVM"
                        SourceCache="@MySourceCache">
    <Columns>
        <!-- Column definitions -->
    </Columns>
</AsyncVMSourceCacheView>
```

---

### KeyedCollectionView\<TKey, TValue\>

**Purpose**: Displays collections with keys, without ViewModels.

**When to Use**:
- Simple display, no VM needed
- Read-only data
- Don't need MVVM features

---

### KeyedVMCollectionView\<TKey, TValue, TValueVM\>

**Purpose**: Displays collections with explicit ViewModels.

**When to Use**:
- Have pre-created VMs
- Don't need observable reader/writer
- Manual VM lifecycle management

---

## Supporting Components

### ObservableDataVM\<TKey, TValue, TValueVM\>

**ViewModel** used internally by `ObservableDataView`. Handles:
- Subscribing to `IObservableReader.Values.Connect()`
- Creating VMs for entities
- Managing observable items collection
- CRUD command coordination
- Edit mode state

**Properties**:
```csharp
public IObservableReader<TKey, TValue>? Data { get; set; }
public IObservableCache<TValueVM, TKey> Items { get; }
public IObservable<IChangeSet<TValueVM, TKey>> ItemsChanged { get; }
public bool CanCreate { get; }
public bool CanDelete { get; }
public bool ShowDeleteColumn { get; set; }
public EditMode AllowedEditModes { get; set; }
```

---

## Utility Classes

### MudDataGridUtilities

Static helpers for building MudDataGrid columns programmatically.

**Methods**:
```csharp
public static void BuildPropertyColumn<TValueVM, TValue>(
    RenderTreeBuilder builder,
    PropertyInfo prop,
    string? propertyPrefix = null)
```

Creates a `PropertyColumn` for the given property.

### AutoColumnUtils

Determines which properties should auto-generate columns.

**Methods**:
```csharp
public static bool DefaultIsAutoColumn(PropertyInfo prop)
// Returns true for: public, readable, primitive/string types

public static bool DefaultIsAutoEditColumn(PropertyInfo prop)
// Returns true for: public, writeable, primitive/string types
```

---

## Dependencies

### NuGet Packages

```xml
<PackageReference Include="MudBlazor" />
<PackageReference Include="ReactiveUI.Blazor" />
<PackageReference Include="DynamicData" />
```

### LionFire Dependencies

```xml
<ProjectReference Include="..\LionFire.Data.Async.Mvvm\" />
<ProjectReference Include="..\LionFire.Reactive\" />
<ProjectReference Include="..\LionFire.Mvvm\" />
<ProjectReference Include="..\LionFire.Blazor.Components\" />
```

---

## Architecture Integration

### Service Resolution Pattern

```
Blazor Component (@page "/bots")
    ↓ Receives via CascadingParameter
WorkspaceServices (IServiceProvider)
    ↓ Passes to
ObservableDataView (DataServiceProvider parameter)
    ↓ Resolves
IObservableReader<string, BotEntity>
IObservableWriter<string, BotEntity>
    ↓ Wraps in
ObservableDataVM<string, BotEntity, BotVM>
    ↓ Creates
ObservableCache<BotVM, string> (Items)
    ↓ Binds to
MudDataGrid<BotVM>
```

### Reactive Update Flow

```
File System Change (Bots/bot1.hjson modified)
    ↓
HjsonFsDirectoryReaderRx (detects change)
    ↓
IObservableReader.Values.Connect() (emits changeset)
    ↓
ObservableDataVM (processes changeset)
    ↓
Items.Edit(changes => ...) (updates cache)
    ↓
MudDataGrid (reactive binding detects change)
    ↓
UI Updates
```

---

## Common Patterns

### Pattern 1: Read-Only List

```razor
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices"
                    ReadOnly="true"
                    AllowedEditModes="EditMode.None">
    <Columns>
        <!-- Columns -->
    </Columns>
</ObservableDataView>
```

### Pattern 2: Editable List with Add

```razor
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices"
                    AllowedEditModes="EditMode.All"
                    CreatableTypes="@(new[] { typeof(BotEntity) })"
                    ReadOnly="false">
    <Columns>
        <!-- Columns -->
    </Columns>
</ObservableDataView>
```

### Pattern 3: Custom VM Factory

```razor
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices"
                    VMFactory="@CreateVM">
    <Columns>
        <!-- Columns -->
    </Columns>
</ObservableDataView>

@code {
    private BotVM CreateVM(string key, Optional<BotEntity> entity)
    {
        if (!entity.HasValue) return null;

        var vm = new BotVM(key, entity.Value);
        // Custom initialization
        vm.LoadAdditionalData();
        return vm;
    }
}
```

### Pattern 4: Navigation to Detail

```razor
<ObservableDataView ...>
    <Columns>
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

---

## Performance Considerations

### Virtualization

For large datasets, enable MudDataGrid virtualization:

```razor
<ObservableDataView ...>
    <!-- MudDataGrid is virtualized by default for performance -->
</ObservableDataView>
```

### Subscription Management

The component properly manages subscriptions:
- Subscribes in `OnParametersSetAsync`
- Disposes subscriptions in `DisposeAsync`
- Uses `CompositeDisposable` for cleanup

### Reactive Updates

Uses DynamicData's efficient change detection:
- Only updates changed items
- Batches multiple changes
- Minimal UI re-renders via `StateHasChanged` throttling

---

## Troubleshooting

### Issue: Component shows empty grid

**Check**:
1. Is `DataServiceProvider` set? (Should be `@WorkspaceServices`)
2. Are `IObservableReader/Writer` services registered in workspace?
3. Check console for errors
4. Verify data exists:
   ```csharp
   var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
   Console.WriteLine($"Keys: {string.Join(", ", reader?.Keys.Items ?? Enumerable.Empty<string>())}");
   ```

### Issue: "Unable to resolve service"

**Cause**: Using root DI container instead of workspace services.

**Solution**: Pass `WorkspaceServices` to `DataServiceProvider`:
```razor
<ObservableDataView DataServiceProvider="@WorkspaceServices" ... />
```

### Issue: Changes not reflected in UI

**Check**:
1. Does entity implement `INotifyPropertyChanged`?
2. Is entity using `ReactiveObject` or similar?
3. Are properties marked `[Reactive]` or raising `PropertyChanged`?

```csharp
// ❌ Wrong - no change notifications
public class BotEntity
{
    public string Name { get; set; }
}

// ✅ Right - reactive properties
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string _name;
}
```

### Issue: VM not created correctly

**Check**:
1. Does VM have constructor: `MyVM(TKey key, TValue value)`?
2. Is VM type public and instantiable?
3. Try providing custom `VMFactory` to debug:
   ```csharp
   VMFactory="@((key, entity) => {
       Console.WriteLine($"Creating VM for {key}");
       return new MyVM(key, entity.Value);
   })"
   ```

---

## Related Documentation

- **[Blazor MVVM Patterns](../../docs/ui/blazor-mvvm-patterns.md)** - When to use ObservableDataView vs manual pattern
- **[Workspace Service Scoping](../../docs/architecture/workspaces/service-scoping.md)** - Understanding workspace services
- **[LionFire.Data.Async.Mvvm](../LionFire.Data.Async.Mvvm/CLAUDE.md)** - ViewModels and reactive patterns
- **[LionFire.Reactive](../LionFire.Reactive/CLAUDE.md)** - Observable readers/writers
- **[MudBlazor Documentation](https://mudblazor.com)** - Underlying component library

---

## Summary

`LionFire.Blazor.Components.MudBlazor` provides **high-level reactive components** that bridge MudBlazor's UI with LionFire's MVVM and reactive persistence patterns.

**Primary Component**: `ObservableDataView` - Automatic reactive data grid with CRUD for workspace documents.

**Use When**: Building list views for workspace-scoped, file-backed entities with minimal code.

**Key Benefit**: 20-30 lines of code for a fully functional, reactive, CRUD-enabled data grid with workspace integration.
