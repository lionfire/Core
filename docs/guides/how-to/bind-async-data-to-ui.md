# How-To: Bind Async Data to UI

## Problem

You need to bind async data operations (`IGetter<T>`, `IValue<T>`) to UI components, showing loading states, handling errors, and updating reactively.

## Solution

Use LionFire's `GetterVM` and `ValueVM` wrappers with ReactiveUI bindings in Blazor for automatic loading state management and reactive updates.

---

## Strategy 1: Direct Getter Binding (Simple)

**Use case**: Simple data fetching with loading indicator.

### Basic Pattern

```csharp
using LionFire.Data.Async.Reactive;

public class UserProfileGetter : GetterRxO<UserProfile>
{
    public UserProfileGetter(string userId, HttpClient httpClient)
        : base(ct => LoadProfile(userId, httpClient, ct))
    {
    }

    private static async Task<IGetResult<UserProfile>> LoadProfile(
        string userId, HttpClient httpClient, CancellationToken ct)
    {
        try
        {
            var response = await httpClient.GetAsync($"/api/users/{userId}", ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var profile = JsonSerializer.Deserialize<UserProfile>(json);

            return GetResult.Success(profile!);
        }
        catch (Exception ex)
        {
            return GetResult.Failure<UserProfile>($"Error loading profile: {ex.Message}");
        }
    }
}
```

### Blazor Component

```razor
@page "/profile/{UserId}"
@inject HttpClient HttpClient

<MudCard>
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">User Profile</MudText>
        </CardHeaderContent>
    </MudCardHeader>
    <MudCardContent>
        @if (profileGetter.IsLoading)
        {
            <MudProgressCircular Indeterminate="true" />
            <MudText>Loading profile...</MudText>
        }
        else if (profileGetter.HasValue)
        {
            var profile = profileGetter.ReadCacheValue;
            <MudText><strong>Name:</strong> @profile.Name</MudText>
            <MudText><strong>Email:</strong> @profile.Email</MudText>
            <MudText><strong>Joined:</strong> @profile.JoinedDate.ToString("d")</MudText>
        }
        else if (profileGetter.ReadState == ReadState.Failed)
        {
            <MudAlert Severity="Severity.Error">
                Failed to load profile
            </MudAlert>
        }
    </MudCardContent>
</MudCard>

@code {
    [Parameter] public string UserId { get; set; } = "";

    private UserProfileGetter profileGetter = null!;

    protected override void OnInitialized()
    {
        profileGetter = new UserProfileGetter(UserId, HttpClient);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await profileGetter.GetIfNeeded();
            StateHasChanged();  // Trigger re-render after load
        }
    }
}
```

---

## Strategy 2: Using GetterVM (Recommended)

**Use case**: Getter with command support and reactive bindings.

### Create GetterVM

```csharp
using LionFire.Data.Async.Mvvm;

public class UserProfilePage : ReactiveObject
{
    public UserProfilePage(string userId, HttpClient httpClient)
    {
        // Create getter
        var profileGetter = new GetterRxO<UserProfile>(
            ct => LoadProfile(userId, httpClient, ct)
        );

        // Wrap in GetterVM for commands
        ProfileGetter = new GetterVM<UserProfile>(profileGetter);

        // Refresh command
        RefreshCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            ProfileGetter.DiscardValue();
            await ProfileGetter.GetIfNeeded();
        });
    }

    public GetterVM<UserProfile> ProfileGetter { get; }
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

    private static async Task<IGetResult<UserProfile>> LoadProfile(
        string userId, HttpClient httpClient, CancellationToken ct)
    {
        // ... implementation
    }
}
```

### Blazor Component with GetterVM

