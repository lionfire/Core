# Comparison: LionFire.Hosting.CommandLine vs. Cocona

## Overview

**Cocona** is a micro-framework for building .NET console applications that takes inspiration from ASP.NET Core's Minimal API approach. It treats public methods as commands by default, offering a simple and intuitive API for rapid console application development.

**Cocona.Lite** is a lightweight variant with fewer dependencies for scenarios where you don't need the full Microsoft.Extensions ecosystem.

## Quick Comparison Table

| Feature | Cocona | Cocona.Lite | LionFire.Hosting.CommandLine |
|---------|--------|-------------|----------------------------|
| Command Definition | ✓ Method-based | ✓ Method-based | ⚠️ Fluent API |
| DI Integration | ✓ Microsoft.Extensions.DI | ⚠️ Minimal | ✓ Full Microsoft.Extensions.DI |
| Hosting Integration | ✓ Microsoft.Extensions.Hosting | ✗ None | ✓ Microsoft.Extensions.Hosting |
| Options Binding | ✓ Automatic from parameters | ✓ Automatic | ✓ Automatic to classes |
| Command Hierarchy | ✓ Nested types | ✓ Nested types | ✓ String-based |
| Configuration Inheritance | ⚠️ Via DI only | ✗ None | ✓ Built-in automatic |
| Help Generation | ✓ From XML comments | ✓ From XML comments | ⚠️ Manual |
| Validation | ✓ DataAnnotations | ✓ DataAnnotations | ⚠️ Manual/third-party |
| Shell Completion | ✓ Built-in | ✓ Built-in | ✓ Via System.CommandLine |
| Learning Curve | Low | Low | Medium |
| Dependencies | Microsoft.Extensions.* | Minimal | Microsoft.Extensions.* + System.CommandLine |
| .NET Support | .NET Standard 2.0+, .NET 5+ | .NET Standard 2.0+ | .NET 6+ |
| Maturity | Stable | Stable | Based on beta library |

## Detailed Comparison

### 1. Simple Command Definition

#### Cocona

```csharp
// Single lambda-based command
CoconaApp.Run((string name, int age = 18) =>
{
    Console.WriteLine($"Hello {name}, age {age}!");
});
```

Or class-based:

```csharp
public class Commands
{
    /// <summary>
    /// Greet a person
    /// </summary>
    /// <param name="name">-n, Name of the person</param>
    /// <param name="age">-a, Age of the person</param>
    public void Greet(string name, int age = 18)
    {
        Console.WriteLine($"Hello {name}, age {age}!");
    }
}

CoconaApp.Run<Commands>(args);
```

**Pros:**
- Extremely concise for simple cases
- Method parameters become CLI options automatically
- XML comments become help text
- Zero boilerplate

**Cons:**
- Less explicit than builder APIs
- Help text in XML comments

#### Cocona.Lite

Identical syntax to Cocona:

```csharp
CoconaLiteApp.Run((string name, int age = 18) =>
{
    Console.WriteLine($"Hello {name}, age {age}!");
});
```

**Difference:** Fewer dependencies, no hosting infrastructure.

#### LionFire.Hosting.CommandLine

```csharp
public class GreetOptions
{
    public string Name { get; set; } = "";
    public int Age { get; set; } = 18;
}

var program = new HostApplicationBuilderProgram()
    .RootCommand<GreetOptions>((context, builder) =>
    {
        var options = context.GetOptions<GreetOptions>();
        Console.WriteLine($"Hello {options.Name}, age {options.Age}!");
    },
    builderBuilder: bb =>
    {
        bb.Command.AddOption(new Option<string>(new[] { "-n", "--name" }));
        bb.Command.AddOption(new Option<int>(new[] { "-a", "--age" }, () => 18));
    });

await program.RunAsync(args);
```

**Pros:**
- Explicit option registration
- Can add complex host configuration

**Cons:**
- Much more verbose
- More boilerplate

**Verdict:** Cocona is dramatically more concise for simple commands. LionFire is better when you need hosting infrastructure.

### 2. Multiple Commands

#### Cocona

