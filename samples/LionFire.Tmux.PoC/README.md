# LionFire.Tmux - Proof of Concept

Real tmux integration demonstration using the LionFire.Tmux library.

## Overview

This is a minimal Blazor Server application that demonstrates **real tmux integration** using the `LionFire.Tmux` library. Unlike the Samples project (which uses mock data), this PoC connects to actual tmux sessions running on your system.

## Features

- ✅ Lists real tmux sessions from your system
- ✅ Shows windows within each session
- ✅ Displays actual terminal output with ANSI color support
- ✅ Send commands to tmux windows interactively
- ✅ Create new tmux sessions
- ✅ Kill sessions and windows
- ✅ VSCode-style tree navigation
- ✅ Real-time command execution

## Prerequisites

**Tmux must be installed:**
- **Ubuntu/Debian**: `sudo apt install tmux`
- **macOS**: `brew install tmux`
- **WSL**: `sudo apt install tmux`

**Verify installation:**
```bash
tmux -V
```

## Running the PoC

### Option 1: From this directory
```bash
cd /mnt/c/src/Core/samples/LionFire.Tmux.PoC
dotnet run
```

### Option 2: Using dotnet_win (on Windows filesystem)
```bash
cd /mnt/c/src/Core/samples/LionFire.Tmux.PoC
dotnet_win run
```

Then navigate to: **http://localhost:5000**

## Usage

### Viewing Existing Sessions

1. The app will automatically list all tmux sessions on startup
2. Sessions appear in the left sidebar as a tree
3. Click a session to expand and see its windows
4. Click a window to view its output

### Sending Commands

1. Select a window from the sidebar
2. Type a command in the input box at the bottom
3. Press Enter or click "Send"
4. The command is sent to the tmux window via `tmux send-keys`
5. The output refreshes after 500ms

**Example commands to try:**
- `echo "Hello from Blazor!"`
- `ls -la`
- `date`
- `whoami`

### Creating New Sessions

1. Click the `+` button in the sidebar header
2. Enter a session name (e.g., `test-session`)
3. Optionally specify a start command (default: `bash`)
4. Click "Create"
5. The new session appears in the tree

### Managing Sessions

- **Refresh**: Click the ⟳ button to refresh session list
- **Kill Session**: (Feature available in tree view)
- **Kill Window**: (Feature available in tree view)

## Architecture

```
LionFire.Tmux.PoC (Blazor Server App)
├── Uses: LionFire.Tmux library
├── Uses: TmuxTreeView component (from Samples)
├── Uses: TerminalViewer component (from Samples)
└── Service: TmuxService (REAL tmux integration)
    └── Calls: tmux list-sessions, list-windows, capture-pane, send-keys
```

## Configuration

The PoC is configured to use **real tmux** by default in `Program.cs`:

```csharp
// Real tmux service
builder.Services.AddTmuxService();
```

## Differences from Samples Project

| Feature | Samples Project | PoC Project |
|---------|----------------|-------------|
| **Service** | MockTmuxService (fake data) | TmuxService (real tmux) |
| **Sessions** | 3 hardcoded sessions | Your actual tmux sessions |
| **Commands** | Simulated responses | Real command execution |
| **Purpose** | Demo/documentation | Testing/proof-of-concept |

## Testing

### Create a test session:
```bash
# In a terminal
tmux new-session -d -s test-work bash
tmux send-keys -t test-work:0 "echo 'This is a test'" Enter
tmux send-keys -t test-work:0 "ls -la" Enter
```

### Then in the PoC app:
1. Refresh sessions (⟳ button)
2. Find "test-work" in the tree
3. Click window 0
4. See the output from your commands!
5. Try sending new commands from the input box

## Troubleshooting

### "Tmux Not Found"
- Make sure tmux is installed: `tmux -V`
- Check PATH includes tmux location
- On WSL, ensure tmux is installed in the Linux environment

### "No Sessions Found"
- Create a test session: `tmux new -d -s test`
- Verify sessions exist: `tmux ls`
- Click refresh in the app

### Commands not executing
- Check the session/window still exists: `tmux ls`
- Try manually: `tmux send-keys -t session:0 "echo test" Enter`
- Check console output for errors

## Files

- `Program.cs` - Registers real TmuxService, shows tmux status on startup
- `Components/Pages/Home.razor` - Main UI (tree + terminal + input)
- `Components/App.razor` - App root
- `Components/Routes.razor` - Routing configuration
- `LionFire.Tmux.PoC.csproj` - Project references

## Next Steps

- [ ] Add auto-refresh (polling every 2 seconds)
- [ ] Add split view (multiple terminals)
- [ ] Add pane support
- [ ] Add session attach/detach
- [ ] Add command history
- [ ] Add keyboard shortcuts

## Related Projects

- **LionFire.Tmux**: Core library (models + services)
- **LionFire.Blazor.Components.Samples**: Demo components with mock data
