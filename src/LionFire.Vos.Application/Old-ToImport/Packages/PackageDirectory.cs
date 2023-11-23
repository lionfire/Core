using LionFire.Vos;
using LionFire.Vos.Mounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Vos.Packages
{
    public class PackageDirectory
    {
        public string Path { get; set; }
        public string LocationName { get; set; }
        public VobMountOptions MountOptions { get; set; }
    }
}
