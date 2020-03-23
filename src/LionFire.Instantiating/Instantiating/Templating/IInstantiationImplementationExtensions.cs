using System;

namespace LionFire.Instantiating
{
    public static class IInstantiationImplementationExtensions
    {
        public static TInstance Create<TInstance>(this IInstantiation template) => template.Parameters.ToInstantiator().Instantiate<TInstance>();

        public static object Create(this IInstantiation template, Type instanceType = null)
        {
            var result = template.Template.Create(instanceType);
            
            //var hasTemplateParameters  = result as ITemplateParameters

            throw new NotImplementedException("Does this implementation exist somewhere?");

            return result;
        }
    }

}