# LionFire.Blazor.Components.MudBlazor.Samples (Blazor United)

This is a **Blazor United** sample application that demonstrates the InlineSelector component and other components from the LionFire.Blazor.Components.MudBlazor library.

## What is Blazor United?

Blazor United supports **multiple render modes** in a single application:
- **Interactive Server** - Components run on the server with SignalR
- **Interactive WebAssembly** - Components run in the browser via WebAssembly
- **Interactive Auto** - Automatically chooses between Server and WebAssembly

This provides the best of both worlds: fast initial load with Server rendering, then transitioning to WebAssembly for offline support.

## Project Structure

- **LionFire.Blazor.Components.MudBlazor.Samples** - Main hosting project (this project)
- **LionFire.Blazor.Components.MudBlazor.Samples.Client** - Client-side WebAssembly components

## Features

- Dark mode support (toggle in top right corner)
- InlineSelector component with:
  - Multiple size options (Large, Medium, Small, Compact, Abbreviated, SingleLetter, Square)
  - Favorites filtering
  - Dropdown view with all items
  - Item creation
  - Custom colors

## Running the Application

```bash
cd /mnt/c/src/Core/samples/LionFire.Blazor.Components.MudBlazor.Samples
dotnet run
```

Or in Windows:
```powershell
cd C:\src\Core\samples\LionFire.Blazor.Components.MudBlazor.Samples
dotnet run
```

Then navigate to `https://localhost:5001` (or the port shown in the console).

## Render Mode

The pages in this application use **InteractiveWebAssembly** render mode, which means they run entirely in the browser after the initial download.

## Development

The interactive components are in the `.Client` project, while the hosting infrastructure is in this main project. Both Server and WebAssembly render modes are supported.
