using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Serialization
{
    [Flags]
    public enum SerlializationSettingsFlags
    {
        None = 0,

        /// <summary>
        /// If not set, binary will be used.
        /// </summary>
        UseText = 1 << 0,

        /// <summary>
        /// Only considered if UseText is set
        /// </summary>
        UseHumanReadable = 1 << 1,

        /// <summary>
        /// Serializers have this capability if they can detect their own format with no extra identifiers.
        /// </summary>
        ImplicitlyIdentifySerializer = 1 << 9,

        /// <summary>
        /// Prepend an identification token at the beginning to indicate which serialization mechanism is used.
        /// </summary>
        ExplicitlyIdentifySerializer = 1 << 10,

        /// <summary>
        /// Prepend an identification token at the beginning to indicate the LionSerialization framework was used to serialize the object.  This comes before the Serializer identifier.
        /// </summary>
        IdentifyFramework = 1 << 11,

        UseDefaults = 1 << 32,
    }

    public static class SerlializationSettingsFlagsExtensions
    {

        public static bool HasBitFlag(this SerlializationSettingsFlags e, SerlializationSettingsFlags other)
        {
             return ((e & other) == other);
        }
    }
}
