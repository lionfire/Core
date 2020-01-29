using LionFire.Vos.Mounts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace LionFire.Stores.Platforms
{
    
    public class PlatformStores
    {

        //public static TMount GetPlatformCommonStores()
        //{

        //    return new TMount { Reference = EntryAssemblyLocation }
        //}

        public static string EntryAssemblyLocation => Assembly.GetEntryAssembly().Location;

        //public static GetPlatformSpecificStores()
        //{
        ////var desc = RuntimeInformation.OSDescription;
        ////https://stackoverflow.com/questions/2819934/detect-windows-version-in-net/8406674
        //    //RuntimeInformation.FrameworkDescription;

        //    switch (Environment.OSVersion.Platform)
        //    {
        //        case PlatformID.MacOSX:
        //            break;
        //        case PlatformID.Unix:
        //            break;
        //        case PlatformID.Win32NT:

        //            break;
        //        //case PlatformID.Win32S:
        //            //break;
        //        //case PlatformID.Win32Windows:
        //        //    break;
        //        //case PlatformID.WinCE:
        //        //    break;
        //        //case PlatformID.Xbox:
        //        //    break;
        //        default:
        //            break;
        //    }
        //}
    }
}
