# VOS Architecture

The Virtual Object System follows a layered architecture designed for flexibility, extensibility, and performance. This document describes the key architectural components and their relationships.

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                         │
│  (Your application code using VOS APIs)                     │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────┴───────────────────────────────────┐
│                      VOS API Layer                          │
│  - IVos (Root Manager)                                      │
│  - IVob (Virtual Objects)                                   │
│  - Handles (Read/Write Access)                              │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────┴───────────────────────────────────┐
│                    VOS Core Layer                           │
│  - Vob Implementation                                       │
│  - Reference System                                         │
│  - Mount Management                                         │
│  - Caching Infrastructure                                   │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────┴───────────────────────────────────┐
│                  Persistence Layer                          │
│  - IPersister Interfaces                                    │
│  - Filesystem Persisters                                    │
│  - Database Persisters                                      │
│  - Archive Persisters                                       │
│  - Custom Persisters                                        │
└─────────────────────────┴───────────────────────────────────┘
```

## Core Components

### 1. IVos - Root Manager

The entry point to the VOS system that manages root nodes and provides access to the virtual tree.

**Key Responsibilities:**
- Manage named root nodes (IRootVob instances)
- Provide configuration through VosOptions
- Service provider integration

**Key Interfaces:**
```csharp
public interface IVos : IHas<IVos>, IHas<IServiceProvider>
{
    IRootVob? Get(string? rootName = null);
    VosOptions Options { get; }
}
```

### 2. IVob - Virtual Object

The fundamental node in the VOS tree, representing both containers and data holders.

**Key Features:**
- Hierarchical parent-child relationships
- Lazy-loaded data access
- Multi-type support through IFlex
- Reference-based addressing
- Handle-based data access

**Key Interfaces:**
```csharp
public interface IVob : IReferenceable<IVobReference>, 
                       IEnumerable<IVob>, 
                       IFlex, 
                       IAcquires, 
                       IParented<IVob>, 
                       IKeyed
{
    string Name { get; }
    string Path { get; }
    IRootVob Root { get; }
    
    // Child access
    IVob GetChild(string subpath);
    IVob this[string subpath] { get; }
    
    // Data access
    IReadHandle<T> GetReadHandle<T>();
    IReadWriteHandle<T> GetReadWriteHandle<T>();
}
```

### 3. Reference System

Provides addressing and location services within the VOS tree.

**Components:**
- **IVobReference**: Points to a location in the VOS tree
- **VobReference<T>**: Type-safe reference implementation
- **Path resolution**: Handles absolute and relative paths

**Features:**
- URL-like syntax: `vos://root/path/to/object`
- Filter support for querying
- Type information preservation

### 4. Mount System

Connects external data sources to locations in the VOS tree.

**Key Components:**
- **IMount**: Defines a mount point
- **Mount**: Implementation with configuration
- **VobMountCache**: Performance optimization
- **Multi-mount support**: Layer multiple sources

**Mount Types:**
- Filesystem mounts
- Archive mounts (ZIP, TAR)
- Database mounts
- Network mounts
- Custom mounts

### 5. Handle System

Provides typed access to data with read/write capabilities.

**Handle Types:**
- **IReadHandle<T>**: Read-only access
- **IReadWriteHandle<T>**: Read and write access
- **IWriteHandle<T>**: Write-only access

**Features:**
- Async operations
- Lazy loading
- Caching support
- Type conversion

### 6. Persistence Layer

Manages the actual storage and retrieval of data.

**Components:**
- **IPersister**: Base interface for all persisters
- **IPersister<TReference>**: Reference-specific persisters
- **Serialization**: Pluggable serializers
- **Conventions**: Naming and path conventions

**Built-in Persisters:**
- FilesystemPersister
- AutoExtensionFilesystemPersister
- RedisPersister
- LiteDBPersister
- SharpZipLibPersister

## Data Flow

### Reading Data

1. Application requests data via `vob.GetReadHandle<T>()`
2. Handle checks cache for existing value
3. If not cached, resolve through mount system
4. Mount determines appropriate persister
5. Persister loads data from source
6. Data is deserialized and cached
7. Value returned to application

### Writing Data

1. Application writes data via `handle.SetValue(value)`
2. Value is validated and serialized
3. Mount system routes to appropriate persister
4. Persister writes to underlying storage
5. Caches are updated/invalidated
6. Confirmation returned to application

## Extension Points

### Custom Persisters

Implement `IPersister<TReference>` to add support for new data sources:

```csharp
public class MyCustomPersister : IPersister<MyReference>
{
    public Task<IGetResult<T>> Get<T>(MyReference reference) { }
    public Task<IPutResult> Put<T>(MyReference reference, T value) { }
}
```

### Custom References

Create custom reference types for specialized addressing:

```csharp
public class MyReference : IReference
{
    public string Scheme => "myscheme";
    public string Path { get; set; }
}
```

### Custom Mounts

Implement custom mount logic for complex scenarios:

```csharp
public class MyMount : IMount
{
    public IVob MountPoint { get; }
    public IReference Target { get; }
}
```

## Performance Considerations

### Caching Strategy

- **VobCache**: Caches Vob instances by path
- **HandleCache**: Caches handle instances
- **ValueCache**: Caches deserialized values
- **Configurable TTL**: Time-based expiration

### Lazy Loading

- Vobs are created on-demand
- Data is loaded only when accessed
- Child enumeration doesn't trigger loads

### Async Operations

- All I/O operations are async
- Parallel loading support
- Cancellation token support

## Thread Safety

- Vob instances are thread-safe for read operations
- Write operations use appropriate locking
- Caches use concurrent collections
- Mount operations are synchronized

## Dependency Injection

VOS integrates with Microsoft.Extensions.DependencyInjection:

```csharp
services.AddVos(options => { });
services.AddVosRoot("data");
services.AddVosFilesystem();
services.AddVosRedis();
```

## Configuration

VOS is configured through `VosOptions`:

```csharp
public class VosOptions
{
    public Dictionary<string, VobRootOptions> Roots { get; }
    public bool EnableCaching { get; set; }
    public TimeSpan CacheTimeout { get; set; }
}
```

## Next Steps

- Understand [Core Concepts](./vos-core-concepts.md) in detail
- Learn about the [Mounting System](./vos-mounting-system.md)
- Explore [Persistence](./vos-persistence.md) options