//using LionFire.DependencyInjection;
//using LionFire.Referencing;
//using System;
//using System.Collections.Generic;

//namespace LionFire.ObjectBus
//{
//    public interface IReferenceProvider
//        : ICompatibleWithSome<string>
//    {
//        IEnumerable<string> UriSchemes { get; }
//        IEnumerable<Type> ReferenceTypes { get; } // REVIEW
//        bool IsValid(IReference reference);

//        IReference TryGetReference(string uriString);
//    }

//    //public abstract class OBaseProvider : IOBaseProvider
//    ////where ReferenceType : IReference
//    //{
//    //    public abstract string[] UriSchemes { get; }

//    //    public abstract IOBase GetOBase(string connectionString);

//    //    public abstract string UriSchemeDefault
//    //    {
//    //        get;
//    //    }

//    //    public abstract IOBase DefaultOBase
//    //    {
//    //        get;
//    //    }

//    //    public abstract IEnumerable<IOBase> OBases
//    //    {
//    //        get;
//    //    }

//    //    public abstract IEnumerable<IOBase> GetOBases(IReference reference);


//    //    public abstract IReference ToReference(string uri);
        
//    //}
//}
