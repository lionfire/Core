using System;

namespace LionFire.Referencing
{
    /// <summary>
    /// Uses string as a backing field, will resolve to System.Uri each time it is requested
    /// </summary>
    public sealed class SlimUriStringReference : ReferenceBaseBase<SlimUriStringReference>, IReference
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

        [SetOnce]
        public Uri Uri
        {
            get => new Uri(key);
            set => Key = value.ToString();
        }

        #endregion

        

        [SetOnce]
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

        [SetOnce]
        public override string Path {
            get => Uri.AbsolutePath;
            protected set => throw new Exception("Use set_Uri or set_Key instead");
        }
        
    }
}
