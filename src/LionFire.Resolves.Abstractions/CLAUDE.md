# LionFire.Data.Abstractions (Resolves.Abstractions)

## Overview

**LionFire.Data.Abstractions** (project name: LionFire.Resolves.Abstractions) defines synchronous data access patterns and result types for LionFire's data layer. This is the foundation for the async patterns in LionFire.Data.Async.Abstractions.

**Layer**: Base Abstractions (Synchronous Data Patterns)
**Target**: .NET 9.0
**Root Namespace**: `LionFire`
**Project Name**: `LionFire.Resolves.Abstractions` → Package: `LionFire.Data.Abstractions`

## Key Dependencies

### NuGet Packages
- **MorseCode.ITask** - Covariant `ITask<T>` interface

### LionFire Dependencies
- **LionFire.Structures** - Collection types and data structures

## Core Concepts

This library provides the foundational synchronous data access abstractions that form the basis for LionFire's data access layer.

### 1. Result Types

**Location**: `Results/`

#### IGetResult

Result type for read operations.

```csharp
// Marker interface for Get results
public interface IGetResult : ITransferResult { }

// Generic result with value
public interface IGetResult<out TValue> : IGetResult, IValueResult<TValue>, IHasValueResult, IErrorResult
{
}
```

**Key Properties** (via interfaces):
- `TValue Value` - Retrieved value
- `bool HasValue` - Whether value exists
- `bool IsSuccess` - Operation succeeded
- `TransferResultFlags Flags` - Result flags (NotFound, Error, etc.)
- `Exception? Error` - Exception if failed

**Extension Methods:**
```csharp
public static class IGetResultX
{
    public static string ToDebugString<TValue>(this IGetResult<TValue> getResult);
    public static Exception ToException<TValue>(this IGetResult<TValue> result);
}
```

#### ISetResult

Result type for write operations.

```csharp
// Marker interface for Set results
public interface ISetResult : ITransferResult, IErrorResult { }

// Generic result with value
public interface ISetResult<out TValue> : ISetResult, IValueResult<TValue>, IHasValueResult
{
}
```

#### IExistsResult

Result type for existence checks.

```csharp
public interface IExistsResult : IResult
{
    bool? Exists { get; }
}
```

#### IHasValueResult

Common interface for results with values.

```csharp
public interface IHasValueResult
{
    bool HasValue { get; }
}
```

#### IValueResult

Generic value container.

```csharp
public interface IValueResult<out TValue>
{
    TValue Value { get; }
}
```

### 2. Transfer Results

**Location**: `Transfer/Results/`

#### ITransferResult

Base interface for data transfer results.

```csharp
public interface ITransferResult : IResult
{
    TransferResultFlags Flags { get; }
}
```

#### TransferResultFlags

```csharp
[Flags]
public enum TransferResultFlags
{
    None = 0,
    Success = 1 << 0,
    NotFound = 1 << 1,
    Error = 1 << 2,
    Timeout = 1 << 3,
    Cancelled = 1 << 4,
    // ... additional flags
}
```

#### IErrorResult

```csharp
public interface IErrorResult
{
    bool IsSuccess { get; }
    Exception? Error { get; }
}
```

### 3. Persistence Abstractions

**Location**: `Persistence/`

#### IODirection

```csharp
[Flags]
public enum IODirection
{
    None = 0,
    Read = 1 << 0,
    Write = 1 << 1,
    ReadWrite = Read | Write
}
```

#### ITransferX

Generic transfer interface.

```csharp
public interface ITransferX
{
    IODirection Direction { get; }
}
```

#### IPersistenceSnapshot

```csharp
public interface IPersistenceSnapshot
{
    DateTimeOffset Timestamp { get; }
    PersistenceFlags Flags { get; }
}
```

#### PersistenceFlags

```csharp
[Flags]
public enum PersistenceFlags
{
    None = 0,
    Loaded = 1 << 0,
    Modified = 1 << 1,
    Persisted = 1 << 2,
    Deleted = 1 << 3,
    // ... additional flags
}
```

### 4. Resolver Interfaces

**Location**: `Resolvers/`

#### IAsyncResolver

```csharp
public interface IAsyncResolver<TKey, TValue>
{
    Task<TValue?> ResolveAsync(TKey key);
}
```

