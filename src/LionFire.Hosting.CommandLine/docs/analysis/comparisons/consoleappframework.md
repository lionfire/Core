# Comparison: LionFire.Hosting.CommandLine vs. ConsoleAppFramework

## Overview

**ConsoleAppFramework** (by CySharp) is a modern, high-performance CLI framework that leverages C# source generators to create zero-overhead command-line applications. It represents a fundamentally different approach from LionFire.Hosting.CommandLine.

## Quick Comparison Table

| Feature | ConsoleAppFramework | LionFire.Hosting.CommandLine |
|---------|---------------------|----------------------------|
| Performance | ✓✓✓ Highest (zero overhead) | ✓ Standard (hosting overhead) |
| AOT Support | ✓ Native AOT | ⚠️ Depends on System.CommandLine |
| Source Generation | ✓ Compile-time | ✗ Runtime only |
| DI Integration | ✓ Built-in | ✓ Built-in |
| Hosting Integration | ✓ Microsoft.Extensions.Hosting | ✓ Microsoft.Extensions.Hosting |
| Command Definition | ✓ Method-based | ✓ Fluent API |
| Options Binding | ✓ Automatic from parameters | ✓ Automatic to classes |
| Command Hierarchy | ✓ Nested types | ✓ String-based |
| Configuration Inheritance | ⚠️ Via DI only | ✓ Built-in |
| Middleware Support | ✓ Filter pipeline | ⚠️ Via DI |
| Validation | ✓ DataAnnotations | ⚠️ Manual/third-party |
| Help Generation | ✓ From XML comments | ✓ Manual |
| Learning Curve | Low-Medium | Medium |
| .NET Requirement | .NET 8+ | .NET 6+ |
| Maturity | Stable | Based on beta library |

## Detailed Comparison

### 1. Basic Command Definition

#### ConsoleAppFramework

```csharp
// Method-based approach - ultra simple
ConsoleApp.Run(args, (string name, int age = 18) =>
{
    Console.WriteLine($"Hello {name}, age {age}!");
});
```

Or class-based:

```csharp
public class MyCommands : ConsoleAppBase
{
    /// <summary>
    /// Greet a person
    /// </summary>
    /// <param name="name">-n, Person's name</param>
    /// <param name="age">-a, Person's age</param>
    public void Hello(string name, int age = 18)
    {
        Console.WriteLine($"Hello {name}, age {age}!");
    }
}

// Run it
ConsoleApp.Run(args, new MyCommands());
```

**Pros:**
- Extremely concise
- XML comments become help text
- Parameters automatically become CLI arguments
- Source generator creates optimal code
- Zero runtime overhead

**Cons:**
- Less explicit than builder APIs
- Help text in XML comments (separate from logic)

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

        // Can register services
        builder.Services.AddSingleton(options);
    },
    builderBuilder: bb =>
    {
        bb.Command.AddOption(new Option<string>("--name", "-n"));
        bb.Command.AddOption(new Option<int>("--age", "-a") { DefaultValue = 18 });
    });

await program.RunAsync(args);
```

**Pros:**
- Explicit option registration
- Full control over option configuration
- Can add complex initialization

**Cons:**
- Much more verbose
- Help text manual
- More boilerplate

**Verdict:** ConsoleAppFramework is dramatically more concise for simple commands. LionFire offers more control at the cost of verbosity.

### 2. Dependency Injection

#### ConsoleAppFramework

Built-in DI with `ConsoleAppBuilder`:

```csharp
var builder = ConsoleApp.CreateBuilder(args);

// Register services
builder.ConfigureServices(services =>
{
    services.AddSingleton<IMyService, MyService>();
    services.AddHttpClient();
});

var app = builder.Build();

// Commands can inject services
app.Add("", (IMyService service, string input) =>
{
    service.Process(input);
});

app.Run();
```

Or via constructor injection in class-based approach:

```csharp
public class MyCommands : ConsoleAppBase
{
    private readonly IMyService _service;

    public MyCommands(IMyService service)
    {
        _service = service;
    }

    public void Process(string input)
    {
        _service.Process(input);
    }
}

var builder = ConsoleApp.CreateBuilder(args);
builder.ConfigureServices(services =>
{
    services.AddSingleton<IMyService, MyService>();
});