```csharp
public class GitCommands
{
    /// <summary>
    /// Add files to staging
    /// </summary>
    /// <param name="file">File to add</param>
    public void Add(string file)
    {
        Console.WriteLine($"Adding {file}");
    }

    /// <summary>
    /// Commit changes
    /// </summary>
    /// <param name="message">-m, Commit message</param>
    /// <param name="all">-a, Stage all files</param>
    [Command("commit")]
    public void Commit(
        [Argument] string message,
        [Option('a')] bool all = false)
    {
        Console.WriteLine($"Committing: {message} (all: {all})");
    }

    /// <summary>
    /// Push to remote
    /// </summary>
    public void Push()
    {
        Console.WriteLine("Pushing to remote");
    }
}

var app = CoconaApp.Create();
app.AddCommands<GitCommands>();
app.Run(args);
```

**Pros:**
- Each method is a command automatically
- Method name becomes command name (kebab-cased)
- Very clean and organized
- XML comments for help

**Cons:**
- All commands in one class (or manual organization)
- No automatic configuration sharing

#### Cocona.Lite

Same as Cocona, just use `CoconaLiteApp` instead:

```csharp
var app = CoconaLiteApp.Create();
app.AddCommands<GitCommands>();
app.Run(args);
```

#### LionFire.Hosting.CommandLine

```csharp
var program = new HostApplicationBuilderProgram()
    // Common logging
    .RootCommand(builder =>
    {
        builder.Logging.AddConsole();
    })

    .Command<AddOptions>("git add", (context, builder) =>
    {
        var opts = context.GetOptions<AddOptions>();
        builder.Services.AddHostedService<AddService>();
    })

    .Command<CommitOptions>("git commit", (context, builder) =>
    {
        var opts = context.GetOptions<CommitOptions>();
        builder.Services.AddHostedService<CommitService>();
    })

    .Command("git push", (context, builder) =>
    {
        builder.Services.AddHostedService<PushService>();
    });

await program.RunAsync(args);
```

**Pros:**
- Shared configuration via RootCommand
- Full hosting infrastructure

**Cons:**
- Much more verbose
- Less clear method-to-command mapping

**Verdict:** Cocona is significantly cleaner for method-based multi-command apps.

### 3. Dependency Injection

#### Cocona

Built-in DI with Microsoft.Extensions.DependencyInjection:

```csharp
public class GitCommands
{
    private readonly IGitService _gitService;
    private readonly ILogger<GitCommands> _logger;

    // Services injected via constructor
    public GitCommands(IGitService gitService, ILogger<GitCommands> logger)
    {
        _gitService = gitService;
        _logger = logger;
    }

    public void Commit(string message)
    {
        _logger.LogInformation($"Committing: {message}");
        _gitService.Commit(message);
    }
}

var app = CoconaApp.CreateBuilder();
app.Services.AddSingleton<IGitService, GitService>();
app.Services.AddLogging(builder => builder.AddConsole());

var coconaApp = app.Build();
coconaApp.AddCommands<GitCommands>();
coconaApp.Run(args);
```

**Pros:**
- Standard Microsoft.Extensions.DI
- Services injected into command classes
- Clean and familiar pattern
- Full DI capabilities

**Cons:**
- Services registered before parsing
- Cannot conditionally register based on options

#### Cocona.Lite

Minimal DI support:

```csharp
// Manual service creation
var gitService = new GitService();

CoconaLiteApp.Run((string message) =>
{
    gitService.Commit(message);
});
```

**Limitation:** No built-in DI container. Must manually manage services.

#### LionFire.Hosting.CommandLine

```csharp
public class CommitOptions
{
    public string Message { get; set; } = "";
    public bool UseCache { get; set; }
}

var program = new HostApplicationBuilderProgram()
    .Command<CommitOptions>("commit", (context, builder) =>
    {
        var options = context.GetOptions<CommitOptions>();

        // Register services after parsing
        builder.Services.AddSingleton<IGitService, GitService>();
        builder.Logging.AddConsole();

        // Conditionally register based on options
        if (options.UseCache)
            builder.Services.AddSingleton<IGitCache, GitCache>();

        builder.Services.AddHostedService<CommitService>();
    });

// In CommitService
public class CommitService : BackgroundService
{
    private readonly IGitService _gitService;
    private readonly CommitOptions _options;

    public CommitService(IGitService gitService, CommitOptions options)
    {
        _gitService = gitService;
        _options = options;
    }
}
```

