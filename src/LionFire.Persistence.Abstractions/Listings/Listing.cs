namespace LionFire.Referencing
{
    public class Listing
    {
        #region Construction

        public Listing() { }
        public Listing(string name, string type = null, string mimeType = null, bool directory = false)
        {
            Name = name;
            Type = type;
            MimeType = mimeType;
            IsDirectory = directory;
        }
        public static implicit operator Listing(string name) => new Listing { Name = name };

        #endregion

        public string Name { get; private set; }
        public string Type { get; }
        public string MimeType { get; }
        public bool IsDirectory { get; }

        public IReference UnderlyingReference { get; set; }
        public string RawName { get; set; }


        public override string ToString() => this.ToXamlAttribute();
    }
}
