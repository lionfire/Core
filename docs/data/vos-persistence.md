# VOS Persistence

The VOS persistence layer provides a flexible abstraction for storing and retrieving data across various storage backends. This document covers the persistence architecture, available persisters, and how to implement custom persistence solutions.

## Overview

VOS persistence is built on a provider model where different persisters handle different types of storage:
- **Filesystem** - Files and directories
- **Database** - Structured data storage
- **Key-Value** - Redis, cache stores
- **Archive** - ZIP, TAR files
- **Network** - Remote resources
- **Custom** - Your own implementations

## Persistence Architecture

### Core Interfaces

#### IPersister

The base interface for all persistence providers:

```csharp
public interface IPersister<in TReference> : IPersister
    where TReference : IReference
{
    // Retrieve data
    Task<IGetResult<T>> Get<T>(TReference reference);
    
    // Store data
    Task<IPutResult> Put<T>(TReference reference, T value);
    
    // Delete data
    Task<IDeleteResult> Delete(TReference reference);
    
    // Check existence
    Task<IExistsResult> Exists(TReference reference);
    
    // List children
    Task<IListingResult> List(TReference reference);
}
```

#### Persistence Operations

Each operation returns a result object indicating success/failure:

```csharp
// Get operation
var result = await persister.Get<Config>(reference);
if (result.IsSuccess)
{
    var config = result.Value;
}
else
{
    HandleError(result.Error);
}

// Put operation
var putResult = await persister.Put(reference, data);
if (putResult.IsSuccess)
{
    var etag = putResult.ETag; // Version identifier
}
```

## Built-in Persisters

### Filesystem Persister

The most common persister for file-based storage.

```csharp
// Configure filesystem persister
services.AddVosFilesystem(options =>
{
    options.RootPath = "/app/data";
    options.CreateDirectories = true;
    options.FileExtensions = new Dictionary<Type, string>
    {
        [typeof(Config)] = ".json",
        [typeof(Data)] = ".dat"
    };
});
```

#### Features:
- Automatic directory creation
- File extension mapping
- Path sanitization
- Atomic writes with temp files

#### Usage:
```csharp
// Filesystem references
var fileRef = new FilesystemReference("/data/config.json");
var dirRef = new FilesystemReference("/data/users/");

// Operations
await persister.Put(fileRef, config);
var listing = await persister.List(dirRef);
```

### AutoExtension Filesystem Persister

Automatically determines file extensions based on content type and available serializers.

```csharp
services.AddVosAutoExtensionFilesystem(options =>
{
    options.SearchPatterns = new[] { "json", "yaml", "xml" };
    options.PreferredExtension = "json";
});
```

#### Features:
- Extension auto-detection
- Multiple format support
- Fallback mechanisms

#### Example:
```csharp
// Saves as config.json, config.yaml, or config.xml
// based on available serializers
var vob = vos["/config"];
await vob.Set(new Config { /* ... */ });
```

### Redis Persister

For high-performance key-value storage.

```csharp
services.AddVosRedis(options =>
{
    options.Configuration = "localhost:6379";
    options.KeyPrefix = "vos:";
    options.DefaultExpiry = TimeSpan.FromHours(1);
});
```

#### Features:
- Key prefixing
- Expiration support
- Pipelining
- Pub/Sub integration

#### Usage:
```csharp
// Redis-backed Vob
var cacheVob = vos["/cache/user:123"];
await cacheVob.Set(userData);

// With expiration
var tempVob = vos["/temp/session:abc"];
await tempVob.Set(sessionData, new PersistenceOptions
{
    Expiry = TimeSpan.FromMinutes(30)
});
```

### Archive Persisters

For reading/writing within compressed archives.

#### SharpZipLib Persister

```csharp
services.AddVosSharpZipLib(options =>
{
    options.CompressionLevel = CompressionLevel.Optimal;
    options.EnableEncryption = true;
});
```

#### Usage:
```csharp
// Access files in ZIP
var zipVob = vos["/archives/data.zip"];
var fileVob = zipVob["internal/file.txt"];
var content = await fileVob.Get<string>();

// Create new archive
var newZip = vos["/archives/new.zip"];
await newZip["file1.txt"].Set("content");
await newZip["file2.json"].Set(new { data = "value" });
```

### Database Persisters

#### LiteDB Persister

Embedded NoSQL database:

```csharp
services.AddVosLiteDB(options =>
{
    options.DatabasePath = "/app/data/vos.db";
    options.CollectionNaming = CollectionNamingStrategy.TypeName;
});
```

#### MongoDB Persister

Full-featured document database:

```csharp
services.AddVosMongoDB(options =>
{
    options.ConnectionString = "mongodb://localhost";
    options.Database = "vos";
    options.CollectionPrefix = "vos_";
});
```

## Serialization

VOS uses a pluggable serialization system.

### Built-in Serializers

```csharp
// JSON (default)
services.AddVosJsonSerializer(options =>
{
    options.Formatting = Formatting.Indented;
    options.NullValueHandling = NullValueHandling.Ignore;
});

// YAML
services.AddVosYamlSerializer();

// XML
services.AddVosXmlSerializer(options =>
{
    options.Indent = true;
    options.OmitXmlDeclaration = false;
});

// Binary
services.AddVosBinarySerializer(); // MessagePack
```

### Custom Serializers

Implement `ISerializer` for custom formats:

```csharp
public class MySerializer : ISerializer
{
    public string[] Extensions => new[] { "custom" };
    
    public Task<T> Deserialize<T>(Stream stream)
    {
        // Custom deserialization
    }
    
    public Task Serialize<T>(Stream stream, T value)
    {
        // Custom serialization
    }
}

// Register
services.AddSingleton<ISerializer, MySerializer>();
```

## Persistence Options

Configure persistence behavior per operation:

```csharp
public class PersistenceOptions
{
    // Serialization format
    public string Format { get; set; }
    
    // Cache control
    public TimeSpan? Expiry { get; set; }
    public bool BypassCache { get; set; }
    
    // Concurrency control
    public string ETag { get; set; }
    public bool CreateOnly { get; set; }
    
    // Performance
    public bool AsyncWrite { get; set; }
    public int RetryCount { get; set; }
}
```

### Usage Examples

```csharp
// Force specific format
await vob.Set(data, new PersistenceOptions 
{ 
    Format = "yaml" 
});

// Conditional update
await vob.Set(data, new PersistenceOptions
{
    ETag = previousETag,
    CreateOnly = false
});

// Async write with retries
await vob.Set(data, new PersistenceOptions
{
    AsyncWrite = true,
    RetryCount = 3
});
```

## Custom Persisters

Implement custom persisters for specialized storage needs.

### Basic Implementation

```csharp
public class MyCustomPersister : IPersister<MyReference>
{
    private readonly MyStorageClient client;
    
    public async Task<IGetResult<T>> Get<T>(MyReference reference)
    {
        try
        {
            var data = await client.GetAsync(reference.Key);
            var value = Deserialize<T>(data);
            return GetResult<T>.Success(value);
        }
        catch (NotFoundException)
        {
            return GetResult<T>.NotFound();
        }
        catch (Exception ex)
        {
            return GetResult<T>.Failure(ex);
        }
    }
    
    public async Task<IPutResult> Put<T>(MyReference reference, T value)
    {
        try
        {
            var data = Serialize(value);
            var etag = await client.PutAsync(reference.Key, data);
            return PutResult.Success(etag);
        }
        catch (Exception ex)
        {
            return PutResult.Failure(ex);
        }
    }
    
    // Implement other methods...
}
```

### Advanced Features

```csharp
public class AdvancedPersister : IPersister<IVobReference>
{
    // Batch operations
    public async Task<IEnumerable<IGetResult<T>>> GetMany<T>(
        IEnumerable<IVobReference> references)
    {
        // Optimize with batch retrieval
    }
    
    // Streaming support
    public async Task<Stream> GetStream(IVobReference reference)
    {
        // Return stream for large data
    }
    
    // Metadata support
    public async Task<IDictionary<string, object>> GetMetadata(
        IVobReference reference)
    {
        // Return file metadata
    }
}
```

### Registration

```csharp
// In startup
services.AddSingleton<IPersister<MyReference>, MyCustomPersister>();

// With factory
services.AddSingleton<IPersisterProvider>(sp =>
    new PersisterProvider(type =>
    {
        if (type == typeof(MyReference))
            return sp.GetService<MyCustomPersister>();
        return null;
    }));
```

## Persistence Patterns

### Unit of Work

Group multiple operations:

