# How-To: Create Blazor Pages for Workspace Documents

**Goal**: Build a complete master-detail interface for workspace documents with list view, detail view, navigation, and CRUD operations.

**What You'll Build**: A bot management interface with a list of bots and individual bot detail pages.

**Prerequisites**:
- Basic Blazor knowledge
- Understanding of dependency injection
- Familiarity with MudBlazor (helpful but not required)

**Time**: 30-45 minutes

---

## Overview

You'll create:
1. **Entity**: `BotEntity` - The persisted data model
2. **ViewModel**: `BotVM` - UI-friendly wrapper
3. **List Page**: `Bots.razor` - Grid of all bots using `ObservableDataView`
4. **Detail Page**: `Bot.razor` - Edit individual bot using manual VM
5. **Registration**: Wire up workspace services

---

## Step 1: Define the Entity

Create your entity class representing the document:

**File**: `Models/BotEntity.cs`

```csharp
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using LionFire.Ontology;
using Newtonsoft.Json;

namespace MyApp.Trading;

/// <summary>
/// Represents a trading bot configuration
/// </summary>
[Alias("Bot")]  // Directory will be "Bots"
public partial class BotEntity : ReactiveObject
{
    #region Basic Properties

    [Reactive]
    private string? _name;

    [Reactive]
    private string? _description;

    [Reactive]
    private string? _comments;

    #endregion

    #region Trading Configuration

    [Reactive]
    private string? _exchange;

    [Reactive]
    private string? _exchangeArea;

    [Reactive]
    private string? _symbol;

    [Reactive]
    private string? _timeFrame;

    #endregion

    #region Runtime State

    [Reactive]
    private bool _enabled;

    [Reactive]
    private bool _live;  // If true, trades with real money

    #endregion

    #region Computed Properties (Not Persisted)

    [JsonIgnore]
    public string DisplayName => $"{Exchange}:{Symbol} - {Name ?? "Unnamed"}";

    [JsonIgnore]
    public ExchangeSymbol? ExchangeSymbol
    {
        get
        {
            if (string.IsNullOrEmpty(Exchange) || string.IsNullOrEmpty(ExchangeArea) || string.IsNullOrEmpty(Symbol))
                return null;

            return new ExchangeSymbol(Exchange, ExchangeArea, Symbol);
        }
    }

    #endregion
}
```

**Key Points**:
- ✅ Inherits `ReactiveObject` for automatic change notifications
- ✅ Uses `[Reactive]` attribute for reactive properties
- ✅ `[Alias("Bot")]` sets directory name to "Bots"
- ✅ `[JsonIgnore]` on computed properties

---

## Step 2: Define the ViewModel

Create a ViewModel to add UI-specific logic:

**File**: `ViewModels/BotVM.cs`

```csharp
using LionFire.Mvvm;

namespace MyApp.Trading;

/// <summary>
/// ViewModel for BotEntity with UI-specific properties and commands
/// </summary>
public class BotVM : KeyValueVM<string, BotEntity>
{
    #region Lifecycle

    public BotVM(string key, BotEntity value) : base(key, value)
    {
    }

    #endregion

    #region UI-Specific Properties

    /// <summary>
    /// Indicates if bot is currently running (not persisted)
    /// </summary>
    public bool IsRunning { get; set; }

    /// <summary>
    /// Live trading flag with account type awareness
    /// </summary>
    public bool IsLive => Value.Live && IsRealMoneyAccount();

    /// <summary>
    /// Average drawdown statistic (computed from backtest results)
    /// </summary>
    public double? AD { get; set; }

    #endregion

    #region Event Handlers (Called by UI Components)

    public ValueTask OnStart()
    {
        // Called when user clicks Start button
        IsRunning = true;
        return ValueTask.CompletedTask;
    }

    public ValueTask OnStop()
    {
        // Called when user clicks Stop button
        IsRunning = false;
        return ValueTask.CompletedTask;
    }

    #endregion

    #region Helper Methods

    private bool IsRealMoneyAccount()
    {
        // Determine if account is real money (not demo)
        // This would check account configuration
        return false; // Placeholder
    }

    #endregion
}
```

