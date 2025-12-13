# How-To: Handle Loading States

## Problem

You need to provide good UX during async operations with loading indicators, error handling, retry mechanisms, and smooth transitions.

## Solution

Use LionFire's `IGetter<T>` with ReactiveUI observable patterns and MudBlazor UI components for comprehensive loading state management.

---

## Strategy 1: Basic Loading Indicators

**Use case**: Simple loading spinner with error handling.

### Using IGetter ReadState

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
            await Task.Delay(2000, ct); // Simulate network delay

            var response = await httpClient.GetAsync($"/api/users/{userId}", ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var profile = JsonSerializer.Deserialize<UserProfile>(json);

            return GetResult.Success(profile!);
        }
        catch (Exception ex)
        {
            return GetResult.Failure<UserProfile>($"Error: {ex.Message}");
        }
    }
}
```

### Blazor Component with Loading States

```razor
@page "/profile/{UserId}"
@inject HttpClient HttpClient

<MudCard>
    <MudCardContent>
        @switch (getter.ReadState)
        {
            case ReadState.NotStarted:
                <MudText>Ready to load</MudText>
                <MudButton OnClick="Load">Load Profile</MudButton>
                break;

            case ReadState.Loading:
                <div class="d-flex align-center gap-4">
                    <MudProgressCircular Indeterminate="true" Size="Size.Small" />
                    <MudText>Loading profile...</MudText>
                </div>
                break;

            case ReadState.Success:
                var profile = getter.ReadCacheValue;
                <MudText><strong>Name:</strong> @profile.Name</MudText>
                <MudText><strong>Email:</strong> @profile.Email</MudText>
                <MudButton OnClick="Reload">Reload</MudButton>
                break;

            case ReadState.Failed:
                <MudAlert Severity="Severity.Error">
                    @getter.QueryResult?.Error
                </MudAlert>
                <MudButton OnClick="Retry">Retry</MudButton>
                break;
        }
    </MudCardContent>
</MudCard>

@code {
    [Parameter] public string UserId { get; set; } = "";

    private UserProfileGetter getter = null!;

    protected override void OnInitialized()
    {
        getter = new UserProfileGetter(UserId, HttpClient);
    }

    private async Task Load()
    {
        await getter.GetIfNeeded();
        StateHasChanged();
    }

    private async Task Reload()
    {
        getter.DiscardValue();
        await getter.GetIfNeeded();
        StateHasChanged();
    }

    private async Task Retry() => await Load();
}
```

---

## Strategy 2: Skeleton Loaders

**Use case**: Show content placeholder while loading.

```razor
@page "/products"
@inject IProductRepository Repository

<MudCard>
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">Products</MudText>
        </CardHeaderContent>
    </MudCardHeader>
    <MudCardContent>
        @if (page.IsLoading)
        {
            <!-- Skeleton loader -->
            @for (int i = 0; i < 5; i++)
            {
                <MudPaper Class="pa-4 mb-2">
                    <MudSkeleton SkeletonType="SkeletonType.Text" Width="60%" />
                    <MudSkeleton SkeletonType="SkeletonType.Text" Width="40%" />
                    <MudSkeleton SkeletonType="SkeletonType.Rectangle" Height="100px" Class="mt-2" />
                </MudPaper>
            }
        }
        else if (page.Products.Any())
        {
            <MudList>
                @foreach (var product in page.Products)
                {
                    <MudListItem>
                        <MudText Typo="Typo.h6">@product.Name</MudText>
                        <MudText Typo="Typo.body2">@product.Description</MudText>
                        <MudText Typo="Typo.caption">$@product.Price</MudText>
                    </MudListItem>
                }
            </MudList>
        }
        else
        {
            <MudText Class="text-center">No products available</MudText>
        }
    </MudCardContent>
</MudCard>

@code {
    [Parameter] public bool IsLoading { get; set; } = true;
    [Parameter] public List<Product> Products { get; set; } = new();

    private ProductsPage page = null!;

    protected override void OnInitialized()
    {
        page = new ProductsPage(Repository);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await page.LoadCommand.Execute();
            StateHasChanged();
        }
    }
}
```

---

## Strategy 3: Progress Indicators

**Use case**: Show progress percentage for long operations.

### ViewModel with Progress Tracking

```csharp
using ReactiveUI;
using System.Reactive;

public class DataImportPage : ReactiveObject
{
    private readonly IDataImporter importer;

