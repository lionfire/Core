# Getting Started: Base Layer Utilities

## Overview

This guide introduces **LionFire.Base** - the foundation layer that augments .NET's BCL with useful utilities and extension methods. These utilities have zero external dependencies and are used throughout the LionFire ecosystem.

**What You'll Learn**:
- Essential collection extensions
- String manipulation utilities
- Type and reflection helpers
- Common patterns and best practices

**Prerequisites**: .NET 9.0+ SDK

---

## Setup

### 1. Create a New Console Project

```bash
dotnet new console -n LionFireBasics
cd LionFireBasics
```

### 2. Add LionFire.Base Package

```bash
dotnet add package LionFire.Base
```

### 3. Open in Your Editor

```bash
code .  # VS Code
# or open in Visual Studio, Rider, etc.
```

---

## Collection Extensions

LionFire.Base provides powerful collection utilities that make working with lists, dictionaries, and enumerables easier.

### AddRange for Multiple Items

Add multiple items to a collection at once:

```csharp
using LionFire;

var names = new List<string>();

// Add multiple items
names.AddRange("Alice", "Bob", "Charlie");

Console.WriteLine(string.Join(", ", names));
// Output: Alice, Bob, Charlie
```

### ForEach Extension

Perform an action on each element:

```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5 };

// Traditional way
foreach (var num in numbers)
{
    Console.WriteLine(num * 2);
}

// LionFire way
numbers.ForEach(num => Console.WriteLine(num * 2));
```

### TryGetValue with Default

Get a dictionary value or return a default:

```csharp
var settings = new Dictionary<string, string>
{
    ["theme"] = "dark",
    ["language"] = "en"
};

// Traditional way
string font = settings.ContainsKey("font") ? settings["font"] : "Arial";

// LionFire way
string font = settings.TryGetValue("font", "Arial");

Console.WriteLine(font);  // Output: Arial
```

### IsNullOrEmpty for Collections

Check if a collection is null or empty:

```csharp
using LionFire;

List<string>? items = null;

if (items.IsNullOrEmpty())
{
    Console.WriteLine("Collection is null or empty");
}

items = new List<string>();
if (items.IsNullOrEmpty())
{
    Console.WriteLine("Collection is empty");
}

items.Add("Hello");
if (!items.IsNullOrEmpty())
{
    Console.WriteLine("Collection has items");
}
```

---

## String Extensions

Powerful string manipulation utilities.

### ToNullIfEmpty

Convert empty strings to null:

```csharp
using LionFire;

string? input = "";
string? result = input.ToNullIfEmpty();

Console.WriteLine(result == null);  // Output: True

// Useful in data processing
string? GetValidName(string input) =>
    input.Trim().ToNullIfEmpty() ?? "Unknown";

Console.WriteLine(GetValidName(""));      // Output: Unknown
Console.WriteLine(GetValidName("  "));    // Output: Unknown
Console.WriteLine(GetValidName("Alice")); // Output: Alice
```

### Before / After

Extract substrings before or after a delimiter:

```csharp
using LionFire;

string email = "user@example.com";

string username = email.Before("@");
string domain = email.After("@");

Console.WriteLine($"Username: {username}");  // Output: Username: user
Console.WriteLine($"Domain: {domain}");      // Output: Domain: example.com

// Works with paths too
string path = "/home/user/documents/file.txt";
string filename = path.After("/", last: true);

Console.WriteLine($"Filename: {filename}");  // Output: Filename: file.txt
```

### Contains (Case-Insensitive)

Case-insensitive string searching:

```csharp
using LionFire;

string text = "Hello World";

// Traditional way
bool contains1 = text.ToLower().Contains("hello".ToLower());

// LionFire way
bool contains2 = text.Contains("hello", StringComparison.OrdinalIgnoreCase);

Console.WriteLine(contains2);  // Output: True
```

### TrimOrNull

Trim and return null if empty:

```csharp
using LionFire;

string? ProcessInput(string? input) => input.TrimOrNull();

Console.WriteLine(ProcessInput("  Hello  "));  // Output: Hello
Console.WriteLine(ProcessInput("  ") == null); // Output: True
Console.WriteLine(ProcessInput(null) == null); // Output: True
```

---

## Type Extensions

Utilities for working with types and reflection.

### GetDefaultValue

Get the default value for any type:

```csharp
using LionFire;

var intDefault = typeof(int).GetDefaultValue();
var stringDefault = typeof(string).GetDefaultValue();
var listDefault = typeof(List<int>).GetDefaultValue();

Console.WriteLine(intDefault);     // Output: 0
Console.WriteLine(stringDefault);  // Output: (null)
Console.WriteLine(listDefault);    // Output: (null)
```

### IsNullable

Check if a type is nullable:

```csharp
using LionFire;

Console.WriteLine(typeof(int?).IsNullable());        // Output: True
Console.WriteLine(typeof(int).IsNullable());         // Output: False
Console.WriteLine(typeof(string).IsNullable());      // Output: True (reference type)
Console.WriteLine(typeof(List<int>).IsNullable());   // Output: True (reference type)
```

### GetFriendlyName

Get a human-readable type name:

```csharp
using LionFire;

Console.WriteLine(typeof(int).GetFriendlyName());                    // Output: Int32
Console.WriteLine(typeof(List<string>).GetFriendlyName());           // Output: List<String>
Console.WriteLine(typeof(Dictionary<int, string>).GetFriendlyName()); // Output: Dictionary<Int32, String>
```

---

## DateTime Extensions

Convenient DateTime utilities.

### Epoch Time

Convert between DateTime and Unix epoch:

```csharp
using LionFire;

// Current epoch time
long now = DateTime.UtcNow.ToUnixTimeMilliseconds();
Console.WriteLine($"Epoch: {now}");

// Convert back
DateTime dt = DateTimeOffset.FromUnixTimeMilliseconds(now).DateTime;
Console.WriteLine($"DateTime: {dt}");
```

### Date Comparisons

Compare dates ignoring time:

```csharp
using LionFire;

DateTime date1 = new DateTime(2025, 1, 15, 10, 30, 0);
DateTime date2 = new DateTime(2025, 1, 15, 14, 45, 0);

bool sameDate = date1.Date == date2.Date;
Console.WriteLine($"Same date: {sameDate}");  // Output: Same date: True
```

---

## Practical Example: Configuration Parser

Let's build a simple configuration file parser using LionFire.Base utilities:

```csharp
using LionFire;

class ConfigParser
{
    public Dictionary<string, string> Parse(string configText)
    {
        var config = new Dictionary<string, string>();

        if (configText.IsNullOrWhiteSpace())
            return config;

        var lines = configText.Split('\n');

        lines.ForEach(line =>
        {
            // Skip comments and empty lines
            var trimmed = line.TrimOrNull();
            if (trimmed == null || trimmed.StartsWith("#"))
                return;

            // Parse key=value
            if (trimmed.Contains("="))
            {
                var key = trimmed.Before("=").Trim();
                var value = trimmed.After("=").Trim();

                if (!key.IsNullOrEmpty())
                {
                    config[key] = value;
                }
            }
        });

        return config;
    }

    public string GetValue(Dictionary<string, string> config, string key, string defaultValue = "")
    {
        return config.TryGetValue(key, defaultValue);
    }
}

// Usage
var parser = new ConfigParser();
var configText = @"
# Application settings
app.name=MyApp
app.version=1.0.0
app.debug=true

# Database
db.host=localhost
db.port=5432
";

var config = parser.Parse(configText);

Console.WriteLine($"App Name: {parser.GetValue(config, "app.name")}");
Console.WriteLine($"Version: {parser.GetValue(config, "app.version")}");
Console.WriteLine($"Debug: {parser.GetValue(config, "app.debug")}");
Console.WriteLine($"DB Host: {parser.GetValue(config, "db.host")}");
Console.WriteLine($"Timeout: {parser.GetValue(config, "db.timeout", "30")}");

// Output:
// App Name: MyApp
// Version: 1.0.0
// Debug: true
// DB Host: localhost
// Timeout: 30
```

