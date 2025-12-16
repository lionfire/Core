# ServiceProvider Disposal Race Condition Analysis

## Status: Resolved (2025-12-16)

The critical issues have been fixed. Some inherent async/Rx design tradeoffs remain but are mitigated.

## Problem Summary

We reach `C:\src\Core\src\LionFire.Reactive.Framework\IO\Reactive\Filesystem\FsObservableCollectionFactoryX.cs` line 63 because `C:\src\Core\src\LionFire.Workspaces.UI.Blazor\UI\Workspaces\Layouts\WorkspaceLayoutVM.cs` ServiceProvider gets disposed while async reactive subscriptions are still executing.

## Root Cause

There are **two disposal race conditions**:

### 1. UserId Subscription Leak (UserLayoutVM.cs:36) — ✅ FIXED

```csharp
// OLD (leaked):
this.WhenAnyValue(x => x.UserId).Subscribe(async _ => await OnUserChanged());

// NEW (fixed):
this.WhenAnyValue(x => x.UserId).Subscribe(async _ => await OnUserChanged()).DisposeWith(disposables);
```

**RESOLVED**: The `.DisposeWith(disposables)` has been added.

### 2. Async Subscription Race (WorkspaceLayoutVM.cs:43-46) — ⚠️ MITIGATED

```csharp
this.WhenAnyValue(x => x.WorkspaceId)
    .DistinctUntilChanged()
    .Subscribe(async workspaceId => await OnWorkspaceChanged())
    .DisposeWith(disposables);
```

While this IS disposed, the `async` handler means disposal doesn't wait for completion. The handler can still be running after disposal.

**MITIGATION**: IsDisposed checks and try/catch blocks have been added (see below).

## The Call Chain to Line 63

Here's how we reach FsObservableCollectionFactoryX.cs:63:

1. **UserId or WorkspaceId changes** → fires subscription
2. → **OnUserChanged()** or **OnWorkspaceChanged()** (async)
3. → **ConfigureUserServices()** (line 77-149 in WorkspaceLayoutVM.cs)
4. → **RegisterObservablesInDir()** (line 133) with `ServiceProvider`
5. → **FsObservableCollectionFactoryX.RegisterObservablesInDir()** (line 42)
6. → **serviceProvider.GetService()** (line 58)
7. → **ObjectDisposedException** caught, returns `null!` (line 63)

Meanwhile, the Blazor component's scoped ServiceProvider has been disposed, but the async operation is still running.

## IsDisposed Checks — ✅ NOW ADDED

**Previous state**: `IsDisposed` check only in `ConfigureWorkspaceServices`, not in `ConfigureUserServices`.

**Current state** (WorkspaceLayoutVM.cs):
- Line 79: `if (IsDisposed) return;` — early exit at method start
- Lines 137-144: Additional `IsDisposed` check before `RegisterObservablesInDir` call
- Lines 146-149: `try/catch (ObjectDisposedException)` wrapping the call

**Note**: TOCTOU (time-of-check-time-of-use) race still theoretically possible but now gracefully handled.

## Code Locations (Updated)

### FsObservableCollectionFactoryX.cs:53-70 — ✅ NOW FULLY PROTECTED
```csharp
DirectoryTypeOptions DirectoryTypeOptions;
HjsonFsDirectoryReaderRx<string, TValue> r;
HjsonFsDirectoryWriterRx<string, TValue> w;
try
{
    DirectoryTypeOptions = new DirectoryTypeOptions
    {
        ExtensionConvention = serviceProvider.GetService<IFileExtensionConvention>() ?? Singleton<DefaultExtensionConvention>.Instance,
    };
    DirectoryTypeOptions.SecondExtension = DirectoryTypeOptions.ExtensionConvention.FileExtensionForType(typeof(TValue));

    r = ActivatorUtilities.CreateInstance<HjsonFsDirectoryReaderRx<string, TValue>>(serviceProvider, dirSelector, DirectoryTypeOptions);
    w = ActivatorUtilities.CreateInstance<HjsonFsDirectoryWriterRx<string, TValue>>(serviceProvider, dirSelector, DirectoryTypeOptions);
}
catch (ObjectDisposedException)
{
    return null!; // NULLABILITY OVERRIDE - ServiceProvider disposed during async race
}
```
**Note**: The `null!` return is handled by the caller at line 33 which checks `if (x != null)` before registering services. All ServiceProvider usages are now inside the try/catch block.