    public DataImportPage(IDataImporter importer)
    {
        this.importer = importer;

        // Import command
        ImportCommand = ReactiveCommand.CreateFromTask<string>(ImportAsync);

        // Track execution
        ImportCommand.IsExecuting
            .ToProperty(this, x => x.IsImporting, out _isImporting);

        // Track errors
        ImportCommand.ThrownExceptions
            .Subscribe(ex =>
            {
                ErrorMessage = ex.Message;
                Progress = 0;
            });
    }

    public ReactiveCommand<string, Unit> ImportCommand { get; }

    private readonly ObservableAsPropertyHelper<bool> _isImporting;
    public bool IsImporting => _isImporting.Value;

    [Reactive] private int _progress;
    public int Progress
    {
        get => _progress;
        set => this.RaiseAndSetIfChanged(ref _progress, value);
    }

    [Reactive] private string _statusMessage = "";
    public string StatusMessage
    {
        get => _statusMessage;
        set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

    [Reactive] private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    private async Task ImportAsync(string filePath)
    {
        Progress = 0;
        StatusMessage = "Reading file...";

        // Simulate progress
        for (int i = 0; i <= 100; i += 10)
        {
            Progress = i;
            StatusMessage = $"Importing... {i}%";
            await Task.Delay(500);
        }

        await importer.ImportAsync(filePath);

        Progress = 100;
        StatusMessage = "Import complete!";
    }
}
```

### Blazor Component with Progress Bar

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
            <div class="mt-4">
                <MudProgressLinear
                    Value="@page.Progress"
                    Color="Color.Primary"
                    Size="Size.Large"
                    Class="mb-2" />
                <MudText Typo="Typo.caption" Align="Align.Center">
                    @page.StatusMessage
                </MudText>
            </div>
        }

        @if (!string.IsNullOrEmpty(page.ErrorMessage))
        {
            <MudAlert Severity="Severity.Error" Class="mt-4">
                @page.ErrorMessage
            </MudAlert>
        }

        @if (page.Progress == 100 && !page.IsImporting)
        {
            <MudAlert Severity="Severity.Success" Class="mt-4">
                Import completed successfully!
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

        subscription = page.WhenAnyValue(
                p => p.IsImporting,
                p => p.Progress,
                p => p.StatusMessage,
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

## Strategy 4: Optimistic Updates

**Use case**: Update UI immediately, then sync with server.

```csharp
public class TodoListPage : ReactiveObject
{
    private readonly SourceCache<Todo, string> todos;
    private readonly ITodoRepository repository;

    public TodoListPage(ITodoRepository repository)
    {
        this.repository = repository;
        todos = new SourceCache<Todo, string>(t => t.Id);

        todos.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var boundTodos)
            .Subscribe();

        Todos = boundTodos;

        // Toggle command with optimistic update
        ToggleCommand = ReactiveCommand.CreateFromTask<Todo>(ToggleTodoAsync);
    }

    public ReadOnlyObservableCollection<Todo> Todos { get; }
    public ReactiveCommand<Todo, Unit> ToggleCommand { get; }

    [Reactive] private string _savingStatus = "";
    public string SavingStatus
    {
        get => _savingStatus;
        set => this.RaiseAndSetIfChanged(ref _savingStatus, value);
    }

    private async Task ToggleTodoAsync(Todo todo)
    {
        // 1. Optimistic update (immediate UI feedback)
        var originalState = todo.IsCompleted;
        todo.IsCompleted = !todo.IsCompleted;
        todos.AddOrUpdate(todo);

        SavingStatus = "Saving...";

        try
        {
            // 2. Sync with server
            await Task.Delay(1000); // Simulate network delay
            await repository.UpdateAsync(todo);

            SavingStatus = "✓ Saved";
            await Task.Delay(1000);
            SavingStatus = "";
        }
        catch (Exception ex)
        {
            // 3. Rollback on error
            todo.IsCompleted = originalState;
            todos.AddOrUpdate(todo);

            SavingStatus = $"✗ Error: {ex.Message}";
            await Task.Delay(3000);
            SavingStatus = "";
        }
    }
}
```

### Blazor Component with Optimistic UI

```razor
@page "/todos"
@inject ITodoRepository Repository
@implements IDisposable

