using System;
using System.Collections;
using LionFire.Ontology;
using LionFire.Structures;

namespace LionFire.Instantiating
{
    // REVIEW - make this interface more minimal and opt-in?

    public interface IInstantiationBase :
    IStateful
    , IEnumerable
    , IKeyed<string>
    , IHasPid
    , IParented
    , IHasTemplate
    , IHasRTemplate
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
}