### UserLayoutVM.cs:36 — ✅ FIXED
```csharp
// Now properly disposed:
this.WhenAnyValue(x => x.UserId).Subscribe(async _ => await OnUserChanged()).DisposeWith(disposables);
```

### WorkspaceLayoutVM.cs:43-46 (async race — inherent design tradeoff)
```csharp
this.WhenAnyValue(x => x.WorkspaceId)
    .DistinctUntilChanged()
    .Subscribe(async workspaceId => await OnWorkspaceChanged())
    .DisposeWith(disposables);
```

### WorkspaceLayoutVM.cs:135-149 — ✅ NOW PROTECTED
```csharp
try
{
    if (IsDisposed)
    {
        Debug.WriteLine("Workspace.LayoutVM.ConfigureUserServices: already Disposed");
    }
    else
    {
        services.RegisterObservablesInDir<Workspace>(ServiceProvider, new DirectoryReferenceSelector(userWorkspacesService.UserWorkspaces) { Recursive = true });
    }
}
catch (ObjectDisposedException)
{
}
```

## Solutions — Status Update

### Option 1: Add IsDisposed check before ServiceProvider usage — ✅ DONE

```csharp
protected override async ValueTask ConfigureUserServices(IServiceCollection services)
{
    if (IsDisposed) return;  // ✅ Added at line 79
    // ...
}
```

### Option 2: Fix the subscription leak in UserLayoutVM.cs:36 — ✅ DONE

```csharp
this.WhenAnyValue(x => x.UserId)
    .Subscribe(async _ => await OnUserChanged())
    .DisposeWith(disposables);  // ✅ Added
```

### Option 3: Guard ServiceProvider access with IsDisposed — ✅ DONE

```csharp
if (!IsDisposed)  // ✅ Added at lines 137-144
{
    services.RegisterObservablesInDir<Workspace>(...);
}
```

### Option 4: Use a cancellation token — ❌ NOT YET IMPLEMENTED

This would be the cleanest solution for fully eliminating the async race:

```csharp
private CancellationTokenSource cts = new();

protected override async ValueTask ConfigureUserServices(IServiceCollection services)
{
    var token = cts.Token;
    token.ThrowIfCancellationRequested();

    // Pass token through all async operations
    await DoWorkAsync(token);
}

public void Dispose()
{
    cts.Cancel();
    disposables.Dispose();
}
```

**Status**: Not implemented. Current mitigations (IsDisposed + try/catch) are sufficient for most use cases.

## Lifecycle Mismatch

The fundamental issue is a **lifecycle mismatch** between:
- **Blazor's scoped ServiceProvider** (disposed when component is disposed)
- **Reactive subscriptions** (may continue executing async operations after disposal)

The `ServiceProvider` stored in WorkspaceLayoutVM (line 27) is likely a **scoped ServiceProvider** from Blazor. When the Blazor component is disposed:

1. The scoped ServiceProvider gets disposed
2. But the reactive subscription (`DisposeWith(disposables)`) might not be disposed yet, or might fire one last time
3. The subscription handler tries to use the now-disposed ServiceProvider
4. `ObjectDisposedException` is thrown at FsObservableCollectionFactoryX.cs:58
5. Caught and silently returns `null!` at line 63

## Recommended Approach — ✅ MOSTLY COMPLETE

| Fix | Status |
|-----|--------|
| Fix subscription leak in UserLayoutVM.cs:36 | ✅ Done |
| Add IsDisposed guards in `ConfigureUserServices` | ✅ Done |
| Add IsDisposed guards in `ConfigureWorkspaceServices` | ✅ Done |
| Extend try/catch in FsObservableCollectionFactoryX to cover all ServiceProvider usages | ✅ Done (2025-12-16) |
| Consider adding CancellationToken | ❌ Optional (current mitigations sufficient) |
| Add unit tests for disposal scenarios | ⚠️ Recommended |

## Remaining Considerations

### Async Race (Inherent Design Tradeoff)
The `async` lambda in `Subscribe()` creates fire-and-forget semantics. When `disposables.Dispose()` is called:
1. The subscription is unsubscribed (no new events)
2. **BUT** any already-executing async handler continues to run
3. That handler may still try to access the disposed `ServiceProvider`

The current mitigations (IsDisposed checks + try/catch) handle this gracefully but don't prevent it.

### If Issues Recur
If disposal race conditions continue to cause problems, consider:
1. **CancellationToken pattern** (Option 4 above)
2. **AsyncLock** to serialize operations
3. **WeakReference** to ServiceProvider
4. Architectural change to **synchronous configuration** during initialization only