<MudCard>
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">Todo List</MudText>
        </CardHeaderContent>
        <MudCardActions>
            @if (!string.IsNullOrEmpty(page.SavingStatus))
            {
                <MudChip Size="Size.Small" Color="GetStatusColor()">
                    @page.SavingStatus
                </MudChip>
            }
        </MudCardActions>
    </MudCardHeader>
    <MudCardContent>
        <MudList>
            @foreach (var todo in page.Todos)
            {
                <MudListItem>
                    <MudCheckBox
                        Checked="@todo.IsCompleted"
                        CheckedChanged="@(() => page.ToggleCommand.Execute(todo).Subscribe())"
                        Label="@todo.Title" />
                </MudListItem>
            }
        </MudList>
    </MudCardContent>
</MudCard>

@code {
    private TodoListPage page = null!;
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        page = new TodoListPage(Repository);

        subscription = page.WhenAnyValue(p => p.SavingStatus)
            .Subscribe(_ => InvokeAsync(StateHasChanged));
    }

    private Color GetStatusColor() => page.SavingStatus switch
    {
        var s when s.StartsWith("✓") => Color.Success,
        var s when s.StartsWith("✗") => Color.Error,
        _ => Color.Info
    };

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

---

## Strategy 5: Debouncing & Throttling

**Use case**: Reduce API calls for search/autocomplete.

```csharp
public class SearchPage : ReactiveObject
{
    private readonly ISearchService searchService;

    public SearchPage(ISearchService searchService)
    {
        this.searchService = searchService;

        // Debounced search (waits for 500ms of no input)
        this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .DistinctUntilChanged()
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .SelectMany(text => Observable.FromAsync(ct => PerformSearchAsync(text, ct)))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(results =>
            {
                Results.Clear();
                Results.AddRange(results);
                IsSearching = false;
            });

        // Track searching state
        this.WhenAnyValue(x => x.SearchText)
            .Subscribe(_ => IsSearching = true);
    }

    [Reactive] private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    [Reactive] private bool _isSearching;
    public bool IsSearching
    {
        get => _isSearching;
        set => this.RaiseAndSetIfChanged(ref _isSearching, value);
    }

    public ObservableCollection<SearchResult> Results { get; } = new();

    private async Task<List<SearchResult>> PerformSearchAsync(string query, CancellationToken ct)
    {
        await Task.Delay(500, ct); // Simulate API delay
        return await searchService.SearchAsync(query, ct);
    }
}
```

### Blazor Search Component

```razor
@page "/search"
@inject ISearchService SearchService
@implements IDisposable

<MudCard>
    <MudCardContent>
        <MudTextField
            @bind-Value="page.SearchText"
            Label="Search"
            Adornment="Adornment.End"
            AdornmentIcon="@(page.IsSearching ? Icons.Material.Filled.HourglassEmpty : Icons.Material.Filled.Search)"
            Immediate="true" />

        @if (page.IsSearching)
        {
            <div class="d-flex align-center gap-2 mt-2">
                <MudProgressCircular Size="Size.Small" Indeterminate="true" />
                <MudText Typo="Typo.caption">Searching...</MudText>
            </div>
        }

        @if (page.Results.Any())
        {
            <MudList Class="mt-4">
                @foreach (var result in page.Results)
                {
                    <MudListItem>
                        <MudText Typo="Typo.h6">@result.Title</MudText>
                        <MudText Typo="Typo.body2">@result.Description</MudText>
                    </MudListItem>
                }
            </MudList>
        }
        else if (!string.IsNullOrWhiteSpace(page.SearchText) && !page.IsSearching)
        {
            <MudText Class="mt-4 text-center">No results found</MudText>
        }
    </MudCardContent>
</MudCard>

@code {
    private SearchPage page = null!;
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        page = new SearchPage(SearchService);

        subscription = page.WhenAnyValue(
                p => p.IsSearching,
                p => p.Results.Count
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

## Strategy 6: Cancellation Support

**Use case**: Cancel long-running operations.

```csharp
public class LongOperationPage : ReactiveObject
{
    private CancellationTokenSource? cts;

    public LongOperationPage()
    {
        // Start command
        StartCommand = ReactiveCommand.CreateFromTask(
            StartOperationAsync,
            this.WhenAnyValue(x => x.IsRunning).Select(running => !running)
        );

        // Cancel command
        CancelCommand = ReactiveCommand.Create(
            CancelOperation,
            this.WhenAnyValue(x => x.IsRunning)
        );
    }

    [Reactive] private bool _isRunning;
    public bool IsRunning
    {
        get => _isRunning;
        set => this.RaiseAndSetIfChanged(ref _isRunning, value);
    }

