namespace LionFire.Referencing
{
    /// <summary>
    /// Just a Path
    /// </summary>
    public sealed class PathReference : IReference
    {
        #region Construction

        public PathReference() { }
        public PathReference(string path) { this.Path = path; }
        public static explicit operator PathReference(string path) => new PathReference(path);
        
        #endregion

        public string Scheme => null;

        public string Path { get; set; }

        public string Key => Path;

        public string Host => null;

        public string Port => null;

    }
}
