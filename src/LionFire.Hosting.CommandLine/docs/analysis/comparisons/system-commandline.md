# Comparison: LionFire.Hosting.CommandLine vs. System.CommandLine

## Overview

**System.CommandLine** is the foundation library that LionFire.Hosting.CommandLine builds upon. This comparison focuses on what LionFire adds on top of the base System.CommandLine functionality.

## Quick Comparison Table

| Feature | System.CommandLine | LionFire.Hosting.CommandLine |
|---------|-------------------|----------------------------|
| Command Definition | ✓ Manual API | ✓ Fluent API with builders |
| Option Parsing | ✓ Built-in | ✓ Inherited |
| Help Generation | ✓ Automatic | ✓ Inherited |
| Tab Completion | ✓ Built-in | ✓ Inherited |
| DI Integration | ⚠️ Via System.CommandLine.Hosting | ✓ First-class, flexible |
| Hosting Integration | ⚠️ Basic (post-build only) | ✓ Advanced (pre-build config) |
| Command Hierarchy | ✓ Manual nesting | ✓ String-based + inheritance |
| Options Binding | ✓ Manual or ModelBinder | ✓ Automatic to typed classes |
| Builder Type Support | ⚠️ IHostBuilder only | ✓ Multiple (IHostBuilder, HostApplicationBuilder, custom) |
| Configuration Inheritance | ✗ Not available | ✓ Core feature |
| Learning Curve | Low | Medium |
| Verbosity | Medium | Low (for hosting scenarios) |

## Detailed Comparison

### 1. Basic Command Definition

#### System.CommandLine

```csharp
var rootCommand = new RootCommand("My CLI application");

var nameOption = new Option<string>(
    name: "--name",
    description: "The name to greet");

var verboseOption = new Option<bool>(
    name: "--verbose",
    description: "Enable verbose output");

rootCommand.AddOption(nameOption);
rootCommand.AddOption(verboseOption);

rootCommand.SetHandler((string name, bool verbose) =>
{
    if (verbose)
        Console.WriteLine($"Verbose: Processing name: {name}");
    Console.WriteLine($"Hello, {name}!");
}, nameOption, verboseOption);

return await rootCommand.InvokeAsync(args);
```

**Pros:**
- Explicit and clear
- No additional abstractions
- Direct control over everything

**Cons:**
- Verbose for hosting scenarios
- No built-in DI integration
- Manual service setup

#### LionFire.Hosting.CommandLine

```csharp
public class GreetOptions
{
    public string Name { get; set; } = "World";
    public bool Verbose { get; set; }
}

var program = new HostApplicationBuilderProgram()
    .RootCommand<GreetOptions>((context, builder) =>
    {
        var options = context.GetOptions<GreetOptions>();

        if (options.Verbose)
        {
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
        }

        builder.Services.AddSingleton(options);
        builder.Services.AddHostedService<GreetService>();
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "My CLI application";
        bb.Command.AddOption(new Option<string>("--name"));
        bb.Command.AddOption(new Option<bool>("--verbose"));
    });

return await program.RunAsync(args);
```

**Pros:**
- Strongly-typed options
- Built-in DI and hosting
- Structured for long-running services

**Cons:**
- More boilerplate for simple commands
- Additional abstraction layer
- Requires understanding of hosting model

**Verdict:** For simple CLI utilities, System.CommandLine is cleaner. For applications needing DI, hosting, or multiple services, LionFire provides better structure.

### 2. Dependency Injection Integration

#### System.CommandLine (with System.CommandLine.Hosting)

```csharp
var rootCommand = new RootCommand();

rootCommand.SetHandler(async (IHost host) =>
{
    // Host is already built - can't configure based on command-line args
    var service = host.Services.GetRequiredService<IMyService>();
    await service.DoWorkAsync();
    await host.RunAsync();
});

return await new CommandLineBuilder(rootCommand)
    .UseHost(args => Host.CreateDefaultBuilder(args),
        builder => builder.ConfigureServices(services =>
        {
            // Configuration happens BEFORE parsing
            // Can't access command-line arguments here
            services.AddSingleton<IMyService, MyService>();
        }))
    .Build()
    .InvokeAsync(args);
```

**Limitations:**
- Host is configured before command-line parsing
- Can't conditionally register services based on arguments
- Limited to `IHostBuilder`

#### LionFire.Hosting.CommandLine

```csharp
public class RunOptions
{
    public bool EnableCache { get; set; }
    public bool EnableMetrics { get; set; }
}

var program = new HostApplicationBuilderProgram()
    .Command<RunOptions>("run", (context, builder) =>
    {
        var options = context.GetOptions<RunOptions>();

        // Conditionally register services based on arguments
        if (options.EnableCache)
        {
            builder.Services.AddStackExchangeRedisCache(/* ... */);
        }

        if (options.EnableMetrics)
        {
            builder.Services.AddOpenTelemetry();
        }

        builder.Services.AddHostedService<AppService>();
    });
```

