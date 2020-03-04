using LionFire.Assets;

namespace LionFire.Assets
{
    // PORTINGGUIDE - was ITemplateParametersInstantiatable
    public interface ITemplateParametersInstantiatableAsset
    {
        ITemplateAssetInstance Instantiate();
    }
}
