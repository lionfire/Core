# LionFire.Blazor.Components

## Overview

**LionFire.Blazor.Components** provides base Blazor components integrated with ReactiveUI for building reactive, MVVM-based Blazor applications. This library includes terminal/log viewing components, async data display components, navigation menu components, and base MVVM infrastructure for Blazor.

**Layer**: Toolkit (Blazor UI Components)
**Target**: .NET 9.0
**Root Namespace**: `LionFire.Blazor.Components`
**SDK**: Microsoft.NET.Sdk.Razor

## Key Dependencies

### NuGet Packages
- **ReactiveUI.Blazor** - ReactiveUI integration for Blazor
- **ReactiveUI.SourceGenerators** - Code generation for reactive properties
- **MediatR** - Mediator pattern for component communication
- **Microsoft.AspNetCore.Components.Web** - ASP.NET Core Blazor components
- **Microsoft.AspNetCore.Components.Authorization** - Authentication/authorization
- **Microsoft.AspNetCore.Components.QuickGrid** - Data grid component
- **CircularBuffer** - Efficient circular buffer for logs
- **DynamicData** - Reactive collections
- **ObservableCollections** - Observable collection types
- **Swordfish.NET.CollectionsV3** - Advanced collection types
- **Microsoft.CodeAnalysis** - Code analysis tools

### LionFire Dependencies
- **LionFire.Flex** - FlexData pattern for runtime extensibility
- **LionFire.Mvvm** - MVVM abstractions and base classes
- **LionFire.UI** - UI abstractions
- **LionFire.UI.Entities** - Entity UI components

## Core Components

### 1. Terminal/Log Components

#### TerminalView Component
**File**: `Terminal/TerminalView.razor`

Real-time log viewer with filtering, buffering, and reactive updates.

**Usage:**
```razor
@page "/logs"
@using LionFire.Blazor.Components.Terminal

<TerminalView ViewModel="@logViewModel" Style="height: 600px;" />

@code {
    private LogVM logViewModel = new LogVM
    {
        MaxBufferSize = 500,
        LogLevel = LogLevel.Debug,
        Reverse = true,
        Debounce = TimeSpan.FromSeconds(1.0)
    };

    protected override void OnInitialized()
    {
        // Add log entries
        logViewModel.Append("Application started", LogLevel.Information, "App");
        logViewModel.Append("Debug info", LogLevel.Debug, "Debug");
    }
}
```

**LogVM Features:**
- **Circular buffer**: Automatic pruning when `MaxBufferSize` exceeded
- **Debouncing**: Throttle UI updates with configurable `Debounce`
- **Filtering**: Filter by `LogLevel`
- **ILogger compatible**: Can be used as Microsoft.Extensions.Logging.ILogger
- **Reactive**: Built on DynamicData's `SourceCache` for efficient updates
- **Reverse mode**: Newest entries first or last

**LogEntry Structure:**
```csharp
public readonly struct LogEntry
{
    public DateTimeOffset Timestamp { get; }
    public DateTimeOffset LocalTimestamp => Timestamp.ToLocalTime();
    public LogLevel Level { get; }
    public string LevelAbbreviation { get; } // "DB", "IN", "ER", etc.
    public string Category { get; }
    public string ShortCategory { get; } // Last segment after '.'
    public string Text { get; }
    public int Index { get; }
}
```

**LogVM Configuration:**
```csharp
var logVM = new LogVM
{
    MaxBufferSize = 1000,        // Maximum log entries
    IsEnabled = true,             // Enable/disable logging
    LogLevel = LogLevel.Information, // Minimum log level
    Reverse = true,               // Newest first
    Debounce = TimeSpan.FromSeconds(2.0) // UI update throttle
};

// Use as ILogger
ILogger logger = logVM;
logger.LogInformation("Hello from ILogger");

// Direct append
logVM.Append("Direct message", LogLevel.Warning, "MyCategory");

// Clear all entries
logVM.Clear();
```

