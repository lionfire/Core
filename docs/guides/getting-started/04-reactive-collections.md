# Getting Started: Reactive Collections

## Overview

This guide introduces **DynamicData** - a powerful reactive collections library that provides observable collections with change tracking, transformations, and efficient updates. Learn how to build reactive data pipelines that automatically update your UI when data changes.

**What You'll Learn**:
- Working with `IObservableCache<T, TKey>`
- Transforming and filtering collections
- Binding to UI collections
- File-based reactive collections
- On-demand resource activation

**Prerequisites**:
- .NET 9.0+ SDK
- Completed [03-mvvm-basics.md](03-mvvm-basics.md) (recommended)
- Basic understanding of LINQ and observables

---

## Setup

### 1. Create a New Console Project

```bash
dotnet new console -n LionFireReactiveCollections
cd LionFireReactiveCollections
```

### 2. Add Required Packages

```bash
dotnet add package LionFire.Reactive
dotnet add package DynamicData
dotnet add package ReactiveUI
```

### 3. Update Program.cs

```csharp
using DynamicData;
using LionFire.Reactive;
using System.Reactive.Linq;

Console.WriteLine("LionFire Reactive Collections");
Console.WriteLine("==============================\n");

// Your code here
```

---

## Your First Observable Collection

Let's start with a simple observable collection using DynamicData.

### Example: Product Catalog

```csharp
using DynamicData;

// Step 1: Define your model
public record Product(string Id, string Name, decimal Price, string Category);

// Step 2: Create a SourceCache
var products = new SourceCache<Product, string>(p => p.Id);

// Step 3: Subscribe to changes
products.Connect()
    .Subscribe(changeSet =>
    {
        Console.WriteLine($"Changes received: {changeSet.Count} items");

        foreach (var change in changeSet)
        {
            Console.WriteLine($"  {change.Reason}: {change.Key}");

            if (change.Reason == ChangeReason.Add || change.Reason == ChangeReason.Update)
            {
                var product = change.Current;
                Console.WriteLine($"    {product.Name} - ${product.Price}");
            }
        }
    });

// Step 4: Add items
products.AddOrUpdate(new Product("1", "Laptop", 999.99m, "Electronics"));
products.AddOrUpdate(new Product("2", "Mouse", 29.99m, "Electronics"));
products.AddOrUpdate(new Product("3", "Desk", 299.99m, "Furniture"));

// Output:
// Changes received: 3 items
//   Add: 1
//     Laptop - $999.99
//   Add: 2
//     Mouse - $29.99
//   Add: 3
//     Desk - $299.99
```

**Key Concepts**:
- `SourceCache<T, TKey>` - Observable cache with key selector
- `Connect()` - Subscribe to changes
- `IChangeSet<T, TKey>` - Batched change notifications
- `ChangeReason` - Add, Update, Remove, Refresh

---

## Transforming Collections

Transform items as they flow through the pipeline.

### Example: Product to ViewModel

```csharp
using DynamicData;
using ReactiveUI;

public record Product(string Id, string Name, decimal Price, string Category);

public class ProductVM : ReactiveObject
{
    public string Id { get; }
    public string Name { get; }
    public string FormattedPrice { get; }
    public string Category { get; }

    public ProductVM(Product product)
    {
        Id = product.Id;
        Name = product.Name;
        FormattedPrice = $"${product.Price:F2}";
        Category = product.Category;
    }
}

// Create source cache
var products = new SourceCache<Product, string>(p => p.Id);

// Transform to ViewModels
products.Connect()
    .Transform(product => new ProductVM(product))
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            if (change.Reason == ChangeReason.Add)
            {
                var vm = change.Current;
                Console.WriteLine($"VM Created: {vm.Name} - {vm.FormattedPrice}");
            }
        }
    });

// Add products
products.AddOrUpdate(new Product("1", "Laptop", 999.99m, "Electronics"));
products.AddOrUpdate(new Product("2", "Mouse", 29.99m, "Electronics"));

// Output:
// VM Created: Laptop - $999.99
// VM Created: Mouse - $29.99
```

**Transform Benefits**:
- Automatic VM creation
- Automatic VM disposal (with `DisposeMany()`)
- Lazy evaluation
- Memory efficient

---

