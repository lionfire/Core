# How-To: Implement a Custom Getter

## Problem

You need to fetch data from a custom source (API, database, etc.) using LionFire's async data patterns with `IGetter<T>`.

## Solution

Implement a custom getter by extending `GetterRxO<T>` or implementing `IGetter<T>` directly.

---

## Approach 1: Simple Stateless Getter (Always Fresh Data)

**Use case**: API calls, real-time data, always fetch latest.

### Step 1: Define Your Data Model

```csharp
public record WeatherForecast(
    string City,
    double Temperature,
    string Condition,
    DateTime Timestamp
);
```

### Step 2: Implement IStatelessGetter

```csharp
using LionFire.Data.Async;

public class WeatherApiGetter : IStatelessGetter<WeatherForecast>
{
    private readonly string city;
    private readonly string apiKey;

    public WeatherApiGetter(string city, string apiKey)
    {
        this.city = city;
        this.apiKey = apiKey;
    }

    public async Task<IGetResult<WeatherForecast>> Get(
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new HttpClient();
            var url = $"https://api.weather.com/v3/weather?city={city}&key={apiKey}";

            var response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var data = JsonSerializer.Deserialize<WeatherForecast>(json);

            return GetResult.Success(data!);
        }
        catch (HttpRequestException ex)
        {
            return GetResult.Failure<WeatherForecast>($"API error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return GetResult.Failure<WeatherForecast>($"Error: {ex.Message}");
        }
    }
}
```

### Step 3: Use the Getter

```csharp
var weatherGetter = new WeatherApiGetter("San Francisco", "your-api-key");

var result = await weatherGetter.Get();
if (result.IsSuccess)
{
    var weather = result.Value!;
    Console.WriteLine($"Temperature in {weather.City}: {weather.Temperature}°F");
    Console.WriteLine($"Condition: {weather.Condition}");
}
else
{
    Console.WriteLine($"Error: {result.Error}");
}
```

---

## Approach 2: Lazy Getter with Caching

**Use case**: Expensive operations, data that doesn't change frequently, caching desired.

### Step 1: Extend GetterRxO

```csharp
using LionFire.Data.Async.Reactive;

public class UserProfileGetter : GetterRxO<UserProfile>
{
    private readonly string userId;
    private readonly HttpClient httpClient;

    public UserProfileGetter(string userId, HttpClient httpClient)
        : base(LoadProfile)
    {
        this.userId = userId;
        this.httpClient = httpClient;
    }

    private static async Task<IGetResult<UserProfile>> LoadProfile(
        CancellationToken ct)
    {
        // Access instance fields through closure
        var getter = (UserProfileGetter)this;

        try
        {
            var url = $"https://api.myapp.com/users/{getter.userId}";
            var response = await getter.httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                return GetResult.Failure<UserProfile>(
                    $"HTTP {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            var profile = JsonSerializer.Deserialize<UserProfile>(json);

            return GetResult.Success(profile!);
        }
        catch (Exception ex)
        {
            return GetResult.Failure<UserProfile>(ex.Message);
        }
    }
}
```

### Better Approach: Use Factory Function

```csharp
public class UserProfileGetter : GetterRxO<UserProfile>
{
    public UserProfileGetter(string userId, HttpClient httpClient)
        : base(ct => LoadProfile(userId, httpClient, ct))
    {
    }

    private static async Task<IGetResult<UserProfile>> LoadProfile(
        string userId,
        HttpClient httpClient,
        CancellationToken ct)
    {
        try
        {
            var url = $"https://api.myapp.com/users/{userId}";
            var response = await httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var profile = JsonSerializer.Deserialize<UserProfile>(json);

            return GetResult.Success(profile!);
        }
        catch (HttpRequestException ex)
        {
            return GetResult.Failure<UserProfile>($"API error: {ex.Message}");
        }
    }
}
```

### Usage with Caching

```csharp
var httpClient = new HttpClient();
var profileGetter = new UserProfileGetter("user123", httpClient);

// First call: fetches from API
await profileGetter.GetIfNeeded();
Console.WriteLine($"Profile: {profileGetter.ReadCacheValue.Name}");

// Second call: uses cache (instant!)
await profileGetter.GetIfNeeded();
Console.WriteLine($"Profile: {profileGetter.ReadCacheValue.Name}");

// Force refresh
profileGetter.DiscardValue();
await profileGetter.GetIfNeeded();
```

