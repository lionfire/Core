using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public interface IHierarchicalTemplate : ITemplate
    {
        IInstantiationCollection Children { get; }
        //IEnumerable<IInstantiation> Children { get; 
            ////set;  // TOPORT if needed 
        //}
    }

    public interface IHierarchicalTemplate<T> : ITemplate<T>, IHierarchicalTemplate
        where T : IHierarchicalTemplateInstance, new()
    {
    }

    public static class IHierarchicalTemplateExtensions
    {
        public static IEnumerable<T> GetTemplatesOfType<T>(this ITemplate template, bool includeSelf = false)
            where T : ITemplate
        {
            if (includeSelf && template is T t) yield return t;

            if(template is IHierarchicalTemplate h && h.Children != null)
            {
                foreach(var result in h.Children.SelectMany(c=> c.Template.GetTemplatesOfType<T>(includeSelf: true)))
                {
                    yield return result;
                }
            }
        }
    }
}