## Filtering Collections

Filter items based on predicates that can change dynamically.

### Example: Category Filter

```csharp
using DynamicData;
using System.Reactive.Subjects;

public record Product(string Id, string Name, decimal Price, string Category);

var products = new SourceCache<Product, string>(p => p.Id);

// Observable filter predicate
var categoryFilter = new BehaviorSubject<string?>(null);

// Create filtered observable
var filtered = products.Connect()
    .Filter(categoryFilter.Select(category =>
        new Func<Product, bool>(p =>
            category == null || p.Category == category)))
    .Subscribe(changeSet =>
    {
        Console.WriteLine($"Filtered results: {changeSet.Count} changes");

        foreach (var change in changeSet)
        {
            if (change.Reason == ChangeReason.Add)
                Console.WriteLine($"  + {change.Current.Name}");
            else if (change.Reason == ChangeReason.Remove)
                Console.WriteLine($"  - {change.Current.Name}");
        }
    });

// Add products
products.AddOrUpdate(new Product("1", "Laptop", 999m, "Electronics"));
products.AddOrUpdate(new Product("2", "Desk", 299m, "Furniture"));
products.AddOrUpdate(new Product("3", "Mouse", 29m, "Electronics"));

// Change filter to Electronics
Console.WriteLine("\nFiltering to Electronics:");
categoryFilter.OnNext("Electronics");

// Change filter to Furniture
Console.WriteLine("\nFiltering to Furniture:");
categoryFilter.OnNext("Furniture");

// Clear filter
Console.WriteLine("\nClearing filter:");
categoryFilter.OnNext(null);

// Output:
// Filtered results: 3 changes
//   + Laptop
//   + Desk
//   + Mouse
//
// Filtering to Electronics:
// Filtered results: 1 changes
//   - Desk
//
// Filtering to Furniture:
// Filtered results: 2 changes
//   - Laptop
//   - Mouse
//   + Desk
//
// Clearing filter:
// Filtered results: 2 changes
//   + Laptop
//   + Mouse
```

---

## Sorting Collections

Sort collections reactively with changeable sort criteria.

### Example: Price Sorting

```csharp
using DynamicData;
using DynamicData.Binding;

public record Product(string Id, string Name, decimal Price, string Category);

var products = new SourceCache<Product, string>(p => p.Id);

// Sort by price
products.Connect()
    .Sort(SortExpressionComparer<Product>.Ascending(p => p.Price))
    .Subscribe(changeSet =>
    {
        Console.WriteLine("Sorted products:");
        foreach (var change in changeSet)
        {
            if (change.Reason == ChangeReason.Add)
            {
                var p = change.Current;
                Console.WriteLine($"  {p.Name} - ${p.Price}");
            }
        }
    });

// Add products (unsorted)
products.AddOrUpdate(new Product("1", "Laptop", 999m, "Electronics"));
products.AddOrUpdate(new Product("2", "Mouse", 29m, "Electronics"));
products.AddOrUpdate(new Product("3", "Desk", 299m, "Furniture"));

// Output:
// Sorted products:
//   Mouse - $29
//   Desk - $299
//   Laptop - $999
```

---

## Binding to Observable Collections

Bind DynamicData to `ObservableCollection<T>` for UI frameworks.

### Example: UI-Ready Collection

```csharp
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;

public record Product(string Id, string Name, decimal Price, string Category);

var products = new SourceCache<Product, string>(p => p.Id);

// Bind to ObservableCollection
ReadOnlyObservableCollection<Product> boundCollection;

var subscription = products.Connect()
    .Filter(p => p.Category == "Electronics")
    .Sort(SortExpressionComparer<Product>.Ascending(p => p.Name))
    .Bind(out boundCollection)
    .Subscribe();

// Add products
products.AddOrUpdate(new Product("1", "Laptop", 999m, "Electronics"));
products.AddOrUpdate(new Product("2", "Desk", 299m, "Furniture"));
products.AddOrUpdate(new Product("3", "Mouse", 29m, "Electronics"));
products.AddOrUpdate(new Product("4", "Keyboard", 79m, "Electronics"));

// boundCollection is now sorted Electronics only
Console.WriteLine($"Bound collection has {boundCollection.Count} items:");
foreach (var product in boundCollection)
{
    Console.WriteLine($"  {product.Name}");
}

// Output:
// Bound collection has 3 items:
//   Keyboard
//   Laptop
//   Mouse
```