#### OutputPane Component
**File**: `Terminal/OutputPane.razor`

Simplified output display pane with QuickGrid-based rendering.

**Features:**
- Timestamp column with millisecond precision
- Log level abbreviation
- Category display with short name
- Message column
- Virtualized rendering for performance

#### ConsoleActivity Component
**File**: `Terminal/ConsoleActivity.razor`

Console activity monitoring component.

### 2. MVVM Components

#### Async Component
**File**: `Mvvm/Async.razor`

Generic component for displaying async operation results with loading state.

**Usage:**
```razor
<Async TSource="int"
       TResult="User"
       TDisplay="string"
       Source="userId"
       Resolver="LoadUserAsync"
       Transform="user => user.Name">
    <Ready Context="userName">
        <h2>Welcome, @userName!</h2>
    </Ready>
    <InProgress>
        <div class="spinner">Loading user...</div>
    </InProgress>
</Async>

@code {
    private int userId = 42;

    private async Task<User> LoadUserAsync(int id)
    {
        await Task.Delay(1000); // Simulate API call
        return await userService.GetUserByIdAsync(id);
    }
}
```

**Type Parameters:**
- `TSource` - Input type (e.g., user ID)
- `TResult` - Async operation result type (e.g., User entity)
- `TDisplay` - Display type after transformation (e.g., string)

**Parameters:**
- `Source` - Input value triggering async operation
- `Resolver` - `Func<TSource, Task<TResult>>` to resolve data
- `Transform` - `Func<TResult, TDisplay>` to transform result for display
- `Ready` - RenderFragment displayed when data ready
- `InProgress` - RenderFragment displayed during loading

**Features:**
- Automatic refresh on first render
- Manual refresh via `Refresh()` method
- Loading/ready state management
- Transformation pipeline for display
- Uses `FireAndForget()` extension for background state updates

#### ItemPage Component
**File**: `Mvvm/Views/ItemPage.cs`

Base class for item detail pages in MVVM pattern.

### 3. Layout Components

#### UserLayoutVM
**File**: `Layouts/UserLayoutVM.cs`

Base ViewModel for user-specific layouts with authentication integration.

**Usage:**
```csharp
public class MyLayoutVM : UserLayoutVM
{
    public MyLayoutVM(AuthenticationStateProvider authStateProvider)
        : base(authStateProvider)
    {
    }

    protected override async ValueTask ConfigureUserServices(IServiceCollection services)
    {
        // Register user-specific services
        services.AddScoped<IUserPreferences, UserPreferences>();
        services.AddScoped<IUserDataService>(sp =>
            new UserDataService(UserId));
    }

    protected override async ValueTask OnUserChanged()
    {
        await base.OnUserChanged();
        // React to user login/logout
        await LoadUserPreferences();
    }
}
```

**Features:**
- **Authentication integration**: Uses `AuthenticationStateProvider`
- **User services**: Per-user `IServiceProvider` via `ConfigureUserServices()`
- **Reactive user tracking**: `UserId` property with change notifications
- **ReactiveUI integration**: Inherits `ReactiveObject`
- **Lifecycle management**: `InitializeAsync()`, `Dispose()` pattern
- **Effective username**: `EffectiveUserName` returns "Anonymous" if not logged in

**Properties:**
```csharp
public string? UserId { get; }                   // Current user ID
public string EffectiveUserName { get; }         // UserId ?? "Anonymous"
public IServiceProvider? UserServices { get; }   // User-specific DI container
public AuthenticationStateProvider AuthStateProvider { get; }
```

**Lifecycle:**
```csharp
// 1. Constructor injection
var vm = new UserLayoutVM(authStateProvider);

// 2. Initialize (async)
await vm.InitializeAsync();

// 3. User changes trigger OnUserChanged()
// 4. UserServices rebuilt via ConfigureUserServices()

// 5. Cleanup
vm.Dispose();
```

