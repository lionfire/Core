# Getting Started: File-Based Persistence

## Overview

This guide introduces **file-based persistence** patterns in LionFire using `IObservableReader` and `IObservableWriter`. Learn how to create reactive, file-backed collections that automatically detect changes, deserialize on-demand, and provide observable change notifications.

**What You'll Learn**:
- Reading files reactively with `IObservableReader`
- Writing files with `IObservableWriter`
- Automatic file watching and deserialization
- HJSON serialization patterns
- Building complete CRUD applications

**Prerequisites**:
- .NET 9.0+ SDK
- Completed [04-reactive-collections.md](04-reactive-collections.md) (recommended)
- Basic understanding of file I/O

---

## Setup

### 1. Create a New Console Project

```bash
dotnet new console -n LionFirePersistence
cd LionFirePersistence
```

### 2. Add Required Packages

```bash
dotnet add package LionFire.Reactive
dotnet add package LionFire.Serialization.Json
dotnet add package System.Text.Json
```

### 3. Create Data Directory

```csharp
var dataDir = "./data";
Directory.CreateDirectory(dataDir);
```

---

## Reading Files Reactively

Use `IObservableReader<TKey, TValue>` to read files with automatic change detection.

### Example: Configuration Files

```csharp
using LionFire.Reactive;
using DynamicData;
using System.Text.Json;

// Step 1: Define your model
public record AppConfig(string Name, string Environment, int Port);

// Step 2: Create observable reader
var configDir = "./configs";
Directory.CreateDirectory(configDir);

var reader = ObservableFsDocuments.Create<AppConfig>(
    dir: configDir,
    deserialize: bytes => JsonSerializer.Deserialize<AppConfig>(bytes)!
).AsObservableCache();

// Step 3: Subscribe to changes
reader.Connect()
    .Subscribe(changeSet =>
    {
        Console.WriteLine($"Configuration changes detected: {changeSet.Count}");

        foreach (var change in changeSet)
        {
            Console.WriteLine($"  {change.Reason}: {change.Key}");

            if (change.Current.HasValue)
            {
                var config = change.Current.Value;
                Console.WriteLine($"    {config.Name} ({config.Environment}) on port {config.Port}");
            }
        }
    });

// Step 4: Create config files
await File.WriteAllTextAsync(
    Path.Combine(configDir, "app.json"),
    JsonSerializer.Serialize(new AppConfig("MyApp", "Production", 8080),
        new JsonSerializerOptions { WriteIndented = true })
);

// Wait for file watcher
await Task.Delay(1500);

// Update config
await File.WriteAllTextAsync(
    Path.Combine(configDir, "app.json"),
    JsonSerializer.Serialize(new AppConfig("MyApp", "Development", 3000),
        new JsonSerializerOptions { WriteIndented = true })
);

await Task.Delay(1500);

// Output:
// Configuration changes detected: 1
//   Add: app.json
//     MyApp (Production) on port 8080
// Configuration changes detected: 1
//   Update: app.json
//     MyApp (Development) on port 3000
```

**Key Features**:
- Automatic file watching (polls every 1 second)
- On-demand deserialization
- Handles adds, updates, and removes
- Observable change notifications

---

## Writing Files

Implement `IObservableWriter<TKey, TValue>` for file writing.

### Example: Simple File Writer

```csharp
using LionFire.Reactive;
using System.Text.Json;

public record Note(string Title, string Content, DateTime Created);

public class NoteWriter : IObservableWriter<string, Note>
{
    private readonly string directory;

    public NoteWriter(string directory)
    {
        this.directory = directory;
        Directory.CreateDirectory(directory);
    }

    public async ValueTask Write(string key, Note value)
    {
        var path = Path.Combine(directory, $"{key}.json");
        var json = JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(path, json);
        Console.WriteLine($"✅ Saved: {key}");
    }

    public async ValueTask<bool> Remove(string key)
    {
        var path = Path.Combine(directory, $"{key}.json");

        if (File.Exists(path))
        {
            File.Delete(path);
            Console.WriteLine($"🗑️  Deleted: {key}");
            return true;
        }

        return false;
    }

    // Observable write operations (simplified for demo)
    public IObservable<WriteOperation<string, Note>> WriteOperations =>
        Observable.Empty<WriteOperation<string, Note>>();
}

// Usage
var writer = new NoteWriter("./notes");

var note = new Note(
    Title: "Meeting Notes",
    Content: "Discuss project timeline",
    Created: DateTime.Now
);

await writer.Write("meeting-2025-11-21", note);

// Check file was created
var files = Directory.GetFiles("./notes");
Console.WriteLine($"Files: {string.Join(", ", files.Select(Path.GetFileName))}");

// Output:
// ✅ Saved: meeting-2025-11-21
// Files: meeting-2025-11-21.json
```

