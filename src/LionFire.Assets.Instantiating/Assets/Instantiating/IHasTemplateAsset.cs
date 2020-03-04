using LionFire.Persistence;

namespace LionFire.Assets
{
    public interface IHasTemplateAsset
    {
        IReadHandle<ITemplateAsset> TemplateAsset { get; set; }
    }
}
