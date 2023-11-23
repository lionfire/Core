namespace LionFire.Life.Model;

public class JournalEntry
{
    public long Id { get; set; }

    public DateOnly? BilledDate { get; set; }

    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }

    public string Description { get; set; }
    public string Notes { get; set; }

    public List<JournalTag> Tags { get; } = new();
}
