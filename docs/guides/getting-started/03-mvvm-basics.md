# Getting Started: MVVM Basics

## Overview

This guide introduces **MVVM (Model-View-ViewModel)** patterns in LionFire using **ReactiveUI**. You'll learn how to create ViewModels that wrap your data models, expose UI-specific properties, and provide commands for user actions.

**What You'll Learn**:
- Creating your first ViewModel with ReactiveUI
- Reactive properties with `[Reactive]` attribute
- Commands with `ReactiveCommand`
- Wrapping async data with ViewModels
- Data binding patterns

**Prerequisites**:
- .NET 9.0+ SDK
- Completed [02-async-data.md](02-async-data.md) (recommended)
- Basic understanding of MVVM pattern

---

## Setup

### 1. Create a New Console Project

```bash
dotnet new console -n LionFireMvvmBasics
cd LionFireMvvmBasics
```

### 2. Add Required Packages

```bash
dotnet add package LionFire.Mvvm
dotnet add package LionFire.Data.Async.Mvvm
dotnet add package ReactiveUI
dotnet add package ReactiveUI.SourceGenerators
```

### 3. Enable Source Generators

Add to your `.csproj` file:

```xml
<PropertyGroup>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
</PropertyGroup>
```

---

## Your First ViewModel

Let's create a simple ViewModel for a task/todo item.

### Step 1: Create the Model (Entity)

```csharp
using ReactiveUI;
using ReactiveUI.SourceGenerators;

// Model: Business logic and data
[Alias("Task")]
public partial class TaskEntity : ReactiveObject
{
    [Reactive] private string _title = "";
    [Reactive] private string? _description;
    [Reactive] private bool _isCompleted;
    [Reactive] private DateTime _dueDate = DateTime.Today;

    // Business logic
    public void Complete()
    {
        IsCompleted = true;
    }

    public bool IsOverdue()
    {
        return !IsCompleted && DateTime.Today > DueDate;
    }
}
```

**Key Points**:
- Inherit from `ReactiveObject`
- Use `[Reactive]` attribute on backing fields
- Source generator creates public properties automatically
- Keep business logic in the model

### Step 2: Create the ViewModel

```csharp
using LionFire.Mvvm;
using ReactiveUI;
using System.Reactive;

// ViewModel: UI-specific concerns
public class TaskVM : ViewModel<TaskEntity>
{
    public TaskVM(TaskEntity task) : base(task)
    {
        // Create commands
        CompleteCommand = ReactiveCommand.Create(Complete);
        DeleteCommand = ReactiveCommand.Create(Delete);

        // React to property changes
        this.WhenAnyValue(vm => vm.Model.IsCompleted)
            .Subscribe(completed =>
            {
                Console.WriteLine($"Task completion changed: {completed}");
            });
    }

    // UI-specific computed properties
    public string DisplayTitle =>
        Model.IsCompleted
            ? $"✓ {Model.Title}"
            : Model.Title;

    public string StatusColor =>
        Model.IsOverdue() ? "Red" :
        Model.IsCompleted ? "Green" :
        "Gray";

    public string DueDateText =>
        Model.DueDate.ToString("MMM dd, yyyy");

    // Commands
    public ReactiveCommand<Unit, Unit> CompleteCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    private void Complete()
    {
        Model.Complete();
        Console.WriteLine($"Completed: {Model.Title}");
    }

    private void Delete()
    {
        Console.WriteLine($"Deleting: {Model.Title}");
        // In real app, remove from collection
    }
}
```

### Step 3: Use the ViewModel

```csharp
// Create model
var task = new TaskEntity
{
    Title = "Write documentation",
    Description = "Complete the getting started guide",
    DueDate = DateTime.Today.AddDays(3)
};

// Create ViewModel
var taskVM = new TaskVM(task);

// Display UI properties
Console.WriteLine($"Title: {taskVM.DisplayTitle}");
Console.WriteLine($"Status: {taskVM.StatusColor}");
Console.WriteLine($"Due: {taskVM.DueDateText}");

// Execute command
taskVM.CompleteCommand.Execute().Subscribe();

// Properties update automatically
Console.WriteLine($"Title: {taskVM.DisplayTitle}");  // Now shows ✓
```

