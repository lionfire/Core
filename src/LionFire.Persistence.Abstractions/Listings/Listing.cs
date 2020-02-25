namespace LionFire.Referencing
{
    public class Listing
    {
        public Listing() { }
        public Listing(string name, string type = null, string mimeType = null, bool directory = false)
        {
            Name = name;
            Type = type;
            MimeType = mimeType;
            IsDirectory = directory;
        }

        public string Name { get; private set; }
        public string Type { get; }
        public string MimeType { get; }
        public bool IsDirectory { get; }


        public static implicit operator Listing(string name) => new Listing { Name = name };

        public override string ToString() => this.ToXamlAttribute();
    }

}
