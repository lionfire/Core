# Comparison: LionFire.Hosting.CommandLine vs. Manual Parsing

## Overview

Manual command-line parsing involves directly processing the `string[] args` parameter without using any library. This comparison explores when manual parsing makes sense versus using LionFire.Hosting.CommandLine.

## Quick Comparison Table

| Feature | Manual Parsing | LionFire.Hosting.CommandLine |
|---------|----------------|----------------------------|
| Code Complexity | Simple for basic, complex for advanced | Medium (abstraction layer) |
| Help Generation | ⚠️ Fully manual | ✓ Via System.CommandLine |
| Tab Completion | ✗ Not available | ✓ Via System.CommandLine |
| Error Handling | ⚠️ Custom implementation | ✓ Built-in |
| Validation | ⚠️ Manual | ⚠️ Manual or third-party |
| Flexibility | ✓✓✓ Complete control | ✓✓ High (within framework) |
| Type Safety | ⚠️ Manual conversion | ✓ Automatic binding |
| Maintainability | ⚠️ Decreases with complexity | ✓ Scales well |
| Dependencies | None | System.CommandLine, Hosting |
| Binary Size | Minimal | Larger |
| Performance | ✓ Fastest (if optimized) | ✓ Fast enough |
| Learning Curve | None | Medium |

## Detailed Comparison

### 1. Simple Argument Parsing

#### Manual Parsing

```csharp
static int Main(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: myapp <name> [age]");
        return 1;
    }

    string name = args[0];
    int age = args.Length > 1 && int.TryParse(args[1], out int parsedAge)
        ? parsedAge
        : 18;

    Console.WriteLine($"Hello {name}, age {age}");
    return 0;
}
```

**Pros:**
- Zero dependencies
- Minimal code for simple cases
- Complete control
- Maximum performance

**Cons:**
- No help generation
- Manual error handling
- No validation
- Positional only (no named options)

#### LionFire.Hosting.CommandLine

```csharp
public class GreetOptions
{
    public string Name { get; set; } = "";
    public int Age { get; set; } = 18;
}

static async Task<int> Main(string[] args)
{
    var program = new HostApplicationBuilderProgram()
        .RootCommand<GreetOptions>((context, builder) =>
        {
            var options = context.GetOptions<GreetOptions>();
            Console.WriteLine($"Hello {options.Name}, age {options.Age}");
        },
        builderBuilder: bb =>
        {
            bb.Command.AddArgument(new Argument<string>("name"));
            bb.Command.AddArgument(new Argument<int>("age") { DefaultValue = 18 });
        });

    return await program.RunAsync(args);
}
```

**Pros:**
- Automatic help generation
- Error messages
- Type conversion
- Tab completion support

**Cons:**
- More boilerplate
- Heavier dependencies

**Verdict:** For truly simple positional args, manual parsing is cleaner. LionFire adds value for named options and help.

### 2. Named Options (Flags)

#### Manual Parsing

```csharp
static int Main(string[] args)
{
    bool verbose = false;
    string output = "output.txt";
    int count = 10;

    for (int i = 0; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "-v":
            case "--verbose":
                verbose = true;
                break;

            case "-o":
            case "--output":
                if (i + 1 < args.Length)
                    output = args[++i];
                else
                {
                    Console.WriteLine("Error: --output requires a value");
                    return 1;
                }
                break;

            case "-c":
            case "--count":
                if (i + 1 < args.Length && int.TryParse(args[++i], out int parsedCount))
                    count = parsedCount;
                else
                {
                    Console.WriteLine("Error: --count requires an integer value");
                    return 1;
                }
                break;

            case "-h":
            case "--help":
                Console.WriteLine("Usage: myapp [options]");
                Console.WriteLine("Options:");
                Console.WriteLine("  -v, --verbose    Enable verbose output");
                Console.WriteLine("  -o, --output     Output file (default: output.txt)");
                Console.WriteLine("  -c, --count      Item count (default: 10)");
                return 0;

            default:
                Console.WriteLine($"Unknown option: {args[i]}");
                return 1;
        }
    }

    Process(verbose, output, count);
    return 0;
}
```

