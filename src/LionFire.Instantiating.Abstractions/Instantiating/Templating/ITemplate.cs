using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public interface ITemplate { }

    public interface ITemplateEx
    {
        Type InstantiationType { get; }
    }

    // UNUSED. REVIEW - avoid this?
    public interface IInstantiatingTemplate // RENAME: IGenerator
    {
        object Instantiate();
    }

    public interface ITemplate<TInstance> : ITemplate, IForInstancesOf<TInstance>
    //where T : new()
    {
    }

    public interface ITemplate<TTemplate, TInstance, TParameters> : ITemplate, IForInstancesOf<TInstance>
        where TTemplate : ITemplate
        where TParameters : ITemplateParameters<TTemplate>
    {
    }
}