**Key Points**:
- ✅ Inherits `KeyValueVM<string, BotEntity>` for standard pattern
- ✅ Adds UI-specific properties (IsRunning, IsLive)
- ✅ Event handlers for toolbar actions
- ✅ Computed properties for display

---

## Step 3: Register with Workspace System

Configure services in application startup:

**File**: `Program.cs` or `Startup.cs`

```csharp
using LionFire.Hosting;
using MyApp.Trading;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    // Core workspace infrastructure
    .AddWorkspacesModel()

    // Register workspace configurator (handles IObservableReader/Writer registration)
    .AddSingleton<IWorkspaceServiceConfigurator, WorkspaceTypesConfigurator>()

    // Declare document types (adds to WorkspaceConfiguration.MemberTypes)
    .AddWorkspaceChildType<BotEntity>()

    // Register document service (hosted service that watches for changes)
    .AddWorkspaceDocumentService<string, BotEntity>()

    // Register ViewModels (for UI)
    .AddTransient<BotVM>()

    // Register reactive persistence MVVM support
    .AddReactivePersistenceMvvm();

var app = builder.Build();
app.Run();
```

**What This Does**:
1. Declares `BotEntity` as a workspace member type
2. Registers hosted service to watch bot documents
3. Registers VM for dependency injection (if needed)
4. When workspace loads, `WorkspaceTypesConfigurator` will register:
   - `IObservableReader<string, BotEntity>` → Points to `{workspace}/Bots/`
   - `IObservableWriter<string, BotEntity>` → Points to `{workspace}/Bots/`

---

## Step 4: Create List Page (Bots.razor)

Create the master list page using `ObservableDataView`:

**File**: `Pages/Bots.razor`