```razor
@page "/profile/{UserId}"
@inject HttpClient HttpClient
@implements IDisposable

<MudCard>
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">User Profile</MudText>
        </CardHeaderContent>
        <MudCardActions>
            <MudIconButton
                Icon="@Icons.Material.Filled.Refresh"
                OnClick="@(() => page.RefreshCommand.Execute().Subscribe())"
                Disabled="@page.ProfileGetter.IsLoading" />
        </MudCardActions>
    </MudCardHeader>
    <MudCardContent>
        @if (page.ProfileGetter.IsLoading)
        {
            <MudProgressCircular Indeterminate="true" />
        }
        else if (page.ProfileGetter.HasValue)
        {
            var profile = page.ProfileGetter.Value!;
            <MudText><strong>Name:</strong> @profile.Name</MudText>
            <MudText><strong>Email:</strong> @profile.Email</MudText>
            <MudText><strong>Joined:</strong> @profile.JoinedDate.ToString("d")</MudText>
        }
        else if (page.ProfileGetter.QueryResult?.IsSuccess == false)
        {
            <MudAlert Severity="Severity.Error">
                @page.ProfileGetter.QueryResult.Error
            </MudAlert>
        }
    </MudCardContent>
</MudCard>

@code {
    [Parameter] public string UserId { get; set; } = "";

    private UserProfilePage page = null!;
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        page = new UserProfilePage(UserId, HttpClient);

        // Subscribe to property changes for reactive updates
        subscription = page.ProfileGetter.WhenAnyValue(g => g.IsLoading)
            .Subscribe(_ => InvokeAsync(StateHasChanged));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await page.ProfileGetter.GetIfNeeded();
        }
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

---

## Strategy 3: ValueVM with Two-Way Binding

**Use case**: Editable data with save/load operations.

### Create ValueVM

```csharp
using LionFire.Data.Async.Mvvm;
using LionFire.Data.Async.Reactive;

public class SettingsPage : ReactiveObject
{
    public SettingsPage(ISettingsRepository repository)
    {
        // Create IValue for read/write operations
        var settingsValue = new ValueRxO<AppSettings>(
            loadFunc: ct => repository.LoadSettingsAsync(ct),
            saveFunc: (settings, ct) => repository.SaveSettingsAsync(settings, ct)
        );

        // Wrap in ValueVM for commands
        SettingsValue = new ValueVM<AppSettings>(settingsValue);

        // Commands
        SaveCommand = ReactiveCommand.CreateFromTask(
            async () => await SettingsValue.Set(),
            SettingsValue.WhenAnyValue(v => v.HasValue)
        );

        ResetCommand = ReactiveCommand.Create(() =>
        {
            SettingsValue.DiscardValue();
            SettingsValue.Value = new AppSettings(); // Default settings
        });
    }

    public ValueVM<AppSettings> SettingsValue { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }
}
```

### Blazor Component with Two-Way Binding

```razor
@page "/settings"
@inject ISettingsRepository Repository
@implements IDisposable

<MudCard>
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">Application Settings</MudText>
        </CardHeaderContent>
    </MudCardHeader>
    <MudCardContent>
        @if (page.SettingsValue.IsLoading)
        {
            <MudProgressCircular Indeterminate="true" />
        }
        else if (page.SettingsValue.HasValue)
        {
            var settings = page.SettingsValue.Value!;

            <MudTextField
                @bind-Value="settings.AppName"
                Label="Application Name" />

            <MudTextField
                @bind-Value="settings.ApiEndpoint"
                Label="API Endpoint" />

            <MudSwitch
                @bind-Checked="settings.EnableLogging"
                Label="Enable Logging" />

            <MudNumericField
                @bind-Value="settings.MaxRetries"
                Label="Max Retries"
                Min="0" Max="10" />
        }
    </MudCardContent>
    <MudCardActions>
        <MudButton
            Color="Color.Primary"
            Disabled="@(!page.SettingsValue.HasValue || page.SettingsValue.IsLoading)"
            OnClick="@(() => page.SaveCommand.Execute().Subscribe())">
            Save
        </MudButton>
        <MudButton
            Color="Color.Secondary"
            OnClick="@(() => page.ResetCommand.Execute().Subscribe())">
            Reset
        </MudButton>
    </MudCardActions>
</MudCard>

@code {
    private SettingsPage page = null!;
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        page = new SettingsPage(Repository);

        // Reactive updates
        subscription = page.SettingsValue
            .WhenAnyValue(v => v.IsLoading, v => v.HasValue)
            .Subscribe(_ => InvokeAsync(StateHasChanged));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await page.SettingsValue.GetIfNeeded();
        }
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

