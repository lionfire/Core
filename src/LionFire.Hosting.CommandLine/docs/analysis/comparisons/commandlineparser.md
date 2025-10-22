# Comparison: LionFire.Hosting.CommandLine vs. CommandLineParser

## Overview

**CommandLineParser** is a popular, mature library that uses an attribute-based approach to define command-line options. It has a fundamentally different design philosophy from LionFire.Hosting.CommandLine.

## Quick Comparison Table

| Feature | CommandLineParser | LionFire.Hosting.CommandLine |
|---------|------------------|----------------------------|
| Options Definition | ✓ Attributes on properties | ✓ Fluent API |
| Command Definition | ✓ Verb attribute | ✓ String hierarchy |
| Parsing Approach | ✓ Declarative | ✓ Imperative |
| DI Integration | ⚠️ Manual | ✓ First-class |
| Hosting Integration | ✗ Not provided | ✓ Core feature |
| Options Validation | ✓ Via attributes | ⚠️ Via DI validators |
| Help Generation | ✓ Automatic from attributes | ✓ Manual via Command.Description |
| Error Handling | ✓ Built-in with errors collection | ✓ Via System.CommandLine |
| Learning Curve | Low | Medium |
| Verbosity | Low | Medium |
| Maturity | Stable (v2.x) | Based on beta library |
| Dependencies | None | System.CommandLine, Hosting |
| .NET Support | .NET Framework, .NET Core, .NET 5+ | .NET 6+ |

## Detailed Comparison

### 1. Basic Option Definition

#### CommandLineParser

```csharp
using CommandLine;

public class Options
{
    [Option('v', "verbose", Required = false, HelpText = "Enable verbose output")]
    public bool Verbose { get; set; }

    [Option('o', "output", Required = true, HelpText = "Output directory")]
    public string OutputDirectory { get; set; } = "";

    [Option('c', "count", Required = false, Default = 10, HelpText = "Number of items")]
    public int Count { get; set; }
}

class Program
{
    static int Main(string[] args)
    {
        return Parser.Default.ParseArguments<Options>(args)
            .MapResult(
                options => Run(options),
                errors => 1);
    }

    static int Run(Options options)
    {
        if (options.Verbose)
            Console.WriteLine($"Processing {options.Count} items to {options.OutputDirectory}");

        // Do work...
        return 0;
    }
}
```

**Pros:**
- Very concise
- Self-documenting (attributes describe options)
- Automatic help generation from attributes
- No manual option registration

**Cons:**
- Options tied to class definition
- Limited runtime flexibility
- No DI or hosting support

#### LionFire.Hosting.CommandLine

```csharp
using LionFire.Hosting.CommandLine;
using Microsoft.Extensions.Hosting;
using System.CommandLine;

public class Options
{
    public bool Verbose { get; set; }
    public string OutputDirectory { get; set; } = "";
    public int Count { get; set; } = 10;
}

class Program
{
    static async Task<int> Main(string[] args)
    {
        var program = new HostApplicationBuilderProgram()
            .RootCommand<Options>((context, builder) =>
            {
                var options = context.GetOptions<Options>();

                if (options.Verbose)
                {
                    builder.Logging.SetMinimumLevel(LogLevel.Debug);
                }

                builder.Services.AddSingleton(options);
                builder.Services.AddHostedService<ProcessorService>();
            },
            builderBuilder: bb =>
            {
                bb.Command.AddOption(new Option<bool>(
                    new[] { "-v", "--verbose" },
                    "Enable verbose output"));
                bb.Command.AddOption(new Option<string>(
                    new[] { "-o", "--output" },
                    "Output directory"));
                bb.Command.AddOption(new Option<int>(
                    new[] { "-c", "--count" },
                    () => 10,
                    "Number of items"));
            });

        return await program.RunAsync(args);
    }
}
```

**Pros:**
- Full DI and hosting support
- Runtime flexibility
- Strongly-typed options in DI container
- Can conditionally configure services based on options

