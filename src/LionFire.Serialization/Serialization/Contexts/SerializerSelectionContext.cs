using System;
using System.Collections.Generic;

namespace LionFire.Serialization
{
    public class SerializerSelectionContext
    {
        public Dictionary<string, float> FlagWeights { get; set; } = new Dictionary<string, float>();

        public bool RoundTripCapable { get; set; }

        public SerializerSelectionScores Scores { get; set; } = new SerializerSelectionScores();

    }

    public class SerializerSelectionScores
    {
        public float SupportedFileExtension { get; set; } = 100;
        public float SupportedMimeType { get; set; } = 100;
    }

}
