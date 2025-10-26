# Workspace Document Types

**Overview**: Deep dive into defining, registering, and managing document types within the workspace system. Covers entity design, persistence patterns, serialization strategies, and the runner pattern for active documents.

---

## Table of Contents

1. [What Are Document Types?](#what-are-document-types)
2. [Defining Document Entities](#defining-document-entities)
3. [Registration Pattern](#registration-pattern)
4. [Persistence Mechanics](#persistence-mechanics)
5. [ViewModel Integration](#viewmodel-integration)
6. [Runner Pattern for Active Documents](#runner-pattern-for-active-documents)
7. [Advanced Patterns](#advanced-patterns)

---

## What Are Document Types?

### Definition

A **document type** is an entity type that:
- Can be persisted as files in workspace subdirectories
- Has associated `IObservableReader/Writer` services registered per workspace
- May have a ViewModel for UI binding
- Optionally has a runner for lifecycle management

### Document vs Other Entities

| Document Type | Regular Entity | Singleton Service |
|---------------|----------------|-------------------|
| **Multiple instances per workspace** | Multiple instances globally | One instance globally |
| **File-backed** (HJSON) | Database or in-memory | Configuration or code |
| **User-created at runtime** | Created by code | Created at startup |
| **IObservableReader/Writer** | DbContext or repository | N/A |
| **Per-workspace scope** | Global scope | Application scope |

**Examples**:
- **Document Types**: BotEntity, Portfolio, Strategy, Project, Article
- **Regular Entities**: User, Order, Transaction (database-backed)
- **Singleton Services**: Configuration, Logger, AppState

---

## Defining Document Entities

### Minimal Entity

```csharp
using ReactiveUI;
using LionFire.Ontology;

[Alias("Bot")]  // Used for directory name and serialization
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;
    [Reactive] private string? _description;
}
```

**Requirements**:
- ✅ Implement `INotifyPropertyChanged` (via `ReactiveObject` recommended)
- ✅ Use `[Alias]` attribute for directory naming
- ✅ Public properties for serialization
- ✅ Default constructor for deserialization

### Best Practices

**1. Use ReactiveObject**:
```csharp
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;  // Generates Name property with INPC
}
```

**Benefits**:
- Automatic `INotifyPropertyChanged` implementation
- Source generator creates boilerplate
- Integrates with ReactiveUI patterns

**2. Add Validation**:
```csharp
using LionFire.Validation;

public partial class BotEntity : ReactiveObject, IValidatable
{
    public ValidationContext ValidateThis(ValidationContext context)
    {
        return context
            .PropertyNotNull(nameof(Name), Name)
            .PropertyInRange(nameof(StopLoss), StopLoss, 0.1, 10.0);
    }
}
```

**3. Use Appropriate Serialization Attributes**:
```csharp
using Newtonsoft.Json;

public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;

    // Computed property - don't serialize
    [JsonIgnore]
    public string DisplayName => $"{Exchange}:{Symbol} - {Name}";

    // Complex property - custom converter
    [JsonConverter(typeof(TimeFrameConverter))]
    [Reactive] private TimeFrame? _timeFrame;
}
```

**4. Support Deep Copying** (for optimization):
```csharp
public partial class BotEntity : ReactiveObject, ICloneable
{
    public object Clone()
    {
        return new BotEntity
        {
            Name = this.Name,
            Description = this.Description,
            // ... copy all properties
        };
    }
}
```

---

## Registration Pattern

### Step-by-Step Registration

**1. Declare Member Type**:
```csharp
services.AddWorkspaceChildType<BotEntity>();
```

**What it does**:
```csharp
// Internally (from AutomationHostingX pattern):
public static IServiceCollection AddWorkspaceChildType<T>(this IServiceCollection services)
    where T : notnull
{
    return services.TryAddEnumerableSingleton<IWorkspaceServiceConfigurator, WorkspaceChildTypeConfigurator<T>>(
        new WorkspaceChildTypeConfigurator<T>()
    );
}
```

**Result**: Adds type to `WorkspaceConfiguration.MemberTypes` list.

**2. Register Document Service**:
```csharp
services.AddWorkspaceDocumentService<string, BotEntity>();
```

**What it does**:
```csharp
services.AddHostedSingleton<DirectoryWorkspaceDocumentService<BotEntity>>();
```

**Result**: Registers hosted service that watches for document changes.

**3. Optional: Register Runner**:
```csharp
services.TryAddEnumerable(
    ServiceDescriptor.Singleton<IWorkspaceDocumentRunner<string, BotEntity>, BotRunner>()
);
```

**Result**: Runner will be invoked when documents change.

### Complete Registration Example

```csharp
public static IServiceCollection AddTradingAutomation(this IServiceCollection services, IConfiguration configuration)
{
    return services
        // 1. Declare workspace member types
        .AddWorkspaceChildType<BotEntity>()
        .AddWorkspaceChildType<Portfolio>()
        .AddWorkspaceChildType<Strategy>()

        // 2. Register document services
        .AddWorkspaceDocumentService<string, BotEntity>()
        .AddWorkspaceDocumentService<string, Portfolio>()
        .AddWorkspaceDocumentService<string, Strategy>()

        // 3. Register ViewModels (for UI)
        .AddTransient<BotVM>()
        .AddTransient<PortfolioVM>()
        .AddTransient<StrategyVM>()

        // 4. Optional: Register runners
        .TryAddEnumerableSingleton<IWorkspaceDocumentRunner<string, BotEntity>, BotRunner>();
}
```

---

## Persistence Mechanics

### File Path Derivation

**Given**:
- Workspace Directory: `C:\Users\Alice\Trading\Workspaces\workspace1`
- Entity Type: `BotEntity`
- Entity Key: `"my-bot"`
- Alias Attribute: `[Alias("Bot")]`

**Derived Path**:
```
1. Get plural name: "Bot" → "Bots"
2. Combine: workspace1 + Bots = C:\Users\Alice\Trading\Workspaces\workspace1\Bots
3. Add filename: Bots + my-bot.hjson = C:\Users\Alice\Trading\Workspaces\workspace1\Bots\my-bot.hjson
```

### Plural Name Derivation

```csharp
// From Alias attribute
[Alias("Bot")] → "Bots"
[Alias("Portfolio")] → "Portfolios"
[Alias("Strategy")] → "Strategies"

// From type name if no Alias
BotEntity → "BotEntities"
Portfolio → "Portfolios"
```

**Convention**: Uses simple pluralization (adds "s" or "ies").

### File Format (HJSON)

**Entity**:
```csharp
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;
    [Reactive] private string? _description;
    [Reactive] private bool _enabled;
    [Reactive] private double? _stopLoss;
}
```

**Serialized** (`Bots/my-bot.hjson`):
```hjson
// Bot Configuration
// Root braces omitted per HJSON convention

name: My Trading Bot
description: Scalping strategy for BTCUSDT
enabled: true
stopLoss: 1.5
```

**Notes**:
- Root braces **omitted** (HJSON convention)
- Comments preserved
- Human-readable and editable
- Version controllable with git

### Serialization Pipeline

```
Entity (BotEntity)
    ↓ JsonConvert.SerializeObject (Newtonsoft.Json)
JsonObject
    ↓ Format as HJSON
HJSON String
    ↓ File.WriteAllText
File (my-bot.hjson)
```

**Deserialization**:
```
File (my-bot.hjson)
    ↓ File.ReadAllText
HJSON String
    ↓ Hjson.HjsonValue.Load
JsonObject
    ↓ JsonConvert.DeserializeObject<BotEntity>
Entity (BotEntity)
```

---

## ViewModel Integration

### Why ViewModels?

**Entities** are for **data**, **ViewModels** are for **UI**:

```csharp
// Entity: Just data
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;
    [Reactive] private double? _stopLoss;
}

// ViewModel: UI-specific logic
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value) { }

    // Computed properties for display
    public string DisplayName => $"{Value.Exchange}:{Value.Symbol} - {Value.Name}";

    // UI state (not persisted)
    public bool IsRunning { get; set; }

    // Commands
    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; }
}
```

### ViewModel Pattern

**KeyValueVM Base**:
```csharp
public class BotVM : KeyValueVM<string, BotEntity>
{
    // Key: string (filename without extension)
    // Value: BotEntity (the actual entity)

    public BotVM(string key, BotEntity value) : base(key, value) { }
}
```

**Benefits**:
- Separates UI logic from data model
- Computed properties don't pollute entity
- Commands and UI state
- Testable without UI

### ObservableReaderWriterItemVM Integration

**For single-item views**:
```csharp
var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
var writer = WorkspaceServices.GetService<IObservableWriter<string, BotEntity>>();

var vm = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);
vm.Id = "my-bot";

// VM automatically:
// - Loads entity from reader
// - Creates BotVM wrapping entity
// - Subscribes to changes
// - Provides Write() method to persist
```

---

## Runner Pattern for Active Documents

### What Are Runners?

**Runners** are observers that react to document changes and manage lifecycle of **active** documents.

**Use Cases**:
- Starting/stopping bots when enabled/disabled
- Subscribing to market data when strategy is active
- Running background tasks for documents
- Coordinating between multiple documents

### Implementing a Runner

```csharp
using LionFire.Workspaces.Services;
using System;

public class BotRunner :
    IWorkspaceDocumentRunner<string, BotEntity>,
    IObserver<BotEntity>
{
    #region Dependencies

    private readonly ILogger<BotRunner> logger;
    private readonly BotExecutionEngine executionEngine;

    public BotRunner(ILogger<BotRunner> logger, BotExecutionEngine executionEngine)
    {
        this.logger = logger;
        this.executionEngine = executionEngine;
    }

    #endregion

    #region IWorkspaceDocumentRunner

    public Type RunnerType => typeof(BotRunner);

    #endregion

    #region IObserver<BotEntity>

    public void OnNext(BotEntity bot)
    {
        logger.LogInformation("Bot document changed: {Name}, Enabled: {Enabled}",
            bot.Name, bot.Enabled);

        // React to changes
        if (bot.Enabled)
        {
            if (!executionEngine.IsRunning(bot))
            {
                logger.LogInformation("Starting bot: {Name}", bot.Name);
                executionEngine.Start(bot);
            }
            else
            {
                logger.LogInformation("Bot already running: {Name}", bot.Name);
            }
        }
        else
        {
            if (executionEngine.IsRunning(bot))
            {
                logger.LogInformation("Stopping bot: {Name}", bot.Name);
                executionEngine.Stop(bot);
            }
        }
    }

    public void OnError(Exception error)
    {
        logger.LogError(error, "Error in BotRunner");
    }

    public void OnCompleted()
    {
        logger.LogInformation("BotRunner completed");
    }

    #endregion
}
```

### Runner Lifecycle

```
Application Startup
    ↓
DirectoryWorkspaceDocumentService<BotEntity> starts (IHostedService)
    ↓
Subscribes to IObservableReader<string, BotEntity>.Values.Connect()
    ↓
For each IWorkspaceDocumentRunner<string, BotEntity>:
    Creates ConcurrentDictionary<string, BotRunner>
    ↓
    When document changes:
        Get or create BotRunner instance for this key
        Call runner.OnNext(botEntity)
```

**Per-Document Instances**:
```
workspace1\Bots\
├── bot-alpha.hjson  → BotRunner instance 1
├── bot-beta.hjson   → BotRunner instance 2
└── bot-gamma.hjson  → BotRunner instance 3
```

Each file gets its own runner instance via `ActivatorUtilities.CreateInstance`.

### Registration

```csharp
// Register runner service (in application startup)
services.TryAddEnumerable(
    ServiceDescriptor.Singleton<IWorkspaceDocumentRunner<string, BotEntity>, BotRunner>()
);
```

**Note**: Use `TryAddEnumerable` to support multiple runners for the same type.

### Multiple Runners for Same Type

```csharp
// Runner 1: Execution
services.TryAddEnumerable(ServiceDescriptor.Singleton<
    IWorkspaceDocumentRunner<string, BotEntity>, BotExecutionRunner>());

// Runner 2: Monitoring
services.TryAddEnumerable(ServiceDescriptor.Singleton<
    IWorkspaceDocumentRunner<string, BotEntity>, BotMonitoringRunner>());

// Runner 3: Analytics
services.TryAddEnumerable(ServiceDescriptor.Singleton<
    IWorkspaceDocumentRunner<string, BotEntity>, BotAnalyticsRunner>());

// All three invoked when bot documents change
```

---

## Advanced Patterns

### Pattern 1: Nested Entities

**Entity with complex properties**:
```csharp
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;

    // Nested entity
    public BotParameters? Parameters { get; set; }
}

public class BotParameters
{
    public double StopLoss { get; set; }
    public double TakeProfit { get; set; }
    public int MaxTrades { get; set; }
}
```

**Serialized**:
```hjson
name: My Bot
parameters: {
  stopLoss: 1.5
  takeProfit: 3.0
  maxTrades: 5
}
```

### Pattern 2: Collections in Documents

```csharp
public partial class PortfolioEntity : ReactiveObject
{
    [Reactive] private string? _name;

    // Collection of references
    public List<string>? BotIds { get; set; }

    // DynamicData cache (not serialized)
    [JsonIgnore]
    public SourceCache<BotReference, string> BotReferences { get; } = new(r => r.Id);
}
```

**Notes**:
- Simple collections (List, Dictionary) serialize naturally
- DynamicData collections need `[JsonIgnore]`
- Can reconstruct DynamicData from List on load

### Pattern 3: Type Polymorphism

```csharp
// Base type
[Alias("Strategy")]
public abstract partial class StrategyEntity : ReactiveObject
{
    [Reactive] private string? _name;
    [Reactive] private string? _strategyType;
}

// Derived types
public partial class MeanReversionStrategy : StrategyEntity
{
    [Reactive] private double? _threshold;
}

public partial class MomentumStrategy : StrategyEntity
{
    [Reactive] private int? _period;
}
```

**Serialization**:
```hjson
// File: Strategies/strategy1.hjson
strategyType: MeanReversion  // Type discriminator
name: Mean Reversion Strategy
threshold: 2.5
```

**Deserialization**:
```csharp
// Custom deserializer based on strategyType property
// Or use JsonTypeInfo for polymorphism
```

### Pattern 4: Document References

```csharp
public partial class PortfolioEntity : ReactiveObject
{
    // Reference to another document by ID
    public string? PrimaryBotId { get; set; }

    // Resolved reference (not persisted)
    [JsonIgnore]
    public BotEntity? PrimaryBot { get; set; }
}

// In runner or VM:
public async Task ResolveReferences(IObservableReader<string, BotEntity> botReader)
{
    if (portfolio.PrimaryBotId != null)
    {
        var result = await botReader.TryGetValue(portfolio.PrimaryBotId);
        if (result.HasValue)
        {
            portfolio.PrimaryBot = result.Value;
        }
    }
}
```

### Pattern 5: Document Versioning

```csharp
public partial class BotEntity : ReactiveObject
{
    // Version for schema evolution
    public int SchemaVersion { get; set; } = 2;

    [Reactive] private string? _name;

    // Migration on load
    public void Migrate()
    {
        if (SchemaVersion < 2)
        {
            // Migrate v1 → v2
            MigrateV1ToV2();
            SchemaVersion = 2;
        }
    }
}
```

---

## Directory Naming Conventions

### Default Convention

**Type Name** → **Directory Name**:
```
BotEntity      → BotEntities
Bot            → Bots
Portfolio      → Portfolios
Strategy       → Strategies
UserConfig     → UserConfigs
```

### Using [Alias] Attribute

```csharp
[Alias("Bot")]
public partial class BotEntity : ReactiveObject { }

// Directory: Bots (not BotEntities)
```

**Recommended**: Always use `[Alias]` for clean directory names.

### Custom Pluralization

If needed, customize in registration:

```csharp
services.RegisterObservablesInSubDirForType<BotEntity>(
    serviceProvider,
    parentDir,
    entitySubdir: "TradingBots"  // Custom directory name
);
```

---

## Serialization Strategies

### HJSON (Default)

**Pros**:
- Human-readable
- Supports comments
- Flexible syntax (trailing commas, unquoted keys)
- Version-control friendly

**Cons**:
- Slightly larger files
- Not as fast as binary formats

**Usage**: Default via `HjsonFsDirectoryReaderRx/WriterRx`

### JSON

If you prefer strict JSON:

```csharp
// Use JsonFsDirectoryReaderRx instead
services.RegisterObservablesInDir<BotEntity>(serviceProvider, dirSelector, useJson: true);
```

### Custom Serialization

Implement `ISerializationProvider`:

```csharp
public class YamlSerializationProvider : ISerializationProvider
{
    public string Extension => ".yaml";

    public T Deserialize<T>(string content) { /* YAML deserialization */ }
    public string Serialize<T>(T obj) { /* YAML serialization */ }
}
```

---

## Best Practices

### 1. Entity Design

✅ **DO**:
- Use `ReactiveObject` for automatic INPC
- Add `[Alias]` attribute for clean directory names
- Use `[JsonIgnore]` for computed/transient properties
- Implement `IValidatable` for validation
- Keep entities focused on data, not UI logic

❌ **DON'T**:
- Put UI-specific logic in entities
- Use complex property types without serialization support
- Forget default constructor
- Use `[Reactive]` on collections (use property with backing field instead)

### 2. File Organization

✅ **DO**:
- Use kebab-case for file names: `my-bot.hjson`
- Keep subdirectory names plural: `Bots`, `Portfolios`
- Use consistent naming across workspace

❌ **DON'T**:
- Use spaces in file names
- Create nested subdirectories manually (flat structure within type directory)
- Mix casing styles

### 3. Registration Order

✅ **DO**:
```csharp
services
    .AddWorkspaceChildType<BotEntity>()                    // 1. Declare type
    .AddWorkspaceDocumentService<string, BotEntity>()      // 2. Register service
    .AddTransient<BotVM>()                                 // 3. Register VM
    .TryAddEnumerable(...BotRunner...)                     // 4. Register runner
```

### 4. Runner Implementation

✅ **DO**:
- Keep runners focused on lifecycle management
- Use dependency injection for services
- Log state transitions
- Handle OnError and OnCompleted

❌ **DON'T**:
- Put business logic in runners (belongs in entity/services)
- Create long-running synchronous operations in OnNext
- Ignore exceptions

---

## Summary

### Document Type Checklist

Creating a new document type:

- [ ] Define entity class (inherit `ReactiveObject`)
- [ ] Add `[Alias("TypeName")]` attribute
- [ ] Use `[Reactive]` for properties
- [ ] Add `[JsonIgnore]` for computed properties
- [ ] Implement `IValidatable` if needed
- [ ] Define ViewModel (inherit `KeyValueVM<string, Entity>`)
- [ ] Register with `AddWorkspaceChildType<T>()`
- [ ] Register document service `AddWorkspaceDocumentService<string, T>()`
- [ ] Register VM `AddTransient<TVM>()`
- [ ] Optional: Implement and register runner
- [ ] Create Blazor pages (list and detail)
- [ ] Test with workspace

### Key Patterns

1. **Entity Pattern**: `ReactiveObject` with `[Reactive]` properties
2. **Alias Pattern**: `[Alias("Name")]` for directory naming
3. **Registration Pattern**: `AddWorkspaceChildType` → `AddWorkspaceDocumentService`
4. **ViewModel Pattern**: `KeyValueVM<TKey, TEntity>`
5. **Runner Pattern**: `IWorkspaceDocumentRunner` + `IObserver<TEntity>`
6. **Persistence Pattern**: HJSON files in plural-named subdirectories

---

## Related Documentation

- **[Workspace Architecture](README.md)** - Overall architecture
- **[Service Scoping](service-scoping.md)** - How services are scoped
- **[Blazor MVVM Patterns](../../ui/blazor-mvvm-patterns.md)** - UI patterns
- **[How-To: Create Workspace Document Type](../../guides/how-to/create-workspace-document-type.md)** - Step-by-step guide
- **Library References**:
  - [LionFire.Workspaces](../../../src/LionFire.Workspaces/CLAUDE.md)
  - [LionFire.Reactive](../../../src/LionFire.Reactive/CLAUDE.md)
