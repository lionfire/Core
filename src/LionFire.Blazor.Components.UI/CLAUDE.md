# LionFire.Blazor.Components.UI

## Overview

**LionFire.Blazor.Components.UI** provides Blazor UI utilities and components, building on LionFire.Blazor.Components with additional UI-focused functionality. This library includes reactive file system access, UI helpers, and integration with LionFire's persistence and reactive frameworks.

**Layer**: Toolkit (Blazor UI Utilities)
**Target**: .NET 9.0
**Root Namespace**: `LionFire`
**SDK**: Microsoft.NET.Sdk.Razor

## Key Dependencies

### NuGet Packages
- **Microsoft.AspNetCore.Components.Web** - ASP.NET Core Blazor components
- **ReactiveUI.SourceGenerators** - Code generation for reactive properties

### LionFire Dependencies
- **LionFire.Blazor.Components** - Base Blazor components with ReactiveUI
- **LionFire.Persistence.Filesystem** - File system persistence backend
- **LionFire.Reactive.Framework** - Reactive extensions and framework

## Core Features

### 1. Reactive File System UI

Integration with filesystem persistence for reactive file-based UI components.

**Example Usage:**
```csharp
@inject IFileSystemPersistence FileSystem

@code {
    private async Task LoadConfiguration()
    {
        var configHandle = FileSystem.GetHandle<AppConfig>("/config/app.hjson");
        var config = await configHandle.Get();

        // React to file changes
        configHandle.GetResults.Subscribe(result =>
        {
            if (result.HasValue)
            {
                ApplyConfiguration(result.Value);
                StateHasChanged();
            }
        });
    }
}
```

### 2. UI Component Utilities

Enhanced UI components and helpers building on the base component library.

### 3. Reactive UI Integration

Seamless integration between ReactiveUI patterns and Blazor components.

**Example:**
```csharp
public partial class ConfigEditorVM : ReactiveObject
{
    [Reactive]
    private AppConfig? _config;

    private readonly IFileSystemPersistence fileSystem;

    public ConfigEditorVM(IFileSystemPersistence fileSystem)
    {
        this.fileSystem = fileSystem;

        // Auto-save on config changes
        this.WhenAnyValue(x => x.Config)
            .Where(c => c != null)
            .Throttle(TimeSpan.FromSeconds(2))
            .Subscribe(async config => await SaveConfig(config!));
    }

    private async Task SaveConfig(AppConfig config)
    {
        var handle = fileSystem.GetHandle<AppConfig>("/config/app.hjson");
        await handle.Set(config);
    }
}
```

## Directory Structure

```
src/LionFire.Blazor.Components.UI/
├── Layouts/                  # Layout components
├── Pages/                    # Page components
├── Component1.razor          # Example component
├── ExampleJsInterop.cs       # JS interop example
└── _Imports.razor            # Shared imports
```

## Design Patterns

### File-Based UI Pattern

Components that load/save state from files:

```csharp
@page "/settings"
@using LionFire.Persistence.Filesystem

<h3>Application Settings</h3>

@if (settings != null)
{
    <EditForm Model="settings" OnValidSubmit="SaveSettings">
        <DataAnnotationsValidator />
        <ValidationSummary />

        <div>
            <label>Theme:</label>
            <InputSelect @bind-Value="settings.Theme">
                <option value="light">Light</option>
                <option value="dark">Dark</option>
            </InputSelect>
        </div>

        <div>
            <label>Language:</label>
            <InputText @bind-Value="settings.Language" />
        </div>

        <button type="submit">Save</button>
    </EditForm>
}

@code {
    [Inject]
    private IFileSystemPersistence FileSystem { get; set; }

    private AppSettings? settings;

    protected override async Task OnInitializedAsync()
    {
        var handle = FileSystem.GetHandle<AppSettings>("/settings/app.hjson");
        settings = await handle.GetOrDefault(() => new AppSettings());
    }

    private async Task SaveSettings()
    {
        var handle = FileSystem.GetHandle<AppSettings>("/settings/app.hjson");
        await handle.Set(settings!);
    }
}
```