**Benefits:**
- Configure host *after* parsing arguments
- Conditional service registration
- Access parsed options during configuration
- Supports `HostApplicationBuilder`, `IHostBuilder`, and custom builders

**Verdict:** LionFire provides much more flexible DI integration, especially for dynamic configuration based on command-line arguments.

### 3. Multiple Commands

#### System.CommandLine

```csharp
var rootCommand = new RootCommand();

var serveCommand = new Command("serve", "Start the server");
var portOption = new Option<int>("--port", () => 8080);
serveCommand.AddOption(portOption);
serveCommand.SetHandler((int port) =>
{
    Console.WriteLine($"Starting server on port {port}");
    // Manual service setup
}, portOption);

var migrateCommand = new Command("migrate", "Run migrations");
var connectionOption = new Option<string>("--connection");
migrateCommand.AddOption(connectionOption);
migrateCommand.SetHandler((string connection) =>
{
    Console.WriteLine($"Running migrations on {connection}");
    // Manual service setup
}, connectionOption);

rootCommand.AddCommand(serveCommand);
rootCommand.AddCommand(migrateCommand);

return await rootCommand.InvokeAsync(args);
```

**Challenges:**
- Repetitive service setup for each command
- No shared configuration between commands
- Manual lifetime management

#### LionFire.Hosting.CommandLine

```csharp
var program = new HostApplicationBuilderProgram()
    // Common configuration for all commands
    .RootCommand(builder =>
    {
        builder.Logging.AddConsole();
        builder.Configuration.AddJsonFile("appsettings.json");
    })

    .Command<ServeOptions>("serve", (context, builder) =>
    {
        var options = context.GetOptions<ServeOptions>();
        builder.Services.AddHostedService<ServerService>();
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Start the server";
        bb.Command.AddOption(new Option<int>("--port", () => 8080));
    })

    .Command<MigrateOptions>("migrate", (context, builder) =>
    {
        var options = context.GetOptions<MigrateOptions>();
        builder.Services.AddHostedService<MigrationService>();
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Run migrations";
        bb.Command.AddOption(new Option<string>("--connection"));
    });

return await program.RunAsync(args);
```

**Benefits:**
- Shared configuration via RootCommand
- Each command gets its own configured host
- Automatic lifetime management
- Strongly-typed options per command

**Verdict:** LionFire significantly reduces boilerplate for multi-command applications with shared infrastructure.

### 4. Nested Commands & Hierarchy

#### System.CommandLine

```csharp
var rootCommand = new RootCommand();

var dbCommand = new Command("database", "Database operations");

var migrateCommand = new Command("migrate", "Run migrations");
migrateCommand.SetHandler(() => Console.WriteLine("Migrating..."));

var seedCommand = new Command("seed", "Seed data");
seedCommand.SetHandler(() => Console.WriteLine("Seeding..."));

dbCommand.AddCommand(migrateCommand);
dbCommand.AddCommand(seedCommand);
rootCommand.AddCommand(dbCommand);

return await rootCommand.InvokeAsync(args);
```

**No configuration inheritance** - each command is independent.

#### LionFire.Hosting.CommandLine

```csharp
var program = new HostApplicationBuilderProgram()
    // Parent command: shared database configuration
    .Command("database", builder =>
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

        builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
    })

    // Child command 1: inherits DbContext + logging config
    .Command("database migrate", builder =>
    {
        builder.Services.AddHostedService<MigrationService>();
    })

    // Child command 2: also inherits parent configuration
    .Command("database seed", builder =>
    {
        builder.Services.AddHostedService<SeedService>();
    });

return await program.RunAsync(args);
```

**Configuration cascade:**
- `database migrate` gets: DbContext + EF logging + MigrationService
- `database seed` gets: DbContext + EF logging + SeedService

**Verdict:** LionFire's inheritance eliminates duplication in command hierarchies. System.CommandLine requires manual configuration sharing.

### 5. Options Binding

#### System.CommandLine

Manual binding using handler parameters:

```csharp
var nameOption = new Option<string>("--name");
var ageOption = new Option<int>("--age");

command.SetHandler((string name, int age) =>
{
    // Use name and age
}, nameOption, ageOption);
```

Or using `ModelBinder`:

```csharp
public class PersonOptions
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
}

var binder = new ModelBinder<PersonOptions>();
command.SetHandler((PersonOptions options) =>
{
    // Use options
}, binder);
```

**Pros:**
- Simple and direct
- Type-safe

**Cons:**
- Options not available in DI container
- Manual binding required
- No automatic service registration

#### LionFire.Hosting.CommandLine

Automatic binding and DI registration:

```csharp
public class PersonOptions
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
}

var program = new HostApplicationBuilderProgram()
    .Command<PersonOptions>("greet", (context, builder) =>
    {
        // Options automatically bound
        var options = context.GetOptions<PersonOptions>();

        // Options automatically registered in DI
        builder.Services.AddHostedService<GreetService>();
    });

// In GreetService:
public class GreetService : IHostedService
{
    private readonly PersonOptions _options;

    public GreetService(PersonOptions options) // Injected!
    {
        _options = options;
    }
}
```

**Pros:**
- Automatic binding to typed classes
- Automatic DI registration
- Options available throughout service graph
- Consistent pattern across all commands

**Cons:**
- Slightly more ceremony for simple cases

**Verdict:** For applications using DI, LionFire's automatic binding is a significant productivity boost.

### 6. Builder Type Flexibility

#### System.CommandLine.Hosting

Tied to `IHostBuilder`:

```csharp
return await new CommandLineBuilder(rootCommand)
    .UseHost(args => Host.CreateDefaultBuilder(args), /* ... */)
    .Build()
    .InvokeAsync(args);
```

Cannot use `HostApplicationBuilder` or other builders.

#### LionFire.Hosting.CommandLine

Multiple builder types supported:

```csharp
// Modern approach: HostApplicationBuilder
var program1 = new HostApplicationBuilderProgram();

// Traditional approach: IHostBuilder
var program2 = new HostBuilderProgram();

// Custom builder
var program3 = new CommandLineProgram<MyCustomBuilder, MyBuilderBuilder>();
```

**Benefit:** Can use modern .NET hosting patterns while maintaining backward compatibility.

## Use Case Recommendations

### Choose System.CommandLine When:

1. **Simple, stateless CLI utilities**
   - Quick scripts, file processors, calculators
   - No need for DI or hosting infrastructure
   - Minimal dependencies preferred

2. **Learning command-line parsing**
   - Understanding the fundamentals
   - No additional abstractions to learn

3. **Maximum control and transparency**
   - Need to understand every step
   - Custom execution patterns

4. **Minimal binary size critical**
   - No hosting overhead
   - Fastest startup time

### Choose LionFire.Hosting.CommandLine When:

1. **Application uses Microsoft.Extensions.Hosting**
   - Background services, timers, lifetime management
   - Structured logging and configuration
   - Already using DI patterns

2. **Multiple commands with shared infrastructure**
   - Database access, API clients, logging
   - Reduces code duplication through inheritance

3. **Dynamic configuration based on arguments**
   - Conditional service registration
   - Feature flags via command-line
   - Environment-specific setup

4. **Complex option handling**
   - Many options across multiple commands
   - Strongly-typed options classes
   - Options need to be injectable

5. **Command hierarchy with shared concerns**
   - Parent commands provide context
   - Child commands extend functionality
   - Natural command grouping

## Migration Path

### From System.CommandLine to LionFire

If you have existing System.CommandLine code and want to add hosting:

**Before:**
```csharp
var command = new Command("run");
command.SetHandler(() =>
{
    var service = new MyService();
    service.Run();
});
```

**After:**
```csharp
var program = new HostApplicationBuilderProgram()
    .Command("run", builder =>
    {
        builder.Services.AddSingleton<MyService>();
        builder.Services.AddHostedService<MyBackgroundService>();
    });
```

**Steps:**
1. Replace handler with builder configuration
2. Move service creation to DI registration
3. Use `IHostedService` for background work
4. Extract options to classes if needed

### Gradual Migration

You can wrap System.CommandLine code in LionFire commands:

```csharp
var program = new HostApplicationBuilderProgram()
    .Command("legacy", (context, builder) =>
    {
        builder.Services.AddSingleton<IHostedService>(sp =>
            new DelegateHostedService(async ct =>
            {
                // Original System.CommandLine handler logic
                await MyLegacyHandler();
            }));
    });
```

## Performance Considerations

### System.CommandLine
- **Parsing overhead:** Minimal
- **Startup time:** Very fast
- **Memory:** Small footprint
- **Suitable for:** Short-lived CLI tools

### LionFire.Hosting.CommandLine
- **Parsing overhead:** Minimal (inherited from System.CommandLine)
- **Startup time:** Slower due to hosting startup
- **Memory:** Larger due to DI container and hosting infrastructure
- **Suitable for:** Long-running applications, complex services

**Note:** For applications already using hosting, LionFire adds negligible overhead. The cost is in the hosting infrastructure itself, not the library.

## Conclusion

LionFire.Hosting.CommandLine is a **higher-level abstraction** built on System.CommandLine. It's designed for a specific use case: **command-line applications that use Microsoft.Extensions.Hosting**.

- **Don't use LionFire** if you need a quick CLI script or tool
- **Do use LionFire** if you're building a hosted application with DI

They complement each other rather than compete:
- System.CommandLine handles parsing (low-level)
- LionFire.Hosting.CommandLine handles hosting integration (high-level)

Both can coexist in the same project if needed - use LionFire for hosted commands and System.CommandLine directly for simple utilities.
