
using LionFire.Dependencies;
using LionFire.Serialization;
using System.Text;

namespace LionFire.Persistence
{
    public class PersistenceOptions : IPersistenceOptions
    {
        public bool ThrowOnMissingSerializer = true;

        public SerializationOptions SerializationOptions { get; set; }

        /// <summary>
        /// Recommended to have this on.  Otherwise, deserialization may fail and you may not know why.
        /// </summary>
        public bool ThrowDeserializationFailureWithReasons { get; set; } = true;

        public bool ThrowOnDeserializationFailure { get; set; } = true;

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        //public PersistenceOptions(ISerializationProvider serializationProvider)
        //{
        //    SerializationProvider = serializationProvider;
        //}

        public bool? PreferStreamReading { get; set; } = true;
        public bool? PreferStreamWriting { get; set; } = true;

        public PersistenceOperation RetrievePersistenceOperationDefaults { get; set; }
        //    = new PersistenceOperation()
        //{
        //    AutoAppendExtension = AutoAppendExtension.Disabled,
        //    Deserialization = new DeserializePersistenceOperation()
        //    {

        //    },
        //};
        public PersistenceOperation PutPersistenceOperationDefaults { get; set; }
        //    = new PersistenceOperation()
        //{
        //    AutoAppendExtension = AutoAppendExtension.Disabled,
        //    //Serialization =  new SerializePersistenceOperation()
        //    //{
        //    //},
        //};

        // FUTURE: IEnumerable<IPersistenceInterceptor> RetrieveInterceptors, PutInterceptors

        //public ISerializationProvider SerializationProvider
        //{
        //    get;
        //    protected set;
        //    //get
        //    //{
        //    //    if (defaultSerializationProvider == null)
        //    //    {
        //    //        defaultSerializationProvider = DependencyLocator.TryGet<ISerializationProvider>();
        //    //    }
        //    //    return defaultSerializationProvider;
        //    //}
        //}
        //private ISerializationProvider defaultSerializationProvider;
    }
}