    [Reactive] private int _progress;
    public int Progress
    {
        get => _progress;
        set => this.RaiseAndSetIfChanged(ref _progress, value);
    }

    [Reactive] private string _statusMessage = "";
    public string StatusMessage
    {
        get => _statusMessage;
        set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    private async Task StartOperationAsync()
    {
        IsRunning = true;
        Progress = 0;
        cts = new CancellationTokenSource();

        try
        {
            for (int i = 0; i <= 100; i += 5)
            {
                cts.Token.ThrowIfCancellationRequested();

                Progress = i;
                StatusMessage = $"Processing... {i}%";
                await Task.Delay(500, cts.Token);
            }

            StatusMessage = "✓ Completed";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "✗ Cancelled";
        }
        finally
        {
            IsRunning = false;
            cts?.Dispose();
            cts = null;
        }
    }

    private void CancelOperation()
    {
        cts?.Cancel();
    }
}
```

### Blazor Component with Cancellation

```razor
@page "/long-operation"
@implements IDisposable

<MudCard>
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">Long Operation</MudText>
        </CardHeaderContent>
    </MudCardHeader>
    <MudCardContent>
        @if (page.IsRunning)
        {
            <MudProgressLinear Value="@page.Progress" Color="Color.Primary" Class="mb-2" />
            <MudText Typo="Typo.caption" Align="Align.Center">@page.StatusMessage</MudText>
        }
        else
        {
            <MudText Align="Align.Center">@page.StatusMessage</MudText>
        }
    </MudCardContent>
    <MudCardActions>
        @if (!page.IsRunning)
        {
            <MudButton
                Color="Color.Primary"
                OnClick="@(() => page.StartCommand.Execute().Subscribe())">
                Start
            </MudButton>
        }
        else
        {
            <MudButton
                Color="Color.Error"
                OnClick="@(() => page.CancelCommand.Execute().Subscribe())">
                Cancel
            </MudButton>
        }
    </MudCardActions>
</MudCard>

@code {
    private LongOperationPage page = null!;
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        page = new LongOperationPage();

        subscription = page.WhenAnyValue(
                p => p.IsRunning,
                p => p.Progress,
                p => p.StatusMessage
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

## Best Practices

### 1. Always Show Loading State

```csharp
// ✅ Good - Clear loading indicator
@if (isLoading)
{
    <MudProgressCircular Indeterminate="true" />
}
else
{
    <Content />
}

// ❌ Avoid - No loading feedback
<Content />  // Might be null while loading
```

### 2. Provide Error Recovery

```csharp
// ✅ Good - Retry button on error
@if (hasError)
{
    <MudAlert Severity="Severity.Error">
        @errorMessage
        <MudButton OnClick="Retry">Retry</MudButton>
    </MudAlert>
}

// ❌ Avoid - No way to recover
@if (hasError)
{
    <MudText>Error occurred</MudText>
}
```

### 3. Use Appropriate Indicators

```csharp
// ✅ Good - Match indicator to operation
await QuickOperation();  // Use small spinner
await LongOperation();   // Use progress bar with percentage
await Search();          // Use debouncing + subtle indicator

// ❌ Avoid - Same indicator for everything
await AnyOperation();  // Always full-screen spinner
```

### 4. Prevent Double Submission

```csharp
// ✅ Good - Disable button while saving
<MudButton
    Disabled="@isSaving"
    OnClick="Save">
    @(isSaving ? "Saving..." : "Save")
</MudButton>

// ❌ Avoid - Multiple submissions possible
<MudButton OnClick="Save">Save</MudButton>
```

---

## Summary

**Loading State Strategies:**

1. **Basic Indicators** - ReadState with spinners
2. **Skeleton Loaders** - Content placeholders
3. **Progress Indicators** - Percentage-based progress
4. **Optimistic Updates** - Immediate UI feedback
5. **Debouncing** - Reduce API calls
6. **Cancellation** - Cancel long operations

**Key Points:**
- Always show loading state
- Provide error recovery (retry)
- Use appropriate indicators
- Prevent double submission
- Support cancellation for long ops

**Related Guides:**
- [Bind Async Data to UI](bind-async-data-to-ui.md)
- [Async Data Basics](../getting-started/02-async-data.md)
- [Create Custom ViewModels](create-custom-viewmodel.md)
- [Async Data Domain Docs](../../data/async/README.md)
