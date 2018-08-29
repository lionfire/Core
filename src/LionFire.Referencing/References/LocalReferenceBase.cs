namespace LionFire.Referencing
{
    public abstract class LocalReferenceBase : IReference
    {
        public string Host => "";
        public string Port => "";

        public abstract string Scheme
        {
            get;
        }
        public abstract string Key { get; }
        public abstract string Path { get; }

    }
}
