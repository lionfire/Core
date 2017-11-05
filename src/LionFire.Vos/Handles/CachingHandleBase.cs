using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Instantiating;
using LionFire.Collections;
using LionFire.Structures;
using System.Collections;

namespace LionFire.ObjectBus
{

    ///// <summary>
    ///// Caches children (using WeakReference)  for efficient repeated child traversal
    ///// TODONE: inherited by Vob, but I think this should not inherit HandleBase.
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <typeparam name="ChildType__"></typeparam>
    //public abstract class CachingHandleBase<T, ChildType__> 
    //    //:
    //    //HandleBase<T>, 
    //    //IHandle<T>, 
    //    //IFreezable, 
    //    //IChangeableReferencable
    //    where T : Vob
    //    where ChildType__ : CachingHandleBase<T, ChildType__>
    //    //, IHandle
    //{
    //    #region Construction

    //    //public CachingHandleBase(T obj = null, bool freezeObjectIfProvided = true)
    //    //    : base(obj, freezeObjectIfProvided)
    //    //{
    //    //    CacheChildren = true;
    //    //}

    //    //internal CachingHandleBase(string uri, T obj = null, bool freezeObjectIfProvided = true)
    //    //    : base(uri, obj, freezeObjectIfProvided)
    //    //{
    //    //    CacheChildren = true;
    //    //}

    //    //public CachingHandleBase(IReference reference, T obj = null, bool freezeObjectIfProvided = true)

    //    //    : base(reference, obj, freezeObjectIfProvided)
    //    //{
    //    //    CacheChildren = true;
    //    //}

    //    //public CachingHandleBase(IReferencable referencable, T obj = null, bool freezeObjectIfProvided = true)
    //    //    : base(referencable, obj, freezeObjectIfProvided)
    //    //{
    //    //    CacheChildren = true;
    //    //}

    //    #endregion

    //    #region Cached Children

    //    // TODO: Change to ConcurrentDictionary
    //    [Ignore]
    //    protected MultiBindableDictionary<string, WeakReferenceX<ChildType__>> children = new MultiBindableDictionary<string, WeakReferenceX<ChildType__>>();

    //    public readonly object SyncRoot = new object();

    //    public bool CacheChildren
    //    {
    //        get { return children != null; }
    //        set
    //        {
    //            if (value == CacheChildren) return;
    //            if (value)
    //            {
    //                children = new MultiBindableDictionary<string, WeakReferenceX<ChildType__>>();
    //            }
    //            else
    //            {
    //                children = null;
    //            }
    //        }
    //    }

    //    private void CleanDeadChildReferences()
    //    {
    //        IEnumerable<KeyValuePair<string, WeakReferenceX<ChildType__>>> ce = (IEnumerable<KeyValuePair<string, WeakReferenceX<ChildType__>>>)children;

    //        foreach (var kvp in ce.ToArray())
    //        {
    //            if (!kvp.Value.IsAlive)
    //            {
    //                children.Remove(kvp.Key);
    //            }
    //        }
    //    }

    //    #region Child accessor



    //    #region Get / Query Logic

    //    protected abstract ChildType__ CreateChild( string childName);

    //    private ChildType__ GetChild(IEnumerable<string> subpathChunks)
    //    {
    //        return GetChild(subpathChunks.GetEnumerator());
    //    }

    //    // DUPLICATED
    //    private ChildType__ GetChild(IEnumerator<string> subpathChunks)
    //    {
    //        if (subpathChunks == null) return this as ChildType__;
    //        ChildType__ child;

    //        if (!subpathChunks.MoveNext() || String.IsNullOrWhiteSpace(subpathChunks.Current))
    //        {
    //            return (ChildType__)this;
    //        }

    //        string childName = subpathChunks.Current;

    //        lock (SyncRoot)
    //        {
    //            var wVob = this.children.TryGetValue(childName);
    //            if (wVob != null)
    //            {
    //                if (!wVob.IsAlive)
    //                {
    //                    //vob = new ChildType(this.vos, this, childName);
    //                    child = CreateChild(childName);
    //                    wVob.Target = child;
    //                }
    //                else
    //                {
    //                    child = wVob.Target;
    //                }
    //            }
    //            else
    //            {
    //                child = CreateChild( childName);
    //                this.children.Add(childName, new WeakReferenceX<ChildType__>(child));
    //            }
    //        }

