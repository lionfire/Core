namespace LionFire.Life.Model;

public class JournalTag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public string? Project { get; set; }
    public string? AccountingCodeId { get; set; }

    public List<AccountingCode> AccountingCodes { get; } = new();
}
