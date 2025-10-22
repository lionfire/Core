# Architecture

## Design Philosophy

LionFire.Hosting.CommandLine is designed around three core principles:

1. **Separation of Concerns**: Command structure, host configuration, and execution are cleanly separated
2. **Configuration Inheritance**: Parent commands can provide configuration to child commands, reducing duplication
3. **Type Safety**: Strongly-typed options and builder patterns throughout

## Architectural Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    Command Line Program                      │
│  ┌────────────────────────────────────────────────────────┐ │
│  │              RootCommand (System.CommandLine)          │ │
│  │  ┌──────────────┐  ┌──────────────┐  ┌─────────────┐  │ │
│  │  │  Command A   │  │  Command B   │  │  Command C  │  │ │
│  │  │  ┌────────┐  │  │  ┌────────┐  │  │             │  │ │
│  │  │  │ Cmd A1 │  │  │  │ Cmd B1 │  │  │             │  │ │
│  │  │  └────────┘  │  │  └────────┘  │  │             │  │ │
│  │  └──────────────┘  └──────────────┘  └─────────────┘  │ │
│  └────────────────────────────────────────────────────────┘ │
│                                                              │
│  ┌────────────────────────────────────────────────────────┐ │
│  │         BuilderBuilder Registry (by command)           │ │
│  │  "": RootBuilderBuilder                                │ │
│  │  "database": DatabaseBuilderBuilder                    │ │
│  │  "database migrate": MigrateBuilderBuilder             │ │
│  └────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              ↓
                    Parse & Match Command
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                    Invocation Context                        │
│  - Parsed arguments                                          │
│  - Resolved command hierarchy                                │
│  - Option values                                             │
└─────────────────────────────────────────────────────────────┘
                              ↓
                   Resolve BuilderBuilder Hierarchy
                              ↓
┌─────────────────────────────────────────────────────────────┐
│              BuilderBuilder Hierarchy                        │
│  [RootBuilderBuilder, DatabaseBuilderBuilder,                │
│   MigrateBuilderBuilder]                                     │
└─────────────────────────────────────────────────────────────┘
                              ↓
                    Create TBuilder Instance
                              ↓
┌─────────────────────────────────────────────────────────────┐
│  Execute Initializers (ancestor → descendant order)          │
│  1. RootBuilderBuilder.Initialize(context, builder)          │
│  2. DatabaseBuilderBuilder.Initialize(context, builder)      │
│  3. MigrateBuilderBuilder.Initialize(context, builder)       │
└─────────────────────────────────────────────────────────────┘
                              ↓
                         Build & Run Host
```

## Core Components

### 1. IProgram

The main program interface that manages the command line application.

**Key responsibilities:**
- Maintains the `RootCommand` (System.CommandLine)
- Maintains registry of `IHostingBuilderBuilder` instances keyed by command hierarchy
- Provides the main entry point: `RunAsync(string[] args)`
- Handles command invocation via `Handler(InvocationContext)`

**Implementations:**
- `CommandLineProgram` - Base implementation
- `CommandLineProgram<TBuilder, TBuilderBuilder>` - Strongly-typed generic version
- `HostApplicationBuilderProgram` - Preconfigured for `HostApplicationBuilder`
- `HostBuilderProgram` - Preconfigured for `IHostBuilder`

### 2. IHostingBuilderBuilder<TBuilder>

The "builder-of-builders" - configures a hosting builder (`TBuilder`) for a specific command.

**Key responsibilities:**
- Store initializers (actions that configure the builder)
- Manage command hierarchy and inheritance
- Create and configure the builder when invoked
- Execute the hierarchy of initializers in order

**Generic type parameter:**
- `TBuilder`: The type of hosting builder (e.g., `HostApplicationBuilder`, `IHostBuilder`)

**Properties:**
- `Command` - The System.CommandLine `Command` instance
- `CommandHierarchy` - Full command path (e.g., "database migrate")
- `Inherit` - Whether to inherit parent command configuration
- `OptionsType` - Type of strongly-typed options class
- `Initializers` - List of configuration actions

**Implementations:**
- `HostApplicationBuilderBuilder` - For `HostApplicationBuilder`
- `HostBuilderBuilder` - For `IHostBuilder`

### 3. HostingBuilderBuilderContext

Provides context during builder initialization.

**Properties:**
- `Program` - Reference to the `IProgram`
- `InvocationContext` - System.CommandLine invocation context
- `Options` - Dictionary of parsed option values
- `OptionsObject` - Strongly-typed options instance
- `CommandHierarchy` - Full command hierarchy string
- `CommandName` - Current command name
- `InitializingForCommandName` - Command currently being initialized (during hierarchy traversal)

### 4. LionFireCommandLineOptions

Container for parsed command-line options, automatically registered in DI.

**Properties:**
- `Options` - Dictionary of option name → value
- `InvocationContext` - System.CommandLine invocation context

**Methods:**
- `As<T>()` - Convert to strongly-typed options class via reflection

## Execution Flow

### 1. Program Construction

```csharp
var program = new HostApplicationBuilderProgram()
    .Command<MyOptions>("mycommand", (context, builder) => { /* ... */ });
