# LionFire.Persistence (LionFire.Data.Persisters)

## Overview

**LionFire.Persistence** provides an open-ended persistence framework for LionFire applications. It defines abstractions for serializers, persisters, and persistence contexts, allowing multiple storage backends (filesystems, databases, HTTP APIs) to coexist and be accessed through a unified interface.

**Layer**: Core Toolkit (Persistence Framework)
**Target**: .NET 9.0
**Root Namespace**: `LionFire`
**Project Name**: `LionFire.Data.Persisters.csproj` → Package: `LionFire.Persistence`

## Key Dependencies

### NuGet Packages
- **Microsoft.Extensions.DependencyInjection** - DI container
- **Microsoft.Extensions.Resilience** - Retry/circuit breaker policies

### LionFire Dependencies
- **LionFire.Applications.Abstractions** - Application abstractions
- **LionFire.Core** - Core metadata and DI
- **LionFire.Data.Async.Abstractions** - Async data patterns
- **LionFire.Extensions.Hosting** - Hosting extensions
- **LionFire.Hosting** - Application hosting
- **LionFire.Persistence.Abstractions** - Persistence abstractions
- **LionFire.Persistence.Handles** - Handle implementations
- **LionFire.Referencing.Abstractions** - Reference system
- **LionFire.Resolves** (Data.Async) - Data resolution
- **LionFire.Structures** - Data structures

## Core Concepts

### Persistence Architecture

```
Application
    ↓
Handle (IH<T>)              - Object handle with Get/Set
    ↓
Persister                   - Strategy for a storage type
    ↓
Serializer                  - Format converter (JSON, HJSON, XML)
    ↓
Storage Backend             - Filesystem, DB, HTTP, etc.
```

### 1. Persistence Operation

**PersistenceOperation** tracks context and metadata for persistence operations.

```csharp
public class PersistenceOperation
{
    public LionSerializeContext SerializeContext { get; set; }
    public PersistenceDirection Direction { get; set; }  // Read or Write
    public IReference? Reference { get; set; }
    public Type? Type { get; set; }
}
```

**LionSerializeContext:**
```csharp
[Flags]
public enum LionSerializeContext
{
    None = 0,
    Persistence = 1 << 0,    // Saving to disk/database
    Network = 1 << 1,        // Network transmission
    Copy = 1 << 2,           // Object cloning
}
```

**PersistenceDirection:**
```csharp
public enum PersistenceDirection
{
    Read,
    Write
}
```

### 2. Serialization Framework

#### ISerializationStrategy

Base interface for serializers.

```csharp
public interface ISerializationStrategy
{
    SerializationFormat DefaultFormat { get; }
    IEnumerable<SerializationFormat>? SupportedInputs { get; }
    SerializationFlags SupportedCapabilities { get; }
}
```

#### SerializationFormat

Describes a serialization format.

```csharp
public class SerializationFormat
{
    public string Extension { get; set; }          // "json", "hjson", "xml"
    public string Name { get; set; }               // "JSON", "HJSON", "XML"
    public string MimeType { get; set; }           // "application/json"
    public string? Description { get; set; }
}
```

#### SerializationFlags

```csharp
[Flags]
public enum SerializationFlags
{
    None = 0,
    Text = 1 << 0,           // Text-based
    Binary = 1 << 1,         // Binary format
    HumanReadable = 1 << 2,  // Human-readable
    Serialize = 1 << 3,      // Supports serialization
    Deserialize = 1 << 4,    // Supports deserialization
}
```

#### SerializerBase

Base class for implementing serializers.

```csharp
public abstract class SerializerBase<TSerializer> : ISerializationStrategy
    where TSerializer : SerializerBase<TSerializer>
{
    public abstract SerializationFormat DefaultFormat { get; }
    public abstract SerializationFlags SupportedCapabilities { get; }

    // Serialize to string
    public abstract (string String, SerializationResult Result) ToString(
        object obj,
        Lazy<PersistenceOperation>? operation = null,
        PersistenceContext? context = null);

    // Deserialize from string
    public abstract DeserializationResult<T> ToObject<T>(
        string str,
        Lazy<PersistenceOperation>? operation = null,
        PersistenceContext? context = null);
}
```