var app = builder.Build();
app.Run<MyCommands>();
```

**Pros:**
- Clean, standard DI pattern
- Services injected into commands directly
- Supports all DI lifetimes
- Minimal ceremony

#### LionFire.Hosting.CommandLine

DI configuration in command builder:

```csharp
var program = new HostApplicationBuilderProgram()
    .Command<Options>("process", (context, builder) =>
    {
        var options = context.GetOptions<Options>();

        // Register services
        builder.Services.AddSingleton<IMyService, MyService>();
        builder.Services.AddHttpClient();

        // Options available to all services
        builder.Services.AddSingleton(options);

        // Background service receives injected services
        builder.Services.AddHostedService<ProcessorService>();
    });

// In ProcessorService:
public class ProcessorService : BackgroundService
{
    private readonly IMyService _service;
    private readonly Options _options;

    public ProcessorService(IMyService service, Options options)
    {
        _service = service;
        _options = options;
    }
}
```

**Pros:**
- Can conditionally register services based on options
- Full hosting infrastructure
- Services get options automatically

**Cons:**
- More boilerplate
- Command logic in hosted services (indirect)

**Verdict:** ConsoleAppFramework has cleaner DI for direct command execution. LionFire is better suited for background services.

### 3. Multiple Commands

#### ConsoleAppFramework

Method-based (each method is a command):

```csharp
public class GitCommands : ConsoleAppBase
{
    /// <summary>Add files to staging</summary>
    public void Add(string file) => Console.WriteLine($"Adding {file}");

    /// <summary>Commit changes</summary>
    public void Commit(string message, bool all = false)
    {
        Console.WriteLine($"Committing: {message}");
    }

    /// <summary>Clone a repository</summary>
    public void Clone(string url) => Console.WriteLine($"Cloning {url}");
}

ConsoleApp.Run<GitCommands>(args);
```

Running: `myapp add file.txt` or `myapp commit "Initial" --all`

**Pros:**
- Each method automatically becomes a command
- Method name is command name (kebab-cased)
- Parameters become options/arguments
- Very clean and intuitive

**Cons:**
- All commands in one class (can get large)
- Limited control over command structure

#### LionFire.Hosting.CommandLine

Explicit command registration:

```csharp
var program = new HostApplicationBuilderProgram()
    .Command<AddOptions>("add", (context, builder) =>
    {
        var opts = context.GetOptions<AddOptions>();
        builder.Services.AddHostedService<AddService>();
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Add files to staging";
        bb.Command.AddArgument(new Argument<string>("file"));
    })

    .Command<CommitOptions>("commit", (context, builder) =>
    {
        var opts = context.GetOptions<CommitOptions>();
        builder.Services.AddHostedService<CommitService>();
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Commit changes";
        bb.Command.AddOption(new Option<string>("--message", "-m"));
        bb.Command.AddOption(new Option<bool>("--all", "-a"));
    });
```

**Pros:**
- Full control over command structure
- Can share configuration between commands
- Explicit and verbose

**Cons:**
- Much more code
- Manual registration
- More ceremony

**Verdict:** ConsoleAppFramework is dramatically more concise for straightforward multi-command apps.

### 4. Nested Commands

#### ConsoleAppFramework

Use nested classes:

```csharp
public class Commands : ConsoleAppBase
{
    public class Database : ConsoleAppBase
    {
        private readonly IDbService _db;

        public Database(IDbService db) => _db = db;

        /// <summary>Run migrations</summary>
        public void Migrate() => Console.WriteLine("Migrating...");

        /// <summary>Seed data</summary>
        public void Seed() => Console.WriteLine("Seeding...");
    }
}

ConsoleApp.Run<Commands>(args);
```

Running: `myapp database migrate`

**Pros:**
- Natural C# nesting
- Type-safe
- Can inject services into nested classes

**Cons:**
- No automatic configuration sharing
- Nested class constructors need service registration

#### LionFire.Hosting.CommandLine

String-based hierarchy with inheritance:

```csharp
var program = new HostApplicationBuilderProgram()
    // Parent: database
    .Command("database", builder =>
    {
        // Shared DB configuration
        builder.Services.AddSingleton<IDbService, DbService>();
    })

    // Child: database migrate (inherits IDbService)
    .Command("database migrate", builder =>
    {
        builder.Services.AddHostedService<MigrationService>();
    })

    // Child: database seed (also inherits IDbService)
    .Command("database seed", builder =>
    {
        builder.Services.AddHostedService<SeedService>();
    });