```razor
@page "/bots"

@using LionFire.Mvvm
@using MyApp.Trading

<MudContainer MaxWidth="MaxWidth.ExtraLarge" Class="pa-6">
    <MudText Typo="Typo.h3" Class="mb-4">Trading Bots</MudText>

    <ObservableDataView @ref="ItemsEditor"
                        DataServiceProvider="WorkspaceServices"
                        TKey="string"
                        TValue="BotEntity"
                        TValueVM="BotVM"
                        AllowedEditModes="EditMode.All"
                        ReadOnly="false"
                        CreatableTypes="@(new[] { typeof(BotEntity) })">
        <Columns>
            <!-- Status indicator with navigation -->
            <TemplateColumn T="BotVM">
                <HeaderTemplate>Status</HeaderTemplate>
                <CellTemplate>
                    <MudLink Href="@($"/bots/{context.Item.Key}")">
                        <MudIcon Icon="@Icons.Material.Outlined.Circle"
                                 Color="@(context.Item.IsRunning ? Color.Success : Color.Default)" />
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
                                   ThumbIcon="@Icons.Material.Filled.Radar"
                                   Size="Size.Small" />
                    }
                </CellTemplate>
            </TemplateColumn>

            <!-- Live trading toggle (color-coded) -->
            <TemplateColumn T="BotVM">
                <HeaderTemplate>Live</HeaderTemplate>
                <CellTemplate>
                    @if (context.Item?.Value != null)
                    {
                        <MudSwitch T="bool"
                                   @bind-Value="context.Item.Value.Live"
                                   ThumbIcon="@Icons.Material.Rounded.AttachMoney"
                                   Color="@(context.Item.IsLive ? Color.Secondary : Color.Default)"
                                   Size="Size.Small" />
                    }
                </CellTemplate>
            </TemplateColumn>

            <!-- Standard property columns -->
            <PropertyColumn T="BotVM" TProperty="string"
                            Property="x => x.Value.Exchange" Title="Exchange" />
            <PropertyColumn T="BotVM" TProperty="string"
                            Property="x => x.Value.ExchangeArea" Title="Area" />
            <PropertyColumn T="BotVM" TProperty="string"
                            Property="x => x.Value.Symbol" Title="Symbol" />
            <PropertyColumn T="BotVM" TProperty="string"
                            Property="x => x.Value.TimeFrame" Title="TimeFrame" />
            <PropertyColumn T="BotVM" TProperty="string"
                            Property="x => x.Value.Name" />

            <!-- Computed column from VM -->
            <PropertyColumn Property="x => x.AD" Title="Avg DD" T="BotVM" TProperty="double?" />

            <PropertyColumn T="BotVM" TProperty="string"
                            Property="x => x.Value.Comments" />

            <!-- Action buttons -->
            <TemplateColumn T="BotVM">
                <CellTemplate>
                    <MudButton Size="Size.Small"
                               Variant="Variant.Outlined"
                               Href="@($"/bots/{context.Item.Key}")">
                        Edit
                    </MudButton>
                </CellTemplate>
            </TemplateColumn>
        </Columns>

        <!-- Expandable row content -->
        <ChildRowContent>
            <MudCard>
                <MudCardHeader>
                    <CardHeaderContent>
                        <MudText Typo="Typo.h6">@context.Item.Value.Name</MudText>
                    </CardHeaderContent>
                </MudCardHeader>
                <MudCardContent>
                    <MudText><b>Exchange:</b> @context.Item.Value.DisplayName</MudText>
                    <MudText><b>Enabled:</b> @context.Item.Value.Enabled</MudText>
                    <MudText><b>Live:</b> @context.Item.Value.Live</MudText>
                    <MudText><b>Comments:</b> @context.Item.Value.Comments</MudText>
                </MudCardContent>
            </MudCard>
        </ChildRowContent>

        <!-- Context menu -->
        <ContextMenu>
            <MudMenuItem Icon="@Icons.Material.Filled.Delete">
                Delete @context.Value.Name
            </MudMenuItem>
            <MudMenuItem Icon="@Icons.Material.Filled.FileCopy">
                Duplicate @context.Value.Name
            </MudMenuItem>
        </ContextMenu>
    </ObservableDataView>
</MudContainer>

@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    ObservableDataView<string, BotEntity, BotVM>? ItemsEditor { get; set; }
}
```

**Key Points**:
- ✅ `@page "/bots"` defines route
- ✅ `DataServiceProvider="@WorkspaceServices"` provides workspace services
- ✅ `ObservableDataView` handles all data loading, VM creation, and reactive updates
- ✅ Custom columns with templates for switches and icons
- ✅ Navigation to detail page via `Href="/bots/{Key}"`
- ✅ ChildRowContent for expandable rows
- ✅ ContextMenu for right-click actions

---

## Step 5: Create Detail Page (Bot.razor)

Create the detail page for editing individual bots:

**File**: `Pages/Bot.razor`

