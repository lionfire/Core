using LionFire.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
   /* public class TypeTemplate : ITemplate<object>, IInstantiatingTemplate
    {
        public Type ObjectType { get; set; }

        public virtual object Instantiate()
        {
            return Activator.CreateInstance(ObjectType);
        }
    }*/

    /// <summary>
    /// Instantiation Pipeline example:
    /// 
    /// Valor state save
    ///  - TEngine (template asset
    ///    - asset reference
    ///  - PEngine
    ///    - inline 
    ///  - State
    ///    - inline
    ///    
    /// Trading 
    ///   - TBot (template asset)
    ///   - Partial Parameters (asset)
    ///   - Partial Parameters2 (inline)
    ///   - on/off state
    ///   - stats state
    ///   - cache state (discardable)
    ///   
    /// 
    /// </summary>

    //public class TypestateHandle
    //{

    //    public object Object { get; set; }
    //    private object obj;
    //    public bool HasObject { get { return obj != null; } }

    //    public Type ObjectType { get; set; }

    //    public Type TemplateType { get; set; }
    //}

    //public class TemplateAssetReadHandle
    //{
    //    public object Object { get; protected set; }

    //    public string AssetSubPath { get; set; }
    //    //public Type
    //}
    
/*
    public class Instantiation
    {
        public Instantiation Parent { get; set; }

        public ITemplate Template { get; set; }

        public string TemplateSubPath { get; set; }
        public Type TemplateType { get; set; }
        public Dictionary<string, object> State { get; set; }
    }
    */
    /// <summary>
    /// Instantiation Stack:
    ///  - ConcreteType 
    ///    - State data
    ///    - Parent: Parameter
    /// </summary>

}
