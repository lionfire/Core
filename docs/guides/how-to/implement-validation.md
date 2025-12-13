# How-To: Implement Validation

## Problem

You need to validate user input in forms, ensuring data integrity before saving, with clear error messages and good UX.

## Solution

Use ReactiveUI.Validation for declarative, reactive validation rules integrated with LionFire's MVVM patterns.

---

## Strategy 1: Basic Property Validation

**Use case**: Simple field validation (required, length, format).

### Step 1: Add ReactiveUI.Validation

```xml
<PackageReference Include="ReactiveUI.Validation" />
```

### Step 2: Create ViewModel with Validation

```csharp
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using LionFire.Mvvm;

public class UserRegistrationVM : ViewModel<User>, IValidatableViewModel
{
    public UserRegistrationVM(User user) : base(user)
    {
        // Username validation
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

        // Email validation
        this.ValidationRule(
            vm => vm.Email,
            email => !string.IsNullOrWhiteSpace(email),
            "Email is required"
        );

        this.ValidationRule(
            vm => vm.Email,
            email => email?.Contains("@") ?? false,
            "Email must be valid"
        );

        // Age validation
        this.ValidationRule(
            vm => vm.Age,
            age => age >= 18,
            "Must be 18 or older"
        );
    }

    public ValidationContext ValidationContext { get; } = new ValidationContext();

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

    public int Age
    {
        get => Model.Age;
        set => Model.Age = value;
    }
}
```

### Step 3: Blazor Component with Validation Display

```razor
@page "/register"
@inject IUserService UserService

<MudCard>
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">User Registration</MudText>
        </CardHeaderContent>
    </MudCardHeader>
    <MudCardContent>
        <MudTextField
            @bind-Value="vm.Username"
            Label="Username"
            Error="@HasError(nameof(vm.Username))"
            ErrorText="@GetErrorText(nameof(vm.Username))"
            Required="true" />

        <MudTextField
            @bind-Value="vm.Email"
            Label="Email"
            Error="@HasError(nameof(vm.Email))"
            ErrorText="@GetErrorText(nameof(vm.Email))"
            Required="true" />

        <MudNumericField
            @bind-Value="vm.Age"
            Label="Age"
            Error="@HasError(nameof(vm.Age))"
            ErrorText="@GetErrorText(nameof(vm.Age))"
            Min="0" Max="120" />

        @if (!vm.ValidationContext.IsValid)
        {
            <MudAlert Severity="Severity.Warning" Class="mt-4">
                Please fix validation errors before submitting
            </MudAlert>
        }
    </MudCardContent>
    <MudCardActions>
        <MudButton
            Color="Color.Primary"
            Disabled="@(!vm.ValidationContext.IsValid)"
            OnClick="Submit">
            Register
        </MudButton>
    </MudCardActions>
</MudCard>

@code {
    private UserRegistrationVM vm = null!;

    protected override void OnInitialized()
    {
        vm = new UserRegistrationVM(new User());
    }

    private bool HasError(string propertyName)
    {
        return vm.ValidationContext
            .GetValidationErrors(propertyName)
            .Any();
    }

    private string? GetErrorText(string propertyName)
    {
        var errors = vm.ValidationContext
            .GetValidationErrors(propertyName)
            .ToList();

        return errors.Any() ? errors.First().Text : null;
    }

    private async Task Submit()
    {
        if (vm.ValidationContext.IsValid)
        {
            await UserService.RegisterAsync(vm.Model);
        }
    }
}
```

---

## Strategy 2: Cross-Field Validation

**Use case**: Validate relationships between fields (password confirmation, date ranges).

