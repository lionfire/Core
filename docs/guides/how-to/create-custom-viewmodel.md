# How-To: Create Custom ViewModels

## Problem

You need to create a custom ViewModel that exposes your domain model to the UI with reactive properties, commands, validation, and computed values.

## Solution

Extend `ViewModel<T>` from LionFire's MVVM framework, which provides ReactiveUI integration and built-in model wrapping.

---

## When to Create Custom ViewModels

**Use custom ViewModels when you need**:
- Complex UI logic separate from domain models
- Reactive commands for user actions
- Computed properties derived from model state
- Input validation before updating the model
- UI-specific state (IsExpanded, IsSelected, etc.)
- Property change notifications to UI

**Skip ViewModels when**:
- Simple data display (use model directly)
- Read-only lists (use model collections)
- No user interaction required

---

## Approach 1: Basic ViewModel

**Use case**: Simple wrapper with reactive properties.

### Step 1: Define Your Model

```csharp
using ReactiveUI;

public class User : ReactiveObject
{
    [Reactive] private string _username = "";
    [Reactive] private string _email = "";
    [Reactive] private bool _isActive = true;
    [Reactive] private DateTime _lastLogin = DateTime.Now;
}
```

### Step 2: Create the ViewModel

```csharp
using LionFire.Mvvm;
using ReactiveUI;
using System.Reactive;

public class UserVM : ViewModel<User>
{
    public UserVM(User user) : base(user)
    {
    }

    // Expose model properties
    public string Username
    {
        get => Model.Username;
        set => Model.Username = value;
    }

    public string Email
    {
        get => Model.Email;
        set => Model.Email = value;
    }

    public bool IsActive
    {
        get => Model.IsActive;
        set => Model.IsActive = value;
    }

    public DateTime LastLogin => Model.LastLogin;
}
```

### Step 3: Use in UI

```razor
@* Blazor component *@
<MudCard>
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">@UserVM.Username</MudText>
        </CardHeaderContent>
    </MudCardHeader>
    <MudCardContent>
        <MudTextField @bind-Value="UserVM.Email" Label="Email" />
        <MudSwitch @bind-Checked="UserVM.IsActive" Label="Active" />
        <MudText>Last Login: @UserVM.LastLogin.ToString("g")</MudText>
    </MudCardContent>
</MudCard>

@code {
    [Parameter] public UserVM UserVM { get; set; } = null!;
}
```

---

## Approach 2: ViewModel with Commands

**Use case**: Add user actions (Save, Delete, etc.).

```csharp
using LionFire.Mvvm;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

public class UserVM : ViewModel<User>
{
    private readonly IUserRepository repository;

    public UserVM(User user, IUserRepository repository) : base(user)
    {
        this.repository = repository;

        // Save command - can execute when model is valid
        var canSave = this.WhenAnyValue(
            vm => vm.Username,
            vm => vm.Email,
            (username, email) => !string.IsNullOrWhiteSpace(username) &&
                                  !string.IsNullOrWhiteSpace(email)
        );

        SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, canSave);

        // Delete command
        DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync);

        // Deactivate command - can execute when active
        var canDeactivate = this.WhenAnyValue(vm => vm.IsActive);
        DeactivateCommand = ReactiveCommand.Create(Deactivate, canDeactivate);
    }

    // Properties
    public string Username
    {
        get => Model.Username;
        set => Model.Username = value;
    }

    public string Email
    {
        get => Model.Email;
        set => Model.Email = value;
    }

    public bool IsActive
    {
        get => Model.IsActive;
        set => Model.IsActive = value;
    }

    // Commands
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    public ReactiveCommand<Unit, Unit> DeactivateCommand { get; }

    // Command implementations
    private async Task SaveAsync()
    {
        await repository.SaveUserAsync(Model);
        Console.WriteLine($"✅ Saved user: {Username}");
    }

    private async Task DeleteAsync()
    {
        await repository.DeleteUserAsync(Model.Username);
        Console.WriteLine($"🗑️ Deleted user: {Username}");
    }

    private void Deactivate()
    {
        IsActive = false;
        Model.LastLogin = DateTime.Now;
    }
}
```

