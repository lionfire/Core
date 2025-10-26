# LionFire.Core

## Overview

**LionFire.Core** is the essential common library providing core interfaces, metadata, DI utilities, and cross-cutting concerns for the LionFire ecosystem. It serves as the foundation for most other LionFire libraries, providing shared abstractions and base implementations.

**Layer**: Core Toolkit (Foundational)
**Target**: .NET 9.0
**Root Namespace**: `LionFire`
**Description**: "Core interfaces, data structures and attributes required by the LionFire.Core suite of mini-frameworks. This package is typically not added on its own, but is a dependency required by many LionFire packages."

## Key Dependencies

### NuGet Packages
- **Microsoft.Extensions.DependencyInjection.Abstractions** - DI abstractions
- **Microsoft.Extensions.Logging.Abstractions** - Logging abstractions
- **Microsoft.Extensions.Hosting** - Hosting abstractions
- **Microsoft.Extensions.Resilience** - Resilience policies
- **System.Collections.Immutable** - Immutable collections
- **System.Reactive** - Reactive extensions (Rx)
- **GitVersion.MsBuild** - Semantic versioning

### LionFire Dependencies
- **LionFire.Base** - BCL augmentation with no external dependencies
- **LionFire.Environment** - Environment detection and configuration
- **LionFire.MultiTyping.Abstractions** - Multi-typing pattern abstractions
- **LionFire.Referencing.Abstractions** - Reference system abstractions
- **LionFire.Structures** - Collection types and data structures
- **LionFire.Validation** - Validation abstractions

## Core Components

### 1. MultiTyping System

**Location**: `MultiTyping/`

The MultiTyping system allows objects to present multiple type interfaces dynamically, enabling flexible composition patterns.

**Key Interfaces:**

```csharp
// Marker interface for multi-typable objects
public interface IMultiTypable { }

// Get sub-objects of specific types
public interface IHas<T> { T Object { get; } }

// Generic multi-type support
public interface IMultiTyped
{
    T AsType<T>() where T : class;
    object? AsType(Type type);
}
```

**Usage:**
```csharp
public class MyObject : IMultiTypable, IHas<ILogger>, IHas<IConfig>
{
    public ILogger Object => logger;
    IConfig IHas<IConfig>.Object => config;

    private ILogger logger;
    private IConfig config;
}

// Retrieve typed sub-objects
var logger = myObject.AsType<ILogger>();
var config = myObject.AsType<IConfig>();
```

**Components Pattern:**

```csharp
public interface IHasComponents
{
    IEnumerable<object> Components { get; }
}

// Objects can expose multiple components
public class CompositeObject : IHasComponents
{
    public IEnumerable<object> Components => new object[]
    {
        logger,
        cache,
        database
    };
}
```

### 2. Dependency Injection Extensions

**Location**: `DependencyInjection/`

Enhanced DI utilities beyond Microsoft.Extensions.DependencyInjection.

**Key Types:**

```csharp
// Service registration extensions
public static class ServiceCollectionExtensions
{
    // Try add enumerable singleton (doesn't throw if exists)
    public static IServiceCollection TryAddEnumerableSingleton<TService, TImplementation>(
        this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService;

    // Conditional registration
    public static IServiceCollection AddIfNotRegistered<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> factory);
}
```

**Dependency Attributes:**

```csharp
// Mark dependency injection properties
[SetOnce]
public class MyService
{
    [Dependency] // Injected via DI
    public ILogger Logger { get; set; }

    [SetOnce] // Can only be set once
    public string ConnectionString { get; set; }
}
```

### 3. Dependencies System

**Location**: `Dependencies/`

Manual dependency resolution and service location patterns.

```csharp
public static class DependencyContext
{
    // Get service from current context
    public static T Current<T>() where T : class;

    // Set current context
    public static void SetCurrent<T>(T value) where T : class;
}

// Dependency injection interface
public interface IHasDependencyContext
{
    IServiceProvider ServiceProvider { get; }
}
```

### 4. Logging Utilities

**Location**: `Logging/`

Enhanced logging beyond Microsoft.Extensions.Logging.

**Key Features:**