---

## Combined Read/Write

Use both reader and writer for complete CRUD operations.

### Example: Todo List with Persistence

```csharp
using LionFire.Reactive;
using DynamicData;
using System.Text.Json;

public record TodoItem(string Id, string Title, bool Completed, DateTime Created);

public class TodoRepository
{
    private readonly string directory;
    private readonly IObservableCache<Optional<TodoItem>, string> reader;

    public TodoRepository(string directory)
    {
        this.directory = directory;
        Directory.CreateDirectory(directory);

        reader = ObservableFsDocuments.Create<TodoItem>(
            dir: directory,
            deserialize: bytes => JsonSerializer.Deserialize<TodoItem>(bytes)!
        ).AsObservableCache();
    }

    public IObservable<IChangeSet<Optional<TodoItem>, string>> Connect() =>
        reader.Connect();

    public async Task Add(TodoItem item)
    {
        var path = Path.Combine(directory, $"{item.Id}.json");
        var json = JsonSerializer.Serialize(item, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(path, json);
        Console.WriteLine($"✅ Added: {item.Title}");
    }

    public async Task Update(TodoItem item)
    {
        await Add(item);  // Same as add
        Console.WriteLine($"📝 Updated: {item.Title}");
    }

    public async Task Delete(string id)
    {
        var path = Path.Combine(directory, $"{id}.json");

        if (File.Exists(path))
        {
            File.Delete(path);
            Console.WriteLine($"🗑️  Deleted: {id}");
        }
    }

    public Optional<TodoItem> Get(string id) =>
        reader.Lookup(id);

    public IEnumerable<TodoItem> GetAll() =>
        reader.Items
            .Where(opt => opt.HasValue)
            .Select(opt => opt.Value!);
}

// Usage
var repo = new TodoRepository("./todos");

// Subscribe to changes
repo.Connect()
    .Subscribe(changeSet =>
    {
        Console.WriteLine($"\n📊 Repository changes: {changeSet.Count}");
    });

// Add items
await repo.Add(new TodoItem("1", "Buy groceries", false, DateTime.Now));
await Task.Delay(1500);

await repo.Add(new TodoItem("2", "Write code", false, DateTime.Now));
await Task.Delay(1500);

// Update item
var item = repo.Get("1");
if (item.HasValue)
{
    var updated = item.Value with { Completed = true };
    await repo.Update(updated);
    await Task.Delay(1500);
}

// List all
Console.WriteLine("\n📋 All todos:");
foreach (var todo in repo.GetAll())
{
    var status = todo.Completed ? "✓" : " ";
    Console.WriteLine($"  [{status}] {todo.Title}");
}

// Delete item
await repo.Delete("1");
await Task.Delay(1500);

// Output:
// ✅ Added: Buy groceries
// 📊 Repository changes: 1
//
// ✅ Added: Write code
// 📊 Repository changes: 1
//
// ✅ Added: Buy groceries
// 📝 Updated: Buy groceries
// 📊 Repository changes: 1
//
// 📋 All todos:
//   [✓] Buy groceries
//   [ ] Write code
//
// 🗑️  Deleted: 1
// 📊 Repository changes: 1
```

---

## HJSON Serialization

HJSON is a user-friendly JSON format with comments and relaxed syntax.

### Example: HJSON Configuration