**Usage:**
```csharp
public class JsonSerializer : SerializerBase<JsonSerializer>
{
    public override SerializationFormat DefaultFormat =>
        new SerializationFormat("json", "JSON", "application/json");

    public override SerializationFlags SupportedCapabilities =>
        SerializationFlags.Text |
        SerializationFlags.HumanReadable |
        SerializationFlags.Serialize |
        SerializationFlags.Deserialize;

    public override (string, SerializationResult) ToString(object obj, ...)
    {
        try
        {
            var json = JsonConvert.SerializeObject(obj);
            return (json, SerializationResult.Success);
        }
        catch (Exception ex)
        {
            return (string.Empty, SerializationResult.Failure(ex));
        }
    }

    public override DeserializationResult<T> ToObject<T>(string str, ...)
    {
        try
        {
            var obj = JsonConvert.DeserializeObject<T>(str);
            return DeserializationResult<T>.Success(obj);
        }
        catch (Exception ex)
        {
            return DeserializationResult<T>.Failure(ex);
        }
    }
}
```

### 3. Persistence Context

```csharp
public class PersistenceContext
{
    public IServiceProvider? ServiceProvider { get; set; }
    public Dictionary<string, object> Metadata { get; } = new();
}
```

### 4. Persistence Result Types

#### SerializationResult

```csharp
public class SerializationResult
{
    public bool IsSuccess { get; init; }
    public Exception? Error { get; init; }

    public static SerializationResult Success { get; } =
        new() { IsSuccess = true };

    public static SerializationResult Failure(Exception ex) =>
        new() { IsSuccess = false, Error = ex };
}
```

#### DeserializationResult

```csharp
public class DeserializationResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public Exception? Error { get; init; }

    public static DeserializationResult<T> Success(T value) =>
        new() { IsSuccess = true, Value = value };

    public static DeserializationResult<T> Failure(Exception ex) =>
        new() { IsSuccess = false, Error = ex };
}
```

### 5. Dependency Injection

Register serializers as enumerable services:

```csharp
public static class PersistenceServiceExtensions
{
    public static IServiceCollection AddSerializer<TSerializer>(
        this IServiceCollection services)
        where TSerializer : class, ISerializationStrategy
    {
        return services.TryAddEnumerableSingleton<ISerializationStrategy, TSerializer>();
    }

    public static IServiceCollection AddPersistence(
        this IServiceCollection services)
    {
        return services
            .AddSingleton<SerializationService>()
            .AddSingleton<PersistenceService>();
    }
}
```

**Usage:**
```csharp
services
    .AddPersistence()
    .AddSerializer<JsonSerializer>()
    .AddSerializer<HjsonSerializer>()
    .AddSerializer<XmlSerializer>();
```

## Common Usage Patterns

### Pattern 1: Register Multiple Serializers

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddPersistence()
        .AddSerializer<JsonSerializer>()
        .AddSerializer<HjsonSerializer>()
        .AddSerializer<YamlSerializer>();

    // Serializer selected based on file extension
    // .json → JsonSerializer
    // .hjson → HjsonSerializer
    // .yaml → YamlSerializer
}
```

### Pattern 2: Custom Serializer

```csharp
public class CustomSerializer : SerializerBase<CustomSerializer>
{
    public override SerializationFormat DefaultFormat =>
        new("custom", "Custom Format", "application/x-custom");

    public override SerializationFlags SupportedCapabilities =>
        SerializationFlags.Text |
        SerializationFlags.Serialize |
        SerializationFlags.Deserialize;

    public override (string, SerializationResult) ToString(object obj, ...)
    {
        // Custom serialization logic
        var result = MyCustomSerializer.Serialize(obj);
        return (result, SerializationResult.Success);
    }

    public override DeserializationResult<T> ToObject<T>(string str, ...)
    {
        // Custom deserialization logic
        var obj = MyCustomSerializer.Deserialize<T>(str);
        return DeserializationResult<T>.Success(obj);
    }
}

// Register
services.AddSerializer<CustomSerializer>();
```

### Pattern 3: Context-Aware Serialization

```csharp
public class ContextAwareSerializer : SerializerBase<ContextAwareSerializer>
{
    public override (string, SerializationResult) ToString(
        object obj,
        Lazy<PersistenceOperation>? operation = null,
        PersistenceContext? context = null)
    {
        var settings = GetSettingsForContext(operation?.Value);

        var json = JsonConvert.SerializeObject(obj, settings);
        return (json, SerializationResult.Success);
    }

