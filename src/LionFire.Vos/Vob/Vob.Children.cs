//#define FUTURE_WeakChildReferences 
// FUTURE:  a way to query children as weak references.  For now, use strong references
// How to do this?  Idea:
//  - WeakReference by default
//  - VobValueChanged fires if data changes, specifically IFlex.Value.  (VobNode, and Non-weak children will hold reference to Parent and block removal)
//    - If IFlex.IsEmpty() becomes false, then promote from weak to strong child reference.
//    - If IFlex.IsEmpty() becomes true, then demote from strong to weak child reference.
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.Instantiating;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Structures;
using LionFire.Vos.Environment;
using LionFire.Vos.Internals;

namespace LionFire.Vos
{
    public partial class Vob : IEnumerable<IVob>
    {
        #region Vob child fields

        // TODO: Change to a new MultiBindable ConcurrentDictionary?
        public IEnumerable<KeyValuePair<string, IVob>> Children
        {
            get
            {
                foreach (var kvp in children)
                {
                    yield return new KeyValuePair<string, IVob>(kvp.Key, kvp.Value);
                }
#if FUTURE_WeakChildReferences
                bool gotNonAlive = false;
                foreach (var kvp in weakChildren)
                {
                    if (!kvp.Value.IsAlive || kvp.Value.Target == null) { gotNonAlive = true; continue; }
                    yield return new KeyValuePair<string, IVob>(kvp.Key, kvp.Value.Target);
                }
                if (gotNonAlive) { CleanChildrenDeadWeakReferences(); }
#endif
            }
        }
#if FUTURE_WeakChildReferences
        //[Ignore]
        //protected MultiBindableDictionary<string, WeakReferenceX<IVob>> weakChildren = new MultiBindableDictionary<string, WeakReferenceX<IVob>>();
        //public readonly object childrenLock = new object();
#endif
        protected ConcurrentDictionary<string, IVob> children = new ConcurrentDictionary<string, IVob>();
        IReadOnlyDictionary<string, IVob> IHierarchyOfKeyed<IVob>.Children => children;

        #region (Internal) Cleanup

#if FUTURE_WeakChildReferences
        private void CleanChildrenDeadWeakReferences()
        {
            var ce = (IEnumerable<KeyValuePair<string, WeakReferenceX<Vob>>>)weakChildren;

            foreach (var kvp in ce.ToArray())
            {
                if (!kvp.Value.IsAlive || kvp.Value.Target == null)
                {
                    weakChildren.Remove(kvp.Key);
                }
            }
        }
#endif

        public void CleanEmptyChildren(bool recurse = false)
        {
            foreach (var child in Children)
            {
                if (child.Value.IsEmpty()) { children.TryRemove(child.Key, out _); }
                else if (recurse) { ((IVobInternals)child.Value).CleanEmptyChildren(true); }
            }
        }

        #endregion

        #endregion

        #region IEnumerable

        public IEnumerator<IVob> GetEnumerator() => children.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Get / Query Logic

        // TODO: enable non-nullable for GetChild and CreateChild
        private IVob GetChild(IEnumerable<string> subpathChunks) // TODO: Use span?
        {
            if (subpathChunks == null || !subpathChunks.Any())
            {
                return this;
            }

            return GetChild(subpathChunks.GetEnumerator());
        }

        protected IVob CreateChild(string childName) => new Vob(this, childName);

        #region Parsing

        // TODO: Use these in GetChild and QueryChild.  
        // MOVE to another file

        public static string ParseNameFromEnvironmentVariable(string environmentReference) => environmentReference.Substring(1);
        public static bool IsEnvironmentVariableReference(string environmentReference) => environmentReference.StartsWith("$");

        public (string StringChunk, VobReference ReferenceChunk) TryResolvePathChunk(string chunk)
        {
            if (IsEnvironmentVariableReference(chunk))
            {
                var envValue = this.Environment()[ParseNameFromEnvironmentVariable(chunk)];
                var envValueString = envValue as string;
                var envValueReference = envValue as VobReference;
                if (!string.IsNullOrEmpty(envValueString))
                {
                    return (envValueString, null);
                }
                else if (envValueReference != null)
                {
                    return (null, envValueReference);
                }
                else
                {
                    if (ThrowOnMissingEnvironmentVariables) throw new Exception($"[Vob child traverse] environment variable not found: {chunk}");
                    else
                    {
                        Debug.WriteLine($"[Vob child traverse] environment variable not found: {chunk}");
                        return (string.Empty, null);
                    }
                }
            }
            return (null, null);
        }
        #endregion

