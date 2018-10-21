using System;

namespace LionFire.Referencing
{
    /// <summary>
    /// Uses string as a backing field, but will also lazily resolve to System.Uri on demand
    /// </summary>
    public sealed class UriStringReference : IReference
    {
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

        public string Key
        {
            get => key;
            set
            {
                if (key != null)
                {
                    throw new AlreadySetException();
                }

                key = value;
            }
        }
        private string key;

        public string Scheme => Uri.Scheme;
        public string Host => Uri.Host;
        public string Port => Uri.Port.ToString();
        public string Path => Uri.AbsolutePath;
    }
}
