using System;
using LionFire.Serialization;

namespace LionFire.Persistence
{
    public class PersistenceContext
    {
        public string RootPath { get; set; }
        public object RootObject { get; set; }

        /// <summary>
        /// Defaults to typeof(object) which will save the full type information.
        /// </summary>
        public Type SaveType { get; set; }

        public bool AllowInstantiator { get; set; }
        public SerializationContext SerializationContext { get; set; }
        public Func<SerializationOperation> SerializationOperationFunc { get; set; }
    }
}
