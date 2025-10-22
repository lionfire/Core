# CLI Libraries Comparison Summary

## Overview

This document provides a comprehensive comparison of popular .NET command-line parsing libraries, helping you choose the right tool for your use case.

## Libraries Compared

1. **LionFire.Hosting.CommandLine** - Hosting-first with configuration inheritance
2. **System.CommandLine** - Microsoft's official library (foundation for LionFire)
3. **CommandLineParser** - Mature attribute-based library
4. **ConsoleAppFramework** - High-performance source-gen framework
5. **CliFx** - Class-first framework with excellent testability
6. **Cocona** - Method-based minimal API style framework
7. **Spectre.Console.Cli** - Rich terminal UI with CLI parsing
8. **Manual Parsing** - Roll your own

## Quick Decision Matrix

| Your Priority | Best Choice | Runner-Up |
|---------------|-------------|-----------|
| **Configuration Inheritance** | LionFire | N/A (unique feature) |
| **Rich Terminal UI** | Spectre.Console.Cli | CliFx |
| **Performance** | ConsoleAppFramework | Manual |
| **Simplicity** | Cocona | Manual |
| **Testability** | CliFx | Spectre.Console.Cli |
| **Declarative Style** | CommandLineParser | CliFx |
| **Method-based Commands** | Cocona | ConsoleAppFramework |
| **Minimal Dependencies** | Manual | Cocona.Lite |
| **Hosting Integration** | LionFire | Cocona |
| **Validation** | CliFx | Spectre.Console.Cli |

## Feature Comparison

| Feature | LionFire | System.CL | CmdLineParser | CAF | CliFx | Cocona | Spectre.Cli | Manual |
|---------|----------|-----------|---------------|-----|-------|--------|-------------|--------|
| **Command Hierarchy** | ✓✓✓ | ✓ | ✗ | ✓ | ✓✓ | ✓ | ✓✓ | ⚠️ |
| **Config Inheritance** | ✓✓✓ | ✗ | ✗ | ⚠️ | ⚠️ | ⚠️ | ⚠️ | ✗ |
| **DI Integration** | ✓✓✓ | ⚠️ | ⚠️ | ✓✓ | ✓✓ | ✓✓✓ | ✓✓ | ✗ |
| **Hosting Integration** | ✓✓✓ | ⚠️ | ✗ | ✓✓ | ⚠️ | ✓✓✓ | ⚠️ | ✗ |
| **Performance** | ⚠️ | ✓✓ | ✓✓ | ✓✓✓ | ✓✓ | ✓✓ | ✓✓ | ✓✓✓ |
| **Help Generation** | ✓✓ | ✓✓✓ | ✓✓✓ | ✓✓ | ✓✓✓ | ✓✓ | ✓✓✓ | ⚠️ |
| **Validation** | ⚠️ | ⚠️ | ✓✓✓ | ✓✓ | ✓✓✓ | ✓✓ | ✓✓ | ⚠️ |
| **Testability** | ✓✓ | ✓✓ | ✓✓ | ✓✓ | ✓✓✓ | ✓✓ | ✓✓✓ | ✓✓✓ |
| **Rich UI** | ⚠️ | ✗ | ✗ | ✗ | ⚠️ | ✗ | ✓✓✓ | ✗ |
| **Learning Curve** | Medium | Low-Med | Low | Medium | Low | Low | Medium | None |
| **Maturity** | Beta | Beta | Stable | Stable | Stable | Stable | Stable | N/A |

**Legend:** ✓✓✓ Excellent, ✓✓ Good, ✓ Adequate, ⚠️ Limited, ✗ Not Available

## Architectural Styles

### Command-Centric
Focus on commands as the primary unit of organization.

**Best for:** Independent commands, user-facing CLI tools

**Libraries:**
- **CliFx** - Class per command
- **Spectre.Console.Cli** - Type-based commands with settings
- **CommandLineParser** - Verb-based with attributes

### Method-Centric
Commands are methods, parameters become options.

**Best for:** Rapid development, internal tools, prototypes

**Libraries:**
- **Cocona** - Public methods as commands
- **ConsoleAppFramework** - Lambda or method commands

### Builder-Centric
Fluent APIs for command registration and configuration.

**Best for:** Hosting scenarios, complex configurations, hierarchies

**Libraries:**
- **LionFire.Hosting.CommandLine** - Builder-of-builders pattern
- **System.CommandLine** - Direct builder API

