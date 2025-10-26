# LionFire.Serialization.Hjson

## Overview

**LionFire.Serialization.Hjson** provides HJSON (Human JSON) serialization support for LionFire's persistence framework. HJSON is a user-friendly JSON format that supports comments, multi-line strings, and relaxed syntax - ideal for configuration files and human-editable data.

**Layer**: Toolkit (Serialization Provider)
**Target**: .NET 9.0
**Root Namespace**: `LionFire.Serialization.Hjson_`

## Key Dependencies

### NuGet Packages
- **Hjson** - HJSON parser and serializer

### LionFire Dependencies
- **LionFire.Core** - Core metadata and DI
- **LionFire.Data.Async.Abstractions** - Async data patterns
- **LionFire.Hosting** - Hosting extensions
- **LionFire.Persistence.Abstractions** - Persistence abstractions
- **LionFire.Persistence** - Persistence framework
- **LionFire.Resolves** (Data.Async) - Data resolution implementations

## What is HJSON?

HJSON (Human JSON) is a configuration file format designed to be easy for humans to read and write. It extends JSON with:

### HJSON Features

**Comments:**
```hjson
// Single-line comments
/* Multi-line
   comments */
{
  "key": "value" // Inline comments
}
```

**Unquoted Keys and Strings:**
```hjson
{
  key: value
  name: John Doe
  path: C:\Users\Name
}
```

**Multiline Strings:**
```hjson
{
  description:
    '''
    This is a multiline
    string that preserves
    line breaks.
    '''
}
```

**Trailing Commas:**
```hjson
{
  item1: value1,
  item2: value2,  // Trailing comma is valid
}
```

**Root Braces Optional:**
```hjson
// No root braces needed (LionFire convention)
key1: value1
key2: value2
```

## Core Components

### 1. HjsonSerializer

**File**: `Serialization/Hjson/HjsonSerializer.cs`

Main serializer implementation integrating HJSON with LionFire's persistence framework.

**Features:**
```csharp
public class HjsonSerializer : SerializerBase<HjsonSerializer>
{
    public override SerializationFlags SupportedCapabilities =>
        SerializationFlags.Text
        | SerializationFlags.HumanReadable
        | SerializationFlags.Deserialize
        | SerializationFlags.Serialize;

    public override SerializationFormat DefaultFormat { get; }
    // Format: "hjson", MIME: "application/hjson"
}
```

**Capabilities:**
- Text-based serialization
- Human-readable format
- Bidirectional (serialize and deserialize)
- Supports JSON as input (auto-converts to HJSON)

### 2. HjsonSerializerSettings

**File**: `Serialization/Hjson/HjsonSerializerSettings.cs`

Configuration options for HJSON serialization.

**Key Settings:**
```csharp
public class HjsonSerializerSettings
{
    public HjsonOptions Options { get; set; }
}

public class HjsonOptions
{
    public bool EmitRootBraces { get; set; } = false; // LionFire convention
    public bool KeepWsc { get; set; }                  // Keep whitespace/comments
}
```

**Context-Specific Settings:**

LionFire supports different serialization contexts:

```csharp
public enum LionSerializeContext
{
    Persistence,  // Saving to disk/database
    Network,      // Sending over network
    Copy          // Object cloning
}
```

**Default Configuration:**

```csharp
// Persistence context (default)
var persistenceSettings = new HjsonSerializerSettings
{
    Options = new()
    {
        EmitRootBraces = false,  // Cleaner files
        KeepWsc = true            // Preserve comments/whitespace
    }
};

// Network context
var networkSettings = new HjsonSerializerSettings
{
    Options = new()
    {
        EmitRootBraces = false,
        KeepWsc = false  // Compact for network transmission
    }
};
```

### 3. Dependency Injection Extensions

**File**: `DependencyInjection/AppHostHjsonExtensions.cs`

Registration extensions for HJSON serialization.

**Usage:**
```csharp
// With IHostBuilder
builder.AddHjson();

// With IServiceCollection
services.AddHjson();

// With IAppHost (LionFire)
app.AddHjson();
```

**What Gets Registered:**

