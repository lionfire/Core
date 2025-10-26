# Data Binding

## Overview

This guide covers how ViewModels bind to Blazor UI components in LionFire. Understanding data binding is essential for building reactive, performant user interfaces with MVVM.

**Key Concept**: Data binding creates a **connection** between ViewModel properties and UI elements, automatically synchronizing data in one or both directions.

---

## Table of Contents

1. [Binding Basics](#binding-basics)
2. [One-Way Binding](#one-way-binding)
3. [Two-Way Binding](#two-way-binding)
4. [Binding to Collections](#binding-to-collections)
5. [Command Binding](#command-binding)
6. [Custom Binding Patterns](#custom-binding-patterns)
7. [Performance Optimization](#performance-optimization)
8. [Common Issues](#common-issues)

---

## Binding Basics

### How Binding Works in Blazor

**Blazor Binding**:
```razor
<!-- UI Element -->
<MudTextField @bind-Value="vm.Value.Name" />
              ↕️ Two-way binding
        ViewModel Property
```

**Flow**:
```
User edits text field
    ↓
@bind-Value updates vm.Value.Name
    ↓
vm.Value.Name setter fires PropertyChanged
    ↓
Blazor detects change
    ↓
StateHasChanged() called
    ↓
Component re-renders
    ↓
Text field shows updated value
```

---

### Requirements for Binding

**Entity Must Implement INotifyPropertyChanged**:

```csharp
// ✅ Correct - Reactive properties
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;  // Auto-generates PropertyChanged
}

// ❌ Wrong - No change notifications
public class BotEntity
{
    public string? Name { get; set; }  // UI won't update!
}
```

---

## One-Way Binding

### Simple Property Display

```razor
<!-- Read-only display -->
<MudText>@vm.Value.Name</MudText>
<MudText>@vm.DisplayName</MudText>
<MudText>@vm.StatusText</MudText>
```

**Use Case**: Displaying data without editing.

---

### Computed Properties

```razor
@code {
    public class BotVM : KeyValueVM<string, BotEntity>
    {
        // Computed property
        public string DisplayName => $"{Value.Name} ({Key})";

        public Color StatusColor => Value.Enabled ? Color.Success : Color.Default;
    }
}

<!-- Bind computed properties -->
<MudText>@vm.DisplayName</MudText>
<MudChip Color="@vm.StatusColor">Status</MudChip>
```

---

### Reactive Computed Properties

```csharp
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value)
    {
        // Reactive computed property (cached)
        StatusText = this.WhenAnyValue(x => x.Value.Enabled)
            .Select(enabled => enabled ? "Running" : "Stopped")
            .ToProperty(this, x => x.StatusText);
    }

    public string StatusText => statusText.Value;
    private readonly ObservableAsPropertyHelper<string> statusText;
}
```

**Usage**:
```razor
<MudText>@vm.StatusText</MudText>
```

**Benefits**:
- Automatically updates when `Value.Enabled` changes
- Cached (doesn't recompute on every access)
- Observable (can be subscribed to)

---

## Two-Way Binding

### @bind-Value (Standard)

```razor
<!-- Two-way binding to entity property -->
<MudTextField Label="Name"
              @bind-Value="vm.Value.Name" />

<MudTextField Label="Description"
              @bind-Value="vm.Value.Description"
              Lines="3" />

<MudSwitch Label="Enabled"
           @bind-Checked="vm.Value.Enabled"
           Color="Color.Primary" />

<MudNumericField Label="Timeout"
                 @bind-Value="vm.Value.Timeout" />
```

**How It Works**:
```
User edits → Property updated → PropertyChanged fired → UI updated
```

---

### @bind-Value with Validation

```razor
<MudTextField Label="Name"
              @bind-Value="vm.Value.Name"
              For="@(() => vm.Value.Name)"
              Validation="@(new Func<string, string?>(ValidateName))" />

@code {
    private string? ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Name is required";
        if (name.Length < 3)
            return "Name must be at least 3 characters";
        return null;
    }
}
```

---

### @bind-Value:after (Blazor Event Callback)

```razor
<MudTextField Label="Name"
              @bind-Value="vm.Value.Name"
              @bind-Value:after="OnNameChanged" />

@code {
    private void OnNameChanged()
    {
        Console.WriteLine($"Name changed to: {vm.Value.Name}");
        // Optionally auto-save
    }
}
```

---

### Debounced Binding

**Problem**: Save on every keystroke is expensive.

**Solution**: Debounce with reactive patterns.

```csharp
public class BotVM : KeyValueVM<string, BotEntity>
{
    private readonly Subject<string> nameChanges = new();

    public BotVM(string key, BotEntity value) : base(key, value)
    {
        // Debounce name changes
        nameChanges
            .Throttle(TimeSpan.FromMilliseconds(500))
            .DistinctUntilChanged()
            .Subscribe(async name => {
                await SaveChanges();
            })
            .DisposeWith(subscriptions);

        // Watch for name changes
        this.WhenAnyValue(x => x.Value.Name)
            .Subscribe(name => nameChanges.OnNext(name))
            .DisposeWith(subscriptions);
    }
}
```

**Usage**:
```razor
<MudTextField Label="Name" @bind-Value="vm.Value.Name" />
<!-- Auto-saves 500ms after user stops typing -->
```

---

## Binding to Collections

### Binding to ObservableCollection

**Entity**:
```csharp
public partial class BotEntity : ReactiveObject
{
    [Reactive] private ObservableCollection<Trade> _trades = new();
}
```

**UI**:
```razor
<MudTable Items="@vm.Value.Trades">
    <HeaderContent>
        <MudTh>Symbol</MudTh>
        <MudTh>Price</MudTh>
    </HeaderContent>
    <RowTemplate>
        <MudTd>@context.Symbol</MudTd>
        <MudTd>@context.Price.ToString("C2")</MudTd>
    </RowTemplate>
</MudTable>
```

**Updates Automatically** when items added/removed.

---

### Binding to DynamicData Cache

**ViewModel**:
```csharp
public class BotsCollectionVM : ReactiveObject
{
    public BotsCollectionVM(IObservableReader<string, BotEntity> reader)
    {
        // DynamicData observable cache
        reader.Values.Connect()
            .Transform(kvp => new BotVM(kvp.Key, kvp.Value))
            .DisposeMany()
            .Bind(out var items)
            .Subscribe();

        Items = items;
    }

    public ReadOnlyObservableCollection<BotVM> Items { get; }
}
```

**UI**:
```razor
<MudDataGrid Items="@vm.Items">
    <Columns>
        <PropertyColumn Property="x => x.Value.Name" />
        <PropertyColumn Property="x => x.Value.Description" />
    </Columns>
</MudDataGrid>
```

---

### Binding to ObservableDataView

**Automatic Collection Binding**:

```razor
<!-- Component handles all collection binding internally -->
<ObservableDataView TKey="string"
                    TValue="BotEntity"
                    TValueVM="BotVM"
                    DataServiceProvider="@WorkspaceServices">
    <Columns>
        <PropertyColumn Property="x => x.Value.Name" />
    </Columns>
</ObservableDataView>
```

**No manual binding required** - component subscribes to `IObservableReader` automatically.

---

## Command Binding

### ReactiveCommand Binding

```csharp
public class BotVM : KeyValueVM<string, BotEntity>
{
    public BotVM(string key, BotEntity value) : base(key, value)
    {
        // Commands
        StartCommand = ReactiveCommand.CreateFromTask(
            StartAsync,
            this.WhenAnyValue(x => x.Value.Enabled, enabled => !enabled)
        );

        DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync);
    }

    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    private async Task StartAsync()
    {
        Value.Enabled = true;
        Value.Start();
    }

    private async Task DeleteAsync()
    {
        // Delete logic
    }
}
```

**Binding to MudButton**:
```razor
<!-- Command binding -->
<MudButton Command="@vm.StartCommand"
           Disabled="@(!vm.StartCommand.CanExecute.Value)">
    Start
</MudButton>

<MudIconButton Icon="@Icons.Material.Filled.Delete"
               Command="@vm.DeleteCommand"
               Disabled="@(!vm.DeleteCommand.CanExecute.Value)" />
```

**Benefits**:
- Automatic enable/disable based on `CanExecute`
- Async operation support
- Reactive dependencies

---

### EventCallback Binding

```razor
<MudButton OnClick="@OnStartClicked">Start</MudButton>
<MudButton OnClick="@(() => vm.Delete())">Delete</MudButton>

@code {
    private async Task OnStartClicked()
    {
        await vm.Start();
    }
}
```

**Use Case**: Simple click handlers without reactive dependencies.

---

## Custom Binding Patterns

### Pattern 1: Master-Detail Binding

```razor
@page "/bots"

<MudGrid>
    <!-- Master: List -->
    <MudItem xs="12" md="6">
        <ObservableDataView ...
                            RowClick="@OnRowClick">
            <Columns>
                <PropertyColumn Property="x => x.Value.Name" />
            </Columns>
        </ObservableDataView>
    </MudItem>

    <!-- Detail: Selected item -->
    <MudItem xs="12" md="6">
        @if (selectedVm != null)
        {
            <MudCard>
                <MudCardContent>
                    <MudTextField Label="Name"
                                  @bind-Value="selectedVm.Value.Name" />
                    <MudTextField Label="Description"
                                  @bind-Value="selectedVm.Value.Description" />
                </MudCardContent>
                <MudCardActions>
                    <MudButton OnClick="@(() => selectedVm.Write())">
                        Save
                    </MudButton>
                </MudCardActions>
            </MudCard>
        }
    </MudItem>
</MudGrid>

@code {
    BotVM? selectedVm;

    void OnRowClick(object sender, DataGridRowClickEventArgs<BotVM> e)
    {
        selectedVm = e.Item;
    }
}
```

---

### Pattern 2: Conditional Binding

```razor
@if (vm.IsEditing)
{
    <!-- Edit mode -->
    <MudTextField Label="Name"
                  @bind-Value="vm.Value.Name" />
}
else
{
    <!-- Display mode -->
    <MudText>@vm.Value.Name</MudText>
}

<MudSwitch Label="Edit Mode"
           @bind-Checked="vm.IsEditing" />
```

---

### Pattern 3: Multi-Select Binding

```razor
<MudSelect Label="Categories"
           T="string"
           MultiSelection="true"
           @bind-SelectedValues="vm.Value.Categories">
    @foreach (var category in availableCategories)
    {
        <MudSelectItem Value="@category">@category</MudSelectItem>
    }
</MudSelect>

@code {
    List<string> availableCategories = new() { "Trading", "Analysis", "Alerts" };
}
```

---

### Pattern 4: Cascading Values

```razor
<!-- Parent component -->
<CascadingValue Name="WorkspaceServices" Value="@WorkspaceServices">
    <CascadingValue Value="@currentBot">
        <ChildComponent />
    </CascadingValue>
</CascadingValue>

<!-- Child component -->
@code {
    [CascadingParameter(Name = "WorkspaceServices")]
    public IServiceProvider? WorkspaceServices { get; set; }

    [CascadingParameter]
    public BotVM? CurrentBot { get; set; }
}
```

---

### Pattern 5: Template Binding

```razor
<MudDataGrid Items="@vm.Items">
    <Columns>
        <!-- Template column with custom binding -->
        <TemplateColumn Title="Status">
            <CellTemplate>
                <MudSwitch T="bool"
                           @bind-Checked="context.Item.Value.Enabled"
                           Color="Color.Primary"
                           Size="Size.Small" />
            </CellTemplate>
        </TemplateColumn>

        <!-- Template column with computed value -->
        <TemplateColumn Title="Actions">
            <CellTemplate>
                <MudIconButton Icon="@Icons.Material.Filled.PlayArrow"
                               OnClick="@(() => context.Item.Start())"
                               Disabled="@context.Item.Value.Enabled" />
                <MudIconButton Icon="@Icons.Material.Filled.Stop"
                               OnClick="@(() => context.Item.Stop())"
                               Disabled="@(!context.Item.Value.Enabled)" />
            </CellTemplate>
        </TemplateColumn>
    </Columns>
</MudDataGrid>
```

---

## Performance Optimization

### Issue 1: Binding in Loops

```razor
<!-- ❌ Bad - Creates binding for every item -->
@foreach (var item in vm.Items)
{
    <MudTextField @bind-Value="item.Name" />
}

<!-- ✅ Good - Use component designed for collections -->
<ObservableDataView Items="@vm.Items">
    <Columns>
        <PropertyColumn Property="x => x.Name" />
    </Columns>
</ObservableDataView>
```

---

### Issue 2: Excessive StateHasChanged

```csharp
// ❌ Bad - StateHasChanged on every property change
this.WhenAnyValue(x => x.Value.Property1)
    .Subscribe(_ => StateHasChanged());

this.WhenAnyValue(x => x.Value.Property2)
    .Subscribe(_ => StateHasChanged());

// ✅ Good - Combine observables
this.WhenAnyValue(
        x => x.Value.Property1,
        x => x.Value.Property2)
    .Throttle(TimeSpan.FromMilliseconds(50))
    .Subscribe(_ => InvokeAsync(StateHasChanged));
```

---

### Issue 3: Binding to Expensive Computations

```razor
<!-- ❌ Bad - Recomputes on every render -->
<MudText>@ExpensiveComputation(vm.Value.Data)</MudText>

<!-- ✅ Good - Cache with reactive property -->
<MudText>@vm.CachedResult</MudText>

@code {
    // In ViewModel
    CachedResult = this.WhenAnyValue(x => x.Value.Data)
        .Select(data => ExpensiveComputation(data))
        .ToProperty(this, x => x.CachedResult);
}
```

---

### Issue 4: Large Collections

```razor
<!-- ❌ Bad - Renders all 10,000 items -->
<MudDataGrid Items="@vm.Items" />  <!-- 10,000 items! -->

<!-- ✅ Good - Virtualization -->
<MudDataGrid Items="@vm.Items"
             Virtualize="true"
             ItemSize="48" />

<!-- ✅ Good - Pagination -->
<MudDataGrid Items="@vm.Items"
             @bind-Page="currentPage"
             RowsPerPage="25" />
```

---

### Issue 5: Binding Entire Entity

```razor
<!-- ⚠️ Caution - Re-renders for any property change -->
<MyComponent Entity="@vm.Value" />

<!-- ✅ Better - Pass only needed properties -->
<MyComponent Name="@vm.Value.Name"
             Description="@vm.Value.Description" />
```

---

## Common Issues

### Issue: UI Not Updating

**Cause**: Entity doesn't implement `INotifyPropertyChanged`.

**Fix**:
```csharp
// ✅ Add ReactiveObject
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;
}
```

**Verify**:
```csharp
// Check if PropertyChanged fires
entity.PropertyChanged += (s, e) => {
    Console.WriteLine($"Property changed: {e.PropertyName}");
};
entity.Name = "New Name";  // Should print "Property changed: Name"
```

---

### Issue: Two-Way Binding Not Working

**Cause**: Missing `@bind-Value` directive.

```razor
<!-- ❌ Wrong - Read-only -->
<MudTextField Value="@vm.Value.Name" />

<!-- ✅ Correct - Two-way binding -->
<MudTextField @bind-Value="vm.Value.Name" />
```

---

### Issue: Binding to Null Values

**Cause**: Property is null.

**Fix**:
```razor
<!-- ❌ Wrong - Crashes if Value is null -->
<MudTextField @bind-Value="vm.Value.Name" />

<!-- ✅ Correct - Null check -->
@if (vm.Value != null)
{
    <MudTextField @bind-Value="vm.Value.Name" />
}

<!-- ✅ Alternative - Null-conditional -->
<MudTextField @bind-Value="vm.Value?.Name" />
```

---

### Issue: Command Not Executing

**Cause**: Command's `CanExecute` returns false.

**Debug**:
```razor
<MudButton Command="@vm.StartCommand"
           Disabled="@(!vm.StartCommand.CanExecute.Value)">
    Start
</MudButton>

<MudText>Can Execute: @vm.StartCommand.CanExecute.Value</MudText>
```

---

### Issue: Memory Leak

**Cause**: Subscriptions not disposed.

**Fix**:
```csharp
@implements IAsyncDisposable

@code {
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
}
```

---

## Related Documentation

- **[MVVM Overview](README.md)** - MVVM architecture overview
- **[ViewModels Guide](viewmodels-guide.md)** - ViewModel patterns
- **[Reactive Patterns](reactive-patterns.md)** - Reactive programming
- **[Blazor MVVM Patterns](../ui/blazor-mvvm-patterns.md)** - UI patterns
- **[Reactive UI Updates](../ui/reactive-ui-updates.md)** - Update flow

---

## Summary

**Binding Types**:

| Type | Syntax | Use Case |
|------|--------|----------|
| **One-Way** | `@vm.Property` | Display data |
| **Two-Way** | `@bind-Value="vm.Property"` | Edit data |
| **Command** | `Command="@vm.MyCommand"` | User actions |
| **Collection** | `Items="@vm.Items"` | Lists/grids |
| **Template** | `<TemplateColumn>` | Custom rendering |
| **Cascading** | `<CascadingValue>` | Pass data down |

**Best Practices**:
1. Always implement `INotifyPropertyChanged` on entities
2. Use `@bind-Value` for two-way binding
3. Dispose subscriptions properly
4. Cache expensive computations with `ToProperty`
5. Use virtualization for large collections
6. Throttle high-frequency updates

**Performance Tips**:
- Avoid binding in loops
- Use `ObservableDataView` for collections
- Cache computed properties
- Combine observables to reduce `StateHasChanged` calls
- Enable virtualization for large lists
