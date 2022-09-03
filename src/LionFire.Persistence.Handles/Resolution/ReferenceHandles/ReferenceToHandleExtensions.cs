﻿using LionFire.Persistence.Handles;
using LionFire.Persistence;
using System;
using LionFire.Referencing.Ex;
using LionFire.Dependencies;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using LionFire.ExtensionMethods;
using System.Collections.ObjectModel;

namespace LionFire.Referencing; // REVIEW - should be in another namespace?

public static partial class ReferenceToHandleExtensions
{
    // TODO: Review and make Read/ReadWrite/Write consistent

    #region ReadHandle

    public static IReadHandle<TValue> GetReadHandle<TValue, TReference>(this ITypedReference<TValue, TReference> reference)
        where TReference : IReference
        => HandleRegistry.GetOrAddRead<IReadHandle<TValue>>(reference.Reference.Url, _ => reference.Reference.TryGetReadHandleProvider().GetReadHandle<TValue>(reference.Reference));

    public static IReadHandle<TValue> GetReadHandle<TValue, TReference>(this TReference reference)
        where TReference : IReference
        => HandleRegistry.GetOrAddRead<IReadHandle<TValue>>(reference.Url, _ => reference.TryGetReadHandleProvider<TReference>().GetReadHandle<TValue>(reference));

    public static IReadHandle<TValue> GetReadHandle<TValue>(this IReference reference, TValue preresolvedValue = default, IServiceProvider serviceProvider = null)
    {
        return HandleRegistry.GetOrAddRead<IReadHandle<TValue>>(reference.Url, _ => reference.GetReadHandleProvider(serviceProvider).GetReadHandle<TValue>(reference, preresolvedValue));
#if Alternative
#else
        ////throw new NotImplementedException("FIXME - I don't think this even works");
        //// REVIEW - should this be using IReferenceToHandleService?
        //// REVIEW - this seems crazy.  Is it slow?  Should an [Obsolete] tell the user to use the <TValue, TReference> overload instead?
        var TReference = reference.GetType();
        return (IReadHandle<TValue>)(
            typeof(IReadHandleProvider<>).MakeGenericType(TReference)
                .GetMethod(nameof(ReferenceToHandleExtensions.GetReadHandle)).MakeGenericMethod(typeof(TValue))
                    .Invoke((typeof(ReferenceToHandleProviderExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(mi => mi.Name == nameof(IReferenceToHandleService.GetReadHandleProvider) && mi.ContainsGenericParameters).First()
                    .MakeGenericMethod(TReference)
                    .Invoke(null, new object[] { /* Upcast */ reference, default(TValue), serviceProvider }))
            , new object[] { reference }));
#endif
    }

    public static IReadHandle<TValue> GetReadHandle<TValue>(this IReference reference)
        => reference.GetReadHandleProvider().GetReadHandle<TValue>(reference)
        ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(IReadHandle<TValue>)} type for reference of type {reference.GetType().FullName}");