        public static bool ThrowOnMissingEnvironmentVariables = false;

        // SIMILAR logic: GetChild and QueryChild
        public IVob GetChild(IEnumerator<string> subpathChunks)
        {
            if (subpathChunks == null) { return this; }

        start:
            if (!subpathChunks.MoveNext() || string.IsNullOrWhiteSpace(subpathChunks.Current)) { return this; }

            string chunk = subpathChunks.Current;

            var x = TryProcessEnvironmentChunk(chunk);
            if (x.Vob != null) return x.Vob.GetChild(subpathChunks);
            else if (x.goToNext) goto start;

            //if (IsEnvironmentVariableReference(childName))
            //{
            //    var envValue = this.Environment()[ParseNameFromEnvironmentVariable(childName)] as string;
            //    if (!string.IsNullOrEmpty(envValue))
            //    {
            //        // TOTEST
            //        return GetChild(envValue.ToPathElements()).GetChild(subpathChunks);
            //    }
            //    else
            //    {
            //        goto start;
            //    }
            //}

            IVob child;
            if (chunk == "..")
            {
                child = Parent;
            }
            else if (chunk == ".")
            {
                child = this;
            }
            else
            {

                child = children.GetOrAdd(chunk, key => CreateChild(key));
#if FUTURE_WeakChildReferences
                lock (childrenLock)
                {
                    var weakRef = weakChildren.TryGetValue(chunk);
                    if (weakRef != null)
                    {
                        var weakRefTarget = weakRef.Target;

                        if (weakRefTarget != null)
                        {
                            child = weakRefTarget;
                        }
                        else
                        {
                            child = CreateChild(chunk);
                            // TOTEST - TODO FIXME - can the weak reference be reused with a new value? or do I need to create a new WeakReference?
                            weakRef.Target = child;
                        }
                    }
                    else
                    {
                        child = CreateChild(chunk);
                        weakChildren.Add(chunk, new WeakReferenceX<IVob>(child));
                    }
                }
#endif
            }

            return child.GetChild(subpathChunks);
        }

        // TODO REVIEW - should this be opt-in?  And/Or should listings escape the environment variable delimiter?
        private (bool goToNext, IVob Vob) TryProcessEnvironmentChunk(string chunk)
        {
            var resolvedChunks = TryResolvePathChunk(chunk);

            if (resolvedChunks.StringChunk != null)
            {
                if (resolvedChunks.StringChunk.Length > 0) return (false, GetChild(resolvedChunks.StringChunk.ToPathElements()));
                else { return (false, null); }
            }
            else if (resolvedChunks.ReferenceChunk != null)
            {
                if (resolvedChunks.ReferenceChunk.IsAbsolute && !(this).IsRoot()) throw new ArgumentException($"Cannot traverse absolute {typeof(VobReference).Name} from a non-root Vob.");
                var referencePathChunks = resolvedChunks.ReferenceChunk.PathChunks;
                if (referencePathChunks.Length == 0) { return (true, null); }
                else { return (false, GetChild(referencePathChunks)); }
            }
            return (false, null);
        }


        public IVob GetChild(string subpath) => GetChild(subpath.ToPathElements()); // RENAME to GetSubPath?  To match IHierarchicalOnDemand
        public IVob QueryChild(string subpath) => QueryChild(subpath.ToPathArray()); // TODO: to path elements?

        /// <summary>
        /// Get the child with the specified name.  The name is the string within subpathChunks found at index 
        /// </summary>
        /// <param name="subpathChunks"></param>
        /// <param name="index"></param>
        /// <returns>Null if any items in subpathChunks are null</returns>
        public IVob GetChild(string[] subpathChunks, int index = 0)
        {
            // SIMILAR logic: GetChild and QueryChild
            IVob intermediateChild;

        start:
            if (subpathChunks == null || index == subpathChunks.Length) { return this; }
            if (index > subpathChunks.Length) throw new ArgumentException("index must be within range of subpathChunks, or equal to subpathChunks.Length");

            string chunk = subpathChunks[index];

            if (chunk == null) return null;

            var x = TryProcessEnvironmentChunk(chunk);
            if (x.Vob != null) return x.Vob.GetChild(subpathChunks, index + 1);
            else if (x.goToNext) { index++; goto start; }

            //var resolvedChunks = TryResolvePathChunk(chunk);

            //if (resolvedChunks.StringChunk != null)
            //{
            //    if (resolvedChunks.StringChunk.Length > 0) return GetChild(resolvedChunks.StringChunk.ToPathElements()).GetChild(subpathChunks, index + 1);
            //    else { index++; goto start; }
            //}
            //else if (resolvedChunks.ReferenceChunk != null)
            //{
            //    if (resolvedChunks.ReferenceChunk.IsAbsolute && !this.IsRoot()) throw new ArgumentException($"Cannot traverse absolute {typeof(VobReference).Name} from a non-root Vob.");
            //    var referencePathChunks = resolvedChunks.ReferenceChunk.PathChunks;
            //    if (referencePathChunks.Length == 0) { index++; goto start; }
            //    else { return GetChild(referencePathChunks).GetChild(subpathChunks, index + 1); }
            //}

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
                intermediateChild = children.GetOrAdd(chunk, key => CreateChild(key));
#if FUTURE_WeakChildReferences
 
                lock (childrenLock)
                {
                    var weakRef = weakChildren.TryGetValue(childName);
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
                        weakChildren.Add(childName, new WeakReferenceX<IVob>(intermediateChild));
                    }
                }
#endif
            }

