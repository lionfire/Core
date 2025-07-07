# VOS Examples and Usage

This document provides practical examples and common usage patterns for the Virtual Object System.

## Getting Started

### Basic Setup

```csharp
using Microsoft.Extensions.DependencyInjection;
using LionFire.Vos;

// Configure services
var services = new ServiceCollection();

// Add VOS with default configuration
services.AddVos();

// Add filesystem support
services.AddVosFilesystem();

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Get VOS instance
var vos = serviceProvider.GetRequiredService<IVos>();
```

### Configuration with Options

```csharp
services.AddVos(options =>
{
    // Configure caching
    options.EnableCaching = true;
    options.CacheTimeout = TimeSpan.FromMinutes(10);
    
    // Add named roots
    options.Roots["data"] = new VobRootOptions
    {
        Name = "data",
        MountPath = "/app/data"
    };
    
    options.Roots["config"] = new VobRootOptions
    {
        Name = "config",
        MountPath = "/etc/myapp"
    };
});
```

## File Operations

### Reading Files

```csharp
// Simple file read
var configVob = vos["/config/app.json"];
var config = await configVob.Get<AppConfig>();

// With error handling
try
{
    var dataVob = vos["/data/users.json"];
    if (await dataVob.Exists())
    {
        var users = await dataVob.Get<List<User>>();
        Console.WriteLine($"Loaded {users.Count} users");
    }
}
catch (DeserializationException ex)
{
    Console.WriteLine($"Failed to load users: {ex.Message}");
}
```

### Writing Files

```csharp
// Simple write
var settingsVob = vos["/config/settings.json"];
await settingsVob.Set(new Settings 
{ 
    Theme = "dark",
    Language = "en-US" 
});

// Write with options
await settingsVob.Set(settings, new PersistenceOptions
{
    Format = "yaml",  // Save as YAML instead of JSON
    CreateOnly = true // Fail if file exists
});
```

### File Manipulation

```csharp
// Copy file
var source = vos["/data/template.json"];
var dest = vos["/data/newfile.json"];
var content = await source.Get<object>();
await dest.Set(content);

// Move file (copy + delete)
await dest.Set(await source.Get<object>());
await source.Delete();

// Check existence
if (await vos["/data/important.dat"].Exists())
{
    // Process file
}
```

## Directory Operations

### Listing Contents

```csharp
// List directory contents
var dataDir = vos["/data"];
foreach (var child in dataDir.Children)
{
    Console.WriteLine($"{child.Key}: {child.Value.Path}");
}

// Recursive listing
async Task ListRecursive(IVob vob, int indent = 0)
{
    var prefix = new string(' ', indent * 2);
    Console.WriteLine($"{prefix}{vob.Name}/");
    
    foreach (var child in vob.Children)
    {
        if (await child.Value.IsDirectory())
        {
            await ListRecursive(child.Value, indent + 1);
        }
        else
        {
            Console.WriteLine($"{prefix}  {child.Key}");
        }
    }
}
```

### Creating Directories

```csharp
// Create directory structure
var projectVob = vos["/projects/myproject"];
await projectVob["src"].EnsureDirectory();
await projectVob["docs"].EnsureDirectory();
await projectVob["tests"].EnsureDirectory();

// Create with initial files
await projectVob["src/main.cs"].Set("// Main file");
await projectVob["docs/README.md"].Set("# My Project");
```

## Configuration Management

### Layered Configuration

```csharp
// Set up configuration layers
public class ConfigurationExample
{
    private readonly IVos vos;
    
    public async Task SetupConfiguration()
    {
        // Mount default configuration
        vos.Mount("/config", "file:///usr/share/app/config", new MountOptions
        {
            Name = "defaults",
            Priority = 0,
            IsReadOnly = true
        });
        
        // Mount user configuration
        vos.Mount("/config", "file:///home/user/.config/app", new MountOptions
        {
            Name = "user",
            Priority = 10
        });
        
        // Mount environment overrides
        vos.Mount("/config", "env://APP_CONFIG_", new MountOptions
        {
            Name = "environment",
            Priority = 20
        });
    }
    
    public async Task<T> GetConfig<T>(string path)
    {
        var configVob = vos[$"/config/{path}"];
        return await configVob.Get<T>();
    }
}
```

