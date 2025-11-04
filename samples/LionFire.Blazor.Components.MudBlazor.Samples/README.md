# LionFire.Blazor.Components.MudBlazor.Samples

This project demonstrates the usage of components from the `LionFire.Blazor.Components.MudBlazor_` library.

## Current Status

✅ **The project builds successfully!** The Blazor Server application is fully functional and ready to run.

### Project Structure

```
LionFire.Blazor.Components.MudBlazor.Samples/
├── Program.cs                  # Application entry point
├── App.razor                   # Root app component
├── Routes.razor                # Routing configuration
├── Layout/
│   └── MainLayout.razor        # Main application layout
├── Pages/
│   ├── Home.razor              # Home page
│   └── InlineSelectorPage.razor # InlineSelector component examples
├── wwwroot/
│   └── app.css                 # Application styles
├── appsettings.json            # Application configuration
└── _Imports.razor              # Global imports

```

## Features

- ✅ **Fully functional Blazor Server application**
- ✅ **InlineSelector Component examples** with multiple usage patterns
- ✅ **MudBlazor 8.6.0 integration** working correctly
- ✅ **Interactive examples** demonstrating all component features

## Using the InlineSelector Component

You can use the `InlineSelector` component in your own Blazor applications:

### Installation

Add a project reference to `LionFire.Blazor.Components.MudBlazor`:

```xml
<ProjectReference Include="..\..\src\LionFire.Blazor.Components.MudBlazor\LionFire.Blazor.Components.MudBlazor.csproj" />
```

### Basic Usage

```razor
@using LionFire.Blazor.Components.MudBlazor_.Components

<InlineSelector TItem="string"
                Items="@items"
                SelectedItem="@selected"
                SelectedItemChanged="@OnSelectionChanged" />

@code {
    private List<string> items = new() { "Item 1", "Item 2", "Item 3" };
    private string selected = "Item 1";

    private async Task OnSelectionChanged(string item)
    {
        selected = item;
        await Task.CompletedTask;
    }
}
```

### Advanced Usage

For complete examples with favorites, dropdowns, and custom templates, see:
- `Pages/InlineSelectorPage.razor` in this samples application
- `/mnt/c/src/core/src/LionFire.Blazor.Components.MudBlazor/Components/InlineSelector.README.md`

## Running the Samples

```bash
cd /mnt/c/src/Core/samples/LionFire.Blazor.Components.MudBlazor.Samples
dotnet run
```

Then navigate to `https://localhost:5001` (or the port shown in the console).

## Component Examples

The samples application includes:

1. **Home Page** - Overview and navigation to sample pages
2. **InlineSelector Examples** - Three comprehensive examples:
   - Simple usage with string items
   - Advanced usage with favorites, dropdown, and creation
   - Custom styling example