**Cons:**
- More verbose
- Manual option registration
- Help text separate from option definition

**Verdict:** CommandLineParser is much more concise for simple scenarios. LionFire is more appropriate when you need DI/hosting.

### 2. Multiple Commands (Verbs)

#### CommandLineParser

```csharp
[Verb("add", HelpText = "Add file contents to the index.")]
public class AddOptions
{
    [Option('v', "verbose")]
    public bool Verbose { get; set; }

    [Value(0, MetaName = "file", HelpText = "File to add", Required = true)]
    public string FileName { get; set; } = "";
}

[Verb("commit", HelpText = "Record changes to the repository.")]
public class CommitOptions
{
    [Option('m', "message", Required = true)]
    public string Message { get; set; } = "";

    [Option('a', "all")]
    public bool All { get; set; }
}

[Verb("clone", HelpText = "Clone a repository.")]
public class CloneOptions
{
    [Value(0, MetaName = "url", Required = true)]
    public string Url { get; set; } = "";
}

class Program
{
    static int Main(string[] args)
    {
        return Parser.Default.ParseArguments<AddOptions, CommitOptions, CloneOptions>(args)
            .MapResult(
                (AddOptions opts) => RunAdd(opts),
                (CommitOptions opts) => RunCommit(opts),
                (CloneOptions opts) => RunClone(opts),
                errors => 1);
    }

    static int RunAdd(AddOptions options) { /* ... */ return 0; }
    static int RunCommit(CommitOptions options) { /* ... */ return 0; }
    static int RunClone(CloneOptions options) { /* ... */ return 0; }
}
```

**Pros:**
- Very clean and declarative
- Each verb has its own options class
- Type-safe dispatch via `MapResult`
- Automatic help for all verbs

**Cons:**
- All handler methods in same place
- No shared configuration between verbs
- Manual setup in each handler

#### LionFire.Hosting.CommandLine

```csharp
public class AddOptions
{
    public bool Verbose { get; set; }
    public string FileName { get; set; } = "";
}

public class CommitOptions
{
    public string Message { get; set; } = "";
    public bool All { get; set; }
}

public class CloneOptions
{
    public string Url { get; set; } = "";
}

var program = new HostApplicationBuilderProgram()
    // Shared configuration for all commands
    .RootCommand(builder =>
    {
        builder.Services.AddLogging();
        builder.Services.AddSingleton<IGitService, GitService>();
    })

    .Command<AddOptions>("add", (context, builder) =>
    {
        var options = context.GetOptions<AddOptions>();
        builder.Services.AddHostedService<AddService>();
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Add file contents to the index";
        bb.Command.AddOption(new Option<bool>(new[] { "-v", "--verbose" }));
        bb.Command.AddArgument(new Argument<string>("file", "File to add"));
    })

    .Command<CommitOptions>("commit", (context, builder) =>
    {
        var options = context.GetOptions<CommitOptions>();
        builder.Services.AddHostedService<CommitService>();
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Record changes to the repository";
        bb.Command.AddOption(new Option<string>(new[] { "-m", "--message" }));
        bb.Command.AddOption(new Option<bool>(new[] { "-a", "--all" }));
    })

    .Command<CloneOptions>("clone", (context, builder) =>
    {
        var options = context.GetOptions<CloneOptions>();
        builder.Services.AddHostedService<CloneService>();
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Clone a repository";
        bb.Command.AddArgument(new Argument<string>("url"));
    });

return await program.RunAsync(args);
```

**Pros:**
- Shared configuration via RootCommand
- Each command gets fully configured DI container
- Strongly-typed options per command
- Services can be injected into each other

**Cons:**
- More verbose
- Manual option/argument registration
- More boilerplate

**Verdict:** CommandLineParser is significantly more concise. LionFire provides better structure for complex applications with shared services.

### 3. Nested Commands