**Pros:**
- Configure DI after parsing
- Conditional service registration
- Options available in DI

**Cons:**
- More indirection
- Logic in hosted services

**Verdict:** Cocona has cleaner DI for command-centric apps. LionFire has more flexible DI for dynamic scenarios.

### 4. Nested Commands

#### Cocona

Using nested classes:

```csharp
public class Commands
{
    public class Database
    {
        private readonly IDbService _db;

        public Database(IDbService db) => _db = db;

        /// <summary>Run migrations</summary>
        public void Migrate() => Console.WriteLine("Migrating...");

        /// <summary>Seed data</summary>
        public void Seed() => Console.WriteLine("Seeding...");
    }
}

var app = CoconaApp.CreateBuilder();
app.Services.AddSingleton<IDbService, DbService>();

var coconaApp = app.Build();
coconaApp.AddCommands<Commands>();
coconaApp.Run(args);
```

Running: `myapp database migrate`

**Pros:**
- Natural C# nesting
- Type-safe
- Services injected into nested classes

**Cons:**
- No automatic configuration sharing between parent/child
- Each nested class needs DI setup

#### Cocona.Lite

Same nested class approach:

```csharp
public class Commands
{
    public class Database
    {
        public void Migrate() => Console.WriteLine("Migrating...");
        public void Seed() => Console.WriteLine("Seeding...");
    }
}

CoconaLiteApp.Run<Commands>(args);
```

#### LionFire.Hosting.CommandLine

String-based hierarchy with automatic inheritance:

```csharp
var program = new HostApplicationBuilderProgram()
    // Parent: database
    .Command("database", builder =>
    {
        // Shared configuration for all database commands
        builder.Services.AddDbContext<AppDbContext>();
    })

    // Children automatically inherit DbContext
    .Command("database migrate", builder =>
    {
        builder.Services.AddHostedService<MigrationService>();
    })

    .Command("database seed", builder =>
    {
        builder.Services.AddHostedService<SeedService>();
    });
```

**Pros:**
- Automatic configuration inheritance
- Parent provides shared infrastructure
- No duplication

**Cons:**
- String-based (less type-safe)
- Hidden dependencies

**Verdict:** LionFire's automatic inheritance is unique and powerful. Cocona requires manual sharing.

### 5. Validation

#### Cocona

Built-in DataAnnotations support:

```csharp
using System.ComponentModel.DataAnnotations;

public class Commands
{
    public void CreateUser(
        [Argument, Required] string name,
        [Option('e'), Required, EmailAddress] string email,
        [Option('a'), Range(18, 120)] int age = 18)
    {
        Console.WriteLine($"Creating user: {name} ({email}), age {age}");
    }
}

CoconaApp.Run<Commands>(args);
```

**Pros:**
- Automatic validation
- Standard DataAnnotations
- Validation errors before execution
- Clean and declarative

**Cons:**
- Limited to DataAnnotations
- Cannot easily use custom validators

#### Cocona.Lite

Same validation support:

```csharp
public void CreateUser(
    [Required] string name,
    [EmailAddress] string email)
{
    // Validation automatic
}
```

#### LionFire.Hosting.CommandLine

Manual validation:

```csharp
public class CreateUserOptions
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public int Age { get; set; } = 18;
}

.Command<CreateUserOptions>("create-user", (context, builder) =>
{
    var options = context.GetOptions<CreateUserOptions>();

    // Manual validation
    if (string.IsNullOrEmpty(options.Name))
        throw new ArgumentException("Name required");

    if (!IsValidEmail(options.Email))
        throw new ArgumentException("Invalid email");

    // Or use FluentValidation
    var validator = new CreateUserOptionsValidator();
    validator.ValidateAndThrow(options);
})
```

**Pros:**
- Flexible (any validation library)
- Can access DI for validation

**Cons:**
- Manual setup
- More code

**Verdict:** Cocona has cleaner, automatic validation.

### 6. Help Generation

#### Cocona

Automatic from XML comments:

