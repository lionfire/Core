using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public partial class Vob
    {
        public IEnumerable<RH<MountHandleObject>> ReadHandles
        {
            get
            {
                if (!InitializeEffectiveMounts())
                {
                    yield break;
                }
                //if (HasMounts)
                {
                    foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
                    {
                        yield return GetMountHandle(mount);
                    }
                }
            }
        }

        //private IEnumerable<KeyValuePair<H, Mount>> ReadHandlesWithMounts
        //{
        //    get
        //    {
        //        if (!InitializeEffectiveMounts()) yield break;
        //        //if (HasMounts)
        //        {
        //            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
        //            {
        //                yield return new KeyValuePair<R, Mount>(GetMountHandle(mount), mount);
        //            }
        //        }
        //    }
        //}

        private IEnumerable<Mount> ReadHandleMounts
        {
            get
            {
                if (!InitializeEffectiveMounts())
                {
                    yield break;
                }
                //if (HasMounts)
                {
                    foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
                    {
                        yield return mount;
                    }
                }

            }
        }

        public bool CanWrite => WriteHandleMounts.Where(m => !m.MountOptions.IsReadOnly).Any();

        public IEnumerable<RH<MountHandleObject>> WriteHandles
        {
            get
            {
                if (!InitializeEffectiveMounts())
                {
                    yield break;
                }

                //if (!HasMounts) yield break;
                foreach (Mount mount in effectiveMountsByWritePriority.Values.SelectMany(x => x))
                {
                    yield return GetMountHandle(mount);
                }
            }
        }

        //private IEnumerable<KeyValuePair<H, Mount>> WriteHandlesWithMounts
        //{
        //    get
        //    {
        //        if (!InitializeEffectiveMounts()) yield break;
        //        //if (HasMounts)
        //        {
        //            foreach (Mount mount in effectiveMountsByWritePriority.Values.SelectMany(x => x))
        //            {
        //                yield return new KeyValuePair<H, Mount>(GetMountHandle(mount), mount);
        //            }
        //        }

        //    }
        //}
        private IEnumerable<Mount> WriteHandleMounts
        {
            get
            {
                if (!InitializeEffectiveMounts())
                {
                    yield break;
                }
                //if (HasMounts)
                {
                    foreach (Mount mount in effectiveMountsByWritePriority.Values.SelectMany(x => x))
                    {
                        yield return mount;
                    }
                }

            }
        }

        private IEnumerable<Mount> EffectiveWriteMounts
        {
            get
            {
                if (effectiveMountsByWritePriority == null) { InitializeEffectiveMounts(); }
                if (effectiveMountsByWritePriority == null) { return Enumerable.Empty<Mount>(); }
                return effectiveMountsByWritePriority.Values.SelectMany(x => x);
            }
        }
        private RH<MountHandleObject> FirstWriteHandle // REVIEW Don't use this?
        {
            get
            {
                //if (!HasMounts) return null;
                foreach (Mount mount in
#if AOT
                        (IEnumerable)
#endif
 EffectiveWriteMounts)
                {
                    if (mount.MountOptions.IsReadOnly && !VosContext.Current.IgnoreReadonly)
                    {
                        continue;
                    }

                    return GetMountHandle(mount);
                }
                return null;
            }
        }
        private H<T> GetFirstWriteHandle<T>()
        {
            //get
            {
                //if (!HasMounts) return null;
                foreach (Mount mount in
#if AOT
                        (IEnumerable)
#endif
 EffectiveWriteMounts)
                {
                    if (mount.MountOptions.IsReadOnly && !VosContext.Current.IgnoreReadonly)
                    {
                        continue;
                    }

                    return GetHandleFromMount<T>(mount);
                }
                return null;
            }
        }

        //private VobHandle<T> GetFirstWriteHandle<T>()
        //{
        //    H objectHandle;
        //    objectHandle = FirstWriteHandle<T>();
        //    return objectHandle;
        //}

        private RH<MountHandleObject> GetFirstWriteHandle()
        {
            RH<MountHandleObject> objectHandle;

            //if (package == null && layer == null)
            {
                objectHandle = FirstWriteHandle;
                return objectHandle;
            }
            //            else
            //            {
            //                if (package != null && layer != null)
            //                {
            //                    string mountName = LionFire.Vos.Mount.GetMountName(package, layer);
            //                    Mount mount = effectiveMountsByName.TryGetValue(mountName);
            //                    if (mount.MountOptions.IsReadOnly)
            //                    {
            //                        l.Trace("GetFirstWriteHandle found mount by name but it IsReadOnly");
            //                        return null;
            //                    }
            //                    else
            //                    {
            //                        return GetMountHandle(mount);
            //                    }
            //                }
            //                else
            //                {
            //                    if (package != null)
            //                    {
            //                        return GetMountHandle(EffectiveWriteMounts.Where(m => m.PackageName == package).FirstOrDefault());
            //                    }
            //                    else // layer != null
            //                    {
            //#if SanityChecks
            //                        if (layer == null) throw new UnreachableCodeException("layer != null");
            //#endif
            //                        return GetMountHandle(EffectiveWriteMounts.Where(m => m.LayerName == layer).FirstOrDefault());
            //                    }
            //                }
            //            }
        }

    }
}
