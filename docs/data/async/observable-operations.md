# Observable Operations

## Overview

This guide covers how to work with **observable async operations** in LionFire's data access system. Observable operations provide reactive streams of operation lifecycle, state changes, and results.

**Key Concept**: Every async operation (Get, Set) can be observed as it happens, enabling reactive UIs and operation tracking.

---

## Table of Contents

1. [Observable Interfaces](#observable-interfaces)
2. [Get Operations](#get-operations)
3. [Set Operations](#set-operations)
4. [State Observables](#state-observables)
5. [Result Streams](#result-streams)
6. [UI Integration Patterns](#ui-integration-patterns)
7. [Error Handling](#error-handling)
8. [Performance Considerations](#performance-considerations)

---

## Observable Interfaces

### IObservableGetOperations\<TValue\>

**Purpose**: Stream of get operations as they occur.

```csharp
public interface IObservableGetOperations<out TValue>
{
    IObservable<ITask<IGetResult<TValue>>> GetOperations { get; }
}
```

**Emits**: Each time `Get()` or `GetIfNeeded()` is called.

---

### IObservableGetState

**Purpose**: Observable loading state.

```csharp
public interface IObservableGetState
{
    bool IsLoading { get; }  // Reactive property
}
```

**Changes**: When operation starts/completes.

---

### IObservableGetResults\<TValue\>

**Purpose**: Stream of get operation results.

```csharp
public interface IObservableGetResults<out TValue>
{
    IObservable<IGetResult<TValue>?> GetResults { get; }
}
```

**Emits**: After each get operation completes.

---

### IObservableSetOperations

**Purpose**: Stream of set operations.

```csharp
public interface IObservableSetOperations
{
    IObservable<ITask> SetOperations { get; }
}
```

---

### IObservableSetState

**Purpose**: Observable saving state.

```csharp
public interface IObservableSetState
{
    bool IsSetting { get; }  // Reactive property
}
```

---

### IObservableSetResults\<TValue\>

**Purpose**: Stream of set operation results.

```csharp
public interface IObservableSetResults<out TValue>
{
    IObservable<ISetResult<TValue>> SetResults { get; }
}
```

---

## Get Operations

### Subscribing to Get Operations

```csharp
IGetter<UserData> getter = ...;

// Subscribe to operations (before results available)
getter.GetOperations.Subscribe(async task =>
{
    Console.WriteLine("Get operation started");

    var result = await task;

    Console.WriteLine($"Get completed: {result.IsSuccess}");
});

// Trigger operation
await getter.Get();
```

**Timeline**:
```
Call Get()
    ↓
GetOperations emits ITask  ← Subscribe receives this
    ↓
Task executes
    ↓
GetResults emits IGetResult  ← Subscribe receives this
```

---

### Use Cases for GetOperations

**1. Loading Indicators**:
```csharp
getter.GetOperations
    .Subscribe(_ => ShowLoadingSpinner());
```

**2. Operation Tracking**:
```csharp
getter.GetOperations
    .Subscribe(task => operationTracker.RegisterOperation(task));
```

**3. Cancellation Support**:
```csharp
private CancellationTokenSource? cts;

getter.GetOperations.Subscribe(_ =>
{
    cts = new CancellationTokenSource();
    CancelButton.Enabled = true;
});

void CancelOperation()
{
    cts?.Cancel();
    cts = null;
}
```

---

## Set Operations

### Subscribing to Set Operations

```csharp
ISetter<Config> setter = ...;

// Subscribe to operations
setter.SetOperations.Subscribe(async task =>
{
    Console.WriteLine("Save operation started");
    await task;
    Console.WriteLine("Save completed");
});

// Trigger operation
await setter.Set(newConfig);
```

---

### Subscribing to Set Results

```csharp
setter.SetResults.Subscribe(result =>
{
    if (result.IsSuccess)
    {
        ShowSuccessNotification("Saved successfully");
    }
    else
    {
        ShowErrorNotification($"Save failed: {result.ErrorMessage}");
    }
});
```

---

## State Observables

### IsLoading (Get State)

```csharp
IGetterRxO<Data> getter = ...;

// Subscribe to loading state changes
getter.WhenAnyValue(g => g.IsLoading)
    .Subscribe(isLoading =>
    {
        if (isLoading)
            ShowLoadingSpinner();
        else
            HideLoadingSpinner();
    });
```

**State Changes**:
```
IsLoading: false → true → false
           ↓       ↓       ↓
UI:      Hide   Show    Hide
```

---

### IsSetting (Set State)

```csharp
ISetterRxO<Config> setter = ...;

// Subscribe to saving state changes
setter.WhenAnyValue(s => s.IsSetting)
    .Subscribe(isSetting =>
    {
        SaveButton.Enabled = !isSetting;
        SaveButton.Text = isSetting ? "Saving..." : "Save";
    });
```

---

### Combined State (IValueRxO)

```csharp
IValueRxO<Data> value = ...;

// Combined busy indicator
value.WhenAnyValue(
        v => v.IsLoading,
        v => v.IsSetting,
        (loading, setting) => loading || setting)
    .Subscribe(isBusy =>
    {
        if (isBusy)
            ShowBusyIndicator();
        else
            HideBusyIndicator();
    });
```

---

## Result Streams

### Get Results

```csharp
getter.GetResults.Subscribe(result =>
{
    if (result == null) return;

    if (result.IsSuccess)
    {
        Console.WriteLine($"Loaded: {result.Value}");
        UpdateUI(result.Value);
    }
    else
    {
        Console.WriteLine($"Error: {result.ErrorMessage}");
        ShowErrorDialog(result.ErrorMessage);
    }
});
```

**Filtering**:
```csharp
// Only successful results
getter.GetResults
    .Where(r => r?.IsSuccess == true)
    .Subscribe(result => UpdateUI(result.Value));

// Only errors
getter.GetResults
    .Where(r => r?.IsSuccess == false)
    .Subscribe(result => LogError(result.Error));
```

---

### Set Results

```csharp
setter.SetResults.Subscribe(result =>
{
    if (result.IsSuccess)
    {
        ShowToast("Saved successfully", ToastType.Success);
        NavigateBack();
    }
    else
    {
        ShowToast($"Save failed: {result.ErrorMessage}", ToastType.Error);
    }
});
```

---

## UI Integration Patterns

### Pattern 1: Loading Spinner

```razor
<MudProgressCircular Indeterminate Visible="@getter.IsLoading" />

<MudButton Command="@LoadCommand" Disabled="@getter.IsLoading">
    @(getter.IsLoading ? "Loading..." : "Load Data")
</MudButton>

@code {
    IGetterRxO<Data> getter;

    ReactiveCommand<Unit, Unit> LoadCommand =>
        ReactiveCommand.CreateFromTask(
            async () => await getter.Get(),
            getter.WhenAnyValue(g => g.IsLoading, loading => !loading)
        );
}
```

---

### Pattern 2: Error Display

```razor
@code {
    [Reactive] private string? _errorMessage;

    protected override void OnInitialized()
    {
        getter.GetResults
            .Where(r => r?.IsSuccess == false)
            .Subscribe(result =>
            {
                ErrorMessage = result.ErrorMessage;
                InvokeAsync(StateHasChanged);
            });
    }
}

@if (!string.IsNullOrEmpty(ErrorMessage))
{
    <MudAlert Severity="Severity.Error">
        @ErrorMessage
    </MudAlert>
}
```

---

### Pattern 3: Save Confirmation

```razor
@code {
    [Reactive] private bool _showSaveSuccess;

    protected override void OnInitialized()
    {
        setter.SetResults
            .Where(r => r.IsSuccess)
            .Subscribe(_ =>
            {
                ShowSaveSuccess = true;
                InvokeAsync(StateHasChanged);

                // Hide after 3 seconds
                Observable.Timer(TimeSpan.FromSeconds(3))
                    .Subscribe(_ =>
                    {
                        ShowSaveSuccess = false;
                        InvokeAsync(StateHasChanged);
                    });
            });
    }
}

@if (ShowSaveSuccess)
{
    <MudAlert Severity="Severity.Success">
        Saved successfully!
    </MudAlert>
}
```

---

### Pattern 4: Operation Queue Display

```razor
@code {
    List<OperationInfo> activeOperations = new();

    protected override void OnInitialized()
    {
        // Track all get operations
        getter.GetOperations.Subscribe(task =>
        {
            var opInfo = new OperationInfo("Loading data...");
            activeOperations.Add(opInfo);
            InvokeAsync(StateHasChanged);

            task.ContinueWith(_ =>
            {
                activeOperations.Remove(opInfo);
                InvokeAsync(StateHasChanged);
            });
        });
    }
}

<!-- Display active operations -->
@foreach (var op in activeOperations)
{
    <MudChip>@op.Description</MudChip>
}
```

---

## Error Handling

### Subscribing to Errors

```csharp
// Pattern 1: In result stream
getter.GetResults
    .Where(r => r?.IsSuccess == false)
    .Subscribe(result =>
    {
        Logger.LogError(result.Error, "Get operation failed");
        ShowErrorDialog(result.ErrorMessage);
    });

// Pattern 2: In operation stream
getter.GetOperations
    .SelectMany(task => task.ToObservable())
    .Catch((Exception ex) =>
    {
        Logger.LogError(ex, "Get operation threw exception");
        return Observable.Empty<IGetResult<Data>>();
    })
    .Subscribe();
```

---

### Retry Patterns

```csharp
// Retry on failure
getter.GetResults
    .Where(r => r?.IsSuccess == false)
    .Throttle(TimeSpan.FromSeconds(5))  // Wait before retry
    .Subscribe(async _ =>
    {
        Console.WriteLine("Retrying...");
        getter.DiscardValue();
        await getter.GetIfNeeded();
    });
```

---

## Performance Considerations

### Subscription Management

```csharp
// ✅ Dispose subscriptions
private CompositeDisposable subscriptions = new();

getter.GetOperations.Subscribe(...)
    .DisposeWith(subscriptions);

public void Dispose() => subscriptions.Dispose();
```

---

### Avoid Excessive Subscriptions

```csharp
// ❌ Bad - Creates new subscription on every render
protected override void OnParametersSet()
{
    getter.GetResults.Subscribe(...);  // Leak!
}

// ✅ Good - Single subscription
protected override void OnInitialized()
{
    subscription = getter.GetResults.Subscribe(...);
}

public async ValueTask DisposeAsync()
{
    subscription?.Dispose();
}
```

---

### Throttle High-Frequency Updates

```csharp
// ✅ Throttle rapid changes
getter.GetResults
    .Throttle(TimeSpan.FromMilliseconds(100))
    .Subscribe(result => UpdateUI(result.Value));
```

---

## Related Documentation

- **[Getters and Setters Guide](getters-setters.md)** - Core patterns
- **[Reactive Patterns](../../mvvm/reactive-patterns.md)** - Reactive programming
- **[Reactive UI Updates](../../ui/reactive-ui-updates.md)** - UI update flow
- **[LionFire.Data.Async.Reactive](../../../src/LionFire.Data.Async.Reactive/CLAUDE.md)** - Implementation details

---

## Summary

**Observable Operation Types**:
- **GetOperations** - Stream of get tasks
- **GetResults** - Stream of get results
- **IsLoading** - Reactive loading state
- **SetOperations** - Stream of set tasks
- **SetResults** - Stream of set results
- **IsSetting** - Reactive saving state

**Common Uses**:
- Loading spinners (`IsLoading`)
- Success notifications (`SetResults`)
- Error handling (`GetResults.Where(r => !r.IsSuccess)`)
- Operation tracking (`GetOperations`)

**Best Practice**: Always dispose subscriptions with `CompositeDisposable` and `DisposeWith()`.
