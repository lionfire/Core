using System;
using System.Collections;
using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.Ontology;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Structures;
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

    public interface IInstantiationBase :
    IStateful
    , IEnumerable
    , IKeyed<string>
    , IHasPid
    , IParented
    , IHasTemplate
    {

        //AssetReference<ITemplate> Template { get; set; }
        //HAsset<ITemplate> Template { get; set; }

        new string Key { get; set; }

        ICloneable Prototype { get; set; }

        object TemplateObj { get; } // TEMP

        ITemplateParameters Parameters { get; set; }

#if AOT
		ActionObject InitializationMethod { get; set; }
#else
        Action<object> InitializationMethod { get; set; }
#endif
        bool HasChildren { get; }

    }

    public interface IInstantiation<TemplateType> : IStateful
    where TemplateType : ITemplate
    {
        IReadHandleBase<TemplateType> Template { get; set; }
    }
}
