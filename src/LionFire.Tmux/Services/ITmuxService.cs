using LionFire.Tmux.Models;

namespace LionFire.Tmux.Services;

/// <summary>
/// Service for interacting with tmux sessions and windows
/// </summary>
public interface ITmuxService
{
    /// <summary>
    /// Get all tmux sessions with their windows
    /// </summary>
    Task<List<TmuxSession>> GetSessionsAsync();

    /// <summary>
    /// Get all windows for a specific session
    /// </summary>
    /// <param name="sessionName">Name of the session</param>
    Task<List<TmuxWindow>> GetWindowsAsync(string sessionName);

    /// <summary>
    /// Capture recent output from a window
    /// </summary>
    /// <param name="sessionName">Session name</param>
    /// <param name="windowIndex">Window index</param>
    /// <param name="lines">Number of lines to capture (default 100)</param>
    Task<List<string>> CaptureWindowOutputAsync(
        string sessionName,
        int windowIndex,
        int lines = 100);

    /// <summary>
    /// Create a new tmux session
    /// </summary>
    /// <param name="name">Session name</param>
    /// <param name="startCommand">Optional command to run (default: bash)</param>
    Task<bool> CreateSessionAsync(string name, string? startCommand = null);

    /// <summary>
    /// Kill a tmux session
    /// </summary>
    /// <param name="name">Session name</param>
    Task<bool> KillSessionAsync(string name);

    /// <summary>
    /// Kill a specific window
    /// </summary>
    /// <param name="sessionName">Session name</param>
    /// <param name="windowIndex">Window index</param>
    Task<bool> KillWindowAsync(string sessionName, int windowIndex);

    /// <summary>
    /// Check if tmux is available on the system
    /// </summary>
    Task<bool> IsTmuxAvailableAsync();

    /// <summary>
    /// Send keys (text input) to a specific window
    /// </summary>
    /// <param name="sessionName">Session name</param>
    /// <param name="windowIndex">Window index</param>
    /// <param name="keys">Text to send (will append Enter if sendEnter is true)</param>
    /// <param name="sendEnter">Whether to send Enter key after the text (default: true)</param>
    Task<bool> SendKeysAsync(string sessionName, int windowIndex, string keys, bool sendEnter = true);
}
