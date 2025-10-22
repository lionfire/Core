# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

This is the **LionFire.Core** monorepo - a collection of mini-frameworks and toolkits for .NET 9.0+ development. The project follows a layered architecture philosophy from minimal base utilities to opinionated frameworks.

**Note**: This is part of a larger development environment at `/mnt/c/src/core` on a Windows WSL2 setup.

## Important: Windows WSL2 Environment

This codebase is on a Windows filesystem mounted in WSL2. **Always use `dotnet-win` instead of `dotnet`** for all .NET operations:

```bash
dotnet-win build
dotnet-win restore
dotnet-win test
dotnet-win run
```

Use `git_win` for git operations that require authentication (e.g., `git_win push`).

## Project Structure

The repository is organized into multiple solutions:
- **Core.sln** - Primary solution for core libraries
- **All.sln** - Comprehensive solution including all projects
- **Execution.sln** - Execution-related projects
- **Machine.sln** - Machine-specific projects

### Key Directories

- `src/` - All source code organized by component
- `test/` - Test projects
- `docs/` - Architecture documentation (especially VOS system in `docs/data/`)
- `attic/` - Deprecated/archived code
- `build/` - Build scripts and tools

## Build System

### Central Package Management

This repository uses **Central Package Management** (CPM):
- Package versions are centrally defined in `Directory.Packages.props`
- Individual `.csproj` files reference packages without version numbers
- To update a package version, edit `Directory.Packages.props` only

### Build Configuration

- Uses `Directory.Build.props` for common MSBuild settings
- Current version: `7.0.0-alpha`
- Uses latest C# language version
- Generates reference assemblies
- Portable PDB symbols with embedded sources
- File-scoped namespaces enabled (`.editorconfig`)

### Common Build Commands

```bash
# Restore packages
dotnet-win restore

# Build the main solution
dotnet-win build Core.sln

# Build specific project
dotnet-win build src/LionFire.Base/LionFire.Base.csproj

# Run tests
dotnet-win test test/LionFire.Core.Tests/LionFire.Core.Tests.csproj

# Build in Release mode (auto-generates NuGet packages)
dotnet-win build Core.sln -c Release
```

## Architecture Layers

### 1. Base Layer (Minimal Dependencies)

**LionFire.Base** - BCL augmentation with no external dependencies
- Collections, DateTime utilities, reflection helpers
- Extension methods for common patterns
- Located in: `src/LionFire.Base/`

**LionFire.Flex** - Strongly-typed dynamic object pattern
- Adds `FlexData` property for runtime type extension
- Depends only on Base

**LionFire.Structures** - Data structures and collection types
- Depends on Base and Flex

### 2. Core Toolkits (Unopinionated, A la carte)

**Hosting** (`src/LionFire.Hosting*/`)
- Extensions to Microsoft.Extensions.Hosting
- Wraps IHostBuilder and HostApplicationBuilder
- Multiple sub-projects for different hosting scenarios

**Serialization** (`src/LionFire.Serialization*/`)
- Open-ended serialization framework
- Multiple serializer implementations (JSON.NET, etc.)

**Referencing** (`src/LionFire.Referencing*/`)
- URL/URI handling and custom schema support
- Foundation for the Handle and Persistence layers

**Handles** (`src/LionFire.Data.Async*/`)
- Object handles with reference (URL) support
- Can load/save/observe data through references

**Persistence** (`src/LionFire.Persistence/`)
- Open-ended persistence framework
- Supports multiple storage backends

**Virtual Object System (VOS)** (`src/LionFire.Vos*/`)
- Mount filesystems, databases, zip files into a virtual filesystem
- Overlay multiple mounts for advanced scenarios
- Hierarchical dependency injection via virtual directories
- **Critical**: Extensively documented in `docs/data/` directory

**Assets/Ided** (`src/LionFire.Assets*/`)
- Simplified entity referencing via primary keys
- Simpler alternative to full VOS for database-like access patterns

**Instantiating** (`src/LionFire.*.Instantiating/`)
- Template-based object instantiation with parameters

### 3. Framework Layer (Opinionated)

**LionFire.Core.Extras** - General-purpose framework utilities
- Less commonly used features
- May depend on multiple toolkit abstractions

**LionFire.AspNetCore.Framework** - Opinionated ASP.NET Core setup
- Best-practice ASP.NET Core application structure

**LionFire.Vos.VosApp** - VOS application framework
- Default capabilities for VOS-based apps (configuration, etc.)

**LionFire.Framework** - Complete integrated framework
- Brings all toolkits together
- Goal: `new HostApplicationBuilder().LionFire()` for full-featured apps

## Working with the Codebase

### Nullability Annotations

The codebase is actively being updated for nullable reference types:
- Recent effort reduced warnings from 40 to 23 (see `nullability-status.md`)
- When adding nullability annotations, document complex cases in `nullability-review.md`
- Simple cases (field initialization, obvious null checks) can be fixed directly

### Testing

