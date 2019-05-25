#if TOPORT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Referencing;
using LionFire.ObjectBus;

namespace LionFire.Vos
{
    public partial class Vob
    {
        #region Children List Accessor

        ///// <summary>
        ///// TODO
        ///// If true, keep the Children list up to date with the children from the underlying mounts
        ///// </summary>
        //public bool SyncChildrenReferences
        //{
        //    get;
        //    set;
        //}

#if !AOT
        public void RetrieveChildrenReferences()
        {
            foreach (var mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
            {
                mount.RootHandle.GetKeys();
            }
        }
#endif

        #region OBase Children Implementation

        // IDEA: Cool idea: Do AsParallel, and populate a custom list class with an "IsDone" option, and maybe download stats like the new windows copy dialog

        public IEnumerable<string> GetChildrenNames(bool includeHidden = false, bool persistedOnly = true)
        {
            var namesDiscovered = new HashSet<string>();
            //List<string> children = new List<string>();
            if (effectiveMountsByReadPriority != null) // RECENTCHANGE 140720 changed == to !=
            {
                if (InitializeEffectiveMounts())
                {
                    foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
                    {
                        foreach (string childName in GetMountHandle(mount).GetKeys())
                        {
                            if (!includeHidden && VosPath.IsHidden(childName)) continue;

                            if (namesDiscovered.Contains(childName)) continue;
                            namesDiscovered.Add(childName);

                            yield return childName;
                        }
                    }
                }
            }
            foreach (var childName in children.Keys)
            {
                if (namesDiscovered.Contains(childName)) continue;
                var child = children[childName].Target;
                if (child == null) continue;
                if (persistedOnly && !child.Handles.Where(h => h.IsPersisted).Any()) continue;

                namesDiscovered.Add(childName);
                yield return childName;
            }
        }

        public IEnumerable<string> GetChildrenNamesOfType(Type childType)
        {
            throw new NotImplementedException("TOPORT");
#if TOPORT
            HashSet<string> namesDiscovered = new HashSet<string>();

            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
            {
                foreach (var c in GetMountHandle(mount).GetChildrenNamesOfType(childType))
                {
                    if (namesDiscovered.Contains(c)) continue;
                    namesDiscovered.Add(c);

                    yield return c;
                }
            }
            //l.Warn("PARTIALLY IMPLEMENTED: GetChildrenNamesOfType - does not filter types");
            //return GetChildrenNames();
#endif
        }

        public IEnumerable<string> GetChildrenNamesOfType<ChildType>()
             where ChildType : class, new()
        {
            throw new NotImplementedException("TOPORT");
#if TOPORT
            HashSet<string> namesDiscovered = new HashSet<string>();

            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
            {
                foreach (var c in GetMountHandle(mount).GetChildrenNamesOfType<ChildType>())
                {
                    if (namesDiscovered.Contains(c)) continue;
                    namesDiscovered.Add(c);
                    yield return c;
                }
            }
            //l.Warn("PARTIALLY IMPLEMENTED: GetChildrenNamesOfType - does not filter types");
            //return GetChildrenNames();
#endif
        }

        //private void RetrieveChildrenHandles()
        //{
        //    List<string> children = new List<string>();
        //    foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
        //    {

        //    }
        //    foreach (var c in GetMountHandle(mount).GetChildrenOfType<ChildType>())
        //    {
        //        yield return c;
        //    }
        //    //return children;
        //}

        public IEnumerable<Vob> GetChildren()
        {

            return this.children.Values.Select(wr => wr.IsAlive ? wr.Target : null).Where(h => h != null);
        }

#if !AOT
        public IEnumerable<VobHandle<ChildType>> GetVobChildrenOfType<ChildType>()
            where ChildType : class, new()
        {
            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
            {
                foreach (var c in GetMountHandle(mount).GetChildrenOfType<ChildType>())
                {
                    yield return new VobHandle<ChildType>(new Vob(vos, this, c.Reference.Name));
                }
            }
        }
#endif

        public IEnumerable<H> GetChildrenOfType(Type childType)
        {
            //List<H<ChildType>> children = new List<H<ChildType>>();
            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
            {
                foreach (var c in GetMountHandle(mount).GetChildrenOfType(childType))
                {
                    yield return c;
                }
            }
            //return children;

            //return this.children.Values.Select(wr => wr.IsAlive ? wr.Target : null).Where(h => h != null).Select(vob => vob.AsVobType<ChildType>());
            //l.Warn("PARTIALLY IMPLEMENTED: GetChildrenNamesOfType - does not filter types");
        }

#if !AOT
        public IEnumerable<H<ChildType>> GetChildrenOfType<ChildType>()
            where ChildType : class//, new()
        {
            //List<H<ChildType>> children = new List<H<ChildType>>();
            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
            {
                var mountHandle = GetMountHandle(mount);

                foreach (var c in mountHandle.GetChildrenOfType<ChildType>())
                {
                    yield return c;
                }
            }
            //return children;

            //return this.children.Values.Select(wr => wr.IsAlive ? wr.Target : null).Where(h => h != null).Select(vob => vob.AsVobType<ChildType>());
            //l.Warn("PARTIALLY IMPLEMENTED: GetChildrenNamesOfType - does not filter types");
        }
#endif

#if !AOT
        public IEnumerable<VobHandle<ChildType>> GetVobHandleChildrenOfType<ChildType>()
            where ChildType : class, new()
        {
            //List<H<ChildType>> children = new List<H<ChildType>>();
            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
            {
                foreach (var c in GetMountHandle(mount).GetChildrenOfType<ChildType>())
                {
                    var vh = this[c.Reference.Name].ToHandle<ChildType>();
                    vh.Mount = mount;
                    //l.Fatal("TEMP GetVobHandleChildrenOfType: " + vh + " mount: " + mount);
                    yield return vh;
                }
            }
            //return children;

            //return this.children.Values.Select(wr => wr.IsAlive ? wr.Target : null).Where(h => h != null).Select(vob => vob.AsVobType<ChildType>());
            //l.Warn("PARTIALLY IMPLEMENTED: GetChildrenNamesOfType - does not filter types");
        }
#endif

        #endregion

        #endregion

    }
}
#endif