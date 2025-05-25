using LionFire.Dependencies;
using LionFire.Referencing;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Persistence.Handles
{

    /// <summary>
    /// For an object:
    ///  - If it implements IReferenceable, do IReferenceable.Reference.GetReadHandle{TValue}, otherwise
    ///  - (if enabled) Check for an existing ObjectHandle for this object, and return it
    ///  - Create and (if enabled) register an ObjectHandle for the object.
    /// </summary>
    public class ObjectHandleProvider : ObjectHandleRegistrar<ObjectHandleProviderOptions>, IObjectHandleProvider
    {
        IEnumerable<IReadHandleProvider> readHandleProviders;
        IEnumerable<IReadWriteHandleProvider> readWriteHandleProviders;
        IEnumerable<IWriteHandleProvider> writeHandleProviders;

        public ObjectHandleProvider(IEnumerable<IReadWriteHandleProvider> rws, IEnumerable<IReadHandleProvider> rs, IEnumerable<IWriteHandleProvider> ws, ObjectHandleProviderOptions options) : base(options)
        {
            this.readWriteHandleProviders = rws;
            this.readHandleProviders = rs;
            this.writeHandleProviders = ws;
        }


        protected override IReadHandle<TValue> CreateReadHandle<TValue>(TValue obj)
        {
            if (Options.CheckIReferenceable && obj is IReferenceable referencable)
            {
                foreach (var r in readHandleProviders.OfType<IPreresolvableReadHandleProvider>())
                {
                    var result = r.GetReadHandlePreresolved(referencable.Reference, obj);
                    if (result != null) return result;
                }
            }
            return obj.ToObjectHandle();
        }
        protected override IReadWriteHandle<TValue> CreateReadWriteHandle<TValue>(TValue obj) => obj.ToObjectHandle();
        protected override IWriteHandle<TValue> CreateWriteHandle<TValue>(TValue obj) => obj.ToObjectHandle();


        
    }

    public static class ObjectHandleProviderExtensions
    {
        public static IReadHandle<TValue> GetObjectReadHandle<TValue>(this TValue value) => DependencyContext.Current.GetRequiredService<IObjectHandleProvider>().GetReadHandle(value);
        public static IReadWriteHandle<TValue> GetObjectReadWriteHandle<TValue>(this TValue value) => DependencyContext.Current.GetRequiredService<IObjectHandleProvider>().GetReadWriteHandle(value);
        public static IWriteHandle<TValue> GetObjectWriteHandle<TValue>(this TValue value) => DependencyContext.Current.GetRequiredService<IObjectHandleProvider>().GetWriteHandle(value);
    }

    //public enum GetHandleFlags
    //{
    //    None = 0,
    //    GetFromRegistry = 1 << 0,
    //    SaveToRegistry = 1 << 1,
    //}
}