    private JsonSerializerSettings GetSettingsForContext(PersistenceOperation? op)
    {
        return op?.SerializeContext switch
        {
            LionSerializeContext.Persistence => new()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include
            },
            LionSerializeContext.Network => new()
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore
            },
            _ => new()
        };
    }
}
```

### Pattern 4: Serialization Service

```csharp
public class SerializationService
{
    private readonly IEnumerable<ISerializationStrategy> serializers;

    public SerializationService(IEnumerable<ISerializationStrategy> serializers)
    {
        this.serializers = serializers;
    }

    public ISerializationStrategy? GetSerializerForExtension(string extension)
    {
        return serializers.FirstOrDefault(s =>
            s.DefaultFormat.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase));
    }

    public ISerializationStrategy? GetSerializerForMimeType(string mimeType)
    {
        return serializers.FirstOrDefault(s =>
            s.DefaultFormat.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase));
    }
}
```

## Integration with Other Libraries

### With Handles

Persistence layer integrates with handle system:

```csharp
// Handle uses persistence layer automatically
var handle = H<AppConfig>.FromPath("/config/app.json");
var config = await handle.Get(); // Uses JsonSerializer
await handle.Set(config);         // Uses JsonSerializer
```

### With Filesystem

Filesystem persistence backend uses serializers:

```csharp
var fileSystem = new FileSystemPersistence();

// Serializer chosen by file extension
var hjsonHandle = fileSystem.GetHandle<Config>("/config/app.hjson"); // HJSON
var jsonHandle = fileSystem.GetHandle<Config>("/config/app.json");   // JSON
```

## Testing Considerations

### Testing Serializers

```csharp
[Fact]
public void Serializer_Roundtrip_PreservesData()
{
    var serializer = new JsonSerializer();
    var original = new MyData { Name = "Test", Value = 42 };

    var (json, serResult) = serializer.ToString(original);
    Assert.True(serResult.IsSuccess);

    var deserResult = serializer.ToObject<MyData>(json);
    Assert.True(deserResult.IsSuccess);
    Assert.Equal(original.Name, deserResult.Value.Name);
    Assert.Equal(original.Value, deserResult.Value.Value);
}
```

### Mocking Serializers

```csharp
var mockSerializer = new Mock<ISerializationStrategy>();
mockSerializer.Setup(x => x.DefaultFormat)
    .Returns(new SerializationFormat("test", "Test", "application/test"));

var service = new SerializationService(new[] { mockSerializer.Object });
var serializer = service.GetSerializerForExtension("test");

Assert.NotNull(serializer);
```

## Related Projects

- **LionFire.Persistence.Abstractions** - Persistence abstractions
- **LionFire.Persistence.Filesystem** - Filesystem backend ([CLAUDE.md](../LionFire.Persistence.Filesystem/CLAUDE.md))
- **LionFire.Persistence.Handles** - Handle implementations
- **LionFire.Serialization.Hjson** - HJSON serializer ([CLAUDE.md](../LionFire.Serialization.Hjson/CLAUDE.md))
- **LionFire.Serialization.Json.Newtonsoft** - JSON.NET serializer

## Summary

**LionFire.Persistence** provides an open-ended persistence framework:

- **Serialization Framework**: Pluggable serializers for multiple formats
- **Persistence Operations**: Context and metadata tracking
- **Multiple Backends**: Filesystem, databases, HTTP APIs
- **Context-Aware**: Different settings for persistence vs network
- **DI Integration**: Enumerable service registration
- **Format Auto-Detection**: Choose serializer by file extension
- **Extensible**: Easy to add new serializers and persisters

**Key Strengths:**
- Multiple storage backends
- Pluggable serialization
- Context-aware serialization
- Format auto-detection
- Comprehensive result types

**Use When:**
- Need multiple storage backends
- Want pluggable serialization
- Require context-aware serialization
- Building data persistence layers

**Typical Use Cases:**
- Configuration file persistence
- Entity persistence to databases
- HTTP API clients/servers
- Multi-format data storage
- Pluggable storage backends
