using System.Collections.Generic;
using System.Linq;
using System;

namespace LionFire.Referencing
{
    
    public abstract class LocalReferenceBase<ConcreteType, TValue> : ReferenceBaseBase<ConcreteType>, IReference<TValue>
        where ConcreteType : ReferenceBaseBase<ConcreteType>, IReference<TValue>
    {
        public bool IsCompatibleWith(string stringUrl) => AllowedSchemes.Contains(stringUrl.GetUriScheme());
        public abstract IEnumerable<string> AllowedSchemes { get; }

        public string Host => "";
        public string Port => "";

        //public abstract string Scheme
        //{
        //    get;
        //}

        protected LocalReferenceBase() { }
        protected LocalReferenceBase(string path) { InternalSetPath(path); }

        #region Path

        [SetOnce]
        public override string Path
        {
            get => path;
            protected set
            {
                if (path != default) throw new AlreadySetException();
                InternalSetPath(value);
            }
        }
        protected string path;

        protected override void InternalSetPath(string path) => this.path = path;

        #endregion

        //public virtual IReference GetChild(string subpath)
        //{
        //    ReferenceUtils.GetChild(this, subpath, ref path);

        //    // Use ctor instead? Or reference factory?

        //    var result = (ReferenceBaseBase)Activator.CreateInstance(this.GetType());
        //    result.CopyFromWithPath(this, this.Path + String.Concat(ReferenceConstants.PathSeparator, subPath));
        //    return result;
        //}

        ////public IReference GetChildSubpath(params string[] subpath)
        //public IReference GetChildSubpath(IEnumerable<string> subpath)
        //{
        //    var sb = new StringBuilder();
        //    bool isFirst = true;
        //    foreach (var subpathChunk in subpath)
        //    {
        //        if (isFirst)
        //        {
        //            isFirst = false;
        //        }
        //        else { sb.Append("/"); }
        //        sb.Append(subpathChunk);
        //    }
        //    return GetChild(sb.ToString());
        //}

    }

    //public static class ReferenceUtils
    //{
    //    public static void CombinePath(this IReference reference, string subpath, ref string path)
    //    {
    //        path = LionPath.Combine(path, subpath);
    //    }

    //    public static IReference GetChild(this IReference reference, string subpath)
    //    {
    //        // Use ctor instead? Or reference factory?

    //        var result = (ReferenceBaseBase<LocalReferenceBase>)Activator.CreateInstance(reference.GetType());
    //        result.CopyFrom(reference, reference.Path + String.Concat(ReferenceConstants.PathSeparator, subpath));
    //        return (IReference)result;
    //    }

    //}

    //public abstract class LocalReferenceBase<T> : LocalReferenceBase
    //{
    //    protected virtual void CopyFrom(IReference other, string newPath = null)
    //    {
    //        this.Path = newPath ?? other.Path;
    //    }
    //}
}