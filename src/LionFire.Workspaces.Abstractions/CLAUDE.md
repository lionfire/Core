# LionFire.Workspaces.Abstractions

## Overview

Core abstractions for the workspace system. This library provides the interfaces and minimal types needed for workspace infrastructure without depending on implementation details.

**Purpose**: Define contracts for workspace services, allowing implementation and UI libraries to depend on abstractions without circular dependencies.

---

## Core Interfaces

### IWorkspace

**Purpose**: Marker interface representing a workspace instance.

```csharp
/// <summary>
/// Data and business logic for a domain object that is a user interface concept
/// </summary>
public interface IWorkspace
{
    // Currently minimal - marker interface
    // Implementations can extend via FlexObject pattern
}
```

**Usage**:
```csharp
// Workspaces are typically persisted as files
// C:\Users\Alice\Trading\Workspaces\workspace1.hjson

// Access via IObservableReader<string, Workspace>
public class Workspace : FlexObject, IWorkspace
{
    // Can add workspace-specific properties via FlexData
}
```

**Future**: May define properties for workspace metadata, settings, or capabilities.

---

### IWorkspaceServiceConfigurator

**Purpose**: Callback interface for configuring workspace-scoped services.

```csharp
public interface IWorkspaceServiceConfigurator
{
    /// <summary>
    /// Called when a workspace service provider is being built.
    /// Allows registration of workspace-scoped services.
    /// </summary>
    ValueTask ConfigureWorkspaceServices(
        IServiceCollection services,
        UserWorkspacesService userWorkspacesService,
        string? workspaceId);
}
```

**When Called**: During workspace service provider creation, before the provider is built.

**Typical Use**: Register `IObservableReader/Writer` instances for workspace document types.

**Implementation Example**:
```csharp
public class WorkspaceTypesConfigurator : IWorkspaceServiceConfigurator
{
    public WorkspaceConfiguration Options { get; }
    public IServiceProvider ServiceProvider { get; }

    public async ValueTask ConfigureWorkspaceServices(
        IServiceCollection services,
        UserWorkspacesService userWorkspacesService,
        string? workspaceId)
    {
        // Get workspace directory
        var workspaceRef = userWorkspacesService.UserWorkspaces?.GetChild(workspaceId);
        if (workspaceRef == null) return;

        // Register IObservableReader/Writer for each member type
        foreach (var type in Options.MemberTypes)
        {
            // Register reader/writer pointing to workspace's subdirectory
            services.RegisterObservablesInSubDirForType(type, ServiceProvider, workspaceRef);
        }
    }
}
```

**Registration**:
```csharp
// Register in root services
services.AddSingleton<IWorkspaceServiceConfigurator, WorkspaceTypesConfigurator>();
services.AddSingleton<IWorkspaceServiceConfigurator, MyCustomConfigurator>();

// When workspace loads, all configurators are invoked
var workspaceServices = new ServiceCollection();
foreach (var configurator in rootServices.GetServices<IWorkspaceServiceConfigurator>())
{
    await configurator.ConfigureWorkspaceServices(
        workspaceServices,
        userWorkspacesService,
        "workspace1"
    );
}
var workspaceProvider = workspaceServices.BuildServiceProvider();
```

**Why It Exists**:
- **Extensibility**: Applications can add custom workspace services
- **Decoupling**: Core workspace infrastructure doesn't know about specific document types
- **Composition**: Multiple configurators can contribute to workspace services

---

## Service Classes

### UserWorkspacesService

**Purpose**: Manages the base directory for user workspaces and provides workspace reference access.

```csharp
public class UserWorkspacesService
{
    #region Dependencies

    public IServiceProvider ServiceProvider { get; }

    #endregion

    #region Lifecycle

    public UserWorkspacesService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    #endregion

    // User-scoped services (optional)
    public IServiceProvider? UserServices { get; set; }

    #region Properties

    // Base directory for all workspaces
    public IReference? UserWorkspaces { get; set; }

    // Convenience accessor for filesystem path
    public string? UserWorkspacesDir
    {
        get => (UserWorkspaces as FileReference)?.Path;
        set => UserWorkspaces = new FileReference(value);
    }

    #endregion
}
```

**Typical Lifecycle**:
1. Registered as singleton in root DI container
2. UI layer (e.g., `WorkspaceLayoutVM`) sets `UserWorkspacesDir` based on current user
3. When workspace is opened, configurators use `UserWorkspaces.GetChild(workspaceId)` to get workspace directory
4. Workspace-scoped services registered pointing to that directory

**Usage Example**:
```csharp
// In UI layer, after user logs in
userWorkspacesService.UserWorkspacesDir = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
    "MyApp",
    "Users",
    username,
    "Workspaces"
);
// Result: C:\ProgramData\MyApp\Users\Alice\Workspaces

// Get specific workspace
var workspace1Ref = userWorkspacesService.UserWorkspaces.GetChild("workspace1");
// Result: C:\ProgramData\MyApp\Users\Alice\Workspaces\workspace1

// Get document subdirectory
var botsRef = workspace1Ref.GetChildSubpath("Bots");
// Result: C:\ProgramData\MyApp\Users\Alice\Workspaces\workspace1\Bots
```

