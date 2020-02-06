using System.ComponentModel;

namespace LionFire.Vos
{
    public enum VosFilters
    {
        /// <summary>
        /// Overlay name
        /// </summary>
        [Description(VosPath.LayerDelimiterString)]
        Layer,

        /// <summary>
        /// Mount name
        /// </summary>
        [Description(VosPath.LocationDelimiterString)]
        Location,
    }
}