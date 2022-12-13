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

namespace LionFire.Vos.Id.Handles
{
    public class VosIdHandleProvider : IReadHandleProvider<IIdReference>, IReadWriteHandleProvider<IIdReference>
        , IReadHandleProvider
        , IReadWriteHandleProvider
    {
        IPersisterProvider<IIdReference> PersisterProvider { get; }
        public VosIdHandleProvider(IPersisterProvider<IIdReference> persisterProvider)
        {
            PersisterProvider = persisterProvider;
        }

        public IReadHandle<T> GetReadHandle<T>(IIdReference reference)
        {
            var persister = (VosIdPersister)PersisterProvider.GetPersister((reference as IPersisterReference)?.Persister);
            if (persister == null) throw new NotFoundException($"Could not find IdedPersister for '{reference}'");

            return new PersisterReadHandle<IIdReference, T, VosIdPersister>(persister, reference.ForType<T>());
        }
        public IReadHandle<T> GetReadHandle<T>(IReference reference)
            => reference is IIdReference iar ? GetReadHandle<T>(iar) : null;

        public Persistence.IReadWriteHandle<T> GetReadWriteHandle<T>(IIdReference reference, T preresolvedValue = default)
            => new PersisterReadWriteHandle<IIdReference, T, VosIdPersister>((VosIdPersister)PersisterProvider.GetPersister((reference as IPersisterReference)?.Persister), reference.ForType<T>(), preresolvedValue);

        public IReadWriteHandle<T> GetReadWriteHandle<T>(IReference reference, T preresolvedValue = default)
            => GetReadWriteHandle((IIdReference)reference, preresolvedValue);
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
}
