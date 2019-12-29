using LionFire.Structures;
using LionFire.Vos.Mounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Vos
{
    public class RootVob : Vob
    {
        public VosOptions VosOptions { get; }

        public static bool AllowMultipleDefaultRoots { get; set; } = false;

        /// <summary>
        /// Empty for default Root
        /// </summary>
        public string RootName { get; }

        public RootVob(VosOptions vosOptions) : base(null, String.Empty)
        {
            if (!AllowMultipleDefaultRoots)
            {
                if (ManualSingleton<RootVob>.Instance != null)
                {
                    throw new AlreadySetException("A default RootVob has already been created.  There can only be one default.  If you wish to create another, provide a rootName.  Set AllowMultipleDefaultRoots to true to allow multiple default RootVobs (only recommended for unit testing or special cases.)");
                }
                else
                {
                    ManualSingleton<RootVob>.Instance = this;
                }
            }
            this.RootName = "";

            foreach (var tMount in vosOptions.MountsForRootName(RootName))
            {
                this.Mount(tMount);
                ////var mount = new Mount(Vob[tMount.Reference.Path], tMount.Reference, tMount.Options?.Package, tMount.Options?.Store, tMount.Options?.Enable ?? true, tMount.Options);
                //var mount = new Mount(Vob[tMount.Reference.Path], tMount.Reference, tMount.Options);
                //Mount(mount);
            }
        }

        public RootVob(string rootName, VosOptions vosOptions) : this(vosOptions)
        {
            this.RootName = rootName;
        }

        
    }
}
