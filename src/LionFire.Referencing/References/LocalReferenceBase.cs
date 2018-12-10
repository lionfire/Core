using System.Collections.Generic;
using System.Linq;

namespace LionFire.Referencing
{
    public abstract class LocalReferenceBase : ReferenceBaseBase, IReference
    {
        public bool IsCompatibleWith(string stringUrl) => AllowedSchemes.Contains(stringUrl.GetUriScheme());
        public abstract IEnumerable<string> AllowedSchemes { get; }

        public string Host => "";
        public string Port => "";

        public abstract string Scheme
        {
            get;
        }
        public abstract string Key { get; }

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

    }

    //public abstract class LocalReferenceBase<T> : LocalReferenceBase
    //{
    //    protected virtual void CopyFrom(IReference other, string newPath = null)
    //    {
    //        this.Path = newPath ?? other.Path;
    //    }
    //}
}