```csharp
// Static logger access
public static class L
{
    public static ILogger Get(string category);
    public static ILogger<T> Get<T>();
}

// Logging extensions
public static class LoggerExtensions
{
    public static void LogTrace(this ILogger logger, Func<string> messageFactory);
    public static void LogIfEnabled(this ILogger logger, LogLevel level, string message);
}
```

### 5. Execution Abstractions

**Location**: `Execution/`

Interfaces for executable objects and lifecycle management.

```csharp
public interface IStartable
{
    Task StartAsync(CancellationToken cancellationToken = default);
}

public interface IStoppable
{
    Task StopAsync(CancellationToken cancellationToken = default);
}

public interface IInitializable
{
    Task InitializeAsync();
}

// Combined lifecycle
public interface IStartableStoppable : IStartable, IStoppable { }
```

**Usage:**
```csharp
public class MyService : IStartableStoppable
{
    public async Task StartAsync(CancellationToken ct)
    {
        // Initialize resources
        await ConnectToDatabase(ct);
    }

    public async Task StopAsync(CancellationToken ct)
    {
        // Cleanup resources
        await DisconnectFromDatabase(ct);
    }
}
```

### 6. Configuration System

**Location**: `Configuration/`

Configuration abstractions and utilities.

```csharp
public interface IHasConfiguration
{
    IConfiguration Configuration { get; }
}

public interface IConfigurationProvider<T>
{
    T GetConfiguration();
}
```

### 7. Events and Messaging

**Location**: `Events/`, `Messaging/`

Event handling and messaging patterns.

```csharp
// Event subscription
public interface IEventSubscription<TEvent>
{
    void OnEvent(TEvent @event);
}

// Event publisher
public interface IEventPublisher
{
    void Publish<TEvent>(TEvent @event);
}

// Message bus
public interface IMessageBus
{
    Task SendAsync<TMessage>(TMessage message);
    IObservable<TMessage> Subscribe<TMessage>();
}
```

### 8. Validation Framework

**Location**: `Validation/`

Validation abstractions integrated with LionFire.Validation.

```csharp
public interface IValidatable
{
    ValidationResult Validate();
}

public interface IValidator<T>
{
    ValidationResult Validate(T obj);
}
```

### 9. Type System Extensions

**Location**: `Types/`

Advanced type scanning, registration, and management.

**Type Registry:**

```csharp
public class TypeRegistry
{
    // Register type by name
    public void Register(string name, Type type);

    // Resolve type by name
    public Type? Resolve(string name);

    // Get all registered types
    public IEnumerable<Type> GetAllTypes();
}
```

**Type Scanning:**

```csharp
public interface ITypeScanner
{
    // Scan assemblies for types matching criteria
    IEnumerable<Type> ScanFor<TInterface>();
    IEnumerable<Type> ScanFor(Type baseType);
}
```

**Type Name Registry:**

```csharp
public class TypeNameMultiRegistry
{
    // Multiple names can map to same type
    public void RegisterAlias(string alias, Type type);
    public Type? ResolveByAlias(string alias);
}
```

### 10. Reflection Utilities

**Location**: `Reflection/`

Enhanced reflection capabilities.

```csharp
public static class ReflectionExtensions
{
    // Get attribute from type
    public static T? GetCustomAttribute<T>(this Type type, bool inherit = true)
        where T : Attribute;

    // Check if type implements interface
    public static bool Implements<TInterface>(this Type type);
    public static bool Implements(this Type type, Type interfaceType);

    // Get all types in assembly implementing interface
    public static IEnumerable<Type> GetTypesImplementing<TInterface>(this Assembly assembly);
}
```

### 11. Reactive Extensions

**Location**: `Reactive/`

Integration with System.Reactive.

```csharp
public static class ObservableExtensions
{
    // Subscribe with automatic disposal
    public static IDisposable SubscribeDisposable<T>(
        this IObservable<T> observable,
        Action<T> onNext);

    // Throttle with latest value
    public static IObservable<T> ThrottleLatest<T>(
        this IObservable<T> source,
        TimeSpan interval);
}
```

### 12. Copying and Cloning

**Location**: `Copying/`

