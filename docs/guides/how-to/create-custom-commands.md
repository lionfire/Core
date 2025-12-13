# How-To: Create Custom Commands

## Problem

You need to create reactive commands that respond to user actions, with proper execution state tracking, cancellation support, and error handling.

## Solution

Use ReactiveUI's `ReactiveCommand` with LionFire's ViewModel patterns for powerful, testable command implementations.

---

## Pattern 1: Basic Commands

**Use case**: Simple synchronous actions.

### Synchronous Command

```csharp
using ReactiveUI;
using System.Reactive;
using LionFire.Mvvm;

public class CounterVM : ViewModel<Counter>
{
    public CounterVM(Counter counter) : base(counter)
    {
        // Simple synchronous command
        IncrementCommand = ReactiveCommand.Create(Increment);
        DecrementCommand = ReactiveCommand.Create(Decrement);
        ResetCommand = ReactiveCommand.Create(Reset);
    }

    public int Count
    {
        get => Model.Count;
        private set => Model.Count = value;
    }

    public ReactiveCommand<Unit, Unit> IncrementCommand { get; }
    public ReactiveCommand<Unit, Unit> DecrementCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }

    private void Increment()
    {
        Count++;
        Console.WriteLine($"Count: {Count}");
    }

    private void Decrement()
    {
        Count--;
        Console.WriteLine($"Count: {Count}");
    }

    private void Reset()
    {
        Count = 0;
        Console.WriteLine("Counter reset");
    }
}
```

### Blazor Usage

```razor
@page "/counter"

<MudCard>
    <MudCardContent>
        <MudText Typo="Typo.h4" Align="Align.Center">Count: @vm.Count</MudText>
    </MudCardContent>
    <MudCardActions Class="justify-center">
        <MudButton
            Color="Color.Primary"
            OnClick="@(() => vm.IncrementCommand.Execute().Subscribe())">
            Increment
        </MudButton>
        <MudButton
            Color="Color.Secondary"
            OnClick="@(() => vm.DecrementCommand.Execute().Subscribe())">
            Decrement
        </MudButton>
        <MudButton
            Color="Color.Default"
            OnClick="@(() => vm.ResetCommand.Execute().Subscribe())">
            Reset
        </MudButton>
    </MudCardActions>
</MudCard>

@code {
    private CounterVM vm = new(new Counter());
}
```

---

## Pattern 2: Commands with Parameters

**Use case**: Commands that need input data.

```csharp
public class TodoListVM : ReactiveObject
{
    public TodoListVM()
    {
        Todos = new ObservableCollection<TodoItem>();

        // Command with string parameter
        AddTodoCommand = ReactiveCommand.Create<string>(AddTodo);

        // Command with object parameter
        RemoveTodoCommand = ReactiveCommand.Create<TodoItem>(RemoveTodo);

        // Command with object parameter
        ToggleTodoCommand = ReactiveCommand.Create<TodoItem>(ToggleTodo);
    }

    public ObservableCollection<TodoItem> Todos { get; }

    public ReactiveCommand<string, Unit> AddTodoCommand { get; }
    public ReactiveCommand<TodoItem, Unit> RemoveTodoCommand { get; }
    public ReactiveCommand<TodoItem, Unit> ToggleTodoCommand { get; }

    private void AddTodo(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return;

        var todo = new TodoItem
        {
            Id = Guid.NewGuid().ToString(),
            Title = title,
            IsCompleted = false
        };

        Todos.Add(todo);
        Console.WriteLine($"✅ Added: {title}");
    }

    private void RemoveTodo(TodoItem todo)
    {
        Todos.Remove(todo);
        Console.WriteLine($"🗑️ Removed: {todo.Title}");
    }

    private void ToggleTodo(TodoItem todo)
    {
        todo.IsCompleted = !todo.IsCompleted;
        Console.WriteLine($"{(todo.IsCompleted ? "✓" : "○")} {todo.Title}");
    }
}
```

### Blazor Usage