---

## Reactive Properties

The `[Reactive]` attribute generates reactive properties with change notifications.

### How It Works

```csharp
public partial class TaskEntity : ReactiveObject
{
    // You write this:
    [Reactive] private string _title = "";

    // Source generator creates this:
    // public string Title
    // {
    //     get => _title;
    //     set => this.RaiseAndSetIfChanged(ref _title, value);
    // }
}
```

### Observing Property Changes

```csharp
public class TaskVM : ViewModel<TaskEntity>
{
    public TaskVM(TaskEntity task) : base(task)
    {
        // Watch single property
        this.WhenAnyValue(vm => vm.Model.Title)
            .Subscribe(title =>
            {
                Console.WriteLine($"Title changed to: {title}");
            });

        // Watch multiple properties
        this.WhenAnyValue(
                vm => vm.Model.Title,
                vm => vm.Model.IsCompleted,
                (title, completed) => new { title, completed })
            .Subscribe(x =>
            {
                Console.WriteLine($"{x.title} - Completed: {x.completed}");
            });

        // React to completion
        this.WhenAnyValue(vm => vm.Model.IsCompleted)
            .Where(completed => completed)
            .Subscribe(_ =>
            {
                Console.WriteLine("🎉 Task completed!");
            });
    }
}
```

---

## Commands with ReactiveCommand

Commands encapsulate user actions with built-in enable/disable logic.

### Basic Commands

```csharp
public class TaskVM : ViewModel<TaskEntity>
{
    public TaskVM(TaskEntity task) : base(task)
    {
        // Simple command
        CompleteCommand = ReactiveCommand.Create(Complete);

        // Async command
        SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);

        // Command with parameter
        UpdatePriorityCommand = ReactiveCommand.Create<int>(UpdatePriority);
    }

    public ReactiveCommand<Unit, Unit> CompleteCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<int, Unit> UpdatePriorityCommand { get; }

    private void Complete() => Model.Complete();

    private async Task SaveAsync()
    {
        Console.WriteLine("Saving...");
        await Task.Delay(1000);
        Console.WriteLine("Saved!");
    }

    private void UpdatePriority(int priority)
    {
        Console.WriteLine($"Priority updated to: {priority}");
    }
}
```

### Commands with CanExecute

```csharp
public class TaskVM : ViewModel<TaskEntity>
{
    public TaskVM(TaskEntity task) : base(task)
    {
        // Complete only if not already completed
        CompleteCommand = ReactiveCommand.Create(
            Complete,
            this.WhenAnyValue(vm => vm.Model.IsCompleted, completed => !completed)
        );

        // Delete only if not completed
        DeleteCommand = ReactiveCommand.Create(
            Delete,
            this.WhenAnyValue(vm => vm.Model.IsCompleted, completed => !completed)
        );

        // Edit only if title is not empty
        EditCommand = ReactiveCommand.Create(
            Edit,
            this.WhenAnyValue(
                vm => vm.Model.Title,
                title => !string.IsNullOrWhiteSpace(title))
        );
    }

    public ReactiveCommand<Unit, Unit> CompleteCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    public ReactiveCommand<Unit, Unit> EditCommand { get; }

    private void Complete() => Model.Complete();
    private void Delete() { /* ... */ }
    private void Edit() { /* ... */ }
}

// Usage
taskVM.CompleteCommand.CanExecute.Subscribe(canExecute =>
{
    Console.WriteLine($"Can complete: {canExecute}");
});
```

---

## Wrapping Async Data in ViewModels

Combine async data patterns with ViewModels.

### Example: User Profile ViewModel

