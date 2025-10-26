# LionFire.Structures

## Overview

**LionFire.Structures** provides collection types, data structures, and structural patterns for LionFire applications. This library focuses on reusable data structures, collection adapters, and structural interfaces that build on LionFire.Base.

**Layer**: Base Toolkit (Data Structures)
**Target**: .NET 9.0
**Root Namespace**: `LionFire`
**Package Tags**: collections, structures, utility

## Key Dependencies

### LionFire Dependencies
- **LionFire.Base** - BCL augmentation
- **LionFire.Flex** - FlexData pattern for runtime extensibility

**No external NuGet dependencies** - keeps this library lightweight.

## Core Components

### 1. Collection Adapters

**Location**: `Collections/Adapters/`

Adapters for transforming collections between different types.

**Key Interfaces:**

```csharp
// Base adapter interface
public interface ICollectionAdapter<TSource, TTarget>
{
    IEnumerable<TTarget> AdaptCollection(IEnumerable<TSource> source);
}

// Read-only collection adapter
public interface IReadOnlyCollectionAdapter<TSource, TTarget>
{
    IReadOnlyCollection<TTarget> Adapt(IReadOnlyCollection<TSource> source);
}
```

**CollectionAdapter Base:**

```csharp
public abstract class CollectionAdapterBase<TSource, TTarget>
    : ICollectionAdapter<TSource, TTarget>
{
    protected abstract TTarget Transform(TSource item);

    public IEnumerable<TTarget> AdaptCollection(IEnumerable<TSource> source)
    {
        return source.Select(Transform);
    }
}
```

**Usage:**
```csharp
public class UserToUserDtoAdapter : CollectionAdapterBase<User, UserDto>
{
    protected override UserDto Transform(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}

// Adapt collection
var users = new List<User> { user1, user2, user3 };
var adapter = new UserToUserDtoAdapter();
var dtos = adapter.AdaptCollection(users);
```

**CollectionAdapterFactory:**

```csharp
public class CollectionAdapterFactory
{
    public ICollectionAdapter<TSource, TTarget> CreateAdapter<TSource, TTarget>(
        Func<TSource, TTarget> transform)
    {
        return new DelegateCollectionAdapter<TSource, TTarget>(transform);
    }
}
```

### 2. Concurrent Collections

**Location**: `Collections/Concurrent/`

Thread-safe collection types.

```csharp
// Concurrent collection interfaces
public interface IConcurrentCollection<T> : ICollection<T>
{
    bool TryAdd(T item);
    bool TryRemove(T item);
}

public interface IConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    bool TryAdd(TKey key, TValue value);
    bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue);
    TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory);
}
```

### 3. Hierarchical Collections

**Location**: `Collections/Hierarchical/`

Tree and hierarchical data structures.

```csharp
public interface IHierarchical<T>
{
    T? Parent { get; }
    IEnumerable<T> Children { get; }
}

public interface ITreeNode<T>
{
    T Value { get; }
    ITreeNode<T>? Parent { get; }
    IList<ITreeNode<T>> Children { get; }
}

public static class HierarchicalExtensions
{
    // Get all ancestors
    public static IEnumerable<T> Ancestors<T>(this IHierarchical<T> node)
        where T : IHierarchical<T>
    {
        var current = node.Parent;
        while (current != null)
        {
            yield return current;
            current = current.Parent;
        }
    }

    // Get all descendants
    public static IEnumerable<T> Descendants<T>(this IHierarchical<T> node)
        where T : IHierarchical<T>
    {
        foreach (var child in node.Children)
        {
            yield return child;
            foreach (var descendant in child.Descendants())
            {
                yield return descendant;
            }
        }
    }
}
```

**Usage:**
```csharp
public class FileSystemNode : IHierarchical<FileSystemNode>
{
    public string Name { get; set; }
    public FileSystemNode? Parent { get; set; }
    public List<FileSystemNode> Children { get; set; } = new();

    IEnumerable<FileSystemNode> IHierarchical<FileSystemNode>.Children => Children;
}

var root = new FileSystemNode { Name = "root" };
var folder1 = new FileSystemNode { Name = "folder1", Parent = root };
root.Children.Add(folder1);

// Get all descendants
var allNodes = root.Descendants();
```

