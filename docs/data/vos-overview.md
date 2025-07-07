# VOS Overview

The LionFire Virtual Object System (VOS) is a comprehensive data abstraction layer that provides a unified, hierarchical interface for accessing and managing data from various sources. Think of it as a virtual filesystem that can mount and access data from files, databases, archives, network sources, and more - all through a consistent API.

## What is VOS?

VOS creates a virtual tree structure where each node (called a **Vob** - Virtual Object) can represent:
- Files and directories from the filesystem
- Database records and collections
- Objects within ZIP archives
- Network resources
- In-memory data structures
- Any custom data source through extensible providers

## Key Features

### 1. **Unified Interface**
Access all data through a consistent API regardless of the underlying storage mechanism.

```csharp
// Access a file
var fileVob = vos["/data/config.json"];

// Access a database record
var dbVob = vos["/database/users/john"];

// Access data in a ZIP archive
var zipVob = vos["/archives/backup.zip/data/file.txt"];
```

### 2. **Hierarchical Organization**
Data is organized in a tree structure with parent-child relationships, similar to a filesystem.

### 3. **Multiple Data Sources**
Mount different data sources at various points in the virtual tree:
- Filesystem directories
- Database connections
- Archive files (ZIP, TAR, etc.)
- Remote services
- Custom data providers

### 4. **Lazy Loading**
Data is loaded on-demand, improving performance and reducing memory usage.

### 5. **Type Safety**
Strong typing support with generic interfaces for type-safe data access.

### 6. **Persistence Abstraction**
Read and write data through a unified persistence layer that handles serialization and storage.

### 7. **Caching and Performance**
Built-in caching mechanisms for frequently accessed data.

### 8. **Extensibility**
Easy to extend with custom data providers, serializers, and mount types.

## Core Components

### Vob (Virtual Object)
The fundamental unit - a node in the virtual tree that can contain data and/or child nodes.

### Reference
A pointer to a location in the VOS tree, similar to a file path.

### Mount
A connection point that links a location in the VOS tree to an external data source.

### Handle
Provides read/write access to the data stored in a Vob.

### Persister
Manages the actual storage and retrieval of data for a specific data source type.

## Use Cases

1. **Configuration Management**
   - Unify configuration from files, databases, and environment variables
   - Override configurations through layered mounts

2. **Asset Management**
   - Organize game assets, media files, or documents
   - Support for asset packages and archives

3. **Data Integration**
   - Aggregate data from multiple sources
   - Provide a unified API for heterogeneous data

4. **Virtual Filesystems**
   - Create application-specific file systems
   - Implement custom storage solutions

5. **Modular Applications**
   - Plugin systems with isolated data spaces
   - Dynamic content loading

## Getting Started

To use VOS in your application:

1. Add VOS to your dependency injection container
2. Configure root nodes and mounts
3. Access data through the VOS interface

```csharp
// In your startup configuration
services.AddVos(options => 
{
    options.AddRoot("data", "/app/data");
    options.AddMount("/config", "file:///etc/myapp");
});

// In your application
var vos = serviceProvider.GetRequiredService<IVos>();
var configVob = vos["/config/settings.json"];
var settings = await configVob.GetReadHandle<AppSettings>().GetValue();
```

## Next Steps

- Learn about the [Architecture](./vos-architecture.md) to understand how VOS is designed
- Explore [Core Concepts](./vos-core-concepts.md) for detailed terminology
- See [Examples and Usage](./vos-examples.md) for practical code samples