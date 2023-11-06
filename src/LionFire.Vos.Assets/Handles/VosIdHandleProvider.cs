#nullable enable
using LionFire.Data.Id;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Vos.Collections.ByType;
using LionFire.Vos.Id.Persisters;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Id.Handles;

// FUTURE: per-HandleProvider Configuration
//public enum HandleRegistryType
//{
//    None = 0,
//    Single = 1,
//    Multi = 2,
//}

//public class HandleProviderOptions
//{
//    public HandleRegistryType HandleRegistryType { get; set; } = HandleRegistryType.Single;
//}


public class VosIdHandleProvider
    : IReadHandleProvider<IIdReference>
    , IReadHandleProvider
    , IReadHandleCreator<IIdReference>
    , IReadHandleCreator

    , IReadWriteHandleProvider<IIdReference>
    , IReadWriteHandleProvider
{

    #region Dependencies

    IPersisterProvider<IIdReference> PersisterProvider { get; }

    #endregion

    #region Lifecycle

    public VosIdHandleProvider(IPersisterProvider<IIdReference> persisterProvider)
    {
        PersisterProvider = persisterProvider;
    }

    #endregion

    #region Read

    #region Provider

    public IReadHandle<T>? GetReadHandle<T>(IReference reference)
        => reference is IIdReference compatible ? GetReadHandle<T>(compatible) : null;

    public IReadHandle<T> GetReadHandle<T>(IIdReference reference) => CreateReadHandle<T>(reference);

    #endregion

    #region Creator

    public IReadHandle<T>? CreateReadHandle<T>(IReference reference, T? preresolvedValue = default)
        => reference is IIdReference compatible ? CreateReadHandle<T>(compatible, preresolvedValue) : null;

    public IReadHandle<T> CreateReadHandle<T>(IIdReference reference, T? preresolvedValue = default)
    {
        var persister = (VosIdPersister)PersisterProvider.GetPersister((reference as IPersisterReference)?.Persister);
        if (persister == null) throw new NotFoundException($"Could not find IdedPersister for '{reference}'");

        return new PersisterReadHandle<IIdReference, T, VosIdPersister>(persister, reference.ForType<T>(), preresolvedValue);
    }

    #endregion

    #endregion

    #region ReadWrite

    public Persistence.IReadWriteHandle<T> GetReadWriteHandle<T>(IIdReference reference, T preresolvedValue = default)
        => new PersisterReadWriteHandle<IIdReference, T, VosIdPersister>((VosIdPersister)PersisterProvider.GetPersister((reference as IPersisterReference)?.Persister), reference.ForType<T>(), preresolvedValue);

    public IReadWriteHandle<T> GetReadWriteHandle<T>(IReference reference, T preresolvedValue = default)
        => GetReadWriteHandle((IIdReference)reference, preresolvedValue);

    #endregion

    #region Write

    #endregion

}

// UNUSED
//public class IdedCollectionTypeProvider : ICollectionTypeProvider<VobReference>
//{
//    public Type GetCollectionType(VobReference reference)
//    {
//        var vob = reference.ToVob();
//        var type = vob.Parent.AcquireOwn<CollectionsByTypeManager>()?.GetCollectionType(vob);
//        return type;
//    }
//}