### 4. Navigation Components

#### NavMenuItem
**File**: `Menus/NavMenuItem.razor`

Navigation menu item component.

### 5. Utility Components

#### CascadingT
**File**: `CascadingT.razor`

Generic cascading value component.

**Usage:**
```razor
<CascadingT Value="myService">
    <ChildContent>
        @* Child components can receive myService via [CascadingParameter] *@
    </ChildContent>
</CascadingT>
```

### 6. Pages

#### Error Page
**File**: `Components/Pages/Error.razor`

Standard error page component.

## Directory Structure

```
src/LionFire.Blazor.Components/
├── Components/
│   └── Pages/
│       └── Error.razor              # Error page
├── Layouts/
│   └── UserLayoutVM.cs              # User layout ViewModel
├── Menus/
│   └── NavMenuItem.razor            # Navigation menu item
├── Mvvm/
│   ├── Async.razor                  # Async data display component
│   └── Views/
│       └── ItemPage.cs              # Item page base class
├── Reactive/                        # Reactive utilities
├── Terminal/
│   ├── ConsoleActivity.razor        # Console activity viewer
│   ├── OutputPane.razor             # Output pane component
│   └── TerminalView.razor           # Terminal/log viewer
├── wwwroot/                         # Static web assets
├── CascadingT.razor                 # Generic cascading value
├── Component1.razor                 # Example component
├── ExampleJsInterop.cs              # JS interop example
├── GlobalUsings.cs                  # Global using directives
└── _Imports.razor                   # Shared imports
```

## Reactive Patterns

### ReactiveUI Integration

Components and ViewModels use ReactiveUI for reactive properties:

```csharp
public partial class UserLayoutVM : ReactiveObject
{
    // Source generator creates property with notifications
    [Reactive]
    private string? _userId;

    // Manually wire up reactive subscriptions
    protected void PostConstructor()
    {
        this.WhenAnyValue(x => x.UserId)
            .Subscribe(async _ => await OnUserChanged());
    }
}
```

**Benefits:**
- Automatic `INotifyPropertyChanged` implementation
- Reactive LINQ queries on properties
- Integration with Blazor's state management

### DynamicData for Collections

Terminal components use DynamicData for reactive collections:

```csharp
private SourceCache<LogEntry, int> sourceCache =
    new SourceCache<LogEntry, int>(x => x.Index);

public IObservableCache<LogEntry, int> Lines => sourceCache;

// Edit atomically
sourceCache.Edit(updater =>
{
    updater.AddOrUpdate(newEntry);

    // Prune old entries
    while (sourceCache.Count > MaxBufferSize)
    {
        updater.Remove(oldestEntry.Index);
    }
});
```

**Benefits:**
- Efficient change notifications
- Atomic batch updates
- Observable queries on collections

## Design Patterns

### MVVM Pattern

Components follow MVVM with clear separation:

**View (Razor):**
```razor
<TerminalView ViewModel="@logViewModel" />
```

