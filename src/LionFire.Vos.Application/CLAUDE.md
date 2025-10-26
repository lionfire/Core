# LionFire.Vos.VosApp (Vos.Application)

## Overview

**LionFire.Vos.VosApp** provides an opinionated application framework built on VOS (Virtual Object System). It offers default configurations, best practices, and common patterns for building VOS-based applications, including automatic configuration, asset management, and persistence integration.

**Layer**: Framework (Opinionated VOS Applications)
**Target**: .NET 9.0
**Root Namespace**: `LionFire.Vos`
**Project Name**: `LionFire.Vos.VosApp.csproj` (in Vos.Application directory)

## Key Dependencies

### LionFire Dependencies
- **LionFire.Applications** - Application framework
- **LionFire.Assets** - Asset management
- **LionFire.Core.Extras** - Core framework extras
- **LionFire.Data.Async.Abstractions** - Async data patterns
- **LionFire.Data.Async.Reactive** - ReactiveUI integration
- **LionFire.Persistence** - Persistence framework
- **LionFire.Persistence.Filesystem** - Filesystem backend
- **LionFire.Resolves** (Data.Async) - Data resolution
- **LionFire.Vos** - Virtual Object System core
- **LionFire.Vos.Packages** - VOS packages

## Core Features

### 1. VOS Application Template

Pre-configured VOS setup with sensible defaults:

```csharp
public class MyApp : VosApp
{
    protected override async Task OnInitialize()
    {
        // VOS already configured with:
        // - Filesystem mounts at /config, /data, /assets
        // - Serialization for .json, .hjson, .yaml
        // - Default overlays for configuration
        // - Asset management

        await base.OnInitialize();

        // Add custom initialization
        await ConfigureCustomMounts();
    }
}
```

### 2. Default Mount Structure

VosApp automatically mounts:

```
vos://
├── /config         → Filesystem: {AppDataPath}/config
│   ├── /defaults   → (Overlay layer: app defaults)
│   └── /user       → (Overlay layer: user overrides)
├── /data           → Filesystem: {AppDataPath}/data
├── /assets         → Filesystem: {AppDataPath}/assets
└── /cache          → Memory mount
```

### 3. Configuration Management

Automatic configuration with overlay support:

```csharp
public class MyApp : VosApp
{
    public AppConfig Config { get; private set; }

    protected override async Task OnInitialize()
    {
        await base.OnInitialize();

        // Load config with user overrides
        var configHandle = Vos.Get<AppConfig>("/config/app.hjson");
        Config = await configHandle.GetOrDefault(() => new AppConfig());

        // Watch for changes
        configHandle.GetResults.Subscribe(result =>
        {
            if (result.HasValue)
            {
                OnConfigChanged(result.Value);
            }
        });
    }

    private void OnConfigChanged(AppConfig newConfig)
    {
        Config = newConfig;
        // Apply configuration changes
    }
}
```

### 4. Asset Management Integration

Built-in asset management:

```csharp
public class MyApp : VosApp
{
    protected override async Task OnInitialize()
    {
        await base.OnInitialize();

        // Assets automatically available
        var logo = await Vos.Get<ImageAsset>("/assets/logo.png").Get();
        var theme = await Vos.Get<ThemeAsset>("/assets/themes/dark.json").Get();
    }
}
```

### 5. Package System

VOS packages for modular features:

```csharp
public class MyApp : VosApp
{
    protected override void ConfigurePackages()
    {
        base.ConfigurePackages();

        // Install VOS packages
        Packages.Install(new LoggingPackage());
        Packages.Install(new AuthenticationPackage());
        Packages.Install(new DatabasePackage());
    }
}
```

## Application Lifecycle

### Startup Sequence

```csharp
// 1. Create application
var app = new MyApp();

// 2. Configure services (DI)
app.ConfigureServices(services =>
{
    services.AddSingleton<IMyService, MyService>();
});

// 3. Initialize (sets up VOS)
await app.InitializeAsync();

// 4. Start
await app.StartAsync();

// 5. Run
await app.RunAsync();

// 6. Stop
await app.StopAsync();
```

### Lifecycle Hooks

```csharp
public class MyApp : VosApp
{
    protected override async Task OnInitialize()
    {
        await base.OnInitialize();
        // VOS configured, mount additional data sources
    }

    protected override async Task OnStart()
    {
        await base.OnStart();
        // Application starting, connect to services
    }

    protected override async Task OnRun()
    {
        await base.OnRun();
        // Main application loop
    }

    protected override async Task OnStop()
    {
        await base.OnStop();
        // Cleanup before exit
    }
}
```

## Common Patterns

### Pattern 1: Configuration with Overlays

```csharp
public class MyApp : VosApp
{
    protected override async Task ConfigureMounts()
    {
        await base.ConfigureMounts();

        // Mount configuration overlays
        Vos.MountOverlay("/config")
            .AddLayer(new FileSystemMount(GetUserConfigPath()))      // User settings (top)
            .AddLayer(new FileSystemMount(GetDefaultConfigPath()))   // App defaults
            .AddLayer(new ResourceMount("embedded://defaults"));     // Embedded defaults
    }

    protected override async Task OnInitialize()
    {
        await base.OnInitialize();

        // Load config - uses overlay hierarchy
        var config = await Vos.Get<AppConfig>("/config/app.hjson").Get();
        // 1. Try user config
        // 2. Fall back to app defaults
        // 3. Fall back to embedded defaults
    }
}
```

