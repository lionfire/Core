using System.Collections.Generic;
using System.Linq;

namespace LionFire.Referencing
{
    public abstract class LocalReferenceBase : IReference
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
        public abstract string Path { get; protected set; }

    }

    //public abstract class LocalReferenceBase<T> : LocalReferenceBase
    //{
    //    protected virtual void CopyFrom(IReference other, string newPath = null)
    //    {
    //        this.Path = newPath ?? other.Path;
    //    }
    //}
}