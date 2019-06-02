
using LionFire.Referencing;

namespace LionFire.Assets
{
    public interface IHasTemplateAsset
    {
        //ITemplateAsset TemplateAsset { get; set; }
        RH
#if !AOT && !UNITY // Unity crashes with contravariant IReadHandle
        <ITemplateAsset>
#endif
 TemplateAsset { get; set; }
    }
}
