#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.Referencing;
using LionFire.Structures;

namespace LionFire.Vos
{

    public abstract class CachingChildren<NodeType>
        where NodeType : CachingChildren<NodeType>
    {
        #region Cached Children

        [Ignore]
        protected MultiBindableDictionary<string, WeakReferenceX<NodeType>> children = new MultiBindableDictionary<string, WeakReferenceX<NodeType>>();

        public bool CacheChildren
        {
            get => children != null;
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
            var ce = (IEnumerable<KeyValuePair<string, WeakReferenceX<NodeType>>>)children;

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

        public NodeType this[string subpath]=> this[0, subpath.ToPathArray()];

        public NodeType this[IEnumerator<string> subpathChunks] => GetChild(subpathChunks);
        
        public NodeType this[int index, string[] subpathChunks]=> GetChild(subpathChunks, index);

        public NodeType this[params string[] subpathChunks] => GetChild(subpathChunks);

        #endregion

        #region Get / Query Logic

        protected abstract NodeType CreateChild(CachingChildren<NodeType> parent, string childName);

        private NodeType GetChild(IEnumerable
#if !AOT
            <string>
#endif
 subpathChunks)
        {
            if (subpathChunks == null || !subpathChunks.Any()) return this as NodeType;

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
                var ccVob = (CachingChildren<NodeType>)vob;

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

        #region Children by VobReference

        public NodeType this[VobReference reference]
        {
            get
            {
                if (reference == null) return this as NodeType;

                return this[reference.Path];
            }
        }

        public NodeType GetChild(VobReference reference)
        {
            return this.GetChild(reference.Path.ToPathArray(), 0);
        }

        public NodeType QueryChild(VobReference reference)
        {
            return this.QueryChild(reference.Path.ToPathArray(), 0);
        }

        #endregion

        #endregion

        #endregion
    }

}
#endif