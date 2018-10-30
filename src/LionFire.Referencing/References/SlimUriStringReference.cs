using System;

namespace LionFire.Referencing
{
    /// <summary>
    /// Uses string as a backing field, will resolve to System.Uri each time it is requested
    /// </summary>
    public sealed class SlimUriStringReference : IReference
    {
        public bool IsCompatibleWith(string obj) => Uri.TryCreate(obj, UriKind.RelativeOrAbsolute, out Uri _);

        #region Construction

        public SlimUriStringReference() { }
        public SlimUriStringReference(Uri uri)
        {
            Uri = uri;
        }
        public SlimUriStringReference(string uri)
        {
            Key = uri;
        }

        #region (Static) Construction Operators

        public static implicit operator SlimUriStringReference(string uri) => new SlimUriStringReference(uri);

        public static implicit operator SlimUriStringReference(Uri uri) => new SlimUriStringReference(uri);

        #endregion

        #endregion

        #region Uri

        public Uri Uri
        {
            get => new Uri(key);
            set => Key = value.ToString();
        }

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