## Performance Comparison

### Startup Time (milliseconds)

```
Manual:               1ms
CAF:                  5ms
System.CommandLine:   10ms
Cocona.Lite:          5ms
CommandLineParser:    8ms
Cocona:               50ms
CliFx:                20ms
Spectre.Console.Cli:  20ms
LionFire:             150ms
```

### Memory Footprint (MB)

```
Manual:               1
Cocona.Lite:          3
CommandLineParser:    5
System.CommandLine:   5
CAF:                  3
CliFx:                10
Spectre.Console.Cli:  10
Cocona:               15
LionFire:             25
```

### Binary Size (KB)

```
Manual:               ~50
Cocona.Lite:          ~50
CAF:                  ~500
System.CommandLine:   ~1000
CommandLineParser:    ~200
CliFx:                ~500
Spectre.Console.Cli:  ~1000
Cocona:               ~1500
LionFire:             ~3000
```

## Use Case Recommendations

### Simple CLI Utilities (1-3 commands)
1. **Cocona** - Fastest to implement
2. **Manual** - No dependencies
3. **CliFx** - If testability important

### Multi-Command Applications (4+ commands)
1. **Cocona** - Clean method-based
2. **CliFx** - Well-organized classes
3. **Spectre.Console.Cli** - Rich UI needed

### Hierarchical Command Structures
1. **LionFire** - If configuration sharing critical
2. **Spectre.Console.Cli** - Good branching support
3. **CliFx** - Manual but clean

### User-Facing Tools
1. **Spectre.Console.Cli** - Beautiful UI
2. **CliFx** - Professional help
3. **CommandLineParser** - Clean help text

### Internal/Dev Tools
1. **Cocona** - Rapid development
2. **ConsoleAppFramework** - High performance
3. **Manual** - Full control

### Service/Long-Running Apps
1. **LionFire** - Native hosting
2. **Cocona** - Hosting support
3. **ConsoleAppFramework** - Background services

### Performance-Critical
1. **ConsoleAppFramework** - Source-gen zero overhead
2. **Manual** - Full optimization control
3. **Cocona.Lite** - Minimal dependencies

### Testable Applications
1. **CliFx** - IConsole abstraction
2. **Spectre.Console.Cli** - Clean separation
3. **Any with DI** - Service injection

## Unique Selling Points

### LionFire.Hosting.CommandLine
- **Unique:** Configuration inheritance in command hierarchy
- **Best for:** Hierarchical commands sharing infrastructure
- **Standout:** Only library with automatic config cascade

### System.CommandLine
- **Unique:** Official Microsoft library
- **Best for:** Foundation for other libraries
- **Standout:** Tab completion, response files

### CommandLineParser
- **Unique:** Most mature attribute-based library
- **Best for:** Declarative lovers
- **Standout:** Zero dependencies, battle-tested

### ConsoleAppFramework
- **Unique:** Source generator for zero overhead
- **Best for:** Performance-critical applications
- **Standout:** Native AOT, fastest startup

### CliFx
- **Unique:** IConsole abstraction for testability
- **Best for:** Test-driven development
- **Standout:** Built-in fake console, no mocking

### Cocona
- **Unique:** Minimal API style for CLI
- **Best for:** Rapid prototyping
- **Standout:** Cocona.Lite variant for zero deps

### Spectre.Console.Cli
- **Unique:** Rich terminal UI integration
- **Best for:** Beautiful user-facing tools
- **Standout:** Progress bars, prompts, tables

### Manual
- **Unique:** Total control
- **Best for:** Learning, constraints
- **Standout:** Zero dependencies, minimal size

## Migration Paths

### When to Migrate

**From Manual → Framework:**
- Adding 3+ commands
- Need help generation
- Want validation

**From Simple → Advanced:**
- Need DI/hosting
- Command hierarchy growing
- Want configuration sharing

**From Framework → Framework:**
- Different priorities (UI, performance, etc.)
- Architecture mismatch
- Feature needs changed

### Migration Difficulty

**Easy Migrations:**
- Manual → Any (just wrap logic)
- System.CommandLine → LionFire (add hosting)
- Cocona.Lite → Cocona (add DI)

**Medium Migrations:**
- CliFx → LionFire (restructure to builders)
- CommandLineParser → Cocona (attributes to methods)
- Any → Spectre.Console.Cli (add UI layer)

