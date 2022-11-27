﻿using LionFire.FlexObjects;
using LionFire.MultiTyping;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
//using LionFire.Extensions.AssignFrom;

namespace LionFire.Vos.Mounts;

public class VobMountOptions : IVobMountOptions, IMultiTypable
{
    #region (Static)

    public static VobMountOptions Default { get; } = new VobMountOptions();
    /// <summary>
    /// Default priority for when Read access is requested (implicitly or explicitly) but no priority integer is provided.  If read access is not specified, the default priority should be regarded as null.
    /// </summary>
    public static int DefaultReadPriority = 0;
    /// <summary>
    /// Default priority for when Write access is requested (implicitly or explicitly) but no priority integer is provided.  If write access is not specified, the default priority should be regarded as null.
    /// </summary>
    public static int DefaultWritePriority = 0;
    public static readonly VobMountOptions DefaultRead = new VobMountOptions() { ReadPriority = DefaultReadPriority };
    public static readonly VobMountOptions DefaultReadWrite = new VobMountOptions() { ReadPriority = DefaultReadPriority, WritePriority = DefaultWritePriority };
    public static readonly VobMountOptions DefaultWrite = new VobMountOptions() { WritePriority = DefaultWritePriority };

    #endregion

    #region Construction

    // TODO: Fluent API for:
    // .AugmentWithDefaults
    // .ExclusiveRead
    // .ExclusiveWrite
    public VobMountOptions(int? readPriority = null, int? writePriority = null, string name = null, IMultiTyped decorators = null)
    {
        ReadPriority = readPriority; 
        WritePriority = writePriority; 
        Name = name; 
        MultiTyped = decorators;
    }

    //public MountOptions(MountOptions other)
    //{
    //    throw new NotImplementedException("TODO");
    //    //using LionFire.Extensions.AssignFrom;
    //    //this.ShallowAssignFrom(other);
    //}
    
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

    #endregion

    #region MultiTyping

        // TODO: Replace with Flex?

    public IMultiTyped MultiTyped
    {
        get
        {
            if (multiTyped == null)
            {
                multiTyped = new MultiType();
            }
            return multiTyped;
        }
        private set
        {
            if (multiTyped != null && !multiTyped.IsEmpty)
            {
                throw new AlreadySetException("MultiTyped is already set and is not empty.  Consider merging in additional values.");
            }
        }
    }
    private IMultiTyped multiTyped;



    #endregion

    #region Identity

    public string RootName { get; set; } // TODO: Avoid this in favor of /../<rootName> syntax?

    #endregion

    #region Description

    public string Name { get; set; }

    #endregion

    #region Settings

    //public string Package { get; set; }
    //public string Store { get; set; }
    public bool IsManuallyEnabled { get; set; }

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

    /// <summary>
    /// True for locations such as:
    ///  - c:\Users\*\* (Except Public/All Users type users)
    ///  - /home/*
    /// False:
    ///  - other locations
    /// </summary>
    public bool? IsOwnedByOperatingSystemUser { get; set; } // True for folders under 

    /// <summary>
    /// True for locations such as:
    ///  - /var/*
    ///  - c:\ProgramData\*
    ///  - c:\Users\*\AppData\*
    ///  
    /// False for locations such as:
    ///  - c:\Program Files\*
    ///  - /usr/lib/*
    ///  - /home/{user}/.packages/*
    /// </summary>
    public bool? IsVariableDataLocation { get; set; } 

    //#region MountAtStartup

    //[DefaultValue(true)]
    //public bool MountAtStartup
    //{
    //    get { return mountAtStartup; }
    //    set { mountAtStartup = value; }
    //}
    //private bool mountAtStartup = true;

    //#endregion

    //#region MountOnDemand

    //[DefaultValue(true)]
    //public bool MountOnDemand
    //{
    //    get { return mountOnDemand; }
    //    set { mountOnDemand = value; }
    //}
    //private bool mountOnDemand = true;

    //#endregion

    #region Derived

    /// <summary>
    /// Returns true if ReadPriority is null.
    /// Setting to true: will set to null.  Setting to false: will set to 0 if currently null, otherwise current value is kept.
    /// </summary>
    public bool IsReadOnly
    {
        get => !ReadPriority.HasValue;

        set
        {
            if (value)
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
    #endregion

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
    public static bool IsReadOnly(this IVobMountOptions mountOptions) => !mountOptions.IsWritable;
}