```csharp
public class PasswordChangeVM : ReactiveObject, IValidatableViewModel
{
    public PasswordChangeVM()
    {
        // Current password required
        this.ValidationRule(
            vm => vm.CurrentPassword,
            pwd => !string.IsNullOrWhiteSpace(pwd),
            "Current password is required"
        );

        // New password requirements
        this.ValidationRule(
            vm => vm.NewPassword,
            pwd => !string.IsNullOrWhiteSpace(pwd),
            "New password is required"
        );

        this.ValidationRule(
            vm => vm.NewPassword,
            pwd => pwd?.Length >= 8,
            "Password must be at least 8 characters"
        );

        // Cross-field validation: passwords must match
        this.ValidationRule(
            vm => vm.ConfirmPassword,
            this.WhenAnyValue(vm => vm.NewPassword),
            (confirm, newPwd) => confirm == newPwd,
            "Passwords must match"
        );

        // Cross-field validation: new password must be different
        this.ValidationRule(
            vm => vm.NewPassword,
            this.WhenAnyValue(vm => vm.CurrentPassword),
            (newPwd, currentPwd) => newPwd != currentPwd,
            "New password must be different from current password"
        );

        // Submit command - only enabled when valid
        var canSubmit = this.IsValid();
        SubmitCommand = ReactiveCommand.CreateFromTask(SubmitAsync, canSubmit);
    }

    public ValidationContext ValidationContext { get; } = new ValidationContext();

    [Reactive] private string _currentPassword = "";
    public string CurrentPassword
    {
        get => _currentPassword;
        set => this.RaiseAndSetIfChanged(ref _currentPassword, value);
    }

    [Reactive] private string _newPassword = "";
    public string NewPassword
    {
        get => _newPassword;
        set => this.RaiseAndSetIfChanged(ref _newPassword, value);
    }

    [Reactive] private string _confirmPassword = "";
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => this.RaiseAndSetIfChanged(ref _confirmPassword, value);
    }

    public ReactiveCommand<Unit, Unit> SubmitCommand { get; }

    private async Task SubmitAsync()
    {
        await Task.Delay(1000);
        Console.WriteLine("✅ Password changed successfully");
    }
}
```

### Blazor Component

```razor
@page "/change-password"

<MudCard>
    <MudCardContent>
        <MudTextField
            @bind-Value="vm.CurrentPassword"
            Label="Current Password"
            InputType="InputType.Password"
            Error="@HasError(nameof(vm.CurrentPassword))"
            ErrorText="@GetErrorText(nameof(vm.CurrentPassword))" />

        <MudTextField
            @bind-Value="vm.NewPassword"
            Label="New Password"
            InputType="InputType.Password"
            Error="@HasError(nameof(vm.NewPassword))"
            ErrorText="@GetErrorText(nameof(vm.NewPassword))" />

        <MudTextField
            @bind-Value="vm.ConfirmPassword"
            Label="Confirm New Password"
            InputType="InputType.Password"
            Error="@HasError(nameof(vm.ConfirmPassword))"
            ErrorText="@GetErrorText(nameof(vm.ConfirmPassword))" />
    </MudCardContent>
    <MudCardActions>
        <MudButton
            Color="Color.Primary"
            Disabled="@(!vm.ValidationContext.IsValid)"
            OnClick="@(() => vm.SubmitCommand.Execute().Subscribe())">
            Change Password
        </MudButton>
    </MudCardActions>
</MudCard>

@code {
    private PasswordChangeVM vm = new();

    private bool HasError(string propertyName) =>
        vm.ValidationContext.GetValidationErrors(propertyName).Any();

    private string? GetErrorText(string propertyName) =>
        vm.ValidationContext.GetValidationErrors(propertyName).FirstOrDefault()?.Text;
}
```

---

## Strategy 3: Async Validation

**Use case**: Validate against external sources (database, API).