**Pros:**
- Complete control
- No dependencies
- Clear logic flow

**Cons:**
- Verbose and repetitive
- Manual help generation
- Easy to make mistakes
- Hard to maintain as options grow

#### LionFire.Hosting.CommandLine

```csharp
public class Options
{
    public bool Verbose { get; set; }
    public string Output { get; set; } = "output.txt";
    public int Count { get; set; } = 10;
}

var program = new HostApplicationBuilderProgram()
    .RootCommand<Options>((context, builder) =>
    {
        var options = context.GetOptions<Options>();
        Process(options.Verbose, options.Output, options.Count);
    },
    builderBuilder: bb =>
    {
        bb.Command.AddOption(new Option<bool>(
            new[] { "-v", "--verbose" },
            "Enable verbose output"));
        bb.Command.AddOption(new Option<string>(
            new[] { "-o", "--output" },
            () => "output.txt",
            "Output file"));
        bb.Command.AddOption(new Option<int>(
            new[] { "-c", "--count" },
            () => 10,
            "Item count"));
    });

return await program.RunAsync(args);
```

**Pros:**
- Automatic help generation
- Type conversion
- Validation
- Less boilerplate for option handling
- Consistent error messages

**Cons:**
- More ceremony for simple cases
- Framework dependency

**Verdict:** For >3 options, LionFire significantly reduces boilerplate and maintenance burden.

### 3. Subcommands

#### Manual Parsing

```csharp
static int Main(string[] args)
{
    if (args.Length == 0)
    {
        PrintHelp();
        return 1;
    }

    string command = args[0];
    string[] commandArgs = args.Skip(1).ToArray();

    switch (command)
    {
        case "add":
            return AddCommand(commandArgs);

        case "commit":
            return CommitCommand(commandArgs);

        case "push":
            return PushCommand(commandArgs);

        case "help":
        case "--help":
        case "-h":
            PrintHelp();
            return 0;

        default:
            Console.WriteLine($"Unknown command: {command}");
            PrintHelp();
            return 1;
    }
}

static int AddCommand(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: myapp add <file>");
        return 1;
    }

    string file = args[0];
    Console.WriteLine($"Adding {file}");
    return 0;
}

static int CommitCommand(string[] args)
{
    string message = "";
    bool all = false;

    for (int i = 0; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "-m":
            case "--message":
                if (i + 1 < args.Length)
                    message = args[++i];
                break;

            case "-a":
            case "--all":
                all = true;
                break;

            default:
                Console.WriteLine($"Unknown option: {args[i]}");
                return 1;
        }
    }

    if (string.IsNullOrEmpty(message))
    {
        Console.WriteLine("Error: Commit message required");
        return 1;
    }

    Console.WriteLine($"Committing: {message} (all: {all})");
    return 0;
}

static void PrintHelp()
{
    Console.WriteLine("Usage: myapp <command> [options]");
    Console.WriteLine("Commands:");
    Console.WriteLine("  add <file>          Add file to staging");
    Console.WriteLine("  commit -m <msg>     Commit changes");
    Console.WriteLine("  push                Push to remote");
}
```

**Challenges:**
- Each command needs own parsing logic
- Help text must be manually maintained
- Grows complex quickly
- No shared option handling

#### LionFire.Hosting.CommandLine

