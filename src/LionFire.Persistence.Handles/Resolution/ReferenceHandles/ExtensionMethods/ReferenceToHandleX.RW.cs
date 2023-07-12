#nullable enable
using LionFire.Persistence.Handles;
using LionFire.Persistence;
using System;
using LionFire.Referencing.Ex;
using LionFire.Dependencies;
using System.Reflection;
using System.Linq;

namespace LionFire.Referencing; // REVIEW - should be in another namespace?

public static partial class ReferenceToReadWriteHandleExtensions
{
    // TODO: Add optional IServiceProvider parameter and implement everywhere

    #region Not preresolved

    // Generic, typed Reference
    public static IReadWriteHandle<TValue> GetReadWriteHandle<TValue, TReference>(this TReference reference, IServiceProvider? serviceProvider = null)
          where TReference : IReference
        => HandleRegistry.GetOrAddReadWrite<IReadWriteHandle<TValue>>(reference?.Url ?? throw new ArgumentNullException(nameof(reference)), _
            => reference.TryGetReadWriteHandleProvider<TReference>(serviceProvider).GetReadWriteHandle<TValue>(reference));

    // Generic
    public static IReadWriteHandle<TValue> GetReadWriteHandle<TValue>(this IReference reference, IServiceProvider? serviceProvider = null)
        => HandleRegistry.GetOrAddReadWrite<IReadWriteHandle<TValue>>(reference?.Url ?? throw new ArgumentNullException(nameof(reference)),
            _ => reference.GetReadWriteHandleProvider(serviceProvider).GetReadWriteHandle<TValue>(reference));

    // Non-generic (reflection helper)
    public static IReadWriteHandle GetReadWriteHandle(this IReference reference, Type type, IServiceProvider? serviceProvider = null)

        => (IReadWriteHandle)typeof(ReferenceToReadWriteHandleExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.Name == nameof(GetReadWriteHandle) && mi.GetGenericArguments().Length == 1).First().MakeGenericMethod(type).Invoke(null, new object[] { reference, serviceProvider });

    #endregion

    #region Preresolved

    // Generic, typed Reference
    public static (IReadWriteHandle<TValue> handle, bool usedPreresolved) GetReadWriteHandlePreStaged<TValue, TReference>(this TReference reference, TValue preStagedValue = default, bool overwriteValue = false)
          where TReference : IReference
    {
        bool usedPreresolved = false;

        var handle = HandleRegistry.GetOrAddReadWrite<IReadWriteHandle<TValue>>(reference?.Url ?? throw new ArgumentNullException(nameof(reference)), _ => reference.TryGetReadWriteHandleProvider<TReference>().GetReadWriteHandle<TValue>(reference, preStagedValue));

        if (!usedPreresolved && overwriteValue) { handle.StagedValue = preStagedValue; usedPreresolved = true; }

        return (handle, usedPreresolved);
    }

#if OLD
    // Generic
    [Obsolete("Use GetReadWriteHandlePrestaged instead")]
    public static (IReadWriteHandle<TValue> handle, bool usedPreresolved) GetReadWriteHandlePreresolved<TValue>(this IReference reference, TValue preresolvedValue = default, bool overwriteValue = false)
    {
        bool usedPreresolved = false;

        var handle = HandleRegistry.GetOrAddReadWrite<IReadWriteHandle<TValue>>(reference?.Url ?? throw new ArgumentNullException(nameof(reference)),
            _ => reference.GetReadWriteHandleProvider().GetReadWriteHandle<TValue>(reference, preresolvedValue));

        if (!usedPreresolved && overwriteValue) { handle.ReadCacheValue = preresolvedValue; usedPreresolved = true; }

        return (handle, usedPreresolved);
    }
#endif
    public static (IReadWriteHandle<TValue> handle, bool usedPreresolved) GetReadWriteHandlePrestaged<TValue>(this IReference reference, TValue? preStagedValue = default, bool overwriteValue = false)
    {
        bool usedPreresolved = false;

        var handle = HandleRegistry.GetOrAddReadWrite<IReadWriteHandle<TValue>>(reference?.Url ?? throw new ArgumentNullException(nameof(reference)),
            _ => reference.GetReadWriteHandleProvider().GetReadWriteHandle<TValue>(reference, preStagedValue));

        if (!usedPreresolved && overwriteValue) { handle.StagedValue = preStagedValue; usedPreresolved = true; }

        return (handle, usedPreresolved);
    }

#if OLD
    // Non-generic
    public static (IReadWriteHandle handle, bool usedPreresolved) GetReadWriteHandlePreresolved(this IReference reference, Type type, object preresolvedValue, bool overwriteValue = false)
    {
        var result = (typeof(ReferenceToReadWriteHandleExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.Name == nameof(GetReadWriteHandlePrestaged) && mi.GetGenericArguments().Length == 1).First().MakeGenericMethod(type).Invoke(null, new object[] { reference, preresolvedValue, overwriteValue })); // TODO - also for ReadHandle if needed?

        throw new NotImplementedException("TODO: extract and return result");
        //return (result.handle, result.usedPreresolved);
    }
#endif

