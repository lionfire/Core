# Persistence Patterns

## Overview

This guide covers **persistence patterns** in LionFire, focusing on file-based and database persistence using `IObservableReader/Writer`. These interfaces provide reactive, cached access to persisted data with automatic change detection.

**Key Concept**: `IObservableReader/Writer` provides a reactive abstraction over persistent storage (files, databases, etc.) with built-in caching and change notifications.

---

## Table of Contents

1. [IObservableReader/Writer](#iobservablereaderwriter)
2. [File-Based Persistence](#file-based-persistence)
3. [HJSON Serialization](#hjson-serialization)
4. [Database Persistence](#database-persistence)
5. [Write-Through Caching](#write-through-caching)
6. [Conflict Resolution](#conflict-resolution)

---

## IObservableReader/Writer

### IObservableReader\<TKey, TValue\>

**Location**: `LionFire.Reactive.Persistence` (via LionFire.Reactive)

**Purpose**: Reactive read access to keyed persistent data.

**This is the foundation for workspace documents.**

```csharp
public interface IObservableReader<TKey, TValue>
    where TKey : notnull
{
    // Observable cache of all values
    IObservableCache<Optional<TValue>, TKey> Values { get; }

    // Observable cache of keys only
    IObservableCache<TMetadata, TKey> Keys { get; }

    // Get single value
    ValueTask<Optional<TValue>> TryGetValue(TKey key);

    // Start watching all keys
    IDisposable ListenAllKeys();

    // Get observable for specific key
    IObservable<Optional<TValue>>? GetValueObservableIfExists(TKey key);
}
```

---

### IObservableWriter\<TKey, TValue\>

**Purpose**: Write operations for persistent data.

```csharp
public interface IObservableWriter<TKey, TValue>
    where TKey : notnull
{
    // Write value
    ValueTask Write(TKey key, TValue value);

    // Delete value
    ValueTask Remove(TKey key);
}
```

---

### IObservableReaderWriter\<TKey, TValue\>

**Purpose**: Combined read/write interface.

```csharp
public interface IObservableReaderWriter<TKey, TValue>
    : IObservableReader<TKey, TValue>
    , IObservableWriter<TKey, TValue>
{
    // Full CRUD operations
}
```

**This is the most commonly used persistence interface.**

---

## File-Based Persistence

### HjsonFsDirectoryReaderRx\<TKey, TValue\>

**Location**: `LionFire.IO.Reactive.Hjson`

**Purpose**: Reads HJSON files from a directory with automatic file watching.

**This is the standard implementation for workspace documents.**

```csharp
public class HjsonFsDirectoryReaderRx<TKey, TValue> : IObservableReader<TKey, TValue>
{
    public HjsonFsDirectoryReaderRx(
        IServiceProvider serviceProvider,
        DirectoryReferenceSelector dirSelector)
    {
        // Sets up FileSystemWatcher
        // Deserializes HJSON files
        // Publishes to DynamicData cache
    }

    // Reactive cache of entities
    public IObservableCache<Optional<TValue>, TKey> Values { get; }

    // File system watching
    private FileSystemWatcher fileWatcher;
}
```

#### How It Works

```
File System Changes
    ↓
FileSystemWatcher events
    ↓
File read + HJSON deserialization
    ↓
SourceCache<TValue, TKey> updated
    ↓
Values.Connect() emits changeset
    ↓
UI components update reactively
```

---

### HjsonFsDirectoryWriterRx\<TKey, TValue\>

**Purpose**: Writes HJSON files to a directory.

```csharp
public class HjsonFsDirectoryWriterRx<TKey, TValue> : IObservableWriter<TKey, TValue>
{
    public async ValueTask Write(TKey key, TValue value)
    {
        var filePath = GetFilePath(key);
        var hjson = HjsonValue.Save(value);
        await File.WriteAllTextAsync(filePath, hjson);

        // File watcher detects change
        // Reader automatically updates cache
    }

    public async ValueTask Remove(TKey key)
    {
        var filePath = GetFilePath(key);
        File.Delete(filePath);

        // File watcher detects deletion
        // Reader automatically removes from cache
    }
}
```

---

### Complete File Persistence Example

```csharp
// Setup (typically in workspace configurator)
var workspaceDir = new FileReference("C:\\Users\\Alice\\Workspaces\\workspace1");
var botsDir = workspaceDir.GetChild("Bots");

// Create reader
var reader = new HjsonFsDirectoryReaderRx<string, BotEntity>(
    serviceProvider,
    new DirectoryReferenceSelector(botsDir) { Recursive = true }
);

// Create writer
var writer = new HjsonFsDirectoryWriterRx<string, BotEntity>(
    serviceProvider,
    new DirectoryReferenceSelector(botsDir)
);

// Register in DI
services.AddSingleton<IObservableReader<string, BotEntity>>(reader);
services.AddSingleton<IObservableWriter<string, BotEntity>>(writer);

// Usage
var reader = services.GetRequiredService<IObservableReader<string, BotEntity>>();

// Read
var bot = await reader.TryGetValue("bot-001");
if (bot.HasValue)
{
    Console.WriteLine($"Bot: {bot.Value.Name}");
}

// Subscribe to all changes
reader.Values.Connect()
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            Console.WriteLine($"{change.Key}: {change.Reason}");
        }
    });

// Write
var writer = services.GetRequiredService<IObservableWriter<string, BotEntity>>();
await writer.Write("bot-001", new BotEntity { Name = "New Bot" });
// File created: C:\Users\Alice\Workspaces\workspace1\Bots\bot-001.hjson
```

---

## HJSON Serialization

### HJSON Format

**HJSON** is a user-friendly variant of JSON:

```hjson
// bot-001.hjson
// Root braces OMITTED per HJSON convention

name: Trading Bot Alpha
description: Scalping strategy for BTCUSDT
exchange: Binance
symbol: BTCUSDT
enabled: true
parameters: {
  timeframe: 15m
  stopLoss: 1.5
  takeProfit: 3.0
  maxOpenTrades: 3
}
comments: '''
  This bot has been performing well
  on the 15m timeframe.
'''
```

**Benefits**:
- Comments allowed
- Quotes optional for strings
- Multi-line strings
- Trailing commas allowed
- More readable than JSON

---

### Custom Serialization

```csharp
public class CustomSerializerReader : IObservableReader<string, MyEntity>
{
    protected virtual MyEntity Deserialize(string fileContent)
    {
        // Custom deserialization logic
        return MyCustomDeserializer.Parse(fileContent);
    }

    protected virtual string Serialize(MyEntity entity)
    {
        return MyCustomSerializer.Format(entity);
    }
}
```

---

## Database Persistence

### Database-Backed Observable Reader

```csharp
public class DbObservableReader<TKey, TValue> : IObservableReader<TKey, TValue>
    where TKey : notnull
    where TValue : class
{
    private readonly IDbContext db;
    private readonly SourceCache<Optional<TValue>, TKey> cache;

    public DbObservableReader(IDbContext db)
    {
        this.db = db;
        this.cache = new SourceCache<Optional<TValue>, TKey>(v => GetKey(v.Value));
    }

    public IObservableCache<Optional<TValue>, TKey> Values => cache.AsObservableCache();

    public async ValueTask<Optional<TValue>> TryGetValue(TKey key)
    {
        // Check cache first
        var cached = cache.Lookup(key);
        if (cached.HasValue)
            return cached.Value;

        // Load from database
        var entity = await db.Set<TValue>().FindAsync(key);

        if (entity != null)
        {
            cache.AddOrUpdate(Optional.Some(entity), key);
            return Optional.Some(entity);
        }

        return Optional.None<TValue>();
    }

    public IDisposable ListenAllKeys()
    {
        // Load all from database
        Task.Run(async () =>
        {
            var allEntities = await db.Set<TValue>().ToListAsync();

            cache.Edit(updater =>
            {
                foreach (var entity in allEntities)
                {
                    updater.AddOrUpdate(Optional.Some(entity), GetKey(entity));
                }
            });
        });

        return Disposable.Empty;
    }

    private TKey GetKey(TValue entity)
    {
        // Extract key from entity (e.g., entity.Id)
        var keyProperty = typeof(TValue).GetProperty("Id");
        return (TKey)keyProperty.GetValue(entity)!;
    }
}
```

---

### Database Writer

```csharp
public class DbObservableWriter<TKey, TValue> : IObservableWriter<TKey, TValue>
    where TKey : notnull
    where TValue : class
{
    private readonly IDbContext db;

    public DbObservableWriter(IDbContext db)
    {
        this.db = db;
    }

    public async ValueTask Write(TKey key, TValue value)
    {
        var existing = await db.Set<TValue>().FindAsync(key);

        if (existing != null)
        {
            db.Entry(existing).CurrentValues.SetValues(value);
        }
        else
        {
            db.Set<TValue>().Add(value);
        }

        await db.SaveChangesAsync();
    }

    public async ValueTask Remove(TKey key)
    {
        var entity = await db.Set<TValue>().FindAsync(key);
        if (entity != null)
        {
            db.Set<TValue>().Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}
```

---

## Write-Through Caching

### How It Works

```
Write Operation
    ↓
IObservableWriter.Write(key, value)
    ↓
1. Write to storage (file, database)
    ↓
2. Update in-memory cache
    ↓
3. Emit change notification
    ↓
UI updates automatically
```

**Benefits**:
- Immediate UI updates (no wait for file watcher)
- Consistent cache state
- No lag between write and read

---

### Implementation Pattern

```csharp
public class WriteThro ughCacheWriter<TKey, TValue> : IObservableWriter<TKey, TValue>
{
    private readonly SourceCache<TValue, TKey> cache;
    private readonly Func<TKey, TValue, Task> persistFunc;

    public async ValueTask Write(TKey key, TValue value)
    {
        // 1. Persist to storage
        await persistFunc(key, value);

        // 2. Update cache immediately
        cache.AddOrUpdate(value, key);

        // Cache emits change notification automatically
    }

    public async ValueTask Remove(TKey key)
    {
        // 1. Delete from storage
        await DeleteFromStorage(key);

        // 2. Remove from cache immediately
        cache.Remove(key);
    }
}
```

---

## Conflict Resolution

### File System Conflicts

**Problem**: File modified externally while in-memory changes pending.

**Strategy 1: Last Write Wins**
```csharp
// Simply overwrite
await writer.Write(key, value);  // Overwrites file
```

**Strategy 2: Detect Conflicts**
```csharp
public async ValueTask WriteWithConflictDetection(TKey key, TValue value)
{
    var currentValue = await reader.TryGetValue(key);

    if (currentValue.HasValue)
    {
        // Check if file modified since last read
        var fileModifiedTime = File.GetLastWriteTimeUtc(GetFilePath(key));

        if (fileModifiedTime > lastReadTime[key])
        {
            throw new ConflictException("File modified externally");
        }
    }

    await writer.Write(key, value);
}
```

**Strategy 3: User Resolution**
```csharp
public async ValueTask<WriteResult> WriteWithUserResolution(
    TKey key,
    TValue value)
{
    var conflict = await DetectConflict(key, value);

    if (conflict != null)
    {
        var resolution = await userConflictResolver.ResolveConflict(conflict);

        switch (resolution)
        {
            case ConflictResolution.OverwriteLocal:
                return await writer.Write(key, value);

            case ConflictResolution.KeepRemote:
                return WriteResult.Cancelled;

            case ConflictResolution.Merge:
                var merged = MergeValues(value, conflict.RemoteValue);
                return await writer.Write(key, merged);
        }
    }

    return await writer.Write(key, value);
}
```

---

## Related Documentation

- **[Async Data Overview](README.md)** - Overview and patterns
- **[Collections](collections.md)** - Async collections
- **[Caching Strategies](caching-strategies.md)** - Cache management
- **[Workspace Architecture](../../architecture/workspaces/README.md)** - Workspace persistence
- **[LionFire.Reactive](../../../src/LionFire.Reactive/CLAUDE.md)** - IObservableReader/Writer details

---

## Summary

**Persistence Patterns**:
- **IObservableReader<TKey, T>** - Reactive read from storage
- **IObservableWriter<TKey, T>** - Write to storage
- **IObservableReaderWriter<TKey, T>** - Combined CRUD

**File-Based**:
- HJSON files with FileSystemWatcher
- Automatic deserialization and caching
- File system change detection

**Database-Based**:
- Entity Framework integration
- Custom database adapters
- Write-through caching

**Key Feature**: Write operations automatically update cache and notify subscribers.

**Most Common**: File-based persistence with `HjsonFsDirectoryReaderRx/WriterRx` for workspace documents.