**Key Points**:
- `Bind(out var collection)` creates `ReadOnlyObservableCollection<T>`
- Perfect for WPF, Avalonia, Blazor data binding
- Automatically synchronized
- Efficient incremental updates

---

## File-Based Observable Collections

Watch files and deserialize into reactive collections.

### Example: Configuration Files

```csharp
using LionFire.Reactive;
using DynamicData;
using System.Text.Json;

public record BotConfig(string Name, bool Enabled, int Interval);

// Watch directory for JSON files
var configDir = "./configs";
Directory.CreateDirectory(configDir);

var configs = ObservableFsDocuments.Create<BotConfig>(
    dir: configDir,
    deserialize: bytes => JsonSerializer.Deserialize<BotConfig>(bytes)!
).AsObservableCache();

// Subscribe to changes
configs.Connect()
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            Console.WriteLine($"{change.Reason}: {change.Key}");

            if (change.Reason == ChangeReason.Add || change.Reason == ChangeReason.Update)
            {
                var config = change.Current.Value;
                Console.WriteLine($"  {config.Name} - Enabled: {config.Enabled}");
            }
        }
    });

// Create config files
await File.WriteAllTextAsync(
    Path.Combine(configDir, "bot1.json"),
    JsonSerializer.Serialize(new BotConfig("Bot 1", true, 5000))
);

await File.WriteAllTextAsync(
    Path.Combine(configDir, "bot2.json"),
    JsonSerializer.Serialize(new BotConfig("Bot 2", false, 10000))
);

// Wait for file watcher to detect changes
await Task.Delay(1500);

// Update a file
await File.WriteAllTextAsync(
    Path.Combine(configDir, "bot1.json"),
    JsonSerializer.Serialize(new BotConfig("Bot 1 Updated", true, 3000))
);

await Task.Delay(1500);

// Output:
// Add: bot1.json
//   Bot 1 - Enabled: True
// Add: bot2.json
//   Bot 2 - Enabled: False
// Update: bot1.json
//   Bot 1 Updated - Enabled: True
```

**Key Features**:
- Automatic file watching (polls every 1 second)
- On-demand deserialization
- Handles file adds, updates, and removes
- Works with any file format

---

## Chaining Operations

Combine multiple operations into powerful pipelines.

### Example: Complete Product Pipeline

```csharp
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;

public record Product(string Id, string Name, decimal Price, string Category, bool InStock);

public class ProductVM
{
    public string Name { get; }
    public string FormattedPrice { get; }
    public string Status { get; }

    public ProductVM(Product p)
    {
        Name = p.Name;
        FormattedPrice = $"${p.Price:F2}";
        Status = p.InStock ? "In Stock" : "Out of Stock";
    }
}

var products = new SourceCache<Product, string>(p => p.Id);
var categoryFilter = new BehaviorSubject<string?>(null);
var showOutOfStock = new BehaviorSubject<bool>(true);

ReadOnlyObservableCollection<ProductVM> displayProducts;

var pipeline = products.Connect()
    // Filter by category
    .Filter(categoryFilter.Select(cat =>
        new Func<Product, bool>(p => cat == null || p.Category == cat)))
    // Filter by stock status
    .Filter(showOutOfStock.Select(show =>
        new Func<Product, bool>(p => show || p.InStock)))
    // Sort by name
    .Sort(SortExpressionComparer<Product>.Ascending(p => p.Name))
    // Transform to ViewModels
    .Transform(p => new ProductVM(p))
    // Bind to collection
    .Bind(out displayProducts)
    .Subscribe();

// Add products
products.AddOrUpdate(new Product("1", "Laptop", 999m, "Electronics", true));
products.AddOrUpdate(new Product("2", "Mouse", 29m, "Electronics", false));
products.AddOrUpdate(new Product("3", "Desk", 299m, "Furniture", true));
products.AddOrUpdate(new Product("4", "Chair", 199m, "Furniture", true));

Console.WriteLine($"All products: {displayProducts.Count}");

// Filter to Electronics
categoryFilter.OnNext("Electronics");
Console.WriteLine($"Electronics only: {displayProducts.Count}");

// Hide out of stock
showOutOfStock.OnNext(false);
Console.WriteLine($"In stock Electronics: {displayProducts.Count}");

foreach (var vm in displayProducts)
{
    Console.WriteLine($"  {vm.Name} - {vm.FormattedPrice} - {vm.Status}");
}

// Output:
// All products: 4
// Electronics only: 2
// In stock Electronics: 1
//   Laptop - $999.00 - In Stock
```