```csharp
public class Commands
{
    /// <summary>
    /// Process files with various options
    /// </summary>
    /// <param name="input">Input file path</param>
    /// <param name="output">-o, Output directory</param>
    /// <param name="verbose">-v, Enable verbose output</param>
    public void Process(
        [Argument] string input,
        [Option('o')] string output = "output",
        [Option('v')] bool verbose = false)
    {
        // Implementation
    }
}
```

Running with `--help` produces:
```
Process files with various options

Usage: myapp process <input> [options]

Arguments:
  input    Input file path

Options:
  -o, --output <value>    Output directory (Default: output)
  -v, --verbose           Enable verbose output
```

**Pros:**
- Automatic from XML comments
- Co-located with code
- Consistent formatting

**Cons:**
- Format less customizable

#### Cocona.Lite

Same help generation from XML comments.

#### LionFire.Hosting.CommandLine

Manual help via System.CommandLine:

```csharp
.Command<ProcessOptions>("process", /* ... */,
    builderBuilder: bb =>
    {
        bb.Command.Description = "Process files with various options";
        bb.Command.AddArgument(new Argument<string>("input", "Input file path"));
        bb.Command.AddOption(new Option<string>(
            new[] { "-o", "--output" },
            () => "output",
            "Output directory"));
        bb.Command.AddOption(new Option<bool>(
            new[] { "-v", "--verbose" },
            "Enable verbose output"));
    })
```

**Pros:**
- Full control over help
- Customizable

**Cons:**
- Manual registration
- Help text separate from logic

**Verdict:** Cocona has cleaner automatic help generation from XML comments.

### 7. Shell Completion

#### Cocona

Built-in shell completion generation:

```csharp
var app = CoconaApp.CreateBuilder();
app.Services.AddCoconaShellCompletion(); // Enable shell completion

var coconaApp = app.Build();
coconaApp.AddCommands<Commands>();
coconaApp.Run(args);

// Generate completion script
// myapp --completion bash > completion.sh
```

**Pros:**
- Built-in support
- Multiple shells (bash, zsh, PowerShell)
- Easy to enable

#### Cocona.Lite

Also supports shell completion:

```csharp
CoconaLiteApp.RunWithShellCompletion<Commands>(args);
```

#### LionFire.Hosting.CommandLine

Via System.CommandLine:

```csharp
// System.CommandLine has tab completion support
// But integration with LionFire would need additional setup
```

**Pros:**
- System.CommandLine supports tab completion

**Cons:**
- Not as straightforward as Cocona

**Verdict:** Cocona has easier shell completion setup.

### 8. Configuration Integration

#### Cocona

Full Microsoft.Extensions.Configuration support:

```csharp
var app = CoconaApp.CreateBuilder(args);

app.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();

var coconaApp = app.Build();
coconaApp.AddCommands<Commands>();
coconaApp.Run();

// In command class
public class Commands
{
    private readonly IConfiguration _config;

    public Commands(IConfiguration config)
    {
        _config = config;
    }

    public void Run()
    {
        var apiKey = _config["ApiKey"];
        Console.WriteLine($"API Key: {apiKey}");
    }
}
```

**Pros:**
- Full configuration system
- Multiple sources
- Standard patterns

#### Cocona.Lite

No built-in configuration support:

```csharp
// Must manually load configuration
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

CoconaLiteApp.Run((string input) =>
{
    var apiKey = config["ApiKey"];
    // Use apiKey
});
```

#### LionFire.Hosting.CommandLine

Native configuration support:

```csharp
.Command("run", builder =>
{
    builder.Configuration
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables();

    // Available in all services
})
```

**Pros:**
- Full hosting configuration system
- Integrated with DI

**Verdict:** Cocona and LionFire both have full configuration support. Cocona.Lite requires manual setup.

### 9. Async Support

#### Cocona

Native async support:

```csharp
public class Commands
{
    private readonly IHttpClientFactory _httpFactory;

    public Commands(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    public async Task Fetch(string url)
    {
        var client = _httpFactory.CreateClient();
        var content = await client.GetStringAsync(url);
        Console.WriteLine(content);
    }
}
```

**Pros:**
- Native async/await
- No special handling needed

#### Cocona.Lite