```

**Pros:**
- Automatic configuration inheritance
- Parent commands provide shared setup
- Reduces duplication

**Cons:**
- String-based (less type-safe)
- More verbose

**Verdict:** LionFire's inheritance is unique and powerful for shared configuration. ConsoleAppFramework is simpler but doesn't inherit setup.

### 5. Validation

#### ConsoleAppFramework

Built-in DataAnnotations support:

```csharp
using System.ComponentModel.DataAnnotations;

public class Commands : ConsoleAppBase
{
    public void Process(
        [Range(1, 100)] int count,
        [Required] string output,
        [EmailAddress] string email)
    {
        // Validation happens automatically before execution
        Console.WriteLine($"Processing {count} items");
    }
}
```

**Pros:**
- Automatic validation
- Standard DataAnnotations
- Validation errors handled automatically
- No manual code needed

**Cons:**
- Limited to DataAnnotations
- Can't easily use custom validators

#### LionFire.Hosting.CommandLine

Manual validation or third-party libraries:

```csharp
public class ProcessOptions
{
    public int Count { get; set; }
    public string Output { get; set; } = "";
    public string Email { get; set; } = "";
}

var program = new HostApplicationBuilderProgram()
    .Command<ProcessOptions>("process", (context, builder) =>
    {
        var opts = context.GetOptions<ProcessOptions>();

        // Manual validation
        if (opts.Count < 1 || opts.Count > 100)
            throw new ArgumentException("Count must be 1-100");

        if (string.IsNullOrEmpty(opts.Output))
            throw new ArgumentException("Output is required");

        // Or use FluentValidation
        var validator = new ProcessOptionsValidator();
        validator.ValidateAndThrow(opts);
    });
```

Or add validators to System.CommandLine options:

```csharp
builderBuilder: bb =>
{
    var countOption = new Option<int>("--count");
    countOption.AddValidator(result =>
    {
        var value = result.GetValueForOption(countOption);
        if (value < 1 || value > 100)
            result.ErrorMessage = "Count must be 1-100";
    });
    bb.Command.AddOption(countOption);
}
```

**Pros:**
- Flexible (any validation library)
- Can access DI for complex validation

**Cons:**
- Manual setup
- More code
- Not as declarative

**Verdict:** ConsoleAppFramework has cleaner, automatic validation.

### 6. Middleware / Filters

#### ConsoleAppFramework

Built-in filter pipeline:

```csharp
public class LoggingFilter : ConsoleAppFilter
{
    public override async Task InvokeAsync(
        ConsoleAppContext context,
        Func<ConsoleAppContext, Task> next)
    {
        Console.WriteLine($"Before: {context.Command.Name}");
        try
        {
            await next(context);
        }
        finally
        {
            Console.WriteLine($"After: {context.Command.Name}");
        }
    }
}

var builder = ConsoleApp.CreateBuilder(args);
builder.ConfigureServices(services =>
{
    services.AddSingleton<ConsoleAppFilter, LoggingFilter>();
});
```

**Pros:**
- Built-in filter pipeline
- Standard middleware pattern
- Can short-circuit execution
- Access to context

**Cons:**
- Requires understanding filter pattern

#### LionFire.Hosting.CommandLine

No built-in middleware, but can use DI:

```csharp
.Command("process", builder =>
{
    // Register middleware-like services
    builder.Services.AddSingleton<ICommandExecutor>(sp =>
        new LoggingCommandExecutor(sp.GetRequiredService<IActualExecutor>()));
});

public class LoggingCommandExecutor : ICommandExecutor
{
    private readonly ICommandExecutor _inner;

    public LoggingCommandExecutor(ICommandExecutor inner) => _inner = inner;

    public async Task ExecuteAsync()
    {
        Console.WriteLine("Before");
        await _inner.ExecuteAsync();
        Console.WriteLine("After");
    }
}
```

**Pros:**
- Flexible (use any pattern)

**Cons:**
- Manual setup
- No standard pattern
- More boilerplate

**Verdict:** ConsoleAppFramework has cleaner middleware support.

### 7. Performance

#### ConsoleAppFramework

**Source Generator Magic:**
- Generates optimal parsing code at compile-time
- Zero reflection at runtime
- Native AOT compatible
- Minimal allocations

**Startup time:** ~1-10ms for simple commands

**Memory:** Minimal overhead

#### LionFire.Hosting.CommandLine

**Runtime Parsing:**
- Uses System.CommandLine parsing
- Hosting infrastructure startup
- DI container initialization
- Background service activation

**Startup time:** ~50-200ms (hosting overhead)

**Memory:** Larger due to hosting and DI

**Verdict:** ConsoleAppFramework is 10-100x faster for short-lived commands. For long-running apps, difference is negligible.

## Architectural Differences

### ConsoleAppFramework: Method/Source-Gen Based

```
Method Definition → Source Generator → Optimized Code
        ↓
   Compile-Time
        ↓
    Zero Overhead Runtime Execution
