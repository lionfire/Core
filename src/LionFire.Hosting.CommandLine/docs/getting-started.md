# Getting Started with LionFire.Hosting.CommandLine

## Overview

LionFire.Hosting.CommandLine is a library that bridges **System.CommandLine** with **Microsoft.Extensions.Hosting**, enabling you to build sophisticated command-line applications that leverage the full power of .NET's dependency injection and hosting infrastructure.

## Key Concept

The library introduces a **builder-of-builders** pattern that separates:
- **Command definition** (what commands exist, what options they have)
- **Host configuration** (what services to register, how to configure the app)
- **Execution context** (parsed command-line arguments, invocation details)

This separation allows for **command hierarchy inheritance**, where configuration from parent commands cascades to child commands, reducing duplication while maintaining flexibility.

## Installation

```xml
<PackageReference Include="System.CommandLine" />
<PackageReference Include="System.CommandLine.Hosting" />
<PackageReference Include="Microsoft.Extensions.Hosting" />
<!-- Project reference or package reference to LionFire.Hosting.CommandLine -->
```

## Quick Start

### Simple Single Command

```csharp
using LionFire.Hosting;
using LionFire.Hosting.CommandLine;
using Microsoft.Extensions.Hosting;

var program = new HostApplicationBuilderProgram()
    .RootCommand(builder =>
    {
        // Configure your host here
        builder.Services.AddSingleton<IMyService, MyService>();
    });

return await program.RunAsync(args);
```

### Command with Options

Define your options class:

```csharp
public class RunOptions
{
    public bool Verbose { get; set; }
    public string? OutputPath { get; set; }
}
```

Configure the command:

```csharp
var program = new HostApplicationBuilderProgram()
    .Command<RunOptions>("run", (context, builder) =>
    {
        var options = context.GetOptions<RunOptions>();

        // Configure based on options
        if (options.Verbose)
        {
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
        }

        builder.Services.AddSingleton(options);
        builder.Services.AddHostedService<MyBackgroundService>();
    },
    builderBuilder: bb =>
    {
        // Configure the System.CommandLine Command
        bb.Command.AddOption(new Option<bool>("--verbose", "Enable verbose output"));
        bb.Command.AddOption(new Option<string>("--output-path", "Output directory"));
    });

return await program.RunAsync(args);
```

### Multiple Commands

```csharp
var program = new HostApplicationBuilderProgram()
    .DefaultArgs("serve") // Default command if none specified

    // Command: serve
    .Command<ServeOptions>("serve", (context, builder) =>
    {
        builder.Services.AddHostedService<WebServerService>();
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Start the web server";
        bb.Command.AddOption(new Option<int>("--port", () => 8080, "Port to listen on"));
    })

    // Command: migrate
    .Command<MigrateOptions>("migrate", (context, builder) =>
    {
        builder.Services.AddHostedService<DatabaseMigrationService>();
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Run database migrations";
        bb.Command.AddOption(new Option<string>("--connection", "Database connection string"));
    });

return await program.RunAsync(args);
```

### Nested Commands (Command Hierarchy)

```csharp
var program = new HostApplicationBuilderProgram()
    // Shared configuration for all database commands
    .Command("database", (context, builder) =>
    {
        // Common DB configuration
        builder.Services.AddDbContext<AppDbContext>();
    })

    // database migrate
    .Command("database migrate", (context, builder) =>
    {
        builder.Services.AddHostedService<MigrationService>();
    })

    // database seed
    .Command("database seed", (context, builder) =>
    {
        builder.Services.AddHostedService<SeedService>();
    });
```

With hierarchy, `database migrate` inherits the configuration from `database`, so `AppDbContext` is automatically registered.

## Core Components

### 1. Program Classes

#### HostApplicationBuilderProgram
Uses `HostApplicationBuilder` (modern .NET hosting model)

```csharp
var program = new HostApplicationBuilderProgram()
    .Command<MyOptions>("mycommand", (context, builder) =>
    {
        // builder is HostApplicationBuilder
        builder.Services.AddSingleton<IService, ServiceImpl>();
    });
```

**When to use:** New applications targeting .NET 6+, simpler configuration

#### HostBuilderProgram
Uses `IHostBuilder` (traditional hosting model)

```csharp
var program = new HostBuilderProgram()
    .Command<MyOptions>("mycommand", (context, builder) =>
    {
        // builder is IHostBuilder
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IService, ServiceImpl>();
        });
    });
```

**When to use:** Compatibility with older code, need for `IHostBuilder`-specific features

#### Generic: CommandLineProgram<TBuilder, TBuilderBuilder>
Allows you to define your own builder type

```csharp
var program = new CommandLineProgram<MyCustomBuilder, MyBuilderBuilder>()
    .Command("cmd", (context, builder) =>
    {
        // builder is MyCustomBuilder
    });
```

**When to use:** Advanced scenarios, custom hosting models

### 2. Context Objects

#### HostingBuilderBuilderContext
Provides access to the execution context:

```csharp
.Command<MyOptions>("cmd", (context, builder) =>
{
    // Get strongly-typed options
    var options = context.GetOptions<MyOptions>();

    // Get raw options dictionary
    var rawOptions = context.Options;

    // Get invocation context (System.CommandLine)
    var invocationContext = context.InvocationContext;

    // Get command metadata
    var commandName = context.CommandName;
    var commandHierarchy = context.CommandHierarchy; // e.g., "database migrate"
})
```

### 3. Builder Configuration

The `builderBuilder` parameter lets you configure the underlying `IHostingBuilderBuilder`:

