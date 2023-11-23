namespace LionFire.FlexObjects;

public class FlexObject : IFlex
{
    public object FlexData { get; set; }

    public FlexObject() { }
    public FlexObject(object value)
    {
        FlexData = value;
    }

    public override string ToString() => FlexData == null ? "(null)" : FlexData.ToString();
}
