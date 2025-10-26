# Reactive Patterns

## Overview

This guide covers reactive programming patterns used in LionFire's MVVM implementation. Understanding these patterns is essential for building responsive, performant applications with ReactiveUI.

**Key Concept**: Reactive programming treats data and events as **streams** that you can observe, transform, and combine.

---

## Table of Contents

1. [Reactive Programming Basics](#reactive-programming-basics)
2. [WhenAnyValue - Property Observables](#whenanyvalue---property-observables)
3. [Observable Subscriptions](#observable-subscriptions)
4. [Throttling and Debouncing](#throttling-and-debouncing)
5. [Combining Observables](#combining-observables)
6. [Subscription Management](#subscription-management)
7. [Common Patterns](#common-patterns)
8. [Performance Optimization](#performance-optimization)

---

## Reactive Programming Basics

### The Observable Pattern

**Traditional Event Handling**:
```csharp
// ❌ Old way - manual event handling
public event PropertyChangedEventHandler? PropertyChanged;

someObject.PropertyChanged += (sender, e) => {
    if (e.PropertyName == "Name")
    {
        UpdateUI();
    }
};
```

**Reactive Programming**:
```csharp
// ✅ Reactive way - observable stream
someObject.WhenAnyValue(x => x.Name)
    .Subscribe(name => UpdateUI());
```

**Benefits**:
- Type-safe (compile-time errors)
- Composable (chain operations)
- Declarative (what, not how)
- Easier to reason about

---

### Observable Streams

Think of observables as **streams of values over time**:

```
Name property changes over time:
    "Bot1" → "Bot2" → "Bot3" → ...
        ↓
WhenAnyValue(x => x.Name)
        ↓
Subscribe(name => Console.WriteLine(name))
        ↓
Output: "Bot1", "Bot2", "Bot3", ...
```

---

## WhenAnyValue - Property Observables

### Basic Usage

**Single Property**:
```csharp
this.WhenAnyValue(x => x.Value.Name)
    .Subscribe(name => {
        Console.WriteLine($"Name changed to: {name}");
    });
```

**Multiple Properties**:
```csharp
this.WhenAnyValue(
        x => x.Value.Name,
        x => x.Value.Description)
    .Subscribe(tuple => {
        var (name, description) = tuple;
        Console.WriteLine($"{name}: {description}");
    });
```

**With Selector**:
```csharp
this.WhenAnyValue(
        x => x.Value.FirstName,
        x => x.Value.LastName,
        (first, last) => $"{first} {last}")
    .Subscribe(fullName => {
        Console.WriteLine($"Full name: {fullName}");
    });
```

---

### WhenAnyValue with Commands

**Enable/Disable Commands**:
```csharp
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value)
    {
        // Command enabled only when bot is not running
        StartCommand = ReactiveCommand.CreateFromTask(
            StartAsync,
            this.WhenAnyValue(x => x.Value.Enabled, enabled => !enabled)
        );

        // Command enabled only when bot is running
        StopCommand = ReactiveCommand.CreateFromTask(
            StopAsync,
            this.WhenAnyValue(x => x.Value.Enabled)
        );
    }

    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; }
}
```

**Binding to UI**:
```razor
<MudButton Command="@vm.StartCommand"
           Disabled="@(!vm.StartCommand.CanExecute.Value)">
    Start
</MudButton>
<MudButton Command="@vm.StopCommand"
           Disabled="@(!vm.StopCommand.CanExecute.Value)">
    Stop
</MudButton>
```

---

### ObservableAsPropertyHelper

**Computed Properties** (cached, observable):

```csharp
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value)
    {
        // Reactive computed property
        StatusText = this.WhenAnyValue(x => x.Value.Enabled)
            .Select(enabled => enabled ? "Running" : "Stopped")
            .ToProperty(this, x => x.StatusText);

        // Multiple properties
        DisplayName = this.WhenAnyValue(
                x => x.Value.Name,
                x => x.Key,
                (name, key) => $"{name} ({key})")
            .ToProperty(this, x => x.DisplayName);
    }

    // Reactive properties (read-only)
    public string StatusText => statusText.Value;
    private readonly ObservableAsPropertyHelper<string> statusText;

    public string DisplayName => displayName.Value;
    private readonly ObservableAsPropertyHelper<string> displayName;
}
```

**Usage**:
```razor
<MudText>@vm.StatusText</MudText>
<MudText>@vm.DisplayName</MudText>
```

**Benefits**:
- Automatically updates when dependencies change
- Cached (doesn't recompute unnecessarily)
- Type-safe
- Observable (can be subscribed to)

---

## Observable Subscriptions

### Basic Subscription

```csharp
IDisposable subscription = this.WhenAnyValue(x => x.Value.Name)
    .Subscribe(name => {
        Console.WriteLine($"Name: {name}");
    });

// Later: clean up
subscription.Dispose();
```

---

### Subscribe with Error Handling

```csharp
this.WhenAnyValue(x => x.Value.Name)
    .Subscribe(
        onNext: name => Console.WriteLine($"Name: {name}"),
        onError: ex => Console.WriteLine($"Error: {ex.Message}"),
        onCompleted: () => Console.WriteLine("Completed")
    );
```

---

### Conditional Subscriptions

**Where** (Filter):
```csharp
this.WhenAnyValue(x => x.Value.Enabled)
    .Where(enabled => enabled)  // Only when enabled
    .Subscribe(_ => {
        Console.WriteLine("Bot enabled!");
    });
```

**DistinctUntilChanged** (Avoid duplicates):
```csharp
this.WhenAnyValue(x => x.Value.Status)
    .DistinctUntilChanged()  // Only when value actually changes
    .Subscribe(status => {
        Console.WriteLine($"Status changed to: {status}");
    });
```

---

### Transformations

**Select** (Map):
```csharp
this.WhenAnyValue(x => x.Value.ProfitLoss)
    .Select(pl => pl.ToString("C2"))  // Format as currency
    .Subscribe(formatted => {
        Console.WriteLine($"P/L: {formatted}");
    });
```

**SelectMany** (Flatten):
```csharp
this.WhenAnyValue(x => x.Value.Orders)
    .SelectMany(orders => orders)  // Flatten collection
    .Subscribe(order => {
        Console.WriteLine($"Order: {order.Id}");
    });
```

---

## Throttling and Debouncing

### Throttle

**Wait for quiet period before emitting**:

```csharp
// Wait 500ms after last change before updating
this.WhenAnyValue(x => x.SearchText)
    .Throttle(TimeSpan.FromMilliseconds(500))
    .Subscribe(async searchText => {
        await PerformSearch(searchText);
    });
```

**Use Case**: Search-as-you-type (avoid excessive API calls).

**Diagram**:
```
User types: a → ab → abc → abcd
             ↓    ↓    ↓     ↓
Throttle(500ms):          [wait 500ms]
                                ↓
                             "abcd"
```

---

### Debounce (Sample)

**Emit at regular intervals**:

```csharp
// Update at most once per second
this.WhenAnyValue(x => x.Value.Price)
    .Sample(TimeSpan.FromSeconds(1))
    .Subscribe(price => {
        UpdateChart(price);
    });
```

**Use Case**: Real-time price updates (avoid UI flooding).

**Diagram**:
```
Price changes: $100 → $101 → $102 → $103 → $104
                ↓      ↓      ↓      ↓      ↓
Sample(1s):   $100          $103          $104
```

---

### Distinct Until Changed

**Only emit when value actually changes**:

```csharp
this.WhenAnyValue(x => x.Value.Status)
    .DistinctUntilChanged()
    .Subscribe(status => {
        Console.WriteLine($"Status changed: {status}");
    });
```

**Without DistinctUntilChanged**:
```
Status: Active → Active → Active → Inactive
        ↓        ↓        ↓        ↓
Output: Active, Active, Active, Inactive  (3 duplicates!)
```

**With DistinctUntilChanged**:
```
Status: Active → Active → Active → Inactive
        ↓                           ↓
Output: Active, Inactive  (duplicates removed)
```

---

## Combining Observables

### CombineLatest

**Combine multiple streams, emit when any changes**:

```csharp
this.WhenAnyValue(x => x.FirstName)
    .CombineLatest(
        this.WhenAnyValue(x => x.LastName),
        (first, last) => $"{first} {last}")
    .Subscribe(fullName => {
        Console.WriteLine(fullName);
    });
```

**Diagram**:
```
FirstName: "John"  → "Jane"
LastName:  "Doe"   → "Smith"
           ↓         ↓
Output:   "John Doe" → "Jane Doe" → "Jane Smith"
```

---

### Merge

**Merge multiple streams into one**:

```csharp
var stream1 = this.WhenAnyValue(x => x.Value1);
var stream2 = this.WhenAnyValue(x => x.Value2);

stream1.Merge(stream2)
    .Subscribe(value => {
        Console.WriteLine($"Value changed: {value}");
    });
```

**Use Case**: React to changes from multiple sources.

---

### Zip

**Pair values from two streams**:

```csharp
var quantities = this.WhenAnyValue(x => x.Quantity);
var prices = this.WhenAnyValue(x => x.Price);

quantities.Zip(prices, (qty, price) => qty * price)
    .Subscribe(total => {
        Console.WriteLine($"Total: ${total}");
    });
```

**Diagram**:
```
Quantities: 10 → 20 → 30
Prices:     $5 → $6 → $7
            ↓    ↓    ↓
Output:     $50, $120, $210
```

---

### StartWith

**Provide initial value**:

```csharp
this.WhenAnyValue(x => x.Value.Name)
    .StartWith("<Not Set>")  // Initial value
    .Subscribe(name => {
        Console.WriteLine($"Name: {name}");
    });
```

**Output**:
```
Name: <Not Set>
Name: Bot1
Name: Bot2
```

---

## Subscription Management

### CompositeDisposable

**Dispose multiple subscriptions at once**:

```csharp
public class MyVM : ReactiveObject, IDisposable
{
    private readonly CompositeDisposable subscriptions = new();

    public MyVM()
    {
        this.WhenAnyValue(x => x.Property1)
            .Subscribe(...)
            .DisposeWith(subscriptions);

        this.WhenAnyValue(x => x.Property2)
            .Subscribe(...)
            .DisposeWith(subscriptions);

        this.WhenAnyValue(x => x.Property3)
            .Subscribe(...)
            .DisposeWith(subscriptions);
    }

    public void Dispose()
    {
        subscriptions.Dispose();  // Disposes all subscriptions
    }
}
```

---

### Automatic Disposal (DisposeWith)

```csharp
// Add to composite
someObservable.Subscribe(...).DisposeWith(subscriptions);

// Dispose when condition met
someObservable
    .Subscribe(...)
    .DisposeWith(this);  // Dispose when 'this' is disposed
```

---

### Unsubscribing

```csharp
// Manual unsubscribe
var subscription = observable.Subscribe(...);
subscription.Dispose();  // Stop receiving notifications

// Automatic via using
using (observable.Subscribe(...))
{
    // Subscription active here
}
// Automatically disposed
```

---

## Common Patterns

### Pattern 1: Form Validation

```csharp
public class LoginVM : ReactiveObject
{
    [Reactive] private string? _username;
    [Reactive] private string? _password;

    public LoginVM()
    {
        // Valid when both fields have values
        var canLogin = this.WhenAnyValue(
                x => x.Username,
                x => x.Password,
                (user, pass) => !string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
            .StartWith(false);

        LoginCommand = ReactiveCommand.CreateFromTask(
            LoginAsync,
            canLogin
        );
    }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    private async Task LoginAsync()
    {
        // Login logic
    }
}
```

---

### Pattern 2: Computed Summary

```csharp
public class CartVM : ReactiveObject
{
    [Reactive] private ObservableCollection<CartItem> _items;

    public CartVM()
    {
        // Recompute total whenever items change
        Total = this.WhenAnyValue(x => x.Items)
            .Select(items => items?.Sum(i => i.Price * i.Quantity) ?? 0)
            .ToProperty(this, x => x.Total);

        ItemCount = this.WhenAnyValue(x => x.Items)
            .Select(items => items?.Sum(i => i.Quantity) ?? 0)
            .ToProperty(this, x => x.ItemCount);
    }

    public decimal Total => total.Value;
    private readonly ObservableAsPropertyHelper<decimal> total;

    public int ItemCount => itemCount.Value;
    private readonly ObservableAsPropertyHelper<int> itemCount;
}
```

---

### Pattern 3: Dependent Commands

```csharp
public class OrderVM : ReactiveObject
{
    [Reactive] private bool _isProcessing;
    [Reactive] private Order? _order;

    public OrderVM()
    {
        // Save enabled when not processing and order exists
        var canSave = this.WhenAnyValue(
                x => x.IsProcessing,
                x => x.Order,
                (processing, order) => !processing && order != null);

        SaveCommand = ReactiveCommand.CreateFromTask(
            SaveAsync,
            canSave
        );

        // Cancel enabled only when processing
        CancelCommand = ReactiveCommand.Create(
            Cancel,
            this.WhenAnyValue(x => x.IsProcessing)
        );
    }

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
}
```

---

### Pattern 4: Cascading Updates

```csharp
public class AddressVM : ReactiveObject
{
    [Reactive] private string? _zipCode;
    [Reactive] private string? _city;
    [Reactive] private string? _state;

    public AddressVM()
    {
        // When zip code changes, auto-fill city and state
        this.WhenAnyValue(x => x.ZipCode)
            .Where(zip => !string.IsNullOrEmpty(zip))
            .Throttle(TimeSpan.FromMilliseconds(500))
            .SelectMany(zip => LookupZipCode(zip))
            .Subscribe(location => {
                City = location.City;
                State = location.State;
            });
    }

    private async Task<Location> LookupZipCode(string zipCode)
    {
        // API call
        return await zipCodeService.Lookup(zipCode);
    }
}
```

---

### Pattern 5: Master-Detail Synchronization

```csharp
public class MasterDetailVM : ReactiveObject
{
    [Reactive] private string? _selectedId;
    [Reactive] private BotEntity? _selectedBot;

    public MasterDetailVM(IObservableReader<string, BotEntity> reader)
    {
        // When selected ID changes, load bot
        this.WhenAnyValue(x => x.SelectedId)
            .Where(id => id != null)
            .SelectMany(async id => {
                var result = await reader.TryGetValue(id);
                return result.HasValue ? result.Value : null;
            })
            .Subscribe(bot => SelectedBot = bot);
    }
}
```

---

## Performance Optimization

### Issue 1: Excessive Subscriptions

```csharp
// ❌ Bad - Creates new subscription on every render
protected override void OnParametersSet()
{
    vm.WhenAnyValue(x => x.Value)
        .Subscribe(_ => StateHasChanged());
}

// ✅ Good - Single subscription
private IDisposable? subscription;

protected override void OnInitialized()
{
    subscription = vm.WhenAnyValue(x => x.Value)
        .Subscribe(_ => InvokeAsync(StateHasChanged));
}

public async ValueTask DisposeAsync()
{
    subscription?.Dispose();
}
```

---

### Issue 2: Memory Leaks

```csharp
// ❌ Bad - Subscription never disposed
public MyVM()
{
    observable.Subscribe(...);  // LEAK!
}

// ✅ Good - Proper disposal
private readonly CompositeDisposable subscriptions = new();

public MyVM()
{
    observable.Subscribe(...)
        .DisposeWith(subscriptions);
}

public void Dispose() => subscriptions.Dispose();
```

---

### Issue 3: Expensive Computations

```csharp
// ❌ Bad - Recomputes on every property access
public string ExpensiveComputation =>
    ComputeExpensiveValue(Value.Data);  // Called every time!

// ✅ Good - Cached with ObservableAsPropertyHelper
ExpensiveResult = this.WhenAnyValue(x => x.Value.Data)
    .Select(data => ComputeExpensiveValue(data))
    .ToProperty(this, x => x.ExpensiveResult);

public string ExpensiveResult => expensiveResult.Value;
```

---

### Issue 4: Chatty Updates

```csharp
// ❌ Bad - Fires on every keystroke
searchBox.WhenAnyValue(x => x.Text)
    .Subscribe(async text => await Search(text));  // Too many API calls!

// ✅ Good - Debounced
searchBox.WhenAnyValue(x => x.Text)
    .Throttle(TimeSpan.FromMilliseconds(500))
    .DistinctUntilChanged()
    .Subscribe(async text => await Search(text));
```

---

## Related Documentation

- **[MVVM Overview](README.md)** - MVVM architecture overview
- **[ViewModels Guide](viewmodels-guide.md)** - ViewModel patterns
- **[Data Binding](data-binding.md)** - UI binding patterns
- **[Reactive UI Updates](../ui/reactive-ui-updates.md)** - UI update flow
- **[ReactiveUI Documentation](https://www.reactiveui.net/)** - Official ReactiveUI docs

---

## Summary

**Key Reactive Patterns**:

| Pattern | Use Case | Example |
|---------|----------|---------|
| **WhenAnyValue** | Observe property changes | `vm.WhenAnyValue(x => x.Name)` |
| **Throttle** | Debounce user input | `.Throttle(TimeSpan.FromMilliseconds(500))` |
| **DistinctUntilChanged** | Avoid duplicate events | `.DistinctUntilChanged()` |
| **CombineLatest** | Combine multiple properties | `.CombineLatest(...)` |
| **ToProperty** | Computed properties | `.ToProperty(this, x => x.FullName)` |
| **DisposeWith** | Subscription management | `.DisposeWith(subscriptions)` |
| **ReactiveCommand** | Commands with can-execute | `ReactiveCommand.Create(..., canExecute)` |

**Best Practices**:
1. Always dispose subscriptions
2. Use `DistinctUntilChanged` to avoid duplicates
3. Throttle user input operations
4. Use `ToProperty` for computed values
5. Keep subscriptions in `CompositeDisposable`

**Performance Tips**:
- Cache expensive computations with `ToProperty`
- Throttle/debounce high-frequency events
- Dispose subscriptions properly
- Avoid creating subscriptions in render methods
