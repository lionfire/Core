//using AppUpdate;
//using AppUpdate.Common;

using System;

namespace LionFire.Applications
{
    [Flags]
    public enum UpdatePolicy
    {
        Unspecified = 0,
        None = 1 << 0,

        /// <summary>
        /// Before starting the primary UI, check for updates, and offer to install update before starting
        /// </summary>
        CheckBeforeStart = 1 << 1,
        CheckAfterStart = 1 << 2,

        AutoDownload = 1 << 10,
        AutoInstall = 1 << 11,
    }
}
