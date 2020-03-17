using LionFire.Dependencies;
using LionFire.Referencing;
using System.Collections.Generic;

namespace LionFire.Persistence.Handles
{
    // TODO: When a handle created elsewhere retrieves an object, optionally have it register the value/handle combo here (perhaps via a global event bus).

    /// <summary>
    /// For an object:
    ///  - If it implements IReferencable, do IReferencable.Reference.GetReadHandle{TValue}, otherwise
    ///  - (if enabled) Check for an existing ObjectHandle for this object, and return it
    ///  - Create and (if enabled) register an ObjectHandle for the object.
    /// </summary>
    public class ObjectHandleRegistrar : HandleRegistrar, IObjectHandleProvider
    {
        IEnumerable<IReadHandleProvider> readHandleProviders;
        IEnumerable<IReadWriteHandleProvider> readWriteHandleProviders;
        IEnumerable<IWriteHandleProvider> writeHandleProviders;

        public ObjectHandleRegistrar(IEnumerable<IReadWriteHandleProvider> rws, IEnumerable<IReadHandleProvider> rs, IEnumerable<IWriteHandleProvider> ws)
        {
            this.readWriteHandleProviders = rws;
            this.readHandleProviders = rs;
            this.writeHandleProviders = ws;
        }

        protected override IReadHandle<TValue> CreateReadHandle<TValue>(TValue obj)
        {
            if (obj is IReferencable referencable)
            {
                foreach (var r in readHandleProviders)
                {
                    var result = r.GetReadHandle<TValue>(referencable.Reference, obj);
                    if (result != null) return result;
                }
            }
            return obj.ToObjectHandle();
        }
        protected override IReadWriteHandle<TValue> CreateReadWriteHandle<TValue>(TValue obj) => obj.ToObjectHandle();
        protected override IWriteHandle<TValue> CreateWriteHandle<TValue>(TValue obj) => obj.ToObjectHandle();
    }

    public static class ObjectHandleRegistrarExtensions
    {
        public static IReadHandle<TValue> GetReadHandle<TValue>(this TValue value) => DependencyContext.Current.GetService<ObjectHandleRegistrar>().GetReadHandle(value);
        public static IReadWriteHandle<TValue> GetReadWriteHandle<TValue>(this TValue value) => DependencyContext.Current.GetService<ObjectHandleRegistrar>().GetReadWriteHandle(value);
        public static IWriteHandle<TValue> GetWriteHandle<TValue>(this TValue value) => DependencyContext.Current.GetService<ObjectHandleRegistrar>().GetWriteHandle(value);
    }
}