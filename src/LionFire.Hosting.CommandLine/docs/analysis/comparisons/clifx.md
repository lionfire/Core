# Comparison: LionFire.Hosting.CommandLine vs. CliFx

## Overview

**CliFx** is a class-first framework for building command-line interfaces with a focus on minimal boilerplate and comprehensive auto-generated help. It takes a different architectural approach from LionFire.Hosting.CommandLine.

## Quick Comparison Table

| Feature | CliFx | LionFire.Hosting.CommandLine |
|---------|-------|----------------------------|
| Command Definition | ✓ Class-based with attributes | ✓ Fluent API |
| DI Integration | ✓ Built-in (optional) | ✓ First-class (required) |
| Hosting Integration | ⚠️ Manual | ✓ Native Microsoft.Extensions.Hosting |
| Options Binding | ✓ Automatic via attributes | ✓ Automatic to classes |
| Command Hierarchy | ✓ Deep nesting supported | ✓ String-based hierarchy |
| Configuration Inheritance | ⚠️ Via base classes | ✓ Built-in automatic |
| Help Generation | ✓✓✓ Comprehensive | ✓ Via System.CommandLine |
| Validation | ✓ Attribute-based | ⚠️ Manual/third-party |
| Testability | ✓✓✓ IConsole abstraction | ✓ Via DI |
| Dependencies | None | System.CommandLine, Hosting |
| Learning Curve | Low | Medium |
| .NET Support | .NET Standard 2.0+ | .NET 6+ |
| Maturity | Stable | Based on beta library |

## Detailed Comparison

### 1. Command Definition

#### CliFx

```csharp
using CliFx;
using CliFx.Attributes;

[Command("greet")]
public class GreetCommand : ICommand
{
    [CommandParameter(0, Description = "Name of person to greet")]
    public string Name { get; init; } = "";

    [CommandOption("age", 'a', Description = "Age of the person")]
    public int Age { get; init; } = 18;

    [CommandOption("verbose", 'v', Description = "Enable verbose output")]
    public bool Verbose { get; init; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (Verbose)
            console.Output.WriteLine($"Processing greeting for {Name}");

        console.Output.WriteLine($"Hello {Name}, age {Age}!");
        return default;
    }
}

// In Program.cs
public static async Task<int> Main() =>
    await new CliApplicationBuilder()
        .AddCommandsFromThisAssembly()
        .Build()
        .RunAsync();
```

**Pros:**
- Very clean class-based approach
- Attributes provide self-documentation
- `IConsole` abstraction for testability
- Automatic command discovery
- No manual option registration

**Cons:**
- Each command is a separate class
- Verbose for simple commands
- Limited runtime flexibility

#### LionFire.Hosting.CommandLine

```csharp
public class GreetOptions
{
    public string Name { get; set; } = "";
    public int Age { get; set; } = 18;
    public bool Verbose { get; set; }
}

var program = new HostApplicationBuilderProgram()
    .Command<GreetOptions>("greet", (context, builder) =>
    {
        var options = context.GetOptions<GreetOptions>();

        if (options.Verbose)
            Console.WriteLine($"Processing greeting for {options.Name}");

        Console.WriteLine($"Hello {options.Name}, age {options.Age}!");
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Greet a person";
        bb.Command.AddArgument(new Argument<string>("name", "Name of person"));
        bb.Command.AddOption(new Option<int>(new[] { "-a", "--age" }, () => 18, "Age"));
        bb.Command.AddOption(new Option<bool>(new[] { "-v", "--verbose" }, "Verbose"));
    });

await program.RunAsync(args);
```

**Pros:**
- Can include host configuration
- Runtime flexibility
- Options class separate from execution

**Cons:**
- More verbose
- Manual option registration
- Less self-documenting

**Verdict:** CliFx is cleaner for straightforward commands. LionFire is better when you need hosting infrastructure.

### 2. Multiple Commands

#### CliFx

```csharp
// Each command is a class
[Command("git add")]
public class GitAddCommand : ICommand
{
    [CommandParameter(0)]
    public string File { get; init; } = "";

    public ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine($"Adding {File}");
        return default;
    }
}

[Command("git commit")]
public class GitCommitCommand : ICommand
{
    [CommandOption("message", 'm', IsRequired = true)]
    public string Message { get; init; } = "";

    [CommandOption("all", 'a')]
    public bool All { get; init; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine($"Committing: {Message} (all: {All})");
        return default;
    }
}

// Auto-discovered via assembly scanning
await new CliApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .Build()
    .RunAsync();
```

