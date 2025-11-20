namespace LionFire.Tmux.Models;

/// <summary>
/// Represents a tmux session
/// </summary>
public class TmuxSession
{
    /// <summary>
    /// Session name
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Number of windows in this session
    /// </summary>
    public int WindowCount { get; set; }

    /// <summary>
    /// List of windows in this session
    /// </summary>
    public List<TmuxWindow> Windows { get; set; } = new();

    /// <summary>
    /// Session creation time (formatted string from tmux)
    /// </summary>
    public string Created { get; set; } = "";

    /// <summary>
    /// Whether this session is currently attached
    /// </summary>
    public bool Attached { get; set; }

    /// <summary>
    /// Last activity timestamp (formatted string from tmux)
    /// </summary>
    public string Activity { get; set; } = "";

    /// <summary>
    /// Session creation timestamp (parsed)
    /// </summary>
    public DateTime? CreatedDateTime { get; set; }

    /// <summary>
    /// Get the active window (if any)
    /// </summary>
    public TmuxWindow? ActiveWindow => Windows.FirstOrDefault(w => w.IsActive);
}
