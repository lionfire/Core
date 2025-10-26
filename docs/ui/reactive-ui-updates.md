# Reactive UI Updates

## Overview

This document explains how reactive updates flow from the file system through observable collections to Blazor UI components in LionFire. Understanding this flow is essential for debugging update issues, optimizing performance, and building responsive applications.

**Key Concept**: Changes propagate automatically through multiple layers using **DynamicData observables** and **ReactiveUI bindings**.

---

## Table of Contents

1. [Update Flow Overview](#update-flow-overview)
2. [Layer-by-Layer Breakdown](#layer-by-layer-breakdown)
3. [Change Detection Mechanisms](#change-detection-mechanisms)
4. [StateHasChanged Optimization](#statehaschanged-optimization)
5. [Performance Considerations](#performance-considerations)
6. [Troubleshooting Updates](#troubleshooting-updates)

---

## Update Flow Overview

### Complete Flow Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                      File System                                 │
│  User edits: workspace1/Bots/bot1.hjson                         │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ↓ FileSystemWatcher detects change
┌─────────────────────────────────────────────────────────────────┐
│                 Persistence Layer                                │
│  HjsonFsDirectoryReaderRx<string, BotEntity>                    │
│  - Reloads file from disk                                       │
│  - Deserializes HJSON → BotEntity                               │
│  - Publishes to observable                                      │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ↓ IObservableReader.Values.Connect()
┌─────────────────────────────────────────────────────────────────┐
│              Observable Cache (DynamicData)                      │
│  SourceCache<BotEntity, string>                                 │
│  - Emits IChangeSet<BotEntity, string>                          │
│  - Change types: Add, Update, Remove, Refresh                   │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ↓ Subscription in VM or Component
┌─────────────────────────────────────────────────────────────────┐
│                    ViewModel Layer                               │
│  ObservableDataVM<string, BotEntity, BotVM>                     │
│  - Processes changeSet                                          │
│  - Creates/updates/removes VMs                                  │
│  - Updates Items observable cache                               │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ↓ Items.Connect() / PropertyChanged events
┌─────────────────────────────────────────────────────────────────┐
│                   Blazor Component                               │
│  ObservableDataView / Manual Component                          │
│  - Receives change notification                                 │
│  - Calls StateHasChanged()                                      │
│  - Blazor re-renders affected parts                             │
└─────────────────────────────────────────────────────────────────┘
                     │
                     ↓ Render tree diff
┌─────────────────────────────────────────────────────────────────┐
│                       UI Display                                 │
│  MudDataGrid / Form elements                                    │
│  - DOM updates applied                                          │
│  - User sees updated data                                       │
└─────────────────────────────────────────────────────────────────┘
```

---

## Layer-by-Layer Breakdown

### Layer 1: File System Monitoring

**Component**: `HjsonFsDirectoryReaderRx`

**Mechanism**: `FileSystemWatcher` monitors workspace directory.

```csharp
// Internal implementation (simplified)
public class HjsonFsDirectoryReaderRx<TKey, TValue>
{
    private FileSystemWatcher watcher;
    private SourceCache<TValue, TKey> cache;

    public HjsonFsDirectoryReaderRx(DirectoryReferenceSelector dirSelector)
    {
        // Set up file watcher
        watcher = new FileSystemWatcher(dirSelector.Path);
        watcher.Changed += OnFileChanged;
        watcher.Created += OnFileCreated;
        watcher.Deleted += OnFileDeleted;
        watcher.EnableRaisingEvents = true;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        // Reload file
        var entity = DeserializeFile<TValue>(e.FullPath);
        var key = GetKeyFromPath(e.FullPath);

        // Update cache (triggers observable)
        cache.AddOrUpdate(entity, key);
    }
}
```

**Triggers**:
- File created in workspace directory
- File modified by user or external process
- File deleted from directory

**Debouncing**: Multiple rapid changes to same file are debounced to prevent excessive updates.

---

### Layer 2: Observable Cache (DynamicData)

**Component**: `SourceCache<TValue, TKey>`

**Purpose**: Manages in-memory cache with change notifications.

**Change Types**:
```csharp
public enum ChangeReason
{
    Add,        // New item added
    Update,     // Existing item modified
    Remove,     // Item deleted
    Refresh,    // Item reference changed but key same
    Moved       // Item moved (rare)
}
```

**Change Propagation**:
```csharp
// Consumers subscribe to changes
IObservable<IChangeSet<TValue, TKey>> observable = cache.Connect();

observable.Subscribe(changeSet =>
{
    foreach (var change in changeSet)
    {
        switch (change.Reason)
        {
            case ChangeReason.Add:
                // Handle new item
                break;
            case ChangeReason.Update:
                // Handle updated item
                // change.Current = new value
                // change.Previous = old value
                break;
            case ChangeReason.Remove:
                // Handle removed item
                break;
        }
    }
});
```

---

### Layer 3: ViewModel Processing

**Component**: `ObservableDataVM<TKey, TValue, TValueVM>`

**Responsibility**: Convert entity changes to ViewModel changes.

```csharp
// Simplified implementation
public partial class ObservableDataVM<TKey, TValue, TValueVM>
{
    private SourceCache<TValueVM, TKey> itemsCache = new();

    public ObservableDataVM(IObservableReader<TKey, TValue> reader)
    {
        // Subscribe to entity changes
        reader.Values.Connect()
            .Subscribe(changeSet =>
            {
                itemsCache.Edit(updater =>
                {
                    foreach (var change in changeSet)
                    {
                        switch (change.Reason)
                        {
                            case ChangeReason.Add:
                                // Create VM for new entity
                                var vm = CreateVM(change.Key, change.Current);
                                updater.AddOrUpdate(vm, change.Key);
                                break;

                            case ChangeReason.Update:
                                // Update existing VM
                                if (updater.Lookup(change.Key).HasValue)
                                {
                                    var existingVM = updater.Lookup(change.Key).Value;
                                    UpdateVM(existingVM, change.Current);
                                    updater.Refresh(change.Key);
                                }
                                break;

                            case ChangeReason.Remove:
                                updater.Remove(change.Key);
                                break;
                        }
                    }
                });
            });
    }

    public IObservable<IChangeSet<TValueVM, TKey>> ItemsChanged =>
        itemsCache.Connect();
}
```

---

### Layer 4: Blazor Component Subscription

**Pattern 1**: ObservableDataView (Automatic)

```csharp
// ObservableDataView.razor.cs (simplified)
public partial class ObservableDataView<TKey, TValue, TValueVM>
{
    private IDisposable? subscription;

    protected override void OnParametersSet()
    {
        if (ViewModel != null)
        {
            // Subscribe to VM changes
            subscription = ViewModel.ItemsChanged
                .Subscribe(_ => InvokeAsync(StateHasChanged));
        }
    }

    public async ValueTask DisposeAsync()
    {
        subscription?.Dispose();
    }
}
```

**Pattern 2**: Manual Component (Manual Subscription)

```razor
@implements IAsyncDisposable

@code {
    IDisposable? subscription;

    protected override void OnInitialized()
    {
        // Subscribe to value changes
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

### Layer 5: Blazor Render System

**Component**: Blazor's diff algorithm

**Process**:
1. `StateHasChanged()` called
2. Component re-renders to virtual DOM
3. Diff algorithm compares old vs new render tree
4. Minimal DOM updates applied

**Optimization**: Only changed elements are updated, not entire page.

---

## Change Detection Mechanisms

### Mechanism 1: File System Watching

**Trigger**: External file changes

**Flow**:
```
File modified on disk
    ↓
FileSystemWatcher event
    ↓
HjsonFsDirectoryReaderRx reloads file
    ↓
Cache updated
    ↓
Observable fires
```

**Latency**: ~10-100ms depending on OS

---

### Mechanism 2: Property Change Notifications

**Trigger**: In-memory entity property changes

**Requirements**:
- Entity implements `INotifyPropertyChanged`
- Property raises `PropertyChanged` event

**Example**:
```csharp
// Entity with ReactiveObject
public partial class BotEntity : ReactiveObject
{
    [Reactive] private string? _name;  // Generates PropertyChanged

    // When Name changes:
    // 1. PropertyChanged("Name") event fires
    // 2. ReactiveUI observables emit
    // 3. Bound UI elements update
}
```

**Flow**:
```
User edits text field
    ↓
@bind-Value updates entity.Name
    ↓
entity.PropertyChanged("Name") fires
    ↓
Blazor detects change
    ↓
StateHasChanged() called
```

---

### Mechanism 3: Observable Collection Changes

**Trigger**: Items added/removed from collection

**Example**:
```csharp
// DynamicData observable
vm.Items.Connect()
    .Subscribe(changeSet =>
    {
        // ChangeSet contains:
        // - Added items
        // - Updated items
        // - Removed items
        InvokeAsync(StateHasChanged);
    });
```

---

## StateHasChanged Optimization

### Problem: Excessive Re-Renders

**Naive Approach**:
```csharp
// ❌ Calls StateHasChanged for EVERY change
reader.Values.Connect()
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            ProcessChange(change);
            StateHasChanged();  // Too many calls!
        }
    });
```

### Solution 1: Batch Updates

```csharp
// ✅ Process all changes, then call StateHasChanged once
reader.Values.Connect()
    .Subscribe(changeSet =>
    {
        foreach (var change in changeSet)
        {
            ProcessChange(change);
        }
        InvokeAsync(StateHasChanged);  // Single call for entire batch
    });
```

---

### Solution 2: Throttling

```csharp
// ✅ Throttle rapid updates
reader.Values.Connect()
    .Throttle(TimeSpan.FromMilliseconds(100))  // Wait 100ms after last change
    .Subscribe(_ =>
    {
        InvokeAsync(StateHasChanged);
    });
```

**Use Case**: High-frequency updates (e.g., real-time price feeds).

---

### Solution 3: ObserveOn (UI Thread)

```csharp
// ✅ Automatically marshal to UI thread
reader.Values.Connect()
    .ObserveOn(SynchronizationContext.Current)
    .Subscribe(_ =>
    {
        StateHasChanged();  // Already on UI thread, no InvokeAsync needed
    });
```

---

## Performance Considerations

### Issue 1: Large Collections

**Problem**: Rendering 1000+ items is slow.

**Solution 1**: Virtualization

```razor
<MudDataGrid Virtualize="true"
             ItemSize="48"
             Items="@items" />
```

**Solution 2**: Pagination

```razor
<MudDataGrid Items="@items"
             @bind-Page="currentPage"
             RowsPerPage="25" />
```

---

### Issue 2: Unnecessary Subscriptions

**Problem**: Multiple subscriptions to same observable.

```csharp
// ❌ Each component subscribes separately
<ComponentA @ref="compA" />  <!-- Subscribes to reader -->
<ComponentB @ref="compB" />  <!-- Subscribes to reader -->
<ComponentC @ref="compC" />  <!-- Subscribes to reader -->
```

**Solution**: Share subscription via ViewModel

```csharp
// ✅ Single subscription in shared VM
<ParentComponent VM="@sharedVM">
    <ComponentA VM="@sharedVM" />
    <ComponentB VM="@sharedVM" />
    <ComponentC VM="@sharedVM" />
</ParentComponent>
```

---

### Issue 3: Heavy Computations in Render

**Problem**: Expensive calculations during render.

```razor
<!-- ❌ Recalculates on every render -->
<MudText>@CalculateComplexMetric(data)</MudText>
```

**Solution**: Memoization

```razor
<!-- ✅ Cache computed values -->
<MudText>@cachedMetric</MudText>

@code {
    string cachedMetric;

    protected override void OnParametersSet()
    {
        cachedMetric = CalculateComplexMetric(data);
    }
}
```

---

## Troubleshooting Updates

### Issue: UI Not Updating

**Checklist**:

1. **Does entity implement INotifyPropertyChanged?**
   ```csharp
   // ❌ Wrong
   public class Bot { public string Name { get; set; } }

   // ✅ Correct
   public partial class Bot : ReactiveObject
   {
       [Reactive] private string? _name;
   }
   ```

2. **Is component subscribed to changes?**
   ```csharp
   // Check for subscription
   subscription = reader.Values.Connect().Subscribe(...);
   ```

3. **Is StateHasChanged called?**
   ```csharp
   // Add logging
   .Subscribe(_ => {
       Console.WriteLine("Change detected");
       InvokeAsync(StateHasChanged);
   });
   ```

4. **Is subscription disposed prematurely?**
   ```csharp
   // Check DisposeAsync is not called too early
   public async ValueTask DisposeAsync()
   {
       Console.WriteLine("Disposing subscription");
       subscription?.Dispose();
   }
   ```

---

### Issue: Updates Too Slow

**Causes**:
- No file system watching (polling instead)
- Excessive re-renders
- Heavy computations in render
- Large collection without virtualization

**Debugging**:
```csharp
// Add timing
var sw = Stopwatch.StartNew();
reader.Values.Connect()
    .Subscribe(changeSet => {
        sw.Stop();
        Console.WriteLine($"Update latency: {sw.ElapsedMilliseconds}ms");
        sw.Restart();
    });
```

---

### Issue: Memory Leaks

**Cause**: Subscriptions not disposed.

**Fix**:
```csharp
// ✅ Always implement IAsyncDisposable
@implements IAsyncDisposable

@code {
    private CompositeDisposable subscriptions = new();

    protected override void OnInitialized()
    {
        reader.Values.Connect()
            .Subscribe(...)
            .DisposeWith(subscriptions);
    }

    public async ValueTask DisposeAsync()
    {
        subscriptions.Dispose();
    }
}
```

---

## Advanced Patterns

### Pattern: Selective Updates

**Scenario**: Only update UI for specific properties.

```csharp
// Subscribe to specific property changes
this.WhenAnyValue(x => x.Value.Name)
    .DistinctUntilChanged()
    .Subscribe(_ => InvokeAsync(StateHasChanged));
```

---

### Pattern: Debouncing User Input

**Scenario**: Save after user stops typing.

```razor
<MudTextField @bind-Value="entity.Name"
              @bind-Value:after="OnNameChanged" />

@code {
    private Subject<string> nameChanges = new();

    protected override void OnInitialized()
    {
        nameChanges
            .Throttle(TimeSpan.FromMilliseconds(500))
            .Subscribe(async _ => await SaveChanges());
    }

    private void OnNameChanged()
    {
        nameChanges.OnNext(entity.Name);
    }
}
```

---

### Pattern: Optimistic Updates

**Scenario**: Update UI immediately, save in background.

```csharp
private async Task UpdateProperty(string value)
{
    // Update UI immediately
    entity.Name = value;
    StateHasChanged();

    // Save in background
    await writer.Write(entity.Key, entity);
}
```

---

## Reactive Update Timeline

**Typical latency** for file change → UI update:

```
File System Change:        0ms
    ↓
FileSystemWatcher:         ~10-50ms (OS dependent)
    ↓
File Read & Deserialize:   ~5-20ms (file size dependent)
    ↓
Cache Update:              ~1ms
    ↓
Observable Notification:   ~1ms
    ↓
VM Processing:             ~1-5ms (collection size dependent)
    ↓
Component Subscription:    ~1ms
    ↓
InvokeAsync:               ~1-10ms (render queue)
    ↓
StateHasChanged:           ~1ms
    ↓
Blazor Diff:               ~5-50ms (complexity dependent)
    ↓
DOM Update:                ~5-20ms (browser dependent)

Total:                     ~30-200ms
```

**Factors Affecting Latency**:
- File size (larger = slower deserialize)
- Collection size (more items = slower VM creation)
- Render complexity (more elements = slower diff)
- Network latency (for Blazor Server)

---

## Related Documentation

- **[Blazor MVVM Patterns](blazor-mvvm-patterns.md)** - When to use each pattern
- **[Component Catalog](component-catalog.md)** - All available components
- **[LionFire.Reactive](../../src/LionFire.Reactive/CLAUDE.md)** - Observable infrastructure
- **[DynamicData Documentation](https://github.com/reactivemarbles/DynamicData)** - Reactive collections library
- **[ReactiveUI Documentation](https://www.reactiveui.net/)** - Reactive MVVM framework

---

## Summary

**Key Takeaways**:

1. **Updates flow automatically** through multiple layers using DynamicData and ReactiveUI
2. **Subscriptions must be disposed** to prevent memory leaks
3. **StateHasChanged optimization** is critical for performance
4. **Entities must implement INotifyPropertyChanged** for property-level updates
5. **File system watching** enables external change detection

**Best Practices**:
- Always use `IAsyncDisposable` and dispose subscriptions
- Batch updates before calling `StateHasChanged`
- Use throttling for high-frequency updates
- Implement virtualization for large collections
- Memoize expensive computations

**Common Pitfall**: Forgetting to implement `INotifyPropertyChanged` on entities, resulting in non-reactive UI.
