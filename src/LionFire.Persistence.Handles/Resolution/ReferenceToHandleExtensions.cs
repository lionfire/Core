using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Persistence;
using System;
using LionFire.Referencing.Ex;
using LionFire.DependencyInjection;

namespace LionFire.Referencing
{

    public static class ReferenceToHandleExtensions
    {

        #region ReadHandle

        public static RH<T> GetReadHandle<T>(this IReference reference) => reference.GetReadHandleProvider().GetReadHandle<T>(reference);
        public static RH<T> ToReadHandle<T>(this IReference reference) => reference.ToReadHandleProvider().GetReadHandle<T>(reference) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(RH<T>)} type for reference of type {reference.GetType().FullName}");

        #endregion

        #region Handles

        public static H<T> GetHandle<T>(this IReference reference) => reference.ToHandleProvider().GetHandle<T>(reference);
        public static H<T> ToHandle<T>(this IReference reference) => reference.ToHandleProvider().GetHandle<T>(reference) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(H<T>)} type for reference of type {reference.GetType().FullName}");

        #endregion

        #region Collections

        public static HC<T> GetCollectionHandle<T>(this IReference reference) => reference.GetCollectionHandleProvider().GetCollectionHandle<T>(reference);
        public static HC<T> ToCollectionHandle<T>(this IReference reference) => reference.ToCollectionHandleProvider().GetCollectionHandle<T>(reference) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(HC<T>)} type for reference of type {reference.GetType().FullName}");
        ////public static C<object> GetCollectionHandle<T>(this IReference reference) => reference.GetCollectionHandleProvider().GetCollectionHandle<object>(reference);

        public static RC<T> GetReadCollectionHandle<T>(this IReference reference) => reference.GetCollectionHandleProvider().GetReadCollectionHandle<T>(reference);
        public static RC<T> ToReadCollectionHandle<T>(this IReference reference) => reference.ToCollectionHandleProvider().GetReadCollectionHandle<T>(reference) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(RC<T>)} type for reference of type {reference.GetType().FullName}");

        #endregion
    }
}