```csharp
public class AccountCreationVM : ReactiveObject, IValidatableViewModel
{
    private readonly IUserService userService;

    public AccountCreationVM(IUserService userService)
    {
        this.userService = userService;

        // Synchronous validation
        this.ValidationRule(
            vm => vm.Username,
            username => !string.IsNullOrWhiteSpace(username),
            "Username is required"
        );

        // Async validation - check if username is available
        this.ValidationRule(
            vm => vm.Username,
            username => Observable.FromAsync(async ct =>
            {
                if (string.IsNullOrWhiteSpace(username))
                    return true; // Skip if empty (handled by required validation)

                // Debounce to avoid excessive API calls
                await Task.Delay(500, ct);

                var isAvailable = await userService.IsUsernameAvailableAsync(username, ct);
                return isAvailable;
            }),
            (vm, isAvailable) => isAvailable,
            "Username is already taken"
        );

        // Async email validation
        this.ValidationRule(
            vm => vm.Email,
            email => Observable.FromAsync(async ct =>
            {
                if (string.IsNullOrWhiteSpace(email))
                    return true;

                await Task.Delay(500, ct);

                var isValid = await userService.ValidateEmailAsync(email, ct);
                return isValid;
            }),
            (vm, isValid) => isValid,
            "Email is invalid or already registered"
        );

        // Submit enabled when valid and not validating
        var canSubmit = this.WhenAnyValue(
            vm => vm.ValidationContext.IsValid,
            vm => vm.IsValidating,
            (valid, validating) => valid && !validating
        );

        SubmitCommand = ReactiveCommand.CreateFromTask(SubmitAsync, canSubmit);
    }

    public ValidationContext ValidationContext { get; } = new ValidationContext();

    [Reactive] private string _username = "";
    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    [Reactive] private string _email = "";
    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    [Reactive] private bool _isValidating;
    public bool IsValidating
    {
        get => _isValidating;
        set => this.RaiseAndSetIfChanged(ref _isValidating, value);
    }

    public ReactiveCommand<Unit, Unit> SubmitCommand { get; }

    private async Task SubmitAsync()
    {
        await userService.CreateAccountAsync(Username, Email);
        Console.WriteLine("✅ Account created");
    }
}
```

### UI with Async Validation Indicator

```razor
@page "/create-account"
@inject IUserService UserService
@implements IDisposable

<MudCard>
    <MudCardContent>
        <MudTextField
            @bind-Value="vm.Username"
            Label="Username"
            Error="@HasError(nameof(vm.Username))"
            ErrorText="@GetErrorText(nameof(vm.Username))"
            Adornment="Adornment.End"
            AdornmentIcon="@GetValidationIcon(nameof(vm.Username))" />

        <MudTextField
            @bind-Value="vm.Email"
            Label="Email"
            Error="@HasError(nameof(vm.Email))"
            ErrorText="@GetErrorText(nameof(vm.Email))"
            Adornment="Adornment.End"
            AdornmentIcon="@GetValidationIcon(nameof(vm.Email))" />

        @if (vm.IsValidating)
        {
            <MudText Typo="Typo.caption" Color="Color.Info">
                Validating...
            </MudText>
        }
    </MudCardContent>
    <MudCardActions>
        <MudButton
            Color="Color.Primary"
            Disabled="@(!vm.ValidationContext.IsValid || vm.IsValidating)"
            OnClick="@(() => vm.SubmitCommand.Execute().Subscribe())">
            Create Account
        </MudButton>
    </MudCardActions>
</MudCard>

@code {
    private AccountCreationVM vm = null!;
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        vm = new AccountCreationVM(UserService);

        subscription = vm.WhenAnyValue(
                v => v.IsValidating,
                v => v.ValidationContext.IsValid
            )
            .Subscribe(_ => InvokeAsync(StateHasChanged));
    }

    private bool HasError(string propertyName) =>
        vm.ValidationContext.GetValidationErrors(propertyName).Any();

    private string? GetErrorText(string propertyName) =>
        vm.ValidationContext.GetValidationErrors(propertyName).FirstOrDefault()?.Text;

    private string GetValidationIcon(string propertyName)
    {
        if (vm.IsValidating)
            return Icons.Material.Filled.HourglassEmpty;

        return HasError(propertyName)
            ? Icons.Material.Filled.Error
            : Icons.Material.Filled.CheckCircle;
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

---

## Strategy 4: Custom Validation Rules

**Use case**: Complex business logic validation.

```csharp
public static class CustomValidationRules
{
    // Email domain validation
    public static ValidationHelper EmailDomain(
        this ValidationContext context,
        IObservable<string> emailObservable,
        string[] allowedDomains,
        string message = "Email domain not allowed")
    {
        return context.AddValidation(
            emailObservable,
            email =>
            {
                if (string.IsNullOrWhiteSpace(email))
                    return true;

                var domain = email.Split('@').LastOrDefault();
                return allowedDomains.Any(d => d.Equals(domain, StringComparison.OrdinalIgnoreCase));
            },
            message
        );
    }