    public static IReadHandle<TValue> CreateReadHandle<TValue>(this IReference reference) => throw new NotImplementedException(); // FUTURE

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
        return (IReadHandle)typeof(ReferenceToHandleExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.Name == nameof(GetReadHandle) && mi.GetGenericArguments().Length == 1 & mi.GetParameters().Length == 1).Single().MakeGenericMethod(referenceValueType).Invoke(null, new object[] { referencable.Reference });

    }
    public static IReadHandle<T> GetExistingReadHandle<T>(this IReference reference)
        => (IReadHandle<T>)HandleRegistry.ReadHandles.TryGetValue(reference?.Url ?? throw new ArgumentNullException(nameof(reference)));

    #endregion

    #region Write Handle

    public static IWriteHandle<T> TryGetWriteHandle<T>(this IReference reference) => HandleRegistry.GetOrAddWrite<IWriteHandle<T>>(reference.Url, _ => reference.GetWriteHandleProvider().GetWriteHandle<T>(reference));

    public static IWriteHandle<T> GetWriteHandle<T>(this IReference reference, T prestagedValue = default, IServiceProvider serviceProvider = null) => HandleRegistry.GetOrAddWrite<IWriteHandle<T>>(reference.Url, _ => reference.GetWriteHandleProvider(serviceProvider).GetWriteHandle<T>(reference, prestagedValue: prestagedValue) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(Persistence.IWriteHandle<T>)} type for reference of type {reference.GetType().FullName}"));

    #endregion

    #region ReadWrite Handle

    #region Existing

    public static IReadWriteHandle GetExistingReadWriteHandle(this IReferencable referencable)
    {
        if(referencable.Reference == null) { return null; }
        Type referenceValueType = null;

        {
            if (referencable is IReferencableValueType rvt) { referenceValueType = rvt.ReferenceValueType ?? throw new ArgumentNullException($"{typeof(IReferencableValueType).Name}.{nameof(rvt.ReferenceValueType)}"); }

            if (referencable.Reference is ITypedReference tr)
            {
                referenceValueType = tr.Type ?? throw new ArgumentNullException($"{typeof(ITypedReference).Name}.{nameof(tr.Type)}");
            }
        }

        if (referenceValueType == null) { throw new ArgumentException($"{nameof(referencable)} must implement IReferencableValueType, or its Reference must implement ITypedReference."); } // ENH: scan the object for IReadWriteHandle<T>

        if (referencable is IHasReadWriteHandle ihrwh)
        {
            var type = typeof(IHasReadWriteHandle<>).MakeGenericType(referenceValueType);
            if (type.IsAssignableFrom(referencable.GetType()))
            {
                return (IReadWriteHandle)type.GetProperty(nameof(IHasReadWriteHandle<object>.ReadWriteHandle)).GetValue(referencable);
            }
        }

        return (IReadWriteHandle)typeof(ReferenceToHandleExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.Name == nameof(GetExistingReadWriteHandle) && mi.GetGenericArguments().Length == 1).First().MakeGenericMethod(referenceValueType).Invoke(null, new object[] { referencable.Reference });
    }

    public static IReadWriteHandle<T> GetExistingReadWriteHandle<T>(this IReference reference)
        => (IReadWriteHandle<T>)HandleRegistry.ReadWriteHandles.TryGetValue(reference?.Url ?? throw new ArgumentNullException(nameof(reference)));

    #endregion

    // Non-generic
    public static IReadWriteHandle GetReadWriteHandle(this IReference reference, Type type, object preresolvedValue = default)
        => (IReadWriteHandle)typeof(ReferenceToHandleExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.Name == nameof(GetReadWriteHandle) && mi.GetGenericArguments().Length == 1).First().MakeGenericMethod(type).Invoke(null, new object[] { reference, preresolvedValue }); // TODO - also for ReadHandle if needed?

    // Generic
    public static IReadWriteHandle<T> GetReadWriteHandle<T>(this IReference reference, T preresolvedValue = default)
        => HandleRegistry.GetOrAddReadWrite<IReadWriteHandle<T>>(reference?.Url ?? throw new ArgumentNullException(nameof(reference)),
            _ => reference.GetReadWriteHandleProvider().GetReadWriteHandle<T>(reference, preresolvedValue));

    // Generic, typed Reference
    public static IReadWriteHandle<T> GetReadWriteHandle<T, TReference>(this TReference reference, T preresolvedValue = default)
          where TReference : IReference
          => HandleRegistry.GetOrAddReadWrite<IReadWriteHandle<T>>(reference?.Url ?? throw new ArgumentNullException(nameof(reference)), _ => reference.TryGetReadWriteHandleProvider<TReference>().GetReadWriteHandle<T>(reference, preresolvedValue));

    // Always create
    public static IReadWriteHandle<T> ToReadWriteHandle<T>(this IReference reference) => reference.GetReadWriteHandleProvider().GetReadWriteHandle<T>(reference)
        ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(IReadWriteHandle<T>)} type for reference of type {reference.GetType().FullName}");

    #endregion

    #region Collections

    #region Listings

    public static IReadHandle<Metadata<IEnumerable<Listing<object>>>> GetListingsHandle(this IReference reference)
        => reference.GetReadHandle<Metadata<IEnumerable<Listing<object>>>>();
    public static IReadHandle<Metadata<IEnumerable<Listing<object>>>> GetListingsHandle(this IReferencable referencable)
        => referencable.Reference.GetReadHandle<Metadata<IEnumerable<Listing<object>>>>();
    public static IReadHandle<Metadata<IEnumerable<Listing<T>>>> GetListingsHandle<T>(this IReference reference)
              => reference.GetReadHandle<Metadata<IEnumerable<Listing<T>>>>();
    public static IReadHandle<Metadata<IEnumerable<Listing<T>>>> ReferenceGetListingsHandle<T>(this IReferencable referencable)
        => referencable.Reference.GetReadHandle<Metadata<IEnumerable<Listing<T>>>>();

    #endregion

    #region ReadHandles

    //public static IReadHandle<Metadata<IEnumerable<Listing<object>>>> GetListingsHandle(this IReference reference)
    //    => reference.GetReadHandle<Metadata<IEnumerable<Listing<object>>>>();
    //public static IReadHandle<Metadata<IEnumerable<Listing<object>>>> GetListingsHandle(this IReferencable referencable)
    //    => referencable.Reference.GetReadHandle<Metadata<IEnumerable<Listing<object>>>>();
    public static ListingValues<T> GetListingValues<T>(this IReference reference)
              => new ListingValues<T>(reference.GetListingsHandle<T>());
    //public static IReadHandle<Metadata<IEnumerable<Listing<T>>>> ReferenceGetListiandle<T>(this IReferencable referencable)
    //=> referencable.Reference.GetReadHandle<Metadata<IEnumerable<Listing<T>>>>();


    #endregion


    public static HC<T> GetCollectionHandle<T>(this IReference reference) => reference.GetCollectionHandleProvider().GetCollectionHandle<T>(reference);

    public static HC<T> ToCollectionHandle<T>(this IReference reference) => reference.ToCollectionHandleProvider().GetCollectionHandle<T>(reference) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(HC<T>)} type for reference of type {reference.GetType().FullName}");
    ////public static C<object> GetCollectionHandle<T>(this IReference reference) => reference.GetCollectionHandleProvider().GetCollectionHandle<object>(reference);

    public static RC<T> GetReadCollectionHandle<T>(this IReference reference) => reference.GetCollectionHandleProvider().GetReadCollectionHandle<T>(reference);

    public static RC<T> ToReadCollectionHandle<T>(this IReference reference) => reference.ToCollectionHandleProvider().GetReadCollectionHandle<T>(reference) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(RC<T>)} type for reference of type {reference.GetType().FullName}");

    #endregion
}
