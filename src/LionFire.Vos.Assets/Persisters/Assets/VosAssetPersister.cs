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
using System.Diagnostics;

namespace LionFire.Vos.Assets.Persisters
{
    /// <summary>
    /// Options for composing IServiceCollection via AddAssets()
    /// </summary>
    public class VosAssetOptions : PersistenceOptions
    {
        #region (Static) Defaults

        public static VosAssetOptions Default => new VosAssetOptions();

        // Default: $assets points to /assets

        public static readonly string DefaultAssetsRootEnvironmentVariable = "assets"; // $assets
        public static readonly IVobReference DefaultAssetsRootEnvironmentVariableValue = new VobReference("/assets");
        public static readonly string DefaultPersisterLocation = "/";

        #endregion

        //public VosAssetOptions()
        //{
        //    Debug.WriteLine($"VAO: ${AssetsRootEnvironmentVariable} = {AssetsRootEnvironmentVariableValue}");
        //}

        /// <summary>
        /// Location of default AssetRoot used to resolve AssetReferences when the Persister property is null.
        /// </summary>
        public string AssetsRootEnvironmentVariable { get; set; } = DefaultAssetsRootEnvironmentVariable;

        public IVobReference AssetsRootEnvironmentVariableReference => new VobReference($"${AssetsRootEnvironmentVariable}");

        public IVobReference AssetsRootEnvironmentVariableValue { get; set; } = DefaultAssetsRootEnvironmentVariableValue;

        /// <summary>
        /// Should typically be a root.
        /// </summary>
        public string PersisterLocation { get; set; } = DefaultPersisterLocation;
    }

    public class VosAssetPersisterOptions : PersistenceOptions
    {
        //public bool UseAssetWriteContext { get; set; } = true;
    }

    // Default assetChannel's root: $assets
    public class VosAssetPersister : PassthroughPersister<IAssetReference, VosAssetPersisterOptions, IVobReference, VosPersister>, IPersister<IAssetReference>
    {
        #region Dependencies

        VosAssetOptions Options { get; }
        IPersisterProvider<IVobReference> VosPersisterProvider { get; }

        #endregion

        #region Cache

        public IVobReference DefaultAssetRoot => Options.AssetsRootEnvironmentVariableReference;
        
        #endregion

        #region Construction

        public VosAssetPersister(VosAssetOptions options, IPersisterProvider<IVobReference> persisterProvider, SerializationOptions serializationOptions) : base(options?.SerializationOptions ?? serializationOptions)
        {
            VosPersisterProvider = persisterProvider;
            Options = options ?? VosAssetOptions.Default;
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

        /// <summary>
        /// If reference.Channel is null, will try AssetWriteContext.Current?.WriteLocation (REVIEW)
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
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
