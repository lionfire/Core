namespace LionFire.Blazor.Components.MudBlazor_.Experimental.Models;

/// <summary>
/// Represents a column in a Kanban board
/// </summary>
public class KanbanColumn
{
    public Guid Id { get; set; }
    public Guid BoardId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Position { get; set; }
    public string Color { get; set; } = "#6200EA"; // Default purple
    public bool IsVisible { get; set; } = true;

    // Column type and greenlight system
    public ColumnType Type { get; set; }
    public GreenlightStatus GreenlightStatus { get; set; }
    public string? GreenlightReason { get; set; }

    // Work in progress limit
    public int? WipLimit { get; set; }

    // Navigation properties
    public KanbanBoard? Board { get; set; }
    public ICollection<KanbanCard> Cards { get; set; } = new List<KanbanCard>();

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum ColumnType
{
    Priority,
    Status,
    Custom
}

public enum GreenlightStatus
{
    NotApproved = 0,  // Red - Agents cannot work
    Conditional = 1,  // Yellow - Agents need permission per task
    Approved = 2      // Green - Agents can autonomously work
}
