namespace LionFire.Tmux.Models;

/// <summary>
/// Represents a window within a tmux session
/// </summary>
public class TmuxWindow
{
    /// <summary>
    /// Name of the parent session
    /// </summary>
    public string SessionName { get; set; } = "";

    /// <summary>
    /// Window index (0-based)
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Window name
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Pane ID (format: %0, %1, etc.)
    /// </summary>
    public string PaneId { get; set; } = "";

    /// <summary>
    /// Whether this window is the active window in its session
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Command currently running in the window
    /// </summary>
    public string CurrentCommand { get; set; } = "";

    /// <summary>
    /// Number of panes in this window
    /// </summary>
    public int PaneCount { get; set; } = 1;

    /// <summary>
    /// Last activity timestamp
    /// </summary>
    public DateTime? LastActivity { get; set; }

    /// <summary>
    /// Full identifier (session:window)
    /// </summary>
    public string FullId => $"{SessionName}:{Index}";
}