Object copying abstractions.

```csharp
public interface ICopyable<T>
{
    T Copy();
}

public interface IDeepCopyable<T>
{
    T DeepCopy();
}

public interface ICopier<TSource, TTarget>
{
    TTarget Copy(TSource source);
}
```

### 13. Default Values

**Location**: `DefaultValues/`

Default value providers.

```csharp
public interface IDefaultable
{
    void SetToDefaults();
}

public interface IDefaultProvider<T>
{
    T GetDefault();
}
```

### 14. Serialization Metadata

**Location**: `Serialization/`

Serialization context and metadata (not actual serializers).

```csharp
[Flags]
public enum LionSerializeContext
{
    None = 0,
    Persistence = 1 << 0,    // Saving to disk/database
    Network = 1 << 1,        // Network transmission
    Copy = 1 << 2,           // Object cloning
    AllSerialization = Persistence | Network | Copy,
    All = AllSerialization
}

[Flags]
public enum SerializationFlags
{
    None = 0,
    Text = 1 << 0,           // Text-based
    Binary = 1 << 1,         // Binary format
    HumanReadable = 1 << 2,  // Human-readable
    Serialize = 1 << 3,      // Supports serialization
    Deserialize = 1 << 4,    // Supports deserialization
}
```

### 15. State Machines

**Location**: `StateMachines/`

State machine abstractions.

```csharp
public interface IStateMachine<TState>
{
    TState CurrentState { get; }
    void TransitionTo(TState newState);
}

public interface IState<TState>
{
    TState State { get; set; }
}
```

### 16. Transactions

**Location**: `Transactions/`

Transaction abstractions.

```csharp
public interface ITransaction : IDisposable
{
    void Commit();
    void Rollback();
}

public interface ITransactionScope
{
    ITransaction BeginTransaction();
}
```

### 17. Resilience Patterns

**Location**: `Resilience/`

Integration with Microsoft.Extensions.Resilience for retry/circuit breaker patterns.

```csharp
public static class ResilienceExtensions
{
    // Apply resilience policy
    public static async Task<T> WithResilience<T>(
        this Task<T> task,
        ResiliencePolicy policy);
}
```

### 18. Hosting Extensions

**Location**: `Hosting/`

Extensions to Microsoft.Extensions.Hosting.

```csharp
public static class HostBuilderExtensions
{
    // Configure services with LionFire conventions
    public static IHostBuilder ConfigureLionFire(
        this IHostBuilder builder,
        Action<IServiceCollection> configure);
}
```

### 19. IO Utilities

**Location**: `IO/`

I/O abstractions and utilities.

```csharp
[Flags]
public enum IODirection
{
    None = 0,
    Read = 1 << 0,
    Write = 1 << 1,
    ReadWrite = Read | Write
}

public interface IReadable<T>
{
    T Read();
}

public interface IWriteable<T>
{
    void Write(T value);
}
```

### 20. Net/Messaging

**Location**: `Net/Messages/`

Network message abstractions.

```csharp
public interface IMessage
{
    string MessageId { get; }
    DateTimeOffset Timestamp { get; }
}

public interface IRequest<TResponse> : IMessage
{
}

public interface IResponse : IMessage
{
    string RequestId { get; }
}
```

### 21. Attributes

**Location**: `Attributes/`

Custom attributes for metadata.

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class IgnoreAttribute : Attribute
{
    // Ignore in serialization, binding, etc.
}

[AttributeUsage(AttributeTargets.Property)]
public class SetOnceAttribute : Attribute
{
    // Property can only be set once
}

[AttributeUsage(AttributeTargets.Class)]
public class SingletonAttribute : Attribute
{
    // Marks class as singleton
}
```

### 22. Framing

**Location**: `Framing/`

Frame-based execution patterns.

```csharp
public interface IFrameProvider
{
    int CurrentFrame { get; }
    IObservable<int> FrameUpdates { get; }
}
```

### 23. Exceptions

**Location**: `Exceptions/`

Custom exception types.

```csharp
public class LionFireException : Exception
{
    public LionFireException(string message) : base(message) { }
}

