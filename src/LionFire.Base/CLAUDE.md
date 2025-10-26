# CLAUDE.md - LionFire.Base Library Guide

## Overview

**LionFire.Base** is the foundational library of the LionFire monorepo with **ZERO external dependencies** (except for .NET SDK assemblies). It provides essential extensions and utilities that augment the BCL (Base Class Library) with practical, reusable functionality.

**Key Philosophy**: Raise the lowest common denominator above BCL without external dependencies. These are utilities you'll use across all projects.

**Target Frameworks**: netstandard2.0, net9.0

---

## Quick Navigation by Category

1. [Extension Methods](#extension-methods) - Collections, Strings, Enums, Reflection, IO
2. [Concurrent Collections](#concurrent-collections) - Thread-safe data structures
3. [Exception Types](#exception-types) - Domain-specific exceptions
4. [Utility Classes](#utility-classes) - DateTime, LINQ, Async/Threading
5. [Patterns & Idioms](#patterns--idioms) - Common design approaches

---

## Extension Methods

### Collections: IEnumerable<T>
**Location**: `ExtensionMethods/System/Collections/Generic/IEnumerableExtensions.cs`

```csharp
// Wraps single item into IEnumerable<T>
var enumerable = item.Yield();  // return new[] { item }

// Perform action on each item
list.Apply(item => Console.WriteLine(item));

// Safe null handling
var items = enumerable.OrEmpty();  // null => Enumerable.Empty<T>()
```

**Key Methods**:
- `Yield<T>()` - Wraps object into single-item enumerable
- `Apply<T>()` - ForEach with inline action
- `OrEmpty<T>()` - Returns empty sequence if null

---

### Collections: IEnumerable (Non-Generic)
**Location**: `ExtensionMethods/System/Collections/Generic/IEnumerableLinqExtensions.cs`

```csharp
// Safe Any() that handles null
if (items.Any())  // null-safe, doesn't throw
{
    // do something
}

// Check minimum count without enumerating entire sequence
if (items.HasAtLeast(5))  // efficient: stops after 5 items
{
    // do something
}
```

**Key Methods**:
- `Any()` - Null-safe Any() without LINQ dependency
- `HasAtLeast(count)` - Efficient count check (doesn't enumerate all)

---

### Collections: IDictionary<TKey, TValue>
**Location**: `ExtensionMethods/System/Collections/Generic/IDictionaryExtensions.cs`

```csharp
// Safe add - returns false if key exists
bool added = dict.TryAdd(key, value);

// Get with default fallback (returns default if key not found)
var value = dict.TryGetValue(key);
var value = dict.TryGetValue(key, defaultValue: "fallback");

// Get or create on demand
var value = dict.GetOrAdd(key, key => new SomeObject());
var value = dict.GetOrAdd(key);  // where TValue : new()

// Add or update existing
dict.AddOrUpdate(key, newValue);

// Sync one dictionary to match another
dict.SetToMatch(otherDict);  // removes missing, adds new
dict.SetToMatch(keys, k => valueFactory(k), (k,v) => onRemove(k,v));
```

**Key Methods**:
- `TryAdd()` - Safe add returning success bool
- `TryGetValue()` - Returns default if key missing
- `GetOrAdd()` - Lazy initialization pattern
- `AddOrUpdate()` - Upsert operation
- `SetToMatch()` - Synchronize two dictionaries

---

### Collections: HashSet<T>
**Location**: `ExtensionMethods/System/Collections/Generic/IHashSetExtensions.cs`

```csharp
var set = new HashSet<string>();

// Safe add - doesn't throw if already exists
set.TryAdd("value");

// Bulk add from enumerable
set.AddRange(items);
```

**Key Methods**:
- `TryAdd()` - Adds without error if exists
- `AddRange()` - Bulk add from enumerable

---

### Collections: ICollection<T>
**Location**: `ExtensionMethods/System/Collections/Generic/ICollectionExtensions.cs`

```csharp
var list = new List<string>();

// Synchronize collection to match another
list.SetToMatch(otherList);
list.SetToMatch(otherList, filter: item => !item.IsDeleted);

// Add only if empty
list.AddIfEmpty(defaultItem);

// Replace all contents with new items
list.Set("item1", "item2", "item3");  // clears and adds
```

**Key Methods**:
- `SetToMatch()` - Sync collection with another (add missing, remove extra)
- `AddIfEmpty()` - Add item only if collection is empty
- `Set()` - Replace all contents with params array

---

### Concurrent Collections: ConcurrentDictionary<TKey, TValue>
**Location**: `ExtensionMethods/System/Collections/Concurrent/ConcurrentDictionaryExtensions.cs`

```csharp
var dict = new ConcurrentDictionary<string, int>();

// Add or throw if key exists
dict.AddOrThrow("key", 42);  // throws ArgumentException if key already present

// Try add returning success bool
bool added = dict.TryAdd("key", 42);

// Generate unique key with value
var (key, value) = dict.AddUnique(() => Guid.NewGuid(), new MyObject());

// Or separate value generator
var key = dict.AddUnique(() => Guid.NewGuid(), id => new MyObject { Id = id });
```

**Key Methods**:
- `AddOrThrow()` - Reference-equal check, throws if key exists
- `TryAdd()` - Returns bool instead of throwing
- `AddUnique()` - Generates unique key with infinite retry loop

---

### String Extensions
**Location**: `ExtensionMethods/System/StringExtensions.cs` and `Text/StringX.cs`

```csharp
// Text manipulation
string result = "helloWorld".InsertSpaceBeforeCaps();  // "hello World"

// Trimming
string result = "path.csproj".TryRemoveFromEnd(".csproj");  // "path"
string result = "LionFire".TryRemoveFromStart("Lion");  // "Fire"

// Prefix checking (PascalCase aware)
if ("LionFireCore".HasPrefix("Lion"))  // true - checks upper case after prefix
if ("Lionfire".HasPrefix("Lion"))  // false - next char not uppercase

// Null-safe check
bool empty = "  ".IsNullOrWhiteSpace();

// Modern transformations
string kebab = "PascalCaseName".ToKebabCase();  // "pascal-case-name"
string trimmed = "path/to/something.txt".TrimEnd(".txt");
```

**Key Methods**:
- `InsertSpaceBeforeCaps()` - "helloWorld" -> "hello World"
- `TryRemoveFromStart/End()` - Conditional removal
- `HasPrefix()` - PascalCase-aware prefix check
- `IsNullOrWhiteSpace()` - Wrapper for string.IsNullOrWhiteSpace
- `ToKebabCase()` - PascalCase to kebab-case conversion

---

### Enum Extensions
**Location**: `ExtensionMethods/System/EnumExtensions.cs`

```csharp
// Bitwise flag checking
bool hasFlag = myFlags.HasAnyFlag(targetFlags);  // Works with any enum

// Handles different underlying types (byte, short, int, long)
```

**Key Methods**:
- `HasAnyFlag()` - Type-safe bitwise AND check

---

### Boolean Extensions
**Location**: `ExtensionMethods/System/BooleanExtensions.cs`

```csharp
// Ternary logic - aggregate boolean collection
var bools = new[] { true, true, true };
bool? result = bools.ToTernary();  // true

var mixed = new[] { true, false, true };
bool? result = mixed.ToTernary();  // null (inconsistent)

var empty = new bool[] { };
bool? result = empty.ToTernary();  // null (no items)
```

**Key Methods**:
- `ToTernary()` - Aggregate booleans to ternary (true/false/null)

---

### Reflection & Assembly Extensions
**Location**: `ExtensionMethods/System/Reflection/AssemblyExtensions.cs`

```csharp
var assembly = typeof(SomeClass).Assembly;

// Find all types with attribute
IList<Type> types = assembly.TypesWith(typeof(SerializableAttribute));
IList<Type> types = assembly.TypesWith<CustomAttribute>();
```

**Key Methods**:
- `TypesWith(attributeType)` - Find all types marked with attribute
- `TypesWith<T>()` - Generic version with type safety

---

### IO & Path Extensions
**Location**: `ExtensionMethods/System/IO/DirectoryExtensions.cs` and `PathExtensions.cs`

```csharp
// Ensure directory exists (creates if missing)
string path = @"C:\my\app\data".EnsureDirectoryExists();

// Path hierarchy checking
bool match = @"C:\app\data\file.txt".PathEqualsOrIsDescendant(@"C:\app");  // true
bool match = @"C:\other\file.txt".PathEqualsOrIsDescendant(@"C:\app");  // false
```

**Key Methods**:
- `EnsureDirectoryExists()` - Creates directory if missing
- `PathEqualsOrIsDescendant()` - Check path hierarchy

---

## Concurrent Collections

### ConcurrentList<T>
**Location**: `ExtensionMethods/System/Collections/Concurrent/ConcurrentList.cs`

Thread-safe list implementation using lock-based synchronization (compatible with older frameworks).

```csharp
var list = new ConcurrentList<string>();

list.Add("item1");
list.AddRange(items);
var count = list.Count;  // Thread-safe
var item = list[0];      // Thread-safe
list.RemoveAt(0);        // Thread-safe

// Enumerator creates snapshot
foreach (var item in list) { }  // Safe under concurrent modifications
```

**Implementation Details**:
- Backed by `ConcurrentQueue<T>` for add operations
- Wraps with `List<T>` for indexed access (with lock)
- Updates list only when dirty (performance optimization)
- All operations fully synchronized

**Use When**: You need indexed access with thread safety (older .NET versions)

---

### ConcurrentHashSet<T>
**Location**: `ExtensionMethods/System/Collections/Concurrent/ConcurrentHashSet.cs`

Thread-safe set using `ReaderWriterLockSlim` for better read concurrency.

```csharp
var set = new ConcurrentHashSet<string>();

set.Add("item");           // Write lock
bool exists = set.Contains("item");  // Read lock (can be concurrent)
set.Remove("item");        // Write lock
set.Clear();               // Write lock
int count = set.Count;     // Read lock

var array = set.ToArray();  // Thread-safe snapshot
foreach (var item in set)   // Safe enumeration
{
}

set.Dispose();  // Dispose ReaderWriterLockSlim
```

**Implementation Details**:
- Uses `ReaderWriterLockSlim` with `SupportsRecursion`
- Read operations (Contains, Count, GetEnumerator) use read lock
- Write operations (Add, Remove, Clear) use write lock
- GetEnumerator returns snapshot (ToArray) for thread safety

**Use When**: You need concurrent reads with occasional writes (better performance than ConcurrentList)

---

### CovariantConcurrentDictionary<TKey, TValue>
**Location**: `Collections/CovariantConcurrentDictionary.cs`

Wrapper around `ConcurrentDictionary<TKey, TValue>` with covariant interfaces.

```csharp
var dict = new CovariantConcurrentDictionary<string, object>();

dict["key"] = new MyObject();
dict.Add("key", value);

// Covariant enumerable of IKeyValuePair
IEnumerable<IKeyValuePair<string, object>> pairs = dict;

bool exists = dict.ContainsKey("key");
bool removed = dict.Remove("key");
```

**Key Feature**: Provides `ICovariantReadOnlyDictionary<TKey, TValue>` interface

---

## Exception Types

### Domain-Specific Exceptions

All domain exceptions inherit from standard `Exception` and support nullable reference types.

**AlreadyException** - Base class for "already done" scenarios
```csharp
throw new AlreadyException("Operation has already been completed");
```

**AlreadySetException** - Property can only be set once
```csharp
private string _name;
public string Name
{
    get => _name;
    set
    {
        if (_name != null) throw new AlreadySetException();
        _name = value;
    }
}
```

**NotInitializedException** - Component not yet initialized
```csharp
if (!initialized) throw new NotInitializedException("Call Initialize() first");
```

**DetailException** - Exception with arbitrary detail object
```csharp
var detail = new { ErrorCode = 500, Reason = "Server error" };
throw new DetailException(detail, "Operation failed");

// Later:
try { }
catch (DetailException ex)
{
    var detail = ex.Detail;  // Access structured data
}
```

**PermanentException** - Indicates error is not temporary (implements `IPotentiallyTemporaryError`)
```csharp
throw new PermanentException("This error will never resolve");

// Check in retry logic:
if (ex is IPotentiallyTemporaryError err 
    && err.IsTemporaryError != PotentiallyTemporaryErrorKind.KnownPermanent)
{
    // Safe to retry
}
```

---

## Utility Classes

### DateTime Utilities
**Location**: `System/DateTimeUtils.cs`, `System/DateExtensions.cs`, `System/DateRangeUtils.cs`

```csharp
var now = DateTime.Now;

// Round to various precisions
var noMillis = now.RoundMillisecondsToZero();  // 14:23:45.000
var noSeconds = now.RoundSecondsToZero();      // 14:23:00.000
var noMinutes = now.RoundMinutesToZero();      // 14:00:00.000
var noHours = now.RoundHoursToZero();          // 00:00:00.000
var startOfMonth = now.RoundDaysToOne();       // First day of month
var startOfYear = now.RoundMonthsToOne();      // Jan 1st

// Compare at specific precision
bool sameMinute = time1.IsSameMinute(time2);
```

**Key Methods**:
- `RoundMillisecondsToZero()`, `RoundSecondsToZero()`, etc.
- `IsSameMinute()` - Component-wise minute comparison

---

### LINQ Utilities
**Location**: `Linq/LinqExtensionMethods.cs`

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

// Safe Any() on possibly-null enumerable
if (items.NullableAny())  // returns false if null
{
}

// Aggregate with default value if empty
int sum = numbers.AggregateOrDefault((a, b) => a + b);      // 15
int sum = empty.AggregateOrDefault((a, b) => a + b);         // 0 (default)
int sum = empty.AggregateOrDefault((a, b) => a + b, 100);   // 100
```

**Key Methods**:
- `NullableAny()` - Safe Any() handling null
- `AggregateOrDefault()` - Aggregate with default fallback

---

### Task & Async Utilities
**Location**: `Threading/Tasks/TaskExtensions.cs`, `Threading/Tasks/AsyncUtils.cs`

```csharp
// Fire and forget with swallowed exceptions
Task.Delay(5000).FireAndForget();
Task.Delay(5000).FireAndForget("MyTaskName");

// SwallowedException event for monitoring
TaskExtensions.SwallowedException += (ex) => Log.Error(ex);

// Get result safely without deadlock
T result = task.GetResultSafe();

// Safe wait without synchronization context issues
await task.WaitSafe();

// Wait with timeout
await task.WithTimeout(millisecondsTimeout: 5000, onException: () => { });

// Wait handle with cancellation support
bool completed = await handle.WaitOneAsync(cancellationToken);
bool completed = await handle.WaitOneAsync(TimeSpan.FromSeconds(5), token);

// Await cancellation token directly
await cancellationToken;  // Throws OperationCanceledException when canceled
```

**Key Methods**:
- `FireAndForget()` - Async fire-and-forget with tracking
- `GetResultSafe()` - Thread-safe result retrieval
- `WaitSafe()` - Safe wait without deadlock
- `WithTimeout()` - Timeout with exception handling
- `WaitOneAsync()` - Async wait handle with cancellation

---

### Default Value Utilities
**Location**: `DefaultValues/DefaultValueExtensions.cs`

Work with `[DefaultValue]` attributes on properties/fields.

```csharp
public class Config
{
    [DefaultValue(42)]
    public int Port { get; set; }

    [DefaultValue("localhost")]
    public string Host { get; set; }
}

var obj = new Config();

// Get default value for member
object def = typeof(Config).GetProperty("Port").GetDefaultValue();  // 42

// Check if member is at default value
bool isDefault = typeof(Config).GetProperty("Port").IsDefaultValue(obj);

// Apply all defaults from attributes
obj.ApplyDefaultValues();

// Check if objects are equal (generic)
bool equal = obj1.GenericEquals(obj2);
bool isDefault = obj.IsDefault();  // Is T equal to default(T)?
```

**Key Methods**:
- `GetDefaultValue()` - PropertyInfo/FieldInfo
- `IsDefaultValue()` - Check if member equals default
- `ApplyDefaultValues()` - Apply all [DefaultValue] attributes
- `GenericEquals()` - Type-safe equality check
- `IsDefault()` - Is value equal to default(T)?

---

### Lazy Utilities
**Location**: `Structures/LazyExtensions.cs`

```csharp
Func<ExpensiveObject> factory = () => new ExpensiveObject();

// Convert factory function to Lazy<T>
Lazy<ExpensiveObject> lazy = factory.ToLazy();
var obj = lazy.Value;  // Initialized on first access
```

**Key Methods**:
- `ToLazy()` - Wrap factory function in Lazy<T>

---

### Service Provider Extensions
**Location**: `System/IServiceProviderExtensions.cs`

```csharp
IServiceProvider services = /* DI container */;

// Get service with type safety (pre-generics fallback)
var logger = services.GetRequiredService<ILogger>(typeof(ILogger));

// Throws ArgumentException if service not found
```

**Key Methods**:
- `GetRequiredService<T>()` - Get service or throw

---

## Directory Utilities
**Location**: `IO/DirectoryX.cs`

```csharp
// Async directory existence check
bool exists = await DirectoryAsync.ExistsAsync("/some/path");
```

---

## Patterns & Idioms

### Execution: Repeat Until Success
**Location**: `Execution/RepeatAllUntilExtensions.cs`, `Execution/RepeatAllUntilTrue.cs`

Retry pattern for operations that may fail temporarily.

```csharp
var items = new[] { itemA, itemB, itemC };

// Retry each item until successful
await items.RepeatAllUntil(
    item => () => ProcessAsync(item),           // Operation
    result => result.IsSuccess,                 // Success predicate
    cancellationToken: cts.Token,
    parallel: false
);

// Or try and collect failures
(bool succeeded, var failures) = await items.TryRepeatAllUntil(
    item => () => ProcessAsync(item),
    result => result.IsSuccess,
    cancellationToken: cts.Token,
    parallel: true  // Process items in parallel
);

if (!succeeded)
{
    foreach (var (item, result) in failures)
    {
        Console.WriteLine($"Item {item} failed with: {result}");
    }
}
```

**Key Features**:
- Repeats all items until each succeeds or no progress made
- Supports parallel processing
- Supports cancellation tokens
- Returns remaining failed items on failure

---

### Error Classification
**Location**: `Execution/IPotentiallyTemporaryError.cs`

```csharp
public enum PotentiallyTemporaryErrorKind
{
    Unknown,           // Might be temporary (default assumption)
    KnownTemporary,    // Definitely temporary (e.g., network timeout)
    KnownPermanent     // Definitely permanent (e.g., invalid parameter)
}

public interface IPotentiallyTemporaryError
{
    PotentiallyTemporaryErrorKind IsTemporaryError { get; }
}

// Usage in retry logic
try
{
    await CallExternalApi();
}
catch (Exception ex)
{
    if (ex is IPotentiallyTemporaryError err)
    {
        bool shouldRetry = err.IsTemporaryError != PotentiallyTemporaryErrorKind.KnownPermanent;
        if (shouldRetry) { /* retry */ }
    }
}
```

---

## Global Usings

**Location**: `GlobalUsings.cs`

The library defines global usings for common namespaces:

```csharp
global using System;
global using System.Collections;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Linq;
global using System.Reflection;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;
```

---

## Design Philosophy

**Why LionFire.Base?**

1. **No External Dependencies** - Suitable as a foundation for all other projects
2. **BCL Augmentation** - Fills common gaps in standard .NET APIs
3. **Practical Utilities** - Focuses on things you use repeatedly
4. **Thread Safety** - Provides concurrent collections for multi-threaded scenarios
5. **Error Semantics** - Domain exceptions communicate intent better than generic ones
6. **Null Handling** - Safe variants of common operations (e.g., `NullableAny()`)

---

## File Organization

```
LionFire.Base/
├── ExtensionMethods/
│   └── System/
│       ├── Collections/
│       │   ├── Concurrent/          # ConcurrentList, ConcurrentHashSet, etc.
│       │   └── Generic/             # IEnumerable, IDictionary, IHashSet, ICollection
│       ├── IO/                      # Directory, Path extensions
│       ├── Reflection/              # Assembly extensions
│       ├── BooleanExtensions.cs
│       ├── EnumExtensions.cs
│       └── StringExtensions.cs
├── Collections/                     # Custom collection types
├── DateTime/                        # DateTime utilities
├── DefaultValues/                   # Default value handling
├── Exceptions/                      # Domain exceptions
├── Execution/                       # Execution patterns
├── IO/                              # Directory utilities
├── Linq/                            # LINQ extensions
├── Reflection/                      # Reflection utilities
├── Serialization/                   # Serialization interfaces
├── Structures/                      # Data structure interfaces
├── System/                          # System utilities
├── Text/                            # String utilities
├── Threading/
│   └── Tasks/                       # Async/Task utilities
└── Types/                           # Type utilities
```

---

## Common Usage Patterns

### Safe Collection Operations
```csharp
// Don't: Throws if key exists
dict["key"] = value;  // Overwrites silently

// Do: Explicit intent
if (!dict.TryAdd("key", value))
{
    // Handle key already exists
}

// Or lazy initialization
var obj = dict.GetOrAdd("key", () => new MyObject());
```

### Thread-Safe Collections
```csharp
// Use ConcurrentHashSet for mostly-read scenarios
var set = new ConcurrentHashSet<string>();
foreach (var item in set) { }  // Safe snapshot

// Use ConcurrentList for indexed access needs
var list = new ConcurrentList<Item>();
var count = list.Count;  // Thread-safe
```

### Retry Logic
```csharp
// For operations that might fail temporarily
await items.RepeatAllUntil(
    item => () => item.InitializeAsync(),
    result => result.Success,
    parallel: true
);
```

### Error Handling
```csharp
// Distinguish error types
try { }
catch (AlreadySetException)
{
    // Property was already set - not an error in some contexts
}
catch (NotInitializedException)
{
    // Initialization order issue
}
catch (DetailException detail)
{
    var errorData = detail.Detail;  // Structured error info
}
```

---

## Performance Considerations

- **ConcurrentHashSet**: Better for read-heavy workloads (ReaderWriterLockSlim)
- **ConcurrentList**: Better for ordered list needs with index access
- **HasAtLeast()**: Doesn't enumerate entire sequence - use when you just need "at least N items"
- **ToArray()**: ConcurrentHashSet.GetEnumerator makes snapshot - safe but allocates

---

## Related Documentation

- See parent `CLAUDE.md` for overview of entire LionFire monorepo
- LionFire.Flex: Adds dynamic property extension on top of Base
- LionFire.Structures: Collection types building on Base
- LionFire.Core.Extras: General-purpose utilities using Base

---

## Quick Reference Sheet

| What | Where | Method |
|------|-------|--------|
| Wrap item in IEnumerable | IEnumerableExtensions | `Yield<T>()` |
| Safe Any() | IEnumerableLinqExtensions | `Any()` |
| Dictionary safe add | IDictionaryExtensions | `TryAdd()` |
| Get or create | IDictionaryExtensions | `GetOrAdd()` |
| Thread-safe set | ConcurrentHashSet | `Add()`, `Contains()` |
| Thread-safe list | ConcurrentList | `Add()`, `[index]` |
| String manipulations | StringExtensions, StringX | `InsertSpaceBeforeCaps()`, `ToKebabCase()` |
| DateTime rounding | DateTimeUtils | `RoundMinutesToZero()` |
| Retry until success | RepeatAllUntilExtensions | `RepeatAllUntil()`, `TryRepeatAllUntil()` |
| Fire and forget | TaskExtensions | `FireAndForget()` |
| Property defaults | DefaultValueExtensions | `ApplyDefaultValues()` |