### Dynamic Configuration

```csharp
public class DynamicConfig
{
    private readonly IVob configRoot;
    
    public DynamicConfig(IVos vos)
    {
        configRoot = vos["/config"];
    }
    
    public async Task<T> GetSetting<T>(string key, T defaultValue = default)
    {
        var vob = configRoot[key];
        if (await vob.Exists())
        {
            return await vob.Get<T>();
        }
        return defaultValue;
    }
    
    public async Task SetSetting<T>(string key, T value)
    {
        await configRoot[key].Set(value);
    }
    
    public async Task WatchSetting<T>(string key, Action<T> onChange)
    {
        var handle = configRoot[key].GetReadHandle<T>();
        handle.ValueChanged += (s, e) => onChange(e.NewValue);
    }
}
```

## Archive Operations

### Working with ZIP Files

```csharp
// Mount and read ZIP archive
vos.Mount("/archive", "zip:///data/backup.zip");

// List archive contents
var archiveVob = vos["/archive"];
await foreach (var entry in archiveVob.EnumerateRecursive())
{
    Console.WriteLine($"Archive contains: {entry.Path}");
}

// Extract file from archive
var fileInZip = vos["/archive/data/important.json"];
var data = await fileInZip.Get<ImportantData>();

// Create new ZIP archive
var newArchive = vos["/output/archive.zip"];
await newArchive["file1.txt"].Set("Content 1");
await newArchive["folder/file2.json"].Set(new { data = "value" });
```

### Nested Archives

```csharp
// Mount ZIP within ZIP
vos.Mount("/outer", "zip:///archives/outer.zip");
vos.Mount("/outer/inner.zip", "zip:///outer/inner.zip");

// Access nested content
var deepFile = vos["/outer/inner.zip/data/file.txt"];
var content = await deepFile.Get<string>();
```

## Database Integration

### Redis Example

```csharp
public class RedisCache
{
    private readonly IVob cacheRoot;
    
    public RedisCache(IVos vos)
    {
        // Mount Redis
        vos.Mount("/cache", "redis://localhost:6379/0", new MountOptions
        {
            Metadata = new Dictionary<string, object>
            {
                ["KeyPrefix"] = "app:cache:"
            }
        });
        
        cacheRoot = vos["/cache"];
    }
    
    public async Task<T> GetOrSet<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
    {
        var vob = cacheRoot[key];
        
        if (await vob.Exists())
        {
            return await vob.Get<T>();
        }
        
        var value = await factory();
        await vob.Set(value, new PersistenceOptions { Expiry = expiry });
        return value;
    }
}
```

### Document Database

```csharp
public class UserRepository
{
    private readonly IVob usersRoot;
    
    public UserRepository(IVos vos)
    {
        // Mount MongoDB collection
        vos.Mount("/db/users", "mongodb://localhost/myapp/users");
        usersRoot = vos["/db/users"];
    }
    
    public async Task<User> GetUser(string id)
    {
        return await usersRoot[id].Get<User>();
    }
    
    public async Task<IEnumerable<User>> GetUsersByRole(string role)
    {
        var query = new VobReference("/db/users",
            filters: new[] { ("role", role) });
        
        var results = new List<User>();
        await foreach (var userVob in vos[query].Children.Values)
        {
            results.Add(await userVob.Get<User>());
        }
        return results;
    }
}
```

## Asset Management

### Game Assets Example

