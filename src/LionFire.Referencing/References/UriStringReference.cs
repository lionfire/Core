using System;

namespace LionFire.Referencing
{
    /// <summary>
    /// Uses string as a backing field, but will also lazily resolve to System.Uri on demand
    /// </summary>
    public sealed class UriStringReference : ReferenceBaseBase<UriStringReference>, IReference
    {
        public bool IsCompatibleWith(string obj) => Uri.TryCreate(obj, UriKind.RelativeOrAbsolute, out Uri _);

        #region Construction

        public UriStringReference() { }
        public UriStringReference(Uri uri)
        {
            Uri = uri;
        }
        public UriStringReference(string uri)
        {
            Key = uri;
        }

        #region (Static) Construction Operators

        public static implicit operator UriStringReference(string uri) => new UriStringReference(uri);

        public static implicit operator UriStringReference(Uri uri) => new UriStringReference(uri);

        #endregion

        #endregion

        #region Uri

        public Uri Uri
        {
            get
            {
                if (uri == null && key != null)
                {
                    uri = new Uri(key);
                }
                return uri;
            }
            set
            {
                if (uri == value)
                {
                    return;
                }

                if (uri != default(Uri))
                {
                    throw new AlreadySetException();
                }

                uri = value;
                Key = uri.ToString();
            }
        }
        private Uri uri;

        #endregion

        public override string Key
        {
            get => key;
            protected set
            {
                if (key != null)
                {
                    throw new AlreadySetException();
                }

                key = value;
            }
        }
        private string key;

        public override string Scheme => Uri.Scheme;
        public string Host => Uri.Host;
        public string Port => Uri.Port.ToString();
        public override string Path { get => Uri.AbsolutePath; protected set => throw new Exception("Use set_Uri"); }
        protected override void InternalSetPath(string path) => throw new Exception("Use set_Uri");
    }
}
