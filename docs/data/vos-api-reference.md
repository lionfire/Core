# VOS API Reference

This document provides a comprehensive API reference for the Virtual Object System.

## IVos Interface

The root interface for accessing the VOS system.

```csharp
public interface IVos : IHas<IVos>, IHas<IServiceProvider>
{
    // Get a named root (null for default root)
    IRootVob? Get(string? rootName = null);
    
    // Access VOS configuration options
    VosOptions Options { get; }
}
```

### Extension Methods

```csharp
// Get a Vob by path
IVob GetVob(this IVos vos, string vobPath)
IVob GetVob(this IVos vos, IVobReference vobReference)

// Try to get a Vob (returns null if not found)
IVob? TryGetVob(this IVos vos, string vobPath)
IVob? TryGetVob(this IVos vos, IVobReference vobReference)
```

### Usage Example
```csharp
var vos = serviceProvider.GetRequiredService<IVos>();
var dataVob = vos.GetVob("/data");
var configVob = vos.TryGetVob("/config") ?? vos.GetVob("/defaults/config");
```

## IVob Interface

The core interface representing a virtual object in the tree.

```csharp
public interface IVob : 
    IReferenceable<IVobReference>,
    IEnumerable<IVob>,
    IFlex,
    IAcquires,
    IParented<IVob>,
    IKeyed
{
    // Basic properties
    string Name { get; }
    string Path { get; }
    IRootVob Root { get; }
    
    // Path information
    IEnumerable<string> PathElements { get; }
    IEnumerable<string> PathElementsReverse { get; }
    
    // Node acquisition
    T Acquire<T>(int minDepth = 0, int maxDepth = -1) where T : class;
    T AcquireOwn<T>() where T : class;
    IEnumerable<T> AcquireEnumerator<T>(int minDepth = 0, int maxDepth = -1) where T : class;
    
    // Child access
    IEnumerable<KeyValuePair<string, IVob>> Children { get; }
    IVob QueryChild(string subpath);
    IVob QueryChild(string[] subpathChunks, int index = 0);
    IVob QueryChild(IVobReference reference);
    IVob GetChild(string subpath);
    IVob GetChild(IEnumerator<string> subpathChunks);
    IVob GetChild(string[] subpathChunks, int index = 0);
    
    // Indexers
    IVob this[string subpath] { get; }
    IVob this[string[] subpathChunks, int index = 0] { get; }
    IVob this[IVobReference reference] { get; }
    
    // Handle creation
    IReadHandle<T> GetReadHandle<T>(T preresolvedValue = default);
    IReadWriteHandle<T> GetReadWriteHandle<T>(T preresolvedValue = default);
    IWriteHandle<T> GetWriteHandle<T>(T prestagedValue = default);
    
    // Layer operations
    Task<IEnumerable<T>> AllLayersOfType<T>();
}
```

### Child Access Methods

#### QueryChild vs GetChild
- **QueryChild**: Returns existing child or null
- **GetChild**: Returns existing child or creates new one

```csharp
// QueryChild - returns null if not exists
var existing = parentVob.QueryChild("config");
if (existing != null) { /* use existing */ }

// GetChild - creates if not exists
var child = parentVob.GetChild("config");
// child is never null
```

### Acquisition Methods

Search up the tree hierarchy for objects:

```csharp
// Find first ILogger up the tree
var logger = vob.Acquire<ILogger>();

// Search with depth constraints
var config = vob.Acquire<IConfiguration>(
    minDepth: 1,  // Skip self
    maxDepth: 3   // Don't go more than 3 levels up
);

// Get from self only
var ownService = vob.AcquireOwn<IMyService>();

// Get all instances up the tree
foreach (var handler in vob.AcquireEnumerator<IEventHandler>())
{
    handler.Handle(evt);
}
```

## IVobReference Interface

References provide addressing for Vobs.

```csharp
public interface IVobReference : 
    IReference, 
    ITypedReference, 
    IReferenceable<IVobReference>, 
    IPersisterReference
{
    // Create typed reference
    IVobReference<T> ForType<T>();
    
    // Allowed URI schemes
    IEnumerable<string> AllowedSchemes { get; }
    
    // Path components
    string[] PathChunks { get; }
    
    // Filters for queries
    ImmutableList<KeyValuePair<string, string>> Filters { get; set; }
    
    // Child reference
    IVobReference this[string childPath] { get; }
}

public interface IVobReference<out TValue> : IVobReference, IReference<TValue>
{
}
```

### Creating References

```csharp
// String implicit conversion
IVobReference ref1 = "/data/users";

// Explicit construction
var ref2 = new VobReference("/data/users");

// Typed reference
var ref3 = new VobReference<User>("/data/users/john");

// With filters
var ref4 = new VobReference("/data/users",
    filters: ImmutableList.Create(
        new KeyValuePair<string, string>("role", "admin"),
        new KeyValuePair<string, string>("active", "true")
    ));

// From Vob
IVobReference ref5 = myVob.Reference;
```

## Handle Interfaces

Handles provide typed data access with specific access patterns.

### IReadHandle<T>

```csharp
public interface IReadHandle<T> : IDefaultable<T>
{
    // Get the value (async)
    Task<T> GetValue();
    
    // Try to get value synchronously
    bool TryGetValue(out T value);
    
    // Check if value exists
    Task<bool> Exists();
    
    // Value changed event
    event EventHandler<ValueChangedEventArgs<T>> ValueChanged;
}
```

