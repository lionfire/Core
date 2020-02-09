using LionFire.Persistence;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Assets;

namespace LionFire.Vos.Assets.Persisters
{
    public class VosAssetOptions : PersistenceOptions
    {
        public IVosReference AssetRoot { get; set; }
        //public IVosReference ContextVob { get; set; }
    }

    public class VosAssetPersister : PassThroughPersister<IAssetReference, VosAssetOptions, IVosReference, VosPersister>, IPersister<IAssetReference>
    {
        VosAssetOptions Options { get; }
        public IVob AssetRoot { get; }

        #region Construction

        public VosAssetPersister(VosAssetOptions options, IPersisterProvider<VosReference> persisterProvider)
        {
            Options = options;
            AssetRoot = Options.AssetRoot.ToVob();
            UnderlyingPersister = (VosPersister)persisterProvider.GetPersister(AssetRoot.Root.Name);

            //var contextVob = options?.ContextVob?.ToVob() ?? "".ToVob();
            //contextVob.AddOwn<VosAssetPersister>(_ => this);
        }

        #endregion

        public string GetTypeKey(Type type) => type.Name;

        public override IVosReference TranslateReference(IAssetReference reference)
            => this.AssetRoot[GetTypeKey(reference.Type)][reference.Path].Reference;

    }
}