### 4. Notification Collections

**Location**: `Collections/Notification/`

Collections with change notifications.

```csharp
public interface INotifyCollectionChanged
{
    event NotifyCollectionChangedEventHandler? CollectionChanged;
}

public interface IObservableCollection<T> : ICollection<T>, INotifyCollectionChanged
{
}
```

### 5. Acquisition Pattern

**Location**: `Acquisition/`

Lazy acquisition and weak reference caching.

**Acquisitor:**

```csharp
public interface IAcquires<TKey, TValue>
{
    TValue Acquire(TKey key);
    bool TryAcquire(TKey key, out TValue value);
}

public class Acquisitor<TKey, TValue>
    where TKey : notnull
{
    private readonly Func<TKey, TValue> factory;
    private readonly Dictionary<TKey, TValue> cache = new();

    public Acquisitor(Func<TKey, TValue> factory)
    {
        this.factory = factory;
    }

    public TValue Acquire(TKey key)
    {
        if (!cache.TryGetValue(key, out var value))
        {
            value = factory(key);
            cache[key] = value;
        }
        return value;
    }
}
```

**Weak Reference Caching:**

```csharp
public class AcquisitorWeakTable<TKey, TValue>
    where TKey : class
    where TValue : class
{
    private readonly Func<TKey, TValue> factory;
    private readonly ConditionalWeakTable<TKey, TValue> cache = new();

    public TValue Acquire(TKey key)
    {
        return cache.GetValue(key, factory);
    }
}
```

**Usage:**
```csharp
// Cache expensive computations
var acquisitor = new Acquisitor<int, ExpensiveObject>(
    id => new ExpensiveObject(id));

var obj1 = acquisitor.Acquire(42); // Creates new
var obj2 = acquisitor.Acquire(42); // Returns cached
Assert.Same(obj1, obj2);
```

### 6. FlexObjects

**Location**: `FlexObjects/`

Flexible object pattern for runtime extensibility.

```csharp
public interface IFlex
{
    object? this[string key] { get; set; }
    bool TryGet(string key, out object? value);
}

public interface IFlexProvider
{
    IFlex Flex { get; }
}

public class FlexObject : IFlex
{
    private Dictionary<string, object?> data = new();

    public object? this[string key]
    {
        get => data.TryGetValue(key, out var value) ? value : null;
        set => data[key] = value;
    }

    public bool TryGet(string key, out object? value)
    {
        return data.TryGetValue(key, out value);
    }
}
```

**Usage:**
```csharp
var flex = new FlexObject();

// Store arbitrary data
flex["CustomProperty"] = "Custom Value";
flex["Metadata"] = new Dictionary<string, string>();

// Retrieve
if (flex.TryGet("CustomProperty", out var value))
{
    Console.WriteLine(value);
}
```

### 7. Keys and Identifiers

**Location**: `Keys/`, `Keys/Id/`

Key selector patterns and identifier types.

**Key Selectors:**

```csharp
public interface IKeySelector<TSource, TKey>
{
    TKey SelectKey(TSource item);
}

public class PropertyKeySelector<TSource, TKey> : IKeySelector<TSource, TKey>
{
    private readonly Func<TSource, TKey> selector;

    public PropertyKeySelector(Func<TSource, TKey> selector)
    {
        this.selector = selector;
    }

    public TKey SelectKey(TSource item) => selector(item);
}
```

**ID Types:**

```csharp
public interface IHasId<TId>
{
    TId Id { get; }
}

public interface IKeyed<TKey>
{
    TKey Key { get; }
}
```

### 8. Composables

**Location**: `Composables/`

Composition patterns for combining behaviors.

```csharp
public interface IComposable<T>
{
    IEnumerable<T> Components { get; }
}

public interface IComposite<T> : IComposable<T>
{
    void AddComponent(T component);
    void RemoveComponent(T component);
}
```

