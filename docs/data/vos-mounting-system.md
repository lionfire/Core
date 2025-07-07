# VOS Mounting System

The VOS mounting system allows you to connect various data sources to locations in the virtual object tree. This provides a unified interface for accessing disparate data sources.

## Overview

Mounts in VOS work similarly to mounting filesystems in Unix-like operating systems. You can:
- Mount filesystems at any point in the VOS tree
- Layer multiple mounts at the same location
- Mount archives, databases, and custom data sources
- Configure read-only or read-write access
- Enable/disable mounts dynamically

## Basic Mounting

### Simple Filesystem Mount

```csharp
// Mount a local directory
vos.Mount("/data", "file:///var/app/data");

// Mount with options
vos.Mount("/config", "file:///etc/myapp", new MountOptions
{
    IsReadOnly = true,
    Name = "System Config"
});
```

### Mount URI Schemes

VOS supports various URI schemes for different data sources:

- `file://` - Local filesystem
- `zip://` - ZIP archives
- `tar://` - TAR archives
- `redis://` - Redis database
- `mongodb://` - MongoDB database
- `http://` / `https://` - HTTP resources
- `vos://` - Another VOS tree

## Mount Configuration

### MountOptions

```csharp
public class MountOptions : IVobMountOptions
{
    // Friendly name for the mount
    public string Name { get; set; }
    
    // If true, no other mounts allowed at this location
    public bool IsExclusive { get; set; }
    
    // If true, no writes allowed through this mount
    public bool IsReadOnly { get; set; }
    
    // Higher priority mounts are checked first
    public int Priority { get; set; }
    
    // Parent mount for layering
    public IMount UpstreamMount { get; set; }
    
    // Custom persister selection
    public string PersisterName { get; set; }
}
```

### Configuration Examples

```csharp
// Read-only mount with high priority
vos.Mount("/system", "file:///usr/share/app", new MountOptions
{
    Name = "System Resources",
    IsReadOnly = true,
    Priority = 100
});

// Exclusive mount (no overlays allowed)
vos.Mount("/secure", "file:///var/secure", new MountOptions
{
    IsExclusive = true,
    Name = "Secure Storage"
});
```

## Layered Mounts

One of VOS's powerful features is the ability to layer multiple mounts at the same location.

### Overlay Example

```csharp
// Base layer - defaults
vos.Mount("/config", "file:///usr/share/app/config", new MountOptions
{
    Name = "Default Config",
    Priority = 0,
    IsReadOnly = true
});

// User layer - overrides
vos.Mount("/config", "file:///home/user/.config/app", new MountOptions
{
    Name = "User Config",
    Priority = 10
});

// Environment layer - highest priority
vos.Mount("/config", "env://APP_CONFIG_", new MountOptions
{
    Name = "Environment Config",
    Priority = 20
});
```

### How Layering Works

1. **Read Operations**: VOS checks mounts from highest to lowest priority
2. **Write Operations**: Writes go to the highest priority writable mount
3. **Enumeration**: Children from all mounts are merged

```csharp
// Reading from layered mounts
var vob = vos["/config/database.json"];
var handle = vob.GetReadHandle<DatabaseConfig>();
var config = await handle.GetValue();
// Returns from highest priority mount that has the file

// Writing to layered mounts
var writeHandle = vob.GetWriteHandle<DatabaseConfig>();
await writeHandle.SetValue(newConfig);
// Writes to highest priority writable mount
```

## Archive Mounts

VOS can mount compressed archives as if they were directories.

### ZIP Archive Mount

```csharp
// Mount a ZIP file
vos.Mount("/backup", "zip:///backups/2024-01-01.zip");

// Access files within the ZIP
var fileVob = vos["/backup/data/users.json"];
var users = await fileVob.Get<List<User>>();

// Mount nested archives
vos.Mount("/nested", "zip:///archives/outer.zip");
vos.Mount("/nested/inner", "zip:///nested/inner.zip");
```

### TAR Archive Mount

```csharp
// Mount TAR/TAR.GZ files
vos.Mount("/archive", "tar:///data/backup.tar.gz");

// Configure compression
vos.Mount("/archive", "tar:///data/backup.tar.bz2", new MountOptions
{
    Metadata = new Dictionary<string, object>
    {
        ["Compression"] = "bzip2"
    }
});
```

## Database Mounts

Mount database collections as VOS paths.

### Redis Mount

```csharp
// Mount Redis database
vos.Mount("/cache", "redis://localhost:6379/0", new MountOptions
{
    Name = "Redis Cache",
    Metadata = new Dictionary<string, object>
    {
        ["Password"] = "secret",
        ["KeyPrefix"] = "app:"
    }
});

// Use Redis-backed Vobs
var cacheVob = vos["/cache/user:123"];
await cacheVob.Set(new UserCache { /* ... */ });
```

### MongoDB Mount

```csharp
// Mount MongoDB collection
vos.Mount("/users", "mongodb://localhost/mydb/users");

// Query through VOS
var adminVob = vos["/users"].QueryChild(new VobReference(
    filters: new[] { ("role", "admin") }
));
```

## Custom Mounts