```csharp
using LionFire.Data.Async;
using LionFire.Data.Async.Mvvm;

public record UserProfile(string UserId, string Name, string Email);

public class UserProfileVM : GetterVM<UserProfile>
{
    public UserProfileVM(IGetter<UserProfile> getter) : base(getter)
    {
        // Create commands
        RefreshCommand = ReactiveCommand.CreateFromTask(RefreshAsync);

        // React to value changes
        this.WhenAnyValue(vm => vm.Value)
            .Where(profile => profile != null)
            .Subscribe(profile =>
            {
                Console.WriteLine($"Profile loaded: {profile.Name}");
            });
    }

    // UI-specific properties
    public string DisplayName => Value?.Name ?? "Loading...";
    public string Email => Value?.Email ?? "";
    public string InitialsColor =>
        Value != null
            ? $"#{Math.Abs(Value.UserId.GetHashCode()) % 0xFFFFFF:X6}"
            : "#CCCCCC";

    // Commands
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

    private async Task RefreshAsync()
    {
        DiscardValue();
        await GetIfNeeded();
    }
}

// Usage
var profileGetter = new GetterRxO<UserProfile>(async ct =>
{
    await Task.Delay(500, ct);
    return GetResult.Success(new UserProfile("user123", "Alice", "alice@example.com"));
});

var profileVM = new UserProfileVM(profileGetter);

// Subscribe to loading state
profileVM.WhenAnyValue(vm => vm.IsLoading)
    .Subscribe(loading =>
    {
        Console.WriteLine(loading ? "⏳ Loading..." : "✅ Loaded");
    });

// Load data
await profileVM.GetIfNeeded();
Console.WriteLine($"Name: {profileVM.DisplayName}");
Console.WriteLine($"Email: {profileVM.Email}");

// Refresh
await profileVM.RefreshCommand.Execute();
```

---

## Practical Example: Todo List Manager

Let's build a complete todo list manager:

```csharp
using LionFire.Mvvm;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Collections.ObjectModel;
using System.Reactive;

// Model
[Alias("TodoItem")]
public partial class TodoItem : ReactiveObject
{
    [Reactive] private string _title = "";
    [Reactive] private bool _isCompleted;

    public void Toggle() => IsCompleted = !IsCompleted;
}

// ViewModel
public class TodoItemVM : ViewModel<TodoItem>
{
    public TodoItemVM(TodoItem item) : base(item)
    {
        ToggleCommand = ReactiveCommand.Create(Toggle);
        DeleteCommand = ReactiveCommand.Create(RequestDelete);
    }

    public string DisplayTitle =>
        Model.IsCompleted ? $"✓ {Model.Title}" : Model.Title;

    public ReactiveCommand<Unit, Unit> ToggleCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    public event EventHandler? DeleteRequested;

    private void Toggle() => Model.Toggle();

    private void RequestDelete()
    {
        DeleteRequested?.Invoke(this, EventArgs.Empty);
    }
}

// Manager ViewModel
public partial class TodoListVM : ReactiveObject
{
    [Reactive] private string _newItemTitle = "";

    public TodoListVM()
    {
        AddCommand = ReactiveCommand.Create(
            AddItem,
            this.WhenAnyValue(
                vm => vm.NewItemTitle,
                title => !string.IsNullOrWhiteSpace(title))
        );

        ClearCompletedCommand = ReactiveCommand.Create(ClearCompleted);
    }

    public ObservableCollection<TodoItemVM> Items { get; } = new();

    public ReactiveCommand<Unit, Unit> AddCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearCompletedCommand { get; }

    public int ActiveCount =>
        Items.Count(vm => !vm.Model.IsCompleted);

    public int CompletedCount =>
        Items.Count(vm => vm.Model.IsCompleted);

    private void AddItem()
    {
        var item = new TodoItem { Title = NewItemTitle };
        var itemVM = new TodoItemVM(item);

        itemVM.DeleteRequested += (s, e) =>
        {
            Items.Remove(itemVM);
            UpdateCounts();
        };

        // React to completion changes
        itemVM.Model.WhenAnyValue(m => m.IsCompleted)
            .Subscribe(_ => UpdateCounts());

        Items.Add(itemVM);
        NewItemTitle = "";
        UpdateCounts();
    }

    private void ClearCompleted()
    {
        var completed = Items.Where(vm => vm.Model.IsCompleted).ToList();
        foreach (var item in completed)
        {
            Items.Remove(item);
        }
        UpdateCounts();
    }

    private void UpdateCounts()
    {
        this.RaisePropertyChanged(nameof(ActiveCount));
        this.RaisePropertyChanged(nameof(CompletedCount));
    }
}

// Usage
var todoList = new TodoListVM();

// Add some items
todoList.NewItemTitle = "Buy groceries";
todoList.AddCommand.Execute().Subscribe();

todoList.NewItemTitle = "Write code";
todoList.AddCommand.Execute().Subscribe();

todoList.NewItemTitle = "Review PRs";
todoList.AddCommand.Execute().Subscribe();

// Display items
Console.WriteLine($"Todo List ({todoList.Items.Count} items)");
Console.WriteLine($"Active: {todoList.ActiveCount}, Completed: {todoList.CompletedCount}");
Console.WriteLine();

foreach (var item in todoList.Items)
{
    Console.WriteLine($"  {item.DisplayTitle}");
}

// Complete an item
todoList.Items[0].ToggleCommand.Execute().Subscribe();
Console.WriteLine($"\nActive: {todoList.ActiveCount}, Completed: {todoList.CompletedCount}");

// Clear completed
todoList.ClearCompletedCommand.Execute().Subscribe();
Console.WriteLine($"After clearing: {todoList.Items.Count} items remaining");
```

