namespace LionFire.Metadata;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
public class ReferenceTypeAttribute : Attribute
{
    public ReferenceTypeAttribute(Type type)
    {
        Type = type;
    }

    public Type Type { get; }
}
