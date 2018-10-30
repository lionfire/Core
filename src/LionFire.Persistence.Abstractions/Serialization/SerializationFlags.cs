using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Serialization
{
    [Flags]
    public enum SerializationFlags
    {
        None = 0,

        /// <summary>
        /// Exclusive with UseText.  If neither are specified, default will be used.
        /// </summary>
        Binary = 1 << 0,

        /// <summary>
        ///  Exclusive with UseBinary.  If neither are specified, default will be used.
        /// </summary>
        Text = 1 << 2,

        /// <summary>
        /// Only considered if UseText is set
        /// </summary>
        HumanReadable = 1 << 3,
        Uglify = 1 << 4,
        Minify = 1 << 5,


        Compress = 1 << 6,
        Decompress = 1 << 7,
        Serialize = 1 << 8,
        Deserialize = 1 << 9,

    }

#if false
    public enum SerializerCapabilityFlags // REVEW - not sure whether to do this or just have interfaces
    {
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
#endif

    //public static class SerlializationSettingsFlagsExtensions
    //{

    //    public static bool HasBitFlag(this SerlializationSettingsFlags e, SerlializationSettingsFlags other)
    //    {
    //         return ((e & other) == other);
    //    }
    //}
}
