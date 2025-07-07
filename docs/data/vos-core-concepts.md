# VOS Core Concepts

This document explains the fundamental concepts and terminology used throughout the Virtual Object System.

## Vob (Virtual Object)

A **Vob** is the fundamental unit in VOS - a node in the virtual tree that can represent data, a container, or both.

### Key Characteristics:
- **Hierarchical**: Vobs form a tree structure with parent-child relationships
- **Addressable**: Each Vob has a unique path in the tree
- **Flexible**: Can hold data of any type through the IFlex interface
- **Lazy**: Data and children are loaded on-demand
- **Layered**: Can aggregate data from multiple sources

### Vob Types:
- **Container Vobs**: Act like directories, containing child Vobs
- **Data Vobs**: Hold actual data values
- **Hybrid Vobs**: Both container and data holder

```csharp
// Access a Vob
IVob configVob = vos["/config"];

// Get children
foreach (var child in configVob.Children)
{
    Console.WriteLine($"Child: {child.Key} at {child.Value.Path}");
}

// Access data
var handle = configVob.GetReadHandle<ConfigData>();
var data = await handle.GetValue();
```

## References

A **Reference** is a pointer to a location in the VOS tree, similar to a URL or file path.

### IVobReference
The primary reference type for VOS locations.

**Components:**
- **Scheme**: Protocol identifier (e.g., "vos")
- **Path**: Location in the virtual tree
- **Type**: Optional type information
- **Filters**: Query parameters for filtering

**Examples:**
```csharp
// Simple path reference
IVobReference ref1 = new VobReference("/data/users/john");

// Typed reference
IVobReference<User> ref2 = new VobReference<User>("/data/users/john");

// Reference with filters
var ref3 = new VobReference("/data/users", 
    filters: new[] { ("role", "admin") });
```

### Reference Resolution
References are resolved to Vobs through the VOS tree traversal:

1. Parse the reference path
2. Start from the appropriate root
3. Traverse through child nodes
4. Apply any filters
5. Return the target Vob

## Mounts

A **Mount** connects an external data source to a location in the VOS tree.

### Mount Properties:
- **MountPoint**: The Vob where the mount is attached
- **Target**: Reference to the external data source
- **Options**: Configuration for the mount
- **IsEnabled**: Can be toggled on/off

### Mount Types:

#### 1. Filesystem Mount
Maps a directory to a VOS path:
```csharp
// Mount local filesystem
vos.Mount("/data", "file:///var/app/data");
```

#### 2. Archive Mount
Provides access to archive contents:
```csharp
// Mount ZIP file
vos.Mount("/backup", "zip:///backups/data.zip");
```

#### 3. Database Mount
Maps database collections:
```csharp
// Mount database
vos.Mount("/users", "mongodb://localhost/mydb/users");
```

#### 4. Overlay Mount
Layers multiple sources:
```csharp
// Layer configuration sources
vos.Mount("/config", "overlay://defaults,user,environment");
```

## Handles

**Handles** provide typed access to data stored in Vobs with specific access patterns.

### Handle Types:

#### IReadHandle<T>
Read-only access to data:
```csharp
IReadHandle<Settings> handle = vob.GetReadHandle<Settings>();
Settings settings = await handle.GetValue();
```

#### IReadWriteHandle<T>
Full read/write access:
```csharp
IReadWriteHandle<User> handle = vob.GetReadWriteHandle<User>();
User user = await handle.GetValue();
user.Name = "Updated";
await handle.SetValue(user);
```

#### IWriteHandle<T>
Write-only access:
```csharp
IWriteHandle<LogEntry> handle = vob.GetWriteHandle<LogEntry>();
await handle.SetValue(new LogEntry { Message = "Event" });
```

### Handle Features:
- **Type Safety**: Compile-time type checking
- **Lazy Loading**: Data loaded on first access
- **Caching**: Automatic caching of values
- **Async Operations**: All I/O is asynchronous

## Persistence

The **Persistence** layer manages how data is stored and retrieved from various sources.

### IPersister
Interface implemented by all persistence providers:

```csharp
public interface IPersister<TReference>
{
    Task<IGetResult<T>> Get<T>(TReference reference);
    Task<IPutResult> Put<T>(TReference reference, T value);
    Task<IDeleteResult> Delete(TReference reference);
    Task<IExistsResult> Exists(TReference reference);
}
```

### Persistence Flow:
1. Handle requests data operation
2. Mount determines appropriate persister
3. Persister performs I/O operation
4. Data is serialized/deserialized
5. Result returned through handle

## Layers and Acquisition

VOS supports **layered data access** where multiple sources can contribute to a single logical view.

### Acquisition
The process of searching up the tree for inherited or default values:

```csharp
// Search up the tree for configuration
var config = vob.Acquire<IConfiguration>();

// Search with depth limits
var settings = vob.Acquire<Settings>(minDepth: 1, maxDepth: 3);
```

### Multi-Layer Support
Multiple data sources can be overlaid:

```csharp
// Get all layers of a type
var allConfigs = await vob.AllLayersOfType<Config>();

// Merge configurations from multiple layers
var merged = ConfigMerger.Merge(allConfigs);
```

## Path System

VOS uses a Unix-like path system with some extensions.

### Path Components:
- `/` - Root separator
- `.` - Current node
- `..` - Parent node (or special roots)
- `%` - Type specifier

### Special Paths:
- `/` - Default root
- `/..` - Root of roots
- `/../namedRoot/` - Access named root

### Path Examples:
```
/data/users/john           - Simple path
/data/users%User/john      - Path with type hint
/../assets/images/logo.png - Named root access
./config                   - Relative path
```

## Service Integration

VOS integrates with dependency injection and service providers.

### Service Provider Access
Vobs can access services through the service provider:

```csharp
public class VobWithServices : Vob
{
    public ILogger Logger => ServiceProvider.GetService<ILogger>();
}
```

### Extension Methods
Many VOS operations are available as extension methods:

```csharp
// Extension methods on IVos
var vob = vos.GetVob("/path");

// Extension methods on IVob  
var handle = vob.GetHandle<T>();
var child = vob.Child("subpath");
```

## Type System

VOS uses several type-related interfaces:

### IFlex
Provides flexible, multi-type object support:
```csharp
// Add typed data to a Vob
vob.AddType<UserProfile>(profile);
vob.AddType<Permissions>(permissions);

// Retrieve typed data
var profile = vob.GetType<UserProfile>();
```

### IMultiTyped
Supports multiple type facets on a single object:
```csharp
// Check for type support
if (vob.HasType<IConfigurable>())
{
    var configurable = vob.AsType<IConfigurable>();
}
```

## Events and Notifications

VOS supports event notifications for data changes:

```csharp
// Subscribe to changes
vob.Changed += (sender, args) => 
{
    Console.WriteLine($"Vob changed: {args.Path}");
};

// Handle child additions
vob.ChildAdded += (sender, child) => 
{
    Console.WriteLine($"Child added: {child.Name}");
};
```

## Best Practices

1. **Use Typed References**: Prefer `VobReference<T>` for type safety
2. **Handle Disposal**: Dispose handles when done to free resources
3. **Async Operations**: Always use async methods for I/O
4. **Error Handling**: Check operation results for success
5. **Caching**: Configure appropriate cache timeouts
6. **Lazy Loading**: Don't enumerate children unnecessarily

## Next Steps

- See the [API Reference](./vos-api-reference.md) for detailed API documentation
- Learn about [Mounting](./vos-mounting-system.md) data sources
- Understand [Persistence](./vos-persistence.md) options