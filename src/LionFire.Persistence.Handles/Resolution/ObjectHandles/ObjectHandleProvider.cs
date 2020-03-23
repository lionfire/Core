using LionFire.Dependencies;
using LionFire.Referencing;
using System.Collections.Generic;

namespace LionFire.Persistence.Handles
{

    /// <summary>
    /// For an object:
    ///  - If it implements IReferencable, do IReferencable.Reference.GetReadHandle{TValue}, otherwise
    ///  - (if enabled) Check for an existing ObjectHandle for this object, and return it
    ///  - Create and (if enabled) register an ObjectHandle for the object.
    /// </summary>
    public class ObjectHandleProvider : HandleRegistrar<ObjectHandleProviderOptions>, IObjectHandleProvider
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
            if (Options.CheckIReferencable && obj is IReferencable referencable)
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

    public static class ObjectHandleProviderExtensions
    {
        public static IReadHandle<TValue> GetReadHandle<TValue>(this TValue value) => DependencyContext.Current.GetService<ObjectHandleProvider>().GetReadHandle(value);
        public static IReadWriteHandle<TValue> GetReadWriteHandle<TValue>(this TValue value) => DependencyContext.Current.GetService<ObjectHandleProvider>().GetReadWriteHandle(value);
        public static IWriteHandle<TValue> GetWriteHandle<TValue>(this TValue value) => DependencyContext.Current.GetService<ObjectHandleProvider>().GetWriteHandle(value);
    }

    //public enum GetHandleFlags
    //{
    //    None = 0,
    //    GetFromRegistry = 1 << 0,
    //    SaveToRegistry = 1 << 1,
    //}
}