namespace LionFire.Referencing
{
    public abstract class LocalReference<ConcreteType, TValue> : LocalReferenceBase<ConcreteType, TValue>
        where ConcreteType : ReferenceBaseBase<ConcreteType>, IReference<TValue>
    {
        public LocalReference(string path) { this.Path = path; }

        public override string Key { get => Path; protected set => Path = value; }

    }
}