Test projects follow the naming pattern: `LionFire.<Component>.Tests`
- Located in `test/` directory
- Use xUnit or MSTest frameworks

```bash
# Run all tests
dotnet-win test

# Run specific test project
dotnet-win test test/LionFire.Core.Tests/LionFire.Core.Tests.csproj

# Run tests with detailed output
dotnet-win test --logger "console;verbosity=detailed"
```

### Adding New Projects

When adding new projects:
1. Place in appropriate `src/` subdirectory
2. Do NOT specify package versions in `.csproj` (use CPM)
3. Follow naming convention: `LionFire.<Component>[.<SubComponent>]`
4. Ensure `Directory.Build.props` settings are inherited
5. Add to appropriate solution file(s)

### Package References

To add a package to a project:

```xml
<!-- In .csproj - NO version attribute -->
<ItemGroup>
  <PackageReference Include="PackageName" />
</ItemGroup>
```

To define or update package version:

```xml
<!-- In Directory.Packages.props at repo root -->
<PackageVersion Include="PackageName" Version="x.y.z" />
```

## Key Concepts

### VOS (Virtual Object System)

The most complex and powerful toolkit. Essential reading for VOS work:
- Start with: `docs/data/README.md`
- Architecture: `docs/data/vos-architecture.md`
- Core concepts: `docs/data/vos-core-concepts.md`
- Examples: `docs/data/vos-examples.md`

Key VOS concepts:
- **Vobs**: Virtual objects (nodes in the virtual tree)
- **References**: Paths to locate objects
- **Mounts**: Connect data sources to the virtual tree
- **Handles**: Read/write access to data
- **Overlays**: Multiple mounts at the same path for layered data

### Multi-Typing

A cross-cutting pattern used throughout the codebase:
- Objects can present multiple type interfaces dynamically
- Enables flexible composition and adaptation patterns
- See `src/LionFire.Core.Extras/MultiTyping/` for implementation

### Dependency Injection

The codebase heavily uses Microsoft.Extensions.DependencyInjection:
- Most components are registered via extension methods on `IServiceCollection`
- Hosting extensions follow pattern: `AddLionFire<Component>()`
- Service locator pattern available but dependency injection preferred

## Solution/Project Organization

The repository contains ~100+ projects organized by functionality. Major areas:

- **Applications**: `LionFire.Applications*` - Application framework
- **AspNetCore**: `LionFire.AspNetCore*` - Web framework extensions
- **Assets**: `LionFire.Assets*` - Entity/asset management
- **Auth**: `LionFire.Auth*` - Authentication integrations
- **Avalon**: `LionFire.Avalon*` - WPF/Avalon UI components
- **Blazor**: `LionFire.Blazor*` - Blazor component libraries
- **Data**: `LionFire.Data*` - Data access patterns
- **Execution**: `LionFire.Execution*` - Execution engines and state machines
- **Hosting**: `LionFire.Hosting*` - Application hosting
- **Persistence**: `LionFire.Persistence*` - Storage backends
- **Serialization**: `LionFire.Serialization*` - Serialization providers
- **Vos**: `LionFire.Vos*` - Virtual Object System

## Development Philosophy

Per README.md:

**Layered Approach:**
1. **Base layer**: Minimal dependencies, BCL augmentation
2. **Toolkits**: Unopinionated, a la carte, may depend on each other
3. **Framework**: Opinionated integration of toolkits

**Design Goals:**
- Decoupled but sharing common interfaces
- Raise the lowest common denominator above BCL
- Enable patterns like multi-typing across components
- Most toolkits are exploratory; some will be promoted to first-class libraries

## Common Patterns

### Extension Methods

The codebase makes heavy use of extension methods for discoverability:
- Collection extensions in `LionFire.Base/Collections/`
- Type extensions in `LionFire.Base/Types/`
- String extensions in `LionFire.Base/Text/`

### Options Pattern

Configuration follows ASP.NET Core options pattern:
- Options classes typically suffixed with `Options`
- Configured via `IServiceCollection.Configure<TOptions>()`
- Validated at startup when possible

### Async/Await

- Async methods suffixed with `Async`
- `IAsyncDisposable` preferred for async cleanup
- Extensive use of `ValueTask<T>` for performance

## SDK and Tooling

- **Target**: .NET 9.0.107 SDK (see `global.json`)
- **Roll forward**: Latest minor version allowed
- **Language**: Latest C# version
- **Platform**: Primarily tested on Windows/WSL2

## Documentation

For detailed architectural documentation, especially on the VOS system, see:
- `docs/data/` - VOS documentation and examples
- `docs/architecture/` - Architectural analysis and recommendations
- Source code comments - Most complex areas are well-documented inline

## Current State

Per README.md:
> "This is only really intended to be usable to me, but feel free to mine the codebase for interesting parts"

The project is under active development with ongoing efforts around:
- Nullable reference type annotations (see `nullability-status.md`)
- Framework consolidation
- Documentation improvements