---

## Best Practices

### 1. Use Extension Methods for Readability

```csharp
// ❌ Avoid - Verbose and hard to read
if (myList != null && myList.Count > 0)
{
    foreach (var item in myList)
    {
        Console.WriteLine(item);
    }
}

// ✅ Good - Concise and clear
if (!myList.IsNullOrEmpty())
{
    myList.ForEach(item => Console.WriteLine(item));
}
```

### 2. Chain Extension Methods

```csharp
// ✅ Good - Readable pipeline
string processedInput = userInput
    .TrimOrNull()?
    .ToLowerInvariant()
    .After(":")
    ?? "default";
```

### 3. Use Null-Coalescing with Extensions

```csharp
// ✅ Good - Safe null handling
string value = config.TryGetValue("key") ?? "default";
```

### 4. Prefer Extension Methods Over Utility Classes

```csharp
// ❌ Avoid - Static utility classes
StringUtils.IsEmpty(myString);

// ✅ Good - Extension methods
myString.IsNullOrEmpty();
```

---

## Common Patterns

### Pattern 1: Safe Collection Access

```csharp
public class DataService
{
    private List<string>? _cache;

    public IEnumerable<string> GetItems()
    {
        // Safe - returns empty if null
        return _cache ?? Enumerable.Empty<string>();

        // Or using LionFire
        return _cache.IsNullOrEmpty()
            ? Enumerable.Empty<string>()
            : _cache;
    }
}
```

### Pattern 2: Configuration with Defaults

```csharp
public class AppConfig
{
    private Dictionary<string, string> _settings = new();

    public string GetSetting(string key, string defaultValue = "")
    {
        return _settings.TryGetValue(key, defaultValue);
    }

    public int GetIntSetting(string key, int defaultValue = 0)
    {
        var value = _settings.TryGetValue(key);
        return int.TryParse(value, out int result) ? result : defaultValue;
    }
}
```

### Pattern 3: String Processing Pipeline

```csharp
public class UrlParser
{
    public (string protocol, string domain, string path) Parse(string url)
    {
        var protocol = url.Before("://") ?? "https";
        var remainder = url.After("://");
        var domain = remainder.Before("/");
        var path = remainder.After("/") ?? "";

        return (protocol, domain, path);
    }
}

// Usage
var parser = new UrlParser();
var (protocol, domain, path) = parser.Parse("https://example.com/api/users");

Console.WriteLine($"Protocol: {protocol}");  // https
Console.WriteLine($"Domain: {domain}");      // example.com
Console.WriteLine($"Path: {path}");          // api/users
```

---

## Summary

**LionFire.Base** provides essential utilities that make C# development more productive:

**Key Extensions**:
- **Collections**: `IsNullOrEmpty()`, `ForEach()`, `AddRange()`, `TryGetValue()`
- **Strings**: `ToNullIfEmpty()`, `Before()`, `After()`, `TrimOrNull()`
- **Types**: `GetDefaultValue()`, `IsNullable()`, `GetFriendlyName()`
- **DateTime**: Epoch conversions, date comparisons

**Benefits**:
- Reduces boilerplate code
- More readable, chainable operations
- Null-safe by design
- Zero dependencies

**Next Steps**:
1. Explore [02-async-data.md](02-async-data.md) to learn about async data access patterns
2. Review the [LionFire.Base source](../../src/LionFire.Base/) for more utilities
3. Check out [LionFire.Structures](../../src/LionFire.Structures/) for advanced data structures

---

## Exercise

Try building a simple log file analyzer that:
1. Reads a log file line by line
2. Parses each line to extract timestamp, level, and message
3. Groups by log level
4. Counts errors and warnings

Use LionFire.Base extensions for string parsing and collection manipulation!

**Hint**: Use `Before()`, `After()`, `IsNullOrEmpty()`, and `ForEach()`.