---

## Approach 3: Database Getter

**Use case**: Load from database with caching.

```csharp
using LionFire.Data.Async.Reactive;
using Microsoft.EntityFrameworkCore;

public class OrderGetter : GetterRxO<Order>
{
    public OrderGetter(string orderId, AppDbContext dbContext)
        : base(ct => LoadOrder(orderId, dbContext, ct))
    {
    }

    private static async Task<IGetResult<Order>> LoadOrder(
        string orderId,
        AppDbContext dbContext,
        CancellationToken ct)
    {
        try
        {
            var order = await dbContext.Orders
                .Include(o => o.Items)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == orderId, ct);

            if (order == null)
            {
                return GetResult.Failure<Order>($"Order {orderId} not found");
            }

            return GetResult.Success(order);
        }
        catch (Exception ex)
        {
            return GetResult.Failure<Order>($"Database error: {ex.Message}");
        }
    }
}

// Usage
using var dbContext = new AppDbContext();
var orderGetter = new OrderGetter("ORDER-001", dbContext);

await orderGetter.GetIfNeeded();
if (orderGetter.HasValue)
{
    var order = orderGetter.ReadCacheValue;
    Console.WriteLine($"Order total: ${order.Total}");
}
```

---

## Approach 4: Composite Getter (Multiple Sources)

**Use case**: Try cache, then API, then database.

```csharp
public class CompositeUserGetter : GetterRxO<User>
{
    public CompositeUserGetter(
        string userId,
        IMemoryCache cache,
        HttpClient httpClient,
        AppDbContext dbContext)
        : base(ct => LoadUser(userId, cache, httpClient, dbContext, ct))
    {
    }

    private static async Task<IGetResult<User>> LoadUser(
        string userId,
        IMemoryCache cache,
        HttpClient httpClient,
        AppDbContext dbContext,
        CancellationToken ct)
    {
        // Try memory cache first
        if (cache.TryGetValue(userId, out User? cachedUser))
        {
            Console.WriteLine("✅ Loaded from memory cache");
            return GetResult.Success(cachedUser!);
        }

        // Try API
        try
        {
            var response = await httpClient.GetAsync(
                $"https://api.myapp.com/users/{userId}", ct);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(ct);
                var user = JsonSerializer.Deserialize<User>(json)!;

                // Cache for 5 minutes
                cache.Set(userId, user, TimeSpan.FromMinutes(5));

                Console.WriteLine("✅ Loaded from API");
                return GetResult.Success(user);
            }
        }
        catch
        {
            Console.WriteLine("⚠️ API failed, falling back to database");
        }

        // Fallback to database
        try
        {
            var user = await dbContext.Users.FindAsync(new object[] { userId }, ct);

            if (user != null)
            {
                cache.Set(userId, user, TimeSpan.FromMinutes(5));
                Console.WriteLine("✅ Loaded from database");
                return GetResult.Success(user);
            }

            return GetResult.Failure<User>($"User {userId} not found");
        }
        catch (Exception ex)
        {
            return GetResult.Failure<User>($"All sources failed: {ex.Message}");
        }
    }
}
```

---

## Approach 5: Parameterized Getter Factory

**Use case**: Create getters dynamically based on parameters.

```csharp
public class ProductGetterFactory
{
    private readonly HttpClient httpClient;
    private readonly string apiBaseUrl;

    public ProductGetterFactory(HttpClient httpClient, string apiBaseUrl)
    {
        this.httpClient = httpClient;
        this.apiBaseUrl = apiBaseUrl;
    }

    public IGetter<Product> CreateGetter(string productId)
    {
        return new GetterRxO<Product>(ct => LoadProduct(productId, ct));
    }

    private async Task<IGetResult<Product>> LoadProduct(
        string productId,
        CancellationToken ct)
    {
        try
        {
            var url = $"{apiBaseUrl}/products/{productId}";
            var response = await httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var product = JsonSerializer.Deserialize<Product>(json);

            return GetResult.Success(product!);
        }
        catch (Exception ex)
        {
            return GetResult.Failure<Product>(ex.Message);
        }
    }
}

// Usage
var factory = new ProductGetterFactory(httpClient, "https://api.shop.com");

var productGetter1 = factory.CreateGetter("PROD-001");
var productGetter2 = factory.CreateGetter("PROD-002");

await productGetter1.GetIfNeeded();
await productGetter2.GetIfNeeded();
```