**Pros:**
- Clean separation of commands
- Easy to organize in files
- Auto-discovery
- Each command is self-contained

**Cons:**
- No shared configuration between commands
- Each command manages its own resources
- Duplication of common setup

#### LionFire.Hosting.CommandLine

```csharp
var program = new HostApplicationBuilderProgram()
    // Shared logging for all commands
    .RootCommand(builder =>
    {
        builder.Logging.AddConsole();
    })

    .Command<GitAddOptions>("git add", (context, builder) =>
    {
        var opts = context.GetOptions<GitAddOptions>();
        builder.Services.AddHostedService<GitAddService>();
    })

    .Command<GitCommitOptions>("git commit", (context, builder) =>
    {
        var opts = context.GetOptions<GitCommitOptions>();
        builder.Services.AddHostedService<GitCommitService>();
    });

await program.RunAsync(args);
```

**Pros:**
- Shared configuration via RootCommand
- Each command gets configured host
- Consistent infrastructure

**Cons:**
- All commands in one place (or manual organization)
- More ceremony
- Less clear separation

**Verdict:** CliFx has better physical separation. LionFire has better logical sharing.

### 3. Dependency Injection

#### CliFx

Built-in DI support (optional):

```csharp
[Command("process")]
public class ProcessCommand : ICommand
{
    private readonly IFileService _fileService;
    private readonly ILogger<ProcessCommand> _logger;

    // Services injected via constructor
    public ProcessCommand(IFileService fileService, ILogger<ProcessCommand> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    [CommandOption("input", 'i')]
    public string InputFile { get; init; } = "";

    public async ValueTask ExecuteAsync(IConsole console)
    {
        _logger.LogInformation($"Processing {InputFile}");
        await _fileService.ProcessAsync(InputFile);
        return default;
    }
}

// Setup DI
public static async Task<int> Main() =>
    await new CliApplicationBuilder()
        .AddCommandsFromThisAssembly()
        .UseTypeActivator(commandTypes =>
        {
            var services = new ServiceCollection();
            services.AddSingleton<IFileService, FileService>();
            services.AddLogging(builder => builder.AddConsole());

            // Register all command types
            foreach (var commandType in commandTypes)
                services.AddTransient(commandType);

            return services.BuildServiceProvider();
        })
        .Build()
        .RunAsync();
```

**Pros:**
- Standard constructor injection
- Services injected into commands
- Clean and familiar pattern

**Cons:**
- DI setup is manual
- Cannot conditionally register services based on options
- Services configured before parsing

#### LionFire.Hosting.CommandLine

```csharp
public class ProcessOptions
{
    public string InputFile { get; set; } = "";
}

var program = new HostApplicationBuilderProgram()
    .Command<ProcessOptions>("process", (context, builder) =>
    {
        var options = context.GetOptions<ProcessOptions>();

        // Register services after parsing options
        builder.Services.AddSingleton<IFileService, FileService>();
        builder.Logging.AddConsole();

        // Can conditionally register
        if (options.InputFile.EndsWith(".json"))
            builder.Services.AddSingleton<IJsonProcessor, JsonProcessor>();

        builder.Services.AddHostedService<ProcessorService>();
    });

// Services injected into hosted service
public class ProcessorService : BackgroundService
{
    private readonly IFileService _fileService;
    private readonly ProcessOptions _options;

    public ProcessorService(IFileService fileService, ProcessOptions options)
    {
        _fileService = fileService;
        _options = options;
    }
}
```

**Pros:**
- Configure services after parsing
- Conditional service registration
- Options available in DI
- Full hosting infrastructure

**Cons:**
- More indirection
- Logic in hosted services, not commands

**Verdict:** CliFx has simpler DI for command-centric apps. LionFire has more flexible DI for dynamic scenarios.

### 4. Nested Commands & Hierarchy

#### CliFx

Deep nesting via command names:

