using LionFire.DependencyInjection;
using LionFire.Referencing;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    public interface IOBaseProvider
    {
        #region OBases

        //IEnumerable<IOBase> OBases { get; }
        //IOBase GetOBaseForConnectionString(string connectionString);
        IOBase TryGetOBase(IReference reference);
        //IReference TryGetReference(string referenceString);
        //IEnumerable<IOBase> GetOBases(IReference reference);

        #endregion
    }

    public interface IDefaultOBaseProvider
    {
        IOBase DefaultOBase { get; }
        IOBase SingleOBase { get; }
    }

    public interface IOBus : IOBaseProvider, IHandleProvider, ICollectionHandleProvider, IReferenceProvider, ICompatibleWithSome<IReference>
    {
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
