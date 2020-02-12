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

    public class Mount : IMount,
#if AOT
        IROStringKeyed
#else
        IKeyed<string>
#endif
    {

        #region Identity

        public IVob MountPoint { get; }

        public IReference Target { get; }
        string IKeyed<string>.Key => Target.Key;

        #endregion

        #region Options

        public IMountOptions Options { get; }

        #endregion

        #region Construction

        public Mount(IVob vob, IReference targetReference, IMountOptions mountOptions = null)
        {
            MountPoint = vob ?? throw new ArgumentNullException($"{nameof(vob)}");
            Target = targetReference;
            this.Options = mountOptions ?? MountOptions.Default;
        }

        public Mount(IVob vob, IVob target, IMountOptions mountOptions = null)
            : this(vob, target.Reference, mountOptions)
        {
        }

        public Mount(IVob mountPointVob, TMount tMount)
            : this(mountPointVob, tMount.Reference, tMount.Options)
        {

            if (true != tMount.MountPoint.ToVob(mountPointVob.Root)?.Path?.Equals(mountPointVob.Reference.Path)) throw new ArgumentException("mountPointVob.Reference does not match tMount.MountPoint");
        }

        #endregion

        #region Derived

        public int VobDepth => LionPath.GetAbsolutePathDepth(MountPoint.Path);

        #endregion

        #region State

        #region IsEnabled

        public bool IsEnabled
        {
            get => isEnabled || Options?.IsManuallyEnabled != true;
            set
            {
                if (isEnabled == value) return;
                isEnabled = value;
                IsEnabledChanged?.Invoke(value);
            }
        }
        private bool isEnabled;

        public event Action<bool> IsEnabledChanged;

        #endregion

        #endregion


        #region REVIEW

        // REVIEW - These used to make the mount referencable via package / store name.  Do I still want this?

        //public string Package => Options.Package;
        //public string Store => Options.Store;

        #endregion

        #region OLD

        //, ITemplateInstance<TMount>
        //public TMount Template { get; set; }

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

        //internal readonly int VobDepth;
        //public readonly string MountName;
        //string IKeyed<string>.Key { get { return MountName; } }

        //object IKeyed.Key { get { return this.Root.Key; } }
        // REVIEW - was this Mount name concept useful?  I don't want it to depend on Packages
        //public static string GetMountName(string packageName = null, string layerName = null)
        //{
        //    var mountName = ((packageName ?? "") + VosPath.LocationDelimiter + (layerName ?? "")).Trim(VosPath.LocationDelimiter);
        //    return string.IsNullOrWhiteSpace(mountName) ? null : mountName;
        //}

        // REVIEW - What was this MountHandleObject for?  I don't think I need it
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

        #endregion

        #region Misc

        public override string ToString() => $"{{Mount {MountPoint} ==> {Target}{(IsEnabled ? "" : " (disabled)")}}}";
        private static readonly ILogger l = Log.Get();

        #endregion
    }
}
