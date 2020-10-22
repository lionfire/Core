#if NAppUpdater
using AppUpdate;
using AppUpdate.Tasks;
using AppUpdate.Common;
#endif

using System.Threading.Tasks;

namespace LionFire.Applications.Updates
{
    public interface IUpdater
    {
        Task<int> UpdatesAvailable { get; }
    }
}