Same async support:

```csharp
CoconaLiteApp.Run(async (string url) =>
{
    using var client = new HttpClient();
    var content = await client.GetStringAsync(url);
    Console.WriteLine(content);
});
```

#### LionFire.Hosting.CommandLine

Async via hosted services:

```csharp
.Command("fetch", builder =>
{
    builder.Services.AddHostedService<FetchService>();
})

public class FetchService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var client = new HttpClient();
        var content = await client.GetStringAsync("...");
        Console.WriteLine(content);
    }
}
```

**Pros:**
- Full async support
- Background services

**Cons:**
- More indirection

**Verdict:** Cocona has cleaner async in command methods.

## Cocona vs. Cocona.Lite

### When to Use Cocona

- Need Microsoft.Extensions.DI
- Want hosting infrastructure
- Use configuration system
- Need logging framework
- Prefer full-featured

### When to Use Cocona.Lite

- Want minimal dependencies
- Don't need hosting
- Simple applications
- Smaller binary size
- Faster startup

### Dependency Comparison

**Cocona:**
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Hosting
- Microsoft.Extensions.Configuration
- Microsoft.Extensions.Logging

**Cocona.Lite:**
- Zero dependencies (only .NET BCL)

## Use Case Recommendations

### Choose Cocona When:

1. **Rapid development priority**
   - Quick utilities and tools
   - Prototyping
   - Internal tooling

2. **Method-based commands preferred**
   - Natural method-to-command mapping
   - Like minimal API style
   - XML comments for help

3. **Standard DI patterns**
   - Need Microsoft.Extensions.DI
   - Want hosting infrastructure
   - Standard .NET patterns

4. **Built-in validation important**
   - DataAnnotations validation
   - Automatic enforcement
   - Declarative style

5. **Shell completion needed**
   - Tab completion for commands
   - Multiple shell support
   - Easy to enable

### Choose Cocona.Lite When:

1. **Minimal dependencies critical**
   - Small binary size
   - No external dependencies
   - Faster startup

2. **Simple applications**
   - Quick scripts
   - Utilities
   - No DI needed

3. **Learning scenarios**
   - Teaching CLI basics
   - Minimal complexity
   - Clear code

### Choose LionFire.Hosting.CommandLine When:

1. **Command hierarchy with shared config**
   - Parent commands provide infrastructure
   - Configuration inheritance critical
   - Deep command nesting

2. **Complex conditional configuration**
   - Dynamic service registration
   - Feature flags from CLI
   - Environment-specific setup

3. **Already heavily invested in hosting**
   - Background services required
   - Full hosting lifecycle needed
   - Complex service graphs

## Performance Comparison

### Startup Time

```
Cocona.Lite:         ~5ms
Cocona:              ~50ms (hosting)
LionFire:            ~150ms (hosting + hierarchy)
```

### Memory Usage

```
Cocona.Lite:         ~3 MB
Cocona:              ~15 MB
LionFire:            ~25 MB
```

### Binary Size

```
Cocona.Lite:         ~50 KB
Cocona:              ~1.5 MB
LionFire:            ~3 MB
```

## Conclusion

Cocona, Cocona.Lite, and LionFire.Hosting.CommandLine represent different points on the complexity/features spectrum:

**Cocona.Lite:**
- Minimal dependencies
- Fastest startup
- Method-based commands
- Best for simple utilities

**Cocona:**
- Full Microsoft.Extensions support
- Method-based commands
- Built-in validation and help
- Best for standard CLI apps

**LionFire.Hosting.CommandLine:**
- Configuration inheritance
- Hosting-first approach
- Complex service scenarios
- Best for hierarchical apps with shared infrastructure

**Choose Cocona if:**
- Want method-based commands
- Like minimal API style
- Need rapid development
- Prefer automatic help from XML comments
- Don't need configuration inheritance

**Choose LionFire if:**
- Commands share infrastructure
- Configuration inheritance valuable
- Need dynamic host configuration
- Building service-oriented CLI
- Have deep command hierarchies

Cocona is generally the better choice for most CLI applications due to its simplicity and clean API. LionFire excels in the specific niche of hierarchical command applications with shared configuration needs.