```csharp
public static IServiceCollection AddHjson(this IServiceCollection services)
{
    services
        // Register as enumerable strategy (allows multiple serializers)
        .TryAddEnumerableSingleton<ISerializationStrategy, HjsonSerializer>()

        // Register specific HjsonSerializer
        .AddSingleton<HjsonSerializer>()

        // Configure for persistence context
        .Configure<HjsonSerializerSettings>(
            LionSerializeContext.Persistence.ToString(),
            c => c.SetDefaults(LionSerializeContext.Persistence))

        // Configure for network context
        .Configure<HjsonSerializerSettings>(
            LionSerializeContext.Network.ToString(),
            c => c.SetDefaults(LionSerializeContext.Network));

    return services;
}
```

### 4. Contract Resolution

**File**: `Serialization/Hjson/LionFireHjsonContractResolver.cs`

Custom contract resolver for HJSON serialization (if needed for advanced scenarios).

**File**: `Serialization/Hjson/SerializationReflectionUtils.cs`

Reflection utilities for serialization.

## Serialization Operations

### Serialize to HJSON

```csharp
var serializer = serviceProvider.GetRequiredService<HjsonSerializer>();

var myObject = new MyConfig
{
    AppName = "My Application",
    Port = 8080,
    Features = new[] { "logging", "auth" }
};

// Serialize to HJSON string
var (hjsonString, result) = serializer.ToString(
    JsonConvert.SerializeObject(myObject),
    operation: null,
    context: null
);

if (result == SerializationResult.Success)
{
    Console.WriteLine(hjsonString);
    // Output:
    // AppName: My Application
    // Port: 8080
    // Features:
    // [
    //   logging
    //   auth
    // ]
}
```

### Deserialize from HJSON

```csharp
var hjsonString = @"
// Application configuration
AppName: My Application
Port: 8080
Features:
[
  logging
  auth
]
";

var deserializeResult = serializer.ToObject<string>(hjsonString);

if (deserializeResult.IsSuccess)
{
    // Result is JSON string
    var jsonString = deserializeResult.Value;
    var config = JsonConvert.DeserializeObject<MyConfig>(jsonString);

    Console.WriteLine($"App: {config.AppName}, Port: {config.Port}");
}
```

## Integration with Persistence

### File Reading/Writing

The serializer integrates with LionFire's persistence framework:

```csharp
// Extension method for read handles
public static class HjsonFileReadHandleFactoryExtensions
{
    // Creates read handles for .hjson files
}
```

**Example with Handles:**

```csharp
// Assuming LionFire.Persistence.Handles is configured
var handle = H<MyConfig>.FromPath("/config/app.hjson");

// Read HJSON file
var config = await handle.Get();

// Modify
config.Port = 9090;

// Write back as HJSON (preserves comments if KeepWsc = true)
await handle.Set(config);
```

## HJSON File Conventions

### LionFire HJSON Conventions

**Root Braces**: Always omit root braces for cleaner files

**Correct:**
```hjson
// Configuration
migrationLevel: 1
features:
{
  logging: true
  auth: true
}
```

**Incorrect:**
```hjson
{
  // Configuration
  migrationLevel: 1
}
```

**See**: `/st/dev/.agent-os/standards/code-style/hjson.md` (if available in your environment)

### File Extensions

- `.hjson` - HJSON files
- `.json` - Can also read JSON files

### Comment Preservation

When `KeepWsc = true` (Persistence context), comments and formatting are preserved:

**Before:**
```hjson
// User preferences
theme: dark  // Default theme
fontSize: 14
```

**After modification and save:**
```hjson
// User preferences
theme: light  // Default theme (comment preserved!)
fontSize: 14
```

## Common Usage Patterns

### Pattern 1: Configuration Files

```csharp
// Startup.cs or Program.cs
public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .AddHjson() // Enable HJSON serialization
            .ConfigureServices((context, services) =>
            {
                // HJSON files can now be used for persistence
            });

        builder.Build().Run();
    }
}
```

**appsettings.hjson:**
```hjson
// Application settings
Logging:
{
  LogLevel:
  {
    Default: Information
    Microsoft: Warning
  }
}

ConnectionStrings:
{
  DefaultConnection: Server=localhost;Database=MyDb
}

// Feature flags
Features:
{
  EnableAuth: true
  EnableCaching: false  // Disabled for debugging
}
```

### Pattern 2: User Preferences

```csharp
public class UserPreferencesService
{
    private readonly IServiceProvider services;

    public async Task<UserPreferences> LoadPreferences(string userId)
    {
        // Uses HJSON serializer automatically
        var handle = services.GetRequiredService<IHandleFactory>()
            .CreateHandle<UserPreferences>($"/users/{userId}/preferences.hjson");

        return await handle.GetIfNeeded();
    }

    public async Task SavePreferences(string userId, UserPreferences prefs)
    {
        var handle = services.GetRequiredService<IHandleFactory>()
            .CreateHandle<UserPreferences>($"/users/{userId}/preferences.hjson");

        await handle.Set(prefs);
    }
}
```

**preferences.hjson:**
```hjson
// User: john.doe
theme: dark
language: en-US
notifications:
{
  email: true
  push: false  // Disabled by user
  sms: false
}
favoriteColors:
[
  blue
  green
  purple
]
```

### Pattern 3: Type Registry

```csharp
public class TypeRegistry
{
    public Dictionary<string, Type> Types { get; set; }

    public static async Task<TypeRegistry> Load(string path)
    {
        var handle = H<TypeRegistry>.FromPath(path);
        return await handle.GetOrDefault(() => new TypeRegistry());
    }
}
```

**types.hjson:**
```hjson
// Type mappings for plugin system
Types:
{
  logger: MyApp.Logging.CustomLogger
  cache: MyApp.Caching.RedisCache
  auth: MyApp.Auth.JwtAuthProvider  // Production auth
}
```

### Pattern 4: Context-Specific Serialization

```csharp
var serializer = services.GetRequiredService<HjsonSerializer>();

// Persistence: Keep comments and whitespace
var persistenceOp = new PersistenceOperation
{
    SerializeContext = LionSerializeContext.Persistence
};

var settings = serializer.SettingsForContext(persistenceOp);
// settings.Options.KeepWsc == true

// Network: Compact format
var networkOp = new PersistenceOperation
{
    SerializeContext = LionSerializeContext.Network
};

var networkSettings = serializer.SettingsForContext(networkOp);
// networkSettings.Options.KeepWsc == false
```

## Advanced Scenarios

### Custom Serialization Settings

```csharp
services.Configure<HjsonSerializerSettings>(
    LionSerializeContext.Persistence.ToString(),
    settings =>
    {
        settings.Options = new()
        {
            EmitRootBraces = false,
            KeepWsc = true,
            // Additional custom options
        };
    }
);
```

### Multiple Serializers

LionFire's serialization framework supports multiple serializers:

```csharp
services
    .AddHjson()              // HJSON serializer
    .AddNewtonsoftJson()     // JSON serializer
    .AddYaml();              // YAML serializer (if available)

// Serializer chosen based on file extension
var hjsonHandle = H<Config>.FromPath("/config/app.hjson");   // Uses HJSON
var jsonHandle = H<Config>.FromPath("/config/app.json");     // Uses JSON
```

### SerializationFormat

```csharp
public SerializationFormat DefaultFormat => new SerializationFormat(
    "hjson",                    // Extension
    "Hjson",                    // Display name
    "application/hjson"         // MIME type
)
{
    Description = "Human JSON"
};
```

## Reactive HJSON (LionFire.Reactive.Framework)

For reactive file watching and automatic reloading, see **LionFire.Reactive.Framework**:

**Components** (in LionFire.Reactive.Framework):
- `HjsonFsDirectoryReaderRx<TKey, TValue>` - Reactive directory reader
- `HjsonFsDirectoryWriterRx<TKey, TValue>` - Reactive directory writer
- `HjsonFsDictionaryProvider` - Observable dictionary provider
- `HjsonFilesystemAssets` - Asset management with file watching

**Example:**
```csharp
// Automatically reloads when file changes
var reader = new HjsonFsDirectoryReaderRx<string, MyConfig>(directoryPath);

reader.Observable.Subscribe(change =>
{
    Console.WriteLine($"Config changed: {change.Key}");
    var newConfig = change.Value;
    // React to configuration changes
});
```

## Directory Structure

