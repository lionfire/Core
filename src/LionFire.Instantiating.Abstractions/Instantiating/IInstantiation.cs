using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.Persistence;
using LionFire.Referencing;
using Microsoft.Extensions.Logging;

namespace LionFire.Instantiating
{
    public delegate void ActionObject(object o);

    //public interface IInstantiation<TChild> : IInstantiation
    //{
    //    IInstantiationCollection<TChild> Children { get; set; }
    //}
    public interface IInstantiation : IInstantiationBase // RENAME IHierarchicalInstantiation
            , ITemplateOverlayable // REVIEW - can this be removed?
    {
        IInstantiationCollection Children { get; set; }
    }

    //public interface IInstantiation<TTemplate> : IStateful
    //where TTemplate : ITemplate
    //{
    //    IReadHandleBase<TTemplate> Template { get; set; }
    //}
    public interface IInstantiation<TTemplate> : IInstantiation // RENAME IHierarchicalInstantiation
         where TTemplate : ITemplate
    {
        //IReadHandleBase<TTemplate> RTemplate { get;  }
    }
}