public class NotSupportedException<T> : NotSupportedException
{
    public NotSupportedException() : base($"Not supported for type {typeof(T).Name}") { }
}
```

## Directory Structure

```
src/LionFire.Core/
├── Attributes/               # Custom attributes
├── Base/                     # Base classes
├── Collections/              # Collection utilities
├── Configuration/            # Configuration abstractions
├── Copying/                  # Copying/cloning
├── DefaultValues/            # Default value providers
├── Dependencies/             # Dependency management
├── DependencyInjection/      # DI extensions
├── Events/                   # Event handling
├── Exceptions/               # Exception types
├── Execution/                # Execution abstractions (IStartable, etc.)
├── Framing/                  # Frame-based execution
├── Hosting/                  # Hosting extensions
├── IO/                       # I/O abstractions
├── LazyLoading/              # Lazy loading patterns
├── Licensing/                # License information
├── Logging/                  # Logging utilities
├── Messaging/                # Messaging patterns
├── Meta/                     # Metadata utilities
├── MultiTyping/              # Multi-typing system
│   ├── Components/           # Component pattern
│   └── Overlaying/           # Overlaying pattern
├── Net/                      # Network abstractions
│   └── Messages/             # Message types
├── Reactive/                 # Reactive extensions
├── Reflection/               # Reflection utilities
├── Resilience/               # Resilience patterns
├── Serialization/            # Serialization metadata
├── StateMachines/            # State machine abstractions
├── Structures/               # Structure types
│   └── Flags/                # Flag utilities
├── Transactions/             # Transaction abstractions
├── Types/                    # Type system extensions
│   ├── Scanning/             # Type scanning
│   ├── TypeNameMultiRegistry/ # Type name registry
│   └── TypeRegistry/         # Type registry
└── Validation/               # Validation integration
```

## Design Patterns

### Multi-Typing Pattern

Enables dynamic composition without multiple inheritance:

```csharp
public interface IMultiTypable { }

public class Document : IMultiTypable
{
    // Can be viewed as different types
}

// Extension method
public static T? AsType<T>(this IMultiTypable obj) where T : class
{
    if (obj is T typed) return typed;
    if (obj is IHas<T> has) return has.Object;
    return null;
}

// Usage
var document = new Document();
var logger = document.AsType<ILogger>(); // May return null or logger component
```

### Dependency Injection Pattern

Enhanced DI with attributes:

```csharp
public class MyService
{
    [Dependency]
    public ILogger<MyService> Logger { get; set; }

    [Dependency]
    public IConfiguration Configuration { get; set; }

    // Dependencies injected after construction
}
```

### Lifecycle Pattern

Standard lifecycle interfaces:

```csharp
public class ManagedService : IStartableStoppable, IInitializable
{
    public async Task InitializeAsync()
    {
        // One-time initialization
    }

    public async Task StartAsync(CancellationToken ct)
    {
        // Start running
    }

    public async Task StopAsync(CancellationToken ct)
    {
        // Stop gracefully
    }
}
```

## Common Usage Patterns

### Pattern 1: Service Registration

```csharp
public static class MyServiceExtensions
{
    public static IServiceCollection AddMyService(this IServiceCollection services)
    {
        return services
            .AddSingleton<IMyService, MyService>()
            .TryAddEnumerableSingleton<IPlugin, MyPlugin>()
            .Configure<MyOptions>(options => options.SetDefaults());
    }
}
```

### Pattern 2: Type Registry

```csharp
var registry = new TypeRegistry();

// Register types
registry.Register("logger", typeof(ConsoleLogger));
registry.Register("logger", typeof(FileLogger)); // Multiple implementations

// Resolve
var loggerType = registry.Resolve("logger");
var logger = Activator.CreateInstance(loggerType);
```

### Pattern 3: Event Publishing

```csharp
public class EventBus : IEventPublisher
{
    private readonly List<object> subscribers = new();

    public void Publish<TEvent>(TEvent @event)
    {
        foreach (var subscriber in subscribers.OfType<IEventSubscription<TEvent>>())
        {
            subscriber.OnEvent(@event);
        }
    }