```csharp
public class AssetManager
{
    private readonly IVos vos;
    
    public AssetManager(IVos vos)
    {
        this.vos = vos;
        SetupAssetMounts();
    }
    
    private void SetupAssetMounts()
    {
        // Base game assets
        vos.Mount("/assets", "file:///game/assets", new MountOptions
        {
            Name = "base",
            Priority = 0
        });
        
        // DLC assets (higher priority)
        vos.Mount("/assets", "zip:///game/dlc/expansion1.pak", new MountOptions
        {
            Name = "expansion1",
            Priority = 10
        });
        
        // User mods (highest priority)
        vos.Mount("/assets", "file:///user/mods", new MountOptions
        {
            Name = "mods",
            Priority = 20
        });
    }
    
    public async Task<T> LoadAsset<T>(string path) where T : class
    {
        var assetVob = vos[$"/assets/{path}"];
        return await assetVob.Get<T>();
    }
    
    public async Task<Texture> LoadTexture(string path)
    {
        var textureData = await LoadAsset<byte[]>($"textures/{path}");
        return Texture.FromBytes(textureData);
    }
}
```

### Template System

```csharp
public class TemplateEngine
{
    private readonly IVob templateRoot;
    
    public TemplateEngine(IVos vos)
    {
        templateRoot = vos["/templates"];
    }
    
    public async Task<string> RenderTemplate(string templateName, object model)
    {
        // Load template
        var template = await templateRoot[$"{templateName}.hbs"].Get<string>();
        
        // Load partials
        var partialsVob = templateRoot["partials"];
        var partials = new Dictionary<string, string>();
        
        foreach (var partial in partialsVob.Children)
        {
            partials[partial.Key] = await partial.Value.Get<string>();
        }
        
        // Render with Handlebars or similar
        return HandlebarsDotNet.Handlebars.Compile(template)(model);
    }
}
```

## Plugin System

### Dynamic Plugin Loading

```csharp
public class PluginManager
{
    private readonly IVos vos;
    private readonly IVob pluginRoot;
    
    public PluginManager(IVos vos)
    {
        this.vos = vos;
        pluginRoot = vos["/plugins"];
    }
    
    public async Task LoadPlugins()
    {
        foreach (var pluginDir in pluginRoot.Children.Values)
        {
            var manifestVob = pluginDir["manifest.json"];
            if (await manifestVob.Exists())
            {
                var manifest = await manifestVob.Get<PluginManifest>();
                await LoadPlugin(pluginDir, manifest);
            }
        }
    }
    
    private async Task LoadPlugin(IVob pluginVob, PluginManifest manifest)
    {
        // Mount plugin resources
        vos.Mount($"/plugins/{manifest.Id}/data", 
            pluginVob["data"].Reference.Path);
        
        // Load plugin assembly
        var assemblyData = await pluginVob[$"{manifest.AssemblyName}.dll"]
            .Get<byte[]>();
        
        var assembly = Assembly.Load(assemblyData);
        // Initialize plugin...
    }
}
```

## Monitoring and Logging

### VOS Event Monitoring

```csharp
public class VosMonitor
{
    public void MonitorVos(IVos vos)
    {
        // Monitor mount events
        vos.MountAdded += (s, e) =>
        {
            Console.WriteLine($"Mount added: {e.Mount.Name} at {e.MountPoint.Path}");
        };
        
        vos.MountRemoved += (s, e) =>
        {
            Console.WriteLine($"Mount removed: {e.Mount.Name}");
        };
    }
    
    public void MonitorVob(IVob vob)
    {
        // Monitor data changes
        var handle = vob.GetReadHandle<object>();
        handle.ValueChanged += (s, e) =>
        {
            Console.WriteLine($"Value changed at {vob.Path}");
        };
        
        // Monitor children
        vob.ChildAdded += (s, child) =>
        {
            Console.WriteLine($"Child added: {child.Path}");
        };
        
        vob.ChildRemoved += (s, child) =>
        {
            Console.WriteLine($"Child removed: {child.Path}");
        };
    }
}
```

### Audit Trail