Implement custom mount providers for specialized data sources.

### Creating a Custom Mount Provider

```csharp
public class MyCustomMountProvider : IMountProvider
{
    public IMount CreateMount(IVob mountPoint, IReference target, MountOptions options)
    {
        return new MyCustomMount(mountPoint, target, options);
    }
}

public class MyCustomMount : Mount
{
    public override IPersister GetPersister()
    {
        return new MyCustomPersister();
    }
}
```

### Registering Custom Providers

```csharp
// In startup
services.AddSingleton<IMountProvider, MyCustomMountProvider>();
services.AddVosMountProvider("custom", typeof(MyCustomMountProvider));

// Usage
vos.Mount("/custom", "custom://my-data-source");
```

## Dynamic Mount Management

Mounts can be managed at runtime.

### Enabling/Disabling Mounts

```csharp
// Get mount reference
var mount = vos.GetMount("/data");

// Disable temporarily
mount.IsEnabled = false;

// Re-enable
mount.IsEnabled = true;
```

### Unmounting

```csharp
// Unmount by path
vos.Unmount("/data");

// Unmount specific mount
var mount = vos.GetMount("/data", "User Data");
vos.Unmount(mount);
```

### Mount Events

```csharp
// Subscribe to mount events
vos.MountAdded += (sender, args) =>
{
    Console.WriteLine($"Mount added: {args.Mount.Name} at {args.MountPoint.Path}");
};

vos.MountRemoved += (sender, args) =>
{
    Console.WriteLine($"Mount removed: {args.Mount.Name}");
};
```

## Mount Resolution

Understanding how VOS resolves paths through mounts.

### Resolution Process

1. Parse the path into segments
2. Starting from root, traverse each segment
3. At each node, check for mounts
4. If mounted, switch to the mounted data source
5. Continue traversal in the new context

### Resolution Example

```csharp
// Given mounts:
vos.Mount("/", "file:///app");
vos.Mount("/data", "redis://localhost");
vos.Mount("/data/archives", "file:///mnt/archives");

// Path resolution for "/data/archives/2024/backup.zip"
// 1. Start at filesystem root (/)
// 2. Switch to Redis at /data
// 3. Switch to filesystem at /data/archives
// 4. Continue in filesystem to /data/archives/2024/backup.zip
```

## Performance Considerations

### Mount Caching

VOS caches mount information for performance:

```csharp
// Configure mount cache
services.AddVos(options =>
{
    options.MountCacheTimeout = TimeSpan.FromMinutes(5);
    options.EnableMountCache = true;
});
```

### Lazy Mount Loading

Mounts can be configured to initialize lazily:

```csharp
vos.Mount("/large-data", "file:///mnt/bigdata", new MountOptions
{
    LazyLoad = true,
    Metadata = new Dictionary<string, object>
    {
        ["PreloadDepth"] = 0  // Don't preload any children
    }
});
```

## Security Considerations

### Read-Only Mounts

Enforce read-only access at the mount level:

```csharp
// System files as read-only
vos.Mount("/system", "file:///usr/share", new MountOptions
{
    IsReadOnly = true,
    DenyList = new[] { "*.key", "*.pem" }  // Additional filtering
});
```

### Access Control

Implement mount-level access control:

```csharp
public class SecureMount : Mount
{
    public override bool CanRead(IVob vob, IPrincipal user)
    {
        return user.IsInRole("Reader");
    }
    
    public override bool CanWrite(IVob vob, IPrincipal user)
    {
        return user.IsInRole("Writer");
    }
}
```

## Best Practices

1. **Mount Order**: Mount from general to specific
2. **Priority Planning**: Use consistent priority schemes
3. **Read-Only Safety**: Mount system resources as read-only
4. **Lazy Loading**: Use for large data sets
5. **Cleanup**: Unmount when no longer needed
6. **Error Handling**: Handle mount failures gracefully

```csharp
// Good practice example
try
{
    vos.Mount("/data", "file:///mnt/data");
}
catch (MountException ex)
{
    logger.LogError(ex, "Failed to mount data directory");
    // Fall back to alternative
    vos.Mount("/data", "file:///tmp/data");
}
```

## Common Patterns

### Configuration Overlay

```csharp
// Layer configuration sources
vos.Mount("/config", "file:///etc/app");        // System
vos.Mount("/config", "file://~/.config/app");   // User  
vos.Mount("/config", "env://APP_");             // Environment
```

### Multi-Tenant Data

```csharp
// Mount tenant-specific data
foreach (var tenant in tenants)
{
    vos.Mount($"/tenants/{tenant.Id}", 
        $"mongodb://localhost/app/tenants/{tenant.Id}");
}
```

### Archive Exploration

```csharp
// Mount archive for exploration
vos.Mount("/explore", "zip:///downloads/data.zip");

// Browse contents
await foreach (var child in vos["/explore"].Children)
{
    Console.WriteLine($"Found: {child.Key}");
}
```

## Next Steps

- Learn about [Persistence](./vos-persistence.md) mechanisms
- See [Examples](./vos-examples.md) for more use cases
- Review the [API Reference](./vos-api-reference.md) for detailed mount APIs