---

## On-Demand Activation

Only activate expensive resources when someone is watching.

### Example: Lazy Database Query

```csharp
using LionFire.Reactive;
using DynamicData;

public record User(string Id, string Name);

// Create on-demand observable
var users = IObservableX.CreateConnectOnDemand<User, string>(
    keySelector: u => u.Id,
    resourceFactory: cache =>
    {
        Console.WriteLine("🟢 Database connection opened");

        // Simulate database polling
        var timer = new System.Timers.Timer(2000);
        timer.Elapsed += async (s, e) =>
        {
            Console.WriteLine("📊 Querying database...");

            // Simulate DB query
            await Task.Delay(500);

            var users = new[]
            {
                new User("1", $"User {DateTime.Now.Second}"),
                new User("2", $"Admin {DateTime.Now.Second}")
            };

            cache.AddOrUpdate(users);
        };

        timer.Start();

        // Return cleanup action
        return Disposable.Create(() =>
        {
            Console.WriteLine("🔴 Database connection closed");
            timer.Stop();
            timer.Dispose();
        });
    }
);

Console.WriteLine("Observable created, but no database connection yet\n");

// Subscribe (activates resource)
Console.WriteLine("Subscribing...");
var subscription = users.Subscribe(changeSet =>
{
    Console.WriteLine($"Received {changeSet.Count} users");
});

await Task.Delay(5000);

// Unsubscribe (deactivates resource)
Console.WriteLine("\nUnsubscribing...");
subscription.Dispose();

await Task.Delay(1000);

// Output:
// Observable created, but no database connection yet
//
// Subscribing...
// 🟢 Database connection opened
// 📊 Querying database...
// Received 2 users
// 📊 Querying database...
// Received 2 users
//
// Unsubscribing...
// 🔴 Database connection closed
```

---

## Practical Example: Real-Time Product Dashboard

```csharp
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;

public record Product(string Id, string Name, decimal Price, string Category, int Stock);

public class ProductDashboard
{
    private readonly SourceCache<Product, string> products;
    private readonly BehaviorSubject<string> searchText = new("");
    private readonly BehaviorSubject<string?> categoryFilter = new(null);

    public ReadOnlyObservableCollection<Product> FilteredProducts { get; }
    public ReadOnlyObservableCollection<string> Categories { get; }

    public int TotalProducts { get; private set; }
    public decimal TotalValue { get; private set; }
    public int LowStockCount { get; private set; }

    public ProductDashboard()
    {
        products = new SourceCache<Product, string>(p => p.Id);

        // Filtered products pipeline
        products.Connect()
            .Filter(searchText.Select(text =>
                new Func<Product, bool>(p =>
                    string.IsNullOrEmpty(text) ||
                    p.Name.Contains(text, StringComparison.OrdinalIgnoreCase))))
            .Filter(categoryFilter.Select(cat =>
                new Func<Product, bool>(p =>
                    cat == null || p.Category == cat)))
            .Sort(SortExpressionComparer<Product>.Ascending(p => p.Name))
            .Bind(out var filtered)
            .Subscribe();

        FilteredProducts = filtered;

        // Categories list
        products.Connect()
            .Transform(p => p.Category)
            .DistinctValues(c => c)
            .Sort(SortExpressionComparer<string>.Ascending(c => c))
            .Bind(out var categories)
            .Subscribe();

        Categories = categories;

        // Statistics
        products.Connect()
            .Subscribe(_ => UpdateStatistics());
    }

    public void AddProduct(Product product) =>
        products.AddOrUpdate(product);

    public void UpdateStock(string id, int newStock)
    {
        var product = products.Lookup(id);
        if (product.HasValue)
        {
            var updated = product.Value with { Stock = newStock };
            products.AddOrUpdate(updated);
        }
    }

    public void Search(string text) =>
        searchText.OnNext(text);

    public void FilterByCategory(string? category) =>
        categoryFilter.OnNext(category);

    private void UpdateStatistics()
    {
        var items = products.Items.ToList();
        TotalProducts = items.Count;
        TotalValue = items.Sum(p => p.Price * p.Stock);
        LowStockCount = items.Count(p => p.Stock < 10);

        Console.WriteLine($"\n📊 Dashboard Stats:");
        Console.WriteLine($"   Total Products: {TotalProducts}");
        Console.WriteLine($"   Total Value: ${TotalValue:F2}");
        Console.WriteLine($"   Low Stock: {LowStockCount}");
    }
}

// Usage
var dashboard = new ProductDashboard();

// Add products
dashboard.AddProduct(new Product("1", "Laptop", 999m, "Electronics", 5));
dashboard.AddProduct(new Product("2", "Mouse", 29m, "Electronics", 50));
dashboard.AddProduct(new Product("3", "Desk", 299m, "Furniture", 15));
dashboard.AddProduct(new Product("4", "Chair", 199m, "Furniture", 8));

Console.WriteLine($"\nCategories: {string.Join(", ", dashboard.Categories)}");

// Search
Console.WriteLine("\nSearching for 'Laptop':");
dashboard.Search("Laptop");
Console.WriteLine($"Results: {dashboard.FilteredProducts.Count}");

// Filter by category
Console.WriteLine("\nFiltering to Furniture:");
dashboard.Search("");
dashboard.FilterByCategory("Furniture");
Console.WriteLine($"Results: {dashboard.FilteredProducts.Count}");

// Update stock
dashboard.UpdateStock("3", 25);
```

