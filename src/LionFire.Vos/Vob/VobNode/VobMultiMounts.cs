#if UNUSED // ?
using LionFire.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos
{
    public class VobMultiMounts
    {
        public Vob Vob { get; }

        public VobMultiMounts(Vob vob)
        {
            this.Vob = vob;

            
        }

        //private bool AreEffectiveMountsInitialized => effectiveMountsByReadPriority != null;
        private bool AreEffectiveMountsInitialized => areEffectiveMountsInitialized;
        bool areEffectiveMountsInitialized;


        #region EffectiveMounts

        internal MultiValueSortedList<int, Mount> effectiveMountsByReadPriority;
        internal MultiValueSortedList<int, Mount> effectiveMountsByWritePriority;
        private Dictionary<string, Mount> effectiveMountsByName;
        
        //private Vob FirstAncestorWithMounts;
        //private int FirstAncestorWithMountsRelativeDepth;
        //private IEnumerable<string> FirstAncestorToThisSubPath;

        internal bool InitializeEffectiveMounts(bool reset = false)
        {
            if (AreEffectiveMountsInitialized && !reset)
            {
                return true;
            }
            areEffectiveMountsInitialized = true;

            //// TODO: OPTIMIZE: To save memory, pass the buck to the first ancestor that has a mount.
            //if (!HasMounts)
            //{
            ////    effectiveMountsByReadPriority = null;
            ////    effectiveMountsByWritePriority = null;
            ////    effectiveMountsByName = null;

            ////    Vob firstAncestorWithMounts = null;
            ////    IEnumerable<string> firstAncestorToThisSubPath = null;
            ////    for (Vob ancestor = this.Parent; ancestor != null; ancestor = ancestor.Parent)
            ////    {
            ////        if (ancestor.HasMounts)
            ////        {
            ////            firstAncestorWithMounts = ancestor;
            ////            firstAncestorToThisSubPath = this.PathElements.Skip(ancestor.VobDepth);
            ////            break;
            ////        }
            ////    }
            ////    this.FirstAncestorWithMounts = firstAncestorWithMounts;
            ////    this.FirstAncestorToThisSubPath = firstAncestorToThisSubPath;
            ////    return;
            //}
            ////else
            ////{
            ////    this.FirstAncestorWithMounts = this;
            ////    this.FirstAncestorToThisSubPath = new string[] { };
            ////}

            effectiveMountsByReadPriority = new MultiValueSortedList<int, Mount>(
#if AOT
				Singleton<IntComparer>.Instance
#endif
);
            effectiveMountsByWritePriority = new MultiValueSortedList<int, Mount>(
#if AOT
				Singleton<IntComparer>.Instance
#endif
);
            effectiveMountsByName = new Dictionary<string, Mount>(); // FUTURE: If this is rarely used, create on demand

            bool gotSomething = false;
            for (Vob vob = this.Vob; vob != null; vob = vob.Parent)
            {
                foreach (KeyValuePair<string, Mount> kvp in
#if AOT
                        (IEnumerable)
#endif
 vob.Mounts)
                {
                    Mount mount = kvp.Value;
                    effectiveMountsByReadPriority.Add(mount.MountOptions.ReadPriority, mount);
                    gotSomething = true;

                    if (VosConfiguration.AllowIsReadonlyOverride || !mount.MountOptions.IsReadOnly)
                    {
                        effectiveMountsByWritePriority.Add(mount.MountOptions.WritePriority, mount);
                    }
                    effectiveMountsByName.Add(mount.Root.Key, mount);
                }
            }
            if (!gotSomething)
            {
                effectiveMountsByWritePriority = null;
                effectiveMountsByReadPriority = null;
                effectiveMountsByName = null;
                //l.Trace("Got no mounts for " + this.ToString());
                return false;
            }
            return true;
        }


        public void OnAncestorMountsChanged(Vob ancestor, INotifyCollectionChangedEventArgs<Mount> e)
        {
            // TODO: Adapt changes
            InitializeEffectiveMounts(reset: true);
        }

        #endregion

    }
}

#endif