**Usage in Blazor**:

```razor
<MudCard>
    <MudCardContent>
        <MudTextField @bind-Value="UserVM.Username" Label="Username" />
        <MudTextField @bind-Value="UserVM.Email" Label="Email" />
        <MudSwitch @bind-Checked="UserVM.IsActive" Label="Active" />
    </MudCardContent>
    <MudCardActions>
        <MudButton
            Color="Color.Primary"
            Disabled="@(!UserVM.SaveCommand.CanExecute.FirstAsync().GetAwaiter().GetResult())"
            OnClick="@(() => UserVM.SaveCommand.Execute().Subscribe())">
            Save
        </MudButton>
        <MudButton
            Color="Color.Error"
            OnClick="@(() => UserVM.DeleteCommand.Execute().Subscribe())">
            Delete
        </MudButton>
        <MudButton
            Disabled="@(!UserVM.IsActive)"
            OnClick="@(() => UserVM.DeactivateCommand.Execute().Subscribe())">
            Deactivate
        </MudButton>
    </MudCardActions>
</MudCard>
```

---

## Approach 3: Computed Properties

**Use case**: Derive values from model state.

```csharp
public class ProductVM : ViewModel<Product>
{
    public ProductVM(Product product) : base(product)
    {
        // Computed property - recalculates when Price or Quantity change
        this.WhenAnyValue(
                vm => vm.Price,
                vm => vm.Quantity,
                (price, qty) => price * qty
            )
            .ToPropertyEx(this, vm => vm.TotalPrice);

        // Computed status based on stock
        this.WhenAnyValue(vm => vm.Quantity)
            .Select(qty => qty switch
            {
                0 => "Out of Stock",
                < 10 => "Low Stock",
                _ => "In Stock"
            })
            .ToPropertyEx(this, vm => vm.StockStatus);

        // Is on sale?
        this.WhenAnyValue(vm => vm.DiscountPercent)
            .Select(discount => discount > 0)
            .ToPropertyEx(this, vm => vm.IsOnSale);
    }

    // Model properties
    public decimal Price
    {
        get => Model.Price;
        set => Model.Price = value;
    }

    public int Quantity
    {
        get => Model.Quantity;
        set => Model.Quantity = value;
    }

    public decimal DiscountPercent
    {
        get => Model.DiscountPercent;
        set => Model.DiscountPercent = value;
    }

    // Computed properties (ObservableAsPropertyHelper backing)
    [ObservableAsProperty] public decimal TotalPrice { get; }
    [ObservableAsProperty] public string StockStatus { get; } = "";
    [ObservableAsProperty] public bool IsOnSale { get; }
}
```

**Usage**:

```razor
<MudCard>
    <MudCardContent>
        <MudTextField @bind-Value="ProductVM.Price" Label="Price" />
        <MudTextField @bind-Value="ProductVM.Quantity" Label="Quantity" />
        <MudTextField @bind-Value="ProductVM.DiscountPercent" Label="Discount %" />

        <MudDivider Class="my-4" />

        <MudText Typo="Typo.h6">Total: $@ProductVM.TotalPrice</MudText>
        <MudChip Color="@GetStockColor()">@ProductVM.StockStatus</MudChip>
        @if (ProductVM.IsOnSale)
        {
            <MudChip Color="Color.Success">On Sale!</MudChip>
        }
    </MudCardContent>
</MudCard>

@code {
    private Color GetStockColor() => ProductVM.StockStatus switch
    {
        "Out of Stock" => Color.Error,
        "Low Stock" => Color.Warning,
        _ => Color.Success
    };
}
```

---

## Approach 4: Validation

**Use case**: Validate input before updating model.