---

## Best Practices

### 1. Always Return GetResult

```csharp
// ✅ Good - Proper error handling
try
{
    var data = await FetchData();
    return GetResult.Success(data);
}
catch (Exception ex)
{
    return GetResult.Failure<T>(ex.Message);
}

// ❌ Avoid - Throwing exceptions
var data = await FetchData();  // May throw!
return GetResult.Success(data);
```

### 2. Support Cancellation

```csharp
// ✅ Good - Respects cancellation
public async Task<IGetResult<T>> Get(CancellationToken ct)
{
    var response = await httpClient.GetAsync(url, ct);  // Pass ct
    return GetResult.Success(await ProcessAsync(response, ct));
}

// ❌ Avoid - Ignores cancellation
public async Task<IGetResult<T>> Get(CancellationToken ct)
{
    var response = await httpClient.GetAsync(url);  // Missing ct
    return GetResult.Success(await ProcessAsync(response));
}
```

### 3. Use GetterRxO for Reactive Properties

```csharp
// ✅ Good - Observable loading state
public class MyGetter : GetterRxO<Data>
{
    // Automatically provides:
    // - IsLoading property
    // - GetResults observable
    // - Caching via HasValue/ReadCacheValue
}

// Use in ViewModel
this.WhenAnyValue(vm => vm.DataGetter.IsLoading)
    .Subscribe(loading => ShowSpinner(loading));
```

### 4. Handle Not Found Gracefully

```csharp
// ✅ Good - Distinguish not found from error
var user = await dbContext.Users.FindAsync(userId);
if (user == null)
{
    return GetResult.Failure<User>($"User {userId} not found");
}
return GetResult.Success(user);

// Or use custom result types
public enum GetStatus { Success, NotFound, Error }
public record GetResult<T>(GetStatus Status, T? Value, string? Error);
```

---

## Testing Custom Getters

```csharp
using Xunit;

public class UserProfileGetterTests
{
    [Fact]
    public async Task Get_ValidUser_ReturnsSuccess()
    {
        // Arrange
        var httpClient = CreateMockClient("user123", new UserProfile { Name = "Alice" });
        var getter = new UserProfileGetter("user123", httpClient);

        // Act
        var result = await getter.Get();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Alice", result.Value!.Name);
    }

    [Fact]
    public async Task Get_InvalidUser_ReturnsFailure()
    {
        // Arrange
        var httpClient = CreateMockClient404();
        var getter = new UserProfileGetter("invalid", httpClient);

        // Act
        var result = await getter.Get();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("404", result.Error);
    }

    [Fact]
    public async Task GetIfNeeded_SecondCall_UsesCachedValue()
    {
        // Arrange
        int callCount = 0;
        var getter = new GetterRxO<Data>(ct =>
        {
            callCount++;
            return Task.FromResult(GetResult.Success(new Data()));
        });

        // Act
        await getter.GetIfNeeded();  // Call 1
        await getter.GetIfNeeded();  // Call 2 (cached)

        // Assert
        Assert.Equal(1, callCount);  // Only called once
    }
}
```

---

## Summary

**Implementing Custom Getters:**

1. **Stateless** (`IStatelessGetter<T>`) - Always fetch fresh data
2. **Cached** (`GetterRxO<T>`) - Lazy loading with caching
3. **Database** - EF Core integration
4. **Composite** - Multiple fallback sources
5. **Factory** - Dynamic getter creation

**Key Points:**
- Always return `GetResult<T>`
- Support cancellation tokens
- Handle errors gracefully
- Use `GetterRxO<T>` for reactive properties

**Related Guides:**
- [Implement Caching](implement-caching.md)
- [Getting Started: Async Data](../getting-started/02-async-data.md)
- [Async Data Domain Docs](../../data/async/README.md)
