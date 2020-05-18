using LionFire.Persistence;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Assets;
using LionFire.Serialization;

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

        public VosAssetPersister(VosAssetOptions options, IPersisterProvider<VosReference> persisterProvider, SerializationOptions serializationOptions) : base(options?.SerializationOptions ?? serializationOptions)
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

        public override IVosReference TranslateReferenceForRead(IAssetReference reference)
            => this.AssetRoot.GetVob()[GetTypeKey(reference.Type)][reference.Path].Reference;

        public override IVosReference TranslateReferenceForWrite(IAssetReference reference)
            => AssetWriteContext.Current?.WriteLocation[GetTypeKey(reference.Type)][reference.Path] 
            ?? this.AssetRoot.GetVob()[GetTypeKey(reference.Type)][reference.Path].Reference;


    }
}
