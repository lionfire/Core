# Architecture

**Analysis Date:** 2026-01-18

## Pattern Overview

**Overall:** Layered Monorepo with Toolkit Composition

**Key Characteristics:**
- Three-tier layered architecture: Base -> Toolkits -> Frameworks
- 240+ projects in a monolithic repository with shared infrastructure
- Central Package Management (CPM) via `Directory.Packages.props`
- Dependency Injection-centric design using Microsoft.Extensions.DependencyInjection
- Abstractions-first pattern: most toolkits have separate `*.Abstractions` projects
- Multi-solution organization (`All.sln`, `Core.sln`, `Execution.sln`, `Machine.sln`)

## Layers

**Base Layer (Minimal Dependencies):**
- Purpose: BCL augmentation and foundational patterns with zero or minimal external dependencies
- Location: `src/LionFire.Base/`, `src/LionFire.Flex/`, `src/LionFire.Structures/`
- Contains: Extension methods, concurrent collections, exception types, data structures
- Depends on: .NET BCL only (LionFire.Base has no NuGet dependencies)
- Used by: All other LionFire projects

**Core Toolkit Layer:**
- Purpose: Cross-cutting metadata, DI utilities, and shared abstractions
- Location: `src/LionFire.Core/`, `src/LionFire.Environment/`
- Contains: MultiTyping system, dependency injection extensions, lifecycle interfaces, type registries
- Depends on: Base layer, Microsoft.Extensions.* packages, System.Reactive
- Used by: Most higher-level toolkits

**Toolkit Layer (A la carte, Unopinionated):**
- Purpose: Domain-specific functionality that can be adopted independently
- Location: Multiple `src/LionFire.*` directories organized by domain
- Contains:
  - **Referencing**: `src/LionFire.Referencing*/` - URL/URI handling, custom schemas
  - **Persistence**: `src/LionFire.Persistence*/` - Storage backend abstractions
  - **Serialization**: `src/LionFire.Serialization*/` - Pluggable serialization (HJSON, JSON.NET, YAML)
  - **Hosting**: `src/LionFire.Hosting*/` - Application hosting extensions
  - **Data Async**: `src/LionFire.Data.Async*/` - Async data access patterns
  - **VOS**: `src/LionFire.Vos*/` - Virtual Object System
  - **Assets**: `src/LionFire.Assets*/` - Entity/primary key referencing
- Depends on: Base, Core, other toolkits as needed
- Used by: Framework layer, applications

**Framework Layer (Opinionated):**
- Purpose: Integrated, opinionated application frameworks
- Location: `src/LionFire.Framework/`, `src/LionFire.AspNetCore.Framework/`, `src/LionFire.Vos.Application/`
- Contains: Pre-configured application bootstrapping, best-practice defaults
- Depends on: Multiple toolkits composed together
- Used by: End-user applications

## Data Flow

**Handle-Based Data Access:**

1. Application creates or requests a Handle via `IReference` (URL-like path)
2. Handle resolves to appropriate Persister based on reference scheme
3. Persister selects Serializer based on file extension or content type
4. Serializer transforms data to/from storage format
5. Data returns through Handle to application

```
Application
    |
    v
Handle<T> (IH<T>)         -- Object handle with reference
    |
    v
Reference (IReference)     -- URL-like path (e.g., "vos:///config/app.json")
    |
    v
Persister                  -- Storage backend (Filesystem, DB, HTTP)
    |
    v
Serializer                 -- Format conversion (JSON, HJSON, YAML)
    |
    v
Storage                    -- Physical data store
```

**VOS Virtual Object System Flow:**

1. Application mounts data sources to VOS paths
2. Vob (Virtual Object) nodes created at each path
3. Overlays layer multiple mounts at same path (read fallback, write to top)
4. Hierarchical DI allows services scoped to virtual directories
5. Data accessed through unified VOS reference scheme

**State Management:**
- Reactive patterns via System.Reactive and ReactiveUI
- `INotifyPropertyChanged` for change notification
- `IObservable<T>` streams for async data
- DynamicData for reactive collections
- Options pattern (`IOptions<T>`) for configuration

## Key Abstractions

