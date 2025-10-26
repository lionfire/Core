# Getters and Setters Guide

## Overview

This comprehensive guide covers LionFire's **Getter and Setter patterns** for async data access. Understanding these patterns is essential for building applications with proper data caching, lazy loading, and reactive updates.

**Key Concept**: Getters retrieve data, Setters write data, and Values combine both with caching and reactive features.

---

## Table of Contents

1. [Interface Hierarchy](#interface-hierarchy)
2. [Getter Patterns](#getter-patterns)
3. [Setter Patterns](#setter-patterns)
4. [Value Pattern (Read + Write)](#value-pattern-read--write)
5. [Result Types](#result-types)
6. [Creating Implementations](#creating-implementations)
7. [Common Patterns](#common-patterns)
8. [Best Practices](#best-practices)

---

## Interface Hierarchy

```
IGetter (marker interface)
│
├── IStatelessGetter<T>
│   └── Always retrieves, no caching
│
└── ILazyGetter
    └── IGetter<T>
        ├── Lazy loading with cache
        ├── Observable operations
        ├── State tracking (IsLoading, HasValue)
        └── Extends to:
            ├── IGetterRxO<T>        (ReactiveObject variant)
            └── IValue<T>            (Read + Write)
                    └── IValueRxO<T>  (ReactiveObject variant)

ISetter<T>
└── Write operations
    └── ISetterRxO<T>  (ReactiveObject variant)
```

---

## Getter Patterns

### IStatelessGetter\<TValue\>

**Purpose**: Stateless data retrieval - always fetches fresh data.

**When to Use**:
- API calls that should always be fresh
- Real-time data that changes frequently
- Don't want caching
- Idempotent operations

#### Interface

```csharp
public interface IStatelessGetter<out TValue> : IGetter
{
    ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default);
}
```

**Characteristics**:
- **Stateless**: No state between calls
- **Covariant**: `out TValue` allows flexible usage
- **Result-Based**: Returns `IGetResult<T>` with success/error info
- **Cancellable**: Supports cancellation tokens

#### Usage Example

```csharp
public class WeatherService
{
    public IStatelessGetter<WeatherData> GetCurrentWeather(string city)
    {
        return new StatelessGetterFunc<WeatherData>(async ct =>
        {
            var response = await httpClient.GetAsync($"/weather/{city}", ct);
            var data = await response.Content.ReadAsAsync<WeatherData>(ct);
            return GetResult.Success(data);
        });
    }
}

// Usage
var weatherGetter = weatherService.GetCurrentWeather("Seattle");

// Each call retrieves fresh data
var result1 = await weatherGetter.Get();  // API call
var result2 = await weatherGetter.Get();  // Another API call

Console.WriteLine($"Temp: {result1.Value.Temperature}°F");
```

---

### ILazyGetter

**Purpose**: Marker interface for lazy-loading capability.

```csharp
public interface ILazyGetter : IDefaultable, IDiscardableValue, IDiscardable
{
    // Lazy loading semantics
    // Call Get() only if value not cached
}
```

**Key Methods** (from interfaces):
- `DiscardValue()` - Clear cached value
- `SetToDefault()` - Reset to default state

---

### IGetter\<TValue\>

**Purpose**: Full-featured lazy getter with caching, observables, and state tracking.

**This is the most commonly used getter interface.**

#### Interface

```csharp
public interface IGetter<out TValue>
    : IStatelessGetter<TValue>           // Can still force retrieve
    , ILazyGetter                         // Supports lazy loading
    , IObservableGetOperations<TValue>    // Observable operations
    , IObservableGetState                 // Observable state
    , IObservableGetResults<TValue>       // Observable results
{
    // Lazy get (uses cache if available)
    ITask<IGetResult<TValue>> GetIfNeeded(CancellationToken cancellationToken = default);

    // Synchronous cache access
    TValue? ReadCacheValue { get; }

    // Cache state
    bool HasValue { get; }
    ReadState ReadState { get; }

    // Cache invalidation
    void DiscardValue();
}
```

#### Key Methods

**Get()** - Always retrieves:
```csharp
var result = await getter.Get();  // Retrieves from source
```

**GetIfNeeded()** - Lazy retrieval:
```csharp
var result = await getter.GetIfNeeded();  // Uses cache if available
```

**ReadCacheValue** - Synchronous cache:
```csharp
if (getter.HasValue)
{
    var cached = getter.ReadCacheValue;  // No await needed
}
```

**DiscardValue()** - Clear cache:
```csharp
getter.DiscardValue();  // Force next GetIfNeeded() to retrieve
```

#### Usage Example

```csharp
public class UserService
{
    public IGetter<UserProfile> GetProfile(string userId)
    {
        return new GetterRxO<UserProfile>(async ct =>
        {
            var profile = await database.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync(ct);

            return GetResult.Success(profile);
        });
    }
}

// Usage
var profileGetter = userService.GetProfile("user123");

// First call: retrieves from database
var result1 = await profileGetter.GetIfNeeded();
Console.WriteLine($"Name: {result1.Value.Name}");

// Second call: returns cached value (no database hit)
var result2 = await profileGetter.GetIfNeeded();

// Synchronous cache access
if (profileGetter.HasValue)
{
    var name = profileGetter.ReadCacheValue.Name;  // No await
}

// Force refresh
profileGetter.DiscardValue();
var result3 = await profileGetter.GetIfNeeded();  // Retrieves again
```

---

### IGetterRxO\<TValue\>

**Purpose**: ReactiveObject variant of `IGetter` - all properties are reactive.

```csharp
public interface IGetterRxO<out TValue>
    : IReactiveObjectEx
    , IGetter<TValue>
{
    // All properties raise PropertyChanged events
    // Can use WhenAnyValue() on any property
}
```

#### Reactive Features

```csharp
IGetterRxO<UserProfile> getter = ...;

// React to loading state
getter.WhenAnyValue(g => g.IsLoading)
    .Subscribe(isLoading => ShowSpinner(isLoading));

// React to value availability
getter.WhenAnyValue(g => g.HasValue)
    .Subscribe(hasValue => EnableUI(hasValue));

// React to cached value changes
getter.WhenAnyValue(g => g.ReadCacheValue)
    .WhereNotNull()
    .Subscribe(profile => DisplayProfile(profile));

// React to read state changes
getter.WhenAnyValue(g => g.ReadState)
    .Subscribe(state => UpdateStatusBar(state));
```

---

## Setter Patterns

### ISetter\<TValue\>

**Purpose**: Async write operations.

```csharp
public interface ISetter<TValue>
{
    Task<ISetResult<TValue>> Set(TValue? value, CancellationToken cancellationToken = default);
}
```

#### Usage Example

```csharp
public class ConfigService
{
    public ISetter<AppSettings> GetSettingsSetter()
    {
        return new SetterFunc<AppSettings>(async (value, ct) =>
        {
            await File.WriteAllTextAsync("settings.json",
                JsonSerializer.Serialize(value), ct);

            return SetResult.Success(value);
        });
    }
}

// Usage
var setter = configService.GetSettingsSetter();

var newSettings = new AppSettings { Theme = "Dark" };
var result = await setter.Set(newSettings);

if (result.IsSuccess)
{
    Console.WriteLine("Settings saved");
}
else
{
    Console.WriteLine($"Error: {result.Error}");
}
```

---

### ISetterRxO\<TValue\>

**Purpose**: ReactiveObject variant - observable write operations.

```csharp
public interface ISetterRxO<TValue>
    : IReactiveObjectEx
    , ISetter<TValue>
    , IObservableSetOperations
    , IObservableSetState
    , IObservableSetResults<TValue>
{
    bool IsSetting { get; }  // Reactive property
}
```

#### Reactive Features

```csharp
ISetterRxO<Config> setter = ...;

// React to save state
setter.WhenAnyValue(s => s.IsSetting)
    .Subscribe(isSetting =>
    {
        SaveButton.Enabled = !isSetting;
        if (isSetting)
            ShowProgressBar();
        else
            HideProgressBar();
    });

// React to save results
setter.SetResults
    .Subscribe(result =>
    {
        if (result.IsSuccess)
            ShowSuccessMessage();
        else
            ShowError(result.Error);
    });
```

---

## Value Pattern (Read + Write)

### IStatelessAsyncValue\<T\>

**Purpose**: Combined read/write without caching.

```csharp
public interface IStatelessAsyncValue<T>
    : IStatelessGetter<T>
    , ISetter<T>
{
    // Get() always retrieves
    // Set() writes immediately
}
```

---

### IValue\<T\>

**Purpose**: Full-featured read/write with caching and observables.

**This is the most commonly used combined interface.**

```csharp
public interface IValue<T>
    : IStatelessAsyncValue<T>
    , IGetter<T>
    , ISetter<T>
{
    // Combines:
    // - Lazy loading (GetIfNeeded)
    // - Caching (ReadCacheValue, HasValue)
    // - Write operations (Set)
    // - Observable operations
    // - State tracking
}
```

#### Usage Example

```csharp
public class AppSettingsService
{
    public IValue<AppSettings> GetSettings()
    {
        return new ValueRxO<AppSettings>(
            getFunc: async ct =>
            {
                var json = await File.ReadAllTextAsync("settings.json", ct);
                return JsonSerializer.Deserialize<AppSettings>(json);
            },
            setFunc: async (value, ct) =>
            {
                var json = JsonSerializer.Serialize(value);
                await File.WriteAllTextAsync("settings.json", json, ct);
            }
        );
    }
}

// Usage
var settings = settingsService.GetSettings();

// Load (lazy)
await settings.GetIfNeeded();

// Read cached value (synchronous)
var currentTheme = settings.ReadCacheValue.Theme;

// Modify
settings.ReadCacheValue.Theme = "Dark";
settings.ReadCacheValue.FontSize = 14;

// Save
var saveResult = await settings.Set(settings.ReadCacheValue);

if (saveResult.IsSuccess)
{
    Console.WriteLine("Settings saved");
}
```

---

### IValueRxO\<T\>

**Purpose**: ReactiveObject variant of `IValue` - all properties reactive.

```csharp
public interface IValueRxO<T>
    : IReactiveObjectEx
    , IValue<T>
    , IGetterRxO<T>
    , ISetterRxO<T>
{
    // Combines all reactive features for read/write
}
```

#### Complete Reactive Example

```csharp
IValueRxO<UserProfile> profile = userService.GetUserProfile(userId);

// Load data
await profile.GetIfNeeded();

// React to loading state
profile.WhenAnyValue(p => p.IsLoading)
    .Subscribe(isLoading => LoadingIndicator.Visible = isLoading);

// React to saving state
profile.WhenAnyValue(p => p.IsSetting)
    .Subscribe(isSetting => SaveButton.Enabled = !isSetting);

// React to value changes
profile.WhenAnyValue(p => p.ReadCacheValue)
    .WhereNotNull()
    .Subscribe(profileData => UpdateUI(profileData));

// Modify and save
profile.ReadCacheValue.Email = "newemail@example.com";
await profile.Set(profile.ReadCacheValue);
```

---

## Result Types

### IGetResult\<TValue\>

```csharp
public interface IGetResult<out TValue>
{
    bool IsSuccess { get; }
    TValue? Value { get; }
    Exception? Error { get; }
    string? ErrorMessage { get; }
}
```

#### Usage

```csharp
var result = await getter.Get();

if (result.IsSuccess)
{
    Console.WriteLine($"Got value: {result.Value}");
}
else
{
    Console.WriteLine($"Error: {result.ErrorMessage}");
    if (result.Error != null)
        Logger.LogError(result.Error, "Failed to get data");
}
```

#### Creating Results

```csharp
// Success
return GetResult.Success(myData);

// Failure
return GetResult.Failure<MyData>("Not found");

// Failure with exception
return GetResult.Failure<MyData>(ex);
```

---

### ISetResult\<TValue\>

```csharp
public interface ISetResult<out TValue>
{
    bool IsSuccess { get; }
    TValue? Value { get; }  // Value that was written
    Exception? Error { get; }
    string? ErrorMessage { get; }
}
```

#### Usage

```csharp
var result = await setter.Set(newValue);

if (result.IsSuccess)
{
    Console.WriteLine("Saved successfully");
}
else
{
    ShowErrorDialog(result.ErrorMessage);
}
```

---

## Creating Implementations

### Creating a Custom Getter

```csharp
public class DatabaseUserGetter : GetterRxO<UserProfile>
{
    private readonly IDbContext db;
    private readonly string userId;

    public DatabaseUserGetter(IDbContext db, string userId)
    {
        this.db = db;
        this.userId = userId;
    }

    protected override async Task<IGetResult<UserProfile>> GetImpl(CancellationToken ct)
    {
        try
        {
            var user = await db.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync(ct);

            if (user == null)
                return GetResult.Failure<UserProfile>("User not found");

            return GetResult.Success(user);
        }
        catch (Exception ex)
        {
            return GetResult.Failure<UserProfile>(ex);
        }
    }
}
```

**Usage**:
```csharp
var getter = new DatabaseUserGetter(dbContext, "user123");
await getter.GetIfNeeded();

if (getter.HasValue)
{
    Console.WriteLine($"Name: {getter.ReadCacheValue.Name}");
}
```

---

### Creating a Custom Setter

```csharp
public class DatabaseUserSetter : SetterRxO<UserProfile>
{
    private readonly IDbContext db;

    public DatabaseUserSetter(IDbContext db)
    {
        this.db = db;
    }

    protected override async Task<ISetResult<UserProfile>> SetImpl(
        UserProfile? value,
        CancellationToken ct)
    {
        if (value == null)
            return SetResult.Failure<UserProfile>("Value cannot be null");

        try
        {
            db.Users.Update(value);
            await db.SaveChangesAsync(ct);
            return SetResult.Success(value);
        }
        catch (Exception ex)
        {
            return SetResult.Failure<UserProfile>(ex);
        }
    }
}
```

---

### Creating a Custom Value

```csharp
public class FileConfigValue : ValueRxO<AppSettings>
{
    private readonly string filePath;

    public FileConfigValue(string filePath)
    {
        this.filePath = filePath;
    }

    protected override async Task<IGetResult<AppSettings>> GetImpl(CancellationToken ct)
    {
        try
        {
            if (!File.Exists(filePath))
                return GetResult.Success(new AppSettings());  // Default

            var json = await File.ReadAllTextAsync(filePath, ct);
            var settings = JsonSerializer.Deserialize<AppSettings>(json);
            return GetResult.Success(settings);
        }
        catch (Exception ex)
        {
            return GetResult.Failure<AppSettings>(ex);
        }
    }

    protected override async Task<ISetResult<AppSettings>> SetImpl(
        AppSettings? value,
        CancellationToken ct)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, json, ct);
            return SetResult.Success(value);
        }
        catch (Exception ex)
        {
            return SetResult.Failure<AppSettings>(ex);
        }
    }
}
```

**Usage**:
```csharp
var config = new FileConfigValue("appsettings.json");

// Load
await config.GetIfNeeded();

// Modify
config.ReadCacheValue.Theme = "Dark";

// Save
await config.Set(config.ReadCacheValue);
```

---

## Common Patterns

### Pattern 1: Load Once, Use Many Times

```csharp
IGetter<AppConfig> configGetter = configService.GetConfig();

// Load once
await configGetter.GetIfNeeded();

// Use cached value multiple times
var timeout = configGetter.ReadCacheValue.Timeout;
var apiKey = configGetter.ReadCacheValue.ApiKey;
var retries = configGetter.ReadCacheValue.MaxRetries;

// No additional loads!
```

---

### Pattern 2: Conditional Refresh

```csharp
public async Task<UserProfile> GetUserProfile(bool forceRefresh = false)
{
    if (forceRefresh)
    {
        profileGetter.DiscardValue();
    }

    await profileGetter.GetIfNeeded();
    return profileGetter.ReadCacheValue;
}
```

---

### Pattern 3: Reactive Auto-Refresh

```csharp
public class LiveDataVM : ReactiveObject
{
    private readonly IGetter<LiveData> dataGetter;

    public LiveDataVM(IGetter<LiveData> dataGetter)
    {
        this.dataGetter = dataGetter;

        // Auto-refresh every 5 seconds
        Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(5))
            .Subscribe(async _ =>
            {
                dataGetter.DiscardValue();
                await dataGetter.GetIfNeeded();
            });

        // Update UI when data changes
        dataGetter.WhenAnyValue(g => g.ReadCacheValue)
            .WhereNotNull()
            .Subscribe(data => CurrentData = data);
    }

    [Reactive] private LiveData? _currentData;
}
```

---

### Pattern 4: Save on Change with Debounce

```csharp
public class AutoSaveVM : ReactiveObject
{
    private readonly IValue<Document> documentValue;

    public AutoSaveVM(IValue<Document> documentValue)
    {
        this.documentValue = documentValue;

        // Load document
        documentValue.GetIfNeeded().FireAndForget();

        // Auto-save 2 seconds after last change
        this.WhenAnyValue(vm => vm.Document)
            .Throttle(TimeSpan.FromSeconds(2))
            .Subscribe(async doc =>
            {
                if (doc != null)
                {
                    await documentValue.Set(doc);
                }
            });
    }

    public Document? Document
    {
        get => documentValue.ReadCacheValue;
        set
        {
            if (documentValue.ReadCacheValue != value)
            {
                // Trigger auto-save via WhenAnyValue
                this.RaisePropertyChanged(nameof(Document));
            }
        }
    }
}
```

---

### Pattern 5: Get or Create

```csharp
public async Task<Config> GetOrCreateConfig(string configId)
{
    var getter = configService.GetConfig(configId);
    var result = await getter.GetIfNeeded();

    if (!result.IsSuccess || result.Value == null)
    {
        // Create default
        var defaultConfig = new Config { Id = configId };
        var setter = configService.GetConfigSetter();
        await setter.Set(defaultConfig);
        return defaultConfig;
    }

    return result.Value;
}
```

---

## Best Practices

### 1. Choose Appropriate Interface

```csharp
// ✅ Use IStatelessGetter for always-fresh data
IStatelessGetter<StockPrice> priceGetter;  // API call every time

// ✅ Use IGetter for cacheable data
IGetter<UserProfile> profileGetter;  // Cache until discarded

// ✅ Use IValue for read/write
IValue<AppSettings> settings;  // Load once, modify, save
```

---

### 2. Check Results

```csharp
// ✅ Always check result status
var result = await getter.Get();
if (result.IsSuccess)
{
    UseData(result.Value);
}
else
{
    HandleError(result.Error);
}

// ❌ Don't assume success
var value = (await getter.Get()).Value;  // May be null!
```

---

### 3. Use GetIfNeeded for Lazy Loading

```csharp
// ✅ Lazy loading
await getter.GetIfNeeded();  // Retrieves only if not cached

// ❌ Always retrieves (unnecessary)
await getter.Get();  // Fetches even if cached
```

---

### 4. Discard Before Refresh

```csharp
// ✅ Explicit refresh
getter.DiscardValue();
await getter.GetIfNeeded();  // Guaranteed fresh

// ❌ GetIfNeeded won't retrieve if cached
await getter.GetIfNeeded();  // May return stale data
```

---

### 5. Use ReactiveObject Variants for UI

```csharp
// ✅ For UI binding
IGetterRxO<Data> getter;  // Can use WhenAnyValue

// ❌ For non-UI services
IGetter<Data> getter;  // No reactive properties (fine for services)
```

---

## Related Documentation

- **[Async Data Overview](README.md)** - Overview and quick start
- **[Observable Operations](observable-operations.md)** - Reactive features
- **[Caching Strategies](caching-strategies.md)** - Cache management
- **[Collections](collections.md)** - Async collections
- **[LionFire.Data.Async.Abstractions](../../../src/LionFire.Data.Async.Abstractions/CLAUDE.md)** - Complete API reference

---

## Summary

**Getter Patterns**:
- **IStatelessGetter<T>** - Always retrieves, no cache
- **IGetter<T>** - Lazy loading with cache
- **IGetterRxO<T>** - ReactiveObject variant

**Setter Patterns**:
- **ISetter<T>** - Async write
- **ISetterRxO<T>** - ReactiveObject variant

**Value Pattern**:
- **IValue<T>** - Combined read/write with cache
- **IValueRxO<T>** - ReactiveObject variant

**Key Methods**:
- `Get()` - Force retrieve
- `GetIfNeeded()` - Lazy retrieve
- `Set(value)` - Write operation
- `DiscardValue()` - Clear cache
- `ReadCacheValue` - Synchronous cache access

**Most Common**: `IGetter<T>` (80% of scenarios), `IValue<T>` (15%), `IStatelessGetter<T>` (5%)
