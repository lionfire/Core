using LionFire.DependencyInjection;
using LionFire.Referencing;
using LionFire.Referencing.Handles;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{

//#error NEXT: a global IHandleProvider should always be used to create handles from IReference, but maybe under the hood and only if IReference's implementation to get a handle is missing.
//#error But what about a general purpose Reference versus a strongly-typed Reference belonging to an OBus?  
//#error QUESTION: How to resolve References to OBus, References to OBase, References to a Handle that the OBase approves of???  (OBase may want to create that handle.)

//#error RH<T> IOBase.CreateHandle<T>(IReference reference)
//#error  RH<T> IOBase<MyKindOfReference>.TryCreateHandle<T>(MyKindOfReference reference)
//#error OBuses should implement IHandleProvider (and register them at app start) or forever hold their peace, in which case fallback general purpose handles will be created which then use IOBaseProvider.TryGetOBase.  Or should I say OBuses must implement IHandleProvider?  In that case, do I still need IOBaseProvider, unless specific OBases want to use it internally, but then it probably doesn't need to be an interface?

//#error Below is superceded
//#error NEXT: Think thru this.  IOBaseProviders provide OBases for References (for multiple IOBuses).
//#error Either IOBaseProviders for an IOBus either tells general purpose handles which IOBase to link up to, or else provides their own Read/Write Handles

    public interface IOBaseProvider
    {
        #region OBases

        //IEnumerable<IOBase> OBases { get; }
        //IOBase GetOBaseForConnectionString(string connectionString);
        IOBase TryGetOBase(IReference reference);

        //(IOBase OBase, RH<T> Replacement) TryGetOBase<T>(RH<T> readHandle);
        //(IOBase OBase, H<T> Replacement) TryGetOBase<T>(H<T> handle);

        //IReference TryGetReference(string referenceString);
        //IEnumerable<IOBase> GetOBases(IReference reference);

        #endregion
    }

    public interface IDefaultOBaseProvider
    {
        IOBase DefaultOBase { get; }
        IOBase SingleOBase { get; }
    }

    public interface IOBus : IOBaseProvider, IHandleProvider, IReadHandleProvider, ICollectionHandleProvider, IReferenceProvider, ICompatibleWithSome<IReference>
    {
        IServiceCollection AddServices(IServiceCollection sc);
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
