using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.VosApp
{
    /// <summary>
    /// Some potential candidates for PackageProvider names
    /// </summary>
    public static class VosAppPackageProviderNames
    {
        /// <summary>
        /// Core data that is considered trusted and part of the essential set of data needed for the application to run
        /// </summary>
        public const string Core = "core";
        
        /// <summary>
        /// Expansions to the core that are typically loaded if present
        /// </summary>
        public const string Expansions = "expansions";

        /// <summary>
        /// Content for the application
        /// </summary>
        public const string Data = "data";

        /// <summary>
        /// Per-User preferences and data.  Typically only the active user has their package activated.
        /// </summary>
        public const string UserData = "userdata";

        /// <summary>
        /// Optional plugins for the application
        /// </summary>
        public const string Plugins = "plugins";

        /// <summary>
        /// Optional extensions for the application
        /// </summary>
        public const string Extensions = "extensions";
    }
}
