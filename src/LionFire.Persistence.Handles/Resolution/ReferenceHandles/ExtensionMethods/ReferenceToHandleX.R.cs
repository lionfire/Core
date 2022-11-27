using LionFire.Persistence.Handles;
using LionFire.Persistence;
using System;
using LionFire.Referencing.Ex;
using LionFire.Dependencies;
using System.Reflection;
using System.Linq;

namespace LionFire.Referencing; // REVIEW - should be in another namespace?

public static partial class ReferenceToReadHandleExtensions
{
    // TODO: Review and make Read/ReadWrite/Write consistent

    #region Not preresolved

    // Generic, typed Reference
    public static IReadHandle<TValue> GetReadHandle<TValue, TReference>(this TReference reference, IServiceProvider serviceProvider = null)
        where TReference : IReference
        => HandleRegistry.GetOrAddRead<IReadHandle<TValue>>(reference.Url, _ => reference.TryGetReadHandleProvider<TReference>(serviceProvider).GetReadHandle<TValue>(reference));

    // Generic, ITypedReference  REVIEW - why is this different than ReadWriteHandleExtensions?
    // UNUSED? - Trying to comment this since I'm not sure if it's really needed. Uncomment if used.  
    //public static IReadHandle<TValue> GetReadHandle<TValue, TReference>(this ITypedReference<TValue, TReference> reference, IServiceProvider serviceProvider = null)
    //    where TReference : IReference
    //    => HandleRegistry.GetOrAddRead<IReadHandle<TValue>>(reference.Reference.Url, _ => reference.Reference.TryGetReadHandleProvider(serviceProvider).GetReadHandle<TValue>(reference.Reference));

    // Generic
    public static IReadHandle<TValue> GetReadHandle<TValue>(this IReference reference, IServiceProvider serviceProvider = null)
        => HandleRegistry.GetOrAddRead<IReadHandle<TValue>>(reference?.Url ?? throw new ArgumentNullException(nameof(reference)),
            _ => reference.GetReadHandleProvider().GetReadHandle<TValue>(reference));

    // Non-generic (reflection helper)
    public static IReadHandle GetReadHandle(this IReference reference, Type type, IServiceProvider serviceProvider = null)
        => (IReadWriteHandle)typeof(ReferenceToReadHandleExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.Name == nameof(GetReadHandle) && mi.GetGenericArguments().Length == 1).First().MakeGenericMethod(type).Invoke(null, new object[] { reference });

    #endregion

    #region Preresolved 
    // no overwrite available, unlike for ReadWriteHandles

    // Non-generic
    public static (IReadHandle<TValue> handle, bool usedPreresolved) GetReadHandlePreresolved<TValue>(this IReference reference, TValue preresolvedValue, IServiceProvider serviceProvider = null)
    {
        bool usedPreresolved = false;
        return (HandleRegistry.GetOrAddRead<IReadHandle<TValue>>(reference.Url, _ =>
        {
            usedPreresolved = true;
            var handle = reference.GetReadHandleProvider(serviceProvider).GetReadHandle(reference, preresolvedValue);
            return handle;
        }), usedPreresolved);

#if OLD
        // OLD - Upcast approach.  Current approach above just relies on the full non-generic stack.
        //  - gets a generic version of IReadHandleProvider<TReference> where TReference is the type of IReference
        // - gets a generic version of the GetReadHandle<T> where T is TValue
        //  - gets a Generic version of the IReferenceToHandleService.GetReadHandleProvider method

        //// REVIEW - should this be using IReferenceToHandleService?
        //// REVIEW - this seems crazy.  Is it slow?  Should an [Obsolete] tell the user to use the <TValue, TReference> overload instead?
        var TReference = reference.GetType();
        return ((IReadHandle<TValue>)(
            typeof(IReadHandleProvider<>).MakeGenericType(TReference)
                .GetMethod(nameof(ReferenceToReadHandleExtensions.GetReadHandle)).MakeGenericMethod(typeof(TValue))
                    .Invoke((typeof(ReferenceToHandleProviderExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(mi => mi.Name == nameof(IReferenceToHandleService.GetReadHandleProvider) && mi.ContainsGenericParameters).First()
                    .MakeGenericMethod(TReference)
                    .Invoke(null, new object[] { /* Upcast */ reference, default(TValue), serviceProvider }))
            , new object[] { reference })), usedPreresolved);
#endif
    }

    #endregion
        
    public static IReadHandle<TValue> CreateReadHandle<TValue>(this IReference reference) => throw new NotImplementedException(); // FUTURE

    #region Existing

    public static IReadHandle GetExistingReadHandle(this IReferencable referencable)
    {
        if (referencable.Reference == null) { return null; }

        Type referenceValueType = null;

        {
            if (referencable.Reference is IReferencableValueType rvt) { referenceValueType = rvt.ReferenceValueType ?? throw new ArgumentNullException($"{typeof(IReferencableValueType).Name}.{nameof(rvt.ReferenceValueType)}"); }

            if (referencable.Reference is ITypedReference tr)
            {
                referenceValueType = tr.Type ?? throw new ArgumentNullException($"{typeof(ITypedReference).Name}.{nameof(tr.Type)}");
            }
        }

        if (referenceValueType == null) { throw new ArgumentException($"{nameof(referencable)} must implement IReferencableValueType, or its Reference must implement ITypedReference."); } // ENH: scan the object for IReadHandle<T>

        if (referencable is IHasReadHandle ihrh)
        {
            var type = typeof(IHasReadHandle<>).MakeGenericType(referenceValueType);
            if (type.IsAssignableFrom(referencable.GetType()))
            {
                return (IReadHandle)type.GetProperty(nameof(IHasReadHandle<object>.ReadHandle)).GetValue(referencable);
            }
        }
        return (IReadHandle)typeof(ReferenceToReadHandleExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.Name == nameof(GetReadHandle) && mi.GetGenericArguments().Length == 1 & mi.GetParameters().Length == 1).Single().MakeGenericMethod(referenceValueType).Invoke(null, new object[] { referencable.Reference });

    }
    public static IReadHandle<T> GetExistingReadHandle<T>(this IReference reference)
        => (IReadHandle<T>)HandleRegistry.ReadHandles.TryGetValue(reference?.Url ?? throw new ArgumentNullException(nameof(reference)));

    #endregion

}
