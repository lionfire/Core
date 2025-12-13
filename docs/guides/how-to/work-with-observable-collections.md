# How-To: Work with Observable Collections

## Problem

You need reactive collections that automatically update the UI when items are added, removed, or modified, with filtering, sorting, and transformation capabilities.

## Solution

Use DynamicData's `SourceCache` and `SourceList` with reactive operators for powerful, efficient observable collections.

---

## Pattern 1: Basic SourceCache

**Use case**: Collection with unique keys (ID-based entities).

### Step 1: Create SourceCache

```csharp
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

public class ProductListVM : ReactiveObject
{
    private readonly SourceCache<Product, string> products;

    public ProductListVM()
    {
        // Create cache with key selector
        products = new SourceCache<Product, string>(p => p.Id);

        // Bind to observable collection for UI
        products.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var boundProducts)
            .Subscribe();

        Products = boundProducts;

        // Commands
        AddCommand = ReactiveCommand.Create<Product>(Add);
        RemoveCommand = ReactiveCommand.Create<string>(Remove);
        UpdateCommand = ReactiveCommand.Create<Product>(Update);
    }

    public ReadOnlyObservableCollection<Product> Products { get; }

    public ReactiveCommand<Product, Unit> AddCommand { get; }
    public ReactiveCommand<string, Unit> RemoveCommand { get; }
    public ReactiveCommand<Product, Unit> UpdateCommand { get; }

    private void Add(Product product)
    {
        products.AddOrUpdate(product);
        Console.WriteLine($"✅ Added: {product.Name}");
    }

    private void Remove(string productId)
    {
        products.Remove(productId);
        Console.WriteLine($"🗑️ Removed: {productId}");
    }

    private void Update(Product product)
    {
        products.AddOrUpdate(product);
        Console.WriteLine($"✏️ Updated: {product.Name}");
    }

    public void LoadProducts(IEnumerable<Product> allProducts)
    {
        products.AddOrUpdate(allProducts);
    }
}
```

### Blazor Usage

```razor
@page "/products"
@implements IDisposable

<MudCard>
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">Products (@vm.Products.Count)</MudText>
        </CardHeaderContent>
    </MudCardHeader>
    <MudCardContent>
        <MudList>
            @foreach (var product in vm.Products)
            {
                <MudListItem>
                    <div class="d-flex justify-space-between align-center">
                        <div>
                            <MudText>@product.Name</MudText>
                            <MudText Typo="Typo.caption">$@product.Price</MudText>
                        </div>
                        <MudIconButton
                            Icon="@Icons.Material.Filled.Delete"
                            Size="Size.Small"
                            OnClick="@(() => vm.RemoveCommand.Execute(product.Id).Subscribe())" />
                    </div>
                </MudListItem>
            }
        </MudList>
    </MudCardContent>
</MudCard>

@code {
    private ProductListVM vm = new();
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        // Subscribe to collection changes
        subscription = vm.Products.ToObservableChangeSet()
            .Subscribe(_ => InvokeAsync(StateHasChanged));

        // Load initial data
        vm.LoadProducts(GetSampleProducts());
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

---

## Pattern 2: Transform Collections

**Use case**: Convert entities to ViewModels.

```csharp
public class UserListVM : ReactiveObject
{
    private readonly SourceCache<User, string> users;

    public UserListVM()
    {
        users = new SourceCache<User, string>(u => u.Id);

        // Transform User → UserVM
        users.Connect()
            .Transform(user => new UserVM(user))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var userVMs)
            .Subscribe();

        UserViewModels = userVMs;
    }

    public ReadOnlyObservableCollection<UserVM> UserViewModels { get; }

    public void LoadUsers(IEnumerable<User> allUsers)
    {
        users.AddOrUpdate(allUsers);
    }
}

public class UserVM : ViewModel<User>
{
    public UserVM(User user) : base(user)
    {
        // Computed display name
        this.WhenAnyValue(
                vm => vm.FirstName,
                vm => vm.LastName,
                (first, last) => $"{first} {last}"
            )
            .ToPropertyEx(this, vm => vm.FullName);
    }