#### CommandLineParser

Not natively supported. Verbs are flat:

```csharp
[Verb("database-migrate")]
public class DatabaseMigrateOptions { }

[Verb("database-seed")]
public class DatabaseSeedOptions { }

// No hierarchy - just naming convention
```

**Limitation:** No true command nesting or inheritance.

#### LionFire.Hosting.CommandLine

Native hierarchical support:

```csharp
var program = new HostApplicationBuilderProgram()
    // Parent: database
    .Command("database", builder =>
    {
        // Shared DB configuration
        builder.Services.AddDbContext<AppDbContext>();
    })

    // Child: database migrate (inherits AppDbContext)
    .Command("database migrate", builder =>
    {
        builder.Services.AddHostedService<MigrationService>();
    })

    // Child: database seed (also inherits AppDbContext)
    .Command("database seed", builder =>
    {
        builder.Services.AddHostedService<SeedService>();
    });
```

**Benefit:** True command hierarchy with configuration inheritance.

**Verdict:** LionFire has first-class support for nested commands. CommandLineParser doesn't.

### 4. Options Validation

#### CommandLineParser

Built-in validation via attributes:

```csharp
public class Options
{
    [Option('p', "port", Required = true)]
    [Range(1, 65535)]
    public int Port { get; set; }

    [Option('e', "email", Required = true)]
    [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
    public string Email { get; set; } = "";

    [Option('f', "file", Required = true)]
    [FileExists] // Custom validator
    public string FilePath { get; set; } = "";
}

// Custom validator
public class FileExistsAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext context)
    {
        if (value is string path && File.Exists(path))
            return ValidationResult.Success!;

        return new ValidationResult("File does not exist");
    }
}
```

**Pros:**
- Declarative validation
- Built-in validation attributes
- Automatic error messages
- Runs before handler execution

**Cons:**
- Limited to attribute-based validation
- Can't access DI services for validation

#### LionFire.Hosting.CommandLine

Validation through DI and System.CommandLine validators:

```csharp
public class Options
{
    public int Port { get; set; }
    public string Email { get; set; } = "";
    public string FilePath { get; set; } = "";
}

var program = new HostApplicationBuilderProgram()
    .RootCommand<Options>((context, builder) =>
    {
        var options = context.GetOptions<Options>();

        // Manual validation (or use FluentValidation, etc.)
        if (options.Port < 1 || options.Port > 65535)
            throw new ArgumentException("Port must be between 1 and 65535");

        if (!File.Exists(options.FilePath))
            throw new ArgumentException($"File not found: {options.FilePath}");

        builder.Services.AddSingleton(options);
    },
    builderBuilder: bb =>
    {
        var portOption = new Option<int>("--port");
        portOption.AddValidator(result =>
        {
            var value = result.GetValueForOption(portOption);
            if (value < 1 || value > 65535)
                result.ErrorMessage = "Port must be between 1 and 65535";
        });

        bb.Command.AddOption(portOption);
    });
```

Or use FluentValidation:

```csharp
public class OptionsValidator : AbstractValidator<Options>
{
    public OptionsValidator()
    {
        RuleFor(x => x.Port).InclusiveBetween(1, 65535);
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.FilePath).Must(File.Exists)
            .WithMessage("File must exist");
    }
}

.Command<Options>("run", (context, builder) =>
{
    var options = context.GetOptions<Options>();

    // Validate using FluentValidation
    var validator = new OptionsValidator();
    validator.ValidateAndThrow(options);

    builder.Services.AddSingleton(options);
});
```

**Pros:**
- Can use any validation library (FluentValidation, DataAnnotations, etc.)
- Can access DI services for complex validation
- Flexible validation timing

**Cons:**
- More manual setup
- Less declarative than attributes
- Validation errors not as automatic

**Verdict:** CommandLineParser has cleaner, more declarative validation. LionFire offers more flexibility but requires more setup.