### 9. CQRS Patterns

**Location**: `Cqrs/`

Command Query Responsibility Segregation patterns.

```csharp
public interface ICommand
{
}

public interface IQuery<TResult>
{
}

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    Task HandleAsync(TCommand command);
}

public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query);
}
```

### 10. Data Abstractions

**Location**: `Data/`

Generic data access patterns.

```csharp
public interface IReadable<T>
{
    T Read();
}

public interface IWriteable<T>
{
    void Write(T value);
}

public interface IReadWrite<T> : IReadable<T>, IWriteable<T>
{
}
```

### 11. Disposable Patterns

**Location**: `Disposable/`

Enhanced disposable patterns.

```csharp
public class DisposableCollection : IDisposable
{
    private readonly List<IDisposable> disposables = new();

    public void Add(IDisposable disposable)
    {
        disposables.Add(disposable);
    }

    public void Dispose()
    {
        foreach (var disposable in disposables)
        {
            disposable?.Dispose();
        }
        disposables.Clear();
    }
}

public static class DisposableExtensions
{
    public static T DisposeWith<T>(this T disposable, DisposableCollection collection)
        where T : IDisposable
    {
        collection.Add(disposable);
        return disposable;
    }
}
```

**Usage:**
```csharp
var disposables = new DisposableCollection();

var stream = File.OpenRead("file.txt").DisposeWith(disposables);
var reader = new StreamReader(stream).DisposeWith(disposables);

// All disposed together
disposables.Dispose();
```

### 12. Events

**Location**: `Events/`

Event pattern abstractions.

```csharp
public interface IEventSource<TEvent>
{
    event EventHandler<TEvent>? Event;
}

public interface IEventSink<TEvent>
{
    void RaiseEvent(TEvent eventArgs);
}
```

### 13. Execution Patterns

**Location**: `Execution/`

Execution state and control.

```csharp
public interface IExecutable
{
    void Execute();
}

public interface IExecutableAsync
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
```

### 14. Formatting

**Location**: `Formatting/`

Formatting abstractions.

```csharp
public interface IFormatter<T>
{
    string Format(T value);
}

public interface IParser<T>
{
    T Parse(string input);
    bool TryParse(string input, out T result);
}
```

### 15. Instantiating

**Location**: `Instantiating/`

Object instantiation patterns.

```csharp
public interface IInstantiator<T>
{
    T Create();
}

public interface IFactory<TKey, TValue>
{
    TValue Create(TKey key);
}
```

### 16. Ontology

**Location**: `Ontology/`

Ontology and classification patterns.

```csharp
public interface IClassifiable
{
    string Classification { get; }
}

public interface ITaggable
{
    IEnumerable<string> Tags { get; }
}
```

### 17. Options

**Location**: `Options/`

Options pattern support.

```csharp
public interface IHasOptions<T>
{
    T Options { get; }
}
```

### 18. Results

**Location**: `Results/`

Result types for operation outcomes.

```csharp
public interface IResult
{
    bool IsSuccess { get; }
    string? ErrorMessage { get; }
}

public interface IResult<T> : IResult
{
    T? Value { get; }
}

public class Result : IResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }

    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
}

public class Result<T> : Result, IResult<T>
{
    public T? Value { get; init; }

    public static Result<T> Success(T value) =>
        new() { IsSuccess = true, Value = value };

    public static new Result<T> Failure(string error) =>
        new() { IsSuccess = false, ErrorMessage = error };
}
```

**Usage:**
```csharp
public Result<User> GetUser(int id)
{
    var user = database.FindUser(id);

    if (user == null)
        return Result<User>.Failure("User not found");

    return Result<User>.Success(user);
}

// Usage
var result = GetUser(42);
if (result.IsSuccess)
{
    Console.WriteLine($"Found: {result.Value.Name}");
}
else
{
    Console.WriteLine($"Error: {result.ErrorMessage}");
}
```

### 19. Singletons

**Location**: `Singletons/`

Singleton pattern helpers.

