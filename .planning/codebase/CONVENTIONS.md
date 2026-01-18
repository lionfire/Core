# Coding Conventions

**Analysis Date:** 2026-01-18

## Naming Patterns

**Files:**
- PascalCase for class files: `FilesystemPersister.cs`, `LionFireHostBuilder.cs`
- Interface files prefixed with `I`: `IMultiTypable.cs`, `ITypeResolver.cs`
- Extension method classes suffixed with `Extensions` or `X`: `StringExtensions.cs`, `HostingX.cs`
- Attribute classes suffixed with `Attribute`: `CodeAttribute.cs`, `IdempotentAttribute.cs`

**Functions/Methods:**
- PascalCase for public methods: `GetOrAdd()`, `TryGetValue()`, `ConfigureServices()`
- Async methods suffixed with `Async`: `StartAsync()`, `StopAsync()`, `ReadStream()` (some older code omits suffix)
- Try pattern for non-throwing variants: `TryAdd()`, `TryGetValue()`, `TryResolve()`
- Extension methods use verb-first naming: `InsertSpaceBeforeCaps()`, `ToKebabCase()`

**Variables:**
- camelCase for local variables and parameters: `serviceProvider`, `configurationBuilder`
- Private fields with underscore prefix in some areas: `_name` (inconsistent - some use `name` directly)
- Static readonly with PascalCase: `Meter`, `ReadAllTextC`, `Current`

**Types:**
- Interfaces prefixed with `I`: `IMultiTypable`, `ILionFireHostBuilder`, `IServiceCollectionEx`
- Generic type parameters: `T`, `TKey`, `TValue`, `TReference`
- Options/Settings classes suffixed with `Options`: `FilesystemPersisterOptions`, `LionFireHostBuilderOptions`

**Namespaces:**
- Match folder structure: `LionFire.Persistence.Filesystem`, `LionFire.Hosting`
- Use file-scoped namespaces (enforced by `.editorconfig`)
- Test namespaces use underscore suffix pattern: `TypeNameRegistry_`, `FilesystemPersister_`

## Code Style

**Formatting:**
- File-scoped namespaces: `namespace LionFire.Hosting;`
- Configured in `.editorconfig`: `csharp_style_namespace_declarations = file_scoped`
- No dedicated formatter tool detected (relies on IDE defaults)

**Linting:**
- Naming style warnings silenced in `src/LionFire.Core/.editorconfig`: `dotnet_diagnostic.IDE1006.severity = silent`
- No global analyzer packages detected

**Nullable Reference Types:**
- Actively being adopted with `#nullable enable` directives
- Ongoing effort documented in project's `nullability-status.md`
- Use `?` suffix for nullable types: `IServiceProvider?`, `string?`

## Import Organization

**Order:**
1. `#nullable enable` directive (when used)
2. System namespaces: `System`, `System.Collections.Generic`, `System.Linq`, `System.Threading.Tasks`
3. Microsoft namespaces: `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.Hosting`
4. Third-party namespaces: `Polly`, `Polly.Registry`
5. LionFire namespaces: `LionFire.Dependencies`, `LionFire.Hosting`, `LionFire.Persistence`

**Path Aliases:**
- No global using aliases detected
- `LionFire.Base` defines global usings in `GlobalUsings.cs`:
  ```csharp
  global using System;
  global using System.Collections.Generic;
  global using System.Linq;
  global using System.Threading.Tasks;
  ```

## Error Handling

**Patterns:**
- Domain-specific exceptions in `LionFire.Base/Exceptions/`:
  - `AlreadySetException` - Property can only be set once
  - `NotInitializedException` - Component not initialized
  - `DetailException` - Exception with structured detail object
  - `PermanentException` - Implements `IPotentiallyTemporaryError`

- Exception handling in async code:
  ```csharp
  catch (IOException ex)
  {
      Debug.WriteLine(ex);
      OpenReadStreamExC.IncrementWithContext();
      throw;
  }
  ```

- Use `Assert.ThrowsAsync<T>()` for expected exceptions in tests:
  ```csharp
  await Assert.ThrowsAsync<NotFoundException>(() =>
      ServiceLocator.Get<FilesystemPersister>().Update(path.ToFileReference(), testContents2));
  ```

