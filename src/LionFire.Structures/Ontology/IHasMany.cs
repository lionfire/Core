using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Ontology 
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasMany<T>
    {
        IEnumerable<T> Objects { get; }
    }

}
