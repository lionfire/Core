namespace LionFire.Blazor.Components.MudBlazor_.Experimental.Models;

/// <summary>
/// Represents a Kanban board containing columns and cards
/// </summary>
public class KanbanBoard
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public ICollection<KanbanColumn> Columns { get; set; } = new List<KanbanColumn>();

    // Board settings
    public bool IsActive { get; set; } = true;
    public bool IsArchived { get; set; }
    public string? Theme { get; set; }
    public string? SettingsJson { get; set; }
}