**ViewModel (C#):**
```csharp
public class LogVM : ReactiveObject, ILogger
{
    [Reactive]
    private int _maxBufferSize = 1000;

    public IObservableCache<LogEntry, int> Lines => sourceCache;
}
```

### Async Component Pattern

The `Async<TSource, TResult, TDisplay>` component demonstrates:
- Generic type parameters for reusability
- Resolver function for data fetching
- Transform function for display
- Render fragments for customization
- Fire-and-forget state updates

```razor
@typeparam TResult
@typeparam TSource
@typeparam TDisplay

@if (Resolved)
{
    @Ready(Transform(Result))
}
else
{
    @InProgress
}

@code {
    [Parameter] public Func<TSource, Task<TResult>> Resolver { get; set; }

    public async Task Refresh()
    {
        Result = await Resolver.Invoke(Source);
        Resolved = true;
        InvokeAsync(StateHasChanged).FireAndForget();
    }
}
```

### User-Specific Services Pattern

`UserLayoutVM` demonstrates per-user service containers:

```csharp
protected virtual async ValueTask ConfigureUserServices(IServiceCollection services)
{
    // Register services specific to current user
    services.AddScoped<IUserData>(sp =>
        new UserData(UserId, connectionString));
}

// Rebuilt when user changes
UserServices = services.BuildServiceProvider();
```

**Use Case**: Different users might need different data sources, permissions, or configurations.

## Testing Considerations

### Mocking Dependencies

For `UserLayoutVM`:
```csharp
var mockAuthProvider = new Mock<AuthenticationStateProvider>();
mockAuthProvider.Setup(x => x.GetAuthenticationStateAsync())
    .ReturnsAsync(new AuthenticationState(claimsPrincipal));

var vm = new UserLayoutVM(mockAuthProvider.Object);
await vm.InitializeAsync();

Assert.Equal("testuser", vm.UserId);
```

### Testing Async Component

```csharp
// Create test resolver
Func<int, Task<string>> resolver = async id =>
{
    await Task.Delay(10);
    return $"User {id}";
};

// Render component
var component = RenderComponent<Async<int, string, string>>(parameters => parameters
    .Add(p => p.Source, 42)
    .Add(p => p.Resolver, resolver)
    .Add(p => p.Transform, s => s.ToUpper())
);

// Wait for resolution
component.WaitForState(() => component.Instance.Resolved);

// Assert result
Assert.Contains("USER 42", component.Markup);
```

### Testing LogVM

```csharp
var logVM = new LogVM
{
    Debounce = TimeSpan.Zero, // Disable debouncing for tests
    MaxBufferSize = 10
};

logVM.Append("Test message", LogLevel.Information, "Test");

Assert.Single(logVM.Lines.Items);
Assert.Equal("Test message", logVM.Lines.Items.First().Text);

// Test buffer pruning
for (int i = 0; i < 20; i++)
    logVM.Append($"Message {i}");

Assert.Equal(10, logVM.Lines.Items.Count());
```

## Common Usage Patterns

### Pattern 1: Log Viewer in Blazor App

```razor
@page "/logs"
@implements IDisposable
@inject ILogger<MyPage> Logger

<h3>Application Logs</h3>
<TerminalView ViewModel="@logVM" Style="height: 400px;" />

@code {
    private LogVM logVM = new();
    private IDisposable? logSubscription;

    protected override void OnInitialized()
    {
        // Bridge ILogger to LogVM
        logSubscription = Logger.WhenAnyValue(x => x /* log events */)
            .Subscribe(logEvent =>
                logVM.Append(logEvent.Message, logEvent.Level));
    }

    public void Dispose() => logSubscription?.Dispose();
}
```

### Pattern 2: Async User Profile

```razor
<Async TSource="string"
       TResult="UserProfile"
       TDisplay="UserProfile"
       Source="@userId"
       Resolver="LoadProfileAsync"
       Transform="profile => profile">
    <Ready Context="profile">
        <div class="profile-card">
            <img src="@profile.AvatarUrl" />
            <h3>@profile.DisplayName</h3>
            <p>@profile.Bio</p>
        </div>
    </Ready>
    <InProgress>
        <div class="loading-spinner"></div>
    </InProgress>
</Async>
```

### Pattern 3: Authenticated Layout

```razor
@inherits LayoutComponentBase
@inject AuthenticationStateProvider AuthStateProvider

<CascadingAuthenticationState>
    <CascadingT Value="@layoutVM">
        <div class="page">
            <header>
                Welcome, @layoutVM.EffectiveUserName
            </header>
            <main>
                @Body
            </main>
        </div>
    </CascadingT>
</CascadingAuthenticationState>

@code {
    private UserLayoutVM layoutVM;

    protected override async Task OnInitializedAsync()
    {
        layoutVM = new UserLayoutVM(AuthStateProvider);
        await layoutVM.InitializeAsync();
    }

    public void Dispose() => layoutVM?.Dispose();
}
```

### Pattern 4: MediatR Integration

Components can communicate via MediatR:

```csharp
@inject IMediator Mediator

@code {
    private async Task OnItemSelected(int itemId)
    {
        // Notify other components
        await Mediator.Publish(new ItemSelectedNotification(itemId));
    }

    protected override void OnInitialized()
    {
        // Subscribe to notifications
        // (Requires custom MediatR subscription mechanism)
    }
}
```

## Integration with Other Libraries

### With LionFire.Mvvm

Components use ViewModels from LionFire.Mvvm:

```csharp
public class MyComponentVM : ViewModelBase
{
    // Inherits MVVM infrastructure
}
```

### With LionFire.UI.Entities

Entity-specific UI components can leverage entity abstractions:

```csharp
@using LionFire.UI.Entities

<EntityEditor Entity="@myEntity" />
```

### With LionFire.Flex

Components can use FlexData for runtime extensions:

```csharp
public class ExtendedLogVM : LogVM, IHasFlexData
{
    public FlexData FlexData { get; } = new();
}

// Add custom properties at runtime
logVM.FlexData["CustomFilter"] = myFilter;
```

## Performance Considerations

### Debouncing Log Updates

For high-frequency log updates:

```csharp
var logVM = new LogVM
{
    Debounce = TimeSpan.FromSeconds(0.5) // Batch updates every 500ms
};
```

### Virtualization

`QuickGrid` in `TerminalView` uses virtualization:
- Only visible rows rendered
- Efficient for large log buffers
- Smooth scrolling performance

### Circular Buffer

`LogVM` uses circular buffer pattern:
- Automatic old entry removal
- Constant memory usage
- O(1) add/remove operations

## Related Projects

- **LionFire.Blazor.Components.MudBlazor** - MudBlazor-specific components ([CLAUDE.md](../LionFire.Blazor.Components.MudBlazor/CLAUDE.md))
- **LionFire.Blazor.Components.UI** - Additional UI utilities ([CLAUDE.md](../LionFire.Blazor.Components.UI/CLAUDE.md))
- **LionFire.Mvvm** - MVVM abstractions ([CLAUDE.md](../LionFire.Mvvm/CLAUDE.md))
- **LionFire.Reactive** - Reactive extensions ([CLAUDE.md](../LionFire.Reactive/CLAUDE.md))
- **LionFire.UI** - UI abstractions ([CLAUDE.md](../LionFire.UI/CLAUDE.md))

## Documentation

- **ReactiveUI Documentation**: https://www.reactiveui.net/
- **DynamicData Documentation**: https://github.com/reactivemarbles/DynamicData
- **Blazor Documentation**: https://learn.microsoft.com/aspnet/core/blazor/

## Summary

**LionFire.Blazor.Components** provides essential building blocks for reactive Blazor applications:

- **Terminal/Log Components**: Real-time log viewing with `TerminalView`, `LogVM`, and `OutputPane`
- **Async Component**: Generic async data display with loading states
- **MVVM Infrastructure**: `UserLayoutVM` with authentication and per-user services
- **ReactiveUI Integration**: Reactive properties and observable collections
- **DynamicData**: Efficient reactive collections for log entries
- **MediatR Support**: Component communication via mediator pattern

**Key Strengths:**
- Strong separation of concerns (MVVM)
- Reactive data binding
- Performance-optimized (virtualization, debouncing, circular buffers)
- Type-safe generic components
- Authentication-aware layouts

**Use When:**
- Building MVVM Blazor applications
- Need real-time log/terminal viewers
- Require async data display with loading states
- Want ReactiveUI in Blazor
- Need per-user service containers

**Typical Use Cases:**
- Admin dashboards with log viewers
- SaaS applications with user-specific data
- Real-time monitoring UIs
- Async data-heavy applications
