# LionFire.Hosting.CommandLine

A command-line framework that bridges **System.CommandLine** with **Microsoft.Extensions.Hosting**, enabling hierarchical command structures with configuration inheritance for .NET applications.

## What Makes This Library Different?

Most command-line libraries focus on parsing arguments. **LionFire.Hosting.CommandLine** focuses on **command hierarchy and configuration inheritance**.

### The Core Value Proposition

```csharp
// Parent command provides infrastructure
.Command("database", builder =>
{
    builder.Services.AddDbContext<AppDbContext>();
    builder.Logging.AddConsole();
})

// Child commands inherit automatically - no duplication!
.Command("database migrate", builder =>
{
    builder.Services.AddHostedService<MigrationService>();
    // AppDbContext and logging already configured ✓
})

.Command("database seed", builder =>
{
    builder.Services.AddHostedService<SeedService>();
    // AppDbContext and logging already configured ✓
})
```

Without inheritance, you'd repeat the same setup in each command. With LionFire, **configure once, inherit everywhere**.

## When Should You Use This Library?

### ✓ Perfect For

- **Hierarchical command structures** (like `git`, `kubectl`, `docker`)
- **Commands sharing infrastructure** (database, HTTP clients, logging)
- **Applications using Microsoft.Extensions.Hosting**
- **Long-running or background services**
- **Complex dependency injection scenarios**

### ✗ Not Ideal For

- Simple CLI utilities with 1-3 commands
- Performance-critical commands (startup ~150ms overhead)
- Minimal dependency requirements
- Quick scripts (<50 lines)

**Rule of thumb:** If your commands naturally group into families that share configuration, this library will save you significant boilerplate.

## Quick Start

### Installation

```bash
dotnet add package System.CommandLine
dotnet add package System.CommandLine.Hosting
dotnet add package Microsoft.Extensions.Hosting
```

Reference LionFire.Hosting.CommandLine in your project.

### Hello World

```csharp
using LionFire.Hosting.CommandLine;
using Microsoft.Extensions.Hosting;

var program = new HostApplicationBuilderProgram()
    .RootCommand(builder =>
    {
        builder.Services.AddHostedService<HelloService>();
    });

return await program.RunAsync(args);

public class HelloService : IHostedService
{
    public Task StartAsync(CancellationToken ct)
    {
        Console.WriteLine("Hello from hosted service!");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
```

### Multi-Command Example

```csharp
public class ServeOptions
{
    public int Port { get; set; } = 8080;
}

public class MigrateOptions
{
    public string ConnectionString { get; set; } = "";
}

var program = new HostApplicationBuilderProgram()
    // Shared configuration for all commands
    .RootCommand(builder =>
    {
        builder.Logging.AddConsole();
    })

    // Command: serve
    .Command<ServeOptions>("serve", (context, builder) =>
    {
        var options = context.GetOptions<ServeOptions>();
        builder.Services.AddSingleton(options);
        builder.Services.AddHostedService<WebServerService>();
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Start the web server";
        bb.Command.AddOption(new Option<int>("--port", () => 8080));
    })

    // Command: migrate
    .Command<MigrateOptions>("migrate", (context, builder) =>
    {
        var options = context.GetOptions<MigrateOptions>();
        builder.Services.AddSingleton(options);
        builder.Services.AddHostedService<MigrationService>();
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Run database migrations";
        bb.Command.AddOption(new Option<string>("--connection-string"));
    });

return await program.RunAsync(args);
```

## Key Features

### 1. Configuration Inheritance

Parent commands configure infrastructure, children extend it:

```csharp
var program = new HostApplicationBuilderProgram()
    // Parent: all API commands share this
    .Command("api", builder =>
    {
        builder.Services.AddHttpClient();
        builder.Configuration.AddJsonFile("api-settings.json");
    })

    // Children automatically get HttpClient and configuration
    .Command("api list", builder => builder.Services.AddHostedService<ListService>())
    .Command("api create", builder => builder.Services.AddHostedService<CreateService>())
    .Command("api delete", builder => builder.Services.AddHostedService<DeleteService>());
```

### 2. Strongly-Typed Options

Define options as classes, automatically bound and injected:

```csharp
public class RunOptions
{
    public bool Verbose { get; set; }
    public string Environment { get; set; } = "Production";
}

.Command<RunOptions>("run", (context, builder) =>
{
    var options = context.GetOptions<RunOptions>();

    // Use options to configure host
    if (options.Verbose)
        builder.Logging.SetMinimumLevel(LogLevel.Debug);

    builder.Environment.EnvironmentName = options.Environment;

    // Options automatically registered in DI
    builder.Services.AddHostedService<AppService>();
})

// In AppService:
public class AppService : BackgroundService
{
    private readonly RunOptions _options;

    public AppService(RunOptions options) // Injected!
    {
        _options = options;
    }
}
```