### Reactive ViewModel Pattern

ViewModels with automatic persistence:

```csharp
public partial class DocumentEditorVM : ReactiveObject, IDisposable
{
    [Reactive]
    private Document? _document;

    [Reactive]
    private bool _isDirty;

    private readonly IFileSystemPersistence persistence;
    private readonly CompositeDisposable disposables = new();
    private readonly string documentPath;

    public DocumentEditorVM(IFileSystemPersistence persistence, string documentPath)
    {
        this.persistence = persistence;
        this.documentPath = documentPath;

        // Load document
        LoadDocument().FireAndForget();

        // Track changes
        this.WhenAnyValue(x => x.Document)
            .Skip(1) // Skip initial load
            .Subscribe(_ => IsDirty = true)
            .DisposeWith(disposables);

        // Auto-save
        this.WhenAnyValue(x => x.IsDirty)
            .Where(isDirty => isDirty)
            .Throttle(TimeSpan.FromSeconds(5))
            .Subscribe(_ => SaveDocument().FireAndForget())
            .DisposeWith(disposables);
    }

    private async Task LoadDocument()
    {
        var handle = persistence.GetHandle<Document>(documentPath);
        Document = await handle.GetOrDefault(() => new Document());
        IsDirty = false;
    }

    public async Task SaveDocument()
    {
        if (Document == null) return;

        var handle = persistence.GetHandle<Document>(documentPath);
        await handle.Set(Document);
        IsDirty = false;
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}
```

## Common Usage Patterns

### Pattern 1: Configuration Editor

```razor
@page "/config"
@implements IDisposable

<h3>Configuration Editor</h3>

@if (vm != null && vm.Config != null)
{
    <div class="config-editor">
        <div class="field">
            <label>App Name:</label>
            <input @bind="vm.Config.AppName" @bind:event="oninput" />
        </div>

        <div class="field">
            <label>Port:</label>
            <input type="number" @bind="vm.Config.Port" @bind:event="oninput" />
        </div>

        @if (vm.IsDirty)
        {
            <div class="alert">
                Unsaved changes (auto-saving...)
            </div>
        }

        <button @onclick="vm.SaveDocument">Save Now</button>
    </div>
}

@code {
    [Inject]
    private IFileSystemPersistence Persistence { get; set; }

    private ConfigEditorVM? vm;

    protected override void OnInitialized()
    {
        vm = new ConfigEditorVM(Persistence, "/config/app.hjson");
    }

    public void Dispose()
    {
        vm?.Dispose();
    }
}
```

### Pattern 2: File Browser UI

```razor
@page "/files"

<h3>File Browser</h3>

<div class="file-tree">
    @foreach (var file in files)
    {
        <div class="file-item" @onclick="() => OpenFile(file)">
            @file.Name
        </div>
    }
</div>

@if (selectedFile != null)
{
    <div class="file-viewer">
        <h4>@selectedFile.Name</h4>
        <pre>@selectedFile.Content</pre>
    </div>
}

@code {
    [Inject]
    private IFileSystemPersistence FileSystem { get; set; }

    private List<FileInfo> files = new();
    private FileData? selectedFile;

    protected override async Task OnInitializedAsync()
    {
        // List files in directory
        files = await FileSystem.ListFiles("/documents");
    }

    private async Task OpenFile(FileInfo file)
    {
        var handle = FileSystem.GetHandle<FileData>(file.FullPath);
        selectedFile = await handle.Get();
        StateHasChanged();
    }
}
```

### Pattern 3: Live Reload

```csharp
public partial class LiveConfigVM : ReactiveObject
{
    [Reactive]
    private AppConfig? _config;

    private readonly IFileSystemPersistence persistence;
    private IDisposable? fileWatcher;

    public LiveConfigVM(IFileSystemPersistence persistence)
    {
        this.persistence = persistence;

        // Watch file for changes
        var handle = persistence.GetHandle<AppConfig>("/config/app.hjson");

        fileWatcher = handle.GetResults
            .Where(result => result.HasValue)
            .Subscribe(result =>
            {
                Config = result.Value;
                OnConfigChanged(result.Value);
            });

        // Initial load
        LoadConfig().FireAndForget();
    }

    private async Task LoadConfig()
    {
        var handle = persistence.GetHandle<AppConfig>("/config/app.hjson");
        Config = await handle.GetOrDefault(() => new AppConfig());
    }

    private void OnConfigChanged(AppConfig config)
    {
        // React to external file changes
        Console.WriteLine($"Config updated: {config.AppName}");
    }

    public void Dispose()
    {
        fileWatcher?.Dispose();
    }
}
```