#### IGetterSync

Synchronous getter interface.

```csharp
public interface IGetterSync<out TValue>
{
    TValue Get();
}
```

### 5. Resolve Abstractions

**Location**: `Resolves/`

#### ICanResolveToDefault

```csharp
public interface ICanResolveToDefault<T>
{
    T? ResolveToDefault();
}
```

#### ResolveException

```csharp
public class ResolveException : Exception
{
    public ResolveException(string message) : base(message) { }
    public ResolveException(string message, Exception innerException)
        : base(message, innerException) { }
}
```

### 6. Lazy Get Result

**Location**: `Resolves/Results/`

```csharp
public interface ILazyGetResult<TValue>
{
    IGetResult<TValue> GetResult { get; }
    bool IsResultAvailable { get; }
}
```

## Directory Structure

```
src/LionFire.Resolves.Abstractions/
├── IO/
│   └── IODirection.cs                # Read/Write direction enum
├── Persistence/
│   ├── IPersistenceSnapshot.cs       # Persistence state snapshot
│   ├── ITransferX.cs                 # Transfer interface
│   └── PersistenceFlags.cs           # Persistence flags enum
├── Resolvers/
│   ├── IAsyncResolver.cs             # Async resolver interface
│   └── IGetterSync.cs                # Sync getter interface
├── Resolves/
│   ├── ICanResolveToDefault.cs       # Default resolution
│   ├── ResolveException.cs           # Resolve exception
│   └── Results/
│       ├── ILazyGetResult.cs         # Lazy get result
│       └── IValueResult.cs           # Value container
├── Results/
│   ├── IHasValueResult.cs            # Has value interface
│   ├── Read/
│   │   ├── IExistsResult.cs          # Exists check result
│   │   └── IGetResult.cs             # Get result
│   └── Write/
│       ├── ISetResult.cs             # Set result
│       └── SetResult.cs              # Set result implementation
└── Transfer/
    ├── Exceptions/
    │   ├── RetrieveException.cs      # Retrieve exception
    │   └── TransferException.cs      # Transfer exception
    └── Results/
        └── IPersistenceResult.cs     # Persistence result
```

## Design Patterns

### Result Pattern

Comprehensive result types for data operations:

```csharp
public IGetResult<User> GetUser(int id)
{
    try
    {
        var user = database.FindUser(id);

        if (user == null)
        {
            return GetResult<User>.NotFound;
        }

        return GetResult<User>.Success(user);
    }
    catch (Exception ex)
    {
        return GetResult<User>.Error(ex);
    }
}

// Usage
var result = GetUser(42);

if (result.IsSuccess && result.HasValue)
{
    Console.WriteLine($"User: {result.Value.Name}");
}
else if (result.Flags.HasFlag(TransferResultFlags.NotFound))
{
    Console.WriteLine("User not found");
}
else if (result.Error != null)
{
    Console.WriteLine($"Error: {result.Error.Message}");
}
```

### Synchronous Data Access

```csharp
public class ConfigGetter : IGetterSync<AppConfig>
{
    private readonly string filePath;

    public ConfigGetter(string filePath)
    {
        this.filePath = filePath;
    }

    public AppConfig Get()
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Config not found: {filePath}");

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<AppConfig>(json)
            ?? throw new InvalidOperationException("Failed to deserialize config");
    }
}
```

## Common Usage Patterns

### Pattern 1: Result Handling

```csharp
public void ProcessGetResult<T>(IGetResult<T> result)
{
    switch (result.Flags)
    {
        case var flags when flags.HasFlag(TransferResultFlags.Success) && result.HasValue:
            ProcessValue(result.Value);
            break;

        case var flags when flags.HasFlag(TransferResultFlags.NotFound):
            HandleNotFound();
            break;

        case var flags when flags.HasFlag(TransferResultFlags.Error):
            HandleError(result.Error);
            break;

        case var flags when flags.HasFlag(TransferResultFlags.Timeout):
            HandleTimeout();
            break;

        default:
            HandleUnknown();
            break;
    }
}
```

### Pattern 2: Existence Checking