    // Non-generic
    public static (IReadWriteHandle handle, bool usedPreresolved) GetReadWriteHandlePrestaged(this IReference reference, Type type, object preStagedValue, bool overwriteValue = false)
    {
        var result = (typeof(ReferenceToReadWriteHandleExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.Name == nameof(GetReadWriteHandlePrestaged) && mi.GetGenericArguments().Length == 1).First().MakeGenericMethod(type).Invoke(null, new object[] { reference, preStagedValue, overwriteValue })); // TODO - also for ReadHandle if needed?

        throw new NotImplementedException("TODO: extract and return result");
        //return (result.handle, result.usedPreresolved);
    }

#endregion

    // Always create
    public static IReadWriteHandle<T> ToReadWriteHandle<T>(this IReference reference) => reference.GetReadWriteHandleProvider().GetReadWriteHandle<T>(reference)
        ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(IReadWriteHandle<T>)} type for reference of type {reference.GetType().FullName}");

    #region Existing

    public static IReadWriteHandle GetExistingReadWriteHandle(this IReferencable referencable)
    {
        if (referencable.Reference == null) { return null; }
        Type referenceValueType = null;

        {
            if (referencable is IReferencableValueType rvt) { referenceValueType = rvt.ReferenceValueType ?? throw new ArgumentNullException($"{typeof(IReferencableValueType).Name}.{nameof(rvt.ReferenceValueType)}"); }

            if (referencable.Reference is ITypedReference tr)
            {
                referenceValueType = tr.Type ?? throw new ArgumentNullException($"{typeof(ITypedReference).Name}.{nameof(tr.Type)}");
            }
        }

        if (referenceValueType == null) { throw new ArgumentException($"{nameof(referencable)} must implement IReferencableValueType, or its Reference must implement ITypedReference."); } // ENH: scan the object for IReadWriteHandle<TValue>

        if (referencable is IHasReadWriteHandle ihrwh)
        {
            var type = typeof(IHasReadWriteHandle<>).MakeGenericType(referenceValueType);
            if (type.IsAssignableFrom(referencable.GetType()))
            {
                return (IReadWriteHandle)type.GetProperty(nameof(IHasReadWriteHandle<object>.ReadWriteHandle)).GetValue(referencable);
            }
        }

        return (IReadWriteHandle)typeof(ReferenceToReadWriteHandleExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.Name == nameof(GetExistingReadWriteHandle) && mi.GetGenericArguments().Length == 1).First().MakeGenericMethod(referenceValueType).Invoke(null, new object[] { referencable.Reference });
    }

    public static IReadWriteHandle<TValue> GetExistingReadWriteHandle<TValue>(this IReference reference)
        => (IReadWriteHandle<TValue>)HandleRegistry.ReadWriteHandles.TryGetValue(reference?.Url ?? throw new ArgumentNullException(nameof(reference)));

    #endregion

}