---

## Strategy 4: Observable Operations with Status

**Use case**: Track operation progress and display detailed status.

```csharp
public class DataImportPage : ReactiveObject
{
    private readonly IDataImporter importer;

    public DataImportPage(IDataImporter importer)
    {
        this.importer = importer;

        // Import command with observable execution
        ImportCommand = ReactiveCommand.CreateFromObservable<string, ImportResult>(
            filePath => Observable.FromAsync(ct => importer.ImportAsync(filePath, ct))
        );

        // Track execution state
        ImportCommand.IsExecuting
            .ToProperty(this, x => x.IsImporting, out _isImporting);

        // Track results
        ImportCommand
            .ToProperty(this, x => x.LastResult, out _lastResult);

        // Track errors
        ImportCommand.ThrownExceptions
            .Subscribe(ex => ErrorMessage = ex.Message);
    }

    public ReactiveCommand<string, ImportResult> ImportCommand { get; }

    private readonly ObservableAsPropertyHelper<bool> _isImporting;
    public bool IsImporting => _isImporting.Value;

    private readonly ObservableAsPropertyHelper<ImportResult?> _lastResult;
    public ImportResult? LastResult => _lastResult.Value;

    [Reactive] private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }
}
```

### UI with Operation Status

```razor
@page "/import"
@inject IDataImporter Importer
@implements IDisposable

<MudCard>
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">Data Import</MudText>
        </CardHeaderContent>
    </MudCardHeader>
    <MudCardContent>
        <MudTextField
            @bind-Value="filePath"
            Label="File Path"
            Disabled="@page.IsImporting" />

        @if (page.IsImporting)
        {
            <MudProgressLinear Indeterminate="true" Color="Color.Primary" />
            <MudText Typo="Typo.caption">Importing data...</MudText>
        }

        @if (page.LastResult != null)
        {
            <MudAlert Severity="Severity.Success" Class="mt-4">
                Imported @page.LastResult.RecordCount records in @page.LastResult.Duration.TotalSeconds.ToString("F2")s
            </MudAlert>
        }

        @if (!string.IsNullOrEmpty(page.ErrorMessage))
        {
            <MudAlert Severity="Severity.Error" Class="mt-4">
                @page.ErrorMessage
            </MudAlert>
        }
    </MudCardContent>
    <MudCardActions>
        <MudButton
            Color="Color.Primary"
            Disabled="@(string.IsNullOrWhiteSpace(filePath) || page.IsImporting)"
            OnClick="@(() => page.ImportCommand.Execute(filePath).Subscribe())">
            Import
        </MudButton>
    </MudCardActions>
</MudCard>

@code {
    private DataImportPage page = null!;
    private IDisposable? subscription;
    private string filePath = "";

    protected override void OnInitialized()
    {
        page = new DataImportPage(Importer);

        // Subscribe to all reactive properties
        subscription = page.WhenAnyValue(
                p => p.IsImporting,
                p => p.LastResult,
                p => p.ErrorMessage
            )
            .Subscribe(_ => InvokeAsync(StateHasChanged));
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

---

## Strategy 5: Collection Binding

**Use case**: Bind observable collections to list UI.

```csharp
using DynamicData;
using DynamicData.Binding;

public class ProductListPage : ReactiveObject
{
    private readonly SourceCache<Product, string> products;

    public ProductListPage(IProductRepository repository)
    {
        products = new SourceCache<Product, string>(p => p.Id);

        // Load products
        LoadCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var allProducts = await repository.GetAllAsync();
            products.AddOrUpdate(allProducts);
        });

        // Search filter
        var searchFilter = this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Select(BuildSearchFilter);

        // Bind to observable collection
        products.Connect()
            .Filter(searchFilter)
            .Transform(p => new ProductVM(p))
            .Sort(SortExpressionComparer<ProductVM>.Ascending(vm => vm.Name))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var boundProducts)
            .Subscribe();

        Products = boundProducts;
    }

    public ReactiveCommand<Unit, Unit> LoadCommand { get; }

    public ReadOnlyObservableCollection<ProductVM> Products { get; }

    [Reactive] private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    private Func<Product, bool> BuildSearchFilter(string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return _ => true;

        return product => product.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
}
```

### Blazor List Component

```razor
@page "/products"
@inject IProductRepository Repository
@implements IDisposable

