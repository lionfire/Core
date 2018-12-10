namespace LionFire.Referencing
{
    public abstract class LocalReference : LocalReferenceBase
    {
        public LocalReference(string path) { this.Path = path; }

        public override string Key => Path;

        

    }
}