### 3. Dynamic Service Registration

Register services conditionally based on command-line arguments:

```csharp
public class FeatureOptions
{
    public bool EnableCache { get; set; }
    public bool EnableMetrics { get; set; }
}

.Command<FeatureOptions>("run", (context, builder) =>
{
    var options = context.GetOptions<FeatureOptions>();

    if (options.EnableCache)
        builder.Services.AddStackExchangeRedisCache(/* ... */);

    if (options.EnableMetrics)
        builder.Services.AddOpenTelemetry();
})
```

### 4. Multiple Builder Types

Support for different hosting patterns:

```csharp
// Modern: HostApplicationBuilder (.NET 6+)
var program1 = new HostApplicationBuilderProgram();

// Traditional: IHostBuilder
var program2 = new HostBuilderProgram();

// Custom: Your own builder
var program3 = new CommandLineProgram<MyBuilder, MyBuilderBuilder>();
```

### 5. Default Arguments

Set defaults when no arguments provided:

```csharp
var program = new HostApplicationBuilderProgram()
    .DefaultArgs("serve --port 8080")
    .Command("serve", /* ... */);

// Running without args executes: serve --port 8080
await program.RunAsync(args);
```

## Architecture

### The Builder-of-Builders Pattern

```
Your Code → Program → BuilderBuilder → Builder → Host → Your Services
```

1. **Program** - Manages command structure and registry
2. **BuilderBuilder** - Stores configuration for a specific command
3. **Builder** - The hosting builder (e.g., `HostApplicationBuilder`)
4. **Host** - The running application host

This separation allows:
- Configuration before the builder is created
- Hierarchy traversal and inheritance
- Dynamic configuration based on parsed arguments

### Execution Flow

```
1. Parse command line
   ↓
2. Resolve command hierarchy ("database migrate" → ["", "database", "database migrate"])
   ↓
3. Get BuilderBuilders for each level
   ↓
4. Create Builder instance
   ↓
5. Execute initializers from ancestors → descendants
   ↓
6. Build and run Host
```

## Documentation

### Getting Started
- [Getting Started Guide](docs/getting-started.md) - Comprehensive tutorial
- [Architecture](docs/architecture.md) - Internal design and patterns

### Comparisons
Understand when to use this library vs. alternatives:
- [vs. System.CommandLine](docs/analysis/comparisons/system-commandline.md) - The foundation we build on
- [vs. CommandLineParser](docs/analysis/comparisons/commandlineparser.md) - Attribute-based alternative
- [vs. ConsoleAppFramework](docs/analysis/comparisons/consoleappframework.md) - High-performance source-gen approach
- [vs. Manual Parsing](docs/analysis/comparisons/manual-parsing.md) - When to just parse yourself
- [vs. CliFx](docs/analysis/comparisons/clifx.md) - Class-first framework with IConsole testability
- [vs. Cocona](docs/analysis/comparisons/cocona.md) - Method-based minimal API style
- [vs. Spectre.Console.Cli](docs/analysis/comparisons/spectre-console-cli.md) - Rich terminal UI integration

### Analysis
- [Tradeoffs Analysis](docs/analysis/tradeoffs.md) - Detailed tradeoff discussion

## Examples

See [Sample Project](Sample/) for working examples.

## Common Patterns

### Environment-Specific Configuration

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

    // Load environment-specific config
    builder.Configuration.AddJsonFile(
        $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true);
})
```

### Conditional Features

```csharp
.Command<Options>("run", (context, builder) =>
{
    var options = context.GetOptions<Options>();

    // Base services always added
    builder.Services.AddSingleton<ICoreService, CoreService>();

    // Optional features
    if (options.EnableCache)
        builder.Services.AddStackExchangeRedisCache(/* ... */);

    if (options.EnableAuth)
        builder.Services.AddAuthentication(/* ... */);

    if (options.EnableMetrics)
        builder.Services.AddOpenTelemetry();
})
```

### Hierarchy with Shared Services

```csharp
var program = new HostApplicationBuilderProgram()
    // All commands get logging
    .RootCommand(builder =>
    {
        builder.Logging.AddConsole();
    })

    // All database commands get DbContext
    .Command("database", builder =>
    {
        builder.Services.AddDbContext<AppDbContext>();
    })

    // Specific commands add their services
    .Command("database migrate", builder =>
    {
        builder.Services.AddHostedService<MigrationService>();
    })

    .Command("database seed", builder =>
    {
        builder.Services.AddHostedService<SeedService>();
    })

    .Command("database backup", builder =>
    {
        builder.Services.AddHostedService<BackupService>();
    });
```

### Opt-Out of Inheritance

```csharp
.Command("isolated", builder =>
{
    // This command starts fresh - no parent configuration
    builder.Services.AddSingleton<IIsolatedService, IsolatedService>();
},
builderBuilder: bb =>
{
    bb.Inherit = false; // Don't inherit from ancestors
})
```

## Best Practices

### 1. Use Strongly-Typed Options

```csharp
// Good
.Command<MyOptions>("cmd", (context, builder) =>
{
    var options = context.GetOptions<MyOptions>();
})

