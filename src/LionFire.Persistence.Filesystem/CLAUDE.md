# LionFire.Persistence.Filesystem

## Overview

**LionFire.Persistence.Filesystem** provides a filesystem-based persistence backend for LionFire. It enables reading and writing objects to/from files with automatic serialization format detection based on file extensions.

**Layer**: Toolkit (Persistence Backend)
**Target**: .NET 9.0
**Root Namespace**: `LionFire.Persistence`

## Key Dependencies

### NuGet Packages
- **Microsoft.Extensions.DependencyInjection** - DI support
- **Microsoft.Extensions.Resilience** - Retry/circuit breaker

### LionFire Dependencies
- **LionFire.Applications.Abstractions** - Application abstractions
- **LionFire.IO.VirtualFilesystem** - Virtual filesystem abstractions
- **LionFire.Persistence** - Persistence framework
- **LionFire.Persistence.Handles** - Handle implementations
- **LionFire.Serialization.Json.Newtonsoft** - JSON serializer

## Core Features

### 1. Filesystem Persistence

Read and write objects to files with automatic format detection:

```csharp
var fileSystem = new FileSystemPersistence("/data");

// Extension determines serializer
var handle = fileSystem.GetHandle<AppConfig>("/config/app.json");  // JSON
var config = await handle.Get();

config.AppName = "Updated";
await handle.Set(config);
```

### 2. Automatic Format Detection

Serializer chosen by file extension:

- `.json` → JSON serializer
- `.hjson` → HJSON serializer
- `.xml` → XML serializer
- `.yaml`/`.yml` → YAML serializer

### 3. Virtual Filesystem Integration

Integrates with LionFire's virtual filesystem layer:

```csharp
public interface IFileSystemPersistence
{
    IH<T> GetHandle<T>(string path);
    Task<bool> Exists(string path);
    Task Delete(string path);
}
```

## Common Usage Patterns

### Pattern 1: Basic File Operations

```csharp
var fs = new FileSystemPersistence("/app/data");

// Read
var configHandle = fs.GetHandle<AppConfig>("/config/app.json");
var config = await configHandle.Get();

// Write
config.Port = 8080;
await configHandle.Set(config);

// Check existence
bool exists = await fs.Exists("/config/app.json");

// Delete
await fs.Delete("/config/old.json");
```

### Pattern 2: Multiple Formats

```csharp
// Same data, different formats
var jsonHandle = fs.GetHandle<Config>("/config/app.json");
var hjsonHandle = fs.GetHandle<Config>("/config/app.hjson");
var yamlHandle = fs.GetHandle<Config>("/config/app.yaml");

var config = await jsonHandle.Get();

// Write to all formats
await jsonHandle.Set(config);
await hjsonHandle.Set(config);
await yamlHandle.Set(config);
```

### Pattern 3: Directory-Based Organization

```csharp
var fs = new FileSystemPersistence("/app/data");

// Organize by type
var usersHandle = fs.GetHandle<UserList>("/users/users.json");
var settingsHandle = fs.GetHandle<Settings>("/settings/app.hjson");
var logsHandle = fs.GetHandle<LogData>("/logs/app.log.json");
```

## Integration with Handles

```csharp
// Via DI
[Inject]
public IFileSystemPersistence FileSystem { get; set; }

public async Task LoadConfiguration()
{
    var handle = FileSystem.GetHandle<AppConfig>("/config/app.hjson");

    // Reactive updates
    handle.GetResults.Subscribe(result =>
    {
        if (result.HasValue)
        {
            ApplyConfig(result.Value);
        }
    });

    var config = await handle.GetIfNeeded();
}
```

## Related Projects

- **LionFire.Persistence** - Persistence framework ([CLAUDE.md](../LionFire.Persistence/CLAUDE.md))
- **LionFire.IO.VirtualFilesystem** - Virtual filesystem
- **LionFire.Serialization.Hjson** - HJSON serializer
- **LionFire.Blazor.Components.UI** - File-based UI components ([CLAUDE.md](../LionFire.Blazor.Components.UI/CLAUDE.md))

## Summary

**LionFire.Persistence.Filesystem** provides filesystem persistence:

- **File-Based Storage**: Read/write objects to files
- **Auto-Format Detection**: Extension-based serializer selection
- **Virtual Filesystem**: Integration with VFS layer
- **Handle Integration**: Works with LionFire handle system
- **Multiple Formats**: JSON, HJSON, YAML, XML support

**Use When:**
- Need file-based persistence
- Want automatic format detection
- Storing configuration files
- File-based data storage

**Typical Use Cases:**
- Application configuration
- User settings persistence
- File-based databases
- Log storage
- Development data files
