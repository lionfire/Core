using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public interface ITemplate { } // RENAME: IGenerator

    public interface ITemplateEx
    {
        Type InstantiationType { get; }
    }

    public interface IInstantiatingTemplate
    {
        object Instantiate();
    }

    public interface ITemplate<T> : ITemplate, IForInstancesOf<T>
        //where T : new()
    {
    }

}