```csharp
public interface ISingleton
{
}

public static class Singleton<T>
    where T : class, new()
{
    private static T? instance;
    private static readonly object lockObj = new();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    instance ??= new T();
                }
            }
            return instance;
        }
    }
}
```

### 20. State

**Location**: `State/`

State management patterns.

```csharp
public interface IStateful<TState>
{
    TState State { get; set; }
}

public interface IReadOnlyStateful<TState>
{
    TState State { get; }
}
```

### 21. Threading

**Location**: `Threading/`

Threading utilities.

```csharp
public interface IThreadSafe
{
    object SyncRoot { get; }
}
```

### 22. Wrappers

**Location**: `Wrappers/`

Wrapper patterns.

```csharp
public interface IWrapper<T>
{
    T Value { get; }
}

public class Wrapper<T> : IWrapper<T>
{
    public T Value { get; }

    public Wrapper(T value)
    {
        Value = value;
    }
}
```

### 23. Attributes

**Location**: `Attributes/`

Custom attributes for structural metadata.

```csharp
[AttributeUsage(AttributeTargets.Property)]
public class SetOnceAttribute : Attribute
{
    // Property can only be set once
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class BlockingAttribute : Attribute
{
    // Marks operation as potentially blocking
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class LazyLoadsAttribute : Attribute
{
    // Marks property as lazy-loaded
}

[AttributeUsage(AttributeTargets.All)]
public class IgnoreAttribute : Attribute
{
    // Ignore in serialization, binding, etc.
}

[AttributeUsage(AttributeTargets.Class)]
public class PublicOnlyAttribute : Attribute
{
    // Only public members should be processed
}
```

## Directory Structure

```
src/LionFire.Structures/
├── Acquisition/              # Lazy acquisition and caching
├── Attributes/               # Custom attributes
├── Collections/              # Collection types
│   ├── Adapters/             # Collection adapters
│   ├── Async/                # Async collections
│   ├── Concurrent/           # Thread-safe collections
│   ├── Hierarchical/         # Tree structures
│   └── Notification/         # Change notification
├── Composables/              # Composition patterns
├── Cqrs/                     # CQRS patterns
├── Data/                     # Data access abstractions
├── Dependencies/             # Dependency patterns
├── Disposable/               # Disposable utilities
├── Events/                   # Event patterns
├── Exceptions/               # Exception types
├── Execution/                # Execution patterns
├── FlexObjects/              # Flexible objects
├── Formatting/               # Formatting abstractions
├── Instantiating/            # Instantiation patterns
├── Keys/                     # Key selectors
│   ├── Id/                   # Identifier types
│   └── KeySelectors/         # Key selection
├── Licensing/                # License metadata
├── Messaging/                # Messaging patterns
├── Objects/                  # Object utilities
├── Ontology/                 # Classification
├── Options/                  # Options pattern
├── Progress/                 # Progress reporting
├── Referencing/              # Reference patterns
├── Results/                  # Result types
├── Serialization/            # Serialization metadata
├── Singletons/               # Singleton patterns
├── State/                    # State management
├── Structures/               # Structure types
├── Threading/                # Threading utilities
├── Types/                    # Type utilities
└── Wrappers/                 # Wrapper patterns
```

## Design Philosophy

**Lightweight Foundation:**
- No external dependencies (only LionFire.Base and LionFire.Flex)
- Reusable structures across all LionFire projects
- Focus on abstractions and patterns

**Composition over Inheritance:**
- Many composable interfaces
- Adapter patterns for type transformations
- Flexible object patterns for runtime extension

**Generic and Reusable:**
- Highly generic interface definitions
- Applicable to many domains
- Building blocks for higher-level libraries

## Common Usage Patterns

### Pattern 1: Collection Transformation

```csharp
public class EntityToDtoAdapter<TEntity, TDto> : CollectionAdapterBase<TEntity, TDto>
{
    private readonly IMapper mapper;

    public EntityToDtoAdapter(IMapper mapper)
    {
        this.mapper = mapper;
    }

    protected override TDto Transform(TEntity entity)
    {
        return mapper.Map<TDto>(entity);
    }
}

// Usage
var adapter = new EntityToDtoAdapter<User, UserDto>(mapper);
var dtos = adapter.AdaptCollection(users);
```

