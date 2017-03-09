using LionFire.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using LionFire.Instantiating.Templating;
using LionFire.Assets;

namespace LionFire.Instantiating
{
    public class InstantiatorsFromTemplateAndStateStrategy : IInstantiationProvider
    {
        public IInstantiator TryProvide(object instance, InstantiationContext context = null)
        {
            var pipe = new InstantiationPipeline();

            if (context == null)
            {
                context = new InstantiationContext(instance);
            }
            
            {
                var attr = instance.GetType().GetTypeInfo().GetCustomAttribute<InstantiatorTypeAttribute>();
                if (attr != null)
                {
                    var instantiator = (IInstantiator)Activator.CreateInstance(attr.Type, instance);
                    pipe.Add(instantiator);
                }
            }

            var templateInstance = instance as ITemplateInstance;
            if (templateInstance != null)
            {
                if (templateInstance.Template != null)
                {
                    if (typeof(IAsset).IsAssignableFrom(templateInstance.Template.GetType()))
                    {
                        context.Dependencies.Add(templateInstance.Template);
                    }
                    else
                    {
                        pipe.Add(new TemplateRestorer(templateInstance.Template, templateInstance));
                    }
                }
            }

            {
                var attr = instance.GetType().GetTypeInfo().GetCustomAttribute<StateAttribute>();
                if (attr != null)
                {
                    pipe.Add(new StateRestorer(instance));
                }
            }

            if (pipe.Count > 0)
            {
                return pipe;
            }
            else
            {
                return null;
            }
        }
    }
}