```csharp
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;

public class RegisterUserVM : ViewModel<User>, IValidatableViewModel
{
    public RegisterUserVM(User user) : base(user)
    {
        // Validation rules
        this.ValidationRule(
            vm => vm.Username,
            username => !string.IsNullOrWhiteSpace(username),
            "Username is required"
        );

        this.ValidationRule(
            vm => vm.Username,
            username => username?.Length >= 3,
            "Username must be at least 3 characters"
        );

        this.ValidationRule(
            vm => vm.Email,
            email => !string.IsNullOrWhiteSpace(email) && email.Contains("@"),
            "Valid email is required"
        );

        this.ValidationRule(
            vm => vm.Password,
            password => password?.Length >= 8,
            "Password must be at least 8 characters"
        );

        this.ValidationRule(
            vm => vm.ConfirmPassword,
            this.WhenAnyValue(vm => vm.Password),
            (confirm, password) => confirm == password,
            "Passwords must match"
        );

        // Submit command - only enabled when valid
        var canSubmit = this.IsValid();
        SubmitCommand = ReactiveCommand.CreateFromTask(SubmitAsync, canSubmit);
    }

    // Validation context
    public ValidationContext ValidationContext { get; } = new ValidationContext();

    // Properties
    public string Username
    {
        get => Model.Username;
        set => Model.Username = value;
    }

    public string Email
    {
        get => Model.Email;
        set => Model.Email = value;
    }

    [Reactive] private string _password = "";
    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    [Reactive] private string _confirmPassword = "";
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => this.RaiseAndSetIfChanged(ref _confirmPassword, value);
    }

    // Commands
    public ReactiveCommand<Unit, Unit> SubmitCommand { get; }

    private async Task SubmitAsync()
    {
        // Model is already populated from bindings
        await SaveUserToDatabase(Model);
        Console.WriteLine($"✅ User registered: {Username}");
    }
}
```

**Usage with Validation Display**:

```razor
<MudCard>
    <MudCardContent>
        <MudTextField
            @bind-Value="VM.Username"
            Label="Username"
            Error="@GetValidationError(nameof(VM.Username))"
            ErrorText="@GetValidationError(nameof(VM.Username))" />

        <MudTextField
            @bind-Value="VM.Email"
            Label="Email"
            Error="@GetValidationError(nameof(VM.Email))"
            ErrorText="@GetValidationError(nameof(VM.Email))" />

        <MudTextField
            @bind-Value="VM.Password"
            Label="Password"
            InputType="InputType.Password"
            Error="@GetValidationError(nameof(VM.Password))"
            ErrorText="@GetValidationError(nameof(VM.Password))" />

        <MudTextField
            @bind-Value="VM.ConfirmPassword"
            Label="Confirm Password"
            InputType="InputType.Password"
            Error="@GetValidationError(nameof(VM.ConfirmPassword))"
            ErrorText="@GetValidationError(nameof(VM.ConfirmPassword))" />
    </MudCardContent>
    <MudCardActions>
        <MudButton
            Color="Color.Primary"
            Disabled="@(!VM.IsValid)"
            OnClick="@(() => VM.SubmitCommand.Execute().Subscribe())">
            Register
        </MudButton>
    </MudCardActions>
</MudCard>

@code {
    [Parameter] public RegisterUserVM VM { get; set; } = null!;

    private bool GetValidationError(string propertyName)
    {
        return VM.ValidationContext
            .GetValidationErrors(propertyName)
            .Any();
    }
}
```

---

## Approach 5: Hierarchical ViewModels

**Use case**: Parent-child ViewModel relationships.

