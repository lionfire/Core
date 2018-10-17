using System;
using System.Collections.Generic;

namespace LionFire.Serialization
{
    public class SerializerSelectionContext
    {
        public Dictionary<string, float> FlagWeights { get; set; } = new Dictionary<string, float>();

        public bool RoundTripCapable { get; set; }

    }

}
