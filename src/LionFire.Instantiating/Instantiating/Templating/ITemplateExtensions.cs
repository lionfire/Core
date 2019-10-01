using LionFire.Validation;
using System;
using System.Linq;

namespace LionFire.Instantiating
{
    public static class ITemplateExtensions
    {
        public static TInstance Create<TInstance>(this ITemplate<TInstance> template)
            where TInstance : new()
        {
            var validates = template as IValidatesCreate;
            if (validates != null)
            {
                validates.Validate().ValidateCreate().EnsureValid();
            }
            var instance = Create((ITemplate)template, typeof(TInstance));
            return (TInstance) instance;
        }

        public static object Create(this ITemplate template, Type instanceType = null)
        {
            if (instanceType == null)
            {
                var interfaceType = template.GetType().GetInterfaces().Where(t => t.Name == typeof(ITemplate).Name + "`1").FirstOrDefault();
                instanceType = interfaceType.GenericTypeArguments[0];
            }
            var inst = Activator.CreateInstance(instanceType);

            var templateInstance = inst as ITemplateInstance;
            if (templateInstance != null)
            {
                templateInstance.SetTemplate(template);
            }

            var hierarchicalTemplate = template as IHierarchicalTemplate;

            if (hierarchicalTemplate != null && hierarchicalTemplate.Children != null)
            {
                var hInstance = inst as IHierarchicalTemplateInstance;
                if (hInstance == null)
                {
                    throw new Exception($"Template of type {template.GetType().Name} is a hierarchical template, but the instance type does not implement {typeof(IHierarchicalTemplateInstance).Name}");
                }
                foreach (var tChild in hierarchicalTemplate.Children)
                {
                    hInstance.Add(tChild.Create());
                }
            }

            return inst;
        }
        
    }

}