using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.Referencing;
using LionFire.Structures;

namespace LionFire.Vos
{
    public partial class Vob
    {
        #region Vob child fields

        // TODO: Change to a new MultiBindable ConcurrentDictionary?
        public IEnumerable<KeyValuePair<string, IVob>> Children
        {
            get
            {
                bool gotNonAlive = false;
                foreach(var kvp in children)
                {
                    if (!kvp.Value.IsAlive || kvp.Value.Target == null) { gotNonAlive = true; continue; }
                    yield return new KeyValuePair<string, IVob>(kvp.Key, kvp.Value.Target);
                }
                if (gotNonAlive) { CleanDeadChildReferences(); }
            }
        }
        [Ignore]
        protected MultiBindableDictionary<string, WeakReferenceX<IVob>> children = new MultiBindableDictionary<string, WeakReferenceX<IVob>>();
        public readonly object childrenLock = new object();

        #region (Private) Cleanup

        private void CleanDeadChildReferences()
        {
            var ce = (IEnumerable<KeyValuePair<string, WeakReferenceX<Vob>>>)children;

            foreach (var kvp in ce.ToArray())
            {
                if (!kvp.Value.IsAlive || kvp.Value.Target == null)
                {
                    children.Remove(kvp.Key);
                }
            }
        }

        #endregion

        #endregion

        #region Get / Query Logic

        private IVob GetChild(IEnumerable<string> subpathChunks) // TODO: Use span?
        {
            if (subpathChunks == null || !subpathChunks.Any())
            {
                return this;
            }

            return GetChild(subpathChunks.GetEnumerator());
        }

        protected IVob CreateChild(string childName) => new Vob(this, childName);

        // SIMILAR logic: GetChild and QueryChild
        public IVob GetChild(IEnumerator<string> subpathChunks)
        {
            if (subpathChunks == null)
            {
                return this;
            }

            IVob child;

            if (!subpathChunks.MoveNext() || string.IsNullOrWhiteSpace(subpathChunks.Current))
            {
                return this;
            }

            string childName = subpathChunks.Current;

            if (childName == "..")
            {
                child = Parent;
            }
            else if (childName == ".")
            {
                child = this;
            }
            else
            {
                lock (childrenLock)
                {
                    var weakRef = children.TryGetValue(childName);
                    if (weakRef != null)
                    {
                        var weakRefTarget = weakRef.Target;

                        if (weakRefTarget != null)
                        {
                            child = weakRefTarget;
                        }
                        else
                        {
                            child = CreateChild(childName);
                            // TOTEST - TODO FIXME - can the weak reference be reused with a new value? or do I need to create a new WeakReference?
                            weakRef.Target = child;
                        }
                    }
                    else
                    {
                        child = CreateChild(childName);
                        children.Add(childName, new WeakReferenceX<IVob>(child));
                    }
                }
            }

            return child.GetChild(subpathChunks);
        }

        /// <summary>
        /// Get the child with the specified name.  The name is the string within subpathChunks found at index 
        /// </summary>
        /// <param name="subpathChunks"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal IVob GetChild(string[] subpathChunks, int index)
        {
            // SIMILAR logic: GetChild and QueryChild

            IVob intermediateChild;

            if (subpathChunks == null || subpathChunks.Length == 0)
            {
                return this;
            }

            string childName = subpathChunks[index];

            if (childName == "..")
            {
                intermediateChild = Parent ?? this;
            }
            else if (childName == ".")
            {
                intermediateChild = this;
            }
            else
            {
                lock (childrenLock)
                {
                    var weakRef = children.TryGetValue(childName);
                    if (weakRef != null)
                    {
                        var weakRefTarget = weakRef.Target;

                        if (weakRefTarget != null)
                        {
                            intermediateChild = weakRefTarget;
                        }
                        else
                        {
                            intermediateChild = CreateChild(childName);
                            // TOTEST - TODO FIXME - can the weak reference be reused with a new value? or do I need to create a new WeakReference?
                            weakRef.Target = intermediateChild;
                        }
                    }
                    else
                    {
                        intermediateChild = CreateChild(childName);
                        children.Add(childName, new WeakReferenceX<IVob>(intermediateChild));
                    }
                }
            }

            if (index == subpathChunks.Length - 1)
            {
                return intermediateChild;
            }
            else
            {
                return intermediateChild[index + 1, subpathChunks];
            }
        }