**IReference:**
- Purpose: URL-like addressing for any data resource
- Examples: `src/LionFire.Referencing.Abstractions/Referencing/IReference.cs`
- Pattern: Scheme + Path (e.g., `file:///path`, `vos:///virtual/path`, `http://api/endpoint`)

**Handle (IH<T>):**
- Purpose: Lazy-loading object wrapper with reference
- Examples: `src/LionFire.Persistence.Handles.Abstractions/`, `src/LionFire.Persistence.Handles/`
- Pattern: IGetter<T> + ISetter<T> with caching, observables, and persistence

**IGetter<T> / ISetter<T> / IValue<T>:**
- Purpose: Async data access with caching, lazy loading, and observability
- Examples: `src/LionFire.Data.Async.Abstractions/Data/Async/`
- Pattern: Stateless (always fetch) vs Lazy (cache-aware) retrieval

**ISerializationStrategy:**
- Purpose: Pluggable format conversion
- Examples: `src/LionFire.Serialization.*/`
- Pattern: Auto-selection by file extension or MIME type

**Vob (Virtual Object):**
- Purpose: Node in VOS virtual tree
- Examples: `src/LionFire.Vos/Vob/`
- Pattern: Hierarchical tree with mounts, overlays, and scoped DI

**IFlex / FlexData:**
- Purpose: Runtime type extension without inheritance
- Examples: `src/LionFire.Flex/FlexObjects/`
- Pattern: Add `FlexData` property to store arbitrary strongly-typed data

**IMultiTypable:**
- Purpose: Dynamic multiple interface presentation
- Examples: `src/LionFire.Core/MultiTyping/`, `src/LionFire.MultiTyping.Abstractions/`
- Pattern: Object presents multiple types via `AsType<T>()` and `IHas<T>`

## Entry Points

**Application Bootstrap (HostApplicationBuilder):**
- Location: `src/LionFire.Hosting/LionFireHostBuilder/`
- Triggers: `new HostApplicationBuilder().LionFire()`
- Responsibilities: Configure services, logging, configuration sources, hosting

**Command-Line Programs:**
- Location: `src/LionFire.Hosting.CommandLine/`
- Triggers: `HostApplicationBuilderProgram.Command<TOptions>()`
- Responsibilities: Parse CLI args, configure commands, run hosted services

**Sample Entry Points:**
- Location: `src/LionFire.Hosting.Samples.Console/Program.cs`
- Pattern: `.LionFire().Run(...)` fluent bootstrap

## Error Handling

**Strategy:** Result types + Exception hierarchy

**Patterns:**
- `IGetResult<T>` / `ISetResult<T>` for async operation outcomes
- `Result<T>` pattern in `src/LionFire.Structures/Results/`
- Domain exceptions: `AlreadySetException`, `NotInitializedException`, `PermanentException`
- `IPotentiallyTemporaryError` for retry decision logic

**Exception Types:**
- `src/LionFire.Base/Exceptions/` - Base exceptions (AlreadyException, DetailException)
- `src/LionFire.Core/Exceptions/` - Core exceptions
- `src/LionFire.Structures/Exceptions/` - Structure-related exceptions

## Cross-Cutting Concerns

**Logging:**
- Framework: Serilog with multiple sinks (Console, File, Loki, OpenTelemetry)
- Configuration: `src/LionFire.Hosting/Logging/`
- Extension: `L.Get<T>()` for static logger access

**Validation:**
- Framework: `IValidatable`, `IValidator<T>` interfaces
- Location: `src/LionFire.Validation/`, `src/LionFire.Core/Validation/`
- Pattern: `Validate()` returns `ValidationResult` with error collection

**Authentication:**
- Integration: OpenIddict client in `src/LionFire.Auth.OpenIddict.Client/`
- Abstraction: `src/LionFire.Identity/`

**Telemetry:**
- Framework: OpenTelemetry API
- Location: `src/LionFire.Telemetry/`
- Integration: `src/LionFire.Vos/` uses OpenTelemetry.Api

**Configuration:**
- Pattern: Microsoft.Extensions.Configuration + Options pattern
- Location: `src/LionFire.Extensions.Configuration/`, `src/LionFire.Hosting/Configuration/`
- Sources: JSON, HJSON, environment variables, command line

---

*Architecture analysis: 2026-01-18*
