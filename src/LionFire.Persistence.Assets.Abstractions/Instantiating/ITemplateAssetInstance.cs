using LionFire.Instantiating;

    // TODO MAYBE RENAME namespace to LionFire.Persistence.Assets
namespace LionFire.Assets
{
    public interface ITemplateAssetInstance : ITemplateInstance
    {
        ITemplateAsset TemplateAsset { get; set; }
    }
}
