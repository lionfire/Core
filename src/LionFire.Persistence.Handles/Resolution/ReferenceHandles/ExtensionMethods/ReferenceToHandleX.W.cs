using LionFire.Persistence;
using System;
using LionFire.Referencing.Ex;
using LionFire.Dependencies;

namespace LionFire.Referencing; // REVIEW - should be in another namespace?

public static partial class ReferenceToWriteHandleExtensions
{

    public static IWriteHandle<T> TryGetWriteHandle<T>(this IReference reference, IServiceProvider serviceProvider = null) => HandleRegistry.GetOrAddWrite<IWriteHandle<T>>(reference.Url, _ => reference.GetWriteHandleProvider(serviceProvider).GetWriteHandle<T>(reference));

    public static IWriteHandle<T> GetWriteHandle<T>(this IReference reference, T prestagedValue = default, IServiceProvider serviceProvider = null) => HandleRegistry.GetOrAddWrite<IWriteHandle<T>>(reference.Url, _ => reference.GetWriteHandleProvider(serviceProvider).GetWriteHandle<T>(reference, prestagedValue: prestagedValue) ?? throw new HasUnresolvedDependenciesException($"Could not get {nameof(Persistence.IWriteHandle<T>)} type for reference of type {reference.GetType().FullName}"));

}
