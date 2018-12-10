namespace LionFire.Referencing
{
    /// <summary>
    /// Just a Path
    /// </summary>
    public sealed class PathReference : ReferenceBaseBase, IReference
    {
        #region Construction

        public PathReference() { }
        public PathReference(string path) { this.Path = path; }
        public static explicit operator PathReference(string path) => new PathReference(path);
        
        #endregion

        public string Scheme => null;


        #region Path

        [SetOnce]
        public override string Path
        {
            get { return path; }
            set
            {
                if (path == value) return;
                if (path != default(string)) throw new AlreadySetException();
                path = value;
            }
        }
        private string path;

        #endregion

        public string Key => Path;

        public string Host => null;

        public string Port => null;

        public bool IsCompatibleWith(string obj) => string.IsNullOrWhiteSpace(obj.GetUriScheme());
    }
}
