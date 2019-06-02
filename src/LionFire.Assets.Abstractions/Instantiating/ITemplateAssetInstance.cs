using LionFire.Instantiating;

namespace LionFire.Assets
{
    public interface ITemplateAssetInstance : ITemplateInstance
    {
        ITemplateAsset TemplateAsset { get; set; }
    }
}
