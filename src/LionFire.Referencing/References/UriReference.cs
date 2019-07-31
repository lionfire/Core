using System;

namespace LionFire.Referencing
{

    public sealed class UriReference : ReferenceBaseBase<UriReference>, IReference
    {
        public bool IsCompatibleWith(string obj) => Uri.TryCreate(obj, UriKind.RelativeOrAbsolute, out Uri _);

        #region Construction

        public UriReference() { }
        public UriReference(Uri uri)
        {
            Uri = uri;
        }
        public UriReference(string uri)
        {
            Uri = new Uri(uri);
        }

        #region (Static) Construction Operators

        public static implicit operator UriReference(string uri) => new UriReference(uri);

        public static implicit operator UriReference(Uri uri) => new UriReference(uri);

        #endregion

        #endregion

        #region Uri

        public Uri Uri
        {
            get => uri;
            set
            {
                if (uri == value)
                {
                    return;
                }

                if (uri != default)
                {
                    throw new AlreadySetException();
                }

                uri = value;
            }
        }

        private Uri uri;

        #endregion

        public override string Key { get => Uri.ToString(); protected set => Uri = new Uri(value); }

        public string Scheme => Uri.Scheme;
        public string Host => Uri.Host;
        public string Port => Uri.Port.ToString();
        public override string Path { get => Uri.AbsolutePath; set => throw new Exception("Use set_Uri"); }

        
    }
}