```csharp
public class OrderVM : ViewModel<Order>
{
    public OrderVM(Order order, IEnumerable<OrderItemVM> items) : base(order)
    {
        Items = new ObservableCollection<OrderItemVM>(items);

        // Total recalculates when items change
        Items.ToObservableChangeSet()
            .AutoRefresh(item => item.Subtotal)
            .Select(_ => Items.Sum(item => item.Subtotal))
            .ToPropertyEx(this, vm => vm.TotalAmount);

        // Commands
        AddItemCommand = ReactiveCommand.Create<Product>(AddItem);
        RemoveItemCommand = ReactiveCommand.Create<OrderItemVM>(RemoveItem);
        CheckoutCommand = ReactiveCommand.CreateFromTask(CheckoutAsync);
    }

    public string OrderNumber => Model.OrderNumber;
    public DateTime OrderDate => Model.OrderDate;

    // Child ViewModels
    public ObservableCollection<OrderItemVM> Items { get; }

    // Computed
    [ObservableAsProperty] public decimal TotalAmount { get; }

    // Commands
    public ReactiveCommand<Product, Unit> AddItemCommand { get; }
    public ReactiveCommand<OrderItemVM, Unit> RemoveItemCommand { get; }
    public ReactiveCommand<Unit, Unit> CheckoutCommand { get; }

    private void AddItem(Product product)
    {
        var item = new OrderItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Quantity = 1,
            UnitPrice = product.Price
        };

        var itemVM = new OrderItemVM(item);
        Items.Add(itemVM);
    }

    private void RemoveItem(OrderItemVM item)
    {
        Items.Remove(item);
    }

    private async Task CheckoutAsync()
    {
        Model.Items = Items.Select(vm => vm.Model).ToList();
        Model.TotalAmount = TotalAmount;

        await SaveOrderAsync(Model);
        Console.WriteLine($"✅ Order {OrderNumber} checked out: ${TotalAmount:F2}");
    }
}

public class OrderItemVM : ViewModel<OrderItem>
{
    public OrderItemVM(OrderItem item) : base(item)
    {
        // Subtotal recalculates when quantity or price changes
        this.WhenAnyValue(
                vm => vm.Quantity,
                vm => vm.UnitPrice,
                (qty, price) => qty * price
            )
            .ToPropertyEx(this, vm => vm.Subtotal);
    }

    public string ProductName => Model.ProductName;

    public int Quantity
    {
        get => Model.Quantity;
        set => Model.Quantity = value;
    }

    public decimal UnitPrice => Model.UnitPrice;

    [ObservableAsProperty] public decimal Subtotal { get; }
}
```

**Usage**:

```razor
<MudCard>
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">Order @OrderVM.OrderNumber</MudText>
            <MudText Typo="Typo.caption">@OrderVM.OrderDate.ToString("g")</MudText>
        </CardHeaderContent>
    </MudCardHeader>
    <MudCardContent>
        <MudTable Items="@OrderVM.Items" Hover="true">
            <HeaderContent>
                <MudTh>Product</MudTh>
                <MudTh>Quantity</MudTh>
                <MudTh>Price</MudTh>
                <MudTh>Subtotal</MudTh>
                <MudTh>Actions</MudTh>
            </HeaderContent>
            <RowTemplate>
                <MudTd>@context.ProductName</MudTd>
                <MudTd>
                    <MudNumericField @bind-Value="context.Quantity" Min="1" Max="99" />
                </MudTd>
                <MudTd>$@context.UnitPrice.ToString("F2")</MudTd>
                <MudTd>$@context.Subtotal.ToString("F2")</MudTd>
                <MudTd>
                    <MudIconButton
                        Icon="@Icons.Material.Filled.Delete"
                        Size="Size.Small"
                        OnClick="@(() => OrderVM.RemoveItemCommand.Execute(context).Subscribe())" />
                </MudTd>
            </RowTemplate>
        </MudTable>

        <MudDivider Class="my-4" />

        <MudText Typo="Typo.h5">Total: $@OrderVM.TotalAmount.ToString("F2")</MudText>
    </MudCardContent>
    <MudCardActions>
        <MudButton
            Color="Color.Primary"
            Disabled="@(!OrderVM.Items.Any())"
            OnClick="@(() => OrderVM.CheckoutCommand.Execute().Subscribe())">
            Checkout
        </MudButton>
    </MudCardActions>
</MudCard>
```

---

## Best Practices

### 1. Keep ViewModels Thin

