namespace LionFire.Persistence.Filesystem
{
    public class ProviderFileReference : FileReference
    {
        #region ProviderName

        // TODO: Freeze IReference properties, instead of SetOnce on a property by property basis
        [SetOnce]
        public override string Persister
        {
            get => provider;
            set
            {
                if (provider == value) return;
                if (provider != default) throw new AlreadySetException();
                provider = value;
            }
        }
        private string provider;

        #endregion

        public ProviderFileReference() { }
        public ProviderFileReference(string provider, string path) : base(path) { this.Persister = provider; }
    }

}
