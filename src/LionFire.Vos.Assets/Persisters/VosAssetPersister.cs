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

    public class VosAssetPersister : PassthroughPersister<IAssetReference, VosAssetOptions, IVosReference, VosPersister>, IPersister<IAssetReference>
    {
        VosAssetOptions Options { get; }
        public IVosReference AssetRoot { get; }

        public VosReference DefaultAssetRoot = "$assets";
        IPersisterProvider<VosReference> PersisterProvider { get; }
        #region Construction

        protected override VosPersister GetUnderlyingPersister => (VosPersister)PersisterProvider.GetPersister(AssetRoot.Persister);

        public VosAssetPersister(VosAssetOptions options, IPersisterProvider<VosReference> persisterProvider)
        {
            PersisterProvider = persisterProvider;
            Options = options;
            AssetRoot = Options.AssetRoot ?? DefaultAssetRoot;
            //UnderlyingPersister = (VosPersister)persisterProvider.GetPersister(AssetRoot.Persister);

            //var contextVob = options?.ContextVob?.ToVob() ?? "".ToVob();
            //contextVob.AddOwn<VosAssetPersister>(_ => this);
        }

        #endregion

        public string GetTypeKey(Type type) => type.Name;

        public override IVosReference TranslateReference(IAssetReference reference)
            => this.AssetRoot.ToVob()[GetTypeKey(reference.Type)][reference.Path].Reference;

    }
}
