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
    public interface IInstantiation : IInstantiationBase
            , ITemplateOverlayable

    {
        IInstantiationCollection Children { get; set; }
    }

    public interface IInstantiation<TemplateType> : IStateful
    where TemplateType : ITemplate
    {
        IReadHandleBase<TemplateType> Template { get; set; }
    }
}
