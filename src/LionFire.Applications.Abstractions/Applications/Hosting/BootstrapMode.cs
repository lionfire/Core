using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Applications.Hosting
{
    public enum BootstrapMode
    {
        /// <summary>
        /// Keep the ServiceCollection, and rebuild the ServiceProvider
        /// </summary>
        Rebuild = 1,

        /// <summary>
        /// [NOT IMPLEMENTED] Keep the same ServiceCollection parameters, and attempt to later reuse any instances that were created at bootstrap time.
        /// </summary>
        Reuse = 2,

        /// <summary>
        /// Discard the ServiceCollection
        /// </summary>
        Discard = 3,
    }
}
