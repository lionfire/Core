using System.Diagnostics;
using System.Text;
using LionFire.Tmux.Models;

namespace LionFire.Tmux.Services;

/// <summary>
/// Real tmux service using actual tmux commands via Process
/// </summary>
public class TmuxService : ITmuxService
{
    private bool? _tmuxAvailable;

    public async Task<List<TmuxSession>> GetSessionsAsync()
    {
        var sessions = new List<TmuxSession>();

        if (!await IsTmuxAvailableAsync())
        {
            return sessions;
        }

        try
        {
            // Format: session_name|session_windows|session_created|session_attached|session_activity
            var format = "#{session_name}|#{session_windows}|#{session_created}|#{session_attached}|#{session_activity}";
            var output = await RunTmuxCommandAsync($"list-sessions -F \"{format}\"");

            if (string.IsNullOrEmpty(output))
            {
                return sessions;
            }

            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length >= 5)
                {
                    var session = new TmuxSession
                    {
                        Name = parts[0],
                        WindowCount = int.TryParse(parts[1], out int count) ? count : 0,
                        Created = parts[2],
                        Attached = parts[3] == "1",
                        Activity = parts[4],
                        CreatedDateTime = ParseUnixTimestamp(parts[2])
                    };

                    // Load windows for this session
                    session.Windows = await GetWindowsAsync(session.Name);

                    sessions.Add(session);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting tmux sessions: {ex.Message}");
        }

        return sessions;
    }

    public async Task<List<TmuxWindow>> GetWindowsAsync(string sessionName)
    {
        var windows = new List<TmuxWindow>();

        if (!await IsTmuxAvailableAsync())
        {
            return windows;
        }

        try
        {
            // Format: window_index|window_name|window_active|pane_current_command|window_panes
            var format = "#{window_index}|#{window_name}|#{window_active}|#{pane_current_command}|#{window_panes}";
            var output = await RunTmuxCommandAsync($"list-windows -t {sessionName} -F \"{format}\"");

            if (string.IsNullOrEmpty(output))
            {
                return windows;
            }

            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length >= 5)
                {
                    windows.Add(new TmuxWindow
                    {
                        SessionName = sessionName,
                        Index = int.TryParse(parts[0], out int idx) ? idx : 0,
                        Name = parts[1],
                        IsActive = parts[2] == "1",
                        CurrentCommand = parts[3],
                        PaneCount = int.TryParse(parts[4], out int panes) ? panes : 1,
                        PaneId = $"%{parts[0]}",
                        LastActivity = DateTime.Now
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting windows for session {sessionName}: {ex.Message}");
        }

        return windows;
    }

    public async Task<List<string>> CaptureWindowOutputAsync(string sessionName, int windowIndex, int lines = 100)
    {
        var output = new List<string>();

        if (!await IsTmuxAvailableAsync())
        {
            output.Add($"[Tmux not available]");
            return output;
        }

        try
        {
            var target = $"{sessionName}:{windowIndex}";
            var result = await RunTmuxCommandAsync($"capture-pane -p -t {target} -S -{lines}");

            if (!string.IsNullOrEmpty(result))
            {
                output.AddRange(result.Split('\n'));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error capturing output from {sessionName}:{windowIndex}: {ex.Message}");
            output.Add($"[Error capturing output: {ex.Message}]");
        }

        return output;
    }

    public async Task<bool> CreateSessionAsync(string name, string? startCommand = null)
    {
        if (!await IsTmuxAvailableAsync())
        {
            return false;
        }

        try
        {
            var command = string.IsNullOrWhiteSpace(startCommand) ? "bash" : startCommand;
            // -d = detached (don't attach immediately)
            var args = $"new-session -d -s {name} {command}";

            var exitCode = await RunTmuxCommandWithExitCodeAsync(args);
            return exitCode == 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating session {name}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> KillSessionAsync(string name)
    {
        if (!await IsTmuxAvailableAsync())
        {
            return false;
        }

        try
        {
            var exitCode = await RunTmuxCommandWithExitCodeAsync($"kill-session -t {name}");
            return exitCode == 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error killing session {name}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> KillWindowAsync(string sessionName, int windowIndex)
    {
        if (!await IsTmuxAvailableAsync())
        {
            return false;
        }

        try
        {
            var target = $"{sessionName}:{windowIndex}";
            var exitCode = await RunTmuxCommandWithExitCodeAsync($"kill-window -t {target}");
            return exitCode == 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error killing window {sessionName}:{windowIndex}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> IsTmuxAvailableAsync()
    {
        // Cache the result to avoid repeated checks
        if (_tmuxAvailable.HasValue)
        {
            return _tmuxAvailable.Value;
        }

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "tmux",
                    Arguments = "-V",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            _tmuxAvailable = process.ExitCode == 0;
        }
        catch
        {
            _tmuxAvailable = false;
        }

        return _tmuxAvailable.Value;
    }

    public async Task<bool> SendKeysAsync(string sessionName, int windowIndex, string keys, bool sendEnter = true)
    {
        if (!await IsTmuxAvailableAsync())
        {
            return false;
        }

        try
        {
            var target = $"{sessionName}:{windowIndex}";

            // Escape special characters and quotes
            var escapedKeys = keys.Replace("\"", "\\\"").Replace("'", "\\'");

            // Build the send-keys command
            var keysToSend = sendEnter ? $"{escapedKeys} Enter" : escapedKeys;
            var args = $"send-keys -t {target} \"{keysToSend}\"";

            var exitCode = await RunTmuxCommandWithExitCodeAsync(args);
            return exitCode == 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending keys to {sessionName}:{windowIndex}: {ex.Message}");
            return false;
        }
    }

    private async Task<string> RunTmuxCommandAsync(string arguments)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "tmux",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var output = new StringBuilder();
            var error = new StringBuilder();

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await Task.WhenAll(outputTask, errorTask);
            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                return outputTask.Result;
            }
            else
            {
                var errorMsg = errorTask.Result;
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    Console.WriteLine($"Tmux command error: {errorMsg}");
                }
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error running tmux command '{arguments}': {ex.Message}");
            return string.Empty;
        }
    }

    private async Task<int> RunTmuxCommandWithExitCodeAsync(string arguments)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "tmux",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            return process.ExitCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error running tmux command '{arguments}': {ex.Message}");
            return -1;
        }
    }

    private DateTime? ParseUnixTimestamp(string timestamp)
    {
        if (long.TryParse(timestamp, out long unixTime))
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
        }
        return null;
    }
}