    public void Subscribe<TEvent>(IEventSubscription<TEvent> subscriber)
    {
        subscribers.Add(subscriber);
    }
}
```

### Pattern 4: Validation

```csharp
public class User : IValidatable
{
    public string Name { get; set; }
    public string Email { get; set; }

    public ValidationResult Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(Name))
            errors.Add("Name is required");

        if (!Email.Contains("@"))
            errors.Add("Invalid email");

        return new ValidationResult(errors);
    }
}
```

### Pattern 5: Resilience

```csharp
public async Task<Data> FetchDataWithRetry()
{
    return await httpClient.GetAsync<Data>(url)
        .WithResilience(new ResiliencePolicy
        {
            MaxRetries = 3,
            RetryDelay = TimeSpan.FromSeconds(1)
        });
}
```

## Integration with Other Libraries

### With LionFire.Base

LionFire.Core builds on LionFire.Base:
- Uses extension methods from Base
- Adds DI and abstractions on top of Base utilities

### With LionFire.Structures

Shares data structure abstractions:
- Both define collection interfaces
- Core focuses on metadata, Structures on implementations

### With LionFire.Persistence

Provides abstractions used by persistence:
- Serialization contexts
- Lifecycle interfaces
- DI patterns

## Testing Considerations

### Mocking Dependencies

```csharp
var mockLogger = new Mock<ILogger<MyService>>();
var service = new MyService
{
    Logger = mockLogger.Object
};

await service.StartAsync();

mockLogger.Verify(x => x.Log(
    LogLevel.Information,
    It.IsAny<EventId>(),
    It.IsAny<It.IsAnyType>(),
    It.IsAny<Exception>(),
    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
    Times.Once);
```

### Testing Lifecycle

```csharp
[Fact]
public async Task Service_Starts_And_Stops()
{
    var service = new ManagedService();

    await service.InitializeAsync();
    await service.StartAsync(CancellationToken.None);

    Assert.True(service.IsRunning);

    await service.StopAsync(CancellationToken.None);

    Assert.False(service.IsRunning);
}
```

## Related Projects

- **LionFire.Base** - BCL augmentation ([CLAUDE.md](../LionFire.Base/CLAUDE.md))
- **LionFire.Structures** - Data structures ([CLAUDE.md](../LionFire.Structures/CLAUDE.md))
- **LionFire.Hosting** - Hosting extensions
- **LionFire.Persistence** - Persistence framework ([CLAUDE.md](../LionFire.Persistence/CLAUDE.md))
- **LionFire.MultiTyping.Abstractions** - Multi-typing abstractions

## Documentation

- **Microsoft.Extensions.DependencyInjection**: https://learn.microsoft.com/dotnet/core/extensions/dependency-injection
- **Microsoft.Extensions.Hosting**: https://learn.microsoft.com/aspnet/core/fundamentals/host/generic-host
- **System.Reactive**: http://reactivex.io/

## Summary

**LionFire.Core** is the foundational library for LionFire applications:

- **MultiTyping**: Dynamic type composition without multiple inheritance
- **Dependency Injection**: Enhanced DI with attributes and utilities
- **Lifecycle Management**: IStartable, IStoppable, IInitializable interfaces
- **Type System**: Type registries, scanning, and name resolution
- **Validation**: Validation abstractions and integration
- **Events/Messaging**: Event publishing and messaging patterns
- **Reactive Extensions**: Integration with System.Reactive
- **Resilience**: Retry and circuit breaker patterns
- **Reflection**: Enhanced reflection utilities
- **Serialization Metadata**: Contexts and flags for serializers

**Key Strengths:**
- Comprehensive cross-cutting concerns
- Flexible composition patterns
- Strong DI integration
- Lifecycle management
- Type system extensions

**Use When:**
- Building LionFire applications
- Need DI enhancements
- Require lifecycle management
- Want multi-typing patterns
- Need type registry/scanning

**Typical Use Cases:**
- Foundation for LionFire applications
- Dependency injection configuration
- Component lifecycle management
- Type discovery and registration
- Event-driven architectures
- Cross-cutting concerns

**Note**: This library is typically a transitive dependency - you rarely add it directly. It's required by most other LionFire packages.
