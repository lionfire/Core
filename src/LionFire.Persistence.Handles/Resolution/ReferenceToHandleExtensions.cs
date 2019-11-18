using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Persistence;
using System;
using LionFire.Referencing.Ex;
using LionFire.Dependencies;

namespace LionFire.Referencing
{

    public static class ReferenceToHandleExtensions
    {

        #region ReadHandle

        public static IReadHandleBase<T> GetReadHandle<T>(this IReference reference) => reference.GetReadHandleProvider().GetReadHandle<T>(reference);
        public static IReadHandleBase<T> ToReadHandle<T>(this IReference reference) => reference.ToReadHandleProvider().GetReadHandle<T>(reference) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(IReadHandleBase<T>)} type for reference of type {reference.GetType().FullName}");

        #endregion

        #region Write Handle

        public static IWriteHandleBase<T> GetWriteHandle<T>(this IReference reference) => reference.ToWriteHandleProvider().GetWriteHandle<T>(reference);
        public static IWriteHandleBase<T> ToWriteHandle<T>(this IReference reference) => reference.ToWriteHandleProvider().GetWriteHandle<T>(reference) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(Persistence.IWriteHandleBase<T>)} type for reference of type {reference.GetType().FullName}");


        #endregion

        #region Handles

        public static IReadWriteHandleBase<T> GetReadWriteHandle<T>(this IReference reference) => reference.ToReadWriteHandleProvider().GetReadWriteHandle<T>(reference);
        public static IReadWriteHandleBase<T> ToReadWriteHandle<T>(this IReference reference) => reference.ToReadWriteHandleProvider().GetReadWriteHandle<T>(reference) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(IReadWriteHandleBase<T>)} type for reference of type {reference.GetType().FullName}");

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