```csharp
public class AuditingVos
{
    private readonly IVob auditRoot;
    
    public AuditingVos(IVos vos)
    {
        auditRoot = vos["/audit"];
    }
    
    public async Task<T> AuditedOperation<T>(
        string operation, 
        string path, 
        Func<Task<T>> action)
    {
        var audit = new AuditEntry
        {
            Operation = operation,
            Path = path,
            Timestamp = DateTime.UtcNow,
            User = Environment.UserName
        };
        
        try
        {
            var result = await action();
            audit.Success = true;
            audit.Result = JsonSerializer.Serialize(result);
            return result;
        }
        catch (Exception ex)
        {
            audit.Success = false;
            audit.Error = ex.Message;
            throw;
        }
        finally
        {
            var auditPath = $"{DateTime.UtcNow:yyyy-MM-dd}/{Guid.NewGuid()}.json";
            await auditRoot[auditPath].Set(audit);
        }
    }
}
```

## Performance Patterns

### Bulk Operations

```csharp
public class BulkProcessor
{
    public async Task ProcessManyFiles(IVob root, Func<IVob, Task> processor)
    {
        const int batchSize = 100;
        var tasks = new List<Task>(batchSize);
        
        foreach (var child in root.EnumerateRecursive())
        {
            tasks.Add(processor(child));
            
            if (tasks.Count >= batchSize)
            {
                await Task.WhenAll(tasks);
                tasks.Clear();
            }
        }
        
        if (tasks.Any())
        {
            await Task.WhenAll(tasks);
        }
    }
}
```

### Caching Strategy

```csharp
public class CachedDataAccess
{
    private readonly IVob dataRoot;
    private readonly MemoryCache cache;
    
    public async Task<T> GetCached<T>(string key) where T : class
    {
        // Check memory cache first
        if (cache.TryGetValue<T>(key, out var cached))
        {
            return cached;
        }
        
        // Check VOS cache layer
        var cacheVob = dataRoot[$".cache/{key}"];
        if (await cacheVob.Exists())
        {
            var cachedData = await cacheVob.Get<CacheEntry<T>>();
            if (cachedData.Expiry > DateTime.UtcNow)
            {
                cache.Set(key, cachedData.Value);
                return cachedData.Value;
            }
        }
        
        // Load from source
        var sourceVob = dataRoot[key];
        var value = await sourceVob.Get<T>();
        
        // Update caches
        await cacheVob.Set(new CacheEntry<T>
        {
            Value = value,
            Expiry = DateTime.UtcNow.AddMinutes(5)
        });
        
        cache.Set(key, value, TimeSpan.FromMinutes(5));
        return value;
    }
}
```

## Testing with VOS

### Unit Testing

```csharp
public class VosTestExample
{
    [Test]
    public async Task TestDataPersistence()
    {
        // Create in-memory VOS
        var services = new ServiceCollection();
        services.AddVos();
        services.AddVosMemory(); // In-memory persister
        
        var provider = services.BuildServiceProvider();
        var vos = provider.GetRequiredService<IVos>();
        
        // Test operations
        var testVob = vos["/test/data"];
        await testVob.Set(new { value = 42 });
        
        var result = await testVob.Get<dynamic>();
        Assert.AreEqual(42, (int)result.value);
    }
}
```

### Mocking VOS

```csharp
public class MockVosSetup
{
    public static IVos CreateMockVos()
    {
        var mockVos = new Mock<IVos>();
        var mockRoot = new Mock<IRootVob>();
        var mockVob = new Mock<IVob>();
        
        // Setup mock behavior
        mockVos.Setup(v => v.Get(It.IsAny<string>()))
            .Returns(mockRoot.Object);
        
        mockRoot.Setup(r => r[It.IsAny<string>"])
            .Returns(mockVob.Object);
        
        mockVob.Setup(v => v.GetReadHandle<It.IsAnyType>())
            .Returns(() => CreateMockHandle<object>());
        
        return mockVos.Object;
    }
}
```

## Next Steps

- Review the [API Reference](./vos-api-reference.md) for detailed API information
- Learn about [Architecture](./vos-architecture.md) for system design
- Explore [Persistence](./vos-persistence.md) for storage options