```

**What happens:**
1. `HostApplicationBuilderProgram` creates a `RootCommand`
2. `.Command<MyOptions>("mycommand", ...)` is called:
   - Creates or retrieves `HostApplicationBuilderBuilder` for "mycommand"
   - Creates `Command` instance and attaches to `RootCommand`
   - Stores the configuration lambda in the builder's `Initializers` list
   - Stores `OptionsType = typeof(MyOptions)`
   - Sets the command's handler to `Program.Handler`

### 2. Command Parsing

```csharp
await program.RunAsync(args);
```

**What happens:**
1. Creates `CommandLineBuilder` with the `RootCommand`
2. Applies System.CommandLine defaults (`.UseDefaults()`)
3. Invokes the command line parser with `args`
4. System.CommandLine matches command and parses options
5. Calls the command's handler (which is `Program.Handler`)

### 3. Handler Execution

```csharp
Task<int> Handler(InvocationContext invocationContext)
```

**What happens:**
1. Extract command hierarchy from `invocationContext` (e.g., "database migrate")
2. Look up `BuilderBuilder` hierarchy:
   - "" (root)
   - "database"
   - "database migrate"
3. Get the leaf `BuilderBuilder` (for "database migrate")
4. Call `BuilderBuilder.Build(program, invocationContext)`

### 4. Builder Creation & Initialization

**In `BuilderBuilderBase<TBuilder>.Build()`:**

1. **Create Builder Instance**
   ```csharp
   var builder = CreateBuilder(); // e.g., new HostApplicationBuilder()
   ```

2. **Create Context**
   ```csharp
   var context = new HostingBuilderBuilderContext
   {
       Program = program,
       InvocationContext = invocationContext,
       HostingBuilderBuilder = this
   };
   ```

3. **Initialize Hierarchy**
   ```csharp
   InitializeHierarchy(program, invocationContext, context, builder);
   ```

   For each ancestor → descendant (e.g., root → database → database migrate):

   a. **Parse Options for Current Command**
      - Extract option values from `ParseResult`
      - Add to `context.Options` dictionary

   b. **Register Options in DI**
      - Register `LionFireCommandLineOptions` as singleton
      - If `OptionsType` is set, create instance via `ModelBinder` and register

   c. **Execute Initializers**
      - Call each initializer: `initializer(context, builder)`
      - This is where your configuration lambdas execute

   d. **Check Inherit Flag**
      - If `Inherit = false`, stop traversing hierarchy

4. **Build Host**
   ```csharp
   return builder.Build(); // Creates IHost
   ```

5. **Run Host**
   ```csharp
   await host.RunAsync(cancellationToken);
   ```

## Command Hierarchy

### Hierarchy String Format

Commands are identified by space-separated hierarchy strings:
- `""` - Root command
- `"database"` - Single-level command
- `"database migrate"` - Nested command
- `"database migrate up"` - Deeply nested command

### Hierarchy Resolution

Given invocation: `myapp database migrate --force`

1. **Parse Command Hierarchy**
   - System.CommandLine parses: `["database", "migrate"]`
   - Library generates hierarchy strings:
     - `""` (root)
     - `"database"`
     - `"database migrate"`

2. **Lookup BuilderBuilders**
   - Look up each hierarchy string in the registry
   - Collect all registered BuilderBuilders
   - Example: `[RootBB, DatabaseBB, MigrateBB]`

3. **Initialize in Order**
   - Reverse the list (ancestor first): `[RootBB, DatabaseBB, MigrateBB]`
   - Execute each `Initialize()` method in order
   - This creates a **configuration cascade**

### Inheritance Control

Each `BuilderBuilder` has an `Inherit` flag (default: `true`):

```csharp
.Command("isolated", builder => { /* ... */ },
    builderBuilder: bb => bb.Inherit = false)
```

When `Inherit = false`:
- Only initializers up to and including this command are executed
- Child commands of "isolated" can still inherit from it
- This creates an inheritance boundary

## Options Binding

### Automatic Binding

When you specify `OptionsType` (or use `Command<TOptions>`), the library:

1. **Creates ModelBinder**
   ```csharp
   var binder = new ModelBinder(OptionsType);
   ```

2. **Binds from ParseResult**
   ```csharp
   var instance = binder.CreateInstance(invocationContext.BindingContext);
   ```

   This uses System.CommandLine's model binding to populate properties.

3. **Registers in DI**
   ```csharp
   services.AddSingleton(OptionsType, instance);
   ```

4. **Stores in Context**
   ```csharp
   context.OptionsObject = instance;
   ```

### Manual Binding

You can also manually define options and bind them:

```csharp
.Command("cmd",
    builder: (context, hab) =>
    {
        // Access via dictionary
        var verbose = (bool)context.Options["verbose"];
    },
    command: cmd =>
    {
        cmd.AddOption(new Option<bool>("--verbose"));
    })
