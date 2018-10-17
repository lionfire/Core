namespace LionFire.Referencing
{
    public abstract class ReferenceBase<ConcreteType> : ReferenceBase
        where ConcreteType : ReferenceBase
    {
        public new ConcreteType GetChild(string subPath)
        {
            return (ConcreteType)base.GetChild(subPath);
        }
    }
}