```csharp
public class FileExistenceChecker
{
    public IExistsResult CheckExists(string path)
    {
        try
        {
            bool exists = File.Exists(path);
            return new ExistsResult { Exists = exists };
        }
        catch (Exception ex)
        {
            return new ExistsResult { Exists = null }; // Indeterminate
        }
    }
}
```

### Pattern 3: Error Propagation

```csharp
public IGetResult<ProcessedData> ProcessData(int id)
{
    var getRawData = GetRawData(id);

    if (!getRawData.IsSuccess)
    {
        // Propagate error
        return GetResult<ProcessedData>.FromError(getRawData.Error);
    }

    try
    {
        var processed = Transform(getRawData.Value);
        return GetResult<ProcessedData>.Success(processed);
    }
    catch (Exception ex)
    {
        return GetResult<ProcessedData>.Error(ex);
    }
}
```

## Integration with LionFire.Data.Async.Abstractions

This library provides the synchronous foundation:

**LionFire.Data.Abstractions (Sync):**
- `IGetResult<T>` - Synchronous get result
- `ISetResult<T>` - Synchronous set result
- `IGetterSync<T>` - Synchronous getter

**LionFire.Data.Async.Abstractions (Async):**
- `IGetResult<T>` - Async get result (same interface)
- `IStatelessGetter<T>` - Async stateless getter
- `IGetter<T>` - Async getter with caching

Both share the same result interfaces for consistency.

## Testing Considerations

### Testing Result Types

```csharp
[Fact]
public void GetResult_Success_HasValue()
{
    var result = GetResult<int>.Success(42);

    Assert.True(result.IsSuccess);
    Assert.True(result.HasValue);
    Assert.Equal(42, result.Value);
    Assert.True(result.Flags.HasFlag(TransferResultFlags.Success));
}

[Fact]
public void GetResult_NotFound_NoValue()
{
    var result = GetResult<int>.NotFound;

    Assert.False(result.IsSuccess);
    Assert.False(result.HasValue);
    Assert.True(result.Flags.HasFlag(TransferResultFlags.NotFound));
}

[Fact]
public void GetResult_Error_HasException()
{
    var exception = new InvalidOperationException("Test error");
    var result = GetResult<int>.Error(exception);

    Assert.False(result.IsSuccess);
    Assert.Equal(exception, result.Error);
    Assert.True(result.Flags.HasFlag(TransferResultFlags.Error));
}
```

### Mocking Getters

```csharp
var mockGetter = new Mock<IGetterSync<AppConfig>>();
mockGetter.Setup(x => x.Get())
    .Returns(new AppConfig { AppName = "Test" });

var config = mockGetter.Object.Get();

Assert.Equal("Test", config.AppName);
```

## Related Projects

- **LionFire.Data.Async.Abstractions** - Async data patterns ([CLAUDE.md](../LionFire.Data.Async.Abstractions/CLAUDE.md))
- **LionFire.Structures** - Data structures ([CLAUDE.md](../LionFire.Structures/CLAUDE.md))
- **LionFire.Persistence.Abstractions** - Persistence abstractions
- **LionFire.Resolves** (Data.Async) - Concrete implementations

## Summary

**LionFire.Data.Abstractions** provides synchronous data access foundations:

- **Result Types**: `IGetResult<T>`, `ISetResult<T>`, `IExistsResult`
- **Transfer Results**: Comprehensive flags and error handling
- **IODirection**: Read/Write/ReadWrite direction enum
- **Persistence Flags**: Track persistence state
- **Sync Getters**: `IGetterSync<T>` for synchronous data access
- **Resolvers**: `IAsyncResolver<TKey, TValue>` for async resolution
- **Exceptions**: `ResolveException`, `TransferException`, `RetrieveException`

**Key Strengths:**
- Lightweight (minimal dependencies)
- Comprehensive result types
- Flag-based status tracking
- Error propagation support
- Synchronous foundation for async patterns

**Use When:**
- Need synchronous data access abstractions
- Require comprehensive result types
- Want flag-based status tracking
- Building data access layers

**Typical Use Cases:**
- Synchronous data access
- Result handling and propagation
- Existence checking
- Error tracking and reporting
- Foundation for async data patterns

**Note**: This library is typically used as a foundation for **LionFire.Data.Async.Abstractions** which provides the async variants of these patterns. Both share the same result interfaces for consistency.
