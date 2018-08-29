using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    public class UriReference : IReference
    {
        #region Construction

        public UriReference() { }
        public UriReference(Uri uri)
        {
            this.Uri = uri;
        }
        public UriReference(string uri)
        {
            this.Uri = new Uri(uri);
        }

        #region (Static) Construction Operators

        public static implicit operator UriReference(string uri)
        {
            return new UriReference(uri);
        }

        public static implicit operator UriReference(Uri uri)
        {
            return new UriReference(uri);
        }

        #endregion

        #endregion

        #region Uri

        public Uri Uri
        {
            get { return uri; }
            set
            {
                if (uri == value) return;
                if (uri != default(Uri)) throw new AlreadySetException();
                uri = value;
            }
        }

        private Uri uri;

        #endregion

        public string Key => Uri.ToString();

        public string Scheme => Uri.Scheme;
        public string Host => Uri.Host;
        public string Port => Uri.Port.ToString();
        public string Path => Uri.AbsolutePath;
    }
}