```razor
@page "/todos"

<MudCard>
    <MudCardContent>
        <MudTextField
            @bind-Value="newTodoTitle"
            Label="New Todo"
            OnKeyDown="OnKeyDown"
            Adornment="Adornment.End"
            AdornmentIcon="@Icons.Material.Filled.Add"
            OnAdornmentClick="@(() => AddTodo())" />

        <MudList>
            @foreach (var todo in vm.Todos)
            {
                <MudListItem>
                    <MudCheckBox
                        Checked="@todo.IsCompleted"
                        CheckedChanged="@(() => vm.ToggleTodoCommand.Execute(todo).Subscribe())"
                        Label="@todo.Title" />
                    <MudIconButton
                        Icon="@Icons.Material.Filled.Delete"
                        Size="Size.Small"
                        Color="Color.Error"
                        OnClick="@(() => vm.RemoveTodoCommand.Execute(todo).Subscribe())" />
                </MudListItem>
            }
        </MudList>
    </MudCardContent>
</MudCard>

@code {
    private TodoListVM vm = new();
    private string newTodoTitle = "";

    private void AddTodo()
    {
        vm.AddTodoCommand.Execute(newTodoTitle).Subscribe();
        newTodoTitle = "";
    }

    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            AddTodo();
        }
    }
}
```

---

## Pattern 3: Commands with CanExecute

**Use case**: Enable/disable commands based on state.

```csharp
public class OrderVM : ViewModel<Order>
{
    public OrderVM(Order order) : base(order)
    {
        // Add item command - always enabled
        AddItemCommand = ReactiveCommand.Create<Product>(AddItem);

        // Remove item command - only when items exist
        var canRemove = this.WhenAnyValue(vm => vm.ItemCount)
            .Select(count => count > 0);
        RemoveLastItemCommand = ReactiveCommand.Create(RemoveLastItem, canRemove);

        // Checkout command - only when items exist and total is valid
        var canCheckout = this.WhenAnyValue(
            vm => vm.ItemCount,
            vm => vm.TotalAmount,
            (count, total) => count > 0 && total > 0
        );
        CheckoutCommand = ReactiveCommand.CreateFromTask(CheckoutAsync, canCheckout);

        // Cancel command - only when order is not finalized
        var canCancel = this.WhenAnyValue(vm => vm.Status)
            .Select(status => status != OrderStatus.Completed && status != OrderStatus.Cancelled);
        CancelCommand = ReactiveCommand.Create(Cancel, canCancel);
    }

    public int ItemCount => Model.Items.Count;
    public decimal TotalAmount => Model.Items.Sum(i => i.Price * i.Quantity);

    public OrderStatus Status
    {
        get => Model.Status;
        set => Model.Status = value;
    }

    public ReactiveCommand<Product, Unit> AddItemCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveLastItemCommand { get; }
    public ReactiveCommand<Unit, Unit> CheckoutCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    private void AddItem(Product product)
    {
        Model.Items.Add(new OrderItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Price = product.Price,
            Quantity = 1
        });
        this.RaisePropertyChanged(nameof(ItemCount));
        this.RaisePropertyChanged(nameof(TotalAmount));
    }

    private void RemoveLastItem()
    {
        if (Model.Items.Any())
        {
            Model.Items.RemoveAt(Model.Items.Count - 1);
            this.RaisePropertyChanged(nameof(ItemCount));
            this.RaisePropertyChanged(nameof(TotalAmount));
        }
    }

    private async Task CheckoutAsync()
    {
        Status = OrderStatus.Processing;
        await Task.Delay(2000); // Simulate payment processing
        Status = OrderStatus.Completed;
        Console.WriteLine($"✅ Order completed: ${TotalAmount:F2}");
    }

    private void Cancel()
    {
        Status = OrderStatus.Cancelled;
        Console.WriteLine("❌ Order cancelled");
    }
}
```

### Blazor Usage with Command States

