using LionFire.Instantiating;

namespace LionFire.Assets.Instantiating
{
    public abstract class TemplateAssetBase<TTemplate, TInstance, TParameters> : AssetBase<TTemplate>, ITemplateAsset<TTemplate, TInstance>
        where TTemplate : ITemplateAsset<TTemplate, TInstance>
        where TParameters : ITemplateParameters<TTemplate, TInstance>, ITemplateParameters
    {
        //private RWAsset<TTemplate> asset;

        public TemplateAssetBase() { }
        public TemplateAssetBase(AssetReference<TTemplate> reference) : base(reference) { }

#if IRWAssetAware
        //public TemplateAssetBase(RWAsset<TTemplate> asset)
        //{
        //    this.asset = asset;
        //}
#endif
    }

#if true
    public class TemplateAssetBase<TTemplate, TInstance> : AssetBase<TTemplate>, ITemplate<TInstance>, ITemplateAsset<TTemplate, TInstance>
        where TTemplate : ITemplateAsset<TTemplate, TInstance>
    {
        public TemplateAssetBase() { }
        public TemplateAssetBase(AssetReference<TTemplate> reference) : base(reference) { }

    }
#else // Another idea:
    public class TemplateAssetBase<TTemplate, TInstance> : TemplateAssetBase<TTemplate, TInstance, Instantiation<TTransferCapability, TransferCapability>>
    {
    }
#endif


}
