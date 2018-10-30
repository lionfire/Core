using System.IO;

namespace LionFire.Persistence
{
    public class SerializePersistenceOperation
    {
        //public PersistenceDirection PersistenceDirection => PersistenceDirection.Serialize;
        /// <summary>
        /// Can be used to determine serialization strategy
        /// FUTURE: Also allow this to be used in place of a parameter to ToString/ToBytes/ToStream?
        /// </summary>
        public object Object { get; set; }

        public Stream Stream { get; set; }
    }

    //public class MimeSerializationContext
    //{
    //    public string MimeType { get; set; }
    //}
}