---

## Best Practices

### 1. Keep ViewModels Thin

```csharp
// ✅ Good - VM adds UI concerns only
public class TaskVM : ViewModel<TaskEntity>
{
    public string DisplayTitle => $"✓ {Model.Title}";
    public ReactiveCommand<Unit, Unit> CompleteCommand { get; }
}

// ❌ Avoid - Business logic in VM
public class TaskVM : ViewModel<TaskEntity>
{
    public void CompleteTask()
    {
        // Complex business logic here - should be in Model!
        Model.IsCompleted = true;
        Model.CompletedDate = DateTime.Now;
        Model.CompletedBy = CurrentUser;
        // ...
    }
}
```

### 2. Use Reactive Properties

```csharp
// ✅ Good - Reactive property
public partial class TaskEntity : ReactiveObject
{
    [Reactive] private string _title = "";
}

// ❌ Avoid - Manual INotifyPropertyChanged
public class TaskEntity : INotifyPropertyChanged
{
    private string title;
    public string Title
    {
        get => title;
        set
        {
            title = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
```

### 3. Dispose Subscriptions

```csharp
// ✅ Good - Proper disposal
public class TaskVM : ViewModel<TaskEntity>, IDisposable
{
    private readonly CompositeDisposable subscriptions = new();

    public TaskVM(TaskEntity task) : base(task)
    {
        this.WhenAnyValue(vm => vm.Model.Title)
            .Subscribe(title => Console.WriteLine(title))
            .DisposeWith(subscriptions);
    }

    public void Dispose() => subscriptions.Dispose();
}
```

### 4. Don't Duplicate Business Logic

```csharp
// ✅ Good - Delegate to model
private void Complete() => Model.Complete();

// ❌ Avoid - Duplicating logic
private void Complete()
{
    Model.IsCompleted = true;  // Should be in Model.Complete()!
}
```

---

## Summary

**LionFire MVVM** with ReactiveUI provides:

**Core Concepts**:
- **ReactiveObject** - Base class for reactive properties
- **[Reactive]** attribute - Source-generated properties
- **ReactiveCommand** - Commands with can-execute logic
- **WhenAnyValue** - Observe property changes
- **ViewModel<T>** - Wrap models with UI concerns

**Benefits**:
- Automatic change notifications
- Composable reactive patterns
- Type-safe observables
- Minimal boilerplate

**Next Steps**:
1. Learn [04-reactive-collections.md](04-reactive-collections.md) for collection patterns
2. Explore [MVVM Domain Docs](../../mvvm/README.md) for advanced patterns
3. Read [Reactive Patterns](../../mvvm/reactive-patterns.md) for composition

---

## Exercise

Build a shopping cart manager that:
1. Has a `Product` model with name, price, and quantity
2. Has a `ProductVM` with formatted price display and add/remove commands
3. Has a `ShoppingCartVM` that manages a collection of products
4. Shows total price and item count
5. Supports clearing the cart

Use ViewModels, reactive properties, and commands!
