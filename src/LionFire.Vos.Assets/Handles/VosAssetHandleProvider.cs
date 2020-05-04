using LionFire.Assets;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Vos.Assets.Persisters;
using LionFire.Vos.Collections.ByType;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Assets.Handles
{
    public class VosAssetHandleProvider : IReadHandleProvider<IAssetReference>, IReadWriteHandleProvider<IAssetReference>
    {
        IPersisterProvider<IAssetReference> PersisterProvider { get; }
        public VosAssetHandleProvider(IPersisterProvider<IAssetReference> persisterProvider)
        {
            PersisterProvider = persisterProvider;
        }

        public IReadHandle<T> GetReadHandle<T>(IAssetReference reference, T preresolvedValue = default)
        {
            var persister = (VosAssetPersister)PersisterProvider.GetPersister(reference.Persister);
            if (persister == null) throw new NotFoundException($"Could not find AssetPersister for '{reference}'");

            return new PersisterReadHandle<IAssetReference, T, VosAssetPersister>(persister, reference, preresolvedValue);
        }
        public IReadHandle<T> GetReadHandle<T>(IReference reference, T preresolvedValue = default)
            => reference is IAssetReference iar ? GetReadHandle<T>(iar, preresolvedValue) : null;

        public Persistence.IReadWriteHandle<T> GetReadWriteHandle<T>(IAssetReference reference, T preresolvedValue = default)
            => new PersisterReadWriteHandle<IAssetReference, T, VosAssetPersister>((VosAssetPersister)PersisterProvider.GetPersister(reference.Persister), reference, preresolvedValue);
    }

    // UNUSED
    //public class AssetCollectionTypeProvider : ICollectionTypeProvider<VosReference>
    //{
    //    public Type GetCollectionType(VosReference reference)
    //    {
    //        var vob = reference.ToVob();
    //        var type = vob.Parent.AcquireOwn<CollectionsByTypeManager>()?.GetCollectionType(vob);
    //        return type;
    //    }
    //}
}