```csharp
// ✅ Good - ViewModel delegates to service
public class UserVM : ViewModel<User>
{
    private readonly IUserService userService;

    public UserVM(User user, IUserService userService) : base(user)
    {
        this.userService = userService;
        SaveCommand = ReactiveCommand.CreateFromTask(
            () => userService.SaveAsync(Model)
        );
    }
}

// ❌ Avoid - Business logic in ViewModel
public class UserVM : ViewModel<User>
{
    public async Task SaveAsync()
    {
        // Heavy business logic here...
        if (Model.Email.Contains("@"))
        {
            using var db = new DbContext();
            // Direct database access...
        }
    }
}
```

### 2. Use Property Dependencies

```csharp
// ✅ Good - Explicit dependencies
this.WhenAnyValue(
    vm => vm.FirstName,
    vm => vm.LastName,
    (first, last) => $"{first} {last}"
).ToPropertyEx(this, vm => vm.FullName);

// ❌ Avoid - Manual property change tracking
public string FullName => $"{FirstName} {LastName}";
```

### 3. Command CanExecute

```csharp
// ✅ Good - Reactive CanExecute
var canSave = this.WhenAnyValue(
    vm => vm.Username,
    username => !string.IsNullOrWhiteSpace(username)
);
SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, canSave);

// ❌ Avoid - Manual CanExecute checking
private bool CanSave() => !string.IsNullOrWhiteSpace(Username);
```

### 4. Dispose Subscriptions

```csharp
// ✅ Good - Dispose subscriptions
public class UserVM : ViewModel<User>, IDisposable
{
    private readonly CompositeDisposable disposables = new();

    public UserVM(User user) : base(user)
    {
        this.WhenAnyValue(vm => vm.Username)
            .Subscribe(username => Console.WriteLine($"Username changed: {username}"))
            .DisposeWith(disposables);
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}
```

---

## Testing Custom ViewModels

```csharp
using Xunit;
using ReactiveUI.Testing;

public class UserVMTests
{
    [Fact]
    public void SaveCommand_ValidUser_CanExecute()
    {
        // Arrange
        var user = new User { Username = "alice", Email = "alice@example.com" };
        var vm = new UserVM(user, new MockUserRepository());

        // Act & Assert
        Assert.True(vm.SaveCommand.CanExecute.FirstAsync().GetAwaiter().GetResult());
    }

    [Fact]
    public void SaveCommand_InvalidUser_CannotExecute()
    {
        // Arrange
        var user = new User { Username = "", Email = "" };
        var vm = new UserVM(user, new MockUserRepository());

        // Act & Assert
        Assert.False(vm.SaveCommand.CanExecute.FirstAsync().GetAwaiter().GetResult());
    }

    [Fact]
    public void TotalPrice_RecalculatesOnChange()
    {
        // Arrange
        var product = new Product { Price = 10.0m, Quantity = 5 };
        var vm = new ProductVM(product);

        // Act
        vm.Price = 20.0m;

        // Assert
        Assert.Equal(100.0m, vm.TotalPrice); // 20 * 5
    }

    [Fact]
    public void Username_Validation_RequiresValue()
    {
        // Arrange
        var user = new User();
        var vm = new RegisterUserVM(user);

        // Act
        vm.Username = "";

        // Assert
        Assert.False(vm.ValidationContext.IsValid);
        Assert.Contains("required",
            vm.ValidationContext.GetValidationErrors(nameof(vm.Username)).First().Text);
    }
}
```

---

## Summary

**Creating Custom ViewModels:**

1. **Basic ViewModel** - Wrap model with reactive properties
2. **With Commands** - Add user actions with ReactiveCommand
3. **Computed Properties** - Derive values with WhenAnyValue
4. **With Validation** - Add input validation rules
5. **Hierarchical** - Parent-child ViewModel relationships

**Key Points:**
- Extend `ViewModel<T>` from LionFire.Mvvm
- Use `ReactiveCommand` for user actions
- Use `WhenAnyValue` for computed properties
- Implement `IValidatableViewModel` for validation
- Keep business logic in services, not ViewModels

**Related Guides:**
- [MVVM Basics](../getting-started/03-mvvm-basics.md)
- [Bind Async Data to UI](bind-async-data-to-ui.md)
- [Handle Loading States](handle-loading-states.md)
- [MVVM Domain Docs](../../mvvm/README.md)