```csharp
public class VosUnitOfWork
{
    private readonly List<Func<Task>> operations = new();
    
    public void Add<T>(IVob vob, T value)
    {
        operations.Add(() => vob.Set(value));
    }
    
    public void Delete(IVob vob)
    {
        operations.Add(() => vob.Delete());
    }
    
    public async Task Commit()
    {
        foreach (var op in operations)
        {
            await op();
        }
    }
}
```

### Caching Strategy

Implement intelligent caching:

```csharp
public class CachingPersister<TReference> : IPersister<TReference>
    where TReference : IReference
{
    private readonly IPersister<TReference> inner;
    private readonly IMemoryCache cache;
    
    public async Task<IGetResult<T>> Get<T>(TReference reference)
    {
        var key = reference.ToString();
        
        if (cache.TryGetValue<T>(key, out var cached))
        {
            return GetResult<T>.Success(cached);
        }
        
        var result = await inner.Get<T>(reference);
        if (result.IsSuccess)
        {
            cache.Set(key, result.Value, TimeSpan.FromMinutes(5));
        }
        
        return result;
    }
}
```

### Versioning

Implement version control:

```csharp
public class VersionedPersister : IPersister<IVobReference>
{
    public async Task<IPutResult> Put<T>(IVobReference reference, T value)
    {
        // Save current as version
        var versionRef = reference.AppendPath($".versions/{DateTime.UtcNow:yyyyMMddHHmmss}");
        await SaveVersion(versionRef, value);
        
        // Save current
        return await basePersister.Put(reference, value);
    }
    
    public async Task<IEnumerable<IVobReference>> GetVersions(IVobReference reference)
    {
        var versionsRef = reference.AppendPath(".versions");
        var listing = await basePersister.List(versionsRef);
        return listing.Items.Select(i => i.Reference);
    }
}
```

## Performance Optimization

### Bulk Operations

```csharp
// Bulk read
var references = new[] { ref1, ref2, ref3 };
var results = await persister.GetMany<Config>(references);

// Bulk write
var items = new Dictionary<IVobReference, object>
{
    [ref1] = config1,
    [ref2] = config2
};
await persister.PutMany(items);
```

### Streaming

For large data:

```csharp
// Stream read
using var stream = await persister.GetStream(reference);
using var reader = new StreamReader(stream);
while (!reader.EndOfStream)
{
    var line = await reader.ReadLineAsync();
    ProcessLine(line);
}

// Stream write
using var stream = await persister.CreateStream(reference);
using var writer = new StreamWriter(stream);
await writer.WriteAsync(largeContent);
```

### Connection Pooling

```csharp
services.AddVosConnectionPool(options =>
{
    options.MaxConnections = 100;
    options.ConnectionTimeout = TimeSpan.FromSeconds(30);
    options.IdleTimeout = TimeSpan.FromMinutes(5);
});
```

## Error Handling

### Retry Policies

```csharp
services.AddVosPersistenceRetry(options =>
{
    options.MaxRetries = 3;
    options.BackoffMultiplier = 2;
    options.InitialDelay = TimeSpan.FromSeconds(1);
    options.RetryableErrors = new[]
    {
        typeof(TimeoutException),
        typeof(NetworkException)
    };
});
```

### Fallback Strategies

```csharp
public class FallbackPersister : IPersister<IVobReference>
{
    private readonly IPersister<IVobReference> primary;
    private readonly IPersister<IVobReference> fallback;
    
    public async Task<IGetResult<T>> Get<T>(IVobReference reference)
    {
        var result = await primary.Get<T>(reference);
        if (!result.IsSuccess && result.Error is NetworkException)
        {
            return await fallback.Get<T>(reference);
        }
        return result;
    }
}
```

## Best Practices

1. **Choose the Right Persister**: Match persister to data characteristics
2. **Handle Errors Gracefully**: Always check result objects
3. **Use Caching Wisely**: Cache frequently accessed, rarely changed data
4. **Batch Operations**: Group related operations for performance
5. **Clean Up Resources**: Dispose of streams and connections
6. **Version Sensitive Data**: Implement versioning for important data
7. **Monitor Performance**: Track persistence metrics

## Next Steps

- See [Examples](./vos-examples.md) for practical usage patterns
- Review [Architecture](./vos-architecture.md) for system design
- Explore [API Reference](./vos-api-reference.md) for detailed APIs