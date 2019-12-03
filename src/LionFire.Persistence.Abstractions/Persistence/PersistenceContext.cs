using System;
using LionFire.Serialization;

namespace LionFire.Persistence
{
    /// <summary>
    /// REVIEW - think about this.  What could this be used for?
    ///  - Transactions
    ///  - Overriding default settings for things like
    ///     - PersistenceOptions (IPersister-specific)
    ///     - ISerializationProvider
    ///     - RootPath - TODO - Consider:  RootPath for IPersisters that have a base dir, though that should now instead be done in ICurriedPersister.BaseReference?  Maybe it could be done in both
    ///     
    /// </summary>
    public class PersistenceContext // : IMultiTypable
    {
        //public MultiType MultiTyped => multiTyped.Value;
        //protected Lazy<MultiType> multiTyped = new Lazy<MultiType>();

        //public Func<PersistenceOperation> GetPersistenceOperation { get; set; } // REVIEW - does this have any knowledge of an op?

        public string RootPath { get; set; }
        //public object RootObject { get; set; } // What was the idea for this?

        /// <summary>
        /// Defaults to typeof(object) which will save the full type information.
        /// </summary>
        public Type SaveType { get; set; } // TODO: put the Write<TValue> TValue type into here, so Serializers can look at it?

        public bool AllowInstantiator { get; set; } // What was this for?  Automatic translation of Instantiator to hydrate the actually-desired type?
        
        public SerializationContext SerializationContext { get; set; }
        public ISerializationProvider SerializationProvider { get; set; }

        //public SerializePersistenceContext Serialization { get; set; }
        public DeserializePersistenceContext Deserialization { get; set; }

        public PersistenceOptions PersistenceOptions { get; set; }

    }
}
