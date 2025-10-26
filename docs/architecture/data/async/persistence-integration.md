# Persistence Integration

**Overview**: How LionFire's async data patterns integrate with the persistence layer through `IObservableReader/Writer`. These interfaces provide file-system backed, reactive collections that power workspace documents and other observable persistence scenarios.

---

## Table of Contents

1. [IObservableReader/Writer Overview](#iobservablereaderwriter-overview)
2. [File System-Backed Collections](#file-system-backed-collections)
3. [Serialization Strategies](#serialization-strategies)
4. [Write-Through Caching](#write-through-caching)
5. [Integration with Workspaces](#integration-with-workspaces)
6. [Complete Examples](#complete-examples)

---

## IObservableReader/Writer Overview

### From LionFire.Reactive

**These interfaces bridge async data access with reactive persistence**.

### IObservableReader\<TKey, TValue\>

**Purpose**: Observable read-only access to a keyed collection with lazy loading.

```csharp
public interface IObservableReader<TKey, TValue>
{
    // Keys: Lightweight metadata cache (e.g., file names)
    IObservableCache<TKey> Keys { get; }

    // Values: Full objects, loaded on-demand
    IObservableCache<Optional<TValue>, TKey> Values { get; }

    // Alias for Values
    IObservableCache<Optional<TValue>, TKey> ObservableCache { get; }

    // Subscribe to all keys/values (triggers loading)
    IDisposable ListenAllKeys();
    ValueTask<IDisposable> ListenAllValues();

    // Get specific value asynchronously
    ValueTask<Optional<TValue>> TryGetValue(TKey key);

    // Get observable for specific key (reactive updates)
    IObservable<TValue?> GetValueObservable(TKey key);
    IObservable<TValue?> GetValueObservableIfExists(TKey key);
}
```

**Key Concepts**:

**Keys vs Values**:
```csharp
// Keys: Always loaded (cheap metadata)
reader.Keys.Items  // ["file1.json", "file2.json", "file3.json"]

// Values: Loaded on-demand (expensive deserialization)
reader.Values.Items  // May contain Optional.None for unloaded items
```

**On-Demand Loading**:
```csharp
// Subscribing to specific key triggers load
var subscription = reader.GetValueObservable("file1.json")
    .Subscribe(value => DisplayData(value));

// Only "file1.json" is deserialized, others remain unloaded
```

### IObservableWriter\<TKey, TValue\>

**Purpose**: Observable write operations with auto-save support.

```csharp
public interface IObservableWriter<TKey, TValue>
{
    // Write value
    ValueTask Write(TKey key, TValue value);

    // Remove value
    ValueTask<bool> Remove(TKey key);

    // Observable stream of write operations
    IObservable<WriteOperation<TKey, TValue>> WriteOperations { get; }

    // Auto-save from observable source
    IDisposable Synchronize(
        IObservable<TValue> source,
        TKey key,
        ...);

    // Auto-save from reactive object
    IDisposable Synchronize(
        IReactiveNotifyPropertyChanged<IReactiveObject> source,
        TKey key,
        ...);
}
```

**Auto-Save**:
```csharp
// Synchronize reactive object to file
var bot = new BotEntity { Name = "My Bot" };

writer.Synchronize(
    source: bot,  // ReactiveObject
    key: "my-bot",
    throttle: TimeSpan.FromSeconds(2)  // Debounce
);

// Now when bot properties change:
bot.Name = "Updated";  // Starts 2-second timer
// ... 2 seconds later: Write("my-bot", bot) called automatically
```

### IObservableReaderWriter\<TKey, TValue\>

**Combined interface**:

```csharp
public interface IObservableReaderWriter<TKey, TValue>
    : IObservableReader<TKey, TValue>
    , IObservableWriter<TKey, TValue>
{
}
```

**Extension Methods**:
```csharp
// Synchronous cache lookup
var result = readerWriter.TryGet("my-key");
if (result.HasValue)
{
    var value = result.Value;
}

// Auto-save value changes
readerWriter.AutosaveValueChanges();
// Subscribes to Values cache, auto-writes on changes
```

---

## File System-Backed Collections

### HjsonFsDirectoryReaderRx\<TKey, TValue\>

**Purpose**: File directory reader with HJSON deserialization.

**What It Does**:
1. Polls directory for `*.hjson` files
2. Keys cache populated with file names
3. Values loaded on-demand when observed
4. File changes automatically detected and reloaded

**Implementation** (simplified):
```csharp
public class HjsonFsDirectoryReaderRx<TKey, TValue> : IObservableReader<TKey, TValue>
{
    private readonly string directoryPath;
    private readonly SourceCache<TKey> keysCache = new(k => k);
    private readonly SourceCache<Optional<TValue>, TKey> valuesCache = new(k => k);

    public IObservableCache<TKey> Keys => keysCache.AsObservableCache();
    public IObservableCache<Optional<TValue>, TKey> Values => valuesCache.AsObservableCache();

    public HjsonFsDirectoryReaderRx(DirectorySelector dirSelector, ...)
    {
        // Start file watching
        ObservableFileInfos.PollOnDemand(directoryPath, "*.hjson")
            .Subscribe(changeSet =>
            {
                foreach (var change in changeSet)
                {
                    switch (change.Reason)
                    {
                        case ChangeReason.Add:
                        case ChangeReason.Update:
                            // Add/update key
                            var key = GetKeyFromFileName(change.Current.Name);
                            keysCache.AddOrUpdate(key);
                            // Value will be loaded when observed
                            break;

                        case ChangeReason.Remove:
                            var removedKey = GetKeyFromFileName(change.Current.Name);
                            keysCache.Remove(removedKey);
                            valuesCache.Remove(removedKey);
                            break;
                    }
                }
            });
    }

    public async ValueTask<Optional<TValue>> TryGetValue(TKey key)
    {
        // Check cache
        var cached = valuesCache.Lookup(key);
        if (cached.HasValue && cached.Value.HasValue)
            return cached.Value;

        // Load from file
        var filePath = GetFilePathForKey(key);
        if (!File.Exists(filePath))
            return Optional<TValue>.None;

        var hjson = await File.ReadAllTextAsync(filePath);
        var value = Hjson.HjsonValue.Load(hjson).Deserialize<TValue>();

        // Cache it
        valuesCache.AddOrUpdate(key, Optional<TValue>.Create(value));

        return Optional<TValue>.Create(value);
    }

    public IObservable<TValue?> GetValueObservableIfExists(TKey key)
    {
        // Observable that emits when file changes
        return Values
            .Connect()
            .Filter(k => k.Equals(key))
            .Transform(opt => opt.HasValue ? opt.Value : default(TValue))
            .QueryWhenChanged(changes => changes.FirstOrDefault());
    }
}
```

### HjsonFsDirectoryWriterRx\<TKey, TValue\>

**Purpose**: File directory writer with HJSON serialization.

**What It Does**:
1. Writes values as HJSON files
2. Publishes write operations to observable
3. Supports auto-save synchronization
4. Thread-safe writes

**Implementation** (simplified):
```csharp
public class HjsonFsDirectoryWriterRx<TKey, TValue> : IObservableWriter<TKey, TValue>
{
    private readonly string directoryPath;
    private readonly Subject<WriteOperation<TKey, TValue>> writeOps = new();

    public IObservable<WriteOperation<TKey, TValue>> WriteOperations => writeOps;

    public async ValueTask Write(TKey key, TValue value)
    {
        var operation = new WriteOperation<TKey, TValue>(key, value);
        writeOps.OnNext(operation);

        try
        {
            var filePath = Path.Combine(directoryPath, $"{key}.hjson");

            // Serialize to HJSON (omit root braces)
            var json = JsonConvert.SerializeObject(value, Formatting.Indented);
            var hjson = Hjson.HjsonValue.Parse(json).ToString(Stringify.Hjson);

            await File.WriteAllTextAsync(filePath, hjson);

            operation.Complete();
        }
        catch (Exception ex)
        {
            operation.Fail(ex);
        }
    }

    public async ValueTask<bool> Remove(TKey key)
    {
        var filePath = Path.Combine(directoryPath, $"{key}.hjson");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }
        return false;
    }
}
```

### ObservableReaderWriterFromComponents

**Combines separate reader and writer**:

```csharp
public class ObservableReaderWriterFromComponents<TKey, TValue>
    : IObservableReaderWriter<TKey, TValue>
{
    private readonly IObservableReader<TKey, TValue> reader;
    private readonly IObservableWriter<TKey, TValue> writer;

    public ObservableReaderWriterFromComponents(
        IObservableReader<TKey, TValue> reader,
        IObservableWriter<TKey, TValue> writer)
    {
        this.reader = reader;
        this.writer = writer;
    }

    // Delegate read operations to reader
    public IObservableCache<TKey> Keys => reader.Keys;
    public IObservableCache<Optional<TValue>, TKey> Values => reader.Values;
    // ...

    // Delegate write operations to writer
    public ValueTask Write(TKey key, TValue value) => writer.Write(key, value);
    public ValueTask<bool> Remove(TKey key) => writer.Remove(key);
    // ...
}
```

**Usage**:
```csharp
var reader = new HjsonFsDirectoryReaderRx<string, BotEntity>(...);
var writer = new HjsonFsDirectoryWriterRx<string, BotEntity>(...);

var readerWriter = new ObservableReaderWriterFromComponents<string, BotEntity>(reader, writer);

// Now have combined read/write
await readerWriter.TryGetValue("bot1");
await readerWriter.Write("bot1", updatedBot);
```

---

## Serialization Strategies

### HJSON (Default)

**Human-Friendly JSON**:

```hjson
// File: Bots/my-bot.hjson
// Root braces omitted (HJSON convention)

name: My Trading Bot
description: Scalping strategy
enabled: true
parameters: {
  stopLoss: 1.5
  takeProfit: 3.0
}
```

**Benefits**:
- Comments supported
- Trailing commas OK
- Unquoted keys
- Human-readable

**Serialization**:
```csharp
// Entity → HJSON
var json = JsonConvert.SerializeObject(entity);
var hjson = Hjson.HjsonValue.Parse(json).ToString(Stringify.Hjson);

// HJSON → Entity
var hjsonText = await File.ReadAllTextAsync(path);
var entity = Hjson.HjsonValue.Load(hjsonText).Deserialize<BotEntity>();
```

### JSON

**Strict JSON** alternative:

```csharp
// Use JsonFsDirectoryReaderRx instead
var reader = new JsonFsDirectoryReaderRx<string, BotEntity>(dirSelector);
```

### Custom Serialization

**Implement custom reader/writer**:

```csharp
public class YamlFsDirectoryReaderRx<TKey, TValue> : IObservableReader<TKey, TValue>
{
    protected override TValue Deserialize(string filePath)
    {
        var yaml = File.ReadAllText(filePath);
        return YamlSerializer.Deserialize<TValue>(yaml);
    }
}
```

---

## Write-Through Caching

### Automatic Persistence

**Auto-save on property changes**:

```csharp
// Create reader/writer
var readerWriter = new ObservableReaderWriterFromComponents<string, BotEntity>(reader, writer);

// Enable auto-save
var subscription = readerWriter.AutosaveValueChanges();

// Now when cached values change:
var bot = readerWriter.Values.Lookup("bot1").Value.Value;
bot.Name = "Updated";  // Automatically triggers Write("bot1", bot)
```

**Debounced Auto-Save**:

```csharp
writer.Synchronize(
    source: bot,  // ReactiveObject entity
    key: "bot1",
    throttle: TimeSpan.FromSeconds(2)  // Debounce 2 seconds
);

// Changes are batched:
bot.Name = "Update 1";  // Starts timer
bot.Enabled = true;     // Resets timer
// ... 2 seconds later: Write("bot1", bot) called once
```

### Write Operations Observable

**Track all writes**:

```csharp
writer.WriteOperations.Subscribe(operation =>
{
    Console.WriteLine($"Writing {operation.Key}");

    operation.Completion.Subscribe(
        onNext: _ => Console.WriteLine($"Write succeeded: {operation.Key}"),
        onError: ex => Console.WriteLine($"Write failed: {ex.Message}")
    );
});
```

---

## Integration with Workspaces

### Workspace Registration

**Workspaces automatically create reader/writer instances**:

```csharp
// In Program.cs
services
    .AddWorkspaceChildType<BotEntity>()
    .AddWorkspaceDocumentService<string, BotEntity>();

// When workspace loads, WorkspaceTypesConfigurator calls:
services.RegisterObservablesInDir<BotEntity>(serviceProvider, dirSelector);

// Which registers:
services.AddSingleton<IObservableReader<string, BotEntity>>(sp =>
{
    var reader = new HjsonFsDirectoryReaderRx<string, BotEntity>(dirSelector, ...);
    return reader;
});

services.AddSingleton<IObservableWriter<string, BotEntity>>(sp =>
{
    var writer = new HjsonFsDirectoryWriterRx<string, BotEntity>(dirSelector, ...);
    return writer;
});

services.AddSingleton<IObservableReaderWriter<string, BotEntity>>(sp =>
{
    var reader = sp.GetRequiredService<IObservableReader<string, BotEntity>>();
    var writer = sp.GetRequiredService<IObservableWriter<string, BotEntity>>();
    return new ObservableReaderWriterFromComponents<string, BotEntity>(reader, writer);
});
```

### Directory Structure Convention

**Workspace**:
```
C:\Users\Alice\Trading\Workspaces\workspace1\
├── Bots\              ← IObservableReader/Writer<string, BotEntity>
│   ├── bot-alpha.hjson
│   ├── bot-beta.hjson
│   └── bot-gamma.hjson
├── Portfolios\        ← IObservableReader/Writer<string, Portfolio>
│   └── portfolio1.hjson
└── Strategies\        ← IObservableReader/Writer<string, Strategy>
    └── strategy1.hjson
```

**Each subdirectory gets its own reader/writer pair**.

### Reactive File Watching

**Files watched for external changes**:

```
1. File modified externally (user edits in text editor)
2. ObservableFileInfos detects change (via polling)
3. HjsonFsDirectoryReaderRx reloads file
4. Deserializes updated entity
5. Updates Values cache
6. Emits changeset via Values.Connect()
7. UI components subscribed to observable update automatically
```

---

## Complete Examples

### Example 1: File-Based Configuration

**Goal**: Application settings persisted as HJSON with auto-save.

**Setup**:
```csharp
// Create reader/writer for settings directory
var dirSelector = new DirectorySelector("C:\\App\\Settings");
var options = new DirectoryTypeOptions { SecondExtension = ".hjson" };

var reader = new HjsonFsDirectoryReaderRx<string, AppSettings>(dirSelector, options);
var writer = new HjsonFsDirectoryWriterRx<string, AppSettings>(dirSelector, options);
var readerWriter = new ObservableReaderWriterFromComponents<string, AppSettings>(reader, writer);

// Register in DI
services.AddSingleton<IObservableReaderWriter<string, AppSettings>>(readerWriter);
```

**Usage in ViewModel**:
```csharp
public class SettingsVM : ReactiveObject
{
    private readonly IObservableReaderWriter<string, AppSettings> settingsRW;

    public ObservableReaderWriterItemVM<string, AppSettings, AppSettingsVM> CurrentSettings { get; }

    public SettingsVM(IObservableReaderWriter<string, AppSettings> settingsRW)
    {
        this.settingsRW = settingsRW;

        CurrentSettings = new ObservableReaderWriterItemVM<string, AppSettings, AppSettingsVM>(
            settingsRW, settingsRW);

        // Load app settings
        CurrentSettings.Id = "app";  // Loads from Settings/app.hjson

        // Enable auto-save
        settingsRW.AutosaveValueChanges();
    }
}
```

**Usage in UI**:
```razor
@inject SettingsVM ViewModel

<MudTextField @bind-Value="ViewModel.CurrentSettings.Value.ApiKey" Label="API Key" />
<MudTextField @bind-Value="ViewModel.CurrentSettings.Value.Endpoint" Label="Endpoint" />

<!-- Changes auto-save via AutosaveValueChanges() -->
```

### Example 2: Workspace Documents

**Goal**: Bot management with list and detail views.

**Registration**:
```csharp
services
    .AddWorkspaceChildType<BotEntity>()
    .AddWorkspaceDocumentService<string, BotEntity>();

// When workspace loads, gets IObservableReaderWriter<string, BotEntity>
```

**List View**:
```razor
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices">
    <!-- Component internally uses reader to populate grid -->
</ObservableDataView>
```

**Detail View**:
```razor
@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    private ObservableReaderWriterItemVM<string, BotEntity, BotVM>? VM { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        var reader = WorkspaceServices.GetService<IObservableReader<string, BotEntity>>();
        var writer = WorkspaceServices.GetService<IObservableWriter<string, BotEntity>>();

        VM = new ObservableReaderWriterItemVM<string, BotEntity, BotVM>(reader, writer);
        VM.Id = BotId;  // Loads from workspace/Bots/{BotId}.hjson
    }

    private async Task Save()
    {
        await VM.Write();  // Writes to workspace/Bots/{BotId}.hjson
    }
}
```

### Example 3: Multi-Source Persistence

**Goal**: Overlay configs from multiple directories (app, user, workspace).

**Setup**:
```csharp
// App-level configs (read-only)
var appReader = new HjsonFsDirectoryReaderRx<string, Config>(appConfigDir);

// User-level configs (writable)
var userReaderWriter = new ObservableReaderWriterFromComponents<string, Config>(
    new HjsonFsDirectoryReaderRx<string, Config>(userConfigDir),
    new HjsonFsDirectoryWriterRx<string, Config>(userConfigDir)
);

// Workspace-level configs (writable)
var workspaceReaderWriter = new ObservableReaderWriterFromComponents<string, Config>(
    new HjsonFsDirectoryReaderRx<string, Config>(workspaceConfigDir),
    new HjsonFsDirectoryWriterRx<string, Config>(workspaceConfigDir)
);

// Merge keys from all sources
var allKeys = appReader.Keys.Connect()
    .Merge(userReaderWriter.Keys.Connect())
    .Merge(workspaceReaderWriter.Keys.Connect())
    .AsObservableCache();

// Priority: workspace > user > app
var config = workspaceReaderWriter.TryGet("theme").Value
    ?? userReaderWriter.TryGet("theme").Value
    ?? appReader.TryGet("theme").Value;
```

---

## Summary

### Key Interfaces

| Interface | Purpose | Typical Implementation |
|-----------|---------|----------------------|
| `IObservableReader<TKey, TValue>` | Read-only reactive collection | HjsonFsDirectoryReaderRx |
| `IObservableWriter<TKey, TValue>` | Write operations | HjsonFsDirectoryWriterRx |
| `IObservableReaderWriter<TKey, TValue>` | Combined read/write | ObservableReaderWriterFromComponents |

### Key Patterns

1. **On-Demand Loading** - Keys loaded immediately, values loaded when observed
2. **File Watching** - Automatic reload when files change externally
3. **Auto-Save** - Synchronize reactive objects to files automatically
4. **Write-Through** - Changes immediately persisted
5. **Observable Operations** - Subscribe to reads/writes for UI feedback

### Integration Points

```
Workspace Services
    ↓ Provide
IObservableReader/Writer<string, BotEntity>
    ↓ Backed by
HjsonFsDirectoryReaderRx / WriterRx
    ↓ Watches
File System (workspace/Bots/*.hjson)
    ↓ Wrapped by
ObservableReaderWriterItemVM
    ↓ Bound to
Blazor UI Components
```

### Benefits

✅ **Reactive** - File changes auto-propagate to UI
✅ **On-Demand** - Only load what's observed
✅ **Auto-Save** - Changes persist automatically
✅ **Workspace-Scoped** - Each workspace has isolated reader/writer
✅ **Observable** - All operations and state changes observable
✅ **File-Based** - Human-readable, version-controllable

### Related Documentation

- **[Core Async Patterns](async-patterns.md)** - IGetter/ISetter/IValue
- **[Reactive Data Patterns](reactive-data.md)** - RxO integration
- **[Workspace Architecture](../workspaces/README.md)** - Workspace usage
- **[MVVM Data Binding](../mvvm/data-binding.md)** - ViewModel wrappers
- **Library References**:
  - [LionFire.Reactive](../../src/LionFire.Reactive/CLAUDE.md)
  - [LionFire.IO.Reactive.Hjson](../../src/LionFire.IO.Reactive.Hjson/CLAUDE.md) (if exists)
