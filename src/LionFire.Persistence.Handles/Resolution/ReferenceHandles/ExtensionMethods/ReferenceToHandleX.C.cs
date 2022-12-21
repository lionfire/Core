﻿using LionFire.Persistence.Handles;
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

    public static IReadHandle<Metadata<IEnumerable<Listing<object>>>> GetListingsHandle(this IReference reference)
        => reference.GetReadHandle<Metadata<IEnumerable<Listing<object>>>>();
    //public static IReadHandle<Metadata<IEnumerable<Listing<object>>>> GetListingsHandle(this IReferencable referencable)
        //=> referencable.Reference.GetReadHandle<Metadata<IEnumerable<Listing<object>>>>();
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

    #region Collections

    public static HC<T> GetCollectionHandle<T>(this IReference reference) => reference.GetCollectionHandleProvider().GetCollectionHandle<T>(reference);

    public static HC<T> ToCollectionHandle<T>(this IReference reference) => reference.ToCollectionHandleProvider().GetCollectionHandle<T>(reference) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(HC<T>)} type for reference of type {reference.GetType().FullName}");
    ////public static C<object> GetCollectionHandle<T>(this IReference reference) => reference.GetCollectionHandleProvider().GetCollectionHandle<object>(reference);

    public static RC<T> GetReadCollectionHandle<T>(this IReference reference) => reference.GetCollectionHandleProvider().GetReadCollectionHandle<T>(reference);

    public static RC<T> ToReadCollectionHandle<T>(this IReference reference) => reference.ToCollectionHandleProvider().GetReadCollectionHandle<T>(reference) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(RC<T>)} type for reference of type {reference.GetType().FullName}");

    #endregion
}
