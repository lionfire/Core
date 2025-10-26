# LionFire.Vos

## Overview

**LionFire.Vos** (Virtual Object System) is LionFire's most powerful and complex toolkit. It provides a virtual filesystem-like tree structure where you can mount multiple data sources (filesystems, databases, zip files) at different paths, with support for overlaying, hierarchical dependency injection, and unified data access.

**Layer**: Advanced Toolkit (Virtual Object System)
**Target**: .NET 9.0
**Root Namespace**: `LionFire.Vos`

**Critical**: See comprehensive VOS documentation in `/mnt/c/src/Core/docs/data/` directory.

## Key Dependencies

### NuGet Packages
- **OpenTelemetry.Api** - Telemetry and observability
- **System.Linq.Async** - Async LINQ operations

### LionFire Dependencies
- **LionFire.Base** - BCL augmentation
- **LionFire.DependencyMachines** - Dependency state machines
- **LionFire.Environment** - Environment detection
- **LionFire.Execution** - Execution abstractions
- **LionFire.Flex** - FlexData pattern
- **LionFire.Instantiating.Abstractions** - Template instantiation
- **LionFire.Persistence** - Persistence framework
- **LionFire.Persistence.Handles** - Handle implementations
- **LionFire.Structures** - Data structures
- **LionFire.Vos.Abstractions** - VOS abstractions

## Core Concepts

### What is VOS?

VOS provides a **virtual tree structure** where:

1. **Vobs** (Virtual Objects) are nodes in the tree
2. **References** are paths to locate objects (`vos:///path/to/object`)
3. **Mounts** connect data sources to the tree
4. **Handles** provide read/write access to data
5. **Overlays** enable multiple mounts at the same path for layered data

### Architecture

```
VOS Root
├── config/          (mount: filesystem → /app/config)
│   ├── app.hjson
│   └── users.json
├── data/            (mount: database → PostgreSQL)
│   ├── users/
│   └── products/
└── assets/          (mount: overlay)
                     ├── layer1: filesystem → /app/assets
                     └── layer2: zip → assets.zip
```

### Key VOS Concepts

**1. Vob (Virtual Object)**
- Node in the virtual tree
- Has a reference (path)
- May have children
- Can load/save data

**2. Reference**
- VOS path: `vos:///path/to/object`
- Identifies location in virtual tree

**3. Mount**
- Connects data source to VOS path
- Types: Filesystem, Database, HTTP, Zip, Memory

**4. Overlay**
- Multiple mounts at same path
- Read from top to bottom (fallback)
- Write to specific layer

**5. Handle**
- Read/write interface to VOS object
- Lazy loading
- Change tracking

## VOS Documentation

**Essential Reading** (in `/mnt/c/src/Core/docs/data/`):
- `README.md` - VOS overview
- `vos-architecture.md` - Architecture deep dive
- `vos-core-concepts.md` - Core concepts explained
- `vos-examples.md` - Usage examples
- `vos-mounts.md` - Mount system
- `vos-overlays.md` - Overlay system
- `vos-handles.md` - Handle usage

## Basic Usage

### 1. Create VOS Instance

```csharp
var vos = new Vos();
await vos.Initialize();
```

### 2. Mount Filesystem

```csharp
// Mount filesystem at /config
vos.Mount("/config", new FileSystemMount("/app/config"));
```

### 3. Access Data

```csharp
// Get object by path
var vob = vos.Get<AppConfig>("/config/app.hjson");

// Load data
var config = await vob.Get();

// Modify and save
config.Port = 8080;
await vob.Set(config);
```

### 4. Overlays

```csharp
// Mount multiple layers
vos.MountOverlay("/assets")
    .AddLayer(new FileSystemMount("/app/assets"))      // Top layer (write here)
    .AddLayer(new ZipMount("default-assets.zip"));      // Fallback layer

// Read: checks top layer first, falls back to zip
var asset = await vos.Get<Asset>("/assets/logo.png").Get();

// Write: goes to top layer
await vos.Get<Asset>("/assets/logo.png").Set(newLogo);
```

## Advanced Features

### Hierarchical Dependency Injection

VOS supports DI at any node:

```csharp
// Register service at /api
vos.Get("/api").ServiceCollection.AddSingleton<IApiClient, ApiClient>();

// Service available to /api and descendants
var apiClient = vos.Get("/api/users").ServiceProvider.GetService<IApiClient>();
```

### Virtual Directories

Navigate tree structure:

```csharp
var configVob = vos.Get("/config");

// List children
var children = await configVob.GetChildren();

// Traverse hierarchy
var parent = configVob.Parent;
var root = configVob.Root;
```

## Mount Types

### Filesystem Mount

```csharp
vos.Mount("/data", new FileSystemMount("/app/data"));
```

### Database Mount

```csharp
vos.Mount("/db", new DatabaseMount(connectionString));
```

### HTTP Mount

```csharp
vos.Mount("/api", new HttpMount("https://api.example.com"));
```

### Zip Mount

```csharp
vos.Mount("/archive", new ZipMount("data.zip"));
```

### Memory Mount

```csharp
vos.Mount("/cache", new MemoryMount());
```

## Common Patterns

### Pattern 1: Configuration with Overlays

```csharp
// User config overrides default config
vos.MountOverlay("/config")
    .AddLayer(new FileSystemMount("/user/config"))     // User overrides
    .AddLayer(new FileSystemMount("/app/defaults"));   // Defaults

var config = await vos.Get<AppConfig>("/config/app.hjson").Get();
// Reads from user config if exists, otherwise defaults
```

### Pattern 2: Multi-Source Data

```csharp
vos.Mount("/users/local", new FileSystemMount("/data/users"));
vos.Mount("/users/remote", new HttpMount("https://api.example.com/users"));

// Access both sources through unified interface
var localUsers = await vos.Get<UserList>("/users/local/users.json").Get();
var remoteUsers = await vos.Get<UserList>("/users/remote/users").Get();
```

### Pattern 3: Hierarchical Configuration

```csharp
// Global config
vos.Mount("/config/global", new FileSystemMount("/app/config"));

// Environment-specific config
vos.Mount("/config/dev", new FileSystemMount("/app/config/dev"));
vos.Mount("/config/prod", new FileSystemMount("/app/config/prod"));

var environment = "prod";
var config = await vos.Get<AppConfig>($"/config/{environment}/app.hjson").Get();
```

## Integration with Persistence

VOS uses the persistence framework:

```csharp
// VOS handles use persistence layer
var handle = vos.Get<MyData>("/data/myfile.hjson");

// Persistence layer chooses serializer by extension
var data = await handle.Get(); // Uses HjsonSerializer
await handle.Set(data);         // Uses HjsonSerializer
```

## Testing Considerations

### Mock VOS

```csharp
var mockVos = new Mock<IVos>();
mockVos.Setup(x => x.Get<AppConfig>(It.IsAny<string>()))
    .Returns(mockVob.Object);

var service = new MyService(mockVos.Object);
```

### In-Memory VOS

```csharp
var vos = new Vos();
vos.Mount("/data", new MemoryMount());

// Use for testing without filesystem
var handle = vos.Get<TestData>("/data/test");
await handle.Set(new TestData { Value = 42 });
```

## Related Projects

- **LionFire.Vos.Abstractions** - VOS abstractions
- **LionFire.Vos.VosApp** - VOS application framework ([CLAUDE.md](../LionFire.Vos.Application/CLAUDE.md))
- **LionFire.Persistence** - Persistence framework ([CLAUDE.md](../LionFire.Persistence/CLAUDE.md))
- **LionFire.Referencing** - Reference system ([CLAUDE.md](../LionFire.Referencing/CLAUDE.md))

## Documentation

**Essential VOS Documentation** in `/mnt/c/src/Core/docs/data/`:
- Overview and architecture
- Core concepts explained
- Mount system details
- Overlay patterns
- Handle usage
- Examples and patterns

## Summary

**LionFire.Vos** provides a powerful Virtual Object System:

- **Virtual Tree**: Hierarchical object structure
- **Mounts**: Connect multiple data sources
- **Overlays**: Layer multiple sources at same path
- **Unified Access**: Single interface for files, DBs, APIs
- **Hierarchical DI**: Services scoped to virtual directories
- **Lazy Loading**: Efficient data access
- **Change Tracking**: Know when data changes

**Key Strengths:**
- Extremely flexible
- Multiple data sources
- Overlay support
- Hierarchical DI
- Unified interface

**Use When:**
- Need virtual filesystem
- Multiple data sources
- Overlay configurations
- Hierarchical organization
- Complex data access patterns

**Typical Use Cases:**
- Configuration management with overlays
- Multi-source data aggregation
- Plugin systems with virtual mounts
- Hierarchical application data
- Development tools and IDEs

**Note**: VOS is the most complex LionFire toolkit. Read the comprehensive documentation in `/mnt/c/src/Core/docs/data/` before using.
