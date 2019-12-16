using LionFire.Applications.Hosting;
using LionFire.Dependencies;
using LionFire.Validation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace LionFire.Instantiating
{
    public static class ITemplateExtensions
    {
        public static TInstance Create<TInstance>(this ITemplate<TInstance> template, bool inject = true)
            where TInstance : new()
        {
            if (template is IValidatesCreate validates) { validates.Validate().ValidateCreate().EnsureValid(); }

            return (TInstance)ITemplateExtensions.Create((ITemplate)template, typeof(TInstance), inject);
        }

        public static TInstance CreateWithoutValidate<TInstance>(this ITemplate<TInstance> template, bool inject = true)
            where TInstance : new()
            => (TInstance)ITemplateExtensions.Create((ITemplate)template, typeof(TInstance), inject);


        public static object Create(this ITemplate template, Type instanceType = null, bool inject = true)
        {
            if (instanceType == null)
            {
                var interfaceType = template.GetType().GetInterfaces().Where(t => t.Name == typeof(ITemplate).Name + "`1").FirstOrDefault();
                instanceType = interfaceType.GenericTypeArguments[0];
            }

            var inst = inject ? ActivatorUtilities.CreateInstance(DependencyContext.Current.ServiceProvider, instanceType) : Activator.CreateInstance(instanceType);

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

        public static IAppHost InstantiateTemplates(IAppHost appHost) // MOVE to ITemplateAppHostExtensions
        {
            foreach (var tComponent in appHost.Children.OfType<ITemplate>().ToArray())
            {
                var component = tComponent.Create();
                //appHost.Add(component);
                appHost.Replace(tComponent, component);
            }

            return appHost;
        }
    }

}