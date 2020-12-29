using System;

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

        public override string Scheme => "path";
        public const string SchemePrefix = "path:";


        #region Path

        [SetOnce]
        public override string Path
        {
            get => path;
            protected set
            {
                if (path == value) return;
                if (path != default) throw new AlreadySetException();
                path = value;
            }
        }
        private string path;
        protected override void InternalSetPath(string path) => this.path = path;

        #endregion

        public override string Key { get => Path; protected set => Path = value; }
        public override string Url { get => SchemePrefix + Key; protected set => throw new NotImplementedException(); }

        public string Host => null;

        public string Port => null;

        public bool IsCompatibleWith(string obj) => string.IsNullOrWhiteSpace(obj.GetUriScheme());
    }
}
