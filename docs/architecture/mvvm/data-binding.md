# Data Access + MVVM Integration

**Overview**: This document explains how LionFire's async data access patterns (`IGetter`, `ISetter`, `IValue`, `IObservableReader/Writer`) integrate with ViewModels to provide reactive, UI-friendly data binding. This integration is the bridge between the data layer and the MVVM layer.

---

## Table of Contents

1. [Integration Overview](#integration-overview)
2. [GetterVM and ValueVM Patterns](#gettervm-and-valuevm-patterns)
3. [ObservableReader/Writer ViewModels](#observablereaderwriter-viewmodels)
4. [Lazy Loading and Caching](#lazy-loading-and-caching)
5. [Polling and Auto-Refresh](#polling-and-auto-refresh)
6. [Workspace Integration](#workspace-integration)
7. [Complete Examples](#complete-examples)

---

## Integration Overview

### The Bridge

**Async Data Layer** → **ViewModel Layer** → **UI Layer**

```
IGetter<T>
    ↓ Wrapped by
GetterVM<T>
    ↓ Bound to
UI (Blazor/WPF/Avalonia)
```

### Why ViewModels for Data Access?

**Raw Data Access** (without ViewModels):
```csharp
// ❌ UI component directly using data access
@inject IGetter<WeatherData> WeatherGetter

@code {
    private WeatherData? weather;
    private bool isLoading;

    protected override async Task OnInitializedAsync()
    {
        isLoading = true;
        var result = await WeatherGetter.Get();
        if (result.HasValue)
            weather = result.Value;
        isLoading = false;
    }
}
```

**Problems**:
- Manual state management (isLoading)
- No automatic refresh
- No polling support
- No reactive updates
- Boilerplate in every component

**With ViewModels**:
```csharp
// ✅ UI component using ViewModel
@inject GetterVM<WeatherData> WeatherVM

<MudProgressLinear Indeterminate="true" Hidden="@(!WeatherVM.IsGetting)" />
@if (WeatherVM.HasValue)
{
    <div>@WeatherVM.FullFeaturedSource.ReadCacheValue.Temperature°</div>
}
<MudButton OnClick="() => WeatherVM.GetCommand.Execute()">Refresh</MudButton>

@code {
    protected override async Task OnInitializedAsync()
    {
        await WeatherVM.GetIfNeeded.Execute();  // Load if not cached
    }
}
```

**Benefits**:
- Automatic state management
- Commands for UI binding
- Polling support
- Reactive updates
- Reusable VM

---

## GetterVM and ValueVM Patterns

### GetterVM\<T\> - Read-Only Data

**Wraps** `IGetter<T>` from async data layer.

```csharp
using LionFire.Mvvm;
using LionFire.Data.Async.Gets;

// Data layer
public class WeatherGetter : IStatelessGetter<WeatherData>
{
    public async Task<IGetResult<WeatherData>> Get(CancellationToken ct = default)
    {
        var data = await httpClient.GetFromJsonAsync<WeatherData>("...");
        return GetResult<WeatherData>.Success(data);
    }
}

// ViewModel layer
var getter = new WeatherGetter();
var vm = new GetterVM<WeatherData>(getter);

// UI binding
vm.GetCommand              // Command to execute get
vm.GetIfNeeded             // Command to get only if not cached
vm.IsGetting              // Bool - operation in progress
vm.HasValue               // Bool - data is cached
vm.FullFeaturedSource     // Access to IGetter RxO with ReadCacheValue
```

**Key Properties**:
```csharp
public partial class GetterVM<TValue> : ReactiveObject
{
    // Source
    public IGetter? Source { get; set; }
    public IGetterRxO<TValue>? FullFeaturedSource { get; }  // Reactive version

    // Commands
    public ReactiveCommand<Unit, IGetResult<TValue>> GetCommand { get; }
    public ReactiveCommand<Unit, IGetResult<TValue>> GetIfNeeded { get; }

    // State
    [ObservableAsProperty] public bool CanGet { get; }
    [ObservableAsProperty] public bool IsGetting { get; }
    [ObservableAsProperty] public bool HasValue { get; }

    // Configuration
    public TimeSpan? PollDelay { get; set; }  // Auto-refresh
    public bool ShowRefresh { get; set; } = true;
}
```

**Usage in Blazor**:
```razor
@inject GetterVM<WeatherData> WeatherVM

<MudCard>
    <MudCardContent>
        @if (WeatherVM.HasValue)
        {
            <MudText>Temp: @WeatherVM.FullFeaturedSource.ReadCacheValue.Temperature°</MudText>
            <MudText>Humidity: @WeatherVM.FullFeaturedSource.ReadCacheValue.Humidity%</MudText>
        }
        else
        {
            <MudText>No data loaded</MudText>
        }
    </MudCardContent>
    <MudCardActions>
        <MudButton OnClick="@(() => WeatherVM.GetCommand.Execute())"
                   Disabled="@WeatherVM.IsGetting">
            @(WeatherVM.IsGetting ? "Loading..." : "Refresh")
        </MudButton>
    </MudCardActions>
</MudCard>
```

### ValueVM\<T\> - Read/Write Data

**Wraps** `IValue<T>` (read + write).

```csharp
// Data layer
public class SettingsValue : IValue<AppSettings>
{
    public async Task<IGetResult<AppSettings>> Get(CancellationToken ct = default) { }
    public async Task<ISetResult<AppSettings>> Set(AppSettings value, CancellationToken ct = default) { }
}

// ViewModel layer
var value = new SettingsValue();
var vm = new ValueVM<AppSettings>(value);

// UI binding
vm.GetCommand              // Load settings
vm.SetCommand              // Save settings
vm.IsGetting              // Loading indicator
vm.IsSetting              // Saving indicator
vm.IsBusy                 // Getting || Setting
```

**Additional Properties**:
```csharp
public partial class ValueVM<TValue> : GetterVM<TValue>
{
    // Inherits all GetterVM properties

    // Set operations
    public ReactiveCommand<TValue, ISetResult<TValue>> SetCommand { get; }
    [ObservableAsProperty] public bool CanSet { get; }
    [ObservableAsProperty] public bool IsSetting { get; }

    // Combined state
    public bool IsBusy => IsGetting || IsSetting;
}
```

**Usage in Blazor**:
```razor
@inject ValueVM<AppSettings> SettingsVM

<MudTextField @bind-Value="@SettingsVM.FullFeaturedSource.ReadCacheValue.ApiKey"
              Label="API Key" />

<MudButton OnClick="@SaveSettings"
           Disabled="@SettingsVM.IsBusy">
    @(SettingsVM.IsSetting ? "Saving..." : "Save")
</MudButton>

@code {
    protected override async Task OnInitializedAsync()
    {
        await SettingsVM.GetIfNeeded.Execute();
    }

    private async Task SaveSettings()
    {
        var settings = SettingsVM.FullFeaturedSource.ReadCacheValue;
        await SettingsVM.SetCommand.Execute(settings);
    }
}
```

---

## ObservableReader/Writer ViewModels

### ObservableReaderItemVM\<TKey, TValue, TValueVM\>

**Wraps** `IObservableReader<TKey, TValue>` for **single-item** access.

```csharp
public partial class ObservableReaderItemVM<TKey, TValue, TValueVM> : ReactiveObject
    where TKey : notnull
    where TValue : notnull
{
    public IObservableReader<TKey, TValue> Data { get; }

    public TKey? Id { get; set; }          // Set to load item
    public TValue? Value { get; }          // Loaded value
    public bool IsLoading { get; }         // Loading state
}
```

**Reactive Loading**:
```csharp
// When Id changes, VM automatically:
// 1. Subscribes to Data.GetValueObservableIfExists(Id)
// 2. Loads value from storage
// 3. Updates Value property
// 4. UI automatically refreshes
```

**Usage**:
```csharp
var reader = workspaceServices.GetService<IObservableReader<string, BotEntity>>();
var vm = new ObservableReaderItemVM<string, BotEntity, BotVM>(reader);

vm.Id = "bot-alpha";  // Triggers load
// vm.Value automatically set when loaded
```

### ObservableReaderWriterItemVM\<TKey, TValue, TValueVM\>

**Adds write capability** to `ObservableReaderItemVM`.

```csharp
public partial class ObservableReaderWriterItemVM<TKey, TValue, TValueVM>
    : ObservableReaderItemVM<TKey, TValue, TValueVM>
{
    public IObservableWriter<TKey, TValue> Writer { get; }

    public ValueTask Write()  // Persist current value
    {
        return Writer.Write(Id, Value);
    }
}
```

**Complete Workflow**:
```csharp
var reader = workspaceServices.GetService<IObservableReader<string, BotEntity>>();
var writer = workspaceServices.GetService<IObservableWriter<string, BotEntity>>();

var vm = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);

// Load
vm.Id = "bot-alpha";
// vm.Value automatically loads

// Edit
vm.Value.Name = "Updated Name";

// Save
await vm.Write();
// File updated: workspace/Bots/bot-alpha.hjson
```

**Usage in Blazor**:
```razor
@page "/bots/{BotId}"

@if (VM?.Value != null)
{
    <MudTextField @bind-Value="VM.Value.Name" Label="Name" />
    <MudButton OnClick="Save">Save</MudButton>
}

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
        VM.Id = BotId;  // Triggers load
    }

    private async Task Save() => await VM.Write();
}
```

### ObservableDataVM\<TKey, TValue, TValueVM\>

**Complete collection management** ViewModel.

```csharp
public class ObservableDataVM<TKey, TValue, TValueVM>
{
    public IObservableReader<TKey, TValue>? Data { get; set; }
    public IObservableCache<TValueVM, TKey> Items { get; }

    // CRUD
    public bool CanCreate { get; }
    public bool CanDelete { get; }
    public EditMode AllowedEditModes { get; set; }

    // UI State
    public bool ShowDeleteColumn { get; set; }
    public IEnumerable<Type> CreatableTypes { get; set; }

    // Factory
    public Func<TKey, Optional<TValue>, TValueVM>? VMFactory { get; set; }
}
```

**Used by**: `ObservableDataView` Blazor component.

**Features**:
- Subscribes to `Data.Values.Connect()`
- Creates VMs for each entity
- Provides reactive Items collection
- Manages CRUD operations

---

## Lazy Loading and Caching

### Pattern: GetIfNeeded

```csharp
var vm = new GetterVM<Data>(getter);

// Loads only if not already cached
await vm.GetIfNeeded.Execute();

if (vm.HasValue)
{
    // Data available from cache
    var data = vm.FullFeaturedSource.ReadCacheValue;
}
```

**vs Get Command**:
```csharp
// Always executes, even if cached
await vm.GetCommand.Execute();
```

### Caching Behavior

**IGetter with caching**:
```csharp
public class CachingWeatherGetter : ILazyGetter<WeatherData>
{
    private WeatherData? cache;

    public async Task<IGetResult<WeatherData>> Get(CancellationToken ct)
    {
        var data = await FetchFromAPI();
        cache = data;
        return GetResult<WeatherData>.Success(data);
    }

    public async Task<IGetResult<WeatherData>> TryGetValue(CancellationToken ct)
    {
        if (cache != null)
            return GetResult<WeatherData>.Success(cache);

        return await Get(ct);  // Falls back to Get
    }
}
```

**GetterVM Integration**:
```csharp
var vm = new GetterVM<WeatherData>(cachingGetter);

// First call: Executes Get, caches result
await vm.GetIfNeeded.Execute();

// Second call: Returns cached value immediately
await vm.GetIfNeeded.Execute();  // Fast!

// Force refresh: Always executes Get
await vm.GetCommand.Execute();
```

### Cache Invalidation

```csharp
// Discard cached value
if (vm.FullFeaturedSource is IValue<WeatherData> value)
{
    value.DiscardValue();
}

// Next GetIfNeeded will fetch fresh data
await vm.GetIfNeeded.Execute();
```

---

## Polling and Auto-Refresh

### Automatic Polling

```csharp
var vm = new GetterVM<StockPrice>(priceGetter)
{
    PollDelay = TimeSpan.FromSeconds(10)  // Refresh every 10 seconds
};

// Polling starts automatically
// GetCommand.Execute() called every 10 seconds
// UI automatically updates via reactive bindings
```

**Smart Polling**:
- Only polls if `PollDelay` is set
- Doesn't poll if source already fires change events
- Stops when VM is disposed

### Manual Refresh

```razor
<MudButton OnClick="@(() => WeatherVM.GetCommand.Execute())"
           Hidden="@(!WeatherVM.ShowRefresh)">
    <MudIcon Icon="@Icons.Material.Filled.Refresh" />
</MudButton>
```

### Throttled Updates

```csharp
public class SearchVM : ReactiveObject
{
    [Reactive] private string? _searchText;

    public SearchVM(IGetter<SearchResults> resultsGetter)
    {
        ResultsVM = new GetterVM<SearchResults>(resultsGetter);

        // Throttle search requests
        this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Where(text => !string.IsNullOrEmpty(text))
            .Subscribe(async text =>
            {
                // Trigger search
                await ResultsVM.GetCommand.Execute();
            });
    }

    public GetterVM<SearchResults> ResultsVM { get; }
}
```

---

## Workspace Integration

### The Pattern

**Workspace services** provide `IObservableReader/Writer` instances that are wrapped by ViewModels.

```
Workspace Service Provider
    ↓ Provides (workspace-scoped)
IObservableReader<string, BotEntity>
IObservableWriter<string, BotEntity>
    ↓ Wrapped by
ObservableReaderWriterItemVM<string, BotEntity, BotVM>
    ↓ Creates
BotVM (wraps BotEntity)
    ↓ Bound to
Blazor UI Component
```

### List + Detail Pattern

**List Page** - Collection ViewModel:
```csharp
// ObservableDataView component uses ObservableDataVM internally
public class ObservableDataVM<TKey, TValue, TValueVM>
{
    public IObservableReader<TKey, TValue>? Data { get; set; }

    public ObservableDataVM()
    {
        // Subscribes to Data.Values.Connect()
        // Creates TValueVM for each TValue
        // Provides Items observable cache
    }
}
```

**Detail Page** - Item ViewModel:
```csharp
// Manual VM creation in component
var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
var writer = WorkspaceServices.GetService<IObservableWriter<string, BotEntity>>();

var vm = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);
vm.Id = "bot-alpha";  // Load specific bot
```

### Complete Example

**Entity**:
```csharp
[Alias("Bot")]
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;
    [Reactive] private bool _enabled;
}
```

**ViewModel**:
```csharp
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value) { }

    public string DisplayName => $"Bot: {Value.Name}";
}
```

**Registration**:
```csharp
services
    .AddWorkspaceChildType<BotEntity>()
    .AddWorkspaceDocumentService<string, BotEntity>()
    .AddTransient<BotVM>();
```

**List UI** (ObservableDataView):
```razor
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices">
    <!-- Columns -->
</ObservableDataView>
```

**Detail UI** (Manual):
```razor
<MudTextField @bind-Value="VM.Value.Name" />
<MudButton OnClick="@(() => VM.Write())">Save</MudButton>

@code {
    private ObservableReaderWriterItemVM<string, BotEntity, BotVM>? VM { get; set; }
}
```

**See**: [Workspace Architecture](../workspaces/README.md) for complete workspace integration.

---

## Complete Examples

### Example 1: Simple Read-Only Data

**Goal**: Display weather data with refresh button.

**Data Layer**:
```csharp
public class WeatherGetter : IStatelessGetter<WeatherData>
{
    private readonly HttpClient httpClient;

    public async Task<IGetResult<WeatherData>> Get(CancellationToken ct = default)
    {
        try
        {
            var data = await httpClient.GetFromJsonAsync<WeatherData>(
                "https://api.weather.com/current", ct);
            return GetResult<WeatherData>.Success(data);
        }
        catch (Exception ex)
        {
            return GetResult<WeatherData>.Failure(ex);
        }
    }
}
```

**ViewModel Layer**:
```csharp
public class WeatherPageVM : ReactiveObject
{
    public WeatherPageVM(IGetter<WeatherData> weatherGetter)
    {
        WeatherVM = new GetterVM<WeatherData>(weatherGetter)
        {
            PollDelay = TimeSpan.FromMinutes(15)  // Auto-refresh
        };

        // Initial load
        WeatherVM.GetIfNeeded.Execute().Subscribe();
    }

    public GetterVM<WeatherData> WeatherVM { get; }
}
```

**UI Layer** (Blazor):
```razor
@inject WeatherPageVM ViewModel

<MudCard>
    <MudCardHeader>
        <MudText Typo="Typo.h6">Current Weather</MudText>
    </MudCardHeader>
    <MudCardContent>
        @if (ViewModel.WeatherVM.IsGetting)
        {
            <MudProgressCircular Indeterminate="true" />
        }
        else if (ViewModel.WeatherVM.HasValue)
        {
            var weather = ViewModel.WeatherVM.FullFeaturedSource.ReadCacheValue;
            <MudText>Temperature: @weather.Temperature°F</MudText>
            <MudText>Conditions: @weather.Conditions</MudText>
            <MudText>Humidity: @weather.Humidity%</MudText>
        }
        else
        {
            <MudText Color="Color.Warning">No data loaded</MudText>
        }
    </MudCardContent>
    <MudCardActions>
        <MudButton OnClick="@(() => ViewModel.WeatherVM.GetCommand.Execute())"
                   Disabled="@ViewModel.WeatherVM.IsGetting"
                   StartIcon="@Icons.Material.Filled.Refresh">
            Refresh
        </MudButton>
    </MudCardActions>
</MudCard>
```

### Example 2: Editable Configuration

**Goal**: Edit and save application settings.

**Data Layer**:
```csharp
public class SettingsValue : IValue<AppSettings>
{
    private AppSettings? cache;

    public async Task<IGetResult<AppSettings>> Get(CancellationToken ct)
    {
        var json = await File.ReadAllTextAsync("settings.json", ct);
        cache = JsonConvert.DeserializeObject<AppSettings>(json);
        return GetResult<AppSettings>.Success(cache);
    }

    public async Task<ISetResult<AppSettings>> Set(AppSettings value, CancellationToken ct)
    {
        var json = JsonConvert.SerializeObject(value, Formatting.Indented);
        await File.WriteAllTextAsync("settings.json", json, ct);
        cache = value;
        return SetResult<AppSettings>.Success(value);
    }

    public Task<IGetResult<AppSettings>> TryGetValue(CancellationToken ct)
        => cache != null
            ? Task.FromResult(GetResult<AppSettings>.Success(cache))
            : Get(ct);
}
```

**ViewModel Layer**:
```csharp
public class SettingsPageVM : ReactiveObject
{
    public SettingsPageVM(IValue<AppSettings> settingsValue)
    {
        SettingsVM = new ValueVM<AppSettings>(settingsValue);

        // Load on startup
        SettingsVM.GetIfNeeded.Execute().Subscribe();

        // Save command
        SaveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var settings = SettingsVM.FullFeaturedSource.ReadCacheValue;
            var result = await SettingsVM.SetCommand.Execute(settings);

            if (result.IsSuccess)
                StatusMessage = "Settings saved successfully";
            else
                StatusMessage = $"Error: {result.Error}";
        },
        canExecute: this.WhenAnyValue(x => x.SettingsVM.HasValue));
    }

    public ValueVM<AppSettings> SettingsVM { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    [Reactive] private string? _statusMessage;
}
```

**UI Layer** (Blazor):
```razor
@inject SettingsPageVM ViewModel

<MudPaper Class="pa-6">
    <MudText Typo="Typo.h5">Application Settings</MudText>

    @if (ViewModel.SettingsVM.HasValue)
    {
        var settings = ViewModel.SettingsVM.FullFeaturedSource.ReadCacheValue;

        <MudTextField @bind-Value="settings.ApiKey" Label="API Key" />
        <MudTextField @bind-Value="settings.ApiEndpoint" Label="Endpoint" />
        <MudSwitch @bind-Value="settings.EnableLogging" Label="Enable Logging" />

        <MudButton OnClick="@(() => ViewModel.SaveCommand.Execute())"
                   Disabled="@ViewModel.SettingsVM.IsBusy"
                   Color="Color.Primary">
            @(ViewModel.SettingsVM.IsSetting ? "Saving..." : "Save")
        </MudButton>

        @if (!string.IsNullOrEmpty(ViewModel.StatusMessage))
        {
            <MudAlert Severity="Severity.Success">@ViewModel.StatusMessage</MudAlert>
        }
    }
    else if (ViewModel.SettingsVM.IsGetting)
    {
        <MudProgressLinear Indeterminate="true" />
    }
</MudPaper>
```

### Example 3: Master-Detail with Workspace Documents

**Goal**: List of bots with detail editing.

**List ViewModel**:
```csharp
public class BotsListVM : ReactiveObject
{
    public BotsListVM(IObservableReader<string, BotEntity> reader)
    {
        BotsVM = new ObservableReaderVM<string, BotEntity, BotVM>(reader)
        {
            AutoLoadAll = true  // Subscribe to all files
        };
    }

    public ObservableReaderVM<string, BotEntity, BotVM> BotsVM { get; }

    [Reactive] private string? _selectedBotId;
}
```

**Detail ViewModel**:
```csharp
public class BotDetailVM : ReactiveObject
{
    public BotDetailVM(
        IObservableReader<string, BotEntity> reader,
        IObservableWriter<string, BotEntity> writer)
    {
        BotVM = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);

        SaveCommand = ReactiveCommand.CreateFromTask(
            async () => await BotVM.Write(),
            canExecute: this.WhenAnyValue(x => x.BotVM.Value).Select(v => v != null)
        );
    }

    public ObservableReaderWriterItemVM<string, BotEntity, BotVM> BotVM { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
}
```

**Blazor UI** (simpler with ObservableDataView):
```razor
<!-- List -->
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices">
    <Columns>
        <TemplateColumn>
            <CellTemplate>
                <MudButton Href="@($"/bots/{context.Item.Key}")">Edit</MudButton>
            </CellTemplate>
        </TemplateColumn>
    </Columns>
</ObservableDataView>

<!-- Detail -->
@page "/bots/{BotId}"
<MudTextField @bind-Value="VM.Value.Name" />
<MudButton OnClick="@(() => VM.Write())">Save</MudButton>
```

---

## Summary

### Data Access → ViewModel Mapping

| Data Access Type | ViewModel Type | Use Case |
|-----------------|----------------|----------|
| `IGetter<T>` | `GetterVM<T>` | Read-only data with refresh |
| `IValue<T>` | `ValueVM<T>` | Read/write data |
| `IObservableReader<TKey, T>` | `ObservableReaderVM<TKey, T, TVM>` | File-based collections |
| `IObservableReader/Writer<TKey, T>` | `ObservableReaderWriterItemVM<TKey, T, TVM>` | Single file item |
| | `ObservableDataVM<TKey, T, TVM>` | Complete CRUD collection |

### Key Benefits

1. **Reactive State** - IsGetting, IsSetting, HasValue
2. **Commands** - GetCommand, SetCommand for UI binding
3. **Caching** - Automatic caching with GetIfNeeded
4. **Polling** - Automatic refresh support
5. **Error Handling** - Built-in error state
6. **Workspace Integration** - Works with workspace-scoped services

### Best Practices

✅ **DO**:
- Use `GetterVM` for read-only data
- Use `ValueVM` for editable data
- Use `ObservableReaderWriterItemVM` for workspace documents
- Set `PollDelay` for auto-refreshing data
- Check `HasValue` before accessing cache
- Dispose VMs to stop polling

❌ **DON'T**:
- Access data layer directly from UI
- Create multiple VMs for same data source
- Forget to execute initial load
- Assume `ReadCacheValue` is always available

### Related Documentation

- **[MVVM Architecture Overview](README.md)**
- **[Reactive MVVM Patterns](reactive-mvvm.md)**
- **[Workspace Architecture](../workspaces/README.md)**
- **Library References**:
  - [LionFire.Data.Async.Mvvm](../../src/LionFire.Data.Async.Mvvm/CLAUDE.md)
  - [LionFire.Data.Async.Abstractions](../../src/LionFire.Data.Async.Abstractions/CLAUDE.md)
  - [LionFire.Reactive](../../src/LionFire.Reactive/CLAUDE.md)
