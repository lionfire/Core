using LionFire.ObjectBus;
using LionFire.Ontology;
using LionFire.Referencing;
using System;
using System.Collections.Generic;

namespace LionFire.Vos
{
    public interface IVosReference : IReference
        //, IHas<IOBase>
        //, IHas<IOBus>
        , ITypedReference
    {
        IEnumerable<string> AllowedSchemes { get; }
        //string Key { get; }

        #region REVIEW - maybe eliminate these, or eliminate Package and replace Location with MountName

        string Location { get; set; }
        string Package { get; set; }

        #endregion

        //string Path { get; set; }
        //string Scheme { get; }
        //Type Type { get; set; }

        bool Equals(object obj);
        //VobHandle<object> GetHandle();
        //VobHandle<T> GetHandle<T>();
        int GetHashCode();
        //VobReadHandle<object> GetReadHandle();
        //VobReadHandle<T> GetReadHandle<T>();
    }
}