    public string FirstName
    {
        get => Model.FirstName;
        set => Model.FirstName = value;
    }

    public string LastName
    {
        get => Model.LastName;
        set => Model.LastName = value;
    }

    [ObservableAsProperty] public string FullName { get; }
}
```

---

## Pattern 3: Filter Collections

**Use case**: Dynamically filter items based on search/criteria.

```csharp
public class FilteredProductsVM : ReactiveObject
{
    private readonly SourceCache<Product, string> products;

    public FilteredProductsVM()
    {
        products = new SourceCache<Product, string>(p => p.Id);

        // Create filter predicate from search text
        var searchFilter = this.WhenAnyValue(vm => vm.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Select(BuildSearchFilter);

        // Category filter
        var categoryFilter = this.WhenAnyValue(vm => vm.SelectedCategory)
            .Select(BuildCategoryFilter);

        // Combine filters
        var combinedFilter = searchFilter.CombineLatest(
            categoryFilter,
            (search, category) => new Func<Product, bool>(p => search(p) && category(p))
        );

        // Apply filters to collection
        products.Connect()
            .Filter(combinedFilter)
            .Transform(p => new ProductVM(p))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var filteredProducts)
            .Subscribe();

        FilteredProducts = filteredProducts;
    }

    public ReadOnlyObservableCollection<ProductVM> FilteredProducts { get; }

    [Reactive] private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    [Reactive] private string? _selectedCategory;
    public string? SelectedCategory
    {
        get => _selectedCategory;
        set => this.RaiseAndSetIfChanged(ref _selectedCategory, value);
    }

    private Func<Product, bool> BuildSearchFilter(string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return _ => true;

        return product => product.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                          product.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }

    private Func<Product, bool> BuildCategoryFilter(string? category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return _ => true;

        return product => product.Category == category;
    }

    public void LoadProducts(IEnumerable<Product> allProducts)
    {
        products.AddOrUpdate(allProducts);
    }
}
```

### Blazor Usage with Filters

```razor
@page "/products/filtered"
@implements IDisposable

<MudCard>
    <MudCardContent>
        <MudTextField
            @bind-Value="vm.SearchText"
            Label="Search"
            Immediate="true"
            Adornment="Adornment.Start"
            AdornmentIcon="@Icons.Material.Filled.Search" />

        <MudSelect
            @bind-Value="vm.SelectedCategory"
            Label="Category"
            Clearable="true">
            <MudSelectItem Value="@("Electronics")">Electronics</MudSelectItem>
            <MudSelectItem Value="@("Clothing")">Clothing</MudSelectItem>
            <MudSelectItem Value="@("Books")">Books</MudSelectItem>
        </MudSelect>

        <MudText Typo="Typo.caption" Class="mt-2">
            Showing @vm.FilteredProducts.Count products
        </MudText>

        <MudList>
            @foreach (var product in vm.FilteredProducts)
            {
                <MudListItem>
                    <MudText>@product.Name</MudText>
                    <MudText Typo="Typo.caption">@product.Category - $@product.Price</MudText>
                </MudListItem>
            }
        </MudList>
    </MudCardContent>
</MudCard>

