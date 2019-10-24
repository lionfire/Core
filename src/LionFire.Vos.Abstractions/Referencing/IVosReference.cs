using LionFire.ObjectBus;
using LionFire.Ontology;
using LionFire.Referencing;
using System;
using System.Collections.Generic;

namespace LionFire.Vos
{
    public interface IVosReference : IReference, IHas<IOBase>, IHas<IOBus>, ITypedReference
    {
        IEnumerable<string> AllowedSchemes { get; }
        //string Key { get; }
        string Location { get; set; }
        string Package { get; set; }
        //string Path { get; set; }
        //string Scheme { get; }
        //Type Type { get; set; }

        bool Equals(object obj);
        //VobHandle<object> GetHandle();
        //VobHandle<TValue> GetHandle<TValue>();
        int GetHashCode();
        //VobReadHandle<object> GetReadHandle();
        //VobReadHandle<TValue> GetReadHandle<TValue>();
    }
}