<MudCard>
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">Products</MudText>
        </CardHeaderContent>
        <MudCardActions>
            <MudIconButton
                Icon="@Icons.Material.Filled.Refresh"
                OnClick="@(() => page.LoadCommand.Execute().Subscribe())" />
        </MudCardActions>
    </MudCardHeader>
    <MudCardContent>
        <MudTextField
            @bind-Value="page.SearchText"
            Label="Search"
            Adornment="Adornment.Start"
            AdornmentIcon="@Icons.Material.Filled.Search" />

        <MudList>
            @foreach (var product in page.Products)
            {
                <MudListItem>
                    <MudText>@product.Name</MudText>
                    <MudText Typo="Typo.caption">$@product.Price</MudText>
                </MudListItem>
            }
        </MudList>

        @if (!page.Products.Any())
        {
            <MudText Typo="Typo.caption" Class="mt-4">No products found</MudText>
        }
    </MudCardContent>
</MudCard>

@code {
    private ProductListPage page = null!;
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        page = new ProductListPage(Repository);

        // Reactive updates for collection changes
        subscription = page.Products.ToObservableChangeSet()
            .Subscribe(_ => InvokeAsync(StateHasChanged));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await page.LoadCommand.Execute();
        }
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

---

## Best Practices

### 1. Always Show Loading State

```csharp
// ✅ Good - Clear loading indicator
@if (getter.IsLoading)
{
    <MudProgressCircular Indeterminate="true" />
}
else if (getter.HasValue)
{
    <div>@getter.Value.Name</div>
}

// ❌ Avoid - No loading feedback
<div>@getter.Value?.Name</div>
```

### 2. Handle Errors Gracefully

```csharp
// ✅ Good - Show error message
@if (getter.QueryResult?.IsSuccess == false)
{
    <MudAlert Severity="Severity.Error">
        @getter.QueryResult.Error
    </MudAlert>
}

// ❌ Avoid - Silent failures
@if (getter.HasValue)
{
    <div>@getter.Value.Name</div>
}
```

### 3. Dispose Subscriptions

```csharp
// ✅ Good - Dispose in component
@implements IDisposable

@code {
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        subscription = page.WhenAnyValue(p => p.Data)
            .Subscribe(_ => InvokeAsync(StateHasChanged));
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}

// ❌ Avoid - Memory leaks
@code {
    protected override void OnInitialized()
    {
        page.WhenAnyValue(p => p.Data)
            .Subscribe(_ => InvokeAsync(StateHasChanged));
        // Never disposed!
    }
}
```

### 4. Use StateHasChanged Appropriately

```csharp
// ✅ Good - Trigger re-render after async operation
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        await getter.GetIfNeeded();
        StateHasChanged();  // Update UI with loaded data
    }
}

// ❌ Avoid - No re-render after load
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        await getter.GetIfNeeded();
        // UI won't update!
    }
}
```

---

## Summary

**Binding Async Data to UI:**

1. **Direct Getter** - Simple binding with manual state checks
2. **GetterVM** - Commands and reactive bindings
3. **ValueVM** - Two-way binding for editable data
4. **Observable Operations** - Track command execution status
5. **Collections** - Bind observable collections with filtering/sorting

**Key Points:**
- Always show loading states
- Handle errors gracefully
- Dispose subscriptions in components
- Use `StateHasChanged` after async operations
- Prefer `GetterVM`/`ValueVM` over raw getters

**Related Guides:**
- [Async Data Basics](../getting-started/02-async-data.md)
- [Create Custom ViewModels](create-custom-viewmodel.md)
- [Handle Loading States](handle-loading-states.md)
- [Async Data Domain Docs](../../data/async/README.md)
