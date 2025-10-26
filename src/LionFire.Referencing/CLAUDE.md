# LionFire.Referencing

## Overview

**LionFire.Referencing** provides URL/URI handling and custom schema support for LionFire. This library enables type-safe references to objects using custom URI schemes, path-based addressing, and object reference tracking.

**Layer**: Core Toolkit (Reference System)
**Target**: .NET 9.0
**Root Namespace**: `LionFire.Referencing`

## Key Dependencies

### NuGet Packages
- **Microsoft.Extensions.Logging.Abstractions** - Logging support

### LionFire Dependencies
- **LionFire.Applications.Abstractions** - Application abstractions
- **LionFire.Core.Extras.Abstractions** - Core extras abstractions
- **LionFire.Core** - Core metadata and DI
- **LionFire.Persistence.Handles.Abstractions** - Handle abstractions
- **LionFire.Referencing.Abstractions** - Reference abstractions
- **LionFire.Structures** - Data structures

## Core Concepts

LionFire's referencing system allows objects to be addressed via URI-like references with custom schemas. Think of it as a generalized URL system where you can define custom protocols (schemes) for accessing different types of resources.

### Reference URL Format

```
scheme://authority/path?query#fragment
```

**Examples:**
- `vos://config/app.hjson` - Virtual Object System file
- `file:///C:/data/config.json` - Local file
- `http://api.example.com/users/42` - HTTP endpoint
- `db://localhost/users/42` - Database record
- `named://MyService` - Named service instance

### 1. Reference Types

**Location**: `References/`

#### IReference

Base interface for all references.

```csharp
public interface IReference
{
    string Scheme { get; }       // e.g., "vos", "file", "http"
    string Url { get; }          // Full URL string
    string Key { get; }          // Unique key for reference
}
```

#### ITypedReference

Reference with associated type information.

```csharp
public interface ITypedReference : IReference
{
    Type Type { get; }           // Type of referenced object
}
```

#### ReferenceBase

Abstract base class for custom references.

```csharp
public abstract class ReferenceBase<ConcreteType> : IReference, ITypedReference
    where ConcreteType : ReferenceBase<ConcreteType>
{
    public abstract Type Type { get; }
    public abstract string Scheme { get; }
    public abstract string Url { get; }
    public abstract IEnumerable<string> AllowedSchemes { get; }

    public bool IsCompatibleWith(string stringUrl);
    public virtual void ValidateScheme(string scheme);
}
```

**Usage:**
```csharp
public class VosReference : ReferenceBase<VosReference>
{
    public override string Scheme => "vos";
    public override Type Type => typeof(object);
    public override IEnumerable<string> AllowedSchemes => new[] { "vos" };

    public string Path { get; set; }

    public override string Url => $"vos://{Path}";

    public VosReference(string path)
    {
        Path = path;
    }
}

// Create reference
var reference = new VosReference("/config/app.hjson");
Console.WriteLine(reference.Url); // "vos:///config/app.hjson"
```

### 2. Common Reference Types

#### UriReference

Reference based on System.Uri.

```csharp
public class UriReference : ReferenceBase<UriReference>
{
    public Uri Uri { get; set; }

    public override string Scheme => Uri.Scheme;
    public override string Url => Uri.ToString();

    public UriReference(string url)
    {
        Uri = new Uri(url);
    }

    public UriReference(Uri uri)
    {
        Uri = uri;
    }
}
```

**Usage:**
```csharp
var reference = new UriReference("http://api.example.com/data");
Console.WriteLine(reference.Scheme); // "http"
Console.WriteLine(reference.Url);    // "http://api.example.com/data"
```

#### PathReference

Path-based reference for file-like hierarchies.

```csharp
public class PathReference : ReferenceBase<PathReference>
{
    public string Path { get; set; }
    public override string Url => $"{Scheme}://{Path}";

    public PathReference(string scheme, string path)
    {
        Scheme = scheme;
        Path = path;
    }
}
```

**Usage:**
```csharp
var fileRef = new PathReference("file", "/data/config.json");
var vosRef = new PathReference("vos", "/config/app.hjson");
```

#### NamedReference

Reference by logical name.

```csharp
public class NamedReference : ReferenceBase<NamedReference>
{
    public string Name { get; set; }
    public override string Url => $"named://{Name}";

    public NamedReference(string name)
    {
        Name = name;
    }
}

public class NamedReference<T> : NamedReference
{
    public override Type Type => typeof(T);

    public NamedReference(string name) : base(name) { }
}
```

**Usage:**
```csharp
var logger = new NamedReference<ILogger>("ApplicationLogger");
Console.WriteLine(logger.Url); // "named://ApplicationLogger"
```

#### LocalReference

Reference to local/in-memory objects.

