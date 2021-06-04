using LionFire.Assets;
using LionFire.Persistence.Persisters;
using LionFire.Vos;
using LionFire.Vos.Assets.Persisters;
using System;

namespace LionFire.Hosting
{
    public class VosAssetsReferenceTranslator : IReferenceTranslator<IAssetReference, IVobReference>
    {
        #region Dependencies

        public VosAssetPersisterProvider VosAssetPersisterProvider { get; }

        #endregion

        #region Construction

        public VosAssetsReferenceTranslator(VosAssetPersisterProvider vosAssetPersisterProvider)
        {
            VosAssetPersisterProvider = vosAssetPersisterProvider ?? throw new ArgumentNullException(nameof(vosAssetPersisterProvider));
        }

        #endregion

        #region IReferenceTranslator

        public IAssetReference ReverseTranslateReference(IVobReference reference) => (VosAssetPersisterProvider.GetPersister(reference.RootName()) ?? MissingPersister(reference)).ReverseTranslateReference(reference);

        public IVobReference TranslateReferenceForRead(IAssetReference reference) => (VosAssetPersisterProvider.GetPersister(reference.Persister) ?? MissingPersister(reference)).TranslateReferenceForRead(reference);
        public IVobReference TranslateReferenceForWrite(IAssetReference reference) => (VosAssetPersisterProvider.GetPersister(reference.Persister) ?? MissingPersister(reference)).TranslateReferenceForWrite(reference);

        #endregion

        public VosAssetPersister MissingPersister(object r) => throw new System.Exception($"{this.GetType().FullName} did not find Persister for {r} of type {r.GetType().Name}");
    }
}
