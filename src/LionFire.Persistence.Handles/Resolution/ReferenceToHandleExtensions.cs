using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Persistence;
using System;
using LionFire.Referencing.Ex;
using LionFire.Dependencies;
using System.Reflection;
using System.Linq;

namespace LionFire.Referencing
{

    public static class ReferenceToHandleExtensions
    {

        #region ReadHandle

        public static IReadHandle<TValue> GetReadHandle<TValue, TReference>(this ITypedReference<TValue, TReference> reference)
            where TReference : IReference
            => reference.Reference.GetReadHandleProvider().GetReadHandle<TValue>(reference.Reference);

        public static IReadHandle<TValue> GetReadHandle<TValue, TReference>(this TReference reference)
            where TReference : IReference
            => reference.GetReadHandleProvider<TReference>().GetReadHandle<TValue>(reference);

        public static IReadHandle<TValue> GetReadHandle<TValue>(this IReference reference, IServiceProvider serviceProvider = null)
        {
#if Alternative
            return reference.GetReadHandleProvider(serviceProvider).GetReadHandle<TValue>(reference)
#else
            //throw new NotImplementedException("FIXME - I don't think this even works");
            // REVIEW - should this be using IReferenceToHandleService?
            // REVIEW - this seems crazy.  Is it slow?  Should an [Obsolete] tell the user to use the <TValue, TReference> overload instead?
            var TReference = reference.GetType();
            return (IReadHandle<TValue>)(
                typeof(IReadHandleProvider<>).MakeGenericType(TReference)
                    .GetMethod("GetReadHandle").MakeGenericMethod(typeof(TValue))
                        .Invoke((typeof(ReferenceToHandleProviderExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .Where(mi => mi.Name == "GetReadHandleProvider" && mi.ContainsGenericParameters).First()
                        .MakeGenericMethod(TReference)
                        .Invoke(null, new object[] { /* Upcast */ reference, serviceProvider }))
                , new object[] { reference }));
#endif
        }

        public static IReadHandle<TValue> GetReadHandle<TValue>(this IReference reference)
            => reference.GetReadHandleProvider().GetReadHandle<TValue>(reference)
            ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(IReadHandle<TValue>)} type for reference of type {reference.GetType().FullName}");

        public static IReadHandle<TValue> CreateReadHandle<TValue>(this IReference reference) => throw new NotImplementedException(); // FUTURE

        #endregion

        #region Write Handle

        public static IWriteHandle<T> GetWriteHandle<T>(this IReference reference) => reference.ToWriteHandleProvider().GetWriteHandle<T>(reference);

        public static IWriteHandle<T> ToWriteHandle<T>(this IReference reference) => reference.ToWriteHandleProvider().GetWriteHandle<T>(reference) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(Persistence.IWriteHandle<T>)} type for reference of type {reference.GetType().FullName}");

        #endregion

        #region Handles

        public static IReadWriteHandle<T> TryGetReadWriteHandle<T>(this IReference reference) => reference.ToReadWriteHandleProvider().GetReadWriteHandle<T>(reference);

        public static IReadWriteHandle<T> ToReadWriteHandle<T>(this IReference reference) => reference.ToReadWriteHandleProvider().GetReadWriteHandle<T>(reference)
            ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(IReadWriteHandle<T>)} type for reference of type {reference.GetType().FullName}");

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