```

- Methods become commands automatically
- Source generator creates specialized parsers
- No runtime reflection
- AOT-friendly

### LionFire.Hosting.CommandLine: Builder-Based

```
Registration → Runtime Parsing → Builder Config → Host Creation → Execution
```

- Explicit registration at runtime
- Hosting infrastructure required
- Flexible but heavier

## Use Case Recommendations

### Choose ConsoleAppFramework When:

1. **Performance is critical**
   - Short-lived commands
   - Startup time matters
   - Native AOT deployment

2. **Simple to moderate CLI applications**
   - Quick tools, converters, utilities
   - Straightforward command structure
   - Standard DI patterns

3. **Want minimal ceremony**
   - Method-based commands
   - Automatic help from XML comments
   - Built-in validation

4. **Modern .NET target**
   - .NET 8+ only
   - Can use source generators
   - Want latest performance

5. **Filter/middleware patterns needed**
   - Pre/post command execution hooks
   - Cross-cutting concerns
   - Standard middleware pipeline

### Choose LionFire.Hosting.CommandLine When:

1. **Command hierarchy with shared config**
   - Parent commands configure infrastructure
   - Child commands inherit setup
   - Deep command nesting

2. **Complex conditional configuration**
   - Different services per environment
   - Feature flags from command-line
   - Dynamic host configuration

3. **Long-running hosted applications**
   - Background services
   - Timed jobs
   - Startup time less critical

4. **Need multiple hosting builder types**
   - Mix IHostBuilder and HostApplicationBuilder
   - Custom hosting patterns
   - Migration scenarios

5. **Explicit control preferred**
   - Want to see all configuration
   - Prefer builder APIs over conventions
   - Complex option configurations

## Mixing Both Libraries

Theoretically possible but not recommended:
- Different command paradigms
- Different hosting setups
- Would be confusing

**Alternative:** Choose one based on primary use case.

## Migration Considerations

### From ConsoleAppFramework to LionFire

**Scenario:** Want to add configuration inheritance

1. Convert methods to builder registrations
2. Extract shared configuration to parent commands
3. Use hosted services for background work

**Effort:** Medium to High

### From LionFire to ConsoleAppFramework

**Scenario:** Need better performance

1. Flatten command hierarchy
2. Convert builder configs to method parameters
3. Move validation to DataAnnotations
4. Extract background work from hosting

**Effort:** Medium

## Performance Benchmark (Hypothetical)

```
Command: myapp process --input file.txt --count 100

ConsoleAppFramework:
- Startup: 5ms
- Parsing: 1ms
- Execution: <command specific>
- Total overhead: ~6ms

LionFire.Hosting.CommandLine:
- Startup: 100ms (hosting)
- Parsing: 2ms
- Builder config: 10ms
- Host creation: 30ms
- Execution: <command specific>
- Total overhead: ~142ms

For 1-second command:
- ConsoleAppFramework: 1.006s
- LionFire: 1.142s

For 10-second command:
- ConsoleAppFramework: 10.006s
- LionFire: 10.142s

For long-running service:
- Both: ~same (startup is one-time)
```

## Conclusion

ConsoleAppFramework and LionFire.Hosting.CommandLine target **overlapping but distinct use cases**:

**ConsoleAppFramework:**
- Modern, high-performance CLI framework
- Method-based command definition
- Source-gen for zero overhead
- Best for short-lived commands
- Excellent middleware support

**LionFire.Hosting.CommandLine:**
- Hosting-first CLI framework
- Builder-based configuration
- Command hierarchy inheritance
- Best for long-running apps
- Explicit control over everything

**Choose ConsoleAppFramework if:**
- Performance is top priority
- You like method-based commands
- Standard CLI patterns suffice
- .NET 8+ is acceptable

**Choose LionFire if:**
- Configuration inheritance is valuable
- Complex conditional setup needed
- Already using hosting infrastructure
- Prefer explicit builder APIs

Both are excellent libraries serving different needs. ConsoleAppFramework is the better general-purpose CLI framework, while LionFire excels in the specific niche of hierarchical command applications with shared configuration.