```razor
@page "/order"
@implements IDisposable

<MudCard>
    <MudCardContent>
        <MudText Typo="Typo.h6">
            Items: @vm.ItemCount | Total: $@vm.TotalAmount.ToString("F2")
        </MudText>
        <MudText Typo="Typo.caption">Status: @vm.Status</MudText>
    </MudCardContent>
    <MudCardActions>
        <MudButton
            Color="Color.Primary"
            OnClick="@(() => vm.AddItemCommand.Execute(sampleProduct).Subscribe())">
            Add Item
        </MudButton>
        <MudButton
            Color="Color.Secondary"
            Disabled="@(!CanExecute(vm.RemoveLastItemCommand))"
            OnClick="@(() => vm.RemoveLastItemCommand.Execute().Subscribe())">
            Remove Last
        </MudButton>
        <MudButton
            Color="Color.Success"
            Disabled="@(!CanExecute(vm.CheckoutCommand))"
            OnClick="@(() => vm.CheckoutCommand.Execute().Subscribe())">
            Checkout
        </MudButton>
        <MudButton
            Color="Color.Error"
            Disabled="@(!CanExecute(vm.CancelCommand))"
            OnClick="@(() => vm.CancelCommand.Execute().Subscribe())">
            Cancel
        </MudButton>
    </MudCardActions>
</MudCard>

@code {
    private OrderVM vm = new(new Order());
    private Product sampleProduct = new() { Id = "1", Name = "Product", Price = 10.00m };
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        subscription = vm.WhenAnyValue(
                v => v.ItemCount,
                v => v.TotalAmount,
                v => v.Status
            )
            .Subscribe(_ => InvokeAsync(StateHasChanged));
    }

    private bool CanExecute(IReactiveCommand command)
    {
        return command.CanExecute.FirstAsync().GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

---

## Pattern 4: Async Commands with Progress

**Use case**: Long-running operations with progress tracking.

```csharp
public class DataProcessorVM : ReactiveObject
{
    public DataProcessorVM()
    {
        // Async command with execution tracking
        ProcessCommand = ReactiveCommand.CreateFromTask<string>(ProcessDataAsync);

        // Track execution state
        ProcessCommand.IsExecuting
            .ToProperty(this, x => x.IsProcessing, out _isProcessing);

        // Track results
        ProcessCommand
            .ToProperty(this, x => x.LastResult, out _lastResult);

        // Track errors
        ProcessCommand.ThrownExceptions
            .Subscribe(ex =>
            {
                ErrorMessage = ex.Message;
                Progress = 0;
            });
    }

    public ReactiveCommand<string, ProcessResult> ProcessCommand { get; }

    private readonly ObservableAsPropertyHelper<bool> _isProcessing;
    public bool IsProcessing => _isProcessing.Value;

    private readonly ObservableAsPropertyHelper<ProcessResult?> _lastResult;
    public ProcessResult? LastResult => _lastResult.Value;

    [Reactive] private int _progress;
    public int Progress
    {
        get => _progress;
        set => this.RaiseAndSetIfChanged(ref _progress, value);
    }

    [Reactive] private string _statusMessage = "";
    public string StatusMessage
    {
        get => _statusMessage;
        set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

    [Reactive] private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    private async Task<ProcessResult> ProcessDataAsync(string filePath)
    {
        Progress = 0;
        ErrorMessage = "";

        for (int i = 0; i <= 100; i += 10)
        {
            Progress = i;
            StatusMessage = $"Processing... {i}%";
            await Task.Delay(500);
        }

        return new ProcessResult
        {
            Success = true,
            RecordsProcessed = 1000,
            Duration = TimeSpan.FromSeconds(5)
        };
    }
}
```

---

## Pattern 5: Command Chaining

**Use case**: Execute commands in sequence.

```csharp
public class DataImportVM : ReactiveObject
{
    public DataImportVM(IDataService dataService)
    {
        // Step 1: Load file
        LoadFileCommand = ReactiveCommand.CreateFromTask<string>(LoadFileAsync);

        // Step 2: Validate data (can only execute after load)
        var canValidate = LoadFileCommand
            .Select(data => data != null);
        ValidateCommand = ReactiveCommand.CreateFromTask(ValidateAsync, canValidate);

        // Step 3: Import data (can only execute after validation)
        var canImport = ValidateCommand
            .Select(isValid => isValid);
        ImportCommand = ReactiveCommand.CreateFromTask(ImportAsync, canImport);

        // Chain the commands
        LoadFileCommand
            .SelectMany(_ => ValidateCommand.Execute())
            .SelectMany(_ => ImportCommand.Execute())
            .Subscribe(
                _ => Console.WriteLine("✅ Import pipeline completed"),
                ex => Console.WriteLine($"❌ Error: {ex.Message}")
            );
    }

    public ReactiveCommand<string, FileData> LoadFileCommand { get; }
    public ReactiveCommand<Unit, bool> ValidateCommand { get; }
    public ReactiveCommand<Unit, Unit> ImportCommand { get; }

    [Reactive] private FileData? _loadedData;
    public FileData? LoadedData
    {
        get => _loadedData;
        set => this.RaiseAndSetIfChanged(ref _loadedData, value);
    }

    private async Task<FileData> LoadFileAsync(string filePath)
    {
        await Task.Delay(1000);
        LoadedData = new FileData { Path = filePath, Records = 100 };
        return LoadedData;
    }

    private async Task<bool> ValidateAsync()
    {
        await Task.Delay(500);
        return LoadedData?.Records > 0;
    }