            if (index == subpathChunks.Length - 1)
            {
                return intermediateChild;
            }
            else
            {
                return intermediateChild[subpathChunks, index + 1];
            }
        }

        public bool DisallowDotDot { get; set; } // TODO: Implement in all relevant methods

        #endregion

        // DUPLICATED - Similar logic as GetChild
        public IVob QueryChild(string[] subpathChunks, int index = 0)
        {
        start:
            if (subpathChunks == null || index == subpathChunks.Length) { return this; }
            if (index > subpathChunks.Length) throw new ArgumentException("index must be within range of subpathChunks, or equal to subpathChunks.Length");

            string chunk = subpathChunks[index];

            var x = TryProcessEnvironmentChunk(chunk);
            if (x.Vob != null) return x.Vob.GetChild(subpathChunks, index + 1);
            else if (x.goToNext) { index++; goto start; }


            //if (chunk.StartsWith("$"))
            //{
            //    var envValue = this.Environment()[chunk.Substring(1)] as string;
            //    if (!string.IsNullOrEmpty(envValue))
            //    {
            //        return QueryChild(envValue.ToPathArray())?.QueryChild(subpathChunks, index + 1);
            //    }
            //    else
            //    {
            //        index++;
            //        goto start;
            //    }
            //}

            IVob intermediateChild;

            string childName = subpathChunks[index];

            if (childName == "..")
            {
                if (DisallowDotDot) throw new NotSupportedException("DisallowDotDot is true.  '..' not allowed in path.");
                intermediateChild = Parent;
            }
            else if (childName == ".")
            {
                intermediateChild = this;
            }
            else
            {
                if (!children.TryGetValue(chunk, out intermediateChild)) { return null; }
#if FUTURE_WeakChildReferences
                var weakRef = weakChildren.TryGetValue(childName);
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
#endif
            }

            return index == subpathChunks.Length - 1 ? intermediateChild : intermediateChild.QueryChild(subpathChunks, index + 1);
        }


        #region (Derived) Index accessors (to GetChild)

        public IVob this[string subpath]
        {
            get
            {
                if (subpath == null)
                {
                    return this;
                }

                return this[subpath.ToPathArray()];
            }
        }

        public IVob this[IEnumerator<string> subpathChunks] => GetChild(subpathChunks);

        public IVob this[IEnumerable<string> subpathChunks] => GetChild(subpathChunks);

        public IVob this[string[] subpathChunks, int index] => GetChild(subpathChunks, index);

        public IVob this[params string[] subpathChunks] => GetChild(subpathChunks);

        #endregion

        #region (Derived) VobReference child getters

        public IVob this[IVobReference reference] => this[reference.Path];

        public IVob GetChild(VobReference reference) => GetChild(reference.Path.ToPathArray(), 0);

        public IVob QueryChild(IVobReference reference) => QueryChild(reference.Path.ToPathArray(), 0);

        #endregion


        public string DumpTree(bool traverseMounts = false)
        {
            return _DumpTree().ToString();
        }

        public StringBuilder _DumpTree(StringBuilder sb = null, string indent = "", bool traverseMounts = false)
        {
            if (sb == null) sb = new StringBuilder();

            foreach (var child in this.Children)
            {
                sb.Append(indent);
                sb.Append("/");
                sb.Append(child.Key);
                sb.AppendLine();
                var childIndent = indent + "  ";
                ((Vob)child.Value)._DumpTree(sb, childIndent, traverseMounts);
            }
            return sb;
        }


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