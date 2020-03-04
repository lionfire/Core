using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace LionFire.Referencing
{
    public class ObjectReferenceRegistrar
    {
        public static ObjectReferenceRegistrar Default { get; } = new ObjectReferenceRegistrar();

        protected ObjectIDGenerator objectIDGenerator = new ObjectIDGenerator();

        ConcurrentDictionary<long, object> Dictionary { get; set; }

        public long Register(object obj)
        {
            long key = objectIDGenerator.GetId(obj, out bool firstTime);

            if (firstTime)
            {
                Dictionary.AddOrUpdate(key, obj, (k, e) => obj);
            }
            return key;
        }
    }
}