```csharp
public abstract class LocalReferenceBase : ReferenceBase<LocalReferenceBase>
{
    public override string Scheme => "local";
    public abstract string LocalPath { get; }
}

public class LocalReference : LocalReferenceBase
{
    public string Path { get; set; }
    public override string LocalPath => Path;

    public LocalReference(string path)
    {
        Path = path;
    }
}
```

#### UriStringReference

Lightweight string-based URI reference.

```csharp
public class UriStringReference : ReferenceBase<UriStringReference>
{
    private readonly string url;

    public UriStringReference(string url)
    {
        this.url = url;
    }

    public override string Url => url;
    public override string Scheme => url.GetUriScheme();
}
```

#### SlimUriStringReference

Memory-efficient URI reference.

```csharp
public struct SlimUriStringReference : IReference
{
    private readonly string url;

    public SlimUriStringReference(string url)
    {
        this.url = url;
    }

    public string Url => url;
    public string Scheme => url.GetUriScheme();
    public string Key => url;
}
```

### 3. Object References

**Location**: `Objects/`

#### ObjectReference

Reference to a tracked object instance.

```csharp
public class ObjectReference<T> : IReference
{
    public T Object { get; }
    public string Url { get; }

    public ObjectReference(T obj, string url)
    {
        Object = obj;
        Url = url;
    }
}
```

#### Object Reference Registry

```csharp
public class ObjectReferenceRegistrar
{
    // Register object with reference
    public void Register(object obj, IReference reference);

    // Get reference for object
    public IReference? GetReference(object obj);

    // Get object for reference
    public object? GetObject(IReference reference);
}

public class ObjectWeakReferenceRegistrar
{
    // Same as above but with weak references
    // Objects can be garbage collected
}
```

**Usage:**
```csharp
var registrar = new ObjectReferenceRegistrar();

var user = new User { Id = 42, Name = "John" };
var reference = new UriReference("db://users/42");

registrar.Register(user, reference);

// Later, retrieve by reference
var retrievedUser = registrar.GetObject(reference);

// Or get reference for object
var retrievedRef = registrar.GetReference(user);
```

### 4. Reference Providers

**Location**: `Resolution/`

#### IReferenceProvider

Resolves references to objects.

```csharp
public interface IReferenceProvider<TReference>
    where TReference : IReference
{
    TReference GetReference(string url);
    TReference GetReference(Type type, string url);
}
```

#### ReferenceProviderBase

Base class for implementing reference providers.

```csharp
public abstract class ReferenceProviderBase<TReference> : IReferenceProvider<TReference>
    where TReference : IReference
{
    public abstract TReference GetReference(string url);

    public virtual TReference GetReference(Type type, string url)
    {
        var reference = GetReference(url);
        if (reference is ITypedReference typedRef && typedRef.Type != type)
        {
            throw new ArgumentException($"Reference type mismatch: expected {type}, got {typedRef.Type}");
        }
        return reference;
    }
}
```

#### ReferenceProviderService

Service for managing multiple reference providers.

```csharp
public class ReferenceProviderService
{
    // Register provider for scheme
    public void RegisterProvider(string scheme, IReferenceProvider provider);

    // Get reference from URL
    public IReference GetReference(string url);

    // Get reference for type
    public IReference<T> GetReference<T>(string url);
}
```

**Usage:**
```csharp
var service = new ReferenceProviderService();

// Register providers
service.RegisterProvider("vos", new VosReferenceProvider());
service.RegisterProvider("file", new FileReferenceProvider());
service.RegisterProvider("http", new HttpReferenceProvider());

// Resolve references
var vosRef = service.GetReference("vos:///config/app.hjson");
var fileRef = service.GetReference("file:///C:/data/config.json");
var httpRef = service.GetReference("http://api.example.com/data");
```

### 5. Overlaying References

**Location**: `References/Overlaying/`

Support for layered/overlayed references (used in VOS overlay system).

```csharp
public abstract class OverlayableReferenceBase : ReferenceBase<OverlayableReferenceBase>
{
    // Support for reference overlaying
}
```

### 6. URI Extensions

**Location**: `References/UriExtensions.cs`

Utility extensions for URI manipulation.

```csharp
public static class UriExtensions
{
    public static string GetUriScheme(this string url);
    public static string GetUriAuthority(this string url);
    public static string GetUriPath(this string url);

    public static Uri CombineUri(this Uri baseUri, string relativePath);
    public static string GetRelativePath(this Uri baseUri, Uri targetUri);
}
```

**Usage:**
```csharp
var scheme = "vos:///config/app.hjson".GetUriScheme(); // "vos"
var path = "vos:///config/app.hjson".GetUriPath();     // "/config/app.hjson"

var baseUri = new Uri("http://api.example.com/v1/");
var combined = baseUri.CombineUri("users/42");          // "http://api.example.com/v1/users/42"
```

