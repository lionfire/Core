using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    public interface IOBaseProvider : IReferenceFactory
    {

        string UriSchemeDefault { get; }

        IOBase DefaultOBase { get; }

        IEnumerable<IOBase> OBases { get; }

        IOBase GetOBase(string connectionString);
        IEnumerable<IOBase> GetOBases(IReference reference);


        void Set(IReference reference, object obj);
    }

    //public abstract class OBaseProvider : IOBaseProvider
    ////where ReferenceType : IReference
    //{
    //    public abstract string[] UriSchemes { get; }

    //    public abstract IOBase GetOBase(string connectionString);

    //    public abstract string UriSchemeDefault
    //    {
    //        get;
    //    }

    //    public abstract IOBase DefaultOBase
    //    {
    //        get;
    //    }

    //    public abstract IEnumerable<IOBase> OBases
    //    {
    //        get;
    //    }

    //    public abstract IEnumerable<IOBase> GetOBases(IReference reference);


    //    public abstract IReference ToReference(string uri);
        
    //}
}