        // DUPLICATED - Similar logic as GetChild
        public IVob QueryChild(string[] subpathChunks, int index)
        {
            if (subpathChunks == null || subpathChunks.Length == 0)
            {
                return this;
            }

            IVob intermediateChild;

            string childName = subpathChunks[index];

            if (childName == "..")
            {
                intermediateChild = Parent;
            }
            else if (childName == ".")
            {
                intermediateChild = this;
            }
            else
            {
                var weakRef = children.TryGetValue(childName);
                if (weakRef != null)
                {
                    var weakRefTarget = weakRef.Target;

                    if (weakRefTarget != null)
                    {
                        intermediateChild = weakRef.Target;
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
            }

            if (index == subpathChunks.Length - 1)
            {
                return intermediateChild;
            }
            else
            {
                return intermediateChild.QueryChild(subpathChunks, index + 1);
            }
        }

        #endregion

        #region (Derived) Index accessors (to GetChild)

        public IVob this[string subpath]
        {
            get
            {
                if (subpath == null)
                {
                    return this;
                }

                return this[0, subpath.ToPathArray()];
            }
        }

        public IVob this[IEnumerator<string> subpathChunks] => GetChild(subpathChunks);

        public IVob this[IEnumerable<string> subpathChunks] => GetChild(subpathChunks);

        public IVob this[int index, string[] subpathChunks] => GetChild(subpathChunks, index);

        public IVob this[params string[] subpathChunks] => GetChild(subpathChunks);

        #endregion

        #region (Derived) VosReference child getters

        public IVob this[VosReference reference] => this[reference.Path];

        public IVob GetChild(VosReference reference) => GetChild(reference.Path.ToPathArray(), 0);

        public IVob QueryChild(VosReference reference) => QueryChild(reference.Path.ToPathArray(), 0);

        #endregion
    }
}

#if OLD // DUPE

//#region Cached Children

//#region Child accessor

//#region Get / Query Logic

////protected abstract Vob CreateChild(string childName);

//private Vob GetChild(IEnumerable
//#if !AOT
//<string>
//#endif
// subpathChunks) => GetChild(subpathChunks.GetEnumerator());

//// DUPLICATED - Similar logic as GetChild and QueryChild
//private Vob GetChild(IEnumerator
//#if !AOT
//<string>
//#endif
// subpathChunks)
//{
//    if (subpathChunks == null)
//    {
//        return this;
//    }

//    Vob child;

//    if (!subpathChunks.MoveNext() || string.IsNullOrWhiteSpace(subpathChunks.Current
//#if AOT
//                as string
//#endif
//))
//    {
//        return this;
//    }

//    string childName = subpathChunks.Current
//#if AOT
// as string
//#endif
//;

//    lock (SyncRoot)
//    {
//        if (childName == "..")
//        {
//            child = Parent;
//        }
//        else if (childName == ".")
//        {
//            child = this;
//        }
//        else
//        {
//            var wVob = children.TryGetValue(childName);
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
//                child = CreateChild(childName);
//                children.Add(childName, new WeakReferenceX<Vob>(child));
//            }
//        }
//    }

//    return child.GetChild(subpathChunks);

//    //if (!subpathChunks.MoveNext())
//    //{
//    //    return child;
//    //}
//    //else
//    //{
//    //    return child[subpathChunks];
//    //}
//}

//// DUPLICATED - Similar logic as GetChild and QueryChild
//internal Vob GetChild(string[] subpathChunks, int index)
//{
//    Vob vob;

//    if (subpathChunks == null || subpathChunks.Length == 0)
//    {
//        return this;
//    }

//    string childName = subpathChunks[index];

//    lock (SyncRoot)
//    {
//        if (childName == "..")
//        {
//            vob = Parent ?? this;
//        }
//        else if (childName == ".")
//        {
//            vob = this;
//        }
//        else
//        {
//            var wVob = children.TryGetValue(childName);
//            if (wVob != null)
//            {
//                if (!wVob.IsAlive || wVob.Target == null)
//                {
//                    vob = CreateChild(childName);
//                    wVob.Target = vob;
//                }
//                else
//                {
//                    vob = wVob.Target;
//                }
//            }
//            else
//            {
//                vob = CreateChild(childName);
//                children.Add(childName, new WeakReferenceX<Vob>(vob));
//            }
//        }
//    }
//#if SanityChecks
//            if (vob == null) throw new UnreachableCodeException("vob == null");
//#endif
//    if (index == subpathChunks.Length - 1)
//    {
//        return vob;
//    }
//    else
//    {
//        return vob[index + 1, subpathChunks];
//    }
//}

//// DUPLICATED - Similar logic as GetChild
//public Vob QueryChild(string[] subpathChunks, int index)
//{
//    if (subpathChunks == null || subpathChunks.Length == 0)
//    {
//        return this;
//    }

//    Vob vob;

//    string childName = subpathChunks[index];

//    if (childName == "..")
//    {
//        vob = Parent;
//    }
//    else if (childName == ".")
//    {
//        vob = this;
//    }
//    else
//    {
//        var wVob = children.TryGetValue(childName);
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
//    }

//    if (index == subpathChunks.Length - 1)
//    {
//        return vob;
//    }
//    else
//    {
//        return vob.QueryChild(subpathChunks, index + 1);
//    }
//}

//#endregion

//#endregion

//#endregion
#endif