```csharp
public class AddOptions
{
    public string File { get; set; } = "";
}

public class CommitOptions
{
    public string Message { get; set; } = "";
    public bool All { get; set; }
}

var program = new HostApplicationBuilderProgram()
    .Command<AddOptions>("add", (context, builder) =>
    {
        var options = context.GetOptions<AddOptions>();
        Console.WriteLine($"Adding {options.File}");
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Add file to staging";
        bb.Command.AddArgument(new Argument<string>("file", "File to add"));
    })

    .Command<CommitOptions>("commit", (context, builder) =>
    {
        var options = context.GetOptions<CommitOptions>();
        Console.WriteLine($"Committing: {options.Message} (all: {options.All})");
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Commit changes";
        bb.Command.AddOption(new Option<string>(
            new[] { "-m", "--message" },
            "Commit message") { IsRequired = true });
        bb.Command.AddOption(new Option<bool>(
            new[] { "-a", "--all" },
            "Stage all files"));
    })

    .Command("push", (context, builder) =>
    {
        Console.WriteLine("Pushing to remote");
    },
    builderBuilder: bb =>
    {
        bb.Command.Description = "Push to remote";
    });

return await program.RunAsync(args);
```

**Pros:**
- Automatic help for all commands
- Consistent option handling
- Type-safe options
- Scales well

**Cons:**
- More initial setup
- Framework dependency

**Verdict:** For multi-command apps, LionFire significantly improves maintainability.

### 4. Complex Scenarios

#### Nested Commands with Manual Parsing

```csharp
static int Main(string[] args)
{
    if (args.Length == 0) { /* help */ return 1; }

    switch (args[0])
    {
        case "database":
            return DatabaseCommands(args.Skip(1).ToArray());

        case "user":
            return UserCommands(args.Skip(1).ToArray());

        default:
            Console.WriteLine($"Unknown command: {args[0]}");
            return 1;
    }
}

static int DatabaseCommands(string[] args)
{
    if (args.Length == 0) { /* help */ return 1; }

    switch (args[0])
    {
        case "migrate":
            return MigrateCommand(args.Skip(1).ToArray());

        case "seed":
            return SeedCommand(args.Skip(1).ToArray());

        case "backup":
            return BackupCommand(args.Skip(1).ToArray());

        default:
            Console.WriteLine($"Unknown database command: {args[0]}");
            return 1;
    }
}

// Each command has own parsing...
```

**Problems:**
- Recursive parsing logic
- Repeated help text management
- No shared configuration
- Error-prone
- Difficult to maintain

#### LionFire.Hosting.CommandLine

```csharp
var program = new HostApplicationBuilderProgram()
    // Parent: database (shared config)
    .Command("database", builder =>
    {
        builder.Services.AddDbContext<AppDbContext>();
    })

    // Children inherit DbContext
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
    })

    .Command("user", builder =>
    {
        builder.Services.AddSingleton<IUserService, UserService>();
    })

    .Command("user create", builder =>
    {
        builder.Services.AddHostedService<CreateUserService>();
    });
```

**Benefits:**
- Configuration inheritance (database → children)
- Automatic help hierarchy
- Centralized command registration
- Type-safe throughout

**Verdict:** For nested commands, manual parsing becomes unmaintainable. LionFire provides critical structure.

## When Manual Parsing Makes Sense

### 1. Extremely Simple Scripts

```csharp
// Convert temperature
// Usage: temp <celsius>
static void Main(string[] args)
{
    if (args.Length != 1 || !double.TryParse(args[0], out double celsius))
    {
        Console.WriteLine("Usage: temp <celsius>");
        return;
    }

    double fahrenheit = celsius * 9 / 5 + 32;
    Console.WriteLine($"{celsius}°C = {fahrenheit}°F");
}
```

**Justification:** Single argument, trivial logic, no options needed.

### 2. Ultra-Constrained Environments

- Embedded systems
- AOT with minimal dependencies
- Binary size is critical (<1KB)
- Startup time ultra-critical

### 3. Learning Exercise

Manual parsing teaches:
- How command-line parsing works
- String processing
- Error handling
- User experience considerations

### 4. Custom Parsing Requirements

- Non-standard option formats
- Domain-specific languages
- Interactive prompts
- Complex state machines

## When LionFire Makes Sense

### 1. Business Applications

- Multiple commands (>3)
- Shared infrastructure (database, logging, etc.)
- Long-running processes
- Complex option validation

### 2. Team Development

- Multiple developers
- Need consistent patterns
- Maintenance over time
- Documentation requirements