```

## Builder Type Flexibility

The library supports multiple builder types through generic programming:

### Pattern 1: Fixed Builder Type

```csharp
var program = new HostApplicationBuilderProgram(); // TBuilder = HostApplicationBuilder
```

All commands use `HostApplicationBuilder`.

### Pattern 2: Per-Command Builder Type

```csharp
var program = new FlexibleProgram();

program.GetOrAdd<HostApplicationBuilderBuilder>("cmd1"); // Uses HostApplicationBuilder
program.GetOrAdd<HostBuilderBuilder>("cmd2");            // Uses IHostBuilder
```

**Constraint:** Command hierarchy must use consistent builder types. You cannot mix `HostApplicationBuilder` and `IHostBuilder` in the same hierarchy due to the inheritance mechanism.

## Extension Points

### 1. Custom Builder Types

Implement `IHostingBuilderBuilder<TBuilder>`:

```csharp
public class MyCustomBuilder : BuilderBuilderBase<MyCustomHostBuilder>
{
    public override IHost Build(MyCustomHostBuilder builder)
    {
        return builder.BuildHost();
    }

    public override IHostingBuilderBuilder ConfigureServices(Action<IServiceCollection> services)
    {
        // ...
    }
}
```

### 2. Custom Program Types

Inherit from `CommandLineProgram`:

```csharp
public class MyCustomProgram : CommandLineProgram<MyBuilder, MyBuilderBuilder>
{
    // Add custom methods
    public MyCustomProgram AddFeature(string name)
    {
        // Custom logic
        return this;
    }
}
```

### 3. Custom Context Enrichment

Use `IFlex` on `HostingBuilderBuilderContext`:

```csharp
// In an initializer
context.Set("MyCustomData", myData);

// Later
var data = context.Get<MyCustomData>("MyCustomData");
```

## Performance Considerations

### BuilderBuilder Registry Lookup

- Dictionary lookup by string key: **O(1)**
- Hierarchy resolution: **O(depth)** where depth is command nesting level

### Initializer Execution

- Each initializer is called once per invocation
- Hierarchy depth affects initialization time
- Typical hierarchy depth: 1-3 levels

### Memory Footprint

- One `BuilderBuilder` instance per registered command
- One `Command` instance per registered command (System.CommandLine)
- Initializer lambdas captured in closures

### Optimization Tips

1. **Minimize Hierarchy Depth**: Keep command nesting shallow (≤3 levels)
2. **Lazy Service Registration**: Only register services needed for the specific command
3. **Shared Initializers**: Put common config in parent commands to reuse code

## Thread Safety

- **Program Construction**: Not thread-safe. Build your program on a single thread.
- **Execution**: Each invocation creates a new builder instance. Multiple concurrent invocations are safe.
- **BuilderBuilder Registry**: Immutable after construction, safe for concurrent reads.

## Comparison with System.CommandLine.Hosting

System.CommandLine.Hosting provides basic integration with `IHostBuilder`:

```csharp
// System.CommandLine.Hosting approach
rootCommand.SetHandler(async (IHost host) =>
{
    // Host is already built
    await host.RunAsync();
});
```

LionFire.Hosting.CommandLine provides:

1. **Pre-Build Configuration**: Configure the builder *before* it's built
2. **Hierarchy Inheritance**: Share configuration across command trees
3. **Strongly-Typed Options**: Automatic binding to options classes
4. **Multiple Builder Types**: Support for `HostApplicationBuilder`, `IHostBuilder`, and custom builders

## Design Decisions

### Why BuilderBuilder Pattern?

**Problem**: System.CommandLine handlers execute after parsing. By then, the host builder is already configured.

**Solution**: Store configuration logic in "builder builders" that execute when the command is invoked, allowing dynamic configuration based on parsed arguments.

### Why Dictionary Registry?

**Alternatives Considered:**
- Tree structure matching command hierarchy
- Separate registries per builder type

**Chosen:** Dictionary with string keys (command hierarchy)

**Rationale:**
- Simple and predictable
- Easy hierarchy lookup
- Supports sparse registration (not all commands need builders)

### Why Inheritance by Default?

**Rationale:**
- Reduces boilerplate for common configurations (logging, DI, etc.)
- Follows the principle of least surprise (child commands extend parents)
- Easy to opt-out when needed (`Inherit = false`)

### Why Support Multiple Builder Types?

**Rationale:**
- Accommodates different hosting patterns (modern vs. traditional)
- Allows migration from `IHostBuilder` to `HostApplicationBuilder`
- Enables custom hosting scenarios

## Future Enhancements

Possible areas for extension:

1. **Async Initializers**: Support `Func<Task>` initializers for async configuration
2. **Middleware Pipeline**: Pre/post-execution hooks for commands
3. **Validation Pipeline**: Declarative validation for options before initialization
4. **Configuration Profiles**: Named configuration sets for different scenarios
5. **Enhanced Error Handling**: Structured error reporting for initialization failures