// Avoid
.Command("cmd", (context, builder) =>
{
    var verbose = (bool)context.Options["verbose"];
})
```

### 2. Organize by Command Hierarchy

```csharp
// Good - natural grouping
.Command("database", /* shared DB config */)
.Command("database migrate", /* migration */)
.Command("database seed", /* seeding */)

// Avoid - flat structure with duplication
.Command("db-migrate", /* config DB + migration */)
.Command("db-seed", /* config DB + seeding */)
```

### 3. Put Common Config in Parents

```csharp
// Good
.Command("api", b => b.AddHttpClient())  // Once
.Command("api list", /* use HttpClient */)
.Command("api create", /* use HttpClient */)

// Avoid
.Command("api list", b => b.AddHttpClient())  // Repeated
.Command("api create", b => b.AddHttpClient())  // Repeated
```

### 4. Separate Command and Host Configuration

```csharp
.Command("run",
    builder: ConfigureHost,        // Host services
    builderBuilder: ConfigureCommand,  // Command options
    command: cmd => cmd.Description = "Run the app");

void ConfigureHost(HostingBuilderBuilderContext context, HostApplicationBuilder builder)
{
    builder.Services.AddHostedService<AppService>();
}

void ConfigureCommand(IHostingBuilderBuilder bb)
{
    bb.Command.AddOption(new Option<bool>("--verbose"));
}
```

## Performance Characteristics

### Startup Time

```
- Command-line parsing: ~2ms
- Hierarchy resolution: ~5ms
- Hosting infrastructure: ~100ms
- DI container creation: ~20ms
- Service activation: ~20ms
Total: ~150ms
```

**Acceptable for:** Long-running applications, interactive tools, development utilities

**Not acceptable for:** Commands <1s execution, tight loops, performance-critical utilities

### Memory Footprint

~25MB baseline (hosting + DI + configuration + logging)

### Binary Size

~3-5MB (including all dependencies)

## Requirements

- .NET 6.0 or later
- System.CommandLine (beta)
- Microsoft.Extensions.Hosting

## Contributing

This library is part of the LionFire.Core monorepo. See the main repository for contribution guidelines.

## License

MIT License - see LICENSE file for details.

## Credits

Built on top of:
- [System.CommandLine](https://github.com/dotnet/command-line-api) - Command-line parsing
- [Microsoft.Extensions.Hosting](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host) - Application hosting

## Frequently Asked Questions

### Q: Why not just use System.CommandLine directly?

**A:** System.CommandLine.Hosting only lets you configure the host *before* parsing arguments. LionFire lets you configure *after* parsing, enabling dynamic service registration based on command-line options. Plus, we add configuration inheritance.

### Q: Is the startup overhead a problem?

**A:** For long-running applications (>10 seconds), the ~150ms startup is negligible. For short-lived commands (<1 second), it's significant. Choose accordingly.

### Q: Can I mix this with other command-line libraries?

**A:** Yes, but it's not recommended. Different paradigms and hosting setups would be confusing. Choose one approach for your application.

### Q: Do I need to understand System.CommandLine?

**A:** Basic understanding helps, especially for option/argument configuration. The LionFire layer handles hosting integration.

### Q: What about help generation?

**A:** Help is generated by System.CommandLine from your `Command` objects. You provide descriptions and help text when configuring commands.

### Q: Can I use this without hosting?

**A:** No, the library is specifically designed for hosted applications. If you don't need hosting, use System.CommandLine directly.

### Q: Is this production-ready?

**A:** The library is stable and tested, but System.CommandLine is still in beta. Consider this in your evaluation.

### Q: How do I handle errors?

**A:** Use standard .NET exception handling. Unhandled exceptions during initialization or execution will be caught and return exit code 1.

### Q: Can I use async configuration?

**A:** Initializers are synchronous. For async setup, use `IHostedService.StartAsync()` in your services.

## Related Projects

- [System.CommandLine](https://github.com/dotnet/command-line-api) - The parsing foundation
- [ConsoleAppFramework](https://github.com/Cysharp/ConsoleAppFramework) - High-performance source-gen alternative
- [CommandLineParser](https://github.com/commandlineparser/commandline) - Attribute-based alternative
- [CliFx](https://github.com/Tyrrrz/CliFx) - Class-first framework with excellent testability
- [Cocona](https://github.com/mayuki/Cocona) - Method-based minimal API style framework
- [Spectre.Console.Cli](https://spectreconsole.net/cli/) - Rich terminal UI with CLI parsing
- [Spectre.Console](https://github.com/spectreconsole/spectre.console) - Beautiful terminal UI library