### 7. Dependency Injection

**Location**: `Hosting/ReferenceProviderServicesExtensions.cs`

```csharp
public static class ReferenceProviderServicesExtensions
{
    public static IServiceCollection AddReferenceProvider<TProvider>(
        this IServiceCollection services)
        where TProvider : class, IReferenceProvider;

    public static IServiceCollection AddReferenceProviderService(
        this IServiceCollection services);
}
```

**Usage:**
```csharp
services
    .AddReferenceProviderService()
    .AddReferenceProvider<VosReferenceProvider>()
    .AddReferenceProvider<FileReferenceProvider>();
```

## Directory Structure

```
src/LionFire.Referencing/
├── Hosting/
│   └── ReferenceProviderServicesExtensions.cs  # DI extensions
├── Objects/
│   ├── ObjectReference.cs                      # Object reference tracking
│   ├── ObjectReferenceRegistrar.cs             # Object registry
│   ├── ObjectWeakReferenceRegistrar.cs         # Weak reference registry
│   ├── RegisteredObjectReference.cs            # Registered object ref
│   └── RegisteredObjectWeakReference.cs        # Weak registered ref
├── References/
│   ├── LocalReference.cs                       # Local references
│   ├── LocalReferenceBase.cs                   # Local reference base
│   ├── NamedReference.cs                       # Named references
│   ├── NamedReference{T}.cs                    # Generic named ref
│   ├── Overlaying/
│   │   └── OverlayableReferenceBase.cs         # Overlayable references
│   ├── PathReference.cs                        # Path-based references
│   ├── ReferenceBase.cs                        # Reference base class
│   ├── ReferenceBase.generic.cs                # Generic reference base
│   ├── ReferenceBaseBase.cs                    # Base of base
│   ├── SlimUriStringReference.cs               # Lightweight URI ref
│   ├── UriExtensions.cs                        # URI utilities
│   ├── UriReference.cs                         # System.Uri reference
│   └── UriStringReference.cs                   # String URI reference
├── Resolution/
│   ├── IReferenceProviderDependencyExtensions.cs # DI extensions
│   ├── IReferenceProviderServiceX.cs           # Provider service ext
│   ├── ReferenceProviderBase.cs                # Provider base class
│   └── ReferenceProviderService.cs             # Provider service
├── Resolvables/                                # Resolvable types
└── ReferenceObjectFactory.cs                   # Reference factory
```

## Design Patterns

### Custom Scheme Pattern

Define custom URI schemes for your domain:

```csharp
public class DatabaseReference : ReferenceBase<DatabaseReference>
{
    public override string Scheme => "db";
    public override Type Type => typeof(object);
    public override IEnumerable<string> AllowedSchemes => new[] { "db", "database" };

    public string ConnectionName { get; set; }
    public string Table { get; set; }
    public string Id { get; set; }

    public override string Url => $"db://{ConnectionName}/{Table}/{Id}";

    public DatabaseReference(string connectionName, string table, string id)
    {
        ConnectionName = connectionName;
        Table = table;
        Id = id;
    }
}

// Usage
var userRef = new DatabaseReference("main", "users", "42");
Console.WriteLine(userRef.Url); // "db://main/users/42"
```

### Reference Provider Pattern

Create providers for custom schemes:

```csharp
public class DatabaseReferenceProvider : ReferenceProviderBase<DatabaseReference>
{
    public override DatabaseReference GetReference(string url)
    {
        if (!url.StartsWith("db://"))
            throw new ArgumentException("Invalid database reference URL");

        // Parse URL: db://connection/table/id
        var parts = url.Substring(5).Split('/');

        return new DatabaseReference(
            connectionName: parts[0],
            table: parts[1],
            id: parts[2]
        );
    }
}

// Register
services.AddReferenceProvider<DatabaseReferenceProvider>();

// Use
var refService = services.GetRequiredService<ReferenceProviderService>();
var dbRef = refService.GetReference("db://main/users/42") as DatabaseReference;
```

### Object Tracking Pattern

Track object instances by reference:

```csharp
public class EntityTracker
{
    private readonly ObjectReferenceRegistrar registrar = new();

    public void Track<T>(T entity, string url)
    {
        var reference = new UriReference(url);
        registrar.Register(entity, reference);
    }

    public IReference? GetReference<T>(T entity)
    {
        return registrar.GetReference(entity);
    }

    public T? GetEntity<T>(string url)
    {
        var reference = new UriReference(url);
        return (T?)registrar.GetObject(reference);
    }
}

// Usage
var tracker = new EntityTracker();

var user = new User { Id = 42, Name = "John" };
tracker.Track(user, "db://users/42");

// Later, retrieve
var reference = tracker.GetReference(user);
var retrievedUser = tracker.GetEntity<User>("db://users/42");
```