    // Credit card validation (Luhn algorithm)
    public static ValidationHelper CreditCard(
        this ValidationContext context,
        IObservable<string> cardNumberObservable,
        string message = "Invalid credit card number")
    {
        return context.AddValidation(
            cardNumberObservable,
            cardNumber =>
            {
                if (string.IsNullOrWhiteSpace(cardNumber))
                    return true;

                cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

                if (!cardNumber.All(char.IsDigit))
                    return false;

                return ValidateLuhn(cardNumber);
            },
            message
        );
    }

    private static bool ValidateLuhn(string cardNumber)
    {
        int sum = 0;
        bool alternate = false;

        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(cardNumber[i].ToString());

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }
}

// Usage
public class PaymentVM : ReactiveObject, IValidatableViewModel
{
    public PaymentVM()
    {
        // Use custom validation
        ValidationContext.EmailDomain(
            this.WhenAnyValue(vm => vm.Email),
            new[] { "company.com", "partner.com" },
            "Must use company or partner email"
        );

        ValidationContext.CreditCard(
            this.WhenAnyValue(vm => vm.CardNumber),
            "Invalid credit card number"
        );
    }

    public ValidationContext ValidationContext { get; } = new ValidationContext();

    [Reactive] private string _email = "";
    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    [Reactive] private string _cardNumber = "";
    public string CardNumber
    {
        get => _cardNumber;
        set => this.RaiseAndSetIfChanged(ref _cardNumber, value);
    }
}
```

---

## Strategy 5: Form-Level Validation Summary

**Use case**: Display all validation errors in one place.

```csharp
public class ComplexFormVM : ReactiveObject, IValidatableViewModel
{
    public ComplexFormVM()
    {
        // Multiple field validations...
        this.ValidationRule(vm => vm.FirstName,
            name => !string.IsNullOrWhiteSpace(name),
            "First name is required");

        this.ValidationRule(vm => vm.LastName,
            name => !string.IsNullOrWhiteSpace(name),
            "Last name is required");

        this.ValidationRule(vm => vm.Email,
            email => !string.IsNullOrWhiteSpace(email) && email.Contains("@"),
            "Valid email is required");

        this.ValidationRule(vm => vm.PhoneNumber,
            phone => !string.IsNullOrWhiteSpace(phone) && phone.Length >= 10,
            "Valid phone number is required");

        // Observable for all errors
        AllErrors = ValidationContext.ValidationStatusChange
            .Select(_ => ValidationContext.GetAllErrors())
            .ToProperty(this, vm => vm.AllErrors);
    }

    public ValidationContext ValidationContext { get; } = new ValidationContext();

    [Reactive] private string _firstName = "";
    public string FirstName
    {
        get => _firstName;
        set => this.RaiseAndSetIfChanged(ref _firstName, value);
    }

    [Reactive] private string _lastName = "";
    public string LastName
    {
        get => _lastName;
        set => this.RaiseAndSetIfChanged(ref _lastName, value);
    }

    [Reactive] private string _email = "";
    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    [Reactive] private string _phoneNumber = "";
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => this.RaiseAndSetIfChanged(ref _phoneNumber, value);
    }

    [ObservableAsProperty]
    public IEnumerable<string> AllErrors { get; }
}
```

### Validation Summary UI

```razor
@page "/contact-form"

