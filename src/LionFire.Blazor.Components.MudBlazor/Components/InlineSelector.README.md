# InlineSelector Component

A flexible MudBlazor-based component for displaying selectable items inline with support for favorites, dropdown view, and item creation.

## Features

### Core Features
- **Inline Display**: Items are displayed horizontally with the selected item highlighted
- **Selected Item Styling**: Selected item has a filled style with bright background color
- **Unselected Item Styling**: Unselected items have transparent background and no border

### Advanced Features
- **Favorites Filter** (`FavoriteFunc`): When set, only displays items marked as favorite
- **Dropdown View** (`ShowDropdown`): Shows a dropdown arrow on hover that displays all available items with star indicators
- **Toggle Favorites** (`SetFavoriteFunc`): Allows users to mark/unmark items as favorites in the dropdown
- **Item Creation** (`CreateFunc`): Shows a + button on hover for creating new items (stacked above dropdown button on the right)
- **Multiple Size Options** (`Size`): Seven different display sizes from large buttons to colored squares
- **Color Customization** (`ItemColorSource`, `SelectedItemColor`): Custom colors per item with automatic rotation fallback

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `IEnumerable<TItem>` | Empty | Collection of items to display |
| `SelectedItem` | `TItem?` | null | Currently selected item |
| `SelectedItemChanged` | `EventCallback<TItem>` | - | Callback when selection changes |
| `ItemTemplate` | `RenderFragment<TItem>` | `item?.ToString()` | Template for rendering each item |
| `FavoriteFunc` | `Func<TItem, bool>?` | null | Function to determine if item is favorite. When set, only favorites are displayed inline |
| `SetFavoriteFunc` | `Func<TItem, Task>?` | null | Function to toggle favorite status. Enables star clicking in dropdown |
| `ShowDropdown` | `bool` | false | Whether to show dropdown button on hover (displays on right) |
| `CreateFunc` | `Func<Task>?` | null | Function to create new items. Shows + button when set (displays on right, above dropdown) |
| `ContainerStyle` | `string?` | null | Custom CSS styles for the container |
| `Size` | `InlineSelectorSize` | `Medium` | Display size: `Medium`, `Large`, `Small`, `Compact`, `Abbreviated`, `SingleLetter`, or `Square` |
| `AbbreviatedMaxLength` | `int` | 3 | Maximum characters when `Size` is `Abbreviated` |
| `ItemTextFunc` | `Func<TItem, string>?` | null | Function to extract text from item for Abbreviated/SingleLetter modes |
| `ItemColorSource` | `Func<TItem, Color>?` | null | Function to get color for each item. If null, rotates through default colors |
| `SelectedItemColor` | `Color?` | null | Custom color for the selected item. If null, uses MudBlazor's Primary color |
| `ItemMinWidth` | `string?` | null | Minimum width for items in CSS units (e.g., "12px", "0.5rem"). Overrides size defaults |
| `ItemMinHeight` | `string?` | null | Minimum height for items in CSS units (e.g., "12px", "0.5rem"). Overrides size defaults |

## Usage Examples

### Basic Usage

```razor
<InlineSelector TItem="string"
                Items="@workspaces"
                SelectedItem="@currentWorkspace"
                SelectedItemChanged="@OnWorkspaceChanged" />

@code {
    private List<string> workspaces = new() { "Dev", "Staging", "Prod" };
    private string currentWorkspace = "Dev";

    private async Task OnWorkspaceChanged(string workspace)
    {
        currentWorkspace = workspace;
        await Task.CompletedTask;
    }
}
```

### With Favorites Only

```razor
<InlineSelector TItem="WorkspaceInfo"
                Items="@allWorkspaces"
                SelectedItem="@selectedWorkspace"
                SelectedItemChanged="@OnWorkspaceChanged"
                ItemTemplate="@((w) => @<span>@w.Name</span>)"
                FavoriteFunc="@((w) => w.IsFavorite)" />
```

### Full Featured (Favorites + Dropdown + Creation)

```razor
<InlineSelector TItem="WorkspaceInfo"
                Items="@allWorkspaces"
                SelectedItem="@selectedWorkspace"
                SelectedItemChanged="@OnWorkspaceChanged"
                ItemTemplate="@workspaceTemplate"
                FavoriteFunc="@((w) => w.IsFavorite)"
                SetFavoriteFunc="@ToggleFavorite"
                ShowDropdown="true"
                CreateFunc="@CreateNewWorkspace" />

@code {
    public class WorkspaceInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsFavorite { get; set; }
    }

    private List<WorkspaceInfo> allWorkspaces = new();
    private WorkspaceInfo? selectedWorkspace;

    private RenderFragment<WorkspaceInfo> workspaceTemplate = (w) => __builder =>
    {
        <MudText Typo="Typo.button">@w.Name</MudText>
    };

    private async Task OnWorkspaceChanged(WorkspaceInfo workspace)
    {
        selectedWorkspace = workspace;
        // Save selection
    }

    private async Task ToggleFavorite(WorkspaceInfo workspace)
    {
        workspace.IsFavorite = !workspace.IsFavorite;
        // Persist favorite status
    }

    private async Task CreateNewWorkspace()
    {
        // Show dialog or navigate to creation page
    }
}
```