### Pattern 2: Hierarchical Navigation

```csharp
public class Folder : IHierarchical<Folder>
{
    public string Name { get; set; }
    public Folder? Parent { get; set; }
    public List<Folder> Children { get; } = new();

    IEnumerable<Folder> IHierarchical<Folder>.Children => Children;

    public string FullPath =>
        string.Join("/", this.Ancestors().Reverse().Append(this).Select(f => f.Name));
}
```

### Pattern 3: Cached Acquisition

```csharp
public class ServiceAcquisitor : Acquisitor<Type, object>
{
    public ServiceAcquisitor(IServiceProvider services)
        : base(type => services.GetRequiredService(type))
    {
    }
}

var acquisitor = new ServiceAcquisitor(serviceProvider);
var logger = (ILogger)acquisitor.Acquire(typeof(ILogger));
```

### Pattern 4: Result Pattern

```csharp
public Result<Customer> CreateCustomer(string name, string email)
{
    if (string.IsNullOrEmpty(name))
        return Result<Customer>.Failure("Name is required");

    if (!IsValidEmail(email))
        return Result<Customer>.Failure("Invalid email");

    var customer = new Customer { Name = name, Email = email };
    database.Add(customer);

    return Result<Customer>.Success(customer);
}

// Usage
var result = CreateCustomer("John", "john@example.com");

if (result.IsSuccess)
{
    Console.WriteLine($"Created customer: {result.Value.Id}");
}
else
{
    Console.WriteLine($"Failed: {result.ErrorMessage}");
}
```

## Testing Considerations

### Testing Adapters

```csharp
[Fact]
public void CollectionAdapter_Transforms_All_Items()
{
    var adapter = new IntToStringAdapter();
    var source = new[] { 1, 2, 3 };

    var result = adapter.AdaptCollection(source);

    Assert.Equal(new[] { "1", "2", "3" }, result);
}
```

### Testing Hierarchies

```csharp
[Fact]
public void Hierarchical_Returns_All_Descendants()
{
    var root = new Folder { Name = "root" };
    var child1 = new Folder { Name = "child1", Parent = root };
    var child2 = new Folder { Name = "child2", Parent = root };
    root.Children.AddRange(new[] { child1, child2 });

    var descendants = root.Descendants().ToList();

    Assert.Equal(2, descendants.Count);
    Assert.Contains(child1, descendants);
    Assert.Contains(child2, descendants);
}
```

## Related Projects

- **LionFire.Base** - BCL augmentation ([CLAUDE.md](../LionFire.Base/CLAUDE.md))
- **LionFire.Flex** - FlexData pattern
- **LionFire.Core** - Core metadata and DI ([CLAUDE.md](../LionFire.Core/CLAUDE.md))
- **LionFire.Data.Abstractions** - Data access abstractions

## Summary

**LionFire.Structures** provides fundamental data structures and patterns:

- **Collection Adapters**: Transform collections between types
- **Hierarchical Collections**: Tree navigation and traversal
- **Acquisition Pattern**: Lazy acquisition with caching
- **FlexObjects**: Runtime-extensible objects
- **Result Types**: Operation outcome representation
- **CQRS Patterns**: Command and query abstractions
- **Disposable Utilities**: Enhanced disposal management
- **Key Selectors**: Generic key extraction
- **Composition Patterns**: Composable behaviors
- **Attribute Metadata**: Structural annotations

**Key Strengths:**
- No external dependencies (except LionFire.Base/Flex)
- Highly reusable abstractions
- Generic and type-safe
- Composition-friendly
- Lightweight foundation

**Use When:**
- Need collection transformation
- Working with hierarchical data
- Require caching/acquisition patterns
- Want result types for error handling
- Need flexible composition patterns

**Typical Use Cases:**
- DTO/Entity transformation
- File system navigation
- Lazy-loaded data caching
- Operation result handling
- Component composition
- Runtime object extension
