namespace LionFire.Inspection;

public class PropertyAttribute : Attribute
{
    public PropertyAttribute() { IsProperty = true; }
    public PropertyAttribute(bool isProperty)
    {
        IsProperty = isProperty;
    }

    public bool IsProperty { get; }
}
