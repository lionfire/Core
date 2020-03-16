
namespace LionFire.Instantiating
{
    //public abstract class TemplateParameters : ITemplateParameters
    //{
    //    //public abstract AssetReference<ITemplate> Template { get; set; }
    //}

    public interface IParentedTemplateParameters<TTemplate> : ITemplateParameters<TTemplate>, IParentedTemplateParameters
    where TTemplate : ITemplate
    { }

    public interface IParentedTemplateParameters : ITemplateParameters
    {
        string ParentKey { get; }
        //object State { get; set; }
    }

    //public abstract class TemplateParameters : ITemplateParameters
    //{
    //    //public abstract AssetReference<ITemplate> Template { get; set; }
    //}
}