### Size Variations

```razor
<!-- Large buttons -->
<InlineSelector TItem="string"
                Items="@workspaces"
                SelectedItem="@selected"
                SelectedItemChanged="@OnChanged"
                Size="InlineSelectorSize.Large" />

<!-- Compact size -->
<InlineSelector TItem="string"
                Items="@workspaces"
                SelectedItem="@selected"
                SelectedItemChanged="@OnChanged"
                Size="InlineSelectorSize.Compact" />

<!-- Abbreviated (3 chars max) -->
<InlineSelector TItem="string"
                Items="@workspaces"
                SelectedItem="@selected"
                SelectedItemChanged="@OnChanged"
                Size="InlineSelectorSize.Abbreviated"
                AbbreviatedMaxLength="3" />

<!-- Single letter -->
<InlineSelector TItem="string"
                Items="@workspaces"
                SelectedItem="@selected"
                SelectedItemChanged="@OnChanged"
                Size="InlineSelectorSize.SingleLetter" />

<!-- Colored squares (no text) -->
<InlineSelector TItem="string"
                Items="@workspaces"
                SelectedItem="@selected"
                SelectedItemChanged="@OnChanged"
                Size="InlineSelectorSize.Square" />
```

### Custom Colors

```razor
<!-- Custom color per item -->
<InlineSelector TItem="Workspace"
                Items="@workspaces"
                SelectedItem="@selected"
                SelectedItemChanged="@OnChanged"
                ItemColorSource="@((w) => w.Color)"
                Size="InlineSelectorSize.Square" />

<!-- Custom selected item color -->
<InlineSelector TItem="string"
                Items="@workspaces"
                SelectedItem="@selected"
                SelectedItemChanged="@OnChanged"
                SelectedItemColor="Color.Success" />

@code {
    public class Workspace
    {
        public string Name { get; set; } = "";
        public MudBlazor.Color Color { get; set; }
    }
}
```

### Custom Item Sizes

```razor
<!-- Custom minimum width and height -->
<InlineSelector TItem="string"
                Items="@workspaces"
                SelectedItem="@selected"
                SelectedItemChanged="@OnChanged"
                Size="InlineSelectorSize.Square"
                ItemMinWidth="48px"
                ItemMinHeight="48px" />

<!-- Abbreviated with custom minimum width -->
<InlineSelector TItem="string"
                Items="@workspaces"
                SelectedItem="@selected"
                SelectedItemChanged="@OnChanged"
                Size="InlineSelectorSize.Abbreviated"
                ItemMinWidth="2rem"
                ItemMinHeight="1.5rem" />
```

## Styling

The component includes scoped CSS that can be overridden:

- `.inline-selector-container`: Main container
- `.inline-selector-items`: Items wrapper
- `.inline-selector-item`: Individual item (unselected)
- `.inline-selector-item-selected`: Selected item
- `.inline-selector-actions`: Action buttons container (right side)
- `.inline-selector-dropdown-button`: Dropdown arrow button
- `.inline-selector-create-button`: Create (+) button
- `.inline-selector-compact`: Compact size styling
- `.inline-selector-square`: Square size styling
- `.inline-selector-dropdown`: Dropdown list container
- `.inline-selector-dropdown-item`: Individual dropdown item

### Custom Container Styling

Use the `ContainerStyle` parameter for custom styling:

```razor
<InlineSelector TItem="string"
                Items="@items"
                SelectedItem="@selected"
                SelectedItemChanged="@OnChanged"
                ContainerStyle="background: linear-gradient(to right, #667eea, #764ba2); padding: 16px; border-radius: 12px;" />
```

## Integration with Existing WorkspaceSelector

To integrate with your existing workspace system:

```razor
@using LionFire.Reactive.Persistence

<InlineSelector TItem="string"
                Items="@(Workspaces?.Keys.Keys ?? Enumerable.Empty<string>())"
                SelectedItem="@currentWorkspace"
                SelectedItemChanged="@OnWorkspaceChanged"
                ShowDropdown="true" />

@code {
    [CascadingParameter(Name = "UserServices")]
    public IServiceProvider? UserServices { get; set; }

    IObservableReaderWriter<string, Workspace>? Workspaces;
    private string? currentWorkspace;

    protected override void OnInitialized()
    {
        Workspaces = UserServices?.GetRequiredService<IObservableReaderWriter<string, Workspace>>();
    }

    private async Task OnWorkspaceChanged(string workspace)
    {
        currentWorkspace = workspace;
        // Handle workspace change
    }
}
```

## Notes

- The component is generic and can work with any type `TItem`
- Hover interactions only appear when the component is hovered
- Action buttons (+ and dropdown) are positioned on the right side and stack vertically
- The dropdown closes automatically after selecting an item
- Star icons in dropdown are only clickable when `SetFavoriteFunc` is provided
- Colors automatically rotate through 8 MudBlazor theme colors if `ItemColorSource` is not provided
- Square size mode displays 32x32 pixel colored squares ideal for visual selection