### Pattern 2: Per-User Data

```csharp
public class MyApp : VosApp
{
    private string currentUser;

    public void SetUser(string userId)
    {
        currentUser = userId;

        // Mount user-specific data
        var userDataPath = Path.Combine(GetDataPath(), "users", userId);
        Vos.Mount($"/users/{userId}", new FileSystemMount(userDataPath));
    }

    public async Task<UserPreferences> GetUserPreferences()
    {
        var path = $"/users/{currentUser}/preferences.hjson";
        return await Vos.Get<UserPreferences>(path).GetOrDefault(() => new UserPreferences());
    }
}
```

### Pattern 3: Plugin System

```csharp
public class MyApp : VosApp
{
    protected override async Task LoadPlugins()
    {
        var pluginDir = GetPluginDirectory();

        foreach (var pluginPath in Directory.GetDirectories(pluginDir))
        {
            var pluginName = Path.GetFileName(pluginPath);

            // Mount plugin at /plugins/{name}
            Vos.Mount($"/plugins/{pluginName}", new FileSystemMount(pluginPath));

            // Load plugin manifest
            var manifest = await Vos.Get<PluginManifest>($"/plugins/{pluginName}/manifest.json").Get();

            // Initialize plugin
            await InitializePlugin(manifest);
        }
    }
}
```

### Pattern 4: Development vs Production

```csharp
public class MyApp : VosApp
{
    protected override async Task ConfigureMounts()
    {
        await base.ConfigureMounts();

        if (Environment.IsDevelopment())
        {
            // Development: use local files
            Vos.Mount("/data", new FileSystemMount("./dev-data"));
        }
        else
        {
            // Production: use database
            Vos.Mount("/data", new DatabaseMount(Configuration.GetConnectionString("Main")));
        }
    }
}
```

## Integration with Other Libraries

### With Blazor

```csharp
public class BlazorVosApp : VosApp
{
    public void ConfigureBlazorServices(IServiceCollection services)
    {
        services.AddSingleton<IVos>(Vos);
        services.AddScoped<VosDataService>();
    }
}

// In Blazor component
@inject IVos Vos

@code {
    private AppConfig config;

    protected override async Task OnInitializedAsync()
    {
        config = await Vos.Get<AppConfig>("/config/app.hjson").Get();
    }
}
```

### With ASP.NET Core

```csharp
public class WebVosApp : VosApp
{
    public void ConfigureWebApp(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IVos>(Vos);

        // Use VOS for configuration
        var config = Vos.Get<AppConfig>("/config/app.hjson").Get().Result;
        builder.Configuration.AddInMemoryCollection(config.ToDictionary());
    }
}
```

## Dependency Injection

VosApp integrates with Microsoft.Extensions.DependencyInjection:

```csharp
public class MyApp : VosApp
{
    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        // Register services
        services.AddSingleton<IMyService, MyService>();
        services.AddScoped<IUserService, UserService>();

        // VOS already registered
        // services.AddSingleton<IVos>(Vos);
    }
}
```

## Testing VosApp

```csharp
public class MyAppTests
{
    [Fact]
    public async Task App_Initializes_Successfully()
    {
        var app = new MyApp();

        await app.InitializeAsync();

        Assert.NotNull(app.Vos);
        Assert.True(app.IsInitialized);
    }

    [Fact]
    public async Task App_Loads_Configuration()
    {
        var app = new MyApp();
        await app.InitializeAsync();

        var config = await app.Vos.Get<AppConfig>("/config/app.hjson").Get();

        Assert.NotNull(config);
    }
}
```

## Related Projects

- **LionFire.Vos** - Virtual Object System core ([CLAUDE.md](../LionFire.Vos/CLAUDE.md))
- **LionFire.Applications** - Application framework
- **LionFire.Assets** - Asset management
- **LionFire.Persistence** - Persistence framework

## Summary

**LionFire.Vos.VosApp** provides an opinionated VOS application framework:

- **Pre-configured VOS**: Sensible defaults out of the box
- **Default Mounts**: /config, /data, /assets, /cache
- **Configuration Overlays**: User settings override app defaults
- **Asset Management**: Built-in asset loading
- **Package System**: Modular feature installation
- **Lifecycle Management**: Initialize, Start, Run, Stop hooks
- **DI Integration**: Full Microsoft.Extensions.DependencyInjection support

**Key Strengths:**
- Quick VOS application setup
- Best practice defaults
- Configuration management
- Extensible via packages

**Use When:**
- Building VOS-based applications
- Need configuration with overlays
- Want opinionated framework
- Require asset management

**Typical Use Cases:**
- Desktop applications with VOS
- Configuration-heavy apps
- Plugin-based systems
- Multi-user applications
- Development tools

**Note**: This is a framework layer - it's opinionated and provides default behaviors. For more control, use LionFire.Vos directly.
