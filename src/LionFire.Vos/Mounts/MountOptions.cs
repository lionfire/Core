using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using LionFire.Extensions.AssignFrom;

namespace LionFire.ObjectBus
{
    public class MountOptions
    {
        #region (Static)
        
        //internal static readonly MountOptions Default;

        //static MountOptions()
        //{
        //    Default = new MountOptions();
        //}

        #endregion

        public MountOptions() { }

        public MountOptions(MountOptions other)
        {
            this.ShallowAssignFrom(other);
        }
#if AOT
		public void AssignFrom(MountOptions o)
		{
			this.IsExclusive = o.IsExclusive;
			this.IsReadOnly = o.IsReadOnly;
			this.MountAtStartup = o.MountAtStartup;
			this.MountOnDemand = o.MountOnDemand;
			this.TryCreateIfMissing = o.TryCreateIfMissing;
			this.ReadPriority = o.ReadPriority;
			this.WritePriority = o.WritePriority;
		}
#endif

        /// <summary>
        /// Physical mounts should be mounted Exclusive.  That means
        ///  no other mounts can be mounted at or below the mount point (TODO - Not enforced yet)
        ///  TODO - automate for file and physical references
        /// </summary>
        public bool IsExclusive { get; set; }
        public bool IsReadOnly { get; set; }

        public bool TryCreateIfMissing { get; set; }

        public int ReadPriority;
        public int WritePriority;

        #region MountAtStartup

        [DefaultValue(true)]
        public bool MountAtStartup
        {
            get { return mountAtStartup; }
            set { mountAtStartup = value; }
        } private bool mountAtStartup = true;

        #endregion

        #region MountOnDemand

        [DefaultValue(true)]
        public bool MountOnDemand
        {
            get { return mountOnDemand; }
            set { mountOnDemand = value; }
        } private bool mountOnDemand = true;

        #endregion

        //public Type ProviderType { get; set; }
        //public IReference ConnectionReference { get;set;}
        // Overlay 
        //   - Priority
        //   - Allow overlays
        //   - Allow underlays
        //   - Common object merge: this, other, merge based on priority
        //   - Common node merge: this, other, merge based on priority

    }
}
