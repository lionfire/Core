using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Referencing
{

    public abstract class ReferenceBase : IReference
        //, IReferenceEx2
    {
        public bool IsCompatibleWith(string stringUrl) => AllowedSchemes.Contains(stringUrl.GetUriScheme());
        public abstract IEnumerable<string> AllowedSchemes { get; }

        #region Scheme

        public abstract string Scheme
        {
            get;
        }

        public virtual bool CanSetScheme { get { return false; } }

        public static bool VerifyScheme = true;

        public virtual void ValidateScheme(string scheme)
        {
            if (scheme != Scheme)
            {
                throw new ArgumentException("Scheme '"
                    + (scheme ?? "null")
                    + "'not valid for this reference type: " + this.GetType().Name);
            }
        }

        #endregion

        public abstract string Key { get; }

        public abstract string Host { get; set; }
        public abstract string Port { get; set; }
        public abstract string Path { get; set; }

        #region Construction

        #region Copy From

        protected virtual void CopyFrom(IReference other, string newPath = null)
        {
            this.Host = other.Host;
            this.Port = other.Port;
            this.Path = newPath ?? other.Path;
        }

        #endregion

        #endregion

        #region Children

        public virtual IReference GetChild(string subPath)
        {
            // Use ctor instead? Or reference factory?

            var result = (ReferenceBase)Activator.CreateInstance(this.GetType());
            result.CopyFrom(this, this.Path + String.Concat(ReferenceConstants.PathSeparator, subPath));
            return result;
        }

        //public IReference GetChildSubpath(params string[] subpath)
        public IReference GetChildSubpath(IEnumerable<string> subpath)
        {
            var sb = new StringBuilder();
            bool isFirst = true;
            foreach (var subpathChunk in subpath)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else { sb.Append("/"); }
                sb.Append(subpathChunk);
            }
            return GetChild(sb.ToString());
        }

        #endregion
        
        #region Misc

        #region Object Overrides

        public override bool Equals(object obj)
        {
            var other = obj as IReference;
            if (other == null)
            {
                return false;
            }

            return this.Key == other.Key;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        #endregion

        #endregion

    }
}