    private async Task ImportAsync()
    {
        await Task.Delay(2000);
        Console.WriteLine($"Imported {LoadedData?.Records} records");
    }
}
```

---

## Pattern 6: Cancellable Commands

**Use case**: Support cancellation for long operations.

```csharp
public class LongOperationVM : ReactiveObject
{
    private CancellationTokenSource? cts;

    public LongOperationVM()
    {
        // Start command - disabled when running
        var canStart = this.WhenAnyValue(vm => vm.IsRunning)
            .Select(running => !running);
        StartCommand = ReactiveCommand.CreateFromTask(StartAsync, canStart);

        // Cancel command - only enabled when running
        var canCancel = this.WhenAnyValue(vm => vm.IsRunning);
        CancelCommand = ReactiveCommand.Create(Cancel, canCancel);

        // Track execution
        StartCommand.IsExecuting
            .ToProperty(this, x => x.IsRunning, out _isRunning);
    }

    private readonly ObservableAsPropertyHelper<bool> _isRunning;
    public bool IsRunning => _isRunning.Value;

    [Reactive] private int _progress;
    public int Progress
    {
        get => _progress;
        set => this.RaiseAndSetIfChanged(ref _progress, value);
    }

    [Reactive] private string _statusMessage = "";
    public string StatusMessage
    {
        get => _statusMessage;
        set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    private async Task StartAsync()
    {
        cts = new CancellationTokenSource();
        Progress = 0;

        try
        {
            for (int i = 0; i <= 100; i += 5)
            {
                cts.Token.ThrowIfCancellationRequested();

                Progress = i;
                StatusMessage = $"Processing... {i}%";
                await Task.Delay(500, cts.Token);
            }

            StatusMessage = "✅ Completed";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "❌ Cancelled";
            Progress = 0;
        }
        finally
        {
            cts?.Dispose();
            cts = null;
        }
    }

    private void Cancel()
    {
        cts?.Cancel();
    }
}
```

---

## Best Practices

### 1. Use Appropriate Return Types

```csharp
// ✅ Good - Return meaningful results
public ReactiveCommand<string, User> LoadUserCommand { get; }
LoadUserCommand = ReactiveCommand.CreateFromTask<string, User>(LoadUserAsync);

// ❌ Avoid - Always returning Unit
public ReactiveCommand<string, Unit> LoadUserCommand { get; }
```

### 2. Handle Errors Gracefully

```csharp
// ✅ Good - Subscribe to errors
SaveCommand.ThrownExceptions
    .Subscribe(ex =>
    {
        ErrorMessage = $"Failed to save: {ex.Message}";
        Logger.LogError(ex, "Save failed");
    });

// ❌ Avoid - Unhandled errors
SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
// No error handling!
```

### 3. Use CanExecute for Business Rules

```csharp
// ✅ Good - Reactive CanExecute
var canSave = this.WhenAnyValue(
    vm => vm.HasChanges,
    vm => vm.IsValid,
    (hasChanges, isValid) => hasChanges && isValid
);
SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, canSave);

// ❌ Avoid - Manual enable/disable
[Reactive] private bool _canSave;
public bool CanSave { get => _canSave; set => this.RaiseAndSetIfChanged(ref _canSave, value); }
```

### 4. Dispose Properly

```csharp
// ✅ Good - Dispose subscriptions
public class MyVM : ReactiveObject, IDisposable
{
    private readonly CompositeDisposable disposables = new();

    public MyVM()
    {
        MyCommand.ThrownExceptions
            .Subscribe(ex => HandleError(ex))
            .DisposeWith(disposables);
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}

// ❌ Avoid - Memory leaks
public MyVM()
{
    MyCommand.ThrownExceptions
        .Subscribe(ex => HandleError(ex));
    // Never disposed!
}
```

---

## Summary

**Command Patterns:**

1. **Basic** - Simple synchronous commands
2. **With Parameters** - Commands that take input
3. **With CanExecute** - Conditional command execution
4. **Async with Progress** - Long operations with tracking
5. **Chaining** - Sequential command execution
6. **Cancellable** - Support cancellation

**Key Points:**
- Use ReactiveCommand for all user actions
- Implement CanExecute for business rules
- Track execution state with IsExecuting
- Handle errors with ThrownExceptions
- Support cancellation for long operations

**Related Guides:**
- [Create Custom ViewModels](create-custom-viewmodel.md)
- [Handle Loading States](handle-loading-states.md)
- [MVVM Basics](../getting-started/03-mvvm-basics.md)
- [MVVM Domain Docs](../../mvvm/README.md)