### 3. User-Facing Tools

- Professional help text
- Tab completion
- Consistent error messages
- Standard conventions

### 4. Rapid Development

- Prototyping quickly
- Changing requirements
- Adding features frequently

## Hybrid Approach

You can mix manual parsing with frameworks:

```csharp
static async Task<int> Main(string[] args)
{
    // Quick manual check for special flags
    if (args.Contains("--version"))
    {
        Console.WriteLine("v1.0.0");
        return 0;
    }

    if (args.Contains("--license"))
    {
        PrintLicense();
        return 0;
    }

    // Use framework for main logic
    var program = new HostApplicationBuilderProgram()
        .Command("run", /* ... */);

    return await program.RunAsync(args);
}
```

**Use cases:**
- Quick checks before framework overhead
- Emergency exit paths
- Debug flags

## Performance Considerations

### Startup Time

```
Manual Parsing:
- Parse args: 0.1ms
- Execute: <command specific>
- Total: ~0.1ms overhead

LionFire.Hosting.CommandLine:
- Parse args: 2ms
- Initialize hosting: 100ms
- Create builder: 10ms
- Build host: 30ms
- Execute: <command specific>
- Total: ~142ms overhead
```

**When it matters:**
- Commands run <1 second
- Invoked thousands of times
- Scripting scenarios

**When it doesn't matter:**
- Long-running apps (>10 seconds)
- Interactive tools
- Development tools

### Binary Size

```
Manual Parsing:
- Application: ~10KB
- Runtime: Framework dependent
- Total: ~10KB

LionFire.Hosting.CommandLine:
- Application: ~50KB
- System.CommandLine: ~500KB
- Microsoft.Extensions.Hosting: ~2MB
- Total: ~2.5MB
```

**When it matters:**
- Embedded systems
- Download size critical
- Deployment constraints

## Maintenance Over Time

### Manual Parsing Evolution

```
Initial (simple):
  50 lines, easy to understand

After 6 months (more features):
  500 lines, getting complex

After 1 year (lots of features):
  2000 lines, hard to maintain
  - Inconsistent error messages
  - Help text out of sync
  - Parsing bugs
  - Feature duplication
```

### LionFire Evolution

```
Initial:
  100 lines, learning curve

After 6 months:
  400 lines, consistent patterns

After 1 year:
  800 lines, scales well
  - Consistent error handling
  - Automatic help generation
  - Type-safe throughout
  - Easy to add features
```

**Verdict:** LionFire scales better with complexity.

## Migration Path

### From Manual to LionFire

**Step 1: Extract Options**
```csharp
// Before: scattered variables
bool verbose = false;
string output = "out.txt";

// After: options class
public class Options
{
    public bool Verbose { get; set; }
    public string Output { get; set; } = "out.txt";
}
```

**Step 2: Replace Parsing**
```csharp
// Before: manual loop
for (int i = 0; i < args.Length; i++) { /* ... */ }

// After: framework registration
var program = new HostApplicationBuilderProgram()
    .RootCommand<Options>((context, builder) =>
    {
        var opts = context.GetOptions<Options>();
        // Original logic here
    });
```

**Step 3: Move to Services** (optional)
```csharp
// Create hosted service for background work
public class AppService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken ct)
    {
        // Original logic
    }
}
```

## Conclusion

**Manual Parsing is best for:**
- Quick scripts (<50 lines total)
- Single argument/simple options
- No dependencies allowed
- Learning exercise
- Ultra-constrained environments

**LionFire.Hosting.CommandLine is best for:**
- Multiple commands (>3)
- Complex option handling
- Shared infrastructure
- Team development
- Long-term maintenance
- Professional tools

**Rule of Thumb:**
- 0-2 options → Consider manual
- 3-5 options → Could go either way
- 6+ options → Use framework
- Multiple commands → Definitely use framework
- Nested commands → Absolutely use framework

The inflection point is around **3 options or 2 commands** - beyond that, framework benefits outweigh manual parsing simplicity.
