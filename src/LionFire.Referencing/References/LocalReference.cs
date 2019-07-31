namespace LionFire.Referencing
{
    public abstract class LocalReference<ConcreteType> : LocalReferenceBase<ConcreteType>
        where ConcreteType : ReferenceBaseBase<ConcreteType>, IReference
    {
        public LocalReference(string path) { this.Path = path; }

        public override string Key { get => Path; protected set => Path = value; }

    }
}