### IReadWriteHandle<T>

```csharp
public interface IReadWriteHandle<T> : IReadHandle<T>, IWriteHandle<T>
{
    // Inherits all read and write operations
}
```

### IWriteHandle<T>

```csharp
public interface IWriteHandle<T>
{
    // Set the value
    Task<ISetResult> SetValue(T value);
    
    // Delete the value
    Task<IDeleteResult> Delete();
    
    // Discard staged changes
    void DiscardValue();
}
```

### Handle Usage Examples

```csharp
// Reading data
var readHandle = vob.GetReadHandle<Configuration>();
if (await readHandle.Exists())
{
    var config = await readHandle.GetValue();
    // Use config
}

// Writing data
var writeHandle = vob.GetWriteHandle<UserProfile>();
var result = await writeHandle.SetValue(new UserProfile 
{ 
    Name = "John",
    Email = "john@example.com" 
});

if (result.IsSuccess)
{
    Console.WriteLine("Profile saved");
}

// Read-write operations
var handle = vob.GetReadWriteHandle<Settings>();
var settings = await handle.GetValue();
settings.Theme = "dark";
await handle.SetValue(settings);
```

## Mount Interfaces

### IMount

```csharp
public interface IMount : IParented<IMount>
{
    // The Vob where this is mounted
    IVob MountPoint { get; }
    
    // Reference to the mounted resource
    IReference Target { get; }
    
    // Enable/disable the mount
    bool IsEnabled { get; set; }
    
    // Mount configuration
    IVobMountOptions Options { get; }
    
    // Depth in the Vob tree
    int VobDepth { get; }
    
    // Parent mount if layered
    IMount UpstreamMount { get; }
}
```

### Mount Configuration

```csharp
public interface IVobMountOptions
{
    string Name { get; set; }
    bool IsExclusive { get; set; }
    bool IsReadOnly { get; set; }
    int Priority { get; set; }
    IMount UpstreamMount { get; set; }
}
```

## Persistence Interfaces

### IPersister<TReference>

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

### Result Interfaces

```csharp
public interface IGetResult<T>
{
    bool IsSuccess { get; }
    T Value { get; }
    Exception Error { get; }
}

public interface IPutResult
{
    bool IsSuccess { get; }
    Exception Error { get; }
    string ETag { get; }
}

public interface IDeleteResult
{
    bool IsSuccess { get; }
    Exception Error { get; }
}
```

## Configuration Classes

### VosOptions

```csharp
public class VosOptions
{
    // Named root configurations
    public Dictionary<string, VobRootOptions> Roots { get; }
    
    // Global settings
    public bool EnableCaching { get; set; }
    public TimeSpan CacheTimeout { get; set; }
    public bool EnableAutoMount { get; set; }
}
```

### VobRootOptions

```csharp
public class VobRootOptions
{
    public string Name { get; set; }
    public string MountPath { get; set; }
    public List<MountOptions> Mounts { get; set; }
}
```

## Extension Methods

### ServiceCollection Extensions

```csharp
// Add VOS to DI container
services.AddVos(options => 
{
    options.EnableCaching = true;
    options.CacheTimeout = TimeSpan.FromMinutes(5);
});

// Add named roots
services.AddVosRoot("data", options => 
{
    options.MountPath = "/app/data";
});

// Add persisters
services.AddVosFilesystem();
services.AddVosRedis();
services.AddVosLiteDB();
```

### Vob Extensions

```csharp
// Path manipulation
string GetRelativePath(this IVob vob, IVob relativeTo)
bool IsAncestorOf(this IVob vob, IVob other)
bool IsDescendantOf(this IVob vob, IVob other)

// Handle shortcuts
Task<T> Get<T>(this IVob vob)
Task Set<T>(this IVob vob, T value)
Task<bool> Delete(this IVob vob)

// Tree traversal
IEnumerable<IVob> Ancestors(this IVob vob)
IEnumerable<IVob> Descendants(this IVob vob)
IVob FindChild(this IVob vob, Func<IVob, bool> predicate)
```

## Async Operations

Most VOS operations support async/await:

```csharp
// Async data access
var handle = vob.GetReadHandle<Data>();
var data = await handle.GetValue();

// Async enumeration
await foreach (var layer in vob.AllLayersOfType<Config>())
{
    ProcessLayer(layer);
}

// Async persistence
var result = await persister.Put(reference, value);
```

## Error Handling

VOS uses result objects and exceptions:

```csharp
try 
{
    var handle = vob.GetReadHandle<Config>();
    var config = await handle.GetValue();
}
catch (VobNotFoundException ex)
{
    // Handle missing Vob
}
catch (PersistenceException ex)
{
    // Handle storage errors
}
catch (SerializationException ex)
{
    // Handle serialization errors
}
```

## Thread Safety

- IVob instances are thread-safe for read operations
- Write operations should be synchronized by the caller
- Handles are not thread-safe - use one per thread
- The IVos root manager is fully thread-safe

## Next Steps

- Learn about the [Mounting System](./vos-mounting-system.md)
- Understand [Persistence](./vos-persistence.md) options
- See [Examples](./vos-examples.md) for practical usage