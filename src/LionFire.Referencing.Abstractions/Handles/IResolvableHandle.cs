using System.Threading.Tasks;

namespace LionFire
{
    // REFACTOR REVIEW - is there a benefit to having this separate from IReadHandle<T>?  Should the ObjectChanged be here too?
    // For now, this is sort of acting as a IReadHandle base that isn't dependant on generic vs object.  
    public interface IResolvableHandle
    {
        bool HasObject { get; }

        Task<bool> TryResolveObject(object persistenceContext = null);
        void ForgetObject();

        
    }
}
