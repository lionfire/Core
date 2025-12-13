# Getting Started: Virtual Object System (VOS)

## Overview

The **Virtual Object System (VOS)** is LionFire's advanced framework for creating virtual filesystems that can mount multiple data sources (filesystems, databases, zip files, APIs) into a unified hierarchical structure. This guide provides a gentle introduction to VOS concepts.

**What You'll Learn**:
- What VOS is and why it exists
- Core VOS concepts (Vobs, References, Mounts)
- Your first VOS mount
- Basic VOS operations
- When to use VOS vs. simpler patterns

**Prerequisites**:
- .NET 9.0+ SDK
- Completed previous getting started guides (recommended)
- Understanding of hierarchical data structures

**Note**: VOS is an advanced system. For most applications, the simpler patterns from previous guides (`IGetter`, `IValue`, file-based persistence) are sufficient. Use VOS when you need:
- Virtual filesystem abstraction
- Multiple overlaid data sources
- Hierarchical dependency injection
- Mount point composition

---

## What is VOS?

VOS creates a **virtual filesystem** where:
- Multiple **data sources** can be **mounted** at different paths
- Data sources can be **overlaid** (multiple mounts at same path)
- Objects are accessed through **virtual paths** (like `/config/database` or `/users/alice`)
- **Dependency injection** cascades through the virtual hierarchy

### Why VOS?

**Traditional Approach** (without VOS):
```csharp
// Hardcoded paths everywhere
var config = JsonSerializer.Deserialize<Config>(
    await File.ReadAllTextAsync("C:/app/config/database.json")
);

var users = await database.GetAllUsers();
var profile = await api.GetUserProfile(userId);
```

**VOS Approach**:
```csharp
// Abstract virtual paths
var config = await vos["config/database"].Get<Config>();
var users = await vos["users"].GetChildren<User>();
var profile = await vos[$"users/{userId}/profile"].Get<UserProfile>();
```

**Benefits**:
- Code doesn't know where data comes from (filesystem, database, API, etc.)
- Easy to swap data sources (development vs. production)
- Overlay multiple sources (local overrides, defaults)
- Consistent API for all data access

---

## Core Concepts

### 1. Vobs (Virtual Objects)

**Vobs** are nodes in the virtual tree:

```
/                           (Root vob)
├── config/                 (Directory vob)
│   ├── app.json           (File vob)
│   └── database.json      (File vob)
├── users/                  (Directory vob)
│   ├── alice/             (Directory vob)
│   └── bob/               (Directory vob)
└── data/                   (Directory vob)
```

Each vob has:
- **Path**: `/config/database.json`
- **Name**: `database.json`
- **Parent**: `/config`
- **Children**: Sub-vobs

### 2. References

**References** are paths to locate vobs:

```csharp
// String references
var ref1 = "/config/database";
var ref2 = "/users/alice/profile";

// Strongly-typed references
VRef<Config> configRef = new VRef<Config>("/config/database");
```

### 3. Mounts

**Mounts** connect data sources to the virtual tree:

```
Virtual Tree:
/config/ ──mount──> C:/app/config/        (Filesystem)
/users/  ──mount──> Database Table         (SQL)
/cache/  ──mount──> Redis                  (Cache)
```

### 4. Overlays

Multiple mounts can overlay the same path:

```
/config/
  ├── Mount 1 (Priority 100): ./local-overrides/
  └── Mount 2 (Priority 0):   ./defaults/

Reading /config/app.json:
  1. Check ./local-overrides/app.json (found → use this)
  2. If not found, check ./defaults/app.json
```

---

## Your First VOS Mount

Let's create a simple VOS setup with a filesystem mount.

### Example: Configuration VOS

```csharp
using LionFire.Vos;
using LionFire.Persistence.Filesystem;

// Step 1: Create VOS root
var vos = new Vos();

// Step 2: Mount a filesystem directory
vos.Mount(
    "/config",                          // Virtual path
    @"C:\MyApp\configs"                // Real filesystem path
);

// Step 3: Access through VOS
var configVob = vos["/config/database"];
var config = await configVob.Get<DatabaseConfig>();

Console.WriteLine($"Database: {config.Host}:{config.Port}");
```

**Key Points**:
- `/config` in VOS maps to `C:\MyApp\configs`
- `/config/database` reads from `C:\MyApp\configs\database.json`
- Code doesn't know it's a filesystem - could be database, API, etc.

---

## Basic VOS Operations

### Reading Data

```csharp
// Get single item
var config = await vos["/config/app"].Get<AppConfig>();

// Get with handle
var handle = vos["/config/app"].ToHandle<AppConfig>();
await handle.Get();
var config = handle.Value;

// Check if exists
var exists = await vos["/config/optional"].Exists();
```

### Writing Data

```csharp
var config = new AppConfig { Port = 8080 };

// Direct write
await vos["/config/app"].Set(config);

// Write with handle
var handle = vos["/config/app"].ToHandle<AppConfig>();
handle.Value = config;
await handle.Set();
```

### Listing Children

```csharp
// Get child names
var children = await vos["/config"].GetChildNames();
foreach (var name in children)
{
    Console.WriteLine($"  {name}");
}

// Get child vobs
var childVobs = await vos["/config"].GetChildren();
foreach (var vob in childVobs)
{
    Console.WriteLine($"  {vob.Path}");
}
```

---

## Multiple Mounts (Overlay)

