using LionFire.Instantiating;

namespace LionFire.Assets.Instantiating
{
    public abstract class TemplateAssetBase<TTemplate, TInstance, TParameters> : AssetBase<TTemplate>, ITemplate<TInstance>
        where TTemplate : ITemplate<TInstance>
        where TParameters : ITemplateParameters<TTemplate, TInstance>, ITemplateParameters
    {
        //private RWAsset<TTemplate> asset;

        //public TemplateAssetBase() { }

#if IRWAssetAware
        //public TemplateAssetBase(RWAsset<TTemplate> asset)
        //{
        //    this.asset = asset;
        //}
#endif
    }

    public class TemplateAssetBase<TTemplate, TInstance> : ITemplate<TInstance>
        where TTemplate : ITemplate<TInstance>
    {
    }

}
