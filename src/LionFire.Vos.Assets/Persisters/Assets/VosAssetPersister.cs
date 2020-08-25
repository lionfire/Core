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
        /// <summary>
        /// Location of default AssetRoot used to resolve AssetReferences when the Persister property is null.
        /// Defaults to "$assets" if unset.
        /// </summary>
        public IVobReference DefaultAssetRoot { get; set; } = DefaultAssetRootDefault;
        //public IVobReference ContextVob { get; set; }

        public static readonly IVobReference DefaultAssetRootDefault = new VobReference("$assets");

    }

    public class VosAssetPersister : PassthroughPersister<IAssetReference, VosAssetOptions, IVobReference, VosPersister>, IPersister<IAssetReference>
    {
        #region Dependencies

        VosAssetOptions Options { get; }
        IPersisterProvider<VobReference> VosPersisterProvider { get; }

        #endregion

        #region Cache

        public IVobReference DefaultAssetRoot { get; }
        
        #endregion

        #region Construction


        public VosAssetPersister(VosAssetOptions options, IPersisterProvider<VobReference> persisterProvider, SerializationOptions serializationOptions) : base(options?.SerializationOptions ?? serializationOptions)
        {
            VosPersisterProvider = persisterProvider;
            Options = options;
            DefaultAssetRoot = Options.DefaultAssetRoot;
            //UnderlyingPersister = (VosPersister)persisterProvider.GetPersister(AssetRoot.Persister);

            //var contextVob = options?.ContextVob?.ToVob() ?? "".ToVob();
            //contextVob.AddOwn<VosAssetPersister>(_ => this);
        }

        #endregion

        //protected override VosPersister GetUnderlyingPersister => (VosPersister)VosPersisterProvider.GetPersister(DefaultAssetRoot.Persister);
        protected override VosPersister GetUnderlyingPersister(IAssetReference reference) => (VosPersister)VosPersisterProvider.GetPersister(GetAssetRootForAssetChannel(reference.Channel).Persister);

        public string GetTypeKey(Type type) => AssetPaths.GetTypeKey(type);

        private IVobReference GetAssetRootForAssetChannel(string assetChannel) 
            => assetChannel == null ? DefaultAssetRoot : new VobReference(assetChannel);

        public override IVobReference TranslateReferenceForRead(IAssetReference reference)
            => GetAssetRootForAssetChannel(reference.Channel).GetVob()[GetTypeKey(reference.Type)][reference.Path].Reference;

        public override IVobReference TranslateReferenceForWrite(IAssetReference reference)
        {
            IVobReference result = null;
            if (reference.Channel == null)
            {
                result = AssetWriteContext.Current?.WriteLocation[GetTypeKey(reference.Type)][reference.Path];
            }
            if (result == null)
            {
                result = GetAssetRootForAssetChannel(reference.Channel).GetVob()[GetTypeKey(reference.Type)][reference.Path].Reference;
            }
            return result;
        }


    }
}
