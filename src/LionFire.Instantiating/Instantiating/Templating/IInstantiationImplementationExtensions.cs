#nullable enable
using System;

namespace LionFire.Instantiating
{
    public static class IInstantiationImplementationExtensions
    {
        public static TInstance Create<TInstance>(this IInstantiation template) => template.Parameters.ToInstantiator().Instantiate<TInstance>();

        public static object Create(this IInstantiation template, Type instanceType = null)
        {
            if (template.Template == null) throw new ArgumentNullException($"Missing template.  Reference: {template.RTemplate?.Reference?.ToString() ?? "null"}");
            var result = template.Template.Create(instanceType);

            if (result  is IParameterizedTemplateInstance pti)
            {
                pti.Parameters = (template as ITemplateParameters) ?? template.Parameters;
            }

            // REVIEW - Where did this code go?  //throw new NotImplementedException("Does this implementation exist somewhere?");  
            template.InitializationMethod?.Invoke(result);

            return result;
        }
    }
}