```csharp
using Hjson;
using LionFire.Reactive;
using DynamicData;

public record DatabaseConfig(
    string Host,
    int Port,
    string Database,
    string Username,
    string Password
);

public class HjsonConfigReader
{
    private readonly IObservableCache<Optional<DatabaseConfig>, string> cache;

    public HjsonConfigReader(string directory)
    {
        cache = ObservableFsDocuments.Create<DatabaseConfig>(
            dir: directory,
            deserialize: bytes =>
            {
                var text = System.Text.Encoding.UTF8.GetString(bytes);
                var hjson = HjsonValue.Parse(text);
                var json = hjson.ToString();
                return System.Text.Json.JsonSerializer.Deserialize<DatabaseConfig>(json)!;
            }
        ).AsObservableCache();
    }

    public IObservable<IChangeSet<Optional<DatabaseConfig>, string>> Connect() =>
        cache.Connect();

    public Optional<DatabaseConfig> Get(string key) =>
        cache.Lookup(key);
}

// Create HJSON config file
var configDir = "./config";
Directory.CreateDirectory(configDir);

var hjsonContent = @"
{
  # Database configuration
  host: localhost
  port: 5432
  database: myapp

  # Credentials
  username: admin
  password: secret123
}";

await File.WriteAllTextAsync(
    Path.Combine(configDir, "database.hjson"),
    hjsonContent
);

// Read with HJSON reader
var reader = new HjsonConfigReader(configDir);

reader.Connect()
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            if (change.Current.HasValue)
            {
                var config = change.Current.Value;
                Console.WriteLine($"Database config loaded:");
                Console.WriteLine($"  Host: {config.Host}:{config.Port}");
                Console.WriteLine($"  Database: {config.Database}");
            }
        }
    });

await Task.Delay(1500);

// Output:
// Database config loaded:
//   Host: localhost:5432
//   Database: myapp
```

---

## Practical Example: Document Management System

```csharp
using LionFire.Reactive;
using DynamicData;
using System.Text.Json;
using System.Collections.ObjectModel;

public record Document(
    string Id,
    string Title,
    string Content,
    string Category,
    DateTime Created,
    DateTime Modified
);

public class DocumentManager
{
    private readonly string directory;
    private readonly IObservableCache<Optional<Document>, string> reader;
    private readonly SourceCache<Document, string> cache;

    public ReadOnlyObservableCollection<Document> Documents { get; }
    public ReadOnlyObservableCollection<string> Categories { get; }

    public DocumentManager(string directory)
    {
        this.directory = directory;
        Directory.CreateDirectory(directory);

        // Initialize reader
        reader = ObservableFsDocuments.Create<Document>(
            dir: directory,
            deserialize: bytes => JsonSerializer.Deserialize<Document>(bytes)!
        ).AsObservableCache();

        // Create local cache
        cache = new SourceCache<Document, string>(d => d.Id);

        // Sync reader to cache
        reader.Connect()
            .Transform(opt => opt.HasValue ? opt.Value : null!)
            .Filter(d => d != null)
            .Subscribe(changeSet =>
            {
                foreach (var change in changeSet)
                {
                    if (change.Reason == ChangeReason.Add || change.Reason == ChangeReason.Update)
                        cache.AddOrUpdate(change.Current);
                    else if (change.Reason == ChangeReason.Remove)
                        cache.Remove(change.Current);
                }
            });

        // Bind documents
        cache.Connect()
            .Sort(SortExpressionComparer<Document>.Descending(d => d.Modified))
            .Bind(out var docs)
            .Subscribe();

        Documents = docs;

        // Bind categories
        cache.Connect()
            .Transform(d => d.Category)
            .DistinctValues(c => c)
            .Sort(SortExpressionComparer<string>.Ascending(c => c))
            .Bind(out var cats)
            .Subscribe();

        Categories = cats;
    }

    public async Task Create(string title, string content, string category)
    {
        var doc = new Document(
            Id: Guid.NewGuid().ToString("N"),
            Title: title,
            Content: content,
            Category: category,
            Created: DateTime.Now,
            Modified: DateTime.Now
        );

        await SaveDocument(doc);
        Console.WriteLine($"✅ Created: {title}");
    }

    public async Task Update(string id, string? title = null, string? content = null)
    {
        var existing = cache.Lookup(id);
        if (!existing.HasValue) return;

        var doc = existing.Value;
        var updated = doc with
        {
            Title = title ?? doc.Title,
            Content = content ?? doc.Content,
            Modified = DateTime.Now
        };

        await SaveDocument(updated);
        Console.WriteLine($"📝 Updated: {updated.Title}");
    }

    public async Task Delete(string id)
    {
        var path = Path.Combine(directory, $"{id}.json");

        if (File.Exists(path))
        {
            File.Delete(path);
            Console.WriteLine($"🗑️  Deleted document");
        }
    }

    public IEnumerable<Document> Search(string query)
    {
        return Documents.Where(d =>
            d.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            d.Content.Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Document> GetByCategory(string category)
    {
        return Documents.Where(d => d.Category == category);
    }

    private async Task SaveDocument(Document doc)
    {
        var path = Path.Combine(directory, $"{doc.Id}.json");
        var json = JsonSerializer.Serialize(doc, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(path, json);
    }
}

// Usage
var docManager = new DocumentManager("./documents");

// Wait for initial load
await Task.Delay(1500);

// Create documents
await docManager.Create("Getting Started", "Welcome to the system", "Tutorial");
await Task.Delay(1500);

await docManager.Create("API Reference", "Complete API docs", "Reference");
await Task.Delay(1500);

await docManager.Create("FAQ", "Frequently asked questions", "Tutorial");
await Task.Delay(1500);

// Display documents
Console.WriteLine($"\n📚 Total documents: {docManager.Documents.Count}");
Console.WriteLine($"📁 Categories: {string.Join(", ", docManager.Categories)}");

Console.WriteLine("\nAll documents:");
foreach (var doc in docManager.Documents)
{
    Console.WriteLine($"  [{doc.Category}] {doc.Title}");
    Console.WriteLine($"      Modified: {doc.Modified:g}");
}

// Search
Console.WriteLine("\nSearch for 'API':");
foreach (var doc in docManager.Search("API"))
{
    Console.WriteLine($"  {doc.Title}");
}

// Filter by category
Console.WriteLine("\nTutorial documents:");
foreach (var doc in docManager.GetByCategory("Tutorial"))
{
    Console.WriteLine($"  {doc.Title}");
}

// Update a document
var firstDoc = docManager.Documents.FirstOrDefault();
if (firstDoc != null)
{
    await docManager.Update(firstDoc.Id, content: "Updated content");
    await Task.Delay(1500);
}
```

