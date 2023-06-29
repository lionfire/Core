using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Data.Gets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Data.Sets;

namespace LionFire.Persistence;

public interface IHandlePersistence : ISets
{
    void AssignFrom(object other);

    bool QueryExistance();

    //bool RetrieveOnDemand { get; set; }
    //void Retrieve();

#if LEGACY
    // FUTURE: Do a weak reverse reference instead if this is needed?
    RetrieveInfo RetrieveInfo { get; set; }
    bool IsRetrieveInfoEnabled { get; set; }
#endif

    ///// <summary>
    ///// Tries to retrieve the object.  If not found, the current object will be set to no value if setToNullOnFail is true.
    ///// </summary>
    //bool TryRetrieve(bool setToNullOnFail = true); superceded by IReadHandle.TryResolveObject(object persistenceContext=null)

    void EnsureRetrieved();

    /// <summary>
    /// TODO: Change to async
    /// </summary>
    /// <param name="persistenceContext"></param>
    /// <returns></returns>
    bool TryEnsureRetrieved(object persistenceContext = null); 

    void RetrieveOrCreate();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns>True if retrieved, false if default is used</returns>
    bool RetrieveOrCreateDefault(object defaultValue);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns>True if retrieved, false if default is used</returns>
    bool RetrieveOrCreateDefault(Func<object> defaultValue);
    void ConstructDefault(bool applyDefaultValues = true);

    /// <summary>
    /// Sets Object to SpecialObject.NullObject.  When saved, this will delete the object at the specified reference.
    /// </summary>
    void Delete();
    bool TryDelete(bool preview = false);

    void Move(IReference newReference);
    void Copy(IReference newReference);

    bool? AutoSave { get; set; }


    bool IsPersisted { get; set; }
}

#if !AOT
public interface IHandlePersistence<T>
    where T : class//, new()
{
    bool RetrieveOrCreateDefault(T defaultValue);
    bool RetrieveOrCreateDefault(Func<T> defaultValue);
}
#endif
