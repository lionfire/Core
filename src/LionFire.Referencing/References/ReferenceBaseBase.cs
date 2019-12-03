using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    // TODO: Get derived classes to override Clone if desired, and eliminate CopyFrom, and use CloneUtil.ShallowClone:

    // OPTIMIZE Clone
    // https://stackoverflow.com/a/52972518/208304
    //public static class CloneUtil<T>
    //{
    //    private static readonly Func<T, object> clone;

    //    static CloneUtil()
    //    {
    //        var cloneMethod = typeof(T).GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
    //        clone = (Func<T, object>)cloneMethod.CreateDelegate(typeof(Func<T, object>));
    //    }

    //    public static T ShallowClone(T obj) => (T)clone(obj);
    //}

    //public static class CloneUtil
    //{
    //    public static T ShallowClone<T>(this T obj) => CloneUtil<T>.ShallowClone(obj);
    //}

    public abstract class ReferenceBaseBase<ConcreteType> 
        : ICloneableReference
        , IReferencable<ConcreteType>
        where ConcreteType : ReferenceBaseBase<ConcreteType>, IReference
    {
        ConcreteType IReferencable<ConcreteType>.Reference => (ConcreteType)this;
        IReference ICloneableReference.Clone() => Clone();
        public virtual ConcreteType Clone()
        {
            var result = (ConcreteType)Activator.CreateInstance(this.GetType());
            result.CopyFrom((ConcreteType)this);
            return result;
        }

        IReference ICloneableReference.CloneWithPath(string newPath) => CloneWithPath(newPath);
        public virtual ConcreteType CloneWithPath(string newPath)
        {
            var result = Clone();
            result.InternalSetPath(newPath);
            return result;
        }

        public abstract string Key { get; protected set; }

        public abstract string Path { get; set; } // TODO BREAKINGCHANGE: Make set protected, use constructors/factories to set Path and other properties

        protected virtual void CopyFrom(ConcreteType other)
        {
            this.Key = other.Key;
        }
        protected virtual void InternalSetPath(string path)=> throw new NotSupportedException();

        #region Children

        //public virtual IReference GetChild(string subPath)
        //{
        //    // Use ctor instead? Or reference factory?

        //    var result = (ReferenceBaseBase)Activator.CreateInstance(this.GetType());
        //    result.CopyFrom(this, this.Path + String.Concat(ReferenceConstants.PathSeparator, subPath));
        //    return result;
        //}

        //public IReference GetChildSubpath(params string[] subpath)
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

        #endregion

    }
}
