# LionFire.Hosting.CommandLine Documentation

## Quick Links

- [Library README](../README.md) - Start here for overview and quick start
- [Getting Started](getting-started.md) - Comprehensive tutorial and usage guide
- [Architecture](architecture.md) - Internal design and patterns

## Documentation Structure

### Core Documentation

1. **[Getting Started](getting-started.md)**
   - Installation and setup
   - Quick start examples
   - Core concepts explained
   - Common patterns
   - Best practices

2. **[Architecture](architecture.md)**
   - Design philosophy
   - Component overview
   - Execution flow
   - Builder-of-builders pattern
   - Command hierarchy resolution
   - Options binding mechanism
   - Extension points

### Analysis & Comparisons

#### [Tradeoffs Analysis](analysis/tradeoffs.md)
Detailed analysis of design decisions:
- Configuration inheritance vs. explicit registration
- Runtime configuration vs. compile-time optimization
- Hosting infrastructure vs. lightweight execution
- Explicit builder API vs. declarative attributes
- String-based vs. type-based hierarchy
- Performance characteristics
- When to choose this library
- Decision tree and scenarios

#### Comparisons with Other Libraries

**[vs. System.CommandLine](analysis/comparisons/system-commandline.md)**
- What LionFire adds on top of System.CommandLine
- DI integration differences
- Command hierarchy comparison
- When to use each

**[vs. CommandLineParser](analysis/comparisons/commandlineparser.md)**
- Attribute-based vs. builder-based approaches
- Declarative vs. imperative patterns
- Validation comparison
- Help generation differences

**[vs. ConsoleAppFramework](analysis/comparisons/consoleappframework.md)**
- Method-based vs. builder-based commands
- Source generation vs. runtime configuration
- Performance comparison
- Middleware/filter patterns

**[vs. Manual Parsing](analysis/comparisons/manual-parsing.md)**
- When manual parsing makes sense
- Complexity tradeoffs
- Maintainability over time
- Migration strategies

**[vs. CliFx](analysis/comparisons/clifx.md)**
- Class-per-command vs. builder-based
- Testability via IConsole abstraction
- Attribute-based configuration
- Environment variable support

**[vs. Cocona](analysis/comparisons/cocona.md)**
- Method-based vs. builder-based commands
- Cocona vs. Cocona.Lite differences
- XML comments for help
- Minimal API style comparison

**[vs. Spectre.Console.Cli](analysis/comparisons/spectre-console-cli.md)**
- Rich terminal UI integration
- Type-based branching
- Built-in validation
- Beautiful help generation

## Recommended Reading Order

### For New Users

1. [Library README](../README.md) - Get the big picture
2. [Getting Started](getting-started.md) - Learn the basics
3. [Tradeoffs Analysis](analysis/tradeoffs.md) - Understand when to use it
4. [Relevant Comparison](analysis/comparisons/) - See how it compares to what you know

### For Existing System.CommandLine Users

1. [vs. System.CommandLine](analysis/comparisons/system-commandline.md)
2. [Getting Started](getting-started.md) - Focus on hierarchy examples
3. [Architecture](architecture.md) - Understand the builder pattern

### For Architecture Enthusiasts

1. [Architecture](architecture.md) - Deep dive into design
2. [Tradeoffs Analysis](analysis/tradeoffs.md) - Understand the decisions
3. [All Comparisons](analysis/comparisons/) - See how it stacks up

### For Decision Makers

1. [Tradeoffs Analysis](analysis/tradeoffs.md) - Start here!
2. [Relevant Comparison](analysis/comparisons/) - Compare to alternatives
3. [Getting Started](getting-started.md) - See if it fits your use case

## Key Concepts

### Configuration Inheritance

The defining feature of this library. Parent commands configure infrastructure, children inherit automatically:

```csharp
.Command("database", b => b.AddDbContext())      // Configured once
.Command("database migrate", b => { /* ... */ }) // Gets DbContext automatically
.Command("database seed", b => { /* ... */ })    // Gets DbContext automatically
```

See [Getting Started - Command Hierarchy Inheritance](getting-started.md#command-hierarchy-inheritance) for details.

### Builder-of-Builders Pattern

The library uses a "builder-of-builders" approach to enable configuration before the host builder is created:

```
Program → BuilderBuilder → Builder → Host
```

This allows dynamic configuration based on parsed arguments. See [Architecture - Builder-of-Builders Pattern](architecture.md#7-builder-of-builders-pattern-vs-direct-configuration) for deep dive.

### Strongly-Typed Options

Options classes are automatically bound from command-line arguments and registered in DI:

```csharp
public class MyOptions { public bool Verbose { get; set; } }

.Command<MyOptions>("cmd", (context, builder) =>
{
    var options = context.GetOptions<MyOptions>();
    // Options also available via DI in all services
})
```

See [Getting Started - Accessing Options in Services](getting-started.md#accessing-options-in-services) for examples.

## Common Questions

**Q: When should I use this library?**
See [Tradeoffs Analysis - When to Choose](analysis/tradeoffs.md#when-to-choose-lionfirecommandlinecommandline)

**Q: How does it compare to other libraries?**
See [Comparisons](analysis/comparisons/) directory

**Q: What's the performance overhead?**
See [Tradeoffs Analysis - Performance Summary](analysis/tradeoffs.md#performance-tradeoff-summary)

**Q: How do I get started?**
See [Getting Started - Quick Start](getting-started.md#quick-start)

**Q: How does the hierarchy work?**
See [Architecture - Command Hierarchy](architecture.md#command-hierarchy)

## Sample Code

The [Sample Project](../Sample/) directory contains working examples demonstrating:
- Basic command setup
- Strongly-typed options
- Command hierarchies
- Configuration inheritance
- Hosted services integration

## Additional Resources

- [System.CommandLine Documentation](https://learn.microsoft.com/en-us/dotnet/standard/commandline/) - The underlying parsing library
- [Microsoft.Extensions.Hosting Documentation](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host) - The hosting infrastructure
- [LionFire.Core Repository](../../..) - Parent repository

## Contributing

This library is part of the LionFire.Core monorepo. Contributions are welcome! Please see the main repository for contribution guidelines.

## Feedback

Found an issue or have a suggestion? Please open an issue in the LionFire.Core repository.

---

**Last Updated:** 2025-10-02
**Library Version:** Compatible with .NET 9.0
**Status:** Active development