```csharp
[Command("database")]
public class DatabaseCommand : ICommand
{
    public ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine("Database operations");
        return default;
    }
}

[Command("database migrate")]
public class DatabaseMigrateCommand : ICommand
{
    [CommandOption("force")]
    public bool Force { get; init; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine($"Migrating (force: {Force})");
        return default;
    }
}

[Command("database seed")]
public class DatabaseSeedCommand : ICommand
{
    public ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine("Seeding");
        return default;
    }
}
```

**Pros:**
- Natural nesting via command names
- Each command is independent
- Clear structure

**Cons:**
- No automatic shared configuration
- Must use base classes for shared logic

Using base class for sharing:

```csharp
public abstract class DatabaseCommandBase : ICommand
{
    protected IDbContext DbContext { get; }

    protected DatabaseCommandBase(IDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public abstract ValueTask ExecuteAsync(IConsole console);
}

[Command("database migrate")]
public class MigrateCommand : DatabaseCommandBase
{
    public MigrateCommand(IDbContext dbContext) : base(dbContext) { }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        // DbContext available from base class
        return default;
    }
}
```

**Limitation:** Shared configuration requires inheritance and DI registration.

#### LionFire.Hosting.CommandLine

Automatic configuration inheritance:

```csharp
var program = new HostApplicationBuilderProgram()
    // Parent: shared configuration
    .Command("database", builder =>
    {
        builder.Services.AddDbContext<AppDbContext>();
        builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
    })

    // Children automatically inherit DbContext + logging config
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
- Automatic inheritance
- No manual base classes
- Configuration cascade

**Cons:**
- String-based (less type-safe)
- Hidden dependencies

**Verdict:** CliFx requires manual sharing via base classes. LionFire provides automatic inheritance.

### 5. Validation

#### CliFx

Attribute-based validation:

```csharp
using System.ComponentModel.DataAnnotations;

[Command("create-user")]
public class CreateUserCommand : ICommand
{
    [CommandOption("email", IsRequired = true)]
    [EmailAddress]
    public string Email { get; init; } = "";

    [CommandOption("age")]
    [Range(18, 120)]
    public int Age { get; init; }

    [CommandOption("name", IsRequired = true)]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = "";

    public ValueTask ExecuteAsync(IConsole console)
    {
        // Validation happens automatically before execution
        console.Output.WriteLine($"Creating user: {Name} ({Email})");
        return default;
    }
}
```

**Pros:**
- Declarative validation
- Automatic enforcement
- Standard DataAnnotations
- Clear and self-documenting

**Cons:**
- Limited to attribute-based validation
- Cannot easily access services for validation

#### LionFire.Hosting.CommandLine

Manual or third-party validation:

```csharp
public class CreateUserOptions
{
    public string Email { get; set; } = "";
    public int Age { get; set; }
    public string Name { get; set; } = "";
}

.Command<CreateUserOptions>("create-user", (context, builder) =>
{
    var options = context.GetOptions<CreateUserOptions>();

    // Manual validation
    if (string.IsNullOrEmpty(options.Email) || !IsValidEmail(options.Email))
        throw new ArgumentException("Invalid email");

    if (options.Age < 18 || options.Age > 120)
        throw new ArgumentException("Age must be 18-120");

    // Or use FluentValidation
    var validator = new CreateUserOptionsValidator();
    validator.ValidateAndThrow(options);

    builder.Services.AddHostedService<CreateUserService>();
})
```

**Pros:**
- Flexible validation libraries
- Can access services for validation

**Cons:**
- Manual setup
- Less declarative

**Verdict:** CliFx has cleaner, automatic validation.

### 6. Help Generation

#### CliFx

Comprehensive automatic help:

```csharp
[Command("process", Description = "Process files with various options")]
public class ProcessCommand : ICommand
{
    [CommandParameter(0, Description = "Input file path")]
    public string InputFile { get; init; } = "";

    [CommandOption("output", 'o', Description = "Output directory")]
    public string OutputDir { get; init; } = "output";

    [CommandOption("verbose", 'v', Description = "Enable verbose logging")]
    public bool Verbose { get; init; }

    public ValueTask ExecuteAsync(IConsole console) => default;
}