## Integration with Other Libraries

### With LionFire.Blazor.Components

Builds on base components:
```csharp
// Use terminal component from base library
<TerminalView ViewModel="@logVM" />

// Add filesystem-backed persistence
@code {
    private async Task SaveLogs()
    {
        var handle = FileSystem.GetHandle<LogData>("/logs/app.hjson");
        await handle.Set(logVM.Lines.Items.ToArray());
    }
}
```

### With LionFire.Reactive.Framework

Uses reactive file watchers:
```csharp
var reader = new HjsonFsDirectoryReaderRx<string, AppConfig>(directoryPath);

reader.Observable.Subscribe(change =>
{
    Console.WriteLine($"Config changed: {change.Key}");
    ApplyConfig(change.Value);
    StateHasChanged();
});
```

### With LionFire.Persistence.Filesystem

Direct integration with filesystem persistence:
```csharp
@inject IFileSystemPersistence FileSystem

@code {
    private async Task LoadData()
    {
        var handle = FileSystem.GetHandle<MyData>("/data/mydata.hjson");
        var data = await handle.Get();
    }
}
```

## Performance Considerations

### Debouncing File Saves

```csharp
this.WhenAnyValue(x => x.Config)
    .Throttle(TimeSpan.FromSeconds(2)) // Wait 2 seconds after last change
    .Subscribe(async config => await SaveConfig(config));
```

### Selective Re-rendering

```csharp
@implements IDisposable

@code {
    private IDisposable? subscription;

    protected override void OnInitialized()
    {
        subscription = vm.WhenAnyValue(x => x.ImportantProperty)
            .Subscribe(_ => StateHasChanged()); // Only re-render on important changes
    }

    public void Dispose() => subscription?.Dispose();
}
```

## Testing Considerations

### Mocking File System

```csharp
var mockFileSystem = new Mock<IFileSystemPersistence>();
mockFileSystem.Setup(x => x.GetHandle<AppConfig>(It.IsAny<string>()))
    .Returns(mockHandle.Object);

var vm = new ConfigEditorVM(mockFileSystem.Object, "/config/app.hjson");

await vm.LoadConfig();

mockHandle.Verify(x => x.Get(), Times.Once);
```

## Related Projects

- **LionFire.Blazor.Components** - Base Blazor components ([CLAUDE.md](../LionFire.Blazor.Components/CLAUDE.md))
- **LionFire.Blazor.Components.MudBlazor** - MudBlazor-specific components
- **LionFire.Persistence.Filesystem** - Filesystem persistence ([CLAUDE.md](../LionFire.Persistence.Filesystem/CLAUDE.md))
- **LionFire.Reactive.Framework** - Reactive extensions

## Summary

**LionFire.Blazor.Components.UI** provides advanced Blazor UI utilities:

- **Reactive File System UI**: Components with filesystem persistence
- **Auto-Save**: Automatic persistence with debouncing
- **Live Reload**: Watch files for external changes
- **File Browsers**: UI components for file navigation
- **Configuration Editors**: Edit HJSON/JSON config files
- **ReactiveUI Integration**: Seamless reactive patterns

**Key Strengths:**
- File-based UI state management
- Reactive file watching
- Automatic persistence
- Clean separation of concerns

**Use When:**
- Need file-based configuration UIs
- Want auto-save functionality
- Require live file reload
- Building file browser interfaces
- Need filesystem-backed state

**Typical Use Cases:**
- Settings/configuration editors
- File browsers and viewers
- Development tools with file watching
- Auto-saving editors
- Live-reloading dashboards
