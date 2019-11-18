using LionFire.Results;
using System.Threading.Tasks;

namespace LionFire.Resolves
{
    public interface IPuts<in T>
    {
        /// <summary>
        /// Set the current Value to value, and initiate a Put to the underlying data store with that value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<ISuccessResult> Put(T value);
    }
}