**Custom Exception Types:**
- Located in `src/LionFire.Base/Exceptions/`
- Inherit from standard `Exception` class
- Support nullable reference types

## Logging

**Framework:** Microsoft.Extensions.Logging

**Patterns:**
- Static logger access via `Log.Get<T>()`:
  ```csharp
  private static readonly Microsoft.Extensions.Logging.ILogger l = Log.Get<FilesystemPersister>();
  ```

- EventId constants for structured logging:
  ```csharp
  public static class FilesystemPersisterEventIds
  {
      public const int ReadAllText = 1000;
      public const int ReadAllBytes = 1010;
      public const int OpenStream = 2000;
  }
  ```

- Log with structured parameters:
  ```csharp
  l.LogInformation(FilesystemPersisterEventIds.ReadAllText, "Reading text: {Path}", path);
  l.LogInformation("Writing {Length} bytes to {Path}", bytes.Length, path);
  ```

**Metrics:**
- OpenTelemetry `Meter` and `Counter` usage:
  ```csharp
  private static readonly Meter Meter = new("LionFire.Persistence.Filesystem", "1.0");
  private static readonly Counter<long> ReadAllTextC = Meter.CreateCounter<long>("ReadAllText");
  ```

## Comments

**When to Comment:**
- XML doc comments on public APIs
- TODO/FIXME comments for incomplete work
- Region markers for code organization: `#region Static`, `#region Dependencies`

**JSDoc/TSDoc:**
- Not applicable (C# codebase)

**XML Documentation:**
- Summary comments on public classes and interfaces:
  ```csharp
  /// <summary>
  /// Persists using NativeFilesystem implementation of IVirtualFilesystem
  /// </summary>
  public class FilesystemPersister : ...
  ```

## Function Design

**Size:**
- Methods generally small and focused
- Larger methods use region markers for organization

**Parameters:**
- Use default values: `TValue defaultValue = default(TValue)`
- Nullable parameters for optional dependencies: `string name = null`
- Factory functions: `Func<TKey, TValue> factory`

**Return Values:**
- Use `Task<T>` for async operations
- Use `ValueTask<T>` for performance-critical paths (mentioned in CLAUDE.md)
- Return `bool` for Try methods success indication
- Return `null` for "not found" scenarios

## Module Design

**Exports:**
- Public classes and interfaces in root namespace
- Extension methods in static classes suffixed with `Extensions` or `X`
- Internal implementation details in nested namespaces

**Barrel Files:**
- Not used (C# pattern)

**Service Registration:**
- Extension methods on `IServiceCollection`:
  ```csharp
  public static IServiceCollection AddMyService(this IServiceCollection services)
  ```
- Naming convention: `Add<Feature>()`, `Use<Feature>()`, `Configure<Feature>()`

## Dependency Injection Patterns

**Constructor Injection:**
```csharp
public FilesystemPersister(
    ISerializationProvider serializationProvider,
    IOptionsMonitor<FilesystemPersisterOptions> optionsMonitor,
    IPersistenceConventions itemKindIdentifier,
    SerializationOptions serializationOptions,
    IServiceProvider serviceProvider)
```

**Options Pattern:**
- Use `IOptionsMonitor<T>` for hot-reloading
- Use `IOptions<T>` for static configuration
- Options classes follow `<Feature>Options` naming

**Dependency Attributes:**
- `[Dependency]` attribute for property injection (in LionFire.Core)
- `[SetOnce]` attribute for immutable properties

## Async Patterns

**Conventions:**
- Prefer `Task.Run()` for CPU-bound work
- Use `ConfigureAwait(false)` in library code:
  ```csharp
  return await FileExists(fsPath).ConfigureAwait(false) ? true : await DirectoryExists(fsPath).ConfigureAwait(false);
  ```
- Support `CancellationToken` in async methods

## Testing Host Pattern

**Common Pattern:**
- Tests create hosts for integration testing:
  ```csharp
  await new HostBuilder()
      .ConfigureServices(services => { ... })
      .RunAsync(serviceProvider => { ... });
  ```

- Test host utilities in `LionFire.Persistence.Testing`:
  ```csharp
  await NewtonsoftJsonFilesystemTestHost.Create().RunAsync(async () => { ... });
  ```

---

*Convention analysis: 2026-01-18*