```
src/LionFire.Serialization.Hjson/
├── DependencyInjection/
│   └── AppHostHjsonExtensions.cs      # DI registration extensions
└── Serialization/
    └── Hjson/
        ├── HjsonFileReadHandleFactoryExtensions.cs  # File handle extensions
        ├── HjsonSerialization.cs                    # Serialization utilities
        ├── HjsonSerializer.cs                       # Main serializer
        ├── HjsonSerializerSettings.cs               # Configuration
        ├── LionFireHjsonContractResolver.cs         # Contract resolution
        └── SerializationReflectionUtils.cs          # Reflection utilities
```

## Design Philosophy

**Human-Friendly Configuration:**
- HJSON is designed for humans to read and edit
- Comments supported for documentation
- Relaxed syntax reduces errors

**Convention over Configuration:**
- Root braces omitted by default (LionFire convention)
- Sensible defaults for persistence vs network contexts

**Seamless Integration:**
- Integrates with LionFire persistence framework
- Works alongside other serializers (JSON, YAML)
- Automatic format detection by file extension

## Performance Considerations

### When to Use HJSON

**Best For:**
- Configuration files
- User-editable settings
- Development/debugging data
- Documentation-heavy data files

**Avoid For:**
- High-throughput APIs (use JSON)
- Binary-heavy data (use binary formats)
- Performance-critical serialization (use Protocol Buffers, MessagePack)

### Optimization Tips

```csharp
// Network context: Disable comment preservation for smaller payloads
var networkSettings = new HjsonSerializerSettings
{
    Options = new() { KeepWsc = false }
};

// Persistence context: Keep comments for maintainability
var persistenceSettings = new HjsonSerializerSettings
{
    Options = new() { KeepWsc = true }
};
```

## Testing Considerations

### Testing Serialization

```csharp
[Fact]
public void HjsonSerializer_Roundtrip_PreservesData()
{
    var serializer = new HjsonSerializer(optionsMonitor);

    var original = new MyConfig { Name = "Test", Value = 42 };
    var json = JsonConvert.SerializeObject(original);

    var (hjson, serResult) = serializer.ToString(json);
    Assert.Equal(SerializationResult.Success, serResult);

    var deserResult = serializer.ToObject<string>(hjson);
    Assert.True(deserResult.IsSuccess);

    var roundtrip = JsonConvert.DeserializeObject<MyConfig>(deserResult.Value);
    Assert.Equal(original.Name, roundtrip.Name);
    Assert.Equal(original.Value, roundtrip.Value);
}
```

### Testing Comment Preservation

```hjson
// Original file with comments
name: Test App  // Application name
value: 42
```

After modification and save, comments should be preserved when `KeepWsc = true`.

## Related Projects

- **LionFire.Serialization** - Base serialization abstractions
- **LionFire.Serialization.Json.Newtonsoft** - JSON.NET serializer
- **LionFire.Reactive.Framework** - Reactive HJSON file watching (see `IO/Reactive/Hjson/`)
- **LionFire.Persistence** - Persistence framework using serializers
- **LionFire.Persistence.Filesystem** - Filesystem persistence backend

## Documentation

- **HJSON Specification**: https://hjson.github.io/
- **HJSON NuGet Package**: https://www.nuget.org/packages/Hjson/
- **LionFire HJSON Conventions**: `/st/dev/.agent-os/standards/code-style/hjson.md`

## Summary

**LionFire.Serialization.Hjson** provides human-friendly HJSON serialization:

- **HJSON Format**: Comments, unquoted strings, multiline strings, trailing commas
- **LionFire Conventions**: Root braces omitted by default
- **Context-Aware**: Different settings for persistence vs network
- **Seamless Integration**: Works with LionFire persistence framework
- **Comment Preservation**: Maintains comments when `KeepWsc = true`
- **Multiple Serializers**: Coexists with JSON, YAML, etc.

**Key Strengths:**
- Human-readable and editable
- Supports comments for documentation
- Relaxed syntax reduces configuration errors
- Preserves formatting and comments

**Use When:**
- Configuration files need human editing
- Documentation via comments is valuable
- Developer-friendly file formats preferred
- Working with settings/preferences

**Typical Use Cases:**
- Application configuration (`appsettings.hjson`)
- User preferences and settings
- Plugin/module registries
- Development/debugging data files
- Type mappings and registries

**Note**: For reactive file watching with automatic reload, use **LionFire.Reactive.Framework** which includes `HjsonFsDirectoryReaderRx` and related components.
