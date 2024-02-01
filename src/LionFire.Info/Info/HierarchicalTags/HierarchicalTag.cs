namespace LionFire.Info.HierarchicalTags;

[Flags]
public enum TagFlags
{
    Unspecified = 0,

    TopLevel = 1 << 1,

    ProperNoun = 1 << 20,
}

public record HierarchicalTag(string Name, string? Parent = null)
{
    public TagFlags TagFlags { get; init; } = TagFlags.Unspecified;

}