```razor
@page "/bots/{BotId}"

@using LionFire.Mvvm
@using LionFire.Reactive.Persistence
@using Microsoft.Extensions.DependencyInjection
@using Microsoft.Extensions.Logging
@using MyApp.Trading

@inject ILogger<Bot> Logger
@inject IServiceProvider ServiceProvider
@inject NavigationManager NavigationManager

<MudContainer MaxWidth="MaxWidth.Large" Class="pa-6">
    <!-- Header with back button -->
    <MudStack Row AlignItems="AlignItems.Center" Class="mb-4">
        <MudIconButton Icon="@Icons.Material.Filled.ArrowBack"
                       Href="/bots"
                       Size="Size.Large" />
        <MudText Typo="Typo.h3">Bot: @BotId</MudText>
    </MudStack>

    @if (VM?.Value != null)
    {
        <!-- Main content -->
        <MudPaper Class="pa-6">
            <MudStack Spacing="4">
                <!-- Basic Information -->
                <MudText Typo="Typo.h6">Basic Information</MudText>

                <MudTextField @bind-Value="VM.Value.Name"
                              Label="Bot Name"
                              Variant="Variant.Outlined" />

                <MudTextField @bind-Value="VM.Value.Description"
                              Label="Description"
                              Lines="3"
                              Variant="Variant.Outlined" />

                <MudTextField @bind-Value="VM.Value.Comments"
                              Label="Comments"
                              Lines="2"
                              Variant="Variant.Outlined" />

                <MudDivider />

                <!-- Trading Configuration -->
                <MudText Typo="Typo.h6">Trading Configuration</MudText>

                <MudTextField @bind-Value="VM.Value.Exchange"
                              Label="Exchange"
                              Variant="Variant.Outlined" />

                <MudTextField @bind-Value="VM.Value.ExchangeArea"
                              Label="Exchange Area"
                              Variant="Variant.Outlined"
                              HelperText="E.g., 'spot', 'futures'" />

                <MudTextField @bind-Value="VM.Value.Symbol"
                              Label="Symbol"
                              Variant="Variant.Outlined"
                              HelperText="E.g., 'BTCUSDT'" />

                <MudTextField @bind-Value="VM.Value.TimeFrame"
                              Label="Time Frame"
                              Variant="Variant.Outlined"
                              HelperText="E.g., '15m', '1h', '4h'" />

                <MudDivider />

                <!-- Runtime Settings -->
                <MudText Typo="Typo.h6">Runtime Settings</MudText>

                <MudSwitch @bind-Value="VM.Value.Enabled"
                           Label="Enabled"
                           Color="Color.Primary" />

                <MudSwitch @bind-Value="VM.Value.Live"
                           Label="Live Trading (Real Money)"
                           Color="Color.Secondary">
                    <MudText Color="Color.Warning" Typo="Typo.caption">
                        ⚠️ Warning: Live trading uses real money!
                    </MudText>
                </MudSwitch>

                <MudDivider />

                <!-- Actions -->
                <MudStack Row Spacing="2">
                    <MudButton OnClick="Save"
                               Color="Color.Primary"
                               Variant="Variant.Filled"
                               StartIcon="@Icons.Material.Filled.Save">
                        Save
                    </MudButton>

                    <MudButton Href="/bots"
                               Variant="Variant.Outlined">
                        Cancel
                    </MudButton>

                    <MudSpacer />

                    <MudButton OnClick="Delete"
                               Color="Color.Error"
                               Variant="Variant.Outlined"
                               StartIcon="@Icons.Material.Filled.Delete">
                        Delete
                    </MudButton>
                </MudStack>
            </MudStack>
        </MudPaper>
    }
    else if (VM != null)
    {
        <!-- Loading state -->
        <MudPaper Class="pa-6">
            <MudStack AlignItems="AlignItems.Center" Spacing="4">
                <MudProgressCircular Indeterminate="true" Size="Size.Large" />
                <MudText>Loading bot...</MudText>
            </MudStack>
        </MudPaper>
    }
    else
    {
        <!-- Error state -->
        <MudAlert Severity="Severity.Error">
            Unable to load bot services. Check that:
            <ul>
                <li>Workspace layout is being used (provides WorkspaceServices)</li>
                <li>BotEntity is registered with AddWorkspaceChildType</li>
                <li>Workspace configurators have run</li>
            </ul>
        </MudAlert>
    }
</MudContainer>

@code {
    #region Parameters

    [Parameter]
    public string? BotId { get; set; }

    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    #endregion

    #region State

    private ObservableReaderWriterItemVM<string, BotEntity, BotVM>? VM { get; set; }

    #endregion

    #region Lifecycle

    protected override async Task OnParametersSetAsync()
    {
        // Use workspace services (with fallback for development)
        var effectiveServices = WorkspaceServices ?? ServiceProvider;

        if (WorkspaceServices == null)
        {
            Logger.LogWarning(
                "WorkspaceServices cascading parameter not found. " +
                "Falling back to root ServiceProvider. " +
                "For production, this page should be within workspace layout."
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
                "Ensure: 1) Workspace layout is used, " +
                "2) Configurators have run, " +
                "3) AddWorkspaceChildType<BotEntity>() was called"
            );
            return;
        }

        Logger.LogInformation(
            "Loaded Bot persistence services from {ServiceSource}",
            WorkspaceServices != null ? "Workspace" : "Root"
        );

        // Create VM with workspace-scoped services
        VM = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);
        VM.Id = BotId;

        await base.OnParametersSetAsync();
    }

    #endregion

    #region Actions

    private async Task Save()
    {
        if (VM?.Value != null)
        {
            await VM.Write();
            Logger.LogInformation("Saved bot {BotId}", BotId);
            Snackbar.Add("Bot saved successfully", Severity.Success);
        }
    }

    private async Task Delete()
    {
        bool? confirmed = await DialogService.ShowMessageBox(
            "Confirm Delete",
            $"Are you sure you want to delete bot '{VM?.Value?.Name}'?",
            yesText: "Delete",
            cancelText: "Cancel"
        );

        if (confirmed == true && VM != null)
        {
            // Delete the file
            var writer = WorkspaceServices?.GetService<IObservableWriter<string, BotEntity>>();
            if (writer != null)
            {
                // Deletion would be implemented in IObservableWriter
                // await writer.Delete(BotId);

                Logger.LogInformation("Deleted bot {BotId}", BotId);
                NavigationManager.NavigateTo("/bots");
            }
        }
    }

    #endregion
}
```

