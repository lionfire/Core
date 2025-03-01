namespace LionFire.Ontology;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
public sealed class PluralAttribute : Attribute
{
    public PluralAttribute(string pluralName)
    {
        this.pluralName = pluralName;
    }

    public string PluralName
    {
        get { return pluralName; }
    }
    private readonly string pluralName;
}
