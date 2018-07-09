namespace LionFire.ObjectBus
{
    public abstract class ReferenceBase<ConcreteType> : ReferenceBase
    {
        public new ConcreteType GetChild(string subPath)
        {
            return (ConcreteType)base.GetChild(subPath);
        }
    }
}