---

## Best Practices

### 1. Use JSON for Structured Data

```csharp
// ✅ Good - JSON for structured data
var config = JsonSerializer.Serialize(data, new JsonSerializerOptions
{
    WriteIndented = true  // Human-readable
});

// ✅ Good - HJSON for user-editable configs
// Allows comments and relaxed syntax
```

### 2. Handle Deserialization Errors

```csharp
// ✅ Good - Graceful error handling
deserialize: bytes =>
{
    try
    {
        return JsonSerializer.Deserialize<T>(bytes)!;
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"Deserialization error: {ex.Message}");
        return default(T)!;
    }
}

// ❌ Avoid - Let exceptions crash the watcher
deserialize: bytes => JsonSerializer.Deserialize<T>(bytes)!
```

### 3. Use Meaningful File Names

```csharp
// ✅ Good - Descriptive file names
$"{doc.Id}.json"           // Unique ID
$"{user.Username}.json"    // Natural key
$"config-{env}.json"       // Categorized

// ❌ Avoid - Generic names
"data.json"
"file1.json"
```

### 4. Atomic Writes

```csharp
// ✅ Good - Write to temp, then move
var tempPath = path + ".tmp";
await File.WriteAllTextAsync(tempPath, json);
File.Move(tempPath, path, overwrite: true);

// ❌ Avoid - Direct write (can corrupt on failure)
await File.WriteAllTextAsync(path, json);
```

---

## Summary

**File-Based Persistence** provides:

**Core Patterns**:
- `IObservableReader<TKey, TValue>` - Reactive file reading
- `IObservableWriter<TKey, TValue>` - File writing
- `ObservableFsDocuments` - Complete file-backed collections
- Automatic file watching
- On-demand deserialization

**Benefits**:
- Simple setup (just files!)
- Automatic change detection
- Human-readable formats
- No database required
- Observable change notifications

**Next Steps**:
1. Read [06-vos-introduction.md](06-vos-introduction.md) for Virtual Object System
2. Explore [Persistence Docs](../../data/async/persistence.md) for advanced patterns
3. Review [Reactive Persistence Architecture](../../architecture/reactive/reactive-persistence.md)

---

## Exercise

Build a blog system that:
1. Stores posts as JSON files
2. Watches directory for changes
3. Provides search functionality
4. Groups posts by tags
5. Sorts by publish date
6. Binds to an ObservableCollection for UI
7. Supports CRUD operations

Use file-based persistence patterns from this guide!