@code {
    private FilteredProductsVM vm = new();
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        subscription = vm.FilteredProducts.ToObservableChangeSet()
            .Subscribe(_ => InvokeAsync(StateHasChanged));

        vm.LoadProducts(GetAllProducts());
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

---

## Pattern 4: Sort Collections

**Use case**: Dynamic sorting with multiple criteria.

```csharp
public class SortedProductsVM : ReactiveObject
{
    private readonly SourceCache<Product, string> products;

    public SortedProductsVM()
    {
        products = new SourceCache<Product, string>(p => p.Id);

        // Create comparer from sort selection
        var sortComparer = this.WhenAnyValue(vm => vm.SortBy)
            .Select(BuildSortComparer);

        products.Connect()
            .Sort(sortComparer)
            .Transform(p => new ProductVM(p))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var sortedProducts)
            .Subscribe();

        SortedProducts = sortedProducts;
    }

    public ReadOnlyObservableCollection<ProductVM> SortedProducts { get; }

    [Reactive] private SortOption _sortBy = SortOption.NameAscending;
    public SortOption SortBy
    {
        get => _sortBy;
        set => this.RaiseAndSetIfChanged(ref _sortBy, value);
    }

    private IComparer<Product> BuildSortComparer(SortOption sortOption)
    {
        return sortOption switch
        {
            SortOption.NameAscending => SortExpressionComparer<Product>
                .Ascending(p => p.Name),

            SortOption.NameDescending => SortExpressionComparer<Product>
                .Descending(p => p.Name),

            SortOption.PriceLowToHigh => SortExpressionComparer<Product>
                .Ascending(p => p.Price),

            SortOption.PriceHighToLow => SortExpressionComparer<Product>
                .Descending(p => p.Price),

            _ => SortExpressionComparer<Product>.Ascending(p => p.Name)
        };
    }

    public void LoadProducts(IEnumerable<Product> allProducts)
    {
        products.AddOrUpdate(allProducts);
    }
}

public enum SortOption
{
    NameAscending,
    NameDescending,
    PriceLowToHigh,
    PriceHighToLow
}
```

---

## Pattern 5: AutoRefresh for Property Changes

**Use case**: Re-evaluate filters when item properties change.

```csharp
public class LiveFilteredProductsVM : ReactiveObject
{
    private readonly SourceCache<Product, string> products;

    public LiveFilteredProductsVM()
    {
        products = new SourceCache<Product, string>(p => p.Id);

        var priceFilter = this.WhenAnyValue(
            vm => vm.MinPrice,
            vm => vm.MaxPrice,
            (min, max) => new Func<Product, bool>(p => p.Price >= min && p.Price <= max)
        );

        products.Connect()
            // AutoRefresh when Price property changes
            .AutoRefresh(p => p.Price)
            .Filter(priceFilter)
            .Transform(p => new ProductVM(p))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var filteredProducts)
            .Subscribe();

        FilteredProducts = filteredProducts;
    }

    public ReadOnlyObservableCollection<ProductVM> FilteredProducts { get; }

    [Reactive] private decimal _minPrice = 0;
    public decimal MinPrice
    {
        get => _minPrice;
        set => this.RaiseAndSetIfChanged(ref _minPrice, value);
    }

    [Reactive] private decimal _maxPrice = 1000;
    public decimal MaxPrice
    {
        get => _maxPrice;
        set => this.RaiseAndSetIfChanged(ref _maxPrice, value);
    }

    public void UpdateProductPrice(string productId, decimal newPrice)
    {
        // When price changes, AutoRefresh will re-evaluate the filter
        var product = products.Lookup(productId);
        if (product.HasValue)
        {
            product.Value.Price = newPrice;
            products.AddOrUpdate(product.Value); // Trigger refresh
        }
    }
}
```

---

## Pattern 6: Grouping Collections

**Use case**: Group items by property.

```csharp
public class GroupedProductsVM : ReactiveObject
{
    private readonly SourceCache<Product, string> products;

    public GroupedProductsVM()
    {
        products = new SourceCache<Product, string>(p => p.Id);

        products.Connect()
            .Group(p => p.Category)
            .Transform(group => new ProductCategoryGroup
            {
                Category = group.Key,
                Products = group.Cache.Connect()
                    .Transform(p => new ProductVM(p))
                    .AsObservableCache()
            })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var groups)
            .Subscribe();

        CategoryGroups = groups;
    }

    public ReadOnlyObservableCollection<ProductCategoryGroup> CategoryGroups { get; }

    public void LoadProducts(IEnumerable<Product> allProducts)
    {
        products.AddOrUpdate(allProducts);
    }
}

public class ProductCategoryGroup
{
    public string Category { get; set; } = "";
    public IObservableCache<ProductVM, string> Products { get; set; } = null!;
}
```

### Blazor Grouped Display

```razor
@page "/products/grouped"
@implements IDisposable

<MudCard>
    <MudCardContent>
        @foreach (var group in vm.CategoryGroups)
        {
            <MudExpansionPanels>
                <MudExpansionPanel Text="@group.Category">
                    <MudList>
                        @foreach (var product in group.Products.Items)
                        {
                            <MudListItem>
                                <MudText>@product.Name - $@product.Price</MudText>
                            </MudListItem>
                        }
                    </MudList>
                </MudExpansionPanel>
            </MudExpansionPanels>
        }
    </MudCardContent>
</MudCard>

@code {
    private GroupedProductsVM vm = new();
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        subscription = vm.CategoryGroups.ToObservableChangeSet()
            .Subscribe(_ => InvokeAsync(StateHasChanged));

        vm.LoadProducts(GetAllProducts());
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

---

## Pattern 7: SourceList (Non-Keyed Collections)

**Use case**: Simple lists without unique keys.

```csharp
public class SimpleListVM : ReactiveObject
{
    private readonly SourceList<string> items;

    public SimpleListVM()
    {
        items = new SourceList<string>();

        items.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var boundItems)
            .Subscribe();

        Items = boundItems;

        AddCommand = ReactiveCommand.Create<string>(Add);
        RemoveCommand = ReactiveCommand.Create<string>(Remove);
        ClearCommand = ReactiveCommand.Create(Clear);
    }

    public ReadOnlyObservableCollection<string> Items { get; }

    public ReactiveCommand<string, Unit> AddCommand { get; }
    public ReactiveCommand<string, Unit> RemoveCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }

    private void Add(string item)
    {
        items.Add(item);
    }

    private void Remove(string item)
    {
        items.Remove(item);
    }

    private void Clear()
    {
        items.Clear();
    }
}
```

---

## Best Practices

### 1. Use SourceCache for Entities with Keys

```csharp
// ✅ Good - Use SourceCache for entities
var products = new SourceCache<Product, string>(p => p.Id);