    //        return child.GetChild(subpathChunks);

    //        //if (!subpathChunks.MoveNext())
    //        //{
    //        //    return child;
    //        //}
    //        //else
    //        //{
    //        //    return child[subpathChunks];
    //        //}
    //    }

    //    // DUPLICATED
    //    internal Vob GetChild(string[] subpathChunks, int index)
    //    {
    //        Vob vob;
    //        //if (index == 0 && subpathChunks.Length == 0) return this;
    //        string childName = subpathChunks[index];

    //        lock (SyncRoot)
    //        {
    //            var wVob = this.children.TryGetValue(childName);
    //            if (wVob != null)
    //            {
    //                if (!wVob.IsAlive)
    //                {
    //                    vob = CreateChild( childName);
    //                    wVob.Target = vob;
    //                }
    //                else
    //                {
    //                    vob = wVob.Target;
    //                }
    //            }
    //            else
    //            {
    //                vob = CreateChild( childName);
    //                this.children.Add(childName, new WeakReferenceX<ChildType__>(vob));
    //            }
    //        }

    //        if (index == subpathChunks.Length - 1)
    //        {
    //            return vob;
    //        }
    //        else
    //        {
    //            return vob[index + 1, subpathChunks];
    //        }
    //    }

    //    public ChildType__ QueryChild(string[] subpathChunks, int index)
    //    {
    //        ChildType__ vob;

    //        string childName = subpathChunks[index];
    //        var wVob = this.children.TryGetValue(childName);
    //        if (wVob != null)
    //        {
    //            if (wVob.IsAlive)
    //            {
    //                vob = wVob.Target;
    //            }
    //            else
    //            {
    //                return null;
    //            }
    //        }
    //        else
    //        {
    //            return null;
    //        }

    //        if (index == subpathChunks.Length - 1)
    //        {
    //            return vob;
    //        }
    //        else
    //        {
    //            return vob.QueryChild(subpathChunks, index + 1);
    //        }
    //    }

    //    #endregion

    //    #region Children by VosReference

    //    public ChildType__ this[VosReference reference]
    //    {
    //        get
    //        {
    //            return this[reference.Path];
    //        }
    //    }

    //    public ChildType__ GetChild(VosReference reference)
    //    {
    //        return this.GetChild(reference.Path.ToPathArray(), 0);
    //    }

    //    public ChildType__ QueryChild(VosReference reference)
    //    {
    //        return this.QueryChild(reference.Path.ToPathArray(), 0);
    //    }

    //    #endregion

    //    #endregion

    //    #endregion

    //}