**Key Properties**:
- `ServiceProvider`: Root application service provider
- `UserServices`: Optional user-scoped service provider (if application has user-level services)
- `UserWorkspaces`: Base directory reference (typically `FileReference`)
- `UserWorkspacesDir`: Convenience string property for filesystem path

**Notes**:
- **Lifetime**: Singleton in root container (but holds per-user state)
- **Configuration**: Set by UI layer based on logged-in user
- **Not Scoped Per Workspace**: This service provides the *base* directory; individual workspace service providers are created separately

---

## Architecture Role

### Layer Diagram

```
Application
    ↓ depends on
LionFire.Workspaces (Implementation)
    ↓ depends on
LionFire.Workspaces.Abstractions (This Library)
    ↑ depended on by
LionFire.Workspaces.UI (UI Layer)
```

**Why Abstractions Exist**:
- **Avoid Circular Dependencies**: UI can depend on abstractions without depending on implementation
- **Extensibility**: Applications can implement `IWorkspaceServiceConfigurator` without depending on core workspace infrastructure
- **Testing**: Can mock `IWorkspace` and `IWorkspaceServiceConfigurator` for tests

---

## Usage Patterns

### Pattern 1: Implementing a Custom Configurator

```csharp
// In your application
public class MyAppWorkspaceConfigurator : IWorkspaceServiceConfigurator
{
    public async ValueTask ConfigureWorkspaceServices(
        IServiceCollection services,
        UserWorkspacesService userWorkspacesService,
        string? workspaceId)
    {
        // Register application-specific workspace services
        services.AddSingleton<IMyAppService, MyAppService>();

        // Configure based on workspace
        var workspaceDir = userWorkspacesService.UserWorkspaces?.GetChild(workspaceId);
        services.AddSingleton(new MyAppConfig {
            WorkspaceDirectory = workspaceDir?.ToString()
        });
    }
}

// Register in root services
services.AddSingleton<IWorkspaceServiceConfigurator, MyAppWorkspaceConfigurator>();
```

### Pattern 2: Setting Up User Workspaces

```csharp
// In UI layout or startup code
public class AppStartup
{
    private readonly UserWorkspacesService userWorkspacesService;

    public async Task InitializeForUser(string username)
    {
        // Set base directory
        userWorkspacesService.UserWorkspacesDir = GetUserWorkspacesPath(username);

        // Optional: Ensure directory exists
        if (!Directory.Exists(userWorkspacesService.UserWorkspacesDir))
        {
            Directory.CreateDirectory(userWorkspacesService.UserWorkspacesDir);
        }

        // Optional: Load workspace metadata
        await InitializeWorkspaceSchemas(userWorkspacesService.UserWorkspaces);
    }

    private string GetUserWorkspacesPath(string username)
    {
        var commonAppData = Environment.GetFolderPath(
            Environment.SpecialFolder.CommonApplicationData
        );
        return Path.Combine(commonAppData, "MyApp", "Users", username, "Workspaces");
    }
}
```

### Pattern 3: Workspace Reference Navigation

```csharp
public class WorkspaceManager
{
    private readonly UserWorkspacesService userWorkspacesService;

    // Get all workspace directories
    public IEnumerable<string> GetWorkspaceIds()
    {
        var workspacesDir = userWorkspacesService.UserWorkspacesDir;
        if (workspacesDir == null || !Directory.Exists(workspacesDir))
            return Enumerable.Empty<string>();

        return Directory.GetDirectories(workspacesDir)
            .Select(Path.GetFileName);
    }

    // Get document directory for a workspace
    public string GetDocumentDirectory<TDocument>(string workspaceId)
    {
        var workspaceRef = userWorkspacesService.UserWorkspaces?.GetChild(workspaceId);
        var pluralName = GetPluralTypeName<TDocument>();
        var documentRef = workspaceRef?.GetChildSubpath(pluralName);
        return documentRef?.ToString() ?? throw new InvalidOperationException();
    }
}
```

---

## Dependencies

### NuGet Packages

```xml
<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" />
```

### LionFire Dependencies

```xml
<ProjectReference Include="..\LionFire.Referencing\" />
<ProjectReference Include="..\LionFire.Persistence.Filesystem\" />
```

---

## Related Documentation

- **[LionFire.Workspaces](../LionFire.Workspaces/CLAUDE.md)** - Implementation library
- **[Workspace Architecture](../../../docs/architecture/workspaces/README.md)** - Architecture overview
- **[Service Scoping](../../../docs/architecture/workspaces/service-scoping.md)** - Service scoping details
- **[LionFire.Workspaces.UI.Blazor](../LionFire.Workspaces.UI.Blazor/CLAUDE.md)** - Blazor UI components

---

## Summary

**LionFire.Workspaces.Abstractions** provides **minimal abstractions** for workspace infrastructure:

✅ **IWorkspace** - Marker interface for workspace instances
✅ **IWorkspaceServiceConfigurator** - Extensibility point for workspace service configuration
✅ **UserWorkspacesService** - Base directory management and workspace reference access

**Use This Library**: When you need to implement custom workspace configurators or depend on workspace types without pulling in full implementation.

**Key Benefit**: Enables extensibility and avoids circular dependencies in workspace ecosystem.
