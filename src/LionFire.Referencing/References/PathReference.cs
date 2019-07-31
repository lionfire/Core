namespace LionFire.Referencing
{
    /// <summary>
    /// Just a Path
    /// </summary>
    public sealed class PathReference : ReferenceBaseBase<PathReference>, IReference
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
            get => path;
            set
            {
                if (path == value) return;
                if (path != default) throw new AlreadySetException();
                path = value;
            }
        }
        private string path;

        #endregion

        public override string Key { get => Path; protected set => Path = value; }

        public string Host => null;

        public string Port => null;

        public bool IsCompatibleWith(string obj) => string.IsNullOrWhiteSpace(obj.GetUriScheme());
    }
}