    public abstract class CachingChildren<NodeType> 
        where NodeType : CachingChildren<NodeType>, IHandle
    {
        #region Cached Children

        [Ignore]
        protected MultiBindableDictionary<string, LionFire.Structures.WeakReferenceX<NodeType>> children = new MultiBindableDictionary<string, LionFire.Structures.WeakReferenceX<NodeType>>();

        public bool CacheChildren
        {
            get { return children != null; }
            set
            {
                if (value == CacheChildren) return;
                if (value)
                {
                    children = new MultiBindableDictionary<string, WeakReferenceX<NodeType>>();
                }
                else
                {
                    children = null;
                }
            }
        }

        private void CleanDeadChildReferences()
        {
            IEnumerable<KeyValuePair<string, WeakReferenceX<NodeType>>> ce = (IEnumerable<KeyValuePair<string, WeakReferenceX<NodeType>>>)children;

            foreach (var kvp in ce.ToArray())
            {
                if (!kvp.Value.IsAlive)
                {
                    children.Remove(kvp.Key);
                }
            }
        }

        #region Child accessor

        #region Index accessors (GetChild)

        public NodeType this[string subpath]
        {
            get
            {
                return this[0, subpath.ToPathArray()];
            }
        }

        public NodeType this[IEnumerator<string> subpathChunks]
        {
            get
            {
                return GetChild(subpathChunks);
            }
        }

        public NodeType this[IEnumerable<string> subpathChunks]
        {
            get
            {
                return GetChild(subpathChunks);
            }
        }

        public NodeType this[int index, string[] subpathChunks]
        {
            get
            {
                return GetChild(subpathChunks, index);
            }
        }

        public NodeType this[params string[] subpathChunks]
        {
            get
            {
                return GetChild(subpathChunks);
            }
        }

        #endregion

        #region Get / Query Logic

        protected abstract NodeType CreateChild(CachingChildren<NodeType> parent, string childName);

        private NodeType GetChild(IEnumerable
#if !AOT
            <string>
#endif
 subpathChunks)
        {
            if (subpathChunks == null ) return this as NodeType;

            return GetChild(subpathChunks.GetEnumerator());
        }

        private NodeType GetChild(IEnumerator
#if !AOT
            <string>
#endif
            subpathChunks)
        {
            if (subpathChunks == null) return this as NodeType;

            NodeType child;

            if (!subpathChunks.MoveNext() || string.IsNullOrWhiteSpace(subpathChunks.Current
#if AOT
            as string
#endif
))
            {
                return (NodeType)this;
            }

            string childName = subpathChunks.Current
#if AOT
            as string
#endif
;

            var wVob = this.children.TryGetValue(childName);
            if (wVob != null)
            {
                if (!wVob.IsAlive)
                {
                    //vob = new ChildType(this.vos, this, childName);
                    child = CreateChild(this, childName);
                    wVob.Target = child;
                }
                else
                {
                    child = wVob.Target;
                }
            }
            else
            {
                child = CreateChild(this, childName);
                this.children.Add(childName, new WeakReferenceX<NodeType>(child));
            }

            return child.GetChild(subpathChunks);

            //if (!subpathChunks.MoveNext())
            //{
            //    return child;
            //}
            //else
            //{
            //    return child[subpathChunks];
            //}
        }

        internal NodeType GetChild(string[] subpathChunks, int index)
        {
            if (subpathChunks == null) return this as NodeType;
            if (index >= subpathChunks.Length) throw new ArgumentOutOfRangeException("index >= subpathChunks.Length");

            NodeType vob;

            string childName = subpathChunks[index];
            var wVob = this.children.TryGetValue(childName);
            if (wVob != null)
            {
                if (!wVob.IsAlive)
                {
                    vob = CreateChild(this, childName);
                    wVob.Target = vob;
                }
                else
                {
                    vob = wVob.Target;
                }
            }
            else
            {
                vob = CreateChild(this, childName);
                this.children.Add(childName, new WeakReferenceX<NodeType>(vob));
            }

            if (index == subpathChunks.Length - 1)
            {
                return vob;
            }
            else
            {
            	var ccVob = (CachingChildren<NodeType>) vob;
				
                return ccVob[index + 1, subpathChunks];
            }
        }

        public NodeType QueryChild(string[] subpathChunks, int index)
        {
            if (subpathChunks == null) return this as NodeType;
            if (index >= subpathChunks.Length) throw new ArgumentOutOfRangeException("index >= subpathChunks.Length");

            NodeType vob;

            string childName = subpathChunks[index];
            var wVob = this.children.TryGetValue(childName);
            if (wVob != null)
            {
                if (wVob.IsAlive)
                {
                    vob = wVob.Target;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            if (index == subpathChunks.Length - 1)
            {
                return vob;
            }
            else
            {
                return vob.QueryChild(subpathChunks, index + 1);
            }
        }

        #endregion

        #region Children by VosReference

        public NodeType this[VosReference reference]
        {
            get
            {
                if (reference == null) return this as NodeType;

                return this[reference.Path];
            }
        }

        public NodeType GetChild(VosReference reference)
        {
            return this.GetChild(reference.Path.ToPathArray(), 0);
        }

        public NodeType QueryChild(VosReference reference)
        {
            return this.QueryChild(reference.Path.ToPathArray(), 0);
        }

        #endregion

        #endregion

        #endregion
    }

}