**Hard Migrations:**
- LionFire → Simple frameworks (lose hierarchy)
- Any → ConsoleAppFramework (architecture change)
- Between any two (different paradigms)

## Decision Tree

```
What's your PRIMARY need?

├─ Configuration Inheritance
│  └─ LionFire.Hosting.CommandLine
│
├─ Rich Terminal UI
│  └─ Spectre.Console.Cli
│
├─ Maximum Performance
│  ├─ Native AOT needed?
│  │  └─ ConsoleAppFramework
│  └─ Manual Parsing
│
├─ Rapid Development
│  ├─ Need DI?
│  │  └─ Cocona
│  └─ Cocona.Lite
│
├─ Maximum Testability
│  └─ CliFx
│
├─ Declarative/Attributes
│  └─ CommandLineParser
│
├─ Hosting Integration
│  ├─ With hierarchy?
│  │  └─ LionFire.Hosting.CommandLine
│  └─ Cocona
│
└─ Learning/Simple
   └─ Manual Parsing
```

## Common Combinations

### Best of Both Worlds

**LionFire + Spectre.Console:**
- Configuration inheritance + Rich UI
- Inject `IAnsiConsole` via DI
- Use Spectre for output, LionFire for structure

**Cocona + Spectre.Console:**
- Fast development + Beautiful UI
- Simple method commands
- Add Spectre.Console for rich output

**System.CommandLine + Custom Hosting:**
- Standard parsing + Your hosting model
- Build your own LionFire-like layer
- Full control

## Recommendations by Team Size

### Solo Developer
1. **Cocona** - Fast, simple
2. **CliFx** - Well-structured
3. **Manual** - Full control

### Small Team (2-5)
1. **CliFx** - Good organization
2. **Cocona** - Easy to learn
3. **Spectre.Console.Cli** - Nice UI

### Medium Team (6-20)
1. **LionFire** - Shared patterns
2. **Spectre.Console.Cli** - Professional
3. **CliFx** - Testable

### Large Team (20+)
1. **LionFire** - Consistency through inheritance
2. **ConsoleAppFramework** - Performance at scale
3. **Spectre.Console.Cli** - Enterprise UI

## Recommendations by Application Type

### Developer Tools
- **Spectre.Console.Cli** - Beautiful output
- **CliFx** - Well-tested
- **Cocona** - Quick iteration

### System Administration
- **LionFire** - Shared infrastructure
- **Cocona** - Many simple commands
- **CliFx** - Clear structure

### Build Tools
- **ConsoleAppFramework** - Performance
- **Cocona** - Simple tasks
- **Manual** - Minimal overhead

### Data Processing
- **ConsoleAppFramework** - High throughput
- **Manual** - Optimized
- **Cocona.Lite** - Lightweight

### Interactive Applications
- **Spectre.Console.Cli** - Rich prompts
- **CliFx** - IConsole abstraction
- **Cocona** - Quick setup

### Microservices CLIs
- **LionFire** - Hosting integration
- **Cocona** - Built-in hosting
- **ConsoleAppFramework** - Performance

## Final Recommendations

### For Most Applications
**Cocona** - Best balance of simplicity and features

### For Hierarchical Applications
**LionFire.Hosting.CommandLine** - Unique configuration inheritance

### For User-facing Tools
**Spectre.Console.Cli** - Beautiful UI wins users

### For Performance-Critical
**ConsoleAppFramework** - Source-gen optimization

### For Testability
**CliFx** - Built for testing

### For Declarative Lovers
**CommandLineParser** - Proven attributes

### For Learning
**Manual** - Understand the basics

### For Flexibility
**System.CommandLine** - Build your own

## Conclusion

There's no single "best" CLI library. The right choice depends on:

1. **Architecture needs** - Hierarchy? Inheritance? Hosting?
2. **Performance requirements** - Startup time? Memory? AOT?
3. **Development speed** - How fast to build?
4. **User experience** - Rich UI? Help quality?
5. **Maintainability** - Team size? Complexity?
6. **Testing needs** - Unit tests? Integration tests?

**Most versatile:** Cocona (covers 80% of use cases)
**Most specialized:** LionFire (hierarchical apps with shared config)
**Most beautiful:** Spectre.Console.Cli (user-facing tools)
**Most performant:** ConsoleAppFramework (performance-critical)
**Most testable:** CliFx (test-driven development)
**Most mature:** CommandLineParser (battle-tested)

Choose based on your specific needs, not popularity or features you won't use.
