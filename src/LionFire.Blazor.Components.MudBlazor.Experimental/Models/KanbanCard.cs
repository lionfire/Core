namespace LionFire.Blazor.Components.MudBlazor_.Experimental.Models;

/// <summary>
/// Represents a card (task) in a Kanban column
/// </summary>
public class KanbanCard
{
    public Guid Id { get; set; }
    public Guid ColumnId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Position { get; set; }

    // Priority and state
    public Priority Priority { get; set; }
    public CardState State { get; set; }

    // Assignment and agent integration
    public string? AssignedTo { get; set; } // Can be human or agent ID
    public string? ClaimedBy { get; set; } // Agent that claimed the task
    public DateTime? ClaimedAt { get; set; }
    public DateTime? ClaimExpiresAt { get; set; }

    // External system integration
    public string? SourceSystem { get; set; } // GitHub, GitLab, Filesystem, etc.
    public string? ExternalId { get; set; }

    // Metadata
    public List<string> Tags { get; set; } = new List<string>();
    public DateTime? DueDate { get; set; }
    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }

    // Progress tracking
    public int PercentComplete { get; set; }
    public string? CurrentStep { get; set; }

    // Navigation properties
    public KanbanColumn? Column { get; set; }
    public ICollection<CardComment> Comments { get; set; } = new List<CardComment>();
    public ICollection<CardAttachment> Attachments { get; set; } = new List<CardAttachment>();

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public enum Priority
{
    Low,
    Medium,
    High,
    Critical
}

public enum CardState
{
    Available,    // Ready for work
    Claimed,      // Agent/human has claimed
    InProgress,   // Active work
    Blocked,      // Waiting on dependency
    Complete      // Finished
}

public class CardComment
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CardAttachment
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}