// ❌ Avoid - ObservableCollection for complex scenarios
var products = new ObservableCollection<Product>();
```

### 2. Always Bind on UI Thread

```csharp
// ✅ Good - ObserveOn main thread before Bind
products.Connect()
    .Filter(filter)
    .Sort(comparer)
    .ObserveOn(RxApp.MainThreadScheduler)
    .Bind(out var items)
    .Subscribe();

// ❌ Avoid - Cross-thread access
products.Connect()
    .Bind(out var items)
    .Subscribe();
```

### 3. Dispose Subscriptions

```csharp
// ✅ Good - Store and dispose subscription
private readonly IDisposable subscription;

public MyVM()
{
    subscription = products.Connect()
        .Bind(out var items)
        .Subscribe();
}

public void Dispose()
{
    subscription.Dispose();
}

// ❌ Avoid - Memory leak
public MyVM()
{
    products.Connect()
        .Bind(out var items)
        .Subscribe();
    // Never disposed!
}
```

### 4. Throttle Expensive Operations

```csharp
// ✅ Good - Throttle filter updates
var searchFilter = this.WhenAnyValue(vm => vm.SearchText)
    .Throttle(TimeSpan.FromMilliseconds(300))
    .Select(BuildFilter);

products.Connect()
    .Filter(searchFilter)
    .Bind(out var items)
    .Subscribe();

// ❌ Avoid - Filter on every keystroke
var searchFilter = this.WhenAnyValue(vm => vm.SearchText)
    .Select(BuildFilter);
```

---

## Summary

**Observable Collection Patterns:**

1. **SourceCache** - Collections with unique keys
2. **Transform** - Convert entities to ViewModels
3. **Filter** - Dynamic filtering with predicates
4. **Sort** - Dynamic sorting with comparers
5. **AutoRefresh** - Re-evaluate when properties change
6. **Grouping** - Group items by property
7. **SourceList** - Simple lists without keys

**Key Points:**
- Use SourceCache for entities with unique keys
- Use SourceList for simple lists
- Always ObserveOn main thread before Bind
- Throttle expensive filter operations
- Dispose subscriptions properly

**Related Guides:**
- [Reactive Collections](../getting-started/04-reactive-collections.md)
- [Create Custom ViewModels](create-custom-viewmodel.md)
- [Implement Master-Detail](implement-master-detail.md)
- [Reactive Domain Docs](../../reactive/README.md)
