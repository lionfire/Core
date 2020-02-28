using System;
using LionFire.IO;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Structures;

namespace LionFire.Persistence
{
    public static class PersistenceOperationFactory
    {
        public static Lazy<PersistenceOperation> Serialize<T>(IReference reference, object obj, ReplaceMode replaceMode, SerializationOptions serializationOptions = null, Action<PersistenceOperation> initializer = null)
         => ((Func<PersistenceOperation>)(() =>
         {
             var result = new PersistenceOperation(reference, new SerializePersistenceOperation()
             {
                 Object = obj,
                 ReplaceMode = replaceMode,
                 SerializationOptions = serializationOptions,
             })
             {
                 Type = typeof(T),
             };
             initializer?.Invoke(result);
             return result;
         })).ToLazy();

        public static Lazy<PersistenceOperation> Deserialize<T>(IReference reference, SerializationOptions serializationOptions = null, Action<PersistenceOperation> initializer = null)
              => ((Func<PersistenceOperation>)(() =>
              {
                  var result = new PersistenceOperation(reference, new DeserializePersistenceOperation()
                  {
                      SerializationOptions = serializationOptions,
                  })
                  {
                      Type = typeof(T),
                  };
                  initializer?.Invoke(result);
                  return result;
              })).ToLazy();

    }
}