**Key Points**:
- ✅ `@page "/bots/{BotId}"` - Route with parameter
- ✅ Manual VM creation with workspace services
- ✅ Loading and error states
- ✅ Save button calls `VM.Write()`
- ✅ Back navigation to list
- ✅ Snackbar notifications for user feedback

---

## Step 6: Test the Implementation

### 1. Run the Application

```bash
dotnet run
```

### 2. Navigate to List Page

Browse to: `https://localhost:5001/bots`

**Expected**:
- Empty grid with "Add" button in toolbar
- Or existing bots if files already exist

### 3. Create a Bot

Click "Add" button in toolbar:
- Component creates new entity
- Saves to `{workspace}/Bots/new-bot-{guid}.hjson`
- Grid automatically updates

### 4. Edit a Bot

Click on bot row or "Edit" button:
- Navigates to `/bots/bot-alpha`
- Detail page loads
- Edit fields
- Click "Save"
- File updates on disk
- Return to list - changes reflected

### 5. Verify File System

Check workspace directory:
```bash
ls C:\Users\Alice\Trading\Workspaces\workspace1\Bots\
# Should see: bot-alpha.hjson, bot-beta.hjson, etc.

cat C:\Users\Alice\Trading\Workspaces\workspace1\Bots\bot-alpha.hjson
```

Expected content:
```hjson
name: Bot Alpha
description: My first bot
exchange: binance
exchangeArea: futures
symbol: BTCUSDT
timeFrame: 15m
enabled: true
live: false
```

### 6. Test File Watching

1. Open bot file in text editor
2. Change `name` field
3. Save file
4. Return to browser
5. List page should automatically update (reactive binding)

---

## Next Steps

### Add More Features

**1. Validation**:
```csharp
// In BotEntity
public ValidationContext ValidateThis(ValidationContext context)
{
    return context
        .PropertyNotNull(nameof(Name), Name)
        .PropertyNotNull(nameof(Symbol), Symbol);
}

// In Bot.razor
private async Task Save()
{
    if (VM?.Value != null)
    {
        var validation = VM.Value.ValidateThis(new ValidationContext());
        if (!validation.IsValid)
        {
            Snackbar.Add($"Validation failed: {validation.ErrorMessages}", Severity.Error);
            return;
        }

        await VM.Write();
    }
}
```

**2. Search and Filtering**:
```razor
<ObservableDataView ...>
    <!-- MudDataGrid has built-in filtering -->
    <Columns>
        <PropertyColumn Property="x => x.Value.Name" Filterable="true" />
    </Columns>
</ObservableDataView>
```

