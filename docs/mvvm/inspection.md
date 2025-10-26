# Object Inspection System

## Overview

LionFire's **Object Inspection System** provides runtime introspection and editing of objects through a property grid interface. This system enables building **generic property editors**, **debugging tools**, and **configuration UIs** without hardcoding property names or types.

**Key Concept**: Inspect any object at runtime and generate a property grid UI automatically.

---

## Table of Contents

1. [What is Object Inspection?](#what-is-object-inspection)
2. [Core Components](#core-components)
3. [InspectorView Component](#inspectorview-component)
4. [Inspector System Architecture](#inspector-system-architecture)
5. [Custom Inspectors](#custom-inspectors)
6. [Property Editors](#property-editors)
7. [Building Custom Property Grids](#building-custom-property-grids)
8. [Advanced Patterns](#advanced-patterns)

---

## What is Object Inspection?

### The Problem

You need to display/edit an object's properties in a UI, but:
- Don't want to hardcode each property
- Object types vary at runtime
- Need to support nested objects
- Want type-appropriate editors (text, number, dropdown, etc.)

### The Solution

**Object Inspection** - Automatically discover properties and generate UI:

```csharp
// Any object
var myBot = new BotEntity
{
    Name = "Trading Bot",
    Exchange = "Binance",
    Enabled = true,
    Timeout = 30
};

// Inspect it
<InspectorView Object="@myBot" />
```

**Result**: Automatic property grid with appropriate editors for each property type.

---

## Core Components

### Libraries

**LionFire.Inspection**
- Core inspection abstractions
- `IInspector`, `IInspectedNode`
- Inspector service and context
- Group-based inspection system

**LionFire.Mvvm**
- Inspection ViewModels
- `InspectorVM`, `NodeVM`
- Reflection-based member ViewModels
- Integration with ReactiveUI

**LionFire.Blazor.Components.MudBlazor**
- Blazor UI components
- `InspectorView.razor`
- Property grid rendering
- Cell editors (text, numeric, select, date)

---

## InspectorView Component

**Location**: `LionFire.Blazor.Components.MudBlazor`

**Purpose**: Displays a property grid for any object with automatic property detection and type-appropriate editors.

### Basic Usage

```razor
@using LionFire.Blazor.Components.MudBlazor_
@inject IServiceProvider ServiceProvider

<InspectorView Object="@myBot" />

@code {
    BotEntity myBot = new BotEntity
    {
        Name = "Bot 1",
        Description = "Trading bot",
        Enabled = true,
        Timeout = 30
    };
}
```

**Result**: Automatic property grid showing:
- Name (text field)
- Description (text field)
- Enabled (checkbox/switch)
- Timeout (numeric field)

---

### Features

#### Filter Toggles

Built-in toolbar for filtering displayed members:

```
[D/d] - Data Members (properties/fields)
[E/e] - Events
[M/m] - Methods
[Diag/diag] - Diagnostics Mode
[Dev/dev] - Developer Mode
[H/h] - Hidden Mode
```

**Usage**:
```csharp
// Show filter toggles
vm.ShowFilterTypes = true;

// Configure what to show
vm.NodeVM.ShowDataMembers = true;   // Properties/fields
vm.NodeVM.ShowEvents = false;       // Events
vm.NodeVM.ShowMethods = false;      // Methods
```

---

#### Nested Object Support

Automatically handles nested objects:

```csharp
public class BotEntity
{
    public string? Name { get; set; }
    public BotConfig Config { get; set; }  // Nested object
}

public class BotConfig
{
    public int Timeout { get; set; }
    public string? ApiKey { get; set; }
}
```

**Inspector automatically expands**:
```
▼ Bot
  ├─ Name: "Trading Bot"
  └─▼ Config
      ├─ Timeout: 30
      └─ ApiKey: "abc123"
```

---

#### Read-Only vs Editable

```razor
<!-- Read-only inspection -->
<InspectorView Object="@myBot" ReadOnly="true" />

<!-- Editable inspection -->
<InspectorView Object="@myBot" ReadOnly="false" />
```

---

### Parameters

```csharp
[Parameter]
public object? Object { get; set; }  // Object to inspect

// ReadOnly support (future)
// [Parameter]
// public bool ReadOnly { get; set; }
```

---

## Inspector System Architecture

### Components Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                    InspectorView.razor                       │
│  Blazor Component                                           │
│  - Renders property grid                                    │
│  - Filter toggles                                           │
└────────────────────┬────────────────────────────────────────┘
                     │ Uses
                     ↓
┌─────────────────────────────────────────────────────────────┐
│                       InspectorVM                            │
│  ViewModel Layer                                            │
│  - Source: object?                                          │
│  - NodeVM: NodeVM                                           │
│  - ShowFilterTypes, DevMode                                 │
└────────────────────┬────────────────────────────────────────┘
                     │ Contains
                     ↓
┌─────────────────────────────────────────────────────────────┐
│                         NodeVM                               │
│  Node ViewModel                                             │
│  - Node: IInspectedNode                                     │
│  - Options: InspectorOptions                                │
│  - ShowDataMembers, ShowEvents, ShowMethods                 │
└────────────────────┬────────────────────────────────────────┘
                     │ Wraps
                     ↓
┌─────────────────────────────────────────────────────────────┐
│                    InspectedNode                             │
│  Inspection Model                                           │
│  - Source: object                                           │
│  - Groups: INodeGroup[] (Properties, Methods, Events, etc.) │
│  - Children: INode[]                                        │
└────────────────────┬────────────────────────────────────────┘
                     │ Analyzed by
                     ↓
┌─────────────────────────────────────────────────────────────┐
│                      IInspector[]                            │
│  Inspector Implementations                                  │
│  - ReflectionInspector (properties, methods, fields)        │
│  - Custom inspectors (user-defined)                         │
└─────────────────────────────────────────────────────────────┘
```

---

### Inspection Flow

```
1. User sets InspectorView.Object = myBot
    ↓
2. InspectorVM.Source = myBot
    ↓
3. NodeVM created for myBot (via WhenAnyValue)
    ↓
4. InspectedNode wraps myBot
    ↓
5. IInspector[] attach to node
    ↓
6. ReflectionInspector analyzes type
    ↓
7. Creates INodeGroup[] for:
   - Properties
   - Fields
   - Methods
   - Events
    ↓
8. Each property becomes IInspectorMember
    ↓
9. InspectorView renders property grid
    ↓
10. User edits property
    ↓
11. PropertyChanged event fires
    ↓
12. UI updates reactively
```

---

## Custom Inspectors

### What Are Custom Inspectors?

**Custom Inspectors** allow you to define how specific types are inspected, beyond default reflection.

**Use Cases**:
- Hide sensitive properties
- Add computed properties
- Custom grouping
- Type-specific metadata

### IInspector Interface

```csharp
public interface IInspector
{
    // Attach inspector to node
    IDisposable? Attach(IInspectedNode node);

    // Check if inspector supports type
    bool IsSourceTypeSupported(Type sourceType);

    // Group definitions
    IReadOnlyDictionary<string, GroupInfo> GroupInfos { get; }

    // Base attachment type
    Type BaseAttachmentType(Type type);
}
```

---

### Creating a Custom Inspector

```csharp
public class BotEntityInspector : IInspector
{
    public IReadOnlyDictionary<string, GroupInfo> GroupInfos { get; } =
        new Dictionary<string, GroupInfo>
        {
            ["BotStatus"] = new GroupInfo
            {
                Key = "BotStatus",
                DisplayName = "Bot Status",
                IsTypeSupported = t => t == typeof(BotEntity)
            },
            ["TradingParams"] = new GroupInfo
            {
                Key = "TradingParams",
                DisplayName = "Trading Parameters"
            }
        };

    public IDisposable? Attach(IInspectedNode node)
    {
        if (node.Source is not BotEntity bot)
            return null;

        // Add custom groups
        var statusGroup = new CustomGroup("BotStatus", "Bot Status");
        statusGroup.Members.Add(new ComputedMember("IsRunning", () => bot.Enabled));
        statusGroup.Members.Add(new ComputedMember("Uptime", () => bot.GetUptime()));
        node.Groups.Add(statusGroup);

        var paramsGroup = new CustomGroup("TradingParams", "Trading Parameters");
        paramsGroup.Members.Add(new PropertyMember(bot, nameof(bot.Symbol)));
        paramsGroup.Members.Add(new PropertyMember(bot, nameof(bot.Timeframe)));
        node.Groups.Add(paramsGroup);

        return new AttachedInspector(this, node);
    }

    public bool IsSourceTypeSupported(Type sourceType)
        => sourceType == typeof(BotEntity);

    public Type BaseAttachmentType(Type type) => type;
}
```

**Registration**:
```csharp
services.AddSingleton<IInspector, BotEntityInspector>();
```

---

## Property Editors

### Built-In Editors

**Location**: `LionFire.Blazor.Components.MudBlazor/PropertyGrid/CellEditors/`

| Type | Editor Component | Example |
|------|------------------|---------|
| **string** | `MudTextInspector.razor` | Text field |
| **int, decimal, etc.** | `MudNumericInspector.razor` | Numeric field |
| **bool** | `MudSwitch` | Toggle switch |
| **DateTime** | `MudDateInspector.razor` | Date picker |
| **Enum** | `MudSelectInspector.razor` | Dropdown select |

---

### Custom Property Editor

**Scenario**: Need custom editor for a specific type.

```razor
<!-- CustomColorEditor.razor -->
@using LionFire.Blazor.Components.MudBlazor_.PropertyGrid

<div class="inspector-cell">
    <MudColorPicker @bind-Value="colorValue"
                    Label="@Label"
                    DisableAlpha="true" />
</div>

@code {
    [Parameter] public Color Value { get; set; }
    [Parameter] public EventCallback<Color> ValueChanged { get; set; }
    [Parameter] public string? Label { get; set; }

    private MudBlazor.Color colorValue
    {
        get => new MudBlazor.Color(Value.R, Value.G, Value.B);
        set
        {
            var newColor = new Color(value.R, value.G, value.B);
            ValueChanged.InvokeAsync(newColor);
        }
    }
}
```

**Registration** (via custom inspector or template):
```csharp
// In custom inspector
public class CustomEditorRegistry
{
    public static Dictionary<Type, Type> EditorTypes = new()
    {
        [typeof(Color)] = typeof(CustomColorEditor)
    };
}
```

---

## Building Custom Property Grids

### Scenario 1: Simple Config Editor

```razor
@page "/settings"
@inject IServiceProvider ServiceProvider

<MudCard>
    <MudCardHeader>
        <MudText Typo="Typo.h5">Application Settings</MudText>
    </MudCardHeader>
    <MudCardContent>
        <InspectorView Object="@appSettings" />
    </MudCardContent>
    <MudCardActions>
        <MudButton OnClick="SaveSettings" Color="Color.Primary">Save</MudButton>
    </MudCardActions>
</MudCard>

@code {
    AppSettings appSettings = new();

    protected override void OnInitialized()
    {
        // Load settings
        appSettings = settingsService.Load();
    }

    private async Task SaveSettings()
    {
        await settingsService.Save(appSettings);
    }
}
```

---

### Scenario 2: Multi-Object Editor

```razor
<MudTabs>
    <MudTabPanel Text="Bot Configuration">
        <InspectorView Object="@bot" />
    </MudTabPanel>
    <MudTabPanel Text="Connection Settings">
        <InspectorView Object="@bot.ConnectionSettings" />
    </MudTabPanel>
    <MudTabPanel Text="Risk Parameters">
        <InspectorView Object="@bot.RiskParameters" />
    </MudTabPanel>
</MudTabs>
```

---

### Scenario 3: Filtered Property Display

```csharp
// Custom inspector to hide sensitive properties
public class SecureConfigInspector : IInspector
{
    public IDisposable? Attach(IInspectedNode node)
    {
        if (node.Source is not SecureConfig config)
            return null;

        // Only show non-sensitive properties
        var safeGroup = new CustomGroup("SafeProperties", "Configuration");

        var type = config.GetType();
        foreach (var prop in type.GetProperties())
        {
            // Skip properties with [Sensitive] attribute
            if (prop.GetCustomAttribute<SensitiveAttribute>() != null)
                continue;

            safeGroup.Members.Add(new PropertyMember(config, prop.Name));
        }

        node.Groups.Add(safeGroup);
        return new AttachedInspector(this, node);
    }

    public bool IsSourceTypeSupported(Type sourceType)
        => sourceType == typeof(SecureConfig);

    public IReadOnlyDictionary<string, GroupInfo> GroupInfos { get; } = ...;
    public Type BaseAttachmentType(Type type) => type;
}
```

---

## Advanced Patterns

### Pattern 1: Inspection with ViewModels

```csharp
public class BotInspectorVM : ReactiveObject
{
    [Reactive] private BotEntity? _bot;
    [Reactive] private object? _inspectedObject;

    public BotInspectorVM()
    {
        // When bot changes, update inspected object
        this.WhenAnyValue(x => x.Bot)
            .Subscribe(bot =>
            {
                // Inspect the entity itself
                InspectedObject = bot;
            });
    }
}
```

**UI**:
```razor
<MudGrid>
    <MudItem xs="6">
        <MudCard>
            <MudCardHeader>Bot Details</MudCardHeader>
            <MudCardContent>
                <MudTextField @bind-Value="vm.Bot.Name" />
                <MudButton OnClick="InspectBot">Inspect</MudButton>
            </MudCardContent>
        </MudCard>
    </MudItem>
    <MudItem xs="6">
        <MudCard>
            <MudCardHeader>Property Inspector</MudCardHeader>
            <MudCardContent>
                <InspectorView Object="@vm.InspectedObject" />
            </MudCardContent>
        </MudCard>
    </MudItem>
</MudGrid>
```

---

### Pattern 2: Multi-Type Inspector

```csharp
public class MultiTypeInspectorVM : ReactiveObject
{
    [Reactive] private object? _selectedObject;

    public MultiTypeInspectorVM()
    {
        Objects = new ObservableCollection<InspectableItem>
        {
            new("Bot 1", new BotEntity { Name = "Bot 1" }),
            new("Portfolio", new Portfolio { Name = "Main" }),
            new("Strategy", new Strategy { Name = "Scalping" })
        };

        this.WhenAnyValue(x => x.SelectedItem)
            .Subscribe(item => SelectedObject = item?.Object);
    }

    public ObservableCollection<InspectableItem> Objects { get; }

    [Reactive] private InspectableItem? _selectedItem;
}

public record InspectableItem(string Name, object Object);
```

**UI**:
```razor
<MudSelect @bind-Value="vm.SelectedItem" Label="Select Object">
    @foreach (var item in vm.Objects)
    {
        <MudSelectItem Value="@item">@item.Name</MudSelectItem>
    }
</MudSelect>

<InspectorView Object="@vm.SelectedObject" />
```

---

### Pattern 3: Inspection with Validation

```csharp
public class ValidatingInspectorVM : ReactiveObject
{
    [Reactive] private BotEntity? _bot;
    [Reactive] private List<string> _validationErrors = new();

    public ValidatingInspectorVM()
    {
        // Validate when properties change
        this.WhenAnyValue(x => x.Bot)
            .Where(bot => bot != null)
            .Subscribe(bot =>
            {
                ValidationErrors.Clear();

                // Validate via inspection
                if (string.IsNullOrEmpty(bot.Name))
                    ValidationErrors.Add("Name is required");

                if (bot.Timeout < 1 || bot.Timeout > 300)
                    ValidationErrors.Add("Timeout must be between 1 and 300 seconds");
            });
    }
}
```

**UI**:
```razor
<InspectorView Object="@vm.Bot" />

@if (vm.ValidationErrors.Any())
{
    <MudAlert Severity="Severity.Error">
        @foreach (var error in vm.ValidationErrors)
        {
            <div>@error</div>
        }
    </MudAlert>
}
```

---

## Inspector Options

### InspectorOptions

```csharp
public class InspectorOptions
{
    public bool ShowDataMembers { get; set; } = true;
    public bool ShowEvents { get; set; } = false;
    public bool ShowMethods { get; set; } = false;

    public bool DiagnosticsMode { get; set; } = false;
    public bool DevMode { get; set; } = true;
    public bool HiddenMode { get; set; } = false;

    public bool ShowAll { get; set; } = false;
}
```

**Usage**:
```csharp
var inspector = new InspectorVM(vmProvider, inspectorService);
inspector.NodeVM.Options.ShowMethods = true;
inspector.NodeVM.Options.DiagnosticsMode = true;
```

---

## Use Cases

### Use Case 1: Configuration Editor

**Scenario**: Generic configuration editor for HJSON files.

```razor
@page "/config/{ConfigId}"

<MudCard>
    <MudCardHeader>
        <MudText Typo="Typo.h5">Configuration: @ConfigId</MudText>
    </MudCardHeader>
    <MudCardContent>
        @if (config != null)
        {
            <InspectorView Object="@config" ReadOnly="false" />
        }
    </MudCardContent>
    <MudCardActions>
        <MudButton OnClick="Save" Color="Color.Primary">Save</MudButton>
    </MudCardActions>
</MudCard>

@code {
    [Parameter] public string? ConfigId { get; set; }

    object? config;

    protected override async Task OnInitializedAsync()
    {
        // Load config dynamically
        config = await configService.Load(ConfigId);
    }

    private async Task Save()
    {
        await configService.Save(ConfigId, config);
    }
}
```

**Benefit**: Works with any config type without hardcoding properties.

---

### Use Case 2: Debug Inspector

**Scenario**: Runtime object inspection for debugging.

```razor
<MudDrawer Open="@showDebugger" Anchor="Anchor.End" Width="400px">
    <MudCard>
        <MudCardHeader>
            <MudText Typo="Typo.h6">Object Inspector</MudText>
        </MudCardHeader>
        <MudCardContent>
            <InspectorView Object="@selectedObject" />
        </MudCardContent>
    </MudCard>
</MudDrawer>

@code {
    bool showDebugger = false;
    object? selectedObject;

    // Called from anywhere in app
    public void InspectObject(object obj)
    {
        selectedObject = obj;
        showDebugger = true;
    }
}
```

---

### Use Case 3: Dynamic Form Builder

**Scenario**: Build forms from metadata at runtime.

```csharp
public class FormBuilderVM : ReactiveObject
{
    public FormBuilderVM(Type entityType)
    {
        // Create instance
        var entity = Activator.CreateInstance(entityType);

        // Inspect to build form
        InspectedObject = entity;
    }

    [Reactive] private object? _inspectedObject;
}
```

**UI**:
```razor
<InspectorView Object="@vm.InspectedObject" />
```

**Use Case**: Admin panels, CMS editors, generic CRUD forms.

---

## Performance Considerations

### Type Metadata Caching

**Inspector system caches type metadata**:
- First inspection: ~50-200ms (reflection + analysis)
- Subsequent inspections: ~1-10ms (cached metadata)

**Optimization**: Type metadata is cached globally, so repeated inspections of same type are fast.

---

### Large Object Graphs

**Problem**: Inspecting deeply nested objects can be slow.

**Solutions**:
1. **Lazy expansion**: Only load children when expanded
2. **Depth limiting**: Don't inspect beyond certain depth
3. **Property filtering**: Only show relevant properties

---

## Comparison to Alternatives

| Approach | Pros | Cons |
|----------|------|------|
| **InspectorView** | Zero code, works with any object, type-appropriate editors | Less control, generic appearance |
| **Custom Form** | Full control, custom validation, branded UX | High code, maintenance overhead |
| **Hybrid** | Custom form with InspectorView for subsections | Balanced | More complex |

**Recommendation**: Use InspectorView for:
- Configuration editors
- Debug tools
- Admin panels
- Rapid prototyping

Use custom forms for:
- User-facing production UIs
- Complex validation
- Custom workflows

---

## Related Documentation

- **[ViewModels Guide](viewmodels-guide.md)** - ViewModel patterns
- **[Data Binding](data-binding.md)** - UI binding
- **[Component Catalog](../ui/component-catalog.md)** - InspectorView component
- **[LionFire.Mvvm](../../src/LionFire.Mvvm/CLAUDE.md)** - Inspection implementation
- **[LionFire.Mvvm.Abstractions](../../src/LionFire.Mvvm.Abstractions/CLAUDE.md)** - Inspection interfaces

---

## Summary

**Object Inspection System** provides:

**Key Features**:
- **Automatic property detection** via reflection
- **Type-appropriate editors** (text, number, date, enum, etc.)
- **Nested object support** with expandable hierarchy
- **Custom inspectors** for type-specific behavior
- **Filtering** (properties, methods, events)
- **Blazor component** for property grid UI

**Primary Component**: `InspectorView` - Drop-in property grid for any object.

**Use Cases**:
- Configuration editors
- Debug/diagnostic tools
- Generic CRUD forms
- Admin panels
- Property browsers

**Performance**: Type metadata cached globally, first inspection ~50-200ms, subsequent ~1-10ms.

**Alternative**: Build custom forms for user-facing UIs where full control is needed.
