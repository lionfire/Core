using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
//using LionFire.Extensions.AssignFrom;

namespace LionFire.Vos.Mounts
{

    public class MountOptions : IMountOptions
    {
        #region (Static)

        public static  MountOptions Default { get; } = new MountOptions();
        public static readonly MountOptions DefaultRead = new MountOptions() { ReadPriority = 1 };
        public static readonly MountOptions DefaultReadWrite = new MountOptions() { ReadPriority = 1, WritePriority = 1 };
        public static readonly MountOptions DefaultWrite = new MountOptions() { WritePriority = 1 };

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

        //public string Package { get; set; }
        //public string Store { get; set; }
        public bool IsDisabled { get; set; }


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

        /// <summary>
        /// Returns true if ReadPriority is null.
        /// Setting to true: will set to null.  Setting to false: will set to 0 if currently null, otherwise current value is kept.
        /// </summary>
        public bool IsReadOnly
        {
            get => !ReadPriority.HasValue;
            
            set {
                if(value)
                {
                    if (!ReadPriority.HasValue) ReadPriority = 1;
                    WritePriority = null;
                }
                else
                {
                    if (!WritePriority.HasValue) WritePriority = 1;
                    // else - keep existing value
                }
            }
        }
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
