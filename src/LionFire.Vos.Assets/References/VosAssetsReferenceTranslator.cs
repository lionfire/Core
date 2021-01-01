using LionFire.Assets;
using LionFire.Persistence.Persisters;
using LionFire.Vos;
using LionFire.Vos.Assets.Persisters;

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
            VosAssetPersisterProvider = vosAssetPersisterProvider;
        }

        #endregion

        #region IReferenceTranslator

        public IAssetReference ReverseTranslateReference(IVobReference reference) => VosAssetPersisterProvider.GetPersister(reference.RootName()).ReverseTranslateReference(reference);

        public IVobReference TranslateReferenceForRead(IAssetReference reference) => VosAssetPersisterProvider.GetPersister(reference.Persister).TranslateReferenceForRead(reference);
        public IVobReference TranslateReferenceForWrite(IAssetReference reference) => VosAssetPersisterProvider.GetPersister(reference.Persister).TranslateReferenceForWrite(reference);

        #endregion
    }
}
