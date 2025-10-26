# Runner Pattern for Lifecycle

## Overview

The **Runner Pattern** provides reactive lifecycle management for components that start/stop based on configuration changes. This pattern is commonly used for workspace documents that represent active, stateful components (bots, services, connections, etc.).

**Key Concept**: Runners observe configuration observables and automatically start/stop based on an "enabled" predicate.

---

## Table of Contents

1. [The Runner Pattern](#the-runner-pattern)
2. [Runner Interface](#runner-interface)
3. [Implementing Runners](#implementing-runners)
4. [Hot-Reload Support](#hot-reload-support)
5. [Fault Tracking](#fault-tracking)
6. [Integration with Workspaces](#integration-with-workspaces)
7. [Common Patterns](#common-patterns)

---

## The Runner Pattern

### The Problem

**Scenario**: You have configuration files for bots. When a bot's config says `enabled: true`, the bot should start. When it changes to `enabled: false`, the bot should stop.

**Naive Approach**:
```csharp
// ❌ Manual lifecycle management
configReader.Values.Connect()
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            var bot = change.Current.Value;

            if (change.Reason == ChangeReason.Add && bot.Enabled)
            {
                StartBot(bot);
            }
            else if (change.Reason == ChangeReason.Update)
            {
                if (bot.Enabled && !oldBot.Enabled)
                    StartBot(bot);
                else if (!bot.Enabled && oldBot.Enabled)
                    StopBot(bot);
                // ... handle hot-reload ...
            }
            else if (change.Reason == ChangeReason.Remove)
            {
                StopBot(bot);
            }
        }
    });
```

**Problems**:
- Lots of boilerplate
- Error-prone state management
- No fault tracking
- Hard to test

---

### The Solution - Runner Pattern

**Declarative approach**:
```csharp
public class BotRunner : Runner<BotEntity, BotRunner>
{
    // Define "enabled" predicate
    static bool IRunner<BotEntity>.IsEnabled(BotEntity config)
        => config.Enabled;

    // Start logic
    protected override async ValueTask<bool> Start(
        BotEntity config,
        Optional<BotEntity> oldConfig)
    {
        Console.WriteLine($"Starting bot: {config.Name}");
        await InitializeBot(config);
        return true;
    }

    // Stop logic
    protected override void Stop(
        Optional<BotEntity> newConfig,
        Optional<BotEntity> oldConfig)
    {
        Console.WriteLine($"Stopping bot");
        CleanupBot();
    }

    // Hot-reload logic
    protected override void OnConfigurationChange(
        BotEntity newConfig,
        Optional<BotEntity> oldConfig)
    {
        Console.WriteLine($"Config changed: {newConfig.Name}");
        UpdateBotParameters(newConfig);
    }
}
```

**Usage**:
```csharp
var runner = new BotRunner();

// Subscribe to config changes
configObservable.Subscribe(runner);  // Runner implements IObserver<T>

// Runner automatically:
// - Starts when config.Enabled == true
// - Stops when config.Enabled == false
// - Hot-reloads when config changes while running
// - Tracks faults and errors
```

---

## Runner Interface

### IRunner\<TValue\>

```csharp
public interface IRunner<in TValue>
{
    // Is this configuration "enabled"?
    static abstract bool IsEnabled(TValue config);

    // Start the runner
    ValueTask<bool> Start(TValue config);

    // Stop the runner
    void Stop(Optional<TValue> newConfig, Optional<TValue> oldConfig);

    // Handle config changes while running
    void OnConfigurationChange(TValue newConfig, Optional<TValue> oldConfig);
}
```

---

### Runner\<TValue, TRunner\>

**Base Class** - Implements `IRunner<TValue>` and `IObserver<TValue>`:

```csharp
public abstract class Runner<TValue, TRunner> : IRunner<TValue>, IObserver<TValue>
    where TRunner : Runner<TValue, TRunner>
{
    // IObserver implementation
    public void OnNext(TValue value);  // Handles lifecycle transitions
    public void OnError(Exception error);
    public void OnCompleted();

    // IRunner implementation (abstract - implement in derived class)
    protected abstract ValueTask<bool> Start(TValue config, Optional<TValue> oldConfig);
    protected abstract void Stop(Optional<TValue> newConfig, Optional<TValue> oldConfig);
    protected virtual void OnConfigurationChange(TValue newConfig, Optional<TValue> oldConfig) { }

    // State tracking
    public WorkerStatus Status { get; }
    public IObservable<Exception> Errors { get; }
}
```

---

## Implementing Runners

### Basic Runner

```csharp
public class BotRunner : Runner<BotEntity, BotRunner>
{
    // Required: Define enabled predicate
    static bool IRunner<BotEntity>.IsEnabled(BotEntity config)
        => config.Enabled;

    // Required: Start logic
    protected override async ValueTask<bool> Start(
        BotEntity config,
        Optional<BotEntity> oldConfig)
    {
        try
        {
            await ConnectToExchange(config.Exchange);
            await InitializeStrategy(config);
            return true;  // Success
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to start bot");
            return false;  // Failure (runner tracks fault)
        }
    }

    // Required: Stop logic
    protected override void Stop(
        Optional<BotEntity> newConfig,
        Optional<BotEntity> oldConfig)
    {
        DisconnectFromExchange();
        CleanupResources();
    }

    // Optional: Hot-reload logic
    protected override void OnConfigurationChange(
        BotEntity newConfig,
        Optional<BotEntity> oldConfig)
    {
        // Update parameters without restart
        if (oldConfig.HasValue && newConfig.Symbol != oldConfig.Value.Symbol)
        {
            // Symbol changed - need restart
            Stop(Optional.Some(newConfig), oldConfig);
            Start(newConfig, oldConfig);
        }
        else
        {
            // Just parameter changes - hot-reload
            UpdateParameters(newConfig);
        }
    }
}
```

---

### Runner with Dependencies

```csharp
public class BotRunner : Runner<BotEntity, BotRunner>
{
    // Inject dependencies
    private readonly IExchangeConnectionFactory exchangeFactory;
    private readonly ILogger<BotRunner> logger;

    public BotRunner(
        IExchangeConnectionFactory exchangeFactory,
        ILogger<BotRunner> logger)
    {
        this.exchangeFactory = exchangeFactory;
        this.logger = logger;
    }

    static bool IRunner<BotEntity>.IsEnabled(BotEntity config)
        => config.Enabled;

    protected override async ValueTask<bool> Start(
        BotEntity config,
        Optional<BotEntity> oldConfig)
    {
        logger.LogInformation("Starting bot {Name}", config.Name);

        // Use injected dependencies
        var connection = await exchangeFactory.CreateConnection(config.Exchange);

        return true;
    }

    protected override void Stop(
        Optional<BotEntity> newConfig,
        Optional<BotEntity> oldConfig)
    {
        logger.LogInformation("Stopping bot");
        CleanupResources();
    }
}
```

---

## Hot-Reload Support

### OnConfigurationChange

**When Called**: Configuration changes while runner is active.

```csharp
protected override void OnConfigurationChange(
    BotEntity newConfig,
    Optional<BotEntity> oldConfig)
{
    if (!oldConfig.HasValue)
        return;  // Shouldn't happen

    // Determine what changed
    var symbolChanged = newConfig.Symbol != oldConfig.Value.Symbol;
    var paramsChanged = newConfig.Parameters != oldConfig.Value.Parameters;

    if (symbolChanged)
    {
        // Restart required for symbol change
        logger.LogInformation("Symbol changed - restarting");
        Stop(Optional.Some(newConfig), oldConfig);
        Start(newConfig, oldConfig).AsTask().Wait();
    }
    else if (paramsChanged)
    {
        // Hot-reload parameters
        logger.LogInformation("Parameters changed - hot-reloading");
        UpdateParameters(newConfig.Parameters);
    }
}
```

**Benefits**:
- Avoid full restart when possible
- Faster configuration updates
- Less downtime
- Preserve state when appropriate

---

## Fault Tracking

### WorkerStatus

```csharp
public enum WorkerStatus
{
    Uninitialized,  // Never started
    Starting,       // Start in progress
    Running,        // Active
    Stopping,       // Stop in progress
    Stopped,        // Stopped cleanly
    Faulted         // Failed to start or crashed
}
```

---

### Error Observable

```csharp
BotRunner runner = new BotRunner();

// Subscribe to errors
runner.Errors.Subscribe(ex =>
{
    Logger.LogError(ex, "Runner error");
    ShowErrorNotification($"Bot error: {ex.Message}");
});

// Feed config changes
configObservable.Subscribe(runner);

// If Start() fails, error emitted
```

---

### Fault Recovery

```csharp
public class FaultTolerantRunner : Runner<BotEntity, FaultTolerantRunner>
{
    private int failureCount = 0;

    protected override async ValueTask<bool> Start(
        BotEntity config,
        Optional<BotEntity> oldConfig)
    {
        try
        {
            await StartBot(config);
            failureCount = 0;  // Reset on success
            return true;
        }
        catch (Exception ex)
        {
            failureCount++;

            if (failureCount >= 3)
            {
                logger.LogError(ex, "Failed to start after 3 attempts");
                return false;  // Give up
            }

            // Retry with backoff
            await Task.Delay(TimeSpan.FromSeconds(failureCount * 5));
            return await Start(config, oldConfig);
        }
    }

    static bool IRunner<BotEntity>.IsEnabled(BotEntity config)
        => config.Enabled;

    protected override void Stop(
        Optional<BotEntity> newConfig,
        Optional<BotEntity> oldConfig)
    {
        StopBot();
    }
}
```

---

## Integration with Workspaces

### IWorkspaceDocumentRunner

```csharp
public interface IWorkspaceDocumentRunner<TKey, TValue>
{
    Type RunnerType { get; }
}
```

**Implementation**:
```csharp
public class BotRunner :
    Runner<BotEntity, BotRunner>,
    IWorkspaceDocumentRunner<string, BotEntity>
{
    public Type RunnerType => typeof(BotRunner);

    static bool IRunner<BotEntity>.IsEnabled(BotEntity config)
        => config.Enabled;

    // ... Start/Stop implementation ...
}
```

**Registration**:
```csharp
services.TryAddEnumerable(
    ServiceDescriptor.Singleton<IWorkspaceDocumentRunner<string, BotEntity>, BotRunner>()
);
```

---

### WorkspaceDocumentService Integration

```
Workspace Document File (bot-001.hjson)
    ↓ File change detected
IObservableReader<string, BotEntity>.Values
    ↓ Emits changeset
WorkspaceDocumentService<BotEntity>
    ↓ For each IWorkspaceDocumentRunner<string, BotEntity>
Get or Create Runner Instance
    ↓ Call OnNext(entity)
BotRunner processes change
    ↓ Checks IsEnabled(entity)
    ↓ If enabled: Start()
    ↓ If disabled: Stop()
Bot starts or stops
```

**See**: [Workspace Architecture - Document Types](../workspaces/document-types.md)

---

## Common Patterns

### Pattern 1: Simple Bot Runner

```csharp
public class TradingBotRunner : Runner<BotConfig, TradingBotRunner>
{
    private TradingBot? activeBot;

    static bool IRunner<BotConfig>.IsEnabled(BotConfig config)
        => config.Enabled;

    protected override async ValueTask<bool> Start(
        BotConfig config,
        Optional<BotConfig> oldConfig)
    {
        activeBot = new TradingBot(config);
        await activeBot.Start();
        return true;
    }

    protected override void Stop(
        Optional<BotConfig> newConfig,
        Optional<BotConfig> oldConfig)
    {
        activeBot?.Stop();
        activeBot = null;
    }
}
```

---

### Pattern 2: Connection Manager Runner

```csharp
public class DatabaseConnectionRunner : Runner<DbConfig, DatabaseConnectionRunner>
{
    private DbConnection? connection;

    static bool IRunner<DbConfig>.IsEnabled(DbConfig config)
        => config.EnableConnection;

    protected override async ValueTask<bool> Start(
        DbConfig config,
        Optional<DbConfig> oldConfig)
    {
        connection = new DbConnection(config.ConnectionString);
        await connection.OpenAsync();
        return true;
    }

    protected override void Stop(
        Optional<DbConfig> newConfig,
        Optional<DbConfig> oldConfig)
    {
        connection?.Close();
        connection?.Dispose();
        connection = null;
    }

    protected override void OnConfigurationChange(
        DbConfig newConfig,
        Optional<DbConfig> oldConfig)
    {
        // Connection string changed - need restart
        if (newConfig.ConnectionString != oldConfig.Value?.ConnectionString)
        {
            Stop(Optional.Some(newConfig), oldConfig);
            Start(newConfig, oldConfig);
        }
    }
}
```

---

### Pattern 3: Polling Service Runner

```csharp
public class PollingSensorRunner : Runner<SensorConfig, PollingSensorRunner>
{
    private IDisposable? pollSubscription;

    static bool IRunner<SensorConfig>.IsEnabled(SensorConfig config)
        => config.EnablePolling;

    protected override async ValueTask<bool> Start(
        SensorConfig config,
        Optional<SensorConfig> oldConfig)
    {
        // Start polling
        pollSubscription = Observable
            .Timer(TimeSpan.Zero, TimeSpan.FromSeconds(config.PollInterval))
            .Subscribe(async _ =>
            {
                var reading = await ReadSensor(config.SensorId);
                PublishReading(reading);
            });

        return true;
    }

    protected override void Stop(
        Optional<SensorConfig> newConfig,
        Optional<SensorConfig> oldConfig)
    {
        pollSubscription?.Dispose();
        pollSubscription = null;
    }

    protected override void OnConfigurationChange(
        SensorConfig newConfig,
        Optional<SensorConfig> oldConfig)
    {
        // Poll interval changed - restart polling
        if (newConfig.PollInterval != oldConfig.Value?.PollInterval)
        {
            Stop(Optional.Some(newConfig), oldConfig);
            Start(newConfig, oldConfig);
        }
    }
}
```

---

## Related Documentation

- **[Reactive Architecture](README.md)** - Overview
- **[Reactive Persistence](reactive-persistence.md)** - IObservableReader/Writer
- **[Workspace Document Types](../workspaces/document-types.md)** - Runner integration
- **[LionFire.Reactive](../../../src/LionFire.Reactive/CLAUDE.md)** - Runner implementation details

---

## Summary

**Runner Pattern** provides:

**Key Features**:
- **Automatic Lifecycle** - Start/stop based on configuration
- **IObserver<T>** - Feed configs via observables
- **Hot-Reload** - Update without restart
- **Fault Tracking** - WorkerStatus and error observables
- **Dependency Injection** - Constructor injection support

**Use Cases**:
- Trading bots (start/stop based on config)
- Database connections (enable/disable)
- Polling services (interval configuration)
- Background workers (any start/stop component)

**Integration**: Commonly used with `IWorkspaceDocumentRunner` for workspace documents that represent active components.

**Benefits**: Declarative lifecycle management, automatic start/stop, hot-reload support, fault tolerance.