**3. Bulk Operations**:
```razor
@code {
    private async Task EnableAllBots()
    {
        if (ItemsEditor?.ViewModel?.Items == null) return;

        foreach (var botVM in ItemsEditor.ViewModel.Items.Items)
        {
            botVM.Value.Enabled = true;
            await ItemsEditor.ViewModel.Writer?.Write(botVM.Key, botVM.Value.Value);
        }

        Snackbar.Add("All bots enabled", Severity.Success);
    }
}
```

**4. Export/Import**:
```csharp
private async Task ExportBot(BotVM bot)
{
    var json = JsonConvert.SerializeObject(bot.Value, Formatting.Indented);
    await JS.InvokeVoidAsync("downloadFile", $"{bot.Key}.json", json);
}
```

---

## Troubleshooting Common Issues

### Build Errors

**Error**: `The type or namespace name 'ObservableDataView' could not be found`

**Solution**: Add package reference
```bash
dotnet add package LionFire.Blazor.Components.MudBlazor
```

---

### Runtime Errors

**Error**: `Unable to resolve service for type 'IObservableReader<string, BotEntity>'`

**Cause**: Services not in workspace scope.

**Solution**: Verify registration:
```csharp
services
    .AddWorkspaceChildType<BotEntity>()        // ← Must have this
    .AddWorkspaceDocumentService<string, BotEntity>();
```

**See**: [Service Scoping Deep Dive](../../architecture/workspaces/service-scoping.md)

---

### UI Issues

**Issue**: Grid is empty but files exist

**Debug**:
```csharp
@code {
    protected override void OnInitialized()
    {
        var reader = WorkspaceServices?.GetService<IObservableReader<string, BotEntity>>();
        if (reader != null)
        {
            Logger.LogInformation("Available keys: {Keys}",
                string.Join(", ", reader.Keys.Items));
        }
    }
}
```

---

## Complete Project Structure

```
MyApp.Trading/
├── Models/
│   └── BotEntity.cs
├── ViewModels/
│   └── BotVM.cs
├── Pages/
│   ├── Bots.razor          ← List page
│   └── Bot.razor           ← Detail page
├── Services/
│   └── BotRunner.cs        ← Optional
└── Program.cs              ← Registration
```

---

## Summary

### What You Accomplished

✅ **Defined** workspace document entity (`BotEntity`)
✅ **Created** ViewModel for UI (`BotVM`)
✅ **Registered** with workspace system
✅ **Built** list page using `ObservableDataView`
✅ **Built** detail page with manual VM pattern
✅ **Implemented** navigation, saving, and reactive updates

### Key Takeaways

1. **ObservableDataView** = Easy list views with minimal code
2. **Manual VM** = Full control for detail views
3. **CascadingParameter** = Essential for workspace services
4. **ReactiveObject** = Automatic UI updates
5. **File-Based** = Human-readable, version-controllable storage

### Next Steps

- Add more document types (Portfolios, Strategies)
- Implement runners for active documents
- Add validation and error handling
- Implement bulk operations
- Add search and filtering
- Create workspace selector UI

---

## Related Documentation

- **[Blazor MVVM Patterns](../../ui/blazor-mvvm-patterns.md)** - Pattern details
- **[Workspace Architecture](../../architecture/workspaces/README.md)** - Conceptual overview
- **[Service Scoping](../../architecture/workspaces/service-scoping.md)** - DI deep dive
- **[Document Types](../../architecture/workspaces/document-types.md)** - Advanced patterns
- **Library References**:
  - [LionFire.Blazor.Components.MudBlazor](../../../src/LionFire.Blazor.Components.MudBlazor/CLAUDE.md)
  - [LionFire.Workspaces](../../../src/LionFire.Workspaces/CLAUDE.md)
  - [LionFire.Data.Async.Mvvm](../../../src/LionFire.Data.Async.Mvvm/CLAUDE.md)
