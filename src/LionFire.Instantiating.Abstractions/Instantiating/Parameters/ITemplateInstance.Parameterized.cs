namespace LionFire.Instantiating
{
    public interface ITemplateInstance<TemplateType, ParametersType> : ITemplateInstance<TemplateType>, IParameterizedTemplateInstance
        where TemplateType : class, ITemplate
        where ParametersType : ITemplateParameters
    {
    }
    
}
