using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.Instantiating;
using LionFire.ObjectBus;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Structures;
using Microsoft.Extensions.Logging;

namespace LionFire.Vos.Mounts
{

    public class Mount :
#if AOT
        IROStringKeyed
#else
        IKeyed<string>
#endif
        , ITemplateInstance<TMount>
    {
        public TMount Template { get; set; }

        #region Identity

        public  Vob Vob { get; }

        #endregion

        #region Construction

        // REVIEW: streamline constructors around Vob, IReference, MountOptions parameters?  Make a TMount constructor?

        public Mount(Vob mountPointVob, TMount template)
            : this(mountPointVob, template.Reference, template.Options)
        {
            Target = template.Reference;
        }

        //public Mount(Vob vob, IReference targetReference, MountOptions mountOptions = null) : this(vob, null, mountOptions)
        //    //targetReference.ToReadWriteHandle<MountHandleObject>()
        //{
        //    Target = targetReference;
        //    this.MountOptions = mountOptions ?? MountOptions.Default;
        //}

        //public Mount(Vob vob, IReference reference, MountOptions mountOptions = null)
        //    : this(vob, reference.ToReadWriteHandle<MountHandleObject>(), mountOptions)
        //{
        //    Root = reference;
        //}

        public Mount(Vob vob, Vob target, MountOptions mountOptions = null)
            : this(vob, target.Reference, mountOptions)
        {
        }

        public Mount(Vob vob, IReference targetReference,  MountOptions mountOptions = null)
        {
            Target = targetReference;
            this.MountOptions = mountOptions ?? MountOptions.Default;
            //bool enable = false;
            if (vob == null)
            {
                throw new ArgumentNullException($"{nameof(vob)}");
            }

            Vob = vob;
            //this.rootHandle = rootHandle;

            //Vob.Mount(this);

            //var vobNode = vob.GetVobNode
            //VobDepth = vob.VobNode.VobDepth;
            //Target = rootHandle.Reference;
            //Package = mountOptions?.Package;
            //Store = mountOptions?.Store;
            //this.MountName =GetMountName(packageName, layerName);

            //IsEnabled = enable; // Do this last, as it triggers a mount
        }

        #endregion

        #region Derived

        //internal readonly int VobDepth;
        internal int VobDepth => LionPath.GetAbsolutePathDepth(Vob.Path);

        #endregion

        public readonly string Package;
        public readonly string Store;
        //public readonly string MountName;
        //string IKeyed<string>.Key { get { return MountName; } }

        //object IKeyed.Key { get { return this.Root.Key; } }
#if AOT
        string IROStringKeyed.Key { get { return this.Root.Key; } }
#else
        string IKeyed<string>.Key => Target.Key;
#endif


        public static string GetMountName(string packageName = null, string layerName = null)
        {
            var mountName = ((packageName ?? "") + VosPath.LocationDelimiter + (layerName ?? "")).Trim(VosPath.LocationDelimiter);
            return string.IsNullOrWhiteSpace(mountName) ? null : mountName;
        }

        ///// <summary>
        ///// Target Handle MountHandleObject
        ///// </summary>
        //public IReadHandle<MountHandleObject> RootHandle
        //{
        //    get
        //    {
        //        if (rootHandle == null)
        //        {
        //            rootHandle = Target.ToReadHandle<MountHandleObject>();
        //        }
        //        return rootHandle;
        //    }
        //}

        public bool IsEnabled { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        //private IReadHandle<MountHandleObject> rootHandle;

        public readonly IReference Target;

        public readonly MountOptions MountOptions;

        #region State

        //public bool IsEnabled
        //{
        //    get => isEnabled;
        //    set
        //    {
        //        if (isEnabled == value)
        //        {
        //            return;
        //        }

        //        isEnabled = value;

        //        if (value)
        //        {
        //            Vob.Mount(this);
        //        }
        //        else
        //        {
        //            Vob.Unmount(this);
        //        }
        //    }
        //}
        //private bool isEnabled;

        #endregion

        public override string ToString() => $"{{Mount {Vob} ==> {Target}}}";

        #region Misc

        private static readonly ILogger l = Log.Get();

        #endregion
    }
}
