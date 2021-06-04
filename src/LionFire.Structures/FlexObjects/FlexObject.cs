namespace LionFire.FlexObjects
{
    public class FlexObject : IFlex
    {
        public object Value { get; set; }

        public FlexObject() { }
        public FlexObject(object value)
        {
            Value = value;
        }

        public override string ToString() => Value == null ? "(null)" : Value.ToString();
    }
}