### 5. Help Generation

#### CommandLineParser

Automatic from attributes:

```csharp
[Verb("commit", HelpText = "Record changes to the repository")]
public class CommitOptions
{
    [Option('m', "message", Required = true, HelpText = "Commit message")]
    public string Message { get; set; } = "";

    [Option('a', "all", HelpText = "Automatically stage all modified files")]
    public bool All { get; set; }

    [Usage(ApplicationAlias = "git")]
    public static IEnumerable<Example> Examples
    {
        get
        {
            yield return new Example("Normal scenario", new CommitOptions { Message = "Initial commit", All = true });
        }
    }
}

// Running with --help produces:
// git commit --message "Initial commit" --all
```

**Pros:**
- Zero effort help generation
- Help text co-located with options
- Examples support
- Usage scenarios

**Cons:**
- Limited customization
- Help format is fixed

#### LionFire.Hosting.CommandLine

Manual help via System.CommandLine:

```csharp
.Command<CommitOptions>("commit", (context, builder) => { /* ... */ },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Record changes to the repository";

        var messageOption = new Option<string>(
            aliases: new[] { "-m", "--message" },
            description: "Commit message");
        messageOption.IsRequired = true;

        var allOption = new Option<bool>(
            aliases: new[] { "-a", "--all" },
            description: "Automatically stage all modified files");

        bb.Command.AddOption(messageOption);
        bb.Command.AddOption(allOption);

        // Examples are not built-in - would need custom implementation
    })
```

**Pros:**
- Full control over help format
- Can customize per platform/culture
- Can integrate with external help systems

**Cons:**
- More manual work
- Help text separated from options definition
- No built-in examples support

**Verdict:** CommandLineParser has superior help generation out of the box.

### 6. Dependency Injection

#### CommandLineParser

No built-in support. Manual integration required:

```csharp
var serviceProvider = new ServiceCollection()
    .AddSingleton<IMyService, MyService>()
    .BuildServiceProvider();

Parser.Default.ParseArguments<Options>(args)
    .MapResult(
        options =>
        {
            var service = serviceProvider.GetRequiredService<IMyService>();
            return Run(options, service);
        },
        errors => 1);
```

**Limitations:**
- Manual service provider creation
- Services not available during option processing
- No hosting lifecycle

#### LionFire.Hosting.CommandLine

First-class DI support:

```csharp
var program = new HostApplicationBuilderProgram()
    .Command<Options>("run", (context, builder) =>
    {
        var options = context.GetOptions<Options>();

        // Register services
        builder.Services.AddSingleton<IMyService, MyService>();
        builder.Services.AddSingleton(options);

        // Add background service
        builder.Services.AddHostedService<MyBackgroundService>();
    });

// In MyBackgroundService:
public class MyBackgroundService : BackgroundService
{
    private readonly IMyService _service;
    private readonly Options _options;

    public MyBackgroundService(IMyService service, Options options)
    {
        _service = service;
        _options = options;
    }
}
```

**Benefits:**
- Native DI container integration
- Options automatically registered
- Full hosting lifecycle
- Scoped, transient, singleton support
- Configuration system integration

**Verdict:** LionFire is designed for DI. CommandLineParser requires manual integration.

## Architectural Differences

### CommandLineParser: Attribute-Driven

```
Attributes → Parse → Options Instance → Handler
```

- Declarative approach
- Options class is the source of truth
- Parsing and validation are automatic
- Handler receives parsed options

### LionFire.Hosting.CommandLine: Builder-Driven

```
Registration → Parse → Builder Configuration → Host Creation → Execution
```

- Imperative approach
- Fluent API for configuration
- Parsing automatic, configuration manual
- Builder receives options and context

## Use Case Recommendations

### Choose CommandLineParser When:

1. **Simple CLI utilities without DI**
   - Quick scripts, converters, processors
   - No need for service infrastructure