<MudCard>
    <MudCardContent>
        @if (vm.AllErrors.Any())
        {
            <MudAlert Severity="Severity.Error" Class="mb-4">
                <MudText Typo="Typo.subtitle2">Please fix the following errors:</MudText>
                <MudList Dense="true">
                    @foreach (var error in vm.AllErrors)
                    {
                        <MudListItem Icon="@Icons.Material.Filled.Error">
                            <MudText Typo="Typo.body2">@error</MudText>
                        </MudListItem>
                    }
                </MudList>
            </MudAlert>
        }

        <MudTextField @bind-Value="vm.FirstName" Label="First Name" />
        <MudTextField @bind-Value="vm.LastName" Label="Last Name" />
        <MudTextField @bind-Value="vm.Email" Label="Email" />
        <MudTextField @bind-Value="vm.PhoneNumber" Label="Phone Number" />
    </MudCardContent>
    <MudCardActions>
        <MudButton
            Color="Color.Primary"
            Disabled="@(!vm.ValidationContext.IsValid)">
            Submit
        </MudButton>
    </MudCardActions>
</MudCard>

@code {
    private ComplexFormVM vm = new();
}
```

---

## Best Practices

### 1. Validate Early and Often

```csharp
// ✅ Good - Immediate validation feedback
<MudTextField
    @bind-Value="vm.Email"
    Immediate="true"
    Error="@HasError(nameof(vm.Email))"
    ErrorText="@GetErrorText(nameof(vm.Email))" />

// ❌ Avoid - Only validate on submit
<MudTextField @bind-Value="vm.Email" />
```

### 2. Disable Submit Until Valid

```csharp
// ✅ Good - Prevent invalid submission
var canSubmit = this.IsValid();
SubmitCommand = ReactiveCommand.CreateFromTask(SubmitAsync, canSubmit);

// ❌ Avoid - Allow invalid submission
SubmitCommand = ReactiveCommand.CreateFromTask(SubmitAsync);
```

### 3. Clear Error Messages

```csharp
// ✅ Good - Specific, actionable messages
this.ValidationRule(
    vm => vm.Username,
    username => username?.Length >= 3,
    "Username must be at least 3 characters"
);

// ❌ Avoid - Vague messages
this.ValidationRule(
    vm => vm.Username,
    username => username?.Length >= 3,
    "Invalid username"
);
```

### 4. Debounce Async Validation

```csharp
// ✅ Good - Debounce to reduce API calls
this.ValidationRule(
    vm => vm.Username,
    username => Observable.FromAsync(async ct =>
    {
        await Task.Delay(500, ct); // Debounce
        return await service.CheckUsernameAsync(username, ct);
    }),
    (vm, isAvailable) => isAvailable,
    "Username is taken"
);

// ❌ Avoid - Validate on every keystroke
this.ValidationRule(
    vm => vm.Username,
    username => Observable.FromAsync(ct =>
        service.CheckUsernameAsync(username, ct)
    ),
    (vm, isAvailable) => isAvailable,
    "Username is taken"
);
```

---

## Summary

**Validation Strategies:**

1. **Basic Property** - Simple field validation
2. **Cross-Field** - Validate relationships between fields
3. **Async** - Validate against external sources
4. **Custom Rules** - Complex business logic
5. **Form-Level** - Validation summary

**Key Points:**
- Use ReactiveUI.Validation for reactive validation
- Validate early with immediate feedback
- Disable submit when invalid
- Debounce async validation
- Provide clear, actionable error messages

**Related Guides:**
- [Create Custom ViewModels](create-custom-viewmodel.md)
- [Bind Async Data to UI](bind-async-data-to-ui.md)
- [MVVM Basics](../getting-started/03-mvvm-basics.md)
- [MVVM Domain Docs](../../mvvm/README.md)
