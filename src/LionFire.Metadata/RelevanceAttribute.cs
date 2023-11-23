namespace LionFire.Metadata;

[System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class RelevanceAttribute : Attribute
{
    public RelevanceFlags Relevance { get; }

    public RelevanceAspect Aspect { get; set; }
    public RelevanceAttribute(RelevanceFlags relevance, RelevanceAspect direction = RelevanceAspect.ReadWrite)
    {
        Relevance = relevance;
        Aspect = direction;
    }
}
