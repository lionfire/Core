using System;
using System.Threading.Tasks;

namespace LionFire
{
    /// <summary>
    /// Used as the base for the IReadHandle interfaces.  (In the future, it perhaps could also be used for Write only handles)
    /// </summary>
    public interface IResolvableHandle
    {
        bool HasObject { get; }
        bool IsResolved { get; }
         event Action<bool> IsResolvedChanged;

        // REVIEW: Is persistenceContext helpful?
        Task<bool> TryResolveObject();
        //Task<bool> TryResolveObject(bool forgetOnFail = false);
        //Task<bool> TryResolveObject(object persistenceContext = null, bool forgetOnFail = false);
        void ForgetObject();
    }
}