## Common Usage Patterns

### Pattern 1: Multi-Scheme References

```csharp
public class ResourceReference
{
    private readonly IReference reference;

    public ResourceReference(string url)
    {
        reference = url.GetUriScheme() switch
        {
            "vos" => new VosReference(url),
            "file" => new UriReference(url),
            "http" or "https" => new UriReference(url),
            "named" => new NamedReference(url.Substring(8)),
            _ => throw new NotSupportedException($"Unsupported scheme in URL: {url}")
        };
    }

    public string Url => reference.Url;
    public string Scheme => reference.Scheme;
}
```

### Pattern 2: Reference Equality

```csharp
public class ReferenceComparer : IEqualityComparer<IReference>
{
    public bool Equals(IReference? x, IReference? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        return x.Url == y.Url;
    }

    public int GetHashCode(IReference obj)
    {
        return obj.Key.GetHashCode();
    }
}

// Usage
var set = new HashSet<IReference>(new ReferenceComparer());
set.Add(new VosReference("/config/app.hjson"));
set.Add(new VosReference("/config/app.hjson")); // Duplicate, not added
```

### Pattern 3: Reference Resolution Chain

```csharp
public class ChainedReferenceProvider : IReferenceProvider<IReference>
{
    private readonly List<IReferenceProvider> providers = new();

    public void AddProvider(IReferenceProvider provider)
    {
        providers.Add(provider);
    }

    public IReference GetReference(string url)
    {
        foreach (var provider in providers)
        {
            try
            {
                return provider.GetReference(url);
            }
            catch
            {
                // Try next provider
            }
        }

        throw new InvalidOperationException($"No provider could resolve: {url}");
    }
}
```

## Integration with Other Libraries

### With LionFire.Persistence

References are used to identify persistence targets:

```csharp
var reference = new VosReference("/data/config.hjson");
var handle = persistence.GetHandle<AppConfig>(reference);
var config = await handle.Get();
```

### With LionFire.Vos

VOS heavily relies on references:

```csharp
var vob = vos.Get<MyData>("/path/to/data");
Console.WriteLine(vob.Reference.Url); // "vos:///path/to/data"
```

## Testing Considerations

### Testing Custom References

```csharp
[Fact]
public void CustomReference_GeneratesCorrectUrl()
{
    var reference = new DatabaseReference("main", "users", "42");

    Assert.Equal("db", reference.Scheme);
    Assert.Equal("db://main/users/42", reference.Url);
}

[Fact]
public void ReferenceBase_Equality_WorksCorrectly()
{
    var ref1 = new VosReference("/config/app.hjson");
    var ref2 = new VosReference("/config/app.hjson");
    var ref3 = new VosReference("/config/other.hjson");

    Assert.Equal(ref1, ref2);
    Assert.NotEqual(ref1, ref3);
}
```

### Mocking Reference Providers

```csharp
var mockProvider = new Mock<IReferenceProvider<VosReference>>();
mockProvider.Setup(x => x.GetReference(It.IsAny<string>()))
    .Returns((string url) => new VosReference(url.Substring(6)));

var reference = mockProvider.Object.GetReference("vos:///test");

Assert.Equal("/test", reference.Path);
```

## Related Projects

- **LionFire.Referencing.Abstractions** - Reference abstractions
- **LionFire.Persistence** - Persistence framework using references
- **LionFire.Vos** - Virtual Object System with VOS references
- **LionFire.Core** - Core metadata and DI

## Documentation

- **URI Specification**: https://tools.ietf.org/html/rfc3986
- **Custom URI Schemes**: https://www.iana.org/assignments/uri-schemes/uri-schemes.xhtml

## Summary

**LionFire.Referencing** provides a comprehensive reference system:

- **Custom URI Schemes**: Define domain-specific URI schemes
- **Reference Types**: UriReference, PathReference, NamedReference, LocalReference
- **Object Tracking**: Associate objects with references
- **Reference Providers**: Resolve URLs to reference objects
- **URI Utilities**: Parse and manipulate URIs
- **Type Safety**: Generic references with type information
- **DI Integration**: Service registration for providers

**Key Strengths:**
- Flexible URI-based addressing
- Custom scheme support
- Object reference tracking
- Provider pattern for extensibility
- Type-safe references

**Use When:**
- Need custom URI schemes
- Want object reference tracking
- Require type-safe URL handling
- Building resource addressing systems

**Typical Use Cases:**
- Virtual Object System (VOS) references
- Database record addressing
- API endpoint references
- File system paths
- Named service resolution
- Multi-protocol resource access

**Note**: This library is foundational for LionFire.Vos and LionFire.Persistence, providing the reference system used throughout the ecosystem.
