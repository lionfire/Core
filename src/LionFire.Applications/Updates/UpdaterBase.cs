#if NAppUpdater
using AppUpdate;
using AppUpdate.Tasks;
using AppUpdate.Common;
#endif

using System.Threading.Tasks;

namespace LionFire.Applications.Updates
{
    public abstract class UpdaterBase : IUpdater
    {
        public abstract Task<int> UpdatesAvailable { get; }

    }
}
