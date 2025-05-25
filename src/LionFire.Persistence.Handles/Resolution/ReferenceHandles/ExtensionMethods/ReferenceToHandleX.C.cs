using LionFire.Persistence.Handles;
using LionFire.Persistence;
using LionFire.Referencing.Ex;
using LionFire.Dependencies;
using System.Collections.Generic;
using LionFire.ExtensionMethods;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;

namespace LionFire.Referencing; // REVIEW - should be in another namespace?

//public static partial class ReferenceToCollectionHandleExtensions { }

public static partial class ReferenceToCollectionHandleExtensions
{

    #region Listings

    public static IReadHandle<Metadata<IEnumerable<IListing<object>>>> GetListingsHandle(this IReference reference)
        => reference.GetReadHandle<Metadata<IEnumerable<IListing<object>>>>();
    //public static IReadHandle<Metadata<IEnumerable<Listing<object>>>> GetListingsHandle(this IReferenceable referencable)
        //=> referencable.Reference.GetReadHandle<Metadata<IEnumerable<Listing<object>>>>();
    public static IReadHandle<Metadata<IEnumerable<IListing<T>>>> GetListingsHandle<T>(this IReference reference)
              => reference.GetReadHandle<Metadata<IEnumerable<IListing<T>>>>();
    public static IReadHandle<Metadata<IEnumerable<IListing<T>>>> ReferenceGetListingsHandle<T>(this IReferenceable referencable)
        => referencable.Reference.GetReadHandle<Metadata<IEnumerable<IListing<T>>>>();

    #endregion

    #region ReadHandles

    //public static IReadHandle<Metadata<IEnumerable<Listing<object>>>> GetListingsHandle(this IReference reference)
    //    => reference.GetReadHandle<Metadata<IEnumerable<Listing<object>>>>();
    //public static IReadHandle<Metadata<IEnumerable<Listing<object>>>> GetListingsHandle(this IReferenceable referencable)
    //    => referencable.Reference.GetReadHandle<Metadata<IEnumerable<Listing<object>>>>();
    public static ListingValues<T> GetListingValues<T>(this IReference reference)
              => new ListingValues<T>(reference.GetListingsHandle<T>());
    //public static IReadHandle<Metadata<IEnumerable<IListing<TValue>>>> ReferenceGetListiandle<TValue>(this IReferenceable referencable)
    //=> referencable.Reference.GetReadHandle<Metadata<IEnumerable<IListing<TValue>>>>();


    #endregion

    #region Collections

    public static HC<T> GetCollectionHandle<T>(this IReference reference) => reference.GetCollectionHandleProvider().GetCollectionHandle<T>(reference);

    public static HC<T> ToCollectionHandle<T>(this IReference reference) => reference.ToCollectionHandleProvider().GetCollectionHandle<T>(reference) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(HC<T>)} type for reference of type {reference.GetType().FullName}");
    ////public static C<object> GetCollectionHandle<TValue>(this IReference reference) => reference.GetCollectionHandleProvider().GetCollectionHandle<object>(reference);

    public static RC<T> GetReadCollectionHandle<T>(this IReference reference) => reference.GetCollectionHandleProvider().GetReadCollectionHandle<T>(reference);

    public static RC<T> ToReadCollectionHandle<T>(this IReference reference) => reference.ToCollectionHandleProvider().GetReadCollectionHandle<T>(reference) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(RC<T>)} type for reference of type {reference.GetType().FullName}");

    #endregion
}