2. **Declarative preference**
   - Prefer attributes over fluent APIs
   - Options definition is the primary concern

3. **Automatic help is critical**
   - Help generation is a top priority
   - Need examples and usage scenarios

4. **Minimal dependencies**
   - Want zero dependencies beyond the library
   - Target .NET Framework or older runtimes

5. **Validation-heavy applications**
   - Heavy use of validation attributes
   - Standard validation patterns

### Choose LionFire.Hosting.CommandLine When:

1. **Application uses Microsoft.Extensions.Hosting**
   - Background services
   - Timed jobs
   - Long-running processes

2. **Complex dependency graph**
   - Many services depend on each other
   - Scoped or transient lifetimes needed
   - Configuration system integration

3. **Command hierarchy with shared config**
   - Nested commands (database migrate, database seed)
   - Parent commands provide shared infrastructure

4. **Dynamic configuration based on args**
   - Conditional service registration
   - Environment-specific setup from options

5. **Modern .NET applications**
   - .NET 6+ only
   - Using latest patterns

## Mixing Both Libraries

It's possible to use both in the same project:

```csharp
// Use CommandLineParser for simple commands
Parser.Default.ParseArguments<SimpleOptions>(args)
    .MapResult(opts => QuickTool(opts), errors => 1);

// Use LionFire for hosted commands
var program = new HostApplicationBuilderProgram()
    .Command<ComplexOptions>("serve", (context, builder) =>
    {
        // Complex hosting setup
    });
```

**When to mix:**
- Application has both simple and complex commands
- Gradual migration from CommandLineParser to hosted model
- Different teams prefer different styles

## Migration Strategy

### From CommandLineParser to LionFire

1. **Keep options classes:**
   ```csharp
   // Remove attributes, keep properties
   public class Options
   {
       public bool Verbose { get; set; }
       public string Output { get; set; } = "";
   }
   ```

2. **Replace Parser with Program:**
   ```csharp
   // Before:
   Parser.Default.ParseArguments<Options>(args)
       .MapResult(opts => Run(opts), errors => 1);

   // After:
   var program = new HostApplicationBuilderProgram()
       .RootCommand<Options>((context, builder) =>
       {
           var opts = context.GetOptions<Options>();
           builder.Services.AddSingleton(opts);
           builder.Services.AddHostedService<RunService>();
       });

   await program.RunAsync(args);
   ```

3. **Move logic to services:**
   ```csharp
   // Before: static method
   static int Run(Options opts) { /* ... */ }

   // After: hosted service
   public class RunService : BackgroundService
   {
       private readonly Options _options;
       public RunService(Options options) => _options = options;

       protected override Task ExecuteAsync(CancellationToken ct)
       {
           // Original logic here
       }
   }
   ```

## Performance Considerations

### CommandLineParser
- **Parsing:** Very fast (attribute reflection is cached)
- **Startup:** Minimal overhead
- **Memory:** Small footprint
- **Binary size:** Tiny (~50KB)

### LionFire.Hosting.CommandLine
- **Parsing:** Similar (uses System.CommandLine)
- **Startup:** Slower (hosting infrastructure)
- **Memory:** Larger (DI container + hosting)
- **Binary size:** Larger (depends on hosting packages)

**Rule of thumb:** CommandLineParser is 10-100x faster to start for simple commands. For long-running applications, the difference is negligible.

## Conclusion

CommandLineParser and LionFire.Hosting.CommandLine target **different use cases**:

**CommandLineParser:**
- Quick CLI utilities
- Simple option parsing
- Declarative attributes
- Minimal dependencies

**LionFire.Hosting.CommandLine:**
- Hosted applications
- Complex service graphs
- DI and configuration
- Long-running processes

**They are not competitors** - they solve different problems. Choose based on:
- Do you need DI/hosting? → LionFire
- Is it a simple script? → CommandLineParser
- Prefer declarative? → CommandLineParser
- Prefer imperative? → LionFire
