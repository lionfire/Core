using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Ontology
{
    public interface IParented
    {
        object Parent { get; set; }
    }
    public interface IParented<T>
    {
        T Parent { get; set; }
    }
}