// Running with --help produces:
// DESCRIPTION
//   Process files with various options
//
// USAGE
//   myapp process <InputFile> [OPTIONS]
//
// ARGUMENTS
//   InputFile    Input file path
//
// OPTIONS
//   -o|--output <value>    Output directory [default: output]
//   -v|--verbose           Enable verbose logging
//   -h|--help              Shows help text
//   --version              Shows version information
```

**Pros:**
- Rich, comprehensive help
- Automatic from attributes
- Consistent formatting
- Examples support

**Cons:**
- Format is fixed

#### LionFire.Hosting.CommandLine

Help via System.CommandLine:

```csharp
.Command<ProcessOptions>("process", /* ... */,
    builderBuilder: bb =>
    {
        bb.Command.Description = "Process files with various options";
        bb.Command.AddArgument(new Argument<string>("input-file", "Input file path"));
        bb.Command.AddOption(new Option<string>(
            new[] { "-o", "--output" },
            () => "output",
            "Output directory"));
        bb.Command.AddOption(new Option<bool>(
            new[] { "-v", "--verbose" },
            "Enable verbose logging"));
    })
```

**Pros:**
- Customizable
- Can integrate with custom help systems

**Cons:**
- Manual configuration
- Help text separate from logic

**Verdict:** CliFx has superior automatic help generation.

### 7. Testability

#### CliFx

Excellent testability via `IConsole`:

```csharp
[Command("greet")]
public class GreetCommand : ICommand
{
    [CommandParameter(0)]
    public string Name { get; init; } = "";

    public ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine($"Hello {Name}!");
        return default;
    }
}

// Test
[Fact]
public async Task GreetCommand_Should_Output_Greeting()
{
    // Arrange
    using var console = new FakeInMemoryConsole();
    var command = new GreetCommand { Name = "World" };

    // Act
    await command.ExecuteAsync(console);

    // Assert
    var output = console.ReadOutputString();
    Assert.Equal("Hello World!\n", output);
}
```

**Pros:**
- `IConsole` abstraction for testing
- Can test commands in isolation
- Built-in fake console
- No mocking needed

**Cons:**
- Must use `IConsole` instead of `Console`

#### LionFire.Hosting.CommandLine

Testing via integration tests:

```csharp
[Fact]
public async Task Should_Execute_Greet_Command()
{
    var program = new HostApplicationBuilderProgram()
        .Command<GreetOptions>("greet", (context, builder) =>
        {
            var options = context.GetOptions<GreetOptions>();
            builder.Services.AddSingleton(options);
            builder.Services.AddHostedService<GreetService>();
        });

    var result = await program.RunAsync(new[] { "greet", "--name", "World" });

    Assert.Equal(0, result);
    // Would need to capture output via logging or custom service
}
```

**Pros:**
- Tests full integration
- Tests with real DI

**Cons:**
- Harder to test in isolation
- Need infrastructure to capture output

**Verdict:** CliFx has superior built-in testability.

### 8. Environment Variables

#### CliFx

Built-in environment variable fallback:

```csharp
[Command]
public class ProcessCommand : ICommand
{
    [CommandOption("api-key", EnvironmentVariable = "API_KEY")]
    public string ApiKey { get; init; } = "";

    public ValueTask ExecuteAsync(IConsole console)
    {
        // ApiKey can come from --api-key or API_KEY env var
        console.Output.WriteLine($"Using API key: {ApiKey}");
        return default;
    }
}
```

**Pros:**
- Declarative environment variable support
- Automatic fallback
- Secure for secrets

**Cons:**
- Limited to options

#### LionFire.Hosting.CommandLine

Via Microsoft.Extensions.Configuration:

```csharp
.Command("process", builder =>
{
    builder.Configuration.AddEnvironmentVariables();

    // Access via configuration system
    var apiKey = builder.Configuration["API_KEY"];
})
```

**Pros:**
- Full configuration system
- Multiple sources
- Flexible

**Cons:**
- Manual setup
- More verbose

**Verdict:** CliFx has cleaner environment variable support.

## Architectural Differences

### CliFx: Class-Per-Command

```
Assembly → Command Discovery → Type Activation → Execute
```

- Each command is a class
- Attributes define options
- DI optional
- Execution via `ICommand.ExecuteAsync()`

### LionFire: Builder-Per-Command

```
Registration → Parsing → Builder Config → Host Creation → Execution
```

- Commands defined via fluent API
- DI required
- Hosting infrastructure mandatory
- Execution via hosted services

## Use Case Recommendations

### Choose CliFx When:

1. **Command-centric applications**
   - Each command is logically independent
   - Commands don't share complex infrastructure
   - Prefer class-based organization

2. **Testability is critical**
   - Need to test commands in isolation
   - Want built-in test infrastructure
   - Prefer `IConsole` abstraction

3. **Rich help text important**
   - Comprehensive help required
   - Examples and usage scenarios
   - Professional CLI feel

4. **Broad .NET compatibility**
   - Need .NET Standard 2.0 support
   - Targeting older frameworks
   - Want zero dependencies

5. **Validation-heavy applications**
   - Many DataAnnotations
   - Declarative validation preferred
   - Standard validation patterns

6. **Environment variable support**
   - Options from environment
   - Secure secret handling
   - 12-factor app patterns

### Choose LionFire.Hosting.CommandLine When:

1. **Command hierarchy with shared config**
   - Natural command grouping
   - Parent commands provide infrastructure
   - Configuration inheritance valuable

2. **Already using Microsoft.Extensions.Hosting**
   - Background services needed
   - Hosting infrastructure in use
   - Full DI capabilities required

3. **Dynamic configuration essential**
   - Conditional service registration
   - Feature flags from CLI
   - Environment-specific setup

4. **Long-running applications**
   - Background workers
   - Scheduled tasks
   - Server-like behavior

## Performance Comparison

### Startup Time

```
CliFx:                    ~10-20ms
LionFire:                 ~150ms
```

**CliFx faster due to:**
- No hosting infrastructure
- Simpler DI (if used)
- Direct command execution

### Memory Usage

```
CliFx:                    ~5-10 MB
LionFire:                 ~25 MB
```

### Binary Size

```
CliFx:                    ~50 KB (library only)
LionFire:                 ~3 MB (with all dependencies)
```

## Migration Strategies

### From CliFx to LionFire

**Scenario:** Need to add hosting infrastructure

1. **Extract options from command classes:**
   ```csharp
   // Before: CliFx
   [Command]
   public class ProcessCommand : ICommand
   {
       [CommandOption("input")]
       public string Input { get; init; } = "";
   }

   // After: Options class
   public class ProcessOptions
   {
       public string Input { get; set; } = "";
   }
   ```

2. **Convert commands to hosted services:**
   ```csharp
   // Before: Execute directly
   public ValueTask ExecuteAsync(IConsole console)
   {
       console.Output.WriteLine("Processing...");
   }

   // After: Hosted service
   public class ProcessService : BackgroundService
   {
       protected override Task ExecuteAsync(CancellationToken ct)
       {
           Console.WriteLine("Processing...");
           return Task.CompletedTask;
       }
   }
   ```

3. **Register in program:**
   ```csharp
   var program = new HostApplicationBuilderProgram()
       .Command<ProcessOptions>("process", (context, builder) =>
       {
           builder.Services.AddHostedService<ProcessService>();
       });
   ```

### From LionFire to CliFx

**Scenario:** Don't need hosting, want simpler model

1. **Create command classes:**
   ```csharp
   [Command("process")]
   public class ProcessCommand : ICommand
   {
       [CommandOption("input")]
       public string Input { get; init; } = "";

       public ValueTask ExecuteAsync(IConsole console)
       {
           // Move logic from hosted service here
           return default;
       }
   }
   ```

2. **Remove hosting setup:**
   ```csharp
   // Replace entire program with:
   await new CliApplicationBuilder()
       .AddCommandsFromThisAssembly()
       .Build()
       .RunAsync();
   ```

## Conclusion

CliFx and LionFire.Hosting.CommandLine serve **different architectural patterns**:

**CliFx:**
- Class-per-command model
- Excellent for independent commands
- Superior testability
- Rich help generation
- Minimal dependencies

**LionFire.Hosting.CommandLine:**
- Hosting-first model
- Excellent for shared infrastructure
- Configuration inheritance
- Background services
- Full DI capabilities

**Choose CliFx if:**
- Commands are independent
- Testability is priority
- Want minimal dependencies
- Prefer class-based organization
- Need .NET Standard 2.0 support

**Choose LionFire if:**
- Commands share infrastructure
- Need hosting capabilities
- Want configuration inheritance
- Building service-oriented CLI
- Already using Microsoft.Extensions.*

Both are excellent libraries with different strengths. The choice depends on whether your architecture is **command-centric** (CliFx) or **hosting-centric** (LionFire).
