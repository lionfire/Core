using LionFire.Copying;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        

        IReference IReferencable.Reference => (ConcreteType)this;

        ConcreteType IReferencable<ConcreteType>.Reference => (ConcreteType)this;
        IReference ICloneableReference.Clone() => Clone();
        public virtual ConcreteType Clone()
        {
            var result = (ConcreteType)Activator.CreateInstance(this.GetType());
            result.CopyFrom((ConcreteType)this);
            return result;
        }

        IReference ICloneableReference.CloneWithPath(string newPath) => CloneWithPath(newPath);

        private static ConstructorInfo Ctor_ConcreteType_NewPath => ctor_ConcreteType_NewPath
            ??= typeof(ConcreteType).GetConstructors().Where(c =>
                {
                    var p = c.GetParameters();
                    if (p.Length != 2) return false;
                    if (p[0].ParameterType != typeof(ConcreteType)) return false;
                    if (p[1].ParameterType != typeof(string)) return false;
                    if (!p[1].Name.ToLowerInvariant().Contains("path")) return false;
                    return true;
                }).FirstOrDefault();
        private static ConstructorInfo ctor_ConcreteType_NewPath;

        /// <summary>
        /// Default implementation:
        ///  - Looks for a constructor with the parameter types (ConcreteType cloneSource, string newPath) (2nd parameter must have "path" in its name, case ignored), and invokes it if present, else
        ///  - invokes Clone() and InternalSetPath(newPath)
        /// </summary>
        /// <param name="newPath"></param>
        /// <returns></returns>
        public virtual ConcreteType CloneWithPath(string newPath)
        {
            if (Ctor_ConcreteType_NewPath != null) return (ConcreteType)Ctor_ConcreteType_NewPath.Invoke(new object[] { this, newPath });

            var result = Clone();
            result.InternalSetPath(newPath);
            return result;
        }

        [Assignment(AssignmentMode.Ignore)]
        public virtual string Persister
        {
            get => null; set { if (value != null) throw new NotSupportedException(); }
        }

        [Assignment(AssignmentMode.Ignore)]
        public abstract string Key { get; protected set; }

        [Assignment(AssignmentMode.Ignore)]
        public virtual string Url { get => Scheme + ":" + Key; protected set => throw new NotImplementedException(); }

        public abstract string Scheme { get; }

        [Assignment(AssignmentMode.Ignore)]
        public abstract string Path { get; protected set; } // TODO BREAKINGCHANGE: Make set protected, use constructors/factories to set Path and other properties

        protected virtual void CopyFrom(ConcreteType other)
        {
            this.Key = other.Key;
        }
        protected virtual void InternalSetPath(string path) => Path = path;

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


        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            //if (obj.GetType() != this.GetType()) return false; // REVIEW 

            if (obj is IKeyed<string> keyed && obj.GetType() == this.GetType())
            {
                return Key == keyed.Key;
            }
            if (obj is IReference reference)
            {
                return Url == reference.Url;
            }
            return false;
        }
    }
}
