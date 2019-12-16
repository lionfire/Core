using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
//using LionFire.Extensions.AssignFrom;

namespace LionFire.Vos
{

    public class MountOptions : IMountOptions
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
            throw new NotImplementedException("TODO");
            //using LionFire.Extensions.AssignFrom;
            //this.ShallowAssignFrom(other);
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

        public string RootName { get; set; }

        public string Name { get; set; }

        public string Package { get; set; }
        public string Store { get; set; }
        public bool Enable { get; set; }


        /// <summary>
        /// Physical mounts should be mounted Exclusive.  That means
        ///  no other mounts can be mounted at or below the mount point (TODO - Not enforced yet)
        ///  TODO - automate for file and physical references
        /// </summary>
        public bool IsExclusive { get; set; } = true;

        public bool IsExclusiveWithReadAndWrite { get; set; } = true;
        /// <summary>
        /// Means no other mounts can be mounted below this Vob
        /// </summary>
        public bool IsSealed { get; set; } = true;
        public bool IsWritable { get; set; } = false;

        public bool TryCreateIfMissing { get; set; }

        public int? ReadPriority { get; set; }
        public int? WritePriority { get; set; }

        #region MountAtStartup

        [DefaultValue(true)]
        public bool MountAtStartup
        {
            get { return mountAtStartup; }
            set { mountAtStartup = value; }
        }
        private bool mountAtStartup = true;

        #endregion

        #region MountOnDemand

        [DefaultValue(true)]
        public bool MountOnDemand
        {
            get { return mountOnDemand; }
            set { mountOnDemand = value; }
        }
        private bool mountOnDemand = true;

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

    public static class MountOptionsExtensions
    {
        public static bool IsReadOnly(this MountOptions mountOptions) => !mountOptions.IsWritable;
    }
}