Combine multiple data sources with priority:

```csharp
var vos = new Vos();

// Mount 1: User overrides (highest priority)
vos.Mount("/config", "./user-config", priority: 100);

// Mount 2: Application defaults (lower priority)
vos.Mount("/config", "./default-config", priority: 0);

// Reading /config/app.json:
// 1. Checks ./user-config/app.json first
// 2. Falls back to ./default-config/app.json if not found
var config = await vos["/config/app"].Get<AppConfig>();
```

**Use Cases**:
- Development vs. Production configs
- User overrides over defaults
- Local cache over remote API

---

## Practical Example: Multi-Source Application

```csharp
using LionFire.Vos;
using LionFire.Persistence.Filesystem;

public class Application
{
    private readonly Vos vos;

    public Application(bool isDevelopment)
    {
        vos = new Vos();

        if (isDevelopment)
        {
            // Development: Local files with overrides
            vos.Mount("/config", "./dev-config", priority: 100);
            vos.Mount("/config", "./default-config", priority: 0);
            vos.Mount("/data", "./dev-data");
        }
        else
        {
            // Production: Secure locations
            vos.Mount("/config", @"C:\ProgramData\MyApp\config");
            vos.Mount("/data", @"D:\AppData");
        }
    }

    public async Task<AppConfig> LoadConfig()
    {
        return await vos["/config/app"].Get<AppConfig>();
    }

    public async Task<IEnumerable<string>> ListUsers()
    {
        return await vos["/data/users"].GetChildNames();
    }

    public async Task<User> GetUser(string username)
    {
        return await vos[$"/data/users/{username}"].Get<User>();
    }

    public async Task SaveUser(User user)
    {
        await vos[$"/data/users/{user.Username}"].Set(user);
    }
}

// Usage
var app = new Application(isDevelopment: true);

var config = await app.LoadConfig();
Console.WriteLine($"Loaded config: {config.AppName}");

await app.SaveUser(new User { Username = "alice", Email = "alice@example.com" });

var users = await app.ListUsers();
Console.WriteLine($"Users: {string.Join(", ", users)}");
```

---

## VOS vs. Simpler Patterns

### When to Use VOS

✅ **Use VOS when you need**:
- Virtual filesystem abstraction
- Multiple overlaid data sources
- Environment-specific mounts (dev/prod)
- Hierarchical dependency injection
- Plugin-based data sources

### When NOT to Use VOS

❌ **Use simpler patterns when**:
- Single data source (just use `IGetter`/`IValue`)
- Flat data structure (just use `IObservableReader`)
- Simple file persistence (just use `File.ReadAllText`)
- No need for abstraction

**Example - Simple is Better**:
```csharp
// ❌ Overkill with VOS
var vos = new Vos();
vos.Mount("/config", "./config");
var config = await vos["/config/app"].Get<AppConfig>();

// ✅ Just use File I/O
var json = await File.ReadAllTextAsync("./config/app.json");
var config = JsonSerializer.Deserialize<AppConfig>(json);

// ✅ Or use IValue
var configValue = new ValueRxO<AppConfig>(
    loadFunc: ct => LoadConfig(ct),
    saveFunc: (config, ct) => SaveConfig(config, ct)
);
await configValue.GetIfNeeded();
```

---

## VOS with Dependency Injection

VOS integrates with hierarchical DI:

```csharp
// Services registered at /config/ are available to /config/database/
vos["/config"].RegisterService<ISerializer>(new JsonSerializer());

// Child vobs inherit parent services
var serializer = vos["/config/database"].ResolveService<ISerializer>();
```

---

## Summary

**VOS (Virtual Object System)** provides:

**Core Concepts**:
- **Vobs** - Virtual objects in a tree
- **References** - Paths to locate vobs
- **Mounts** - Connect data sources
- **Overlays** - Multiple mounts with priority

**Benefits**:
- Data source abstraction
- Environment-specific configuration
- Overlay patterns (user overrides, defaults)
- Hierarchical DI

**When to Use**:
- ✅ Multiple data sources
- ✅ Need abstraction layer
- ✅ Complex mounting scenarios
- ❌ Simple applications (use `IGetter`/`IValue` instead)

**Next Steps**:
1. Review [VOS Documentation](../../data/vos/vos-overview.md) for complete guide
2. Explore [VOS Core Concepts](../../data/vos/vos-core-concepts.md)
3. Read [VOS Examples](../../data/vos/vos-examples.md) for advanced patterns
4. Study [VOS Architecture](../../data/vos/vos-architecture.md) for design details

---

## Exercise

Build a plugin system where:
1. Plugins are mounted at `/plugins/{name}`
2. Each plugin has a config at `/plugins/{name}/config`
3. Shared defaults are mounted at `/defaults`
4. Plugin configs overlay defaults
5. List all plugins
6. Load specific plugin configuration

Use VOS mounting and overlay patterns!

---

## Advanced Topics

For advanced VOS usage, see:
- **[VOS Architecture](../../data/vos/vos-architecture.md)** - System design
- **[VOS Mounting System](../../data/vos/vos-mounting-system.md)** - Advanced mounts
- **[VOS Persistence](../../data/vos/vos-persistence.md)** - Storage backends
- **[VOS API Reference](../../data/vos/vos-api-reference.md)** - Complete API

VOS is a powerful system for complex scenarios, but remember: simpler patterns are often better for straightforward applications!
