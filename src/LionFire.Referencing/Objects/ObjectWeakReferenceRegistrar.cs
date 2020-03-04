using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace LionFire.Referencing
{
    public class ObjectWeakReferenceRegistrar
    {
        public static ObjectWeakReferenceRegistrar Default { get; } = new ObjectWeakReferenceRegistrar();

        protected ObjectIDGenerator objectIDGenerator = new ObjectIDGenerator();

        ConcurrentDictionary<long, WeakReference> Dictionary { get; set; }

        public long Register(object obj)
        {
            long key = objectIDGenerator.GetId(obj, out bool firstTime);

            if (firstTime)
            {
                Dictionary.AddOrUpdate(key, new WeakReference(obj), (k, e) => new WeakReference(obj));
            }
            return key;
        }
    }
}
