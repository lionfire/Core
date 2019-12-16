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

namespace LionFire.Vos
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

        public readonly Vob Vob;

        #endregion

        #region Construction

        // REVIEW: streamline constructors around Vob, IReference, MountOptions parameters?  Make a TMount constructor?

        public Mount(RootVob root, TMount template)
            : this(root[template.Reference.Path], template.Reference, template.Options)
        {
            Target = template.Reference;
        }

        public Mount(Vob vob, IReference targetReference, MountOptions mountOptions = null) : this(vob, targetReference.ToReadWriteHandle<MountHandleObject>(), mountOptions)
        {
            Target = targetReference;
        }

        //public Mount(Vob vob, IReference reference, MountOptions mountOptions = null)
        //    : this(vob, reference.ToReadWriteHandle<MountHandleObject>(), mountOptions)
        //{
        //    Root = reference;
        //}

        public Mount(Vob vob, Vob target, MountOptions mountOptions = null)
            : this(vob, target.GetHandle<MountHandleObject>(), mountOptions)
        {
        }

        private Mount(Vob vob, IReadWriteHandle<MountHandleObject> rootHandle,  MountOptions mountOptions = null)
        {
            bool enable = false;
            if (vob == null)
            {
                throw new ArgumentNullException($"{nameof(vob)}");
            }

            Vob = vob;
            VobDepth = vob.VobNode.VobDepth;
            Target = rootHandle.Reference;
            this.rootHandle = rootHandle;
            Package = mountOptions?.Package;
            Store = mountOptions?.Store;
            //this.MountName =GetMountName(packageName, layerName);
            MountOptions = mountOptions;//.HasValue ? mountOptions.Value : MountOptions.Default;

            IsEnabled = enable; // Do this last, as it triggers a mount
        }

        #endregion

        #region Derived

        internal readonly int VobDepth;
        
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

        /// <summary>
        /// Target Handle MountHandleObject
        /// </summary>
        public IReadHandle<MountHandleObject> RootHandle
        {
            get
            {
                if (rootHandle == null)
                {
                    rootHandle = Target.ToReadHandle<MountHandleObject>();
                }
                return rootHandle;
            }
        }
        private IReadHandle<MountHandleObject> rootHandle;

        public readonly IReference Target;

        public readonly MountOptions MountOptions;

        #region State

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (isEnabled == value)
                {
                    return;
                }

                isEnabled = value;

                if (value)
                {
                    Vob.Mount(this);
                }
                else
                {
                    Vob.Unmount(this);
                }
            }
        }
        private bool isEnabled;

        #endregion

        public override string ToString() => $"{{Mount {Vob} ==> {Target}}}";

        #region Misc

        private static readonly ILogger l = Log.Get();

        #endregion
    }
}
