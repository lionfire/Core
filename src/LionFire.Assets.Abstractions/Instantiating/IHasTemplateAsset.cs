
using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.Assets
{
    public interface IHasTemplateAsset
    {
        //ITemplateAsset TemplateAsset { get; set; }
        IReadHandleBase
#if !AOT && !UNITY // Unity crashes with contravariant IReadHandle
        <ITemplateAsset>
#endif
 TemplateAsset { get; set; }
    }
}