---

## Best Practices

### 1. Use SourceCache for Keyed Data

```csharp
// ✅ Good - Keyed collection
var products = new SourceCache<Product, string>(p => p.Id);

// ❌ Avoid - SourceList (unkeyed) when you have keys
var products = new SourceList<Product>();
```

### 2. Dispose Transformations

```csharp
// ✅ Good - Dispose ViewModels when removed
products.Connect()
    .Transform(p => new ProductVM(p))
    .DisposeMany()  // Important!
    .Subscribe();

// ❌ Avoid - Memory leak
products.Connect()
    .Transform(p => new ProductVM(p))
    .Subscribe();
```

### 3. Use Bind for UI Collections

```csharp
// ✅ Good - Automatic synchronization
ReadOnlyObservableCollection<Product> collection;
products.Connect().Bind(out collection).Subscribe();

// ❌ Avoid - Manual collection management
var collection = new ObservableCollection<Product>();
products.Connect().Subscribe(changeSet => {
    // Manual add/remove logic - error prone!
});
```

### 4. Filter with Observable Predicates

```csharp
// ✅ Good - Dynamic filtering
var filter = new BehaviorSubject<string?>(null);
products.Connect()
    .Filter(filter.Select(f => new Func<Product, bool>(p => ...)))
    .Subscribe();

// ❌ Avoid - Static filter
products.Connect()
    .Filter(p => p.Category == "Electronics")  // Can't change!
    .Subscribe();
```

---

## Summary

**DynamicData** provides:

**Core Features**:
- `SourceCache<T, TKey>` - Observable keyed collections
- `Transform()` - Map items to different types
- `Filter()` - Dynamic filtering with predicates
- `Sort()` - Reactive sorting
- `Bind()` - Synchronize with UI collections

**Benefits**:
- Efficient change tracking
- Composable operators
- Automatic cleanup
- Memory efficient
- UI-ready collections

**Next Steps**:
1. Read [05-persistence.md](05-persistence.md) for file-based collections
2. Explore [Reactive Domain Docs](../../reactive/README.md) for advanced patterns
3. Review [DynamicData Documentation](https://github.com/reactivemarbles/DynamicData)

---

## Exercise

Build a task manager dashboard that:
1. Stores tasks in a `SourceCache`
2. Filters tasks by completion status
3. Groups tasks by priority
4. Sorts by due date
5. Binds to an `ObservableCollection` for UI
6. Calculates statistics (completed count, overdue count)

Use the patterns from this guide!