```csharp
.Command<MyOptions>("cmd",
    builder: (context, hab) => { /* configure host */ },
    builderBuilder: bb =>
    {
        // Configure System.CommandLine Command
        bb.Command.Description = "My command description";
        bb.Command.AddAlias("alias");

        // Configure options type binding
        bb.OptionsType = typeof(MyOptions);

        // Control inheritance
        bb.Inherit = false; // Don't inherit parent configurations
    })
```

## Advanced Features

### Command Hierarchy Inheritance

By default, child commands inherit configuration from parent commands:

```csharp
var program = new HostApplicationBuilderProgram()
    // Root: configure logging for all commands
    .RootCommand(builder =>
    {
        builder.Logging.AddConsole();
    })

    // Parent: configure database for all DB commands
    .Command("db", builder =>
    {
        builder.Services.AddDbContext<AppDbContext>();
    })

    // Child: inherits logging + database config
    .Command("db migrate", builder =>
    {
        builder.Services.AddHostedService<MigrationService>();
        // AppDbContext and console logging are already configured!
    });
```

To disable inheritance for a specific command:

```csharp
.Command("standalone", builder => { /* ... */ },
    builderBuilder: bb => bb.Inherit = false)
```

### Default Arguments

Set default arguments when none are provided:

```csharp
var program = new HostApplicationBuilderProgram()
    .DefaultArgs("serve --port 8080")
    .Command("serve", /* ... */);

// Running without args will execute: serve --port 8080
await program.RunAsync(args);
```

### Accessing Options in Services

Options are automatically registered in DI:

```csharp
public class MyBackgroundService : BackgroundService
{
    private readonly LionFireCommandLineOptions _cliOptions;
    private readonly MyOptions _options;

    public MyBackgroundService(
        LionFireCommandLineOptions cliOptions, // Raw options
        MyOptions options)                     // Strongly-typed options
    {
        _cliOptions = cliOptions;
        _options = options;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Access options here
        var verbose = _options.Verbose;
        return Task.CompletedTask;
    }
}
```

### Flexible Builder Type

For maximum flexibility, you can switch builder types per command:

```csharp
var program = new FlexibleProgram()
    .GetOrAdd<HostApplicationBuilderBuilder>("cmd1")
    .GetOrAdd<HostBuilderBuilder>("cmd2")
    .GetOrAdd<MyCustomBuilder>("cmd3");
```

This is rarely needed but allows mixing different hosting patterns in the same application.

## Best Practices

### 1. Organize Options Classes

```csharp
// Group related options together
public class ServeOptions
{
    public int Port { get; set; } = 8080;
    public string Host { get; set; } = "localhost";
    public bool Https { get; set; }
}

// Use data annotations for validation
public class DatabaseOptions
{
    [Required]
    public string ConnectionString { get; set; } = null!;

    [Range(1, 100)]
    public int MaxRetries { get; set; } = 3;
}
```

### 2. Separate Concerns

```csharp
var program = new HostApplicationBuilderProgram()
    .Command<MyOptions>("run",
        builder: ConfigureHost,        // Host configuration
        builderBuilder: ConfigureCommand, // Command configuration
        command: cmd => cmd.Description = "Run the application");

void ConfigureHost(HostingBuilderBuilderContext context, HostApplicationBuilder builder)
{
    var options = context.GetOptions<MyOptions>();
    // Configure services, logging, etc.
}

void ConfigureCommand(IHostingBuilderBuilder bb)
{
    // Configure System.CommandLine aspects
    bb.Command.AddOption(new Option<bool>("--verbose"));
}
```

### 3. Leverage Hierarchy for Common Config

```csharp
// Common configuration in parent
.Command("api", builder =>
{
    builder.Services.AddHttpClient();
    builder.Services.AddLogging();
})

// Specific commands inherit common setup
.Command("api list", builder => builder.Services.AddHostedService<ListService>())
.Command("api create", builder => builder.Services.AddHostedService<CreateService>())
```

### 4. Use Strongly-Typed Options

Prefer strongly-typed options classes over raw option dictionaries:

```csharp
// Good
.Command<RunOptions>("run", (context, builder) =>
{
    var options = context.GetOptions<RunOptions>();
    if (options.Verbose) { /* ... */ }
})

// Avoid
.Command("run", (context, builder) =>
{
    if ((bool)context.Options["verbose"]) { /* ... */ }
})
```

## Common Patterns

### Pattern 1: Multi-Environment Support

```csharp
.Command<AppOptions>("run", (context, builder) =>
{
    var options = context.GetOptions<AppOptions>();

    builder.Environment.EnvironmentName = options.Environment switch
    {
        "dev" => "Development",
        "staging" => "Staging",
        "prod" => "Production",
        _ => builder.Environment.EnvironmentName
    };
})
```

### Pattern 2: Conditional Service Registration

```csharp
.Command<FeatureOptions>("run", (context, builder) =>
{
    var options = context.GetOptions<FeatureOptions>();

    if (options.EnableCache)
    {
        builder.Services.AddStackExchangeRedisCache(/* ... */);
    }

    if (options.EnableMetrics)
    {
        builder.Services.AddOpenTelemetry();
    }
})
```

### Pattern 3: Command-Specific Configuration Sources

```csharp
.Command("run", (context, builder) =>
{
    builder.Configuration.AddJsonFile("appsettings.run.json", optional: true);
})

.Command("migrate", (context, builder) =>
{
    builder.Configuration.AddJsonFile("appsettings.migrate.json", optional: true);
})
```

## Next Steps

- Review [Architecture](./architecture.md) to understand the internal design
- Explore [Advanced Scenarios](./advanced-scenarios.md) for complex use cases
- Read [Comparison with Other Libraries](./analysis/comparisons/) to understand tradeoffs
- Check out the [Sample Project